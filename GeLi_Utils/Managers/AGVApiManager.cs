
using GeLi_Utils.Services.WMS;
using GeLiData_WMS;
using GeLiData_WMSImp;
using GeLiData_WMSUtils;
using GeLiService_WMS.Entity;
using GeLiService_WMS.Entity.AGVApiEntity;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Helper.WMS;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Threads;
using GeLiService_WMS.Threads.DiffFloorThreads;
using GeLiService_WMS.Utils.RabbitMQ;
using GeLiService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Managers
{
    /// <summary>
    /// AGV接口各种返回值的分解
    /// 1、任务状态回写
    /// 2、设备状态上传
    /// 3、任务中请求客户系统获取任务点位
    /// 4、AGV警报上传
    /// </summary>
    public class AGVApiManager
    {
        AGVMissionService _missionService;
        AGVMissionFloorService _floorService;
        TrayStateService _trayStateService;
        WareLocationService _wareLocationService;
        DeviceStatesService _deviceStatesService;
        AGVAlarmLogService _alarmLogService;
        AGVRunModelService _runModelService;
        WareLocationLockHisService _wareLoactionLockHisService;
        StockRecordService _stockRecordService;

        RedisHelper redisHelper = new RedisHelper();
        ChooseTiShengJiHelper _chooseTiShengJiHelper = null;
        WareLocationTrayManager _wareLocationTrayNoManager = null;
        public AGVApiManager(AGVMissionService missionService, AGVMissionFloorService floorService,
            TrayStateService trayStateService, WareLocationService wareLocationService,
            DeviceStatesService deviceStatesService, AGVAlarmLogService alarmLogService,
            AGVRunModelService runModelService,WareLocationLockHisService wareLoactionLockHisService
            , TiShengJiInfoService tiShengJiInfoService, StockRecordService stockRecordService
             ,WareLocationTrayManager wareLocationTrayNoManager)
        {
            _missionService = missionService;
            _floorService = floorService;
            _trayStateService = trayStateService;
            _wareLocationService = wareLocationService;
            _deviceStatesService = deviceStatesService;
            _alarmLogService = alarmLogService;
            _runModelService = runModelService;
            _wareLoactionLockHisService=wareLoactionLockHisService;
            _stockRecordService=stockRecordService;
            _wareLocationTrayNoManager = wareLocationTrayNoManager;
            _chooseTiShengJiHelper = new ChooseTiShengJiHelper(tiShengJiInfoService);
            

        }

        /// <summary>
        /// 接收任务状态接口
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public RunResult<string> UpdateMissionStates(MissionState ms)
        {
            //两种模式：一种同楼层任务，一种跨楼层任务
            //同楼层直接更新AGVMission

            //跨楼层更新跨楼层任务、最后更新AGVMission

            //第一步：判断是否跨楼层任务
            //         (其他线程修改)     (分类线程)    (执行线程)     (执行线程)     (执行线程从子线程更新)
            //总任务 ： SendState   空--》 已分类  --》  步骤一   --》  步骤二   --》  成功(发送失败)
            //          RunState    空--》 空      --》  空       --》  空       --》  已完成(已取消、执行失败)
            //         (接口更新)                                                     (接口更新)

            //                           (执行线程)
            //分任务 :  SendState  空 --》成功（发送失败）
            //          RunState   空 --》已下发 --》运行中 --》等待确认 --》已完成(已取消、执行失败、发送失败)
            //         (接口更新)        (接口更新....................)
            string missionNo = ms.taskId;
            Logger.Default.Process(new Log(LevelType.Info, $"接收来自{missionNo}的回发，到达的点位为{ms.targetPoint},AGV编号为{ms.agvNo}"));

            string topMissionNo = missionNo.Split('-')[0];
            AGVMissionInfo agvMission = _missionService.GetIQueryable(u => u.MissionNo == topMissionNo).FirstOrDefault();//拿到总任务
            AGVMissionBase aGVMissionBase = new AGVMissionBase();
            if(missionNo.Contains('-')) // 表示分任务
            {
                AGVMissionInfo_Floor agvMission_floor = _floorService.GetIQueryable(u => u.MissionNo == missionNo).FirstOrDefault();
                aGVMissionBase.StartLocation = agvMission_floor.StartLocation;
                aGVMissionBase.EndLocation= agvMission_floor.EndLocation;

            }
            else//表示总任务
            {
              
                aGVMissionBase.StartLocation = agvMission.StartLocation;
                aGVMissionBase.EndLocation = agvMission.EndLocation;
            }



           
            string runState = GetAGVState(ms, aGVMissionBase);
            string deviceID = string.Empty;
            if (!string.IsNullOrEmpty(ms.agvNo.ToString()))
                deviceID = ms.agvNo.ToString();
            string trayNo = string.Empty;
            bool ret = true;

         
            if (missionNo.Contains('-'))
            {
                if (missionNo.EndsWith(DiffFloorFactory.oneStr))
                    ret = false;//表示步骤一
                //更新分任务
               
                if(runState == StockState.RunState_Success) //执行成功
                {
                    _floorService.UpdateByPlus(u => u.MissionNo == ms.taskId,
                     u => new AGVMissionInfo_Floor { RunState = runState,StateTime=DateTime.Now, AGVCarId = deviceID });
                }
                else
                {
                    //目前只有已取货
                    _floorService.UpdateByPlus(u => u.MissionNo == ms.taskId,
                    u => new AGVMissionInfo_Floor { RunState = runState, AGVCarId = deviceID });
                }
                missionNo = missionNo.Split('-')[0];


            }
            //步骤一任务完成，不能修改总任务状态，步骤二才修改总任务总状态
         
            //ret=false 表示步骤一
            if ((runState == StockState.RunState_Success&& ret)
                    || runState == StockState.RunState_Error)
            {
                if(agvMission.Mark!=MissionType.MoveToMaPanJi)
                _missionService.UpdateByPlus(u => u.MissionNo == missionNo,
                   u => new AGVMissionInfo
                   {
                       RunState = runState,
                       SendState = StockState.SendState_Success,
                       AGVCarId = deviceID,
                       NodeTime = DateTime.Now,
                   });
                else //如果是移动到码盘机的操作
                    _missionService.UpdateByPlus(u => u.MissionNo == missionNo,
                      u => new AGVMissionInfo
                      {
                          RunState = StockState.RunState_AgvPutDownMaPan,
                          SendState = StockState.SendState_Success,
                          AGVCarId = deviceID,
                          NodeTime = DateTime.Now,
                      });
            }
            //else
            //{
            //    _missionService.UpdateByPlus(u => u.MissionNo == missionNo,
            //           u => new AGVMissionInfo { RunState = runState, NodeTime = DateTime.Now, AGVCarId = deviceID });
            //}
            //不是任务结束，后续仓位修改根本没有关系，直接返回完成给AGV系统
            if (runState != StockState.RunState_Success
               && runState != StockState.RunState_Cancel
                    && runState != StockState.RunState_RunFail
                    && runState != StockState.RunState_SendFail)
            {
                return RunResult<string>.True();
            }

            agvMission.RunState = runState;
            //完成的话，要对托盘的库位进行操作
            //步骤一完成时只对起点仓位进行操作
            //步骤二完成时只对终点仓位进行操作
            //同楼层完成时起点终点仓位进行操作

            //仓位操作：TrayState表+WareLocation表+WareLock表
            
            if (!ms.taskId.Contains('-')) // 如果是同楼层任务
            {
                TrayState trayState = _trayStateService.GetByTrayNo(agvMission.TrayNo,
                  false, DbMainSlave.Master); //从条码表获取条码
                WareLocation startWl = _wareLocationService.GetByAGVPo(agvMission.StartLocation
                    , false, DbMainSlave.Master);
                WareLocation endWl = _wareLocationService.GetByAGVPo(agvMission.EndLocation
                    , false, DbMainSlave.Master); //拿到结束库位
               

                if (runState == StockState.RunState_Success)//任务成功
                {
                    if(trayState==null)//表示空托
                    {
                        if (agvMission.Mark==MissionType.MoveToMaPanJi)
                            _wareLocationTrayNoManager.ChangeEmptyWarelocationInMaPanJi(startWl);
                        else if(agvMission.Mark==MissionType.MoveOutMaPanJi)
                            _wareLocationTrayNoManager.ChangeEmptyWarelocationOutMaPanJi(endWl);
                        else
                        _wareLocationTrayNoManager.ChangeEmptyWarelocation(startWl,endWl);

                    }
                    else//表示非空托
                    {
                        WareLocation oldWl = _wareLocationService.GetIQueryable(
                   u => u.TrayState_ID == trayState.ID, false,
                   DbMainSlave.Master).FirstOrDefault(); //拿到原先条码绑定的库位

                        if (agvMission.Mark == MissionType.GoodOnline)//如果是物料上线
                        {
                            _wareLocationTrayNoManager.ChangeTrayWareLocation(1, oldWl, trayState);//解绑缓存区库位的条码
                            _wareLocationTrayNoManager.OccupyEmptyWarelocation(endWl);//占用现在的位置

                        }
                        else if (agvMission.Mark == MissionType.GoodOfflineInHuanCun)//物料下线到缓存区
                        {
                            _wareLocationTrayNoManager.ChangeTrayWareLocation(1, oldWl, trayState);//为旧的位置解绑货物
                            _wareLocationTrayNoManager.ChangeTrayWareLocation(0, endWl, trayState);//为新位置绑定货物
                        }
                        else if (agvMission.Mark == MissionType.GoodOfflineInChanXian)//物料直接下到产线
                        {
                            _wareLocationTrayNoManager.ChangeTrayWareLocation(1, startWl, trayState);//为旧位置解绑货物
                            _wareLocationTrayNoManager.OccupyEmptyWarelocation(endWl); //终点绑定空托
                        }
                    }
                        //起点终点仓位进行操作
                        //_wareLocationTrayNoManager.ChangeTrayWareLocation(1, oldWl, trayState); // 改变库位的状态，0绑定，1解绑
                    //if (endWl != null &&
                    //    endWl.WareArea.WareAreaClass.AreaClass != AreaClassType.ChuKuArea)
                    //{
                    //    _wareLocationTrayNoManager.ChangeTrayWareLocation(0, endWl, trayState);//为新位置绑定货物
                    //    _stockRecordService.AddStockRecord(agvMission, trayState, 
                    //        oldWl!=null?oldWl.WareLocaNo:String.Empty);
                    //}
                }
                else if (runState == StockState.RunState_Cancel //取消任务
                    || runState == StockState.RunState_RunFail  //运行失败
                    || runState == StockState.RunState_SendFail) //发送失败
                {
                   
                }
            }
            else if(ms.taskId.EndsWith(DiffFloorFactory.oneStr))//一楼区域
            {
                AGVMissionInfo_Floor aGVMissionInfo_Floor = _floorService.FirstOrDefault(
                    u=>u.MissionNo== ms.taskId,true);
                TrayState trayState = _trayStateService.GetByTrayNo(agvMission.TrayNo,
                  false, DbMainSlave.Master);
                WareLocation oldWl = _wareLocationService.GetIQueryable(
                    u => u.TrayState_ID == trayState.ID, false,
                    DbMainSlave.Master).FirstOrDefault();
                if (runState == StockState.RunState_Success)
                {
                    //起点仓位进行操作
                    _wareLocationTrayNoManager.ChangeTrayWareLocation(1, oldWl, trayState);
                }
                else if (runState == StockState.RunState_Cancel
                    || runState == StockState.RunState_RunFail
                    || runState == StockState.RunState_SendFail)
                {
                    ret = true;
                    WareLocation endWl = _wareLocationService.GetByAGVPo(agvMission.EndLocation
                    , false, DbMainSlave.Master);
                    _wareLocationTrayNoManager.ChangeTrayWareLocation(1, endWl, trayState);
                    if (aGVMissionInfo_Floor!=null && aGVMissionInfo_Floor.StateMsg == "已搬起")
                    {
                        _wareLocationTrayNoManager.ChangeTrayWareLocation(1, oldWl, trayState);
                        _stockRecordService.AddStockRecord(agvMission, trayState,
                            oldWl != null ? oldWl.WareLocaNo : String.Empty);
                    }
                    else
                        _wareLocationTrayNoManager.ChangeTrayWareLocation(0, oldWl, trayState);
                   
                }
                //保存所需的信息调度需要的AGV接口并留存到数据库中
                //2.

            }
            else if (ms.taskId.EndsWith(DiffFloorFactory.twoStr)) //二楼任务不同与一楼任务，需要保存二楼状态，并更新最新的AGVMissionInfo表为已完成，以及调整AGVMissionInfo_Floor跨楼层任务为已完成，
            {
                AGVMissionInfo_Floor aGVMissionInfo_Floor = _floorService.FirstOrDefault(
                    u => u.MissionNo == ms.taskId, true);
                TrayState trayState = _trayStateService.GetByTrayNo(agvMission.TrayNo,
                  false, DbMainSlave.Master);
                WareLocation endWl = _wareLocationService.GetByAGVPo(agvMission.EndLocation
                     , false, DbMainSlave.Master);
                if (runState == StockState.RunState_Success)
                {
                    _stockRecordService.AddStockRecord(agvMission, trayState,
                            trayState.WareLocation != null ? trayState.WareLocation.WareLocaNo : string.Empty);
                    //终点仓位进行操作
                    if (endWl!=null &&
                        endWl.WareArea.WareAreaClass.AreaClass!=AreaClassType.ChuKuArea)
                        _wareLocationTrayNoManager.ChangeTrayWareLocation(0, endWl, trayState);
                    
                    _chooseTiShengJiHelper.RemoveMissionInTSJ(agvMission.WHName, agvMission.MissionNo);

                }
                else if (runState == StockState.RunState_Cancel
                    || runState == StockState.RunState_RunFail
                    || runState == StockState.RunState_SendFail)
                {
                    //有负载，直接仓位与托盘解绑
                    if (aGVMissionInfo_Floor == null || aGVMissionInfo_Floor.StateMsg == "已搬起")
                    {
                        _stockRecordService.AddStockRecord(agvMission, trayState,
                            trayState.WareLocation!=null ?trayState.WareLocation.WareLocaNo:string.Empty);
                        _wareLocationTrayNoManager.ChangeTrayWareLocation(1, endWl, trayState);
                    }
                    else
                    {
                        Logger.Default.Process(new Log(LevelType.Warn, $"开始停止提升机任务{aGVMissionInfo_Floor.TSJ_Name}:原因：{ms.taskId}任务"));
                        //没有负载，停止提升机，所有阶段二已运行但是未负载的任务都取消
                        redisHelper.StringSet($"IsFloorTaskStop:{aGVMissionInfo_Floor.TSJ_Name}", 1,null,"BackService");
                        
                    }

                    _chooseTiShengJiHelper.RemoveMissionInTSJ(agvMission.WHName, agvMission.MissionNo);
                }
            }
            //ret=true 表示步骤二结束或总任务结束
            //前面已经将未完结的状态进行了拦截，到这里的状态都是完结状态
            //if (ret)//完成
            //{
            //    string[] strArr = agvMission.EndPosition.Split('-');
            //    string lieName = $"{strArr[0]}-{strArr[1]}-";

            //    string[] strArr2 = agvMission.StartPosition.Split('-');
            //    string lieName2 = $"{strArr2[0]}-{strArr2[1]}-";
            //    RabbitMQUtils.Send(WareLieStateThread.queueName, lieName);
            //    RabbitMQUtils.Send(WareLieStateThread.queueName, lieName2);
            //}
            return RunResult<string>.True();
        }

        private string GetAGVState(MissionState ms,AGVMissionBase aGVMissionInfo) //到达起点终点都会调用这个接口，并且状态都为1011
        {
            Logger.Default.Process(new Log(LevelType.Info, $"获取的起点为{aGVMissionInfo.StartLocation}，终点为{aGVMissionInfo.EndLocation}"));
            if (ms.targetPoint == aGVMissionInfo.StartLocation) //如果此时是报的起点
                return StockState.RunState_AgvGetGoods;
            if (ms.targetPoint == aGVMissionInfo.EndLocation) //如果此时是到达的终点
                return StockState.RunState_Success;
            else
                return StockState.RunState_RunFail;
        }

        string keyStart = "CarStates";
        /// <summary>
        /// 设备状态上传，然后保存到数据库
        /// </summary>
        /// <param name = "ds" ></ param >
        /// < returns ></ returns >
        public RunResult<string> UpdateCarStates(DeviceStates ds)
        {
            //存在Redis中，每次都判断是否改变并延长过期时间，然后有改变再更新数据库，20秒过期
            
            if (ds.data != null)
            {
                //DataTable dTable = _deviceStatesService.ClassToDataTable(typeof(DeviceStatesInfo));
                List<DeviceStatesInfo> list = new List<DeviceStatesInfo>(ds.data.Count);
                List<int> missionlist = new List<int>();
                List<int> floorlist = new List<int>();
                foreach (DeviceStatesData dsd in ds.data)
                {
                    DeviceStatesInfo dsi = new DeviceStatesInfo();
                    //Logger.Default.Process(new Log(LevelType.Info, dsd.ToString()));
                    dsi.deviceCode = dsd.deviceCode;
                    dsi.payLoad = dsd.payLoad??"0.0";

                    if (dsd.devicePostionRec != null &&
                        dsd.devicePostionRec.Length > 0)
                    {
                        dsi.devicePostionX = dsd.devicePostionRec[0].ToString();
                        dsi.devicePostionY = dsd.devicePostionRec[1].ToString();
                        dsi.devicePostionRec = dsd.devicePostionRec[0].ToString() + "," + dsd.devicePostionRec[1].ToString();
                    }
                    else
                    {
                        dsi.devicePostionX =string.Empty;
                        dsi.devicePostionY = string.Empty;
                        dsi.devicePostionRec = string.Empty;
                    }
                    dsi.devicePosition = dsd.devicePosition?? string.Empty; 
                    dsi.battery = dsd.battery ?? string.Empty;
                    dsi.deviceName = dsd.deviceName ?? string.Empty;
                    dsi.deviceStatusInt = dsd.deviceStatus;
                    dsi.recTime = DateTime.Now;
                    switch (dsi.deviceStatusInt)
                    {
                        case null:
                            dsi.deviceStatus = string.Empty;
                            break;
                        case 0:
                            dsi.deviceStatus = "离线";
                            break;
                        case 1:
                            dsi.deviceStatus = "空闲";
                            break;
                        case 2:
                            dsi.deviceStatus = "故障";
                            break;
                        case 3:
                            dsi.deviceStatus = "初始化中";
                            break;
                        case 4:
                            dsi.deviceStatus = "任务中";
                            break;
                        case 5:
                            dsi.deviceStatus = "充电";
                            break;
                        case 7:
                            dsi.deviceStatus = "升级中";
                            break;
                        default:
                            break;
                    }

                    DeviceStatesInfo dsi_Old = redisHelper.HashGet<DeviceStatesInfo>
                        (keyStart,dsi.deviceName);
                  
                    if (dsi_Old != null && IsEqual(dsi_Old, dsi))
                        redisHelper.HashSet(keyStart, dsi.deviceName, dsi, TimeSpan.FromSeconds(30));
                    else
                    {
                        list.Add(dsi);

                        //AGV状态有变化而且在任务中
                        //判断AGV是否在任务中
                        if(dsi.deviceStatus == "任务中" && dsd.payLoad == "1.0")
                        {
                            Logger.Default.Process(new Log(LevelType.Info,
                                $"{dsi.deviceName}已搬起货物"));
                            DateTime yesTime = DateTime.Now.AddDays(-1);
                            AGVMissionInfo aGVMission = _missionService.FirstOrDefault(
                            u => u.OrderTime>= yesTime && u.AGVCarId == dsi.deviceName 
                            && u.IsFloor == 0 && u.RunState == StockState.RunState_Running,
                            true,DbMainSlave.Master);
                            if (aGVMission != null)
                            {
                                missionlist.Add(aGVMission.ID);
                                Logger.Default.Process(new Log(LevelType.Info,
                                $"{aGVMission.MissionNo}:{dsi.deviceName}已搬起{aGVMission.TrayNo}"));
                            }
                            else
                            {
                                AGVMissionInfo_Floor floorMission = _floorService.FirstOrDefault(
                                    u => u.OrderTime >= yesTime && u.AGVCarId == dsi.deviceName
                                    && u.RunState == StockState.RunState_Running,
                                    true, DbMainSlave.Master);

                                if (floorMission!=null)
                                {
                                    floorlist.Add(floorMission.ID);
                                    Logger.Default.Process(new Log(LevelType.Info,
                                $"{floorMission.MissionNo}:{dsi.deviceName}已搬起{floorMission.TrayNo}"));
                                }
                            }
                        }
                    }
                }
                if (list.Count > 0)
                {
                    DataTable dTable = _deviceStatesService.ConvertToDataTable(list);
                    _deviceStatesService.SetDataTableToTable(dTable, "DeviceStatesInfo");
                    foreach(var temp in list)
                        redisHelper.HashSet(keyStart, temp.deviceName, temp, TimeSpan.FromSeconds(30));

                    if (missionlist.Count > 0)
                    {
                        int[] arr= missionlist.ToArray();
                        _missionService.UpdateByPlus(u=> arr.Contains(u.ID),
                            u=>new AGVMissionInfo {StateMsg = "已搬起" });
                        
                    }
                    if (floorlist.Count > 0)
                    {
                        int[] arr = floorlist.ToArray();
                        _floorService.UpdateByPlus(u => arr.Contains(u.ID),
                            u => new AGVMissionInfo_Floor { StateMsg = "已搬起" });
                    }
                }
            }
            return RunResult<string>.True();
        }
        string[] timeName = { "ID", "recTime" };
        private bool IsEqual<T>(T ts1, T ts2)
        {
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(typeof(T)))
            {
                if (!(pd.GetValue(ts1) ?? string.Empty).Equals(pd.GetValue(ts2) ?? string.Empty)
                    && !timeName.Contains(pd.Name))
                {
                    return false;
                }
            }
            return true;
        }


        public IQueryable<AGVAlarmLog> GetWarnInFor(DateTime time1,DateTime time2)
        {
          
            return   _alarmLogService.GetLogByTime(time1,time2);
        }

      

        public string GetAGVState(int stateCode)
        {
            string msg = string.Empty;
          
             if (stateCode == 1011)
            {
                msg = StockState.RunState_Success;
            }
            else 
            {
                msg = StockState.RunState_Error;
            }
            return msg;
        }

        private DateTime ConvertTime(string str)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(str);
                return dt;
            }
            catch
            {
                return DateTime.Now;
            }
        }

    }
}

using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoImp;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity;
using NanXingService_WMS.Entity.AGVApiEntity;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Helper.WMS;
using NanXingService_WMS.Services;
using NanXingService_WMS.Services.WMS;
using NanXingService_WMS.Services.WMS.AGV;
using NanXingService_WMS.Threads;
using NanXingService_WMS.Threads.DiffFloorThreads;
using NanXingService_WMS.Utils.RabbitMQ;
using NanXingService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Managers
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
            string missionNo = ms.orderId;
            string runState = GetAGVState(ms.status);
            string deviceID = string.Empty;
            if (!string.IsNullOrEmpty(ms.deviceNum))
                deviceID = ms.deviceNum;
            string trayNo = string.Empty;
            bool ret = true;
            if (missionNo.Contains('-'))
            {
                if (missionNo.EndsWith(DiffFloorFactory.oneStr))
                    ret = false;//表示步骤一
                //更新分任务
               
                if(runState == StockState.RunState_Success)
                {
                    _floorService.UpdateByPlus(u => u.MissionNo == ms.orderId,
                     u => new AGVMissionInfo_Floor { RunState = runState,StateTime=DateTime.Now, AGVCarId = deviceID });
                }
                else
                {
                    _floorService.UpdateByPlus(u => u.MissionNo == ms.orderId,
                    u => new AGVMissionInfo_Floor { RunState = runState, AGVCarId = deviceID });
                }
                missionNo = missionNo.Split('-')[0];


            }
            //步骤一任务完成，不能修改总任务状态，步骤二才修改总任务总状态
            AGVMissionInfo agvMission = _missionService.GetIQueryable(u => u.MissionNo == missionNo).FirstOrDefault();
            //ret=false 表示步骤一
            if ((runState == StockState.RunState_Success&& ret)
                    || runState == StockState.RunState_Cancel
                    || runState == StockState.RunState_RunFail
                    || runState == StockState.RunState_SendFail)
            {
                _missionService.UpdateByPlus(u => u.MissionNo == missionNo,
                   u => new AGVMissionInfo
                   {
                       RunState = runState,
                       SendState = StockState.SendState_Success,
                       AGVCarId = deviceID
                   });
            }
            else
            {
                _missionService.UpdateByPlus(u => u.MissionNo == missionNo,
                       u => new AGVMissionInfo { RunState = runState, AGVCarId = deviceID });
            }
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
            
            if (!ms.orderId.Contains('-'))
            {
                TrayState trayState = _trayStateService.GetByTrayNo(agvMission.TrayNo,
                  false, DbMainSlave.Master);
                WareLocation endWl = _wareLocationService.GetByAGVPo(agvMission.EndLocation
                    , false, DbMainSlave.Master);
                WareLocation oldWl = _wareLocationService.GetIQueryable(
                    u => u.TrayState_ID == trayState.ID, false,
                    DbMainSlave.Master).FirstOrDefault();
                if (runState == StockState.RunState_Success)
                {
                    //起点终点仓位进行操作
                    _wareLocationTrayNoManager.ChangeTrayWareLocation(1, oldWl, trayState);
                    if (endWl != null &&
                        endWl.WareArea.WareAreaClass.AreaClass != AreaClassType.ChuKuArea)
                    {
                        _wareLocationTrayNoManager.ChangeTrayWareLocation(0, endWl, trayState);
                        _stockRecordService.AddStockRecord(agvMission, trayState, 
                            oldWl!=null?oldWl.WareLocaNo:String.Empty);
                    }
                }
                else if (runState == StockState.RunState_Cancel
                    || runState == StockState.RunState_RunFail
                    || runState == StockState.RunState_SendFail)
                {
                    //判断AGVMissionInfo是否已搬起
                    _wareLocationTrayNoManager.ChangeTrayWareLocation(1, endWl, trayState);

                    if (agvMission.StateMsg== "已搬起")
                    {
                        _wareLocationTrayNoManager.ChangeTrayWareLocation(1, oldWl, trayState);//旧仓位解绑
                        _stockRecordService.AddStockRecord(agvMission, trayState,
                             oldWl != null ? oldWl.WareLocaNo : String.Empty);
                    }
                    else
                        _wareLocationTrayNoManager.ChangeTrayWareLocation(0, oldWl, trayState);
                }
            }
            else if(ms.orderId.EndsWith(DiffFloorFactory.oneStr))
            {
                AGVMissionInfo_Floor aGVMissionInfo_Floor = _floorService.FirstOrDefault(
                    u=>u.MissionNo== ms.orderId,true);
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
            }
            else if (ms.orderId.EndsWith(DiffFloorFactory.twoStr))
            {
                AGVMissionInfo_Floor aGVMissionInfo_Floor = _floorService.FirstOrDefault(
                    u => u.MissionNo == ms.orderId, true);
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
                        Logger.Default.Process(new Log(LevelType.Warn, $"开始停止提升机任务{aGVMissionInfo_Floor.TSJ_Name}:原因：{ms.orderId}任务"));
                        //没有负载，停止提升机，所有阶段二已运行但是未负载的任务都取消
                        redisHelper.StringSet($"IsFloorTaskStop:{aGVMissionInfo_Floor.TSJ_Name}", 1,null,"BackService");
                        
                    }

                    _chooseTiShengJiHelper.RemoveMissionInTSJ(agvMission.WHName, agvMission.MissionNo);
                }
            }
            //ret=true 表示步骤二结束或总任务结束
            //前面已经将未完结的状态进行了拦截，到这里的状态都是完结状态
            if (ret)
            {
                string[] strArr = agvMission.EndPosition.Split('-');
                string lieName = $"{strArr[0]}-{strArr[1]}-";

                string[] strArr2 = agvMission.StartPosition.Split('-');
                string lieName2 = $"{strArr2[0]}-{strArr2[1]}-";
                RabbitMQUtils.Send(WareLieStateThread.queueName, lieName);
                RabbitMQUtils.Send(WareLieStateThread.queueName, lieName2);
            }
            return RunResult<string>.True();
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

        /// <summary>
        /// 记录AGV警报 
        /// </summary>
        /// <param name="al"></param>
        /// <returns></returns>
        public int AddAGVAlarmlog(Alarmlog al)
        {
            Logger.Default.Process(new Log(LevelType.Info, al.ToString()));
            AGVAlarmLog aal = new AGVAlarmLog();
            if (al.deviceNum != null && al.deviceNum.Length > 0)
                aal.deviceNum = al.deviceNum;
            aal.alarmDesc = al.alarmDesc;
            aal.alarmType = al.alarmType;
            aal.areaId = al.areaId;
            aal.alarmReadFlag = al.alarmReadFlag;
            if (al.channelDeviceId != null && al.channelDeviceId.Length > 0)
                aal.channelDeviceId = al.channelDeviceId;
            aal.alarmSource = al.alarmSource;
            if (al.channelName != null && al.channelName.Length > 0)
                aal.channelName = al.channelName;

            aal.alarmDate = ConvertTime(al.alarmDate);
            if (al.deviceName != null && al.deviceName.Length > 0)
                aal.deviceName = al.deviceName;
            aal.recTime = DateTime.Now;

            Logger.Default.Process(new Log(LevelType.Info, aal.ToString()));

            _alarmLogService.Insert(aal);
            


            return _alarmLogService.SaveChanges();
        }

        public RunResult<string> GetMissionTarget(MissionTarget mt)
        {
            //1、判断任务类型
            //2、判断返回的是Start还是end
            //   start则返回该列第一个货物，end则返回该列第一个空/预进仓位
            if(mt==null)
                return RunResult<string>.False("参数不能为空");
            Logger.Default.Process(new Log(LevelType.Info, $"{mt.orderId}:请求位置"));
            string redisKey = $"ApiReturn:{mt.orderId}";
            if (redisHelper.KeyExists(redisKey))
            {
                string ret = redisHelper.StringGet(redisKey);
                Logger.Default.Process(new Log(LevelType.Info, $"{mt.orderId}:返回{ret}"));
                return RunResult<string>.True(ret);
            }
            string lieName = string.Empty;
            AGVMissionBase missionBase = null;
            if (mt.orderId.Contains("-"))
                missionBase = _floorService.FirstOrDefault(u => u.MissionNo == mt.orderId,
                    true, DbMainSlave.Master);
            else
                missionBase = _missionService.FirstOrDefault(u => u.MissionNo == mt.orderId,
                    true, DbMainSlave.Master);
            
            if (missionBase == null)
                return RunResult<string>.True(ConfigurationManager.AppSettings["ReturnWLNo"]);

            string middlePosition = string.Empty;
            //if (string.IsNullOrEmpty(missionBase.StartMiddlePosition))
            //    middlePosition = missionBase.EndMiddlePosition;
            //else
            //    middlePosition=missionBase.StartMiddlePosition;
            
            WareLocation wareLocation = null;
            using (redisHelper.CreateLock("ReturnApi:"+ lieName,TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(500)))
            {
                //取到锁之后第一时间去缓存先看看有没有数据

                if (redisHelper.KeyExists(redisKey))
                {
                    string ret = redisHelper.StringGet(redisKey);
                    Logger.Default.Process(new Log(LevelType.Info, $"{mt.orderId}:返回{ret}"));
                    return RunResult<string>.True(ret);
                }

                AGVRunModel runModel = _runModelService.FirstOrDefault(u => u.AGVModelCode == mt.modelProcessCode);
                if (runModel.ApiRuturnPath.ToLower() == "{end}")
                {
                    middlePosition = missionBase.EndMiddlePosition;
                }else if (runModel.ApiRuturnPath.ToLower() == "{start}")
                {
                    middlePosition = missionBase.StartMiddlePosition;
                }
                string[] arr = middlePosition.Split('-');
                lieName = $"{arr[0]}-{arr[1]}-";

                wareLocation = GetWareLocationInLie(lieName, runModel.ApiRuturnPath.ToLower());

                if (wareLocation == null)
                    return runModel.ApiRuturnPath.ToLower() == "{start}" ?
                        RunResult<string>.False($"{lieName}该列已空") : RunResult<string>.False($"{lieName}该列已满");


                //成功获取到仓位，需要替换任务原有的目标仓位或起点仓位，
                //包括AGVMissionInfo ,floorMission
                if (mt.orderId.Contains("-"))
                {
                    string MissionNo = mt.orderId.Split('-')[0];
                    if (runModel.ApiRuturnPath.ToLower() == "{start}")
                    {
                        if (mt.orderId.EndsWith(DiffFloorFactory.oneStr))
                        {
                            if (wareLocation.WareArea.WareAreaClass.AreaClass!= AreaClassType.RuKuArea)
                            {
                                string trayNo = wareLocation.TrayState.TrayNO;
                                _missionService.UpdateByPlus(u => u.MissionNo == MissionNo,
                                  u => new AGVMissionInfo
                                  {
                                      StartLocation = wareLocation.AGVPosition,
                                      StartPosition = wareLocation.WareLocaNo,
                                      TrayNo = trayNo,
                                      //ApiReturnPoStr = wareLocation.AGVPosition
                                  });
                                string MissionNo_Two = MissionNo + DiffFloorFactory.twoStr;
                                _floorService.UpdateByPlus(u => u.MissionNo == MissionNo_Two,
                                    u => new AGVMissionInfo_Floor{  TrayNo = trayNo  });

                            }
                            else
                            {
                                _missionService.UpdateByPlus(u => u.MissionNo == MissionNo,
                                  u => new AGVMissionInfo
                                  {
                                      StartLocation = wareLocation.AGVPosition,
                                      StartPosition = wareLocation.WareLocaNo,
                                  });
                            }
                        }
                        if(wareLocation.WareArea.WareAreaClass.AreaClass != AreaClassType.RuKuArea)
                        {
                            string trayNo = wareLocation.TrayState.TrayNO;
                            _floorService.UpdateByPlus(u => u.MissionNo == mt.orderId,
                             u => new AGVMissionInfo_Floor
                             {
                                 StartLocation = wareLocation.AGVPosition,
                                 StartPosition = wareLocation.WareLocaNo,
                                 ApiReturnPoStr = wareLocation.AGVPosition,
                                 TrayNo = trayNo
                             });
                        }
                        else
                        {
                            _floorService.UpdateByPlus(u => u.MissionNo == mt.orderId,
                             u => new AGVMissionInfo_Floor
                             {
                                 StartLocation = wareLocation.AGVPosition,
                                 StartPosition = wareLocation.WareLocaNo,
                                 ApiReturnPoStr = wareLocation.AGVPosition,
                             });
                        }
                        
                    }
                    else if (runModel.ApiRuturnPath.ToLower() == "{end}")
                    {
                        if (mt.orderId.EndsWith(DiffFloorFactory.twoStr))
                        {
                            _missionService.UpdateByPlus(u => u.MissionNo == MissionNo,
                            u => new AGVMissionInfo
                            {
                                EndLocation = wareLocation.AGVPosition,
                                EndPosition = wareLocation.WareLocaNo,
                                //ApiReturnPoStr = wareLocation.AGVPosition
                            });
                        }
                        _floorService.UpdateByPlus(u => u.MissionNo == mt.orderId,
                              u => new AGVMissionInfo_Floor
                              {
                                  EndLocation = wareLocation.AGVPosition,
                                  EndPosition = wareLocation.WareLocaNo,
                                  ApiReturnPoStr = wareLocation.AGVPosition
                              });
                    }
                }
                else
                {
                    if (runModel.ApiRuturnPath.ToLower() == "{start}")
                    {
                        if(wareLocation.WareArea.WareAreaClass.AreaClass != AreaClassType.RuKuArea)
                        {
                            AGVMissionInfo info = _missionService.FirstOrDefault(u => u.MissionNo == mt.orderId
                            , false, DbMainSlave.Master);
                            info.StartLocation = wareLocation.AGVPosition;
                            info.StartPosition = wareLocation.WareLocaNo;
                            info.ApiReturnPoStr = wareLocation.AGVPosition;
                            info.TrayNo = wareLocation.TrayState.TrayNO;
                            _missionService.SaveChanges();

                            //_missionService.UpdateByPlus(u => u.MissionNo == mt.orderId,
                            //u => new AGVMissionInfo
                            //{
                            //    StartLocation = wareLocation.AGVPosition,
                            //    StartPosition = wareLocation.WareLocaNo,
                            //    ApiReturnPoStr = wareLocation.AGVPosition,
                            //    TrayNo = wareLocation.TrayState.TrayNO
                            //});
                        }
                        else
                        {
                            _missionService.UpdateByPlus(u => u.MissionNo == mt.orderId,
                            u => new AGVMissionInfo
                            {
                                StartLocation = wareLocation.AGVPosition,
                                StartPosition = wareLocation.WareLocaNo,
                                ApiReturnPoStr = wareLocation.AGVPosition
                            });
                        }
                    }
                    else if (runModel.ApiRuturnPath.ToLower() == "{end}")
                    {
                        _missionService.UpdateByPlus(u => u.MissionNo == mt.orderId,
                          u => new AGVMissionInfo
                          {
                              EndLocation = wareLocation.AGVPosition,
                              EndPosition = wareLocation.WareLocaNo,
                              ApiReturnPoStr = wareLocation.AGVPosition
                          });
                    }
                }
                string state = runModel.ApiRuturnPath.ToLower() == "{start}" ?
                   WareLocaState.NoTray : WareLocaState.HasTray;
                if (wareLocation.WareArea.WareAreaClass.AreaClass != AreaClassType.RuKuArea
                    || wareLocation.WareArea.WareAreaClass.AreaClass != AreaClassType.ChuKuArea)
                    _wareLocationService.UpdateByPlus(u => u.ID == wareLocation.ID,
                        u => new WareLocation { WareLocaState = state });
            }
            Logger.Default.Process(new Log(LevelType.Info, $"{mt.orderId}:返回{wareLocation.WareLocaNo}"));
            redisHelper.StringSet($"ApiReturn:{mt.orderId}", wareLocation.AGVPosition, TimeSpan.FromMinutes(5));
            return RunResult<string>.True(wareLocation.AGVPosition);
        }

        public WareLocation GetWareLocationInLie(string lieName,string returnType)
        {
            Logger.Default.Process(new Log(LevelType.Info, $"请求列号:{lieName};返回类型:{returnType}"));
            var expression=DbBaseExpand.True<WareLocation>();
            expression = expression.And(u => u.WareLoca_Lie == lieName
            && u.WareArea.WareAreaClass.AreaClass != "等待区");
            if(returnType=="{start}")
                expression = expression.And(u =>(u.WareLocaState == WareLocaState.HasTray
                      || u.WareLocaState == WareLocaState.PreOut
                      || u.WareArea.WareAreaClass.AreaClass == AreaClassType.RuKuArea));
            else
                expression = expression.And(u => (u.WareLocaState == WareLocaState.NoTray
                     || u.WareLocaState == WareLocaState.PreIn
                     || u.WareArea.WareAreaClass.AreaClass == AreaClassType.ChuKuArea));
            List<WareLocation> list = _wareLocationService.GetList(expression, true, DbMainSlave.Master);
            if (list == null||list.Count==0)
                return null;
            WareLocation wareLocation = null;
            IEnumerable<WareLocation> q = null;
            if (list[0].WareArea.WareAreaClass.AreaClass == AreaClassType.RuKuArea
                || list[0].WareArea.WareAreaClass.AreaClass == AreaClassType.ChuKuArea)
            {
                q = list.AsEnumerable();
            }
            else if (returnType == "{start}")
            {
                //获取货物
                q = list.Where(u => u.TrayState_ID != null);
            }
            else if(returnType == "{end}")
            {
                //获取空仓位
                q = list.Where(u => u.TrayState_ID == null);
            } 
            if ((list[0].WareArea.InstockRule == "优先使用小的仓位号"&& returnType == "{start}")
                || (list[0].WareArea.InstockRule == "优先使用大的仓位号" && returnType == "{end}"))
                wareLocation = q.OrderByDescending(u => u.WareLoca_Index.Length)
                .ThenByDescending(u => u.WareLoca_Index).ToList().FirstOrDefault();
            else if ((list[0].WareArea.InstockRule == "优先使用小的仓位号" && returnType == "{end}")
                || (list[0].WareArea.InstockRule == "优先使用大的仓位号" && returnType == "{start}"))
                wareLocation = q.OrderBy(u => u.WareLoca_Index.Length)
                .ThenBy(u => u.WareLoca_Index).ToList().FirstOrDefault();


            return wareLocation;
        }

        public string GetAGVState(int stateCode)
        {
            string msg = string.Empty;
            if (stateCode == 9)
            {
                msg = StockState.RunState_HasSend;
            }
            else if (stateCode == 6)
            {
                msg = StockState.RunState_Running;
            }
            else if (stateCode == 7)
            {
                msg = StockState.RunState_RunFail;
            }
            else if (stateCode == 5)
            {
                msg = StockState.RunState_SendFail;
            }
            else if (stateCode == 3)
            {
                msg = StockState.RunState_Cancel;
            }
            else if (stateCode == 8)
            {
                msg = StockState.RunState_Success;
            }
            else if (stateCode == 10)
            {
                msg = StockState.RunState_WaitRun;
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

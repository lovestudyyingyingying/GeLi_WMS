using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Services;
using NanXingService_WMS.Services.WMS.AGV;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.AGVUtils;
using NanXingService_WMS.Utils.RabbitMQ;
using NanXingService_WMS.Utils.RedisUtils;
using NanXingService_WMS.Utils.ThreadUtils;
using NanXingService_WMS.Utils.TishengjiUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanXingService_WMS.Threads.DiffFloorThreads
{
    /// <summary>
    /// 跨楼层执行线程
    /// </summary>
    public class TiShengJiThread
    {
        TiShengJiInfo _tiShengJiInfo;
        public TiShengJiHelper tiShengJiHelper;
        public MyTask runTask;
        public MyTask stepTwofailTask;


        AGVOrderUtils agvOrderUtils ;
        ReckonSecondUtils reckonSecondUtils = new ReckonSecondUtils();

        AGVMissionFloorService floorService = new AGVMissionFloorService();
        AGVMissionService missionService = new AGVMissionService();
        TrayStateService trayStateService = new TrayStateService();
        TiShengJiRunRecordService tiShengJiRunRecordService = new TiShengJiRunRecordService();
        RabbitMQUtils rabbitMqUtils = new RabbitMQUtils();
        RedisHelper redisHelper = new RedisHelper();
        DateTime dtime = DateTime.Now;
        DateTime firstTime = DateTime.Now;
        string runningKey = string.Empty;
        bool stoped=false;
        //Expression<Func<AGVMissionInfo_Floor, bool>> run_expression;
        public TiShengJiThread(TiShengJiInfo tiShengJiInfo)
        {
            _tiShengJiInfo = tiShengJiInfo;
            runningKey=$"IsFloorTaskStop:{_tiShengJiInfo.TsjName}";
            agvOrderUtils = new AGVOrderUtils(tiShengJiInfo.AGVServerIP);
            //状态获取
            tiShengJiHelper = new TiShengJiHelper(tiShengJiInfo);
            runTask = new MyTask(new Action(RunControl), 2, true).StartTask();
            //stepTwofailTask = rabbitMqUtils.Recevice<int>(stepTwoErrorQueue, StepTwoFail);
        }

        
        public void RunControl()
        {
            try
            {
                if (redisHelper.StringGet<int>(runningKey) == 1)
                {
                    if (!stoped)
                    {
                        Logger.Default.Process(new Log(LevelType.Info,
                            $"{_tiShengJiInfo.TsjName}跨楼层线程已暂停"));
                        stoped = true;
                    }
                    //停止所有阶段二未负载的任务
                    //StepTwoFail();

                    Thread.Sleep(5000);
                    return;
                }
                if (stoped)
                {
                    Logger.Default.Process(new Log(LevelType.Info,
                           $"{_tiShengJiInfo.TsjName}跨楼层线程已启动"));
                    stoped = false;
                }
                dtime = DateTime.Now.AddDays(-1);
                
                //判断阶段二任务
                
                ContinueTask();
                //更新总任务状态:步骤一正常任务由执行流程处理
                //这里主要处理 步骤二任务、步骤一异常任务：已取消、执行失败、发送失败
                UpdateTask(dtime);

                //执行分任务
                FloorTask();
            }
            catch(Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                       $"DiffFloorRunThread:\r\n{ex.ToString()}"));
            }
            
        }
        /// <summary>
        /// 递归将所有运行中又未搬起的货物全部取消掉
        /// </summary>
        private void StepTwoFail()
        {
            //没有负载，停止线程，所有阶段二已运行但是未负载的任务都取消
            List<AGVMissionInfo_Floor> floorList = floorService.GetList(
                u =>u.TSJ_Name==_tiShengJiInfo.TsjName && u.MissionNo.EndsWith(DiffFloorFactory.twoStr)
                && u.StateMsg.Length==0 && (u.RunState==StockState.RunState_Running 
                || u.RunState==StockState.RunState_Emtpy|| u.RunState == StockState.RunState_HasSend 
                || u.RunState == StockState.RunState_WaitRun),
                true,DbMainSlave.Master) ;
           
            if (floorList.Count>0)
            {
                string[] orderIds = floorList.Select(u => u.MissionNo).ToArray();
                agvOrderUtils.CancelMission(orderIds);
            }
        }

        /// <summary>
        /// 等待确认的任务
        /// </summary>
        /// <param name="list"></param>
        private void ContinueTask()
        {
            //判断任务是步骤1还是步骤2
            //判断提升机是否出货就位,然后判断步骤
            //步骤1,则直接执行
            //步骤2,判断小车的任务条码是否提升机第一个搬出条码，
            //如果是，则直接执行
            //如果否，则将条码互换...

            //判断等待执行的任务
            if (tiShengJiHelper.state == null
                || tiShengJiHelper.state.deviceState != DeviceState.Normal)
                return;


            List<AGVMissionInfo_Floor> waitList = floorService.GetQuery(u =>
                  u.OrderTime >= dtime && u.TSJ_Name == _tiShengJiInfo.TsjName
                  && u.RunState == StockState.RunState_WaitRun, true, DbMainSlave.Master)
                .OrderBy(u => u.ID).ToList();
            foreach (AGVMissionInfo_Floor temp in waitList)
            {
                Logger.Default.Process(new Log(LevelType.Info,
                             $"DiffFloorRunThread:{_tiShengJiInfo.TsjName}提升机是否出货就位"));
                //如果任务已继续，则跳过
                if (temp.IsContinued == 1)
                {
                    continue;
                }
                //任务一判断提升机对接位没有货物
                if (temp.MissionNo.EndsWith(DiffFloorFactory.oneStr)
                    && ((temp.StartPosition.StartsWith("1") && tiShengJiHelper.state.F1DuiJieWei == GunZhouState.NoBox)
                || (temp.StartPosition.StartsWith("2") && tiShengJiHelper.state.F2DuiJieWei == GunZhouState.NoBox)
                || (temp.StartPosition.StartsWith("3") && tiShengJiHelper.state.F3DuiJieWei == GunZhouState.NoBox)))
                {
                    agvOrderUtils.ContinueTask(temp.MissionNo);
                }
                //任务二判断提升机是否出货就位
                else if (temp.MissionNo.EndsWith(DiffFloorFactory.twoStr) &&(
                    (temp.EndPosition.StartsWith("1") && tiShengJiHelper.state.F1DuiJieWei == GunZhouState.HasBox)
                || (temp.EndPosition.StartsWith("2") && tiShengJiHelper.state.F2DuiJieWei == GunZhouState.HasBox)
                || (temp.EndPosition.StartsWith("3") && tiShengJiHelper.state.F3DuiJieWei == GunZhouState.HasBox)))
                    {
                    //等待小车的任务条码是否提升机第一个搬出条码
                    //string prosn=RedisCacheHelper.Get<string>("Tsj_" + temp.StartPosition.Substring(0, 1) + "_Out1");
                    if (temp.TiShengJiRunRecord == null || temp.TiShengJiRunRecord.TrayCount == 1)
                    {
                        agvOrderUtils.ContinueTask(temp.MissionNo);
                    }
                    else
                    {
                        int count = floorService.GetCount(u =>
                            u.TiShengJiRecord_ID == temp.TiShengJiRecord_ID
                            && u.IsContinued == 1, true, DbMainSlave.Master);
                        //int count = temp.TiShengJiRunRecord.AGVMissionInfo_Floor.
                        //    Where(u => u.IsContinued == 1).Count();
                        string outTrayNo = string.Empty;
                        if (count == 0)
                            //等于0则是第一个任务，用最外面的条码
                            outTrayNo = temp.TiShengJiRunRecord.OutsideTrayNo;
                        else
                            //不等于0则是第二个任务，用最里面的条码
                            outTrayNo = temp.TiShengJiRunRecord.InsideTrayNo;
                        Logger.Default.Process(new Log(LevelType.Info,
                            $"DiffFloorRunThread:{_tiShengJiInfo.TsjName}获取条码{outTrayNo}"));

                        //条码相等则继续，不相等则调换条码后继续任务
                        if (outTrayNo == temp.TrayNo)
                        {
                            agvOrderUtils.ContinueTask(temp.MissionNo);
                            floorService.UpdateByPlus(u => u.ID == temp.ID,
                                u => new AGVMissionInfo_Floor { IsContinued = 1 });
                            Logger.Default.Process(new Log(LevelType.Info,
                            $"DiffFloorRunThread:{_tiShengJiInfo.TsjName}继续任务{temp.MissionNo}:{temp.TrayNo}"));
                        }
                        else
                        {
                            agvOrderUtils.ContinueTask(temp.MissionNo);
                            //调转条码
                            floorService.UpdateByPlus(u => u.ID == temp.ID,
                                u => new AGVMissionInfo_Floor { TrayNo = outTrayNo, IsContinued = 1 });
                            missionService.UpdateByPlus(u => u.ID == temp.MissionFloor_ID,
                               u => new AGVMissionInfo{ TrayNo = outTrayNo });
                            
                            Logger.Default.Process(new Log(LevelType.Info,
                                $"DiffFloorRunThread:{_tiShengJiInfo.TsjName}继续任务{temp.MissionNo}:{ outTrayNo }"));
                          
                        }
                    }
                }
            }


        }

        /// <summary>
        /// 更新总任务状态
        /// </summary>
        /// <param name="dTime"></param>
        private void UpdateTask(DateTime dTime)
        {
            //获取步骤一任务，而且为已取消、执行失败、发送失败
            //将步骤一任务的状态转为总任务状态与步骤二状态
            List<AGVMissionInfo_Floor> bzOneList=floorService.GetIQueryable(
                u => u.OrderTime > dTime && u.SendState == StockState.SendState_BzOne
                && u.AGVMissionInfo.RunState!=u.RunState
             &&(u.SendState== StockState.SendState_Fail || u.RunState== StockState.RunState_Cancel
             || u.RunState== StockState.RunState_RunFail|| u.RunState== StockState.RunState_SendFail)
              ).ToList();
            DataTable floor_dt = floorService.ClassToDataTable(typeof(AGVMissionInfo_Floor));
            DataTable mission_dt = missionService.ClassToDataTable(typeof(AGVMissionInfo));

            foreach (var temp in bzOneList)
            {
                temp.AGVMissionInfo.RunState = temp.RunState;
                AGVMissionInfo_Floor buzhou2 = temp.AGVMissionInfo.AGVMissionInfo_Floor
                    .Where(u => u.MissionNo.EndsWith(DiffFloorFactory.twoStr)).FirstOrDefault();
                buzhou2.RunState = temp.RunState;
                mission_dt = missionService.ParseInDataTable(mission_dt, temp.AGVMissionInfo);
                floor_dt = floorService.ParseInDataTable(floor_dt, buzhou2);
            }
            missionService.UpdateMany(mission_dt);
            floorService.UpdateMany(floor_dt);

            //步骤二任务所有完结状态与总任务不一致的
            List<AGVMissionInfo_Floor> bzTwoList = floorService.GetIQueryable(
                u => u.OrderTime > dTime && u.SendState == StockState.SendState_BzTwo
                && u.AGVMissionInfo.RunState != u.RunState
             && (u.SendState == StockState.SendState_Fail || u.RunState == StockState.RunState_Cancel
             || u.RunState == StockState.RunState_RunFail || u.RunState == StockState.RunState_SendFail
             || u.RunState == StockState.RunState_Success)
              ).ToList();
            mission_dt.Rows.Clear();
            foreach(var temp in bzTwoList)
            {
                temp.AGVMissionInfo.RunState = temp.RunState;
                mission_dt = missionService.ParseInDataTable(mission_dt, temp.AGVMissionInfo);
            }
            missionService.UpdateMany(mission_dt);
        }

        #region 跨楼层执行流程
        //情况一

        private void FloorTask()
        {
            //Logger.Default.Process(new Log(LevelType.Info,
            //           $"DiffFloorRunThread:开始跨楼层任务判断流程"));

            //判断提升机运行状态,不是空闲则退出任务
            if (tiShengJiHelper.state==null
                || tiShengJiHelper.state.deviceState != DeviceState.Normal
                || tiShengJiHelper.state.carState != TiShengCarState.NoJob)
                return;

            //1、获取所有未完成步骤1的跨楼层总任务
            List<AGVMissionInfo> noFinishList = missionService.GetIQueryable(
                u => u.OrderTime > dtime && u.WHName== _tiShengJiInfo.TsjName
                && (u.SendState== StockState.SendState_Group || u.SendState==StockState.SendState_BzOne)
                , true, DbMainSlave.Master)
                .ToList();
            //Logger.Default.Process(new Log(LevelType.Info,
            //            $"DiffFloorRunThread:未完成步骤1的跨楼层总任务数量{noFinishList.Count}"));

            //步骤1总任务的ID数组
            int[] buzhouOne_ids=noFinishList.Where(u => u.SendState == StockState.SendState_BzOne)
                .Select(u => u.ID).ToArray();
            //还没开始发送的 总任务的ID数组
            int[] noRun_ids = noFinishList.Where(u => u.SendState == StockState.SendState_Group)
                .Select(u => u.ID).ToArray();
            //Logger.Default.Process(new Log(LevelType.Info,
            //  $"DiffFloorRunThread:\r\n未完成步骤1的跨楼层总任务数量{noFinishList.Count}\r\n还没开始发送的任务数量{noRun_ids.Count()}"));
            if (buzhouOne_ids.Length == 0)
            {
                StepAZero(noRun_ids);
            }
            else if (buzhouOne_ids.Length == 1)
            {
                StepAOne(noRun_ids, buzhouOne_ids[0]);
            }
            else if (buzhouOne_ids.Length >= 2)
            {
                StepATwo(buzhouOne_ids);
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="noRun_ids">没有进入步骤一的总任务ID</param>
        private void StepAZero(int[] noRun_ids)
        {
            List<AGVMissionInfo_Floor> list = floorService.GetIQueryable(u=> 
            noRun_ids.Contains((int)u.MissionFloor_ID)
            && u.MissionNo.EndsWith(DiffFloorFactory.oneStr)
                , true, DbMainSlave.Master)
                     .OrderBy(u => u.ID).ToList();
            //判断数量>=2
            //判断前两条步骤1任务的批次、规格、起点楼层、终点楼层是否一致
            //如果一直  直接发送  两条
            //如果不一致直接发送第一条
            //判断数量==1`
            //直接发送第一条
            bool sendOne = true;
            if (list.Count >= 2
                && list[0].StartLocation.Substring(0, 1) == list[1].StartLocation.Substring(0, 1)
                && list[0].EndLocation.Substring(0, 1) == list[1].EndLocation.Substring(0, 1)
                && IsSameProType(list[0].TrayNo, list[1].TrayNo))
            {
                sendOne = false;
            }
            //有排队任务则发送，没有则跳出
            if (list.Count > 0)
            {
                if (sendOne)
                {
                    agvOrderUtils.SendFloorOrder(list[0]);
                    int MissionID = (int)list[0].MissionFloor_ID;
                    missionService.UpdateByPlus(u => u.ID == MissionID,
                        u => new AGVMissionInfo{ SendState = StockState.SendState_BzOne });
                    firstTime = DateTime.Now;
                }
                else
                {
                    agvOrderUtils.SendFloorOrder(list[0]);
                    agvOrderUtils.SendFloorOrder(list[1]);
                    int ID_1 = (int)list[0].MissionFloor_ID;
                    int ID_2 = (int)list[1].MissionFloor_ID;

                    missionService.UpdateByPlus(u => 
                    u.ID == ID_1 || u.ID == ID_2,
                        u => new AGVMissionInfo { SendState = StockState.SendState_BzOne });
                }
            }
        }

        private void StepAOne(int[] noRun_ids, int BuZhouOne_id)
        {
            AGVMissionInfo_Floor nextFloorMission = floorService.GetIQueryable(u =>
            noRun_ids.Contains((int)u.MissionFloor_ID) && u.MissionNo.EndsWith(DiffFloorFactory.oneStr)
                    , true, DbMainSlave.Master)
                     .OrderBy(u => u.ID).FirstOrDefault();
            AGVMissionInfo_Floor runningFloor = floorService.GetIQueryable(
                u => u.MissionFloor_ID == BuZhouOne_id && u.MissionNo.EndsWith(DiffFloorFactory.oneStr)
                , true,DbMainSlave.Master)
                .FirstOrDefault();

            //判断是否有下一个跨楼层任务
            if (nextFloorMission != null 
                && runningFloor.StartLocation.Substring(0, 1) == nextFloorMission.StartLocation.Substring(0, 1)
                && runningFloor.EndLocation.Substring(0, 1) == nextFloorMission.EndLocation.Substring(0, 1)
                && IsSameProType(runningFloor.TrayNo, nextFloorMission.TrayNo))
            {
                agvOrderUtils.SendFloorOrder(nextFloorMission);
                int ID_1 = (int)nextFloorMission.MissionFloor_ID;
                missionService.UpdateByPlus(u =>
                u.ID == ID_1,
                    u => new AGVMissionInfo { SendState = StockState.SendState_BzOne });
            }else if (
                runningFloor.RunState == StockState.RunState_Success&&
                //没有新任务并等待时间大于50秒
               ( (nextFloorMission == null && reckonSecondUtils.ReckonSeconds(firstTime, DateTime.Now) > 50)
                || //有新任务但目标楼层不一致
                 (nextFloorMission!=null && ! (runningFloor.StartLocation.Substring(0, 1) == nextFloorMission.StartLocation.Substring(0, 1)
                && runningFloor.EndLocation.Substring(0, 1) == nextFloorMission.EndLocation.Substring(0, 1)
                && IsSameProType(runningFloor.TrayNo, nextFloorMission.TrayNo)))))
            {
                //执行步骤二
                SendBuZhouSecond(new int[1] { BuZhouOne_id },
                    runningFloor.StartLocation.Substring(0, 1));
            }
        }
        
        private void StepATwo(int[] BuZhouOne_ids)
        {
            List<AGVMissionInfo_Floor> list=floorService.GetIQueryable(
                u => BuZhouOne_ids.Contains((int)u.MissionFloor_ID)
                &&u.MissionNo.EndsWith(DiffFloorFactory.oneStr),true,DbMainSlave.Master)
                .OrderBy(u => u.ID).ToList();
            Logger.Default.Process(new Log(LevelType.Info,
                       $"DiffFloorRunThread:{_tiShengJiInfo.TsjName}\r\n任务一{list[0].MissionNo}：{list[0].RunState}\r\n任务二{list[1].MissionNo}：{list[1].RunState}"));
            if ( list[0].RunState == StockState.RunState_Success 
                && list[1].RunState == StockState.RunState_Success)
            {
                SendBuZhouSecond(BuZhouOne_ids, list[0].StartLocation.Substring(0, 1));
            }

        }

        /// <summary>
        /// 发送步骤二任务，并修改总任务
        /// </summary>
        /// <param name="missionId">总任务ID</param>
        /// <param name="startFloor">起始楼层</param>
        private void SendBuZhouSecond(int[] missionId,string startFloor)
        {
            var floor_q = floorService.GetIQueryable(
                    u => missionId.Contains((int)u.MissionFloor_ID)
                    , true, DbMainSlave.Master).OrderBy(u=>u.ID);

            List<AGVMissionInfo_Floor> list_BuZhouOne =
               floor_q.Where(u => u.MissionNo.EndsWith(DiffFloorFactory.oneStr)).
               OrderBy(u=>u.StateTime).ToList();

            List<AGVMissionInfo_Floor> list=
                floor_q.Where(u => u.MissionNo.EndsWith(DiffFloorFactory.twoStr)).ToList();
            string endFloor = list[0].EndLocation.Substring(0, 1);
            
            //发送提升机任务
            AddRecord(list_BuZhouOne, list, startFloor, endFloor);
            tiShengJiHelper.SendTSJOrder(startFloor, endFloor, list.Count);
            //将步骤二任务发送出去
            list.ForEach(item =>
            {
                agvOrderUtils.SendFloorOrder(item);
                int ID_1 = (int)item.MissionFloor_ID;
                missionService.UpdateByPlus(u => u.ID== ID_1,
                u => new AGVMissionInfo { SendState = StockState.SendState_BzTwo });
                Thread.Sleep(500);
            });
            
        }

        

        private void AddRecord(List<AGVMissionInfo_Floor> list_BuzhouOne
            , List<AGVMissionInfo_Floor> list_BuzhouTwo,
            string startFloor,string endFloor)
        {
            //将已完成的任务一按时间排序
            string line = list_BuzhouOne.Count > 1 ? list_BuzhouOne[1].MissionNo.Replace("-1", "-2") + ":" 
                + list_BuzhouOne[1].TrayNo : string.Empty;
            Logger.Default.Process(new Log(LevelType.Info,
            $"{_tiShengJiInfo.TsjName}\r\n任务一:{list_BuzhouOne[0].MissionNo.Replace("-1","-2")}:{list_BuzhouOne[0].TrayNo}\r\n任务二:{line}"));

            TiShengJiRunRecord record = new TiShengJiRunRecord();
            record.TsjName = _tiShengJiInfo.TsjName;
            record.TsjIp = _tiShengJiInfo.TsjIp;
            record.TsjPort = _tiShengJiInfo.TsjPort;
            record.OrderTime = DateTime.Now;
            record.StartFloor = startFloor;
            record.EndFloor = endFloor;
            record.TrayCount = list_BuzhouOne.Count;
            record.InsideTrayNo = list_BuzhouOne[0].TrayNo;
            record.OutsideTrayNo = list_BuzhouOne.Count == 2 ? list_BuzhouOne[1].TrayNo : string.Empty;
            string str= list_BuzhouOne[0].MissionNo.Split('-')[0] + "-2"+ 
                (list_BuzhouOne.Count == 2 ? "," + list_BuzhouOne[1].MissionNo.Split('-')[0] + "-2" : string.Empty);

            tiShengJiRunRecordService.Insert(record);
            tiShengJiRunRecordService.SaveChanges();
            //将排序后的记录关联阶段2任务
            int[] ids = list_BuzhouTwo.Select(u => u.ID).ToArray();
            floorService.UpdateByPlus(u => ids.Contains(u.ID),
                u => new AGVMissionInfo_Floor { TiShengJiRecord_ID = record.ID });
            string idsStr = string.Join(",", ids);
            Logger.Default.Process(new Log(LevelType.Info,
            $"绑定跨楼层条码顺序:{_tiShengJiInfo.TsjName}\r\n任务:{idsStr}\r\n条码顺序{record.ID}:{ record.InsideTrayNo }:{record.OutsideTrayNo}"));
        }

        private bool IsSameProType(string trayNo1,string trayNo2)
        {
            TrayState trayState1 = trayStateService.GetByTrayNo(trayNo1);
            TrayState trayState2 = trayStateService.GetByTrayNo(trayNo2);
            if (trayState1 == null || trayState2 == null)
                return false;
            if (trayState1.batchNo != trayState2.batchNo)
                return false;
            if (trayState1.proname != trayState2.proname)
                return false;
            if (trayState1.spec != trayState2.spec)
                return false;
            if (trayState1.probiaozhun != trayState2.probiaozhun)
                return false;
            return true;
        }

        #endregion 

    }
}

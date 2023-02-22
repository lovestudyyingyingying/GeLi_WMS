
using GeLi_Utils.Entity.TiShengJiEntity;
using GeLi_Utils.Helpers;
using GeLi_Utils.Utils.AGVUtils;
using GeLiData_WMS;
using GeLiData_WMSUtils;
using GeLiService_WMS.Entity.AGVOrderEntity;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Utils;

using GeLiService_WMS.Utils.RabbitMQ;
using GeLiService_WMS.Utils.RedisUtils;
using GeLiService_WMS.Utils.ThreadUtils;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OrderResult = GeLi_Utils.Entity.AGVApiEntity.OrderResult;

namespace GeLiService_WMS.Threads.DiffFloorThreads
{
    /// <summary>
    /// 跨楼层执行线程
    /// </summary>
    public class TiShengJiThread
    {
        TiShengJiInfo _tiShengJiInfo;
        //public TiShengJiHelper tiShengJiHelper;
        public MyTask runTask;
        public MyTask stepTwofailTask;


        AGVOrderHelper agvOrderHelpers;
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
            var AGVServerIP = ConfigurationManager.AppSettings["AGVIPAndPort"].ToString();
            _tiShengJiInfo = tiShengJiInfo;
            runningKey=$"IsFloorTaskStop:{_tiShengJiInfo.TsjName}";
            agvOrderHelpers = new AGVOrderHelper(AGVServerIP);
            //状态获取
            //tiShengJiHelper = new TiShengJiHelper(tiShengJiInfo);
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
                dtime = DateTime.Now.AddDays(-110);
                
                //判断阶段二任务
                
               // ContinueTask();
                //更新总任务状态:步骤一正常任务由执行流程处理
                //这里主要处理 步骤二任务、步骤一异常任务：已取消、执行失败、发送失败
                UpdateTask(dtime);

                //跨楼层执行分任务
                FloorTask();
                //空托盘运输到托盘缓冲区
                //EmptyTrayToBuffer();

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
        //private void StepTwoFail()
        //{
        //    //没有负载，停止线程，所有阶段二已运行但是未负载的任务都取消
        //    List<AGVMissionInfo_Floor> floorList = floorService.GetList(
        //        u =>u.TSJ_Name==_tiShengJiInfo.TsjName && u.MissionNo.EndsWith(DiffFloorFactory.twoStr)
        //        && u.StateMsg.Length==0 && (u.RunState==StockState.RunState_Running 
        //        || u.RunState==StockState.RunState_Emtpy|| u.RunState == StockState.RunState_HasSend 
        //        || u.RunState == StockState.RunState_WaitRun),
        //        true,DbMainSlave.Master) ;
           
        //    if (floorList.Count>0)
        //    {
        //        string[] orderIds = floorList.Select(u => u.MissionNo).ToArray();
        //        agvOrderUtils.CancelMission(orderIds);
        //    }
        //}

        /// <summary>
        /// 等待确认的任务
        /// </summary>
        /// <param name="list"></param>
        //private void ContinueTask()
        //{
        //    //判断任务是步骤1还是步骤2
        //    //判断提升机是否出货就位,然后判断步骤
        //    //步骤1,则直接执行
        //    //步骤2,判断小车的任务条码是否提升机第一个搬出条码，
        //    //如果是，则直接执行
        //    //如果否，则将条码互换...

        //    //判断等待执行的任务
        //    if (tiShengJiHelper.state == null
        //        || tiShengJiHelper.state.deviceState != DeviceState.Normal)
        //        return;


        //    List<AGVMissionInfo_Floor> waitList = floorService.GetQuery(u =>
        //          u.OrderTime >= dtime && u.TSJ_Name == _tiShengJiInfo.TsjName
        //          && u.RunState == StockState.RunState_WaitRun, true, DbMainSlave.Master)
        //        .OrderBy(u => u.ID).ToList();
        //    foreach (AGVMissionInfo_Floor temp in waitList)
        //    {
        //        Logger.Default.Process(new Log(LevelType.Info,
        //                     $"DiffFloorRunThread:{_tiShengJiInfo.TsjName}提升机是否出货就位"));
        //        //如果任务已继续，则跳过
        //        if (temp.IsContinued == 1)
        //        {
        //            continue;
        //        }
        //        //任务一判断提升机对接位没有货物
        //        if (temp.MissionNo.EndsWith(DiffFloorFactory.oneStr)
        //            && ((temp.StartPosition.StartsWith("1") && tiShengJiHelper.state.F1DuiJieWei == GunZhouState.NoBox)
        //        || (temp.StartPosition.StartsWith("2") && tiShengJiHelper.state.F2DuiJieWei == GunZhouState.NoBox)
        //        || (temp.StartPosition.StartsWith("3") && tiShengJiHelper.state.F3DuiJieWei == GunZhouState.NoBox)))
        //        {
        //            agvOrderUtils.ContinueTask(temp.MissionNo);
        //        }
        //        //任务二判断提升机是否出货就位
        //        else if (temp.MissionNo.EndsWith(DiffFloorFactory.twoStr) &&(
        //            (temp.EndPosition.StartsWith("1") && tiShengJiHelper.state.F1DuiJieWei == GunZhouState.HasBox)
        //        || (temp.EndPosition.StartsWith("2") && tiShengJiHelper.state.F2DuiJieWei == GunZhouState.HasBox)
        //        || (temp.EndPosition.StartsWith("3") && tiShengJiHelper.state.F3DuiJieWei == GunZhouState.HasBox)))
        //            {
        //            //等待小车的任务条码是否提升机第一个搬出条码
        //            //string prosn=RedisCacheHelper.Get<string>("Tsj_" + temp.StartPosition.Substring(0, 1) + "_Out1");
        //            if (temp.TiShengJiRunRecord == null || temp.TiShengJiRunRecord.TrayCount == 1)
        //            {
        //                agvOrderUtils.ContinueTask(temp.MissionNo);
        //            }
        //            else
        //            {
        //                int count = floorService.GetCount(u =>
        //                    u.TiShengJiRecord_ID == temp.TiShengJiRecord_ID
        //                    && u.IsContinued == 1, true, DbMainSlave.Master);
        //                //int count = temp.TiShengJiRunRecord.AGVMissionInfo_Floor.
        //                //    Where(u => u.IsContinued == 1).Count();
        //                string outTrayNo = string.Empty;
        //                if (count == 0)
        //                    //等于0则是第一个任务，用最外面的条码
        //                    outTrayNo = temp.TiShengJiRunRecord.OutsideTrayNo;
        //                else
        //                    //不等于0则是第二个任务，用最里面的条码
        //                    outTrayNo = temp.TiShengJiRunRecord.InsideTrayNo;
        //                Logger.Default.Process(new Log(LevelType.Info,
        //                    $"DiffFloorRunThread:{_tiShengJiInfo.TsjName}获取条码{outTrayNo}"));

        //                //条码相等则继续，不相等则调换条码后继续任务
        //                if (outTrayNo == temp.TrayNo)
        //                {
        //                    agvOrderUtils.ContinueTask(temp.MissionNo);
        //                    floorService.UpdateByPlus(u => u.ID == temp.ID,
        //                        u => new AGVMissionInfo_Floor { IsContinued = 1 });
        //                    Logger.Default.Process(new Log(LevelType.Info,
        //                    $"DiffFloorRunThread:{_tiShengJiInfo.TsjName}继续任务{temp.MissionNo}:{temp.TrayNo}"));
        //                }
        //                else
        //                {
        //                    agvOrderUtils.ContinueTask(temp.MissionNo);
        //                    //调转条码
        //                    floorService.UpdateByPlus(u => u.ID == temp.ID,
        //                        u => new AGVMissionInfo_Floor { TrayNo = outTrayNo, IsContinued = 1 });
        //                    missionService.UpdateByPlus(u => u.ID == temp.MissionFloor_ID,
        //                       u => new AGVMissionInfo{ TrayNo = outTrayNo });
                            
        //                    Logger.Default.Process(new Log(LevelType.Info,
        //                        $"DiffFloorRunThread:{_tiShengJiInfo.TsjName}继续任务{temp.MissionNo}:{ outTrayNo }"));
                          
        //                }
        //            }
        //        }
        //    }


        //}
        

       
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

            if (_tiShengJiInfo.TiShengJiState==null
                || _tiShengJiInfo.TiShengJiState.deviceState != DeviceState.Normal)
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
            else if (buzhouOne_ids.Length >=1)
            {
                StepAOne(noRun_ids, buzhouOne_ids[0]);
            }
            //else if (buzhouOne_ids.Length >= 2)
            //{
            //    StepATwo(buzhouOne_ids);
            //}

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="noRun_ids">没有进入步骤一的总任务ID</param>
        private void StepAZero(int[] noRun_ids)
        {
            AGVMissionFloorService aGVMissionFloorService = new AGVMissionFloorService();
            List<TiShengJiInfo> TiShengJiInfo1s = new List<TiShengJiInfo>();
            DbBase<TiShengJiInfo> TiShengJiInfoDbBase = new DbBase<TiShengJiInfo>();
            TiShengJiInfo1s = TiShengJiInfoDbBase.GetAll();
            List<AGVMissionInfo_Floor> list = aGVMissionFloorService.GetIQueryable(u=> 
            noRun_ids.Contains((int)u.MissionFloor_ID)
            && u.MissionNo.EndsWith(DiffFloorFactory.oneStr)
                , true, DbMainSlave.Master)
                     .OrderBy(u => u.ID).ToList();

            foreach (var item in TiShengJiInfo1s)
            {
                if (list.Count > 0)
                {
                    if (item.TiShengJiState == null) continue;
                    if (item.TiShengJiState.deviceState == DeviceState.Normal && item.TiShengJiState.F1DuiJieWei == TiShengJiStateEntity.AllowUpMission)
                    {
                     OrderResult result = agvOrderHelpers.SendFloorOrder(list[0]);
                        //Logger.Default.Process(new Log(LevelType.Info,"跨楼层任务执行："+ result.ToString()));

                        if (result.code == 200)
                            list[0].SendState = ResultStr.success;
                       // list[0].SendState = ResultStr.success;
                        else
                            list[0].SendState = ResultStr.fail;
                        int MissionID = (int)list[0].MissionFloor_ID;
                        missionService.UpdateByPlus(u => u.ID == MissionID,
                            u => new AGVMissionInfo { SendState = StockState.SendState_BzOne });
                        firstTime = DateTime.Now;
                        aGVMissionFloorService.Update(list[0]);
                        aGVMissionFloorService.SaveChanges();


                    }

                }
            }
            
        }

        private void StepAOne(int[] noRun_ids, int BuZhouOne_id)
        {
            List<TiShengJiInfo> TiShengJiInfo1s = new List<TiShengJiInfo>();
            DbBase<TiShengJiInfo> TiShengJiInfoDbBase = new DbBase<TiShengJiInfo>();
            TiShengJiInfo1s = TiShengJiInfoDbBase.GetAll();
            AGVMissionInfo_Floor nextFloorMission = floorService.GetIQueryable(u =>
            noRun_ids.Contains((int)u.MissionFloor_ID) && u.MissionNo.EndsWith(DiffFloorFactory.oneStr)
                    , true, DbMainSlave.Master)
                     .OrderBy(u => u.ID).FirstOrDefault();
            AGVMissionInfo_Floor runningFloor = floorService.GetIQueryable(
                u => u.MissionFloor_ID == BuZhouOne_id && u.MissionNo.EndsWith(DiffFloorFactory.oneStr)
                , true,DbMainSlave.Master)
                .FirstOrDefault();
            foreach (var item in TiShengJiInfo1s)
            {
                if (item.TiShengJiState == null) continue;
                if (runningFloor.RunState == StockState.RunState_Success && item.TiShengJiState.F2DuiJieWei== TiShengJiStateEntity.SecFloorHadGood)
                {
                    //执行步骤二
                    SendBuZhouSecond(new int[1] { BuZhouOne_id },
                        runningFloor.StartPosition.Substring(0, 1));
                }
            }
            
        }
        
      

        /// <summary>
        /// 发送步骤二任务，并修改总任务
        /// </summary>
        /// <param name="missionId">总任务ID</param>
        /// <param name="startFloor">起始楼层</param>
        private void SendBuZhouSecond(int[] missionId,string startFloor)
        {
            AGVMissionFloorService aGVMissionFloorService = new AGVMissionFloorService();
            var floor_q = aGVMissionFloorService.GetIQueryable(
                    u => missionId.Contains((int)u.MissionFloor_ID)
                    , true, DbMainSlave.Master).OrderBy(u=>u.ID);

            List<AGVMissionInfo_Floor> list_BuZhouOne =
               floor_q.Where(u => u.MissionNo.EndsWith(DiffFloorFactory.oneStr)).
               OrderBy(u=>u.StateTime).ToList();

            List<AGVMissionInfo_Floor> list=
                floor_q.Where(u => u.MissionNo.EndsWith(DiffFloorFactory.twoStr)).ToList();
            string endFloor = list[0].EndPosition.Substring(0, 1);
            TiShengJiInfo tiShengJiInfo1 = new TiShengJiInfo();
            //读取提升机状态
            AddRecord(list_BuZhouOne, list, startFloor, endFloor);
            //tiShengJiHelper.SendTSJOrder(startFloor, endFloor, list.Count);

            //将步骤二任务发送出去

            if (list!=null&&list.Count!=0)
            {
                OrderResult result = agvOrderHelpers.SendFloorOrder(list[0]);
                //Logger.Default.Process(new Log(LevelType.Info,"跨楼层任务执行："+ result.ToString()));

                if (result.code == 200)
                    list[0].SendState = ResultStr.success;
                // list[0].SendState = ResultStr.success;
                else
                    list[0].SendState = ResultStr.fail; 
                //
                int ID_1 = (int)list[0].MissionFloor_ID;
                missionService.UpdateByPlus(u => u.ID == ID_1,
                u => new AGVMissionInfo { SendState = StockState.SendState_BzTwo });
                aGVMissionFloorService.Update(list[0]);
                aGVMissionFloorService.SaveChanges();
                Thread.Sleep(500);
             
            }
                
           
            
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
           // record.OutsideTrayNo = list_BuzhouOne.Count == 2 ? list_BuzhouOne[1].TrayNo : string.Empty;
            //string str= list_BuzhouOne[0].MissionNo.Split('-')[0] + "-2"+ 
            //    (list_BuzhouOne.Count == 2 ? "," + list_BuzhouOne[1].MissionNo.Split('-')[0] + "-2" : string.Empty);

            tiShengJiRunRecordService.Insert(record);
            tiShengJiRunRecordService.SaveChanges();
            //将排序后的记录关联阶段2任务
            int[] ids = list_BuzhouTwo.Select(u => u.ID).ToArray();
            floorService.UpdateByPlus(u => ids.Contains(u.ID),
                u => new AGVMissionInfo_Floor { TiShengJiRecord_ID = record.ID });
            string idsStr = string.Join(",", ids);
            Logger.Default.Process(new Log(LevelType.Info,
            $"绑定跨楼层条码:{_tiShengJiInfo.TsjName}\r\n任务:{idsStr}\r\n条码{record.ID}:{ record.InsideTrayNo }"));
        }

       
        #endregion 

    }
}

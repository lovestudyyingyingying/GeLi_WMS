using GeLi_Utils.Entity.AGVApiEntity;
using GeLi_Utils.Entity.AGVOrderEntity;
using GeLi_Utils.Entity.MaPanJiStateEntity;
using GeLi_Utils.Entity.PDAApiEntity;
using GeLi_Utils.Entity.StockEntity;
using GeLi_Utils.Entity.WareAreaEntity;
using GeLi_Utils.Services.WMS;
using GeLiData_WMS;
using GeLiData_WMS.Dao;
using GeLiData_WMSUtils;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Managers;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Utils.RedisUtils;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UtilsSharp.Shared.Standard;

namespace GeLiService_WMS.Services.WMS
{
    public class MovestockManager
    {
        AGVMissionService _missionService;//AGV任务服务  （对AGVMissionInfo的增删改查）
        LiuShuiHaoService _liuShuiHaoService; // 流水服务
        WareLocationService _wareLocationService; //仓位位置服务
        WareLocationTrayManager _wareLocationTrayNoManager;
        TrayStateService _trayStateService;
        WareLocationLockHisService _wareLocationLockHisService = new WareLocationLockHisService(); //仓库锁服务
        MaPanJiInfoService _maPanJiInfoService;
        MaPanJiStateService _maPanJiStateService;
        TiShengJiInfoService _tiShengJiInfoService;

        RedisHelper redisHelper = new RedisHelper(); //redishelper
                                                     //TrayStateService _trayStateService;


        public MovestockManager(AGVMissionService missionService,
             LiuShuiHaoService liuShuiHaoService, WareLocationService wareLocationService, TiShengJiInfoService tiShengJiInfoService) //构造函数里传入四个服务
        {
            _missionService = missionService;
            _liuShuiHaoService = liuShuiHaoService;
            _wareLocationService = wareLocationService;
            _tiShengJiInfoService = tiShengJiInfoService;
            //实例化赋值的过程
        }

        public MovestockManager(AGVMissionService missionService,
            LiuShuiHaoService liuShuiHaoService, WareLocationService wareLocationService, WareLocationTrayManager wareLocationTrayNoManager, TrayStateService trayStateService, MaPanJiInfoService maPanJiInfoService, MaPanJiStateService maPanJiStateService) //构造函数里传入三个服务
        {
            _maPanJiInfoService = maPanJiInfoService;
            _missionService = missionService;
            _liuShuiHaoService = liuShuiHaoService;
            _wareLocationService = wareLocationService;
            _maPanJiInfoService = maPanJiInfoService;
            _wareLocationTrayNoManager = wareLocationTrayNoManager;
            _trayStateService = trayStateService;

            //实例化赋值的过程
        }




        /// <summary>
        /// 获取楼层库位数据
        /// </summary>
        /// <param name="wlNo">仓位号<param>
        /// <param name="position">楼层</param>
        /// <returns></returns>
        public List<WareLocation> GetWls(string postion, string inputArea, string protype = "")
        {
            //从pda自动进仓进来仓位号默认为空
            //当仓位号不为空则直接获取该仓位号仓位信息
            //当仓位号为空，则获取楼层的所有入库位信息
            //传入楼层
            List<WareLocation> list = string.IsNullOrEmpty(protype) ?
                _wareLocationService.GetList(u =>
                u.IsOpen == 1 && u.WareArea.WareHouse.WHName == postion
                && u.WareArea.WareAreaClass.AreaClass == inputArea, true, DbMainSlave.Master).OrderBy(u => u.ID).ToList()
                :
                _wareLocationService.GetList(u =>
                u.IsOpen == 1 && u.WareArea.WareHouse.WHName == postion
                && u.WareArea.WareAreaClass.AreaClass == inputArea && u.WareArea.protype == protype, true, DbMainSlave.Master).OrderBy(u => u.ID).ToList();//拿到开启的仓位与和相同楼层与仓位状态为入库起点的
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputArea"></param>
        /// <returns></returns>
        public List<WareLocation> GetWls(string inputArea)
        {
            //从pda自动进仓进来仓位号默认为空
            //当仓位号不为空则直接获取该仓位号仓位信息
            //当仓位号为空，则获取楼层的所有入库位信息
            //传入楼层
            List<WareLocation> list = _wareLocationService.GetList(u =>
                u.IsOpen == 1 && u.WareArea.WareAreaClass.AreaClass == inputArea && u.WareLocaState == WareLocaState.HasTray
                );//获取起点库位
            return list;
        }

        #region  旧版任务下发点对点
        /// <summary>
        /// 正常任务下发包括上线和下线
        /// </summary>
        /// <param name="TrayNo"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="userID"></param>
        /// <param name="position"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public object MoveIn(string TrayNo, string startPosition, string endPosition,
            string userID, string position, string remark, string missionType, string processName, string moveType,bool IsIgnoreStart=false,bool isIgnoreEnd=false) // 标签号 ，起点位置, 结束位置 ， 操作人 ， 当前位置 ，备注默认空，类型（货物，空托）
        {
            using (var redislock = redisHelper.CreateLock(startPosition + endPosition, TimeSpan.FromSeconds(10),
                 TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
            {
                Logger.Default.Process(new Log(LevelType.Info, $"对{startPosition}到{endPosition}发起任务请求"));

                DateTime dtime = DateTime.Now.AddDays(-1);


                int runQ = _missionService.GetCount(u => u.OrderTime > dtime && (u.TrayNo == TrayNo
                || u.EndPosition == endPosition) //获取日期大于昨天的并且标签号或结束位置等于传入值的任务
                &&
                 //已下发到表 或者 执行中
                 (u.SendState.Length == 0 ||
                 (u.SendState.Length > 0

                 && !(u.SendState == StockState.SendState_Success
                    && u.RunState == StockState.RunState_Success)

                 && u.RunState != StockState.RunState_RunFail
                 && u.RunState != StockState.RunState_SendFail
                 && u.RunState != StockState.RunState_Cancel)), true, DbMainSlave.Master);
                if (runQ > 0)
                {

                    //如果有则调度失败
                    //result.SetError(StockResult.MovestockError_TrayInMissionError);
                    return new { success = false, message = StockResult.MovestockError_TrayInMissionError };
                }

                //拿到需要放置的结束仓位，终点
                    WareLocation endWl = _wareLocationService.GetIQueryable(u => u.WareLocaNo == endPosition,
                        true, DbMainSlave.Master).FirstOrDefault();
                if (!isIgnoreEnd)
                {
                    if (endWl == null)
                    {
                        //没有这个仓位

                        return new { success = false, message = StockResult.MovestockError_FindEndWLSRError };
                    }

                    if (endWl.WareLocaState == WareLocaState.PreIn || endWl.WareLocaState == WareLocaState.PreOut)
                    {
                        //预进预出

                        return new { success = false, message = StockResult.MovestockError_EndWLIsUseError };
                    }

                    if (endWl.WareLocaState == WareLocaState.HasTray)
                    {

                        return new { success = false, message = StockResult.InstockError_EndWLHasTrayError };

                    }
                }  
                   //拿起点的仓位
                WareLocation startWl = _wareLocationService.GetIQueryable(u => u.WareLocaNo == startPosition,
                        true, DbMainSlave.Master).FirstOrDefault();
             
                if (!IsIgnoreStart)
                {
                  

                    if (startWl == null)
                    {

                        return new { success = false, message = StockResult.MovestockError_FindEndWLSRError };
                    }

                    //表示起始位置没有货物
                    if (startWl.WareLocaState == WareLocaState.NoTray)
                    {

                        return new { success = false, message = StockResult.MovestockError_TrayNoGoodError };
                    }

                    if (startWl.WareLocaState == WareLocaState.PreIn || startWl.WareLocaState == WareLocaState.PreOut)
                    {
                        //预进预出
                        return new { success = false, message = StockResult.MovestockError_EndWLIsUseError };
                    }
                }
                string mark = string.Empty;
                string trayNoToMission = string.Empty;
                if (string.IsNullOrEmpty(TrayNo)) //条码为空
                {
                    if (missionType == GoodType.EmptyTray)//表示空托上下线
                    {
                        trayNoToMission = "空托";
                        Logger.Default.Process(new Log(LevelType.Info, $"空托任务"));





                    }
                    if (missionType == GoodType.GoodTray)//表示物料
                    {
                        if (moveType == "下线" && string.IsNullOrEmpty(TrayNo))// 胀管缓存下线时没有条码但是是下线操作
                        {
                            trayNoToMission = processName.Substring(0, 2) + "成品" + startWl.WareArea.protype;
                        }
                        else
                        {
                            if (startWl.TrayState == null)
                            {
                                return new { success = false, message = StockResult.MovestockError_TrayNoGoodError };
                            }
                            trayNoToMission = startWl.TrayState.TrayNO;
                            mark = MissionType.GoodOnline;
                        }
                    }
                }
                else//有条码表示空托下线
                {
                    trayNoToMission = TrayNo;
                    if (endWl.WareArea.WareAreaClass.Remark == WareAreaType.CacheArea)
                        mark = MissionType.GoodOfflineInHuanCun;
                    if (endWl.WareArea.WareAreaClass.Remark == WareAreaType.ProductionLine)
                        mark = MissionType.GoodOfflineInChanXian;

                }

                AGVMissionInfo agvMission = new AGVMissionInfo();
                agvMission.MissionNo = _liuShuiHaoService.GetAGVMissionNoLSH(); //流水号
                agvMission.Reserve1 = processName;
                agvMission.Reserve2 = startWl.WareArea.WareHouse.WHName;

                agvMission.TrayNo = trayNoToMission;
                agvMission.StartPosition = startWl.WareLocaNo;
                agvMission.StartLocation = startWl.AGVPosition;

                agvMission.StartMiddlePosition = startWl.WareLocaNo;//具体仓位
                agvMission.StartMiddleLocation = startWl.AGVPosition;

                agvMission.EndMiddlePosition = endWl.WareLocaNo;
                agvMission.EndMiddleLocation = endWl.AGVPosition;

                agvMission.EndPosition = endWl.WareLocaNo;
                agvMission.EndLocation = endWl.AGVPosition;

                agvMission.Mark = mark;
                agvMission.OrderTime = DateTime.Now;
                agvMission.OrderGroupId = string.Empty;
                agvMission.AGVCarId = string.Empty;
                agvMission.userId = userID;
                agvMission.IsFloor =
                    (startWl.WareArea.WareHouse.Reserve1
                    == endWl.WareArea.WareHouse.Reserve1)
                    ? 0 : 1;
                if (agvMission.IsFloor == 0)
                    agvMission.WHName = startWl.WareArea.WareHouse.WHName;//如果同层则传入仓库名
                agvMission.SendState = string.Empty;
                agvMission.RunState = string.Empty;
                agvMission.SendMsg = string.Empty;
                agvMission.StateMsg = string.Empty;
                agvMission.ModelProcessCode =
                     //agvMission.IsFloor == 0 ?
                     ////(startWls[0].AGVPosition.StartsWith("1") ? "711" : "2")
                     //startWl.WareArea.WareHouse.AGVModelCode :
                     string.Empty;
                if (remark != string.Empty)
                    agvMission.Remark = remark; //给备注传值


                if (_missionService.Add(agvMission))
                {
                    Logger.Default.Process(new Log(LevelType.Info, $"成功下发任务{agvMission.MissionNo}"));
                    TrayState trayState = null;
                    if (!string.IsNullOrEmpty(TrayNo))//起始标签不为空表示下线
                    {

                        trayState = _trayStateService.GetByTrayNo(TrayNo);
                        //绑定起始位置
                        //startWl.TrayState = taryState;
                        //_wareLocationTrayNoManager.ChangeTrayWareLocation(0, startWl, taryState);
                    }
                    //对warelocation表的   更新起点终点库位状态
                    WareLoactionLockHis wareLoactionLockHisEnd = new WareLoactionLockHis();
                    wareLoactionLockHisEnd.WareLocaNo = endWl.WareLocaNo;
                    wareLoactionLockHisEnd.PreState = WareLocaState.PreIn;
                    wareLoactionLockHisEnd.Locker = userID;
                    DateTime lockTime = DateTime.Parse(
                                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.F"));
                    wareLoactionLockHisEnd.LockTime = lockTime;
                    _wareLocationLockHisService.Insert(wareLoactionLockHisEnd);
                    _wareLocationLockHisService.SaveChanges();


                    _wareLocationService.UpdateByPlus(u => u.ID == endWl.ID,
                        u => new WareLocation
                        { WareLocaState = WareLocaState.PreIn, LockHis_ID = wareLoactionLockHisEnd.ID });

                    WareLoactionLockHis wareLoactionLockHisStart = new WareLoactionLockHis();
                    wareLoactionLockHisStart.WareLocaNo = startWl.WareLocaNo;
                    wareLoactionLockHisStart.PreState = WareLocaState.PreOut;
                    wareLoactionLockHisStart.Locker = userID;

                    wareLoactionLockHisStart.LockTime = lockTime;
                    _wareLocationLockHisService.Insert(wareLoactionLockHisStart);
                    _wareLocationLockHisService.SaveChanges();
                    if (trayState == null)
                        _wareLocationService.UpdateByPlus(u => u.ID == startWl.ID,
                           u => new WareLocation
                           { WareLocaState = WareLocaState.PreOut, LockHis_ID = wareLoactionLockHisStart.ID });
                    else
                    {
                        //trayState.WareLocation_ID=startWl.ID;
                        _wareLocationService.UpdateByPlus(u => u.ID == startWl.ID,
                      u => new WareLocation
                      { WareLocaState = WareLocaState.PreOut, LockHis_ID = wareLoactionLockHisStart.ID, TrayState_ID = trayState.ID });
                        _trayStateService.UpdateByPlus(u => u.ID == trayState.ID,
                           u => new TrayState
                           { WareLocation_ID = startWl.ID });

                    }
                    return new { success = true, message = "成功" };
                }
                else
                {
                    return new { success = false, message = StockResult.MovestockError_WriteMissionError };
                }
            }
        }


        /// <summary>
        /// 插队任务:用于下发插队任务
        /// </summary>
        /// <param name="TrayNo"></param>
        /// <param name="startPosition"></param>
        /// <param name="endArea"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public object JumpQueue(string TrayNo, string startPosition, string endArea, string userID,string processName)
        {
            using (var redislock = redisHelper.CreateLock(startPosition + endArea, TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
            {
                Logger.Default.Process(new Log(LevelType.Info, $"对{startPosition}到{endArea}发起插队任务请求"));
                DateTime dtime = DateTime.Now.AddDays(-1);

               //拿起点的仓位
                WareLocation startWl = _wareLocationService.GetIQueryable(u => u.WareLocaNo == startPosition,
                    true, DbMainSlave.Master).FirstOrDefault();

                if (startWl == null)
                {

                    return new { success = false, message = StockResult.MovestockError_FindEndWLSRError };
                }

                //表示起始位置没有货物
                if (startWl.WareLocaState == WareLocaState.NoTray)
                {

                    return new { success = false, message = StockResult.MovestockError_TrayNoGoodError };
                }

                if (startWl.WareLocaState == WareLocaState.PreIn || startWl.WareLocaState == WareLocaState.PreOut)
                {
                    //预进预出
                    return new { success = false, message = StockResult.MovestockError_EndWLIsUseError };
                }
                AGVMissionJumpQueueService aGVMissionJumpQueueService = new AGVMissionJumpQueueService();
                int count = aGVMissionJumpQueueService.GetIQueryable(u => u.StartPosition == startPosition).Count();
                if(count>0)
                    return new { success = false, message = "当前起点已经有插队任务"};
                AGVMissionJumpQueue aGVMissionJumpQueue = new AGVMissionJumpQueue();
                aGVMissionJumpQueue.MissionNo = _liuShuiHaoService.GetJumpQueueNoLSH();
                aGVMissionJumpQueue.InsertTime = DateTime.Now;
                aGVMissionJumpQueue.TrayNo = TrayNo;
                aGVMissionJumpQueue.Reserve1 = processName;
                aGVMissionJumpQueue.StartPosition = startWl.WareLocaNo;
                aGVMissionJumpQueue.StartLocation = startWl.AGVPosition;
                aGVMissionJumpQueue.TargetArea = endArea;
                aGVMissionJumpQueue.userId = userID;
                aGVMissionJumpQueueService.Insert(aGVMissionJumpQueue);
                aGVMissionJumpQueueService.SaveChanges();
                return new { success = true, message = "成功" };
            }
        }
        /// <summary>
        /// 前端给定起点和终点区域用于下到缓存区的操作1.校验是否可以发送
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="wareArea"></param>
        /// <returns></returns>
        public object MoveToHuanCun(string startPosition, string wareArea)
        {
            //拿起点的仓位
            WareLocation startWl = _wareLocationService.GetIQueryable(u => u.WareLocaNo == startPosition,
                true, DbMainSlave.Master).FirstOrDefault();

            if (startWl == null)
            {

                return new { success = false, message = StockResult.MovestockError_FindEndWLSRError };
            }

            //表示起始位置没有货物
            if (startWl.WareLocaState == WareLocaState.NoTray)
            {

                return new { success = false, message = StockResult.MovestockError_TrayNoGoodError };
            }

            if (startWl.WareLocaState == WareLocaState.PreIn || startWl.WareLocaState == WareLocaState.PreOut)
            {
                //预进预出
                return new { success = false, message = StockResult.MovestockError_EndWLIsUseError };
            }





            return null;
        }


        public BaseResult<string> MoveOutMaPanJi(string TrayNo, string startPosition, string endPosition,
          string userID, string position, string remark, string missionType, string processName, string moveType) // 标签号 ，起点位置(库位), 结束位置 ， 操作人 ， 当前位置 ，备注默认空，类型（货物，空托）
        {
            //Logger.Default.Process(new Log(LevelType.Info, $"测试"));
            DateTime dtime = DateTime.Now.AddDays(-1);
            var result = new BaseResult<string>();
            int runQ = _missionService.GetCount(u => u.OrderTime > dtime && (u.TrayNo == TrayNo
            || u.EndPosition == endPosition) //获取日期大于昨天的并且标签号或结束位置等于传入值的任务
            &&
             //已下发到表 或者 执行中
             (u.SendState.Length == 0 ||
             (u.SendState.Length > 0

             && !(u.SendState == StockState.SendState_Success
                && u.RunState == StockState.RunState_Success)

             && u.RunState != StockState.RunState_RunFail
             && u.RunState != StockState.RunState_SendFail
             && u.RunState != StockState.RunState_Cancel)), true, DbMainSlave.Master);
            if (runQ > 0)
            {

                //如果有则调度失败
                result.SetError(StockResult.MovestockError_TrayInMissionError);
                return result;
            }

            //拿到需要放置的结束仓位，终点

            WareLocation endWl = _wareLocationService.GetIQueryable(u => u.WareLocaNo == endPosition,
                true, DbMainSlave.Master).FirstOrDefault();
            if (endWl == null)
            {
                //没有这个仓位
                result.SetError(StockResult.MovestockError_FindEndWLSRError);
                return result;
            }

            if (endWl.WareLocaState == WareLocaState.PreIn || endWl.WareLocaState == WareLocaState.PreOut)
            {
                //预进预出
                result.SetError(StockResult.MovestockError_EndWLIsUseError);
                return result;
            }

            if (endWl.WareLocaState == WareLocaState.HasTray)
            {
                result.SetError(StockResult.InstockError_EndWLHasTrayError);
                return result;
            }

            //拿起点的仓位
            MaPanJiInfo startWl = _maPanJiInfoService.GetIQueryable(u => u.MpjName == startPosition,
                true, DbMainSlave.Master).FirstOrDefault();

            if (startWl == null)
            {

                result.SetError(StockResult.MovestockError_FindEndWLSRError);
                return result;
            }

            string mark = string.Empty;
            string trayNoToMission = string.Empty;
            if (string.IsNullOrEmpty(TrayNo)) //条码为空
            {
                if (missionType == GoodType.EmptyTray)//表示空托上下线
                {
                    trayNoToMission = "空托";

                    if (endWl.WareArea.WareAreaClass.Remark == WareAreaType.EmptyArea)
                        mark = MissionType.MoveOutMaPanJi; //移动到空托
                }

            }


            AGVMissionInfo agvMission = new AGVMissionInfo();
            agvMission.MissionNo = _liuShuiHaoService.GetAGVMissionNoLSH(); //流水号
            agvMission.Reserve1 = processName;
            // agvMission.Reserve2 = startWl.TsjName;

            agvMission.TrayNo = trayNoToMission;
            agvMission.StartPosition = startWl.MpjName;
            agvMission.StartLocation = startWl.MpjPosition;

            agvMission.StartMiddlePosition = startWl.MpjName;//具体仓位
            agvMission.StartMiddleLocation = startWl.MpjPosition;

            agvMission.EndMiddlePosition = endWl.WareLocaNo;
            agvMission.EndMiddleLocation = endWl.AGVPosition;

            agvMission.EndPosition = endWl.WareLocaNo;
            agvMission.EndLocation = endWl.AGVPosition;

            agvMission.Mark = mark;
            agvMission.OrderTime = DateTime.Now;
            agvMission.OrderGroupId = string.Empty;
            agvMission.AGVCarId = string.Empty;
            agvMission.userId = userID;
            agvMission.IsFloor = 0;
            if (agvMission.IsFloor == 0)
                agvMission.WHName = endWl.WareArea.WareHouse.WHName;//如果同层则传入仓库名
            agvMission.SendState = string.Empty;
            agvMission.RunState = string.Empty;
            agvMission.SendMsg = string.Empty;
            agvMission.StateMsg = string.Empty;
            agvMission.ModelProcessCode =
                 //agvMission.IsFloor == 0 ?
                 ////(startWls[0].AGVPosition.StartsWith("1") ? "711" : "2")
                 //startWl.WareArea.WareHouse.AGVModelCode :
                 string.Empty;
            if (remark != string.Empty)
                agvMission.Remark = remark; //给备注传值

            if (_missionService.Add(agvMission))
            {
                Logger.Default.Process(new Log(LevelType.Info, $"成功下发任务{agvMission.MissionNo}"));
                TrayState trayState = null;
                if (!string.IsNullOrEmpty(TrayNo))//起始标签不为空表示下线
                {

                    trayState = _trayStateService.GetByTrayNo(TrayNo);
                    //绑定起始位置
                    //startWl.TrayState = taryState;
                    //_wareLocationTrayNoManager.ChangeTrayWareLocation(0, startWl, taryState);
                }
                //对warelocation表的   更新起点终点库位状态
                WareLoactionLockHis wareLoactionLockHisEnd = new WareLoactionLockHis();
                wareLoactionLockHisEnd.WareLocaNo = endWl.WareLocaNo;
                wareLoactionLockHisEnd.PreState = WareLocaState.PreIn;
                wareLoactionLockHisEnd.Locker = userID;
                DateTime lockTime = DateTime.Parse(
                              DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.F"));
                wareLoactionLockHisEnd.LockTime = lockTime;
                _wareLocationLockHisService.Insert(wareLoactionLockHisEnd);
                _wareLocationLockHisService.SaveChanges();
                _wareLocationService.UpdateByPlus(u => u.ID == endWl.ID,
                    u => new WareLocation
                    { WareLocaState = WareLocaState.PreIn, LockHis_ID = wareLoactionLockHisEnd.ID });

                //WareLoactionLockHis wareLoactionLockHisStart = new WareLoactionLockHis();
                //wareLoactionLockHisStart.WareLocaNo = startWl.WareLocaNo;
                //wareLoactionLockHisStart.PreState = WareLocaState.PreOut;
                //wareLoactionLockHisStart.Locker = userID;

                //wareLoactionLockHisStart.LockTime = lockTime;
                //_wareLocationLockHisService.Insert(wareLoactionLockHisStart);
                //_wareLocationLockHisService.SaveChanges();
                //if (trayState == null)
                //    _wareLocationService.UpdateByPlus(u => u.ID == startWl.ID,
                //       u => new WareLocation
                //       { WareLocaState = WareLocaState.PreOut, LockHis_ID = wareLoactionLockHisStart.ID });
                //else
                //{
                //    //trayState.WareLocation_ID=startWl.ID;
                //    _wareLocationService.UpdateByPlus(u => u.ID == startWl.ID,
                //          u => new WareLocation
                //          { WareLocaState = WareLocaState.PreOut, LockHis_ID = wareLoactionLockHisStart.ID, TrayState_ID = trayState.ID });


                //    _trayStateService.UpdateByPlus(u => u.ID == trayState.ID,
                //       u => new TrayState
                //       { WareLocation_ID = startWl.ID });

                //}
                result.SetOk();
                return result;
            }
            else
            {
                result.SetError(StockResult.MovestockError_WriteMissionError);
                return result;
            }
        }


        /// <summary>
        /// 根据终点区域拆分为缓存区点位
        /// </summary>
        /// <returns></returns>
        public string SplitAreaToPosition(string WareAre)
        {

            return _wareLocationService.GetIQueryable(u => u.WareArea.WareAreaClass.Reserve2.Contains(WareAre) && u.IsOpen == 1 && u.WareLocaState == WareLocaState.NoTray).Select(u => u.WareLocaNo).FirstOrDefault();

            //switch (WareAre)
            //{
            //    case "热烘干产线1号区":
            //        return _wareLocationService.GetIQueryable(u => u.WareArea.WareAreaClass.AreaClass == "热胀管缓存区" && u.IsOpen == 1 && u.WareLocaState == WareLocaState.NoTray).Select(u => u.WareLocaNo).FirstOrDefault();
            //    case "热烘干产线2号区":
            //        return _wareLocationService.GetIQueryable(u => u.WareArea.WareAreaClass.AreaClass == "热胀管缓存区" && u.IsOpen == 1 && u.WareLocaState == WareLocaState.NoTray).Select(u => u.WareLocaNo).FirstOrDefault();
            //    case "冷烘干产线1号区":
            //        return _wareLocationService.GetIQueryable(u => u.WareArea.WareAreaClass.AreaClass == "冷胀管缓存区" && u.IsOpen == 1 && u.WareLocaState == WareLocaState.NoTray).Select(u => u.WareLocaNo).FirstOrDefault();
            //    case "冷烘干产线2号区":
            //        return _wareLocationService.GetIQueryable(u => u.WareArea.WareAreaClass.AreaClass == "冷胀管缓存区" && u.IsOpen == 1 && u.WareLocaState == WareLocaState.NoTray).Select(u => u.WareLocaNo).FirstOrDefault();
            //    case "冷氮检产线1号区":
            //        return _wareLocationService.GetIQueryable(u => u.WareArea.WareAreaClass.AreaClass == "冷缓存区" && u.IsOpen == 1 && u.WareLocaState == WareLocaState.NoTray).Select(u => u.WareLocaNo).FirstOrDefault();
            //    case "冷氮检产线2号区":
            //        return _wareLocationService.GetIQueryable(u => u.WareArea.WareAreaClass.AreaClass == "冷缓存区" && u.IsOpen == 1 && u.WareLocaState == WareLocaState.NoTray).Select(u => u.WareLocaNo).FirstOrDefault();
            //    case "热氮检产线1号区":
            //        return _wareLocationService.GetIQueryable(u => u.WareArea.WareAreaClass.AreaClass == "热缓存区" && u.IsOpen == 1 && u.WareLocaState == WareLocaState.NoTray).Select(u => u.WareLocaNo).FirstOrDefault();
            //    case "热氮检产线2号区":
            //        return _wareLocationService.GetIQueryable(u => u.WareArea.WareAreaClass.AreaClass == "热缓存区" && u.IsOpen == 1 && u.WareLocaState == WareLocaState.NoTray).Select(u => u.WareLocaNo).FirstOrDefault();
            //    default:
            //        return string.Empty;
            //}

        }



    
        /// <summary>
        /// 正常任务下发包括上线和下线
        /// </summary>
        /// <param name="TrayNo"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="userID"></param>
        /// <param name="position"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public BaseResult<string> MoveIn_Su(string TrayNo, string startPosition, string endPosition,
            string userID, string position, string remark, string missionType, string processName, string moveType, TiShengJiInfo _tiShengJiInfo, TiShengJiRunRecordService tiShengJiRunRecordService) // 标签号 ，起点位置, 结束位置 ， 操作人 ， 当前位置 ，备注默认空，类型（货物，空托）
        {
            using (var redislock = redisHelper.CreateLock(startPosition + endPosition, TimeSpan.FromSeconds(10),
                 TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
            {
                BaseResult<string> baseResult = new BaseResult<string>();
                Logger.Default.Process(new Log(LevelType.Info, $"对{startPosition}到{endPosition}发起任务请求"));

                DateTime dtime = DateTime.Now.AddDays(-1);


                int runQ = _missionService.GetCount(u => u.OrderTime > dtime && (u.TrayNo == TrayNo
                || u.EndPosition == endPosition) //获取日期大于昨天的并且标签号或结束位置等于传入值的任务
                &&
                 //已下发到表 或者 执行中
                 (u.SendState.Length == 0 ||
                 (u.SendState.Length > 0

                 && !(u.SendState == StockState.SendState_Success
                    && u.RunState == StockState.RunState_Success)

                 && u.RunState != StockState.RunState_RunFail
                 && u.RunState != StockState.RunState_SendFail
                 && u.RunState != StockState.RunState_Cancel)), true, DbMainSlave.Master);
                if (runQ > 0)
                {
                    baseResult.Code = 7000;
                    baseResult.Msg = StockResult.MovestockError_TrayInMissionError;
                    //如果有则调度失败
                    //result.SetError(StockResult.MovestockError_TrayInMissionError);
                    return baseResult;
                }

                //拿到需要放置的结束仓位，终点

                WareLocation endWl = _wareLocationService.GetIQueryable(u => u.WareLocaNo == endPosition,
                    true, DbMainSlave.Master).FirstOrDefault();
                if (endWl == null)
                {
                    //没有这个仓位
                    baseResult.Code = 7000;
                    baseResult.Msg = StockResult.MovestockError_FindEndWLSRError;

                    return baseResult;
                }   

                if (endWl.WareLocaState == WareLocaState.PreIn || endWl.WareLocaState == WareLocaState.PreOut)
                {
                    //预进预出
                    baseResult.Code = 7000;
                    baseResult.Msg = StockResult.MovestockError_EndWLIsUseError;

                    return baseResult;
                    
                }

                if (endWl.WareLocaState == WareLocaState.HasTray)
                {
                    baseResult.Code = 7000;
                    baseResult.Msg = StockResult.InstockError_EndWLHasTrayError;

                    return baseResult;
                   

                }

                //拿起点的仓位
                WareLocation startWl = _wareLocationService.GetIQueryable(u => u.WareLocaNo == startPosition,
                    true, DbMainSlave.Master).FirstOrDefault();

                if (startWl == null)
                {
                    baseResult.Code = 7000;
                    baseResult.Msg = StockResult.MovestockError_FindEndWLSRError;
                    return baseResult; 
                }

                //表示起始位置没有货物
                if (startWl.WareLocaState == WareLocaState.NoTray)
                {
                    baseResult.Code = 7000;
                    baseResult.Msg = StockResult.MovestockError_TrayNoGoodError;
                    return baseResult;
                    
                }

                if (startWl.WareLocaState == WareLocaState.PreIn || startWl.WareLocaState == WareLocaState.PreOut)
                {
                    //预进预出
                    baseResult.Code = 7000;
                    baseResult.Msg = StockResult.MovestockError_EndWLIsUseError;
                    return baseResult;
                   
                }
                string mark = string.Empty;
                string trayNoToMission = string.Empty;
                if (string.IsNullOrEmpty(TrayNo)) //条码为空
                {
                    if (missionType == GoodType.EmptyTray)//表示空托上下线
                    {
                        trayNoToMission = "空托";
                        Logger.Default.Process(new Log(LevelType.Info, $"空托任务"));





                    }
                    if (missionType == GoodType.GoodTray)//表示物料
                    {
                        if (moveType == "下线" && string.IsNullOrEmpty(TrayNo))// 胀管缓存下线时没有条码但是是下线操作
                        {
                            trayNoToMission = processName.Substring(0, 2) + "成品" + startWl.WareArea.protype;
                        }
                        else
                        {
                            if (startWl.TrayState == null)
                            {
                                baseResult.Code = 7000;
                                baseResult.Msg = StockResult.MovestockError_TrayNoGoodError;
                                return baseResult;
                               
                            }
                            trayNoToMission = startWl.TrayState.TrayNO;
                            mark = MissionType.GoodOnline;
                        }
                    }
                }
                else//有条码表示空托下线
                {
                    trayNoToMission = TrayNo;
                    if (endWl.WareArea.WareAreaClass.Remark == WareAreaType.CacheArea)
                        mark = MissionType.GoodOfflineInHuanCun;
                    if (endWl.WareArea.WareAreaClass.Remark == WareAreaType.ProductionLine)
                        mark = MissionType.GoodOfflineInChanXian;

                }

                AGVMissionInfo agvMission = new AGVMissionInfo();
                agvMission.MissionNo = _liuShuiHaoService.GetAGVMissionNoLSH(); //流水号
                agvMission.Reserve1 = processName;
                agvMission.Reserve2 = startWl.WareArea.WareHouse.WHName;

                agvMission.TrayNo = trayNoToMission;
                agvMission.StartPosition = startWl.WareLocaNo;
                agvMission.StartLocation = startWl.AGVPosition;

                agvMission.StartMiddlePosition = startWl.WareLocaNo;//具体仓位
                agvMission.StartMiddleLocation = startWl.AGVPosition;

                agvMission.EndMiddlePosition = endWl.WareLocaNo;
                agvMission.EndMiddleLocation = endWl.AGVPosition;

                agvMission.EndPosition = endWl.WareLocaNo;
                agvMission.EndLocation = endWl.AGVPosition;

                agvMission.Mark = mark;
                agvMission.OrderTime = DateTime.Now;
                agvMission.OrderGroupId = string.Empty;
                agvMission.AGVCarId = string.Empty;
                agvMission.userId = userID;
                agvMission.IsFloor =
                    (startWl.WareArea.WareHouse.Reserve1
                    == endWl.WareArea.WareHouse.Reserve1)
                    ? 0 : 1;
                if (agvMission.IsFloor == 0)
                    agvMission.WHName = startWl.WareArea.WareHouse.WHName;//如果同层则传入仓库名
                agvMission.SendState = string.Empty;
                agvMission.RunState = string.Empty;
                agvMission.SendMsg = string.Empty;
                agvMission.StateMsg = string.Empty;
                agvMission.ModelProcessCode =
                     //agvMission.IsFloor == 0 ?
                     ////(startWls[0].AGVPosition.StartsWith("1") ? "711" : "2")
                     //startWl.WareArea.WareHouse.AGVModelCode :
                     string.Empty;
                if (remark != string.Empty)
                    agvMission.Remark = remark; //给备注传值


                if (_missionService.Add(agvMission))
                {
                    Logger.Default.Process(new Log(LevelType.Info, $"成功下发任务{agvMission.MissionNo}"));
                    TrayState trayState = null;
                    if (!string.IsNullOrEmpty(TrayNo))//起始标签不为空表示下线
                    {

                        trayState = _trayStateService.GetByTrayNo(TrayNo);
                        //绑定起始位置
                        //startWl.TrayState = taryState;
                        //_wareLocationTrayNoManager.ChangeTrayWareLocation(0, startWl, taryState);
                    }
                    //对warelocation表的   更新起点终点库位状态
                    WareLoactionLockHis wareLoactionLockHisEnd = new WareLoactionLockHis();
                    wareLoactionLockHisEnd.WareLocaNo = endWl.WareLocaNo;
                    wareLoactionLockHisEnd.PreState = WareLocaState.PreIn;
                    wareLoactionLockHisEnd.Locker = userID;
                    DateTime lockTime = DateTime.Parse(
                                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.F"));
                    wareLoactionLockHisEnd.LockTime = lockTime;
                    _wareLocationLockHisService.Insert(wareLoactionLockHisEnd);
                    _wareLocationLockHisService.SaveChanges();


                    _wareLocationService.UpdateByPlus(u => u.ID == endWl.ID,
                        u => new WareLocation
                        { WareLocaState = WareLocaState.PreIn, LockHis_ID = wareLoactionLockHisEnd.ID });

                    WareLoactionLockHis wareLoactionLockHisStart = new WareLoactionLockHis();
                    wareLoactionLockHisStart.WareLocaNo = startWl.WareLocaNo;
                    wareLoactionLockHisStart.PreState = WareLocaState.PreOut;
                    wareLoactionLockHisStart.Locker = userID;

                    wareLoactionLockHisStart.LockTime = lockTime;
                    _wareLocationLockHisService.Insert(wareLoactionLockHisStart);
                    _wareLocationLockHisService.SaveChanges();
                    if (trayState == null)
                        _wareLocationService.UpdateByPlus(u => u.ID == startWl.ID,
                           u => new WareLocation
                           { WareLocaState = WareLocaState.PreOut, LockHis_ID = wareLoactionLockHisStart.ID });
                    else
                    {
                        //trayState.WareLocation_ID=startWl.ID;
                        _wareLocationService.UpdateByPlus(u => u.ID == startWl.ID,
                      u => new WareLocation
                      { WareLocaState = WareLocaState.PreOut, LockHis_ID = wareLoactionLockHisStart.ID, TrayState_ID = trayState.ID });
                        _trayStateService.UpdateByPlus(u => u.ID == trayState.ID,
                           u => new TrayState
                           { WareLocation_ID = startWl.ID });

                    }
                    //提升机得物料流水信息
                    if (_tiShengJiInfo!=null&& agvMission.TrayNo!= GoodType.EmptyTray)
                    {
                        AddRecord(agvMission,_tiShengJiInfo,tiShengJiRunRecordService);
                    }
                    baseResult.Code = 200;
                    baseResult.Msg = "成功";
                    return baseResult;
                   
                }
                else
                {
                    baseResult.Code = 700;
                    baseResult.Msg = StockResult.MovestockError_WriteMissionError;
                    return baseResult;
                }
            }
        }
        private void AddRecord(AGVMissionInfo aGVMissionInfo,TiShengJiInfo _tiShengJiInfo,TiShengJiRunRecordService tiShengJiRunRecordService)
        {

            TiShengJiRunRecord record = new TiShengJiRunRecord();
            record.TsjName = _tiShengJiInfo.TsjName;
            record.TsjIp = _tiShengJiInfo.TsjIp;
            record.TsjPort = _tiShengJiInfo.TsjPort;
            record.OrderTime = DateTime.Now;
            record.TrayCount = 1;
            record.InsideTrayNo = aGVMissionInfo.TrayNo;
            record.Reserve1 = "0";
            record.Reserve2 = aGVMissionInfo.Reserve1;
            tiShengJiRunRecordService.Insert(record);
            tiShengJiRunRecordService.SaveChanges();
            //将排序后的记录关联阶段2任务
            //int[] ids = aGVMissionInfo.Select(u => u.ID).ToArray();
            _missionService.UpdateByPlus(u => u.ID == aGVMissionInfo.ID,
                u => new AGVMissionInfo { Reserve3 = record.ID.ToString() });
            // string idsStr = string.Join(",", ids);
            Logger.Default.Process(new Log(LevelType.Info,
            $"绑定跨楼层条码:{_tiShengJiInfo.TsjName}\r\n任务:{aGVMissionInfo.MissionNo}\r\n条码{record.ID}:{ record.InsideTrayNo }"));
        }
        public object MoveToMaPanJi(string startPosition, string endPosition, string userID, string processName)
        {
            //加锁
            using (var redislock = redisHelper.CreateLock(startPosition + endPosition, TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
            {
                WareLocation endWl = _wareLocationService.GetIQueryable(u => u.WareLocaNo == endPosition,
                 true, DbMainSlave.Master).FirstOrDefault();
                if (endWl == null)
                {
                    //没有这个仓位

                    return new { success = false, message = StockResult.MovestockError_FindEndWLSRError };
                }
                //获取起点位置
                WareLocation startWl = _wareLocationService.GetIQueryable(u => u.WareLocaNo == startPosition,
                  true, DbMainSlave.Master).FirstOrDefault();

                if (startWl == null)
                {

                    return new { success = false, message = StockResult.MovestockError_FindEndWLSRError };
                }

                //表示起始位置没有货物
                if (startWl.WareLocaState == WareLocaState.NoTray)
                {

                    return new { success = false, message = StockResult.MovestockError_TrayNoGoodError };
                }

                if (startWl.WareLocaState == WareLocaState.PreIn || startWl.WareLocaState == WareLocaState.PreOut)
                {
                    //预进预出
                    return new { success = false, message = StockResult.MovestockError_EndWLIsUseError };
                }
                AGVMissionInfo agvMission = new AGVMissionInfo();
                agvMission.MissionNo = _liuShuiHaoService.GetAGVMissionNoLSH(); //流水号
                agvMission.Reserve1 = processName;
                agvMission.Reserve2 = startWl.WareArea.WareHouse.WHName;

                agvMission.TrayNo = "空托";
                agvMission.StartPosition = startWl.WareLocaNo;
                agvMission.StartLocation = startWl.AGVPosition;

                agvMission.StartMiddlePosition = startWl.WareLocaNo;//具体仓位
                agvMission.StartMiddleLocation = startWl.AGVPosition;

                agvMission.EndMiddlePosition = endWl.WareLocaNo;
                agvMission.EndMiddleLocation = endWl.AGVPosition;

                agvMission.EndPosition = endWl.WareLocaNo;
                agvMission.EndLocation = endWl.AGVPosition;

                agvMission.Mark = MissionType.MoveToMaPanJi; //移动到码盘机;
                agvMission.OrderTime = DateTime.Now;
                agvMission.OrderGroupId = string.Empty;
                agvMission.AGVCarId = string.Empty;
                agvMission.userId = userID;
                agvMission.IsFloor =
                    (startWl.WareArea.WareHouse.Reserve1
                    == endWl.WareArea.WareHouse.Reserve1)
                    ? 0 : 1;
                if (agvMission.IsFloor == 0)
                    agvMission.WHName = startWl.WareArea.WareHouse.WHName;//如果同层则传入仓库名
                agvMission.SendState = string.Empty;
                agvMission.RunState = string.Empty;
                agvMission.SendMsg = string.Empty;
                agvMission.StateMsg = string.Empty;
                agvMission.ModelProcessCode =
                     //agvMission.IsFloor == 0 ?
                     ////(startWls[0].AGVPosition.StartsWith("1") ? "711" : "2")
                     //startWl.WareArea.WareHouse.AGVModelCode :
                     string.Empty;

                agvMission.Remark = string.Empty; //给备注传值


                if (_missionService.Add(agvMission))
                {
                    Logger.Default.Process(new Log(LevelType.Info, $"成功下发码盘机任务{agvMission.MissionNo}"));
                    WareLoactionLockHis wareLoactionLockHisStart = new WareLoactionLockHis();
                    wareLoactionLockHisStart.WareLocaNo = startWl.WareLocaNo;
                    wareLoactionLockHisStart.PreState = WareLocaState.PreOut;
                    wareLoactionLockHisStart.Locker = userID;

                    wareLoactionLockHisStart.LockTime = DateTime.Now;
                    _wareLocationLockHisService.Insert(wareLoactionLockHisStart);
                    _wareLocationLockHisService.SaveChanges();

                    _wareLocationService.UpdateByPlus(u => u.ID == startWl.ID,
                       u => new WareLocation
                       { WareLocaState = WareLocaState.PreOut, LockHis_ID = wareLoactionLockHisStart.ID });
                    return new { success = true, message = "成功" };
                }
                else
                {
                    return new { success = false, message = StockResult.MovestockError_WriteMissionError };
                }

            }
        }
        #endregion

        /// <summary>
        /// AGV申请进入码盘机
        /// </summary>
        /// <param name="deviceApply"></param>
        /// <returns></returns>
        public OrderResult AgvMoveInMaPanJi(DeviceApply deviceApply)
        {
            Logger.Default.Process(new Log(LevelType.Info, $"PDA_deviceApply:任务{deviceApply.taskId}申请进入码盘机"));
            if (deviceApply == null)
                return new OrderResult() { code = 999, msg = AgvManPanJiErrorState.CanotGetSatate };

            var misson = _missionService.GetList(u => u.MissionNo == deviceApply.taskId).FirstOrDefault();
            if (misson == null)
                return new OrderResult() { code = 999, msg = "没有找到此任务" };
            if (misson.Mark != MissionType.MoveToMaPanJi && misson.Mark != MissionType.MoveOutMaPanJi)
                return new OrderResult() { code = 999, msg = "此任务不是码盘机任务" };
            if (misson.RunState == StockState.RunState_AgvInMaPan)
                return new OrderResult() { code = 999, msg = "AGV已进入" };
            if (misson.RunState == StockState.RunState_Success)
                return new OrderResult() { code = 999, msg = "该任务已完成" };

            //对设备是否可进进行判断
            var maPanJiSatae = _maPanJiInfoService.GetMaPanJiStateByMaPanJiName(deviceApply.deviceId);
            Logger.Default.Process(new Log(LevelType.Info, $"PDA_deviceApply:从数据库中获取码盘机的状态为{maPanJiSatae}"));

            if (string.IsNullOrEmpty(maPanJiSatae))
                return new OrderResult() { code = 999, msg = AgvManPanJiErrorState.CanotGetSatate };

            if (maPanJiSatae == MaPanJiStateSummarize.original || maPanJiSatae == MaPanJiStateSummarize.Idle)
                return new OrderResult() { code = 999, msg = AgvManPanJiErrorState.MaPanJiNoInDieBan };

            if (maPanJiSatae == MaPanJiStateSummarize.DeliverTask)
                return new OrderResult() { code = 999, msg = AgvManPanJiErrorState.MaPanJiReadying };

            if (maPanJiSatae == MaPanJiStateSummarize.DieBaning)
                return new OrderResult() { code = 999, msg = AgvManPanJiErrorState.MaPanJiDieBanIng };
            if (maPanJiSatae == "码盘机故障")
                return new OrderResult() { code = 999, msg = maPanJiSatae };

            if (deviceApply.action == "put")//表示去放货（拿货去叠板）
            {

                if (maPanJiSatae == MaPanJiStateSummarize.TaskingCanIn)
                {
                    Logger.Default.Process(new Log(LevelType.Info, $"PDA_deviceApply:根据码盘机判断为允许进入状态"));

                    //表示允许进入 改写当前task任务状态表示AGV进入 回写成功允许进入
                    misson.RunState = StockState.RunState_AgvInMaPan;
                    _missionService.UpdateByPlus(u => u.ID == misson.ID, u => new AGVMissionInfo { RunState = misson.RunState, NodeTime = DateTime.Now });
                    Logger.Default.Process(new Log(LevelType.Info, $"PDA_deviceApply:改写数据库{misson.ID}成功"));

                    return new OrderResult() { code = 200, msg = "成功success" };

                }
            }
            else if (deviceApply.action == "get")//表示满盘去拿走（拿货去叠板）
                if (maPanJiSatae == MaPanJiStateSummarize.FullEmptyTray)
                {
                    Logger.Default.Process(new Log(LevelType.Info, $"PDA_deviceApply:判断为拿货并且码盘机状态为满货"));
                    //表示允许进入 改写当前task任务状态表示AGV进入 回写成功允许进入
                    misson.RunState = StockState.RunState_AgvInMaPan;
                    _missionService.UpdateByPlus(u => u.ID == misson.ID, u => new AGVMissionInfo { RunState = misson.RunState, NodeTime = DateTime.Now });
                    return new OrderResult() { code = 200, msg = "成功success" };

                }

            return new OrderResult() { code = 999, msg = AgvManPanJiErrorState.CanotGetSatate };


        }


        /// <summary>
        /// AGV离开码盘机告知
        /// </summary>
        /// <param name="deviceApply"></param>
        /// <returns></returns>
        public OrderResult AgvMoveOutMaPanJi(DeviceApply deviceApply)
        {
            Logger.Default.Process(new Log(LevelType.Info, $"PDA_deviceApply:任务{deviceApply.taskId}告知离开码盘机"));
            var misson = _missionService.GetList(u => u.MissionNo == deviceApply.taskId).FirstOrDefault();
            _missionService.UpdateByPlus(u => u.ID == misson.ID, u => new AGVMissionInfo { RunState = StockState.RunState_AgvLeaveMaPan, SendState = StockState.SendState_Success });
            Logger.Default.Process(new Log(LevelType.Info, $"PDA_deviceApply:更新任务{misson.ID}成功"));
            return new OrderResult() { code = 200, msg = "成功success" };
        }

        public object FindWearLocationStartAndEnd(string startPosition, string endPosition, string startArea, string endArea, string protype, string missionType, object nextArea, object nextLocation)
        {
            using (var redislock = redisHelper.CreateLock(startArea + endArea, TimeSpan.FromSeconds(10),
                 TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
            {
                List<WareLocation> startList = missionType == "上线" ? GetWls(startPosition, startArea) : GetWls(startPosition, startArea, protype);
                List<WareLocation> startList2 = _wareLocationService.ConvertList(startList);
                List<WareLocation> endList = new List<WareLocation>();


                endList = missionType == "上线" ? GetWls(endPosition, endArea, protype) : GetWls(endPosition, endArea);
                if (nextArea != null && endList.Where(u => u.WareLocaState == "空").Count() == 0)//如果有缓存区的判断(如果终点满位)
                {
                    string nextPosition = nextLocation.ToString();
                    endList = GetWls(nextPosition, nextArea.ToString(), string.Empty);

                }
                List<WareLocation> endList2 = _wareLocationService.ConvertList(endList);
                List<PostWarelocation> startwarelocations = new List<PostWarelocation>();
                foreach (var item in startList2)
                {
                    var startWareLocation = new PostWarelocation();
                    startWareLocation.name = item.WareLocaNo;
                    startWareLocation.state = item.WareLocaState;
                    startwarelocations.Add(startWareLocation);

                }
                List<PostWarelocation> endwarelocations = new List<PostWarelocation>();
                foreach (var item in endList2)
                {
                    var end = new PostWarelocation();
                    end.name = item.WareLocaNo;
                    end.state = item.WareLocaState;
                    endwarelocations.Add(end);
                }
                var result = new { success = true, message = "操作成功", data = new FindWareLocationEntity { startWareLocation = startwarelocations.ToArray(), endWareLocation = endwarelocations.ToArray() } };

                //var result = new { data = list2 };
                //return new
                //  {
                //      startWareLocation = new { startList2.Select(u => new {name= u.WareLocaNo,state =  u.WareLocaState } },
                //      endWareLocation = new { name = endList2.Select(u => u.WareLocaNo), state = endList2.Select(u => u.WareLocaState) }
                //  };
                return result;
            }

        }

        /// <summary>
        /// 获取相关区域
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="startArea"></param>
        /// <param name="endArea"></param>
        /// <returns></returns>
        public object FindWearLocationStartAndEnd(string startPosition, string endPosition, string startArea, string endArea,bool isIgnoreStartState=false)
        {
            using (var redislock = redisHelper.CreateLock(startArea + endArea, TimeSpan.FromSeconds(10),
                 TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
            {
                List<WareLocation> startList = GetWls(startPosition, startArea);
                List<WareLocation> startList2 = _wareLocationService.ConvertList(startList);
                List<WareLocation> endList = new List<WareLocation>();

                endList = GetWls(endPosition, endArea);
                List<WareLocation> endList2 = _wareLocationService.ConvertList(endList);


                List<PostWarelocation> startwarelocations = new List<PostWarelocation>();

                if (!isIgnoreStartState)
                {
                    foreach (var item in startList2.Where(u => u.WareLocaState == WareLocaState.HasTray))
                    {
                        var startWareLocation = new PostWarelocation();
                        startWareLocation.name = item.WareLocaNo;
                        startWareLocation.state = item.WareLocaState;
                        startwarelocations.Add(startWareLocation);

                    }
                }
                else
                {
                    foreach (var item in startList2)
                    {
                        var startWareLocation = new PostWarelocation();
                        startWareLocation.name = item.WareLocaNo;
                        startWareLocation.state = item.WareLocaState;
                        startwarelocations.Add(startWareLocation);
                    }
                }

                List<PostWarelocation> endwarelocations = new List<PostWarelocation>();
                foreach (var item in endList2.Where(u => u.WareLocaState == WareLocaState.NoTray))
                {
                    var end = new PostWarelocation();
                    end.name = item.WareLocaNo;
                    end.state = item.WareLocaState;
                    endwarelocations.Add(end);
                }
                var result = new { success = true, message = "操作成功", data = new FindWareLocationEntity { startWareLocation = startwarelocations.ToArray(), endWareLocation = endwarelocations.ToArray() } };

                //var result = new { data = list2 };
                //return new
                //  {
                //      startWareLocation = new { startList2.Select(u => new {name= u.WareLocaNo,state =  u.WareLocaState } },
                //      endWareLocation = new { name = endList2.Select(u => u.WareLocaNo), state = endList2.Select(u => u.WareLocaState) }
                //  };
                return result;
            }

        }



        public object FindStartPointAndEndArea(string startArea, string areaRemark)
        {
            using (var redislock = redisHelper.CreateLock(startArea + areaRemark, TimeSpan.FromSeconds(10),
               TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
            {
                List<WareLocation> startList = GetWls(startArea);
                List<WareLocation> startList2 = _wareLocationService.ConvertList(startList);
                List<WareAreaClass> endAreaClass = GetArea(areaRemark);
                List<PostWarelocation> startwarelocations = new List<PostWarelocation>();
                List<PostWarelocation> endwarelocations = new List<PostWarelocation>();
                foreach (var item in startList2.Where(u => u.WareLocaState == WareLocaState.HasTray))
                {
                    var end = new PostWarelocation();
                    end.name = item.WareLocaNo;
                    end.state = item.WareLocaState;
                    startwarelocations.Add(end);
                }
                foreach (var item in endAreaClass)
                {
                    var end = new PostWarelocation();
                    end.name = item.AreaClass;
                    end.state = string.Empty;
                    endwarelocations.Add(end);
                }

                var result = new { success = true, message = "操作成功", data = new FindWareLocationEntity { startWareLocation = startwarelocations.ToArray(), endWareLocation = endwarelocations.ToArray() } };
                return result;
            }
        }

        private List<WareAreaClass> GetArea(string areaRemark)
        {
            WareAreaClassService wareAreaClassService = new WareAreaClassService();
            var list = wareAreaClassService.GetList(u => u.Reserve1.Contains(areaRemark) && u.IsOpen == true);
            var list2 = wareAreaClassService.ConvertList(list);
            return list2;
        }



        /// <summary>
        /// 任务下发包括上线和下线
        /// </summary>
        /// <param name="TrayNo"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="userID"></param>
        /// <param name="position"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public BaseResult<string> MoveOutTiShengJi(string TrayNo, string startPosition, string endPosition,
            string userID, string position, string remark, string missionType, string processName, string moveType) // 标签号 ，起点位置(库位), 结束位置 ， 操作人 ， 当前位置 ，备注默认空，类型（货物，空托）
        {
            //Logger.Default.Process(new Log(LevelType.Info, $"测试"));
            DateTime dtime = DateTime.Now.AddDays(-1);
            var result = new BaseResult<string>();
            int runQ = _missionService.GetCount(u => u.OrderTime > dtime && (u.TrayNo == TrayNo
            || u.EndPosition == endPosition) //获取日期大于昨天的并且标签号或结束位置等于传入值的任务
            &&
             //已下发到表 或者 执行中
             (u.SendState.Length == 0 ||
             (u.SendState.Length > 0

             && !(u.SendState == StockState.SendState_Success
                && u.RunState == StockState.RunState_Success)

             && u.RunState != StockState.RunState_RunFail
             && u.RunState != StockState.RunState_SendFail
             && u.RunState != StockState.RunState_Cancel)), true, DbMainSlave.Master);
            if (runQ > 0)
            {

                //如果有则调度失败
                result.SetError(StockResult.MovestockError_TrayInMissionError);
                return result;
            }

            //拿到需要放置的结束仓位，终点

            WareLocation endWl = _wareLocationService.GetIQueryable(u => u.WareLocaNo == endPosition,
                true, DbMainSlave.Master).FirstOrDefault();
            if (endWl == null)
            {
                //没有这个仓位
                result.SetError(StockResult.MovestockError_FindEndWLSRError);
                return result;
            }

            if (endWl.WareLocaState == WareLocaState.PreIn || endWl.WareLocaState == WareLocaState.PreOut)
            {
                //预进预出
                result.SetError(StockResult.MovestockError_EndWLIsUseError);
                return result;
            }

            if (endWl.WareLocaState == WareLocaState.HasTray)
            {
                result.SetError(StockResult.InstockError_EndWLHasTrayError);
                return result;
            }

            //拿起点的仓位
            TiShengJiInfo startWl = _tiShengJiInfoService.GetIQueryable(u => u.TsjName == startPosition,
                true, DbMainSlave.Master).FirstOrDefault();

            if (startWl == null)
            {

                result.SetError(StockResult.MovestockError_FindEndWLSRError);
                return result;
            }

            string mark = string.Empty;
            string trayNoToMission = string.Empty;
            if (string.IsNullOrEmpty(TrayNo)) //条码为空
            {
                if (missionType == GoodType.EmptyTray)//表示空托上下线
                {
                    trayNoToMission = "空托";

                    if (endWl.WareArea.WareAreaClass.Remark == WareAreaType.EmptyArea)
                        mark = MissionType.MoveOutNull_TSJ; //移动到空托区
                }

            }


            AGVMissionInfo agvMission = new AGVMissionInfo();
            agvMission.MissionNo = _liuShuiHaoService.GetAGVMissionNoLSH(); //流水号
            agvMission.Reserve1 = processName;
            // agvMission.Reserve2 = startWl.TsjName;

            agvMission.TrayNo = trayNoToMission;
            agvMission.StartPosition = startWl.TsjName;
            agvMission.StartLocation = startWl.TsjPosition_1F;

            agvMission.StartMiddlePosition = startWl.TsjName;//具体仓位
            agvMission.StartMiddleLocation = startWl.TsjPosition_1F;

            agvMission.EndMiddlePosition = endWl.WareLocaNo;
            agvMission.EndMiddleLocation = endWl.AGVPosition;

            agvMission.EndPosition = endWl.WareLocaNo;
            agvMission.EndLocation = endWl.AGVPosition;

            agvMission.Mark = mark;
            agvMission.OrderTime = DateTime.Now;
            agvMission.OrderGroupId = string.Empty;
            agvMission.AGVCarId = string.Empty;
            agvMission.userId = userID;
            agvMission.IsFloor = 0;
            if (agvMission.IsFloor == 0)
                agvMission.WHName = endWl.WareArea.WareHouse.WHName;//如果同层则传入仓库名
            agvMission.SendState = string.Empty;
            agvMission.RunState = string.Empty;
            agvMission.SendMsg = string.Empty;
            agvMission.StateMsg = string.Empty;
            agvMission.ModelProcessCode =
                 //agvMission.IsFloor == 0 ?
                 ////(startWls[0].AGVPosition.StartsWith("1") ? "711" : "2")
                 //startWl.WareArea.WareHouse.AGVModelCode :
                 string.Empty;
            if (remark != string.Empty)
                agvMission.Remark = remark; //给备注传值

            if (_missionService.Add(agvMission))
            {
                Logger.Default.Process(new Log(LevelType.Info, $"成功下发任务{agvMission.MissionNo}"));
                TrayState trayState = null;
                if (!string.IsNullOrEmpty(TrayNo))//起始标签不为空表示下线
                {

                    trayState = _trayStateService.GetByTrayNo(TrayNo);
                    //绑定起始位置
                    //startWl.TrayState = taryState;
                    //_wareLocationTrayNoManager.ChangeTrayWareLocation(0, startWl, taryState);
                }
                //对warelocation表的   更新起点终点库位状态
                WareLoactionLockHis wareLoactionLockHisEnd = new WareLoactionLockHis();
                wareLoactionLockHisEnd.WareLocaNo = endWl.WareLocaNo;
                wareLoactionLockHisEnd.PreState = WareLocaState.PreIn;
                wareLoactionLockHisEnd.Locker = userID;
                DateTime lockTime = DateTime.Parse(
                              DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.F"));
                wareLoactionLockHisEnd.LockTime = lockTime;
                _wareLocationLockHisService.Insert(wareLoactionLockHisEnd);
                _wareLocationLockHisService.SaveChanges();
                _wareLocationService.UpdateByPlus(u => u.ID == endWl.ID,
                    u => new WareLocation
                    { WareLocaState = WareLocaState.PreIn, LockHis_ID = wareLoactionLockHisEnd.ID });

                //WareLoactionLockHis wareLoactionLockHisStart = new WareLoactionLockHis();
                //wareLoactionLockHisStart.WareLocaNo = startWl.WareLocaNo;
                //wareLoactionLockHisStart.PreState = WareLocaState.PreOut;
                //wareLoactionLockHisStart.Locker = userID;

                //wareLoactionLockHisStart.LockTime = lockTime;
                //_wareLocationLockHisService.Insert(wareLoactionLockHisStart);
                //_wareLocationLockHisService.SaveChanges();
                //if (trayState == null)
                //    _wareLocationService.UpdateByPlus(u => u.ID == startWl.ID,
                //       u => new WareLocation
                //       { WareLocaState = WareLocaState.PreOut, LockHis_ID = wareLoactionLockHisStart.ID });
                //else
                //{
                //    //trayState.WareLocation_ID=startWl.ID;
                //    _wareLocationService.UpdateByPlus(u => u.ID == startWl.ID,
                //          u => new WareLocation
                //          { WareLocaState = WareLocaState.PreOut, LockHis_ID = wareLoactionLockHisStart.ID, TrayState_ID = trayState.ID });


                //    _trayStateService.UpdateByPlus(u => u.ID == trayState.ID,
                //       u => new TrayState
                //       { WareLocation_ID = startWl.ID });

                //}
                result.SetOk();
                return result;
            }
            else
            {
                result.SetError(StockResult.MovestockError_WriteMissionError);
                return result;
            }
        }

       

        public ProcessTypeParam GetMissionType(string processName)
        {
            var dbBase = new DbBase<ProcessTypeParam>();
            return dbBase.GetIQueryable(u => u.processName == processName).FirstOrDefault();

        }


        public ProcessTypeParam GetMissionType(string processName, string startRemark, string endRemark)
        {
            var dbBase = new DbBase<ProcessTypeParam>();
            return dbBase.GetIQueryable(u => u.processName == processName && u.strartRemark == startRemark && u.endRemark == endRemark).FirstOrDefault();

        }

        public object ChangeWareLocation(PostWarelocation warelocation)
        {

            try
            {
                var names = warelocation.name;
                using (var redislock = redisHelper.CreateLock(names, TimeSpan.FromSeconds(10),
                 TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
                {
                    var warelocations = _wareLocationService.GetIQueryable(u => names == u.WareLocaNo, true, DbMainSlave.Master).FirstOrDefault();

                    if (warelocation == null)
                        return new { success = false, message = "未找到该库位" };
                    if (warelocations.WareLocaState == WareLocaState.PreIn || warelocations.WareLocaState == WareLocaState.PreOut)
                        return new { success = false, message = "该库位正在预进预出" };
                    if (warelocations.WareLocaState == warelocation.state)
                        return new { success = false, message = "该库位已被修改" };
                    warelocations.WareLocaState = warelocation.state;
                    _wareLocationService.Update(warelocations);
                    _wareLocationService.SaveChanges();
                    return new { success = true, message = "成功" };
                }
            }
            catch (Exception ee)
            {
                Logger.Default.Process(new Log(LevelType.Error, ee.ToString()));
                return new { success = false, message = "未知错误" };
            }
        }
    }
}
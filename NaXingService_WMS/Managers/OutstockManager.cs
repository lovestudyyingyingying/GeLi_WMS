using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity;
using NanXingService_WMS.Entity.InstockEntity;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Helper.WMS;
using NanXingService_WMS.Services;
using NanXingService_WMS.Services.WMS;
using NanXingService_WMS.Services.WMS.AGV;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace NanXingService_WMS.Managers
{
    public class OutstockManager
    {
        
        WareLocationService _wareLocationService;
        StockPlanService _stockPlanService;
        LiuShuiHaoService _liuShuiHaoService;
        AGVMissionService _agvMissionService;
        WareLocationLockHisService _wareLoactionLockHisService;
        StockRecordService _stockRecordService;
        public OutstockHelper outstockHelper;
        public OutstockManager(WareLocationService wareLocationService,
            StockPlanService stockPlanService, LiuShuiHaoService liuShuiHaoService
            , AGVMissionService agvMissionService, WareLocationLockHisService wareLoactionLockHisService
            , StockRecordService stockRecordService)
        {
            _wareLocationService = wareLocationService;
            _stockPlanService = stockPlanService;
            _liuShuiHaoService = liuShuiHaoService;
            _agvMissionService = agvMissionService;
            _wareLoactionLockHisService = wareLoactionLockHisService;
            _stockRecordService = stockRecordService;
            outstockHelper = new OutstockHelper(_wareLocationService, _wareLoactionLockHisService);
        }

        #region 获取仓库可用产品数量

        public List<StockProItem> GetPros(string position,bool isHuancun)
        {
            return outstockHelper.GetStockProItem(position, isHuancun);
        }

        #endregion

        

        #region 出仓写入数据库

        public RunResult<string> Outstock(StockPlan stockPlan)
        {
            bool ret = _stockPlanService.AddStockPlan(stockPlan);
            if (!ret)
                return RunResult<string>.False("添加仓存任务失败");

            List<WareLocation> startWls=outstockHelper.GetOutStockProWl(stockPlan);
            List<WareLocation> endWls=outstockHelper.GetTargetWls(stockPlan,startWls.Count);
            if (startWls.Count!=endWls.Count)
            {
                DataTable wareLockDt = _wareLoactionLockHisService.ClassToDataTable(typeof(WareLoactionLockHis));
                //回滚预出预进
                startWls.ForEach(item =>
                {
                    if (item.WareLoactionLockHis != null)
                    {
                        WareLoactionLockHis wareLoactionLockHis = item.WareLoactionLockHis;
                        wareLoactionLockHis.UnLockTime = DateTime.Now;
                        wareLockDt = _wareLoactionLockHisService.ParseInDataTable(wareLockDt, wareLoactionLockHis);
                    }

                    item.WareLocaState = WareLocaState.HasTray;
                });
                DataTable startDt=_wareLocationService.ConvertToDataTable(startWls);
                _wareLoactionLockHisService.BatchUpdateData(wareLockDt, "WareLoactionLockHis");
                _wareLocationService.BatchUpdateData(startDt,"WareLocation");

                wareLockDt.Clear();
                endWls.ForEach(item =>
                {
                    if (item.WareLoactionLockHis != null)
                    {
                        WareLoactionLockHis wareLoactionLockHis = item.WareLoactionLockHis;
                        wareLoactionLockHis.UnLockTime = DateTime.Now;
                        wareLockDt = _wareLoactionLockHisService.ParseInDataTable(wareLockDt, wareLoactionLockHis);
                    }
                    item.WareLocaState = WareLocaState.NoTray;
                });
                _wareLoactionLockHisService.BatchUpdateData(wareLockDt, "WareLoactionLockHis");

                DataTable endDt = _wareLocationService.ConvertToDataTable(endWls);
                _wareLocationService.BatchUpdateData(endDt, "WareLocation");
                return RunResult<string>.False($"获取的起点数量与终点数量不一致：{startWls.Count}-{endWls.Count}"); ;
            }

            AGVMissionInfo agvMission = null;
            DataTable dt = _agvMissionService.ClassToDataTable(typeof(AGVMissionInfo));
            for (int i = 0; i < startWls.Count; i++)
            {
                if (agvMission == null)
                {
                    agvMission = new AGVMissionInfo();
                    agvMission.Mark = stockPlan.mark;
                    agvMission.OrderTime = DateTime.Now;
                    agvMission.userId = stockPlan.planUser;
                    agvMission.StockPlan_ID = stockPlan.ID;
                    agvMission.OrderGroupId = string.Empty;
                    agvMission.AGVCarId = string.Empty;
                    agvMission.SendState = string.Empty;
                    agvMission.RunState = string.Empty;
                    agvMission.SendMsg = string.Empty;
                    agvMission.StateMsg = string.Empty;
                    agvMission.ModelProcessCode =string.Empty;
                }
                bool ret1 = startWls[i].WareArea.InstockRule == "优先使用小的仓位号";

                string lieName1 = startWls[i].WareLoca_Lie;
                var q0 = _wareLocationService.GetIQueryable(
                   u => u.WareLoca_Lie == lieName1
                   , true);

                WareLocation startMiddleWl = (ret1 ? q0.OrderByDescending(u =>
                    u.WareLoca_Index.Length).ThenByDescending(u=>u.WareLoca_Index) :
                    q0.OrderBy(u =>
                    u.WareLoca_Index.Length).ThenBy(u => u.WareLoca_Index))
                   .FirstOrDefault();

                bool ret2 = endWls[i].WareArea.InstockRule == "优先使用小的仓位号";
                string lieName2 = endWls[i].WareLoca_Lie;
                var q1 = _wareLocationService.GetIQueryable(
                   u => u.WareLoca_Lie == lieName2
                   , true);
                WareLocation endMiddleWl = (ret2? q1.OrderByDescending(u =>
                    u.WareLoca_Index.Length).ThenByDescending(u => u.WareLoca_Index) :
                    q1.OrderBy(u =>
                    u.WareLoca_Index.Length).ThenBy(u => u.WareLoca_Index))
                    .FirstOrDefault();
                agvMission.IsFloor =
                        (startWls[i].WareArea.WareHouse.WHName == 
                        endWls[i].WareArea.WareHouse.WHName)
                        ? 0 : 1;
                if(agvMission.IsFloor == 0)
                {
                    agvMission.ModelProcessCode = string.Empty;
                    agvMission.WHName = !string.IsNullOrEmpty(stockPlan.position)? stockPlan.position 
                        : startWls[i].WareArea.WareHouse.WHName;
                    if (agvMission.Mark == MissionType.MoveOut_TSJ)
                        agvMission.Mark = MissionType.MovestockType;
                    if (agvMission.WHName == "07一楼")
                    {
                        agvMission.OrderGroupId = endWls[i].WareLoca_Lie;
                    }
                }
                
                agvMission.MissionNo = _liuShuiHaoService.GetAGVMissionNoLSH();
                agvMission.TrayNo = startWls[i].TrayState.TrayNO;
                agvMission.StartLocation = startWls[i].AGVPosition;
                agvMission.StartPosition = startWls[i].WareLocaNo;

                agvMission.StartMiddlePosition = startMiddleWl.WareLocaNo;
                agvMission.StartMiddleLocation = startMiddleWl.AGVPosition;

                agvMission.EndMiddlePosition = endMiddleWl.WareLocaNo;
                agvMission.EndMiddleLocation = endMiddleWl.AGVPosition;
                agvMission.OrderGroupId = agvMission.IsFloor == 1
                    ? endWls[i].WareLoca_Lie:string.Empty;

                agvMission.EndLocation = endWls[i].AGVPosition;
                agvMission.EndPosition = endWls[i].WareLocaNo;
                dt =_agvMissionService.AddToDataTable(dt, agvMission);
            }
            _agvMissionService.AddMany(dt);
            return RunResult<string>.True();
        }
        #endregion


        #region 缓存批量手动出库

        public RunResult<string> HandHCOutStock(string batchNo,string userID)
        {
            var wareLocations = _wareLocationService.GetList(u => u.TrayState != null 
            && u.TrayState.batchNo == batchNo && u.WareArea.WareAreaClass.AreaClass==AreaClassType.HuanCunArea,
            false,DbMainSlave.Master);

            if (wareLocations==null || wareLocations.Count <= 0)
            {
                return RunResult<string>.False("缓存区中没有找到该批次");
            }
            List<BingWareTrayLog> loglist = new List<BingWareTrayLog>(wareLocations.Count);

            using (TransactionScope tran =new TransactionScope())
            {
                try
                {
                    List<StockRecord> list = new List<StockRecord>(wareLocations.Count);
                    
                    var trayStates = wareLocations.Select(u => u.TrayState).ToList();
                    foreach (var warelocation in wareLocations)
                    {
                        BingWareTrayLog bingWareTrayLog = new BingWareTrayLog();
                        _wareLoactionLockHisService.UnLock(warelocation);
                        var record= _stockRecordService.GetStockRecordByHand(warelocation.TrayState, warelocation.WareLocaNo,
                            userID, DateTime.Now, false);
                        list.Add(record);
                        bingWareTrayLog.trayNo = warelocation.TrayState.TrayNO;
                        bingWareTrayLog.trayStateID = warelocation.TrayState_ID??0;
                        bingWareTrayLog.changeWl = warelocation.WareLocaNo;
                        loglist.Add(bingWareTrayLog);
                        warelocation.WareLocaState = WareLocaState.NoTray;
                        warelocation.TrayState_ID = null;
                        warelocation.LockHis_ID = null;
                    }

                    _stockRecordService.InsertBulk(list.AsQueryable());
                    _stockRecordService.SaveChanges();
                    _wareLocationService.SaveChanges();

                    foreach (var trayState in trayStates)
                    {
                        trayState.WareLocation_ID = null;
                    }
                    _wareLocationService.SaveChanges();
                    
                    tran.Complete();
                   
                }
                catch (Exception ex)
                {
                    Logger.Default.Process(new Log(LevelType.Error, ex.ToString()));
                    return RunResult<string>.False("失败：\r\n"+ex.ToString());
                }
            }
            foreach (var traylog in loglist)
            {
                Logger.Default.Process(new Log(LevelType.Info,
                    $"{traylog.changeWl}:解绑货物{traylog.trayNo}:ID {traylog.trayStateID}"));
            }
            return RunResult<string>.True();
        }
        #endregion 缓存批量手动出库


        #region 成品批量手动出库


        #endregion 成品批量手动出库

    }
}

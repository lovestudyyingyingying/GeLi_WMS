using GeLi_Utils.Entity.StockEntity;
using GeLi_Utils.Entity.WareAreaEntity;
using GeLi_Utils.Services.WMS;
using GeLi_Utils.Utils.AGVUtils;
using GeLiData_WMS;
using GeLiData_WMS.Dao;
using GeLiData_WMSUtils;
using GeLiService_WMS;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Managers;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsSharp.Shared.Standard;

namespace GeLi_Utils.Threads.SameFloorThreads
{
    public class DryProductionLineThread
    {
       // WareHouse _wareHouse;
        AGVMissionService _missionService = new AGVMissionService();//AGV任务服务  （对AGVMissionInfo的增删改查）
        LiuShuiHaoService _liuShuiHaoService = new LiuShuiHaoService(); // 流水服务
        WareLocationService _wareLocationService = new WareLocationService(); //仓位位置服务
        TrayStateService _trayStateService = new TrayStateService();
        WareLocationLockHisService _wareLocationLockHisService = new WareLocationLockHisService(); //仓库锁服务
      
       

       // AGVOrderHelper aGVOrderHelper;

        //string waitRun = "等待执行";
        public MyTask myTask;
        //启动线程前要传入仓库名
        public DryProductionLineThread()
        {
            //var AGVServerIP = ConfigurationManager.AppSettings["AGVIPAndPort"].ToString();
            //_wareHouse = wareHouse;
            //aGVOrderHelper = new AGVOrderHelper(AGVServerIP);
            ////_agvMissionService = agvMissionService;
            myTask = new MyTask(new Action(Run),
                        3, true).StartTask();
        }

        public void Run()
        {
            DbBase<WareLocation> wareLocationDbBase = new DbBase<WareLocation>();
            BaseResult<string> baseResult = new BaseResult<string>();
            WareLocationTrayManager wareLocationTrayManager = new WareLocationTrayManager(_trayStateService,
            _wareLocationService, _wareLocationLockHisService);

            MovestockManager movestockManager = new MovestockManager(_missionService,
            _liuShuiHaoService, _wareLocationService, wareLocationTrayManager, _trayStateService, null,null);
            List<WareLocation> wareLocation = new List<WareLocation>();

            wareLocation = wareLocationDbBase.GetList(u => (u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.hotDryLine1 
            || u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.hotDryLine2 || u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.coldDryLine1 
            || u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.coldDryLine2 || u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.coldSprayLine 
            || u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.hotSprayLine) && u.WareLocaState == WareLocaState.NoTray, true, DbMainSlave.Master);

           var coldHotExpansionCache = wareLocationDbBase.GetList(u => (u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.hotExpansionCacheArea
            || u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.coldExpansionCacheArea) && u.WareLocaState == WareLocaState.HasTray, true, DbMainSlave.Master);

            WareLocation itemWareLocation = new WareLocation();
            DbBase<AGVMissionJumpQueue> aGVMissionJumpQueueDbBase = new DbBase<AGVMissionJumpQueue>();
            List<AGVMissionJumpQueue> aGVMissionJumpQueues = new List<AGVMissionJumpQueue>();
            
            aGVMissionJumpQueues = aGVMissionJumpQueueDbBase.GetList(u => u.IsSendSuccess == false&& (u.TargetArea == WareAreaEntity.hotDryLine1
            || u.TargetArea == WareAreaEntity.hotDryLine2 || u.TargetArea == WareAreaEntity.coldDryLine1
            || u.TargetArea == WareAreaEntity.coldDryLine2 || u.TargetArea == WareAreaEntity.coldSprayLine
            || u.TargetArea == WareAreaEntity.hotSprayLine), true, DbMainSlave.Master).OrderBy(u=>u.InsertTime).ToList();
            if (aGVMissionJumpQueues!=null && aGVMissionJumpQueues.Count!=0&& wareLocation!=null && wareLocation.Count!=0)
            {
                foreach (AGVMissionJumpQueue item in aGVMissionJumpQueues)
                {
                    AGVMissionJumpQueue aGVMissionJumpQueue = item;
                    itemWareLocation = wareLocation.Where(u => u.WareArea.WareAreaClass.AreaClass == item.TargetArea).FirstOrDefault();
                    if (itemWareLocation!=null)
                    {
                         baseResult = movestockManager.MoveIn_Su(item.TrayNo, item.StartPosition, itemWareLocation.WareLocaNo, item.userId, "", "", GoodType.GoodTray, item.Reserve1, "上线",null,null);
                        if (baseResult.Code == 200)
                        {
                            aGVMissionJumpQueue.IsSendSuccess = true;
                            aGVMissionJumpQueueDbBase.Update(aGVMissionJumpQueue);
                            aGVMissionJumpQueueDbBase.SaveChanges();
                        }
                        Logger.Default.Process(new Log(LevelType.Info,
                        $"DryProductionLineThread:处理插队任务{item.MissionNo}"+ baseResult.Msg));
                    }
                }

            }
            else if (wareLocation != null && wareLocation.Count != 0)
            {
                foreach (WareLocation item in wareLocation)
                {
                    WareLocation wareLocationUpate = item;
                    itemWareLocation = coldHotExpansionCache.Where(u => u.Reserve1 == item.WareArea.WareAreaClass.AreaClass).OrderBy(u => u.Reserve2).FirstOrDefault();
                    if (itemWareLocation != null)
                    {
                         baseResult =  movestockManager.MoveIn_Su(itemWareLocation.TrayState.proname, itemWareLocation.WareLocaNo, item.WareLocaNo,coldHotExpansionCache.FirstOrDefault().Reserve1+"线程", "", "", GoodType.GoodTray, item.Reserve1, "上线",null,null);
                        if (baseResult.Code == 200)
                        {
                            wareLocationUpate.Reserve1 = "";
                            wareLocationUpate.Reserve2 = "";
                            wareLocationUpate.TrayState_ID = null;
                            wareLocationDbBase.Update(wareLocationUpate);
                            wareLocationDbBase.SaveChanges();
                        }
                    }
                    Logger.Default.Process(new Log(LevelType.Info,
                        $"DryProductionLineThread:处理胀管缓存{itemWareLocation.TrayState.TrayNO}+{itemWareLocation.Reserve1}" + baseResult.Msg));
                }
            }
           
        }
    }
}

using GeLi_Utils.Entity.StockEntity;
using GeLi_Utils.Entity.TiShengJiEntity;
using GeLi_Utils.Entity.WareAreaEntity;
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
using GeLiService_WMS.Utils.RedisUtils;
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
    public class ColdHotCacheThread
    {
        TiShengJiInfo _tiShengJiInfo;
        AGVMissionService _missionService = new AGVMissionService();//AGV任务服务  （对AGVMissionInfo的增删改查）
        LiuShuiHaoService _liuShuiHaoService = new LiuShuiHaoService(); // 流水服务
        WareLocationService _wareLocationService = new WareLocationService(); //仓位位置服务
        TrayStateService _trayStateService = new TrayStateService();
        WareLocationLockHisService _wareLocationLockHisService = new WareLocationLockHisService(); //仓库锁服务


        AGVOrderHelper agvOrderHelpers;
       // ReckonSecondUtils reckonSecondUtils = new ReckonSecondUtils();

       
        TiShengJiRunRecordService tiShengJiRunRecordService = new TiShengJiRunRecordService();
       // RabbitMQUtils rabbitMqUtils = new RabbitMQUtils();
        RedisHelper redisHelper = new RedisHelper();
        DateTime dtime = DateTime.Now;
        DateTime firstTime = DateTime.Now;
        string runningKey = string.Empty;
        bool stoped = false;
        //AGVOrderHelper aGVOrderHelper;

        //string waitRun = "等待执行";
        public MyTask myTask;
        //启动线程前要传入仓库名
        public ColdHotCacheThread(TiShengJiInfo tiShengJiInfo)
        {
            //var AGVServerIP = ConfigurationManager.AppSettings["AGVIPAndPort"].ToString();
            _tiShengJiInfo = tiShengJiInfo;
            //aGVOrderHelper = new AGVOrderHelper(AGVServerIP);
            //_agvMissionService = agvMissionService;
            myTask = new MyTask(new Action(Run),
                        3, true).StartTask();
        }

        public void Run()
        {
            if (_tiShengJiInfo!= null&& _tiShengJiInfo.TiShengJiState!=null&& _tiShengJiInfo.TiShengJiState.F1DuiJieWei == TiShengJiStateEntity.AllowUpMission)
            {
                DbBase<WareLocation> wareLocationDbBase = new DbBase<WareLocation>();
                BaseResult<string> baseResult = new BaseResult<string>();
                WareLocationTrayManager wareLocationTrayManager = new WareLocationTrayManager(_trayStateService,
                _wareLocationService, _wareLocationLockHisService);

                MovestockManager movestockManager = new MovestockManager(_missionService,
                _liuShuiHaoService, _wareLocationService, wareLocationTrayManager, _trayStateService, null, null);

                List<WareLocation> wareLocation = new List<WareLocation>();

                wareLocation = wareLocationDbBase.GetList(u => (u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.coldCache
                || u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.hotCache) && u.WareLocaState == WareLocaState.HasTray, true, DbMainSlave.Master);

               

              
                DbBase<AGVMissionJumpQueue> aGVMissionJumpQueueDbBase = new DbBase<AGVMissionJumpQueue>();
                List<AGVMissionJumpQueue> aGVMissionJumpQueues = new List<AGVMissionJumpQueue>();

                aGVMissionJumpQueues = aGVMissionJumpQueueDbBase.GetList(u => u.IsSendSuccess == false && (u.TargetArea == WareAreaEntity.coldCache
                || u.TargetArea == WareAreaEntity.hotCache), true, DbMainSlave.Master).OrderBy(u => u.InsertTime).ToList();
                if (aGVMissionJumpQueues != null && aGVMissionJumpQueues.Count != 0)
                {
                    foreach (AGVMissionJumpQueue item in aGVMissionJumpQueues)
                    {
                        AGVMissionJumpQueue aGVMissionJumpQueue = item;
                        
                        if (_tiShengJiInfo.TiShengJiState.F1DuiJieWei == TiShengJiStateEntity.AllowUpMission)
                        {
                            baseResult = movestockManager.MoveIn_Su(item.TrayNo, item.StartPosition, _tiShengJiInfo.TsjName, item.userId, "", "", GoodType.GoodTray, item.Reserve1, "上线", _tiShengJiInfo, tiShengJiRunRecordService);
                            if (baseResult.Code == 200)
                            {
                                aGVMissionJumpQueue.IsSendSuccess = true;
                                aGVMissionJumpQueueDbBase.Update(aGVMissionJumpQueue);
                                aGVMissionJumpQueueDbBase.SaveChanges();
                            }
                            Logger.Default.Process(new Log(LevelType.Info,
                            $"ColdHotCacheThread:处理插队任务{item.MissionNo}" + baseResult.Msg));
                        }
                    }

                }
                else if (wareLocation != null && wareLocation.Count != 0)
                {
                    foreach (WareLocation item in wareLocation)
                    {
                        WareLocation wareLocationUpate = item;
                        if (_tiShengJiInfo.TiShengJiState.F1DuiJieWei == TiShengJiStateEntity.AllowUpMission)
                        {
                            baseResult = movestockManager.MoveIn_Su(item.TrayState.TrayNO, item.WareLocaNo, _tiShengJiInfo.TsjName, item.Reserve1 + "线程", "", "", GoodType.GoodTray, item.Reserve1, "上线", _tiShengJiInfo,tiShengJiRunRecordService);
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
                            $"ColdHotCacheThread:处理冷热缓存{item.TrayState.TrayNO}+{item.Reserve1}" + baseResult.Msg));
                    }
                }
            }
        }

        
    }
}

using GeLi_Utils.Entity.StockEntity;
using GeLi_Utils.Entity.TiShengJiEntity;
using GeLi_Utils.Entity.WareAreaEntity;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsSharp.Shared.Standard;

namespace GeLi_Utils.Threads.SameFloorThreads
{
    public class ColdHotNitrogenOnLineThread
    {
        TiShengJiInfo _tiShengJiInfo;
        AGVMissionService _missionService = new AGVMissionService();//AGV任务服务  （对AGVMissionInfo的增删改查）
        LiuShuiHaoService _liuShuiHaoService = new LiuShuiHaoService(); // 流水服务
        WareLocationService _wareLocationService = new WareLocationService(); //仓位位置服务
        TrayStateService _trayStateService = new TrayStateService();
        WareLocationLockHisService _wareLocationLockHisService = new WareLocationLockHisService(); //仓库锁服务


        //AGVOrderHelper agvOrderHelpers;
        // ReckonSecondUtils reckonSecondUtils = new ReckonSecondUtils();
       

        TiShengJiRunRecordService tiShengJiRunRecordService = new TiShengJiRunRecordService();
        
        // RabbitMQUtils rabbitMqUtils = new RabbitMQUtils();
        //RedisHelper redisHelper = new RedisHelper();
        //DateTime dtime = DateTime.Now;
        //DateTime firstTime = DateTime.Now;
        //string runningKey = string.Empty;
        //bool stoped = false;
        //AGVOrderHelper aGVOrderHelper;

        //string waitRun = "等待执行";
        public MyTask myTask;
        //启动线程前要传入仓库名
        public ColdHotNitrogenOnLineThread(TiShengJiInfo tiShengJiInfo)
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
            if (_tiShengJiInfo != null && _tiShengJiInfo.TiShengJiState != null && _tiShengJiInfo.TiShengJiState.F2DuiJieWei == TiShengJiStateEntity.SecFloorHadGood)
            {
                DbBase<WareLocation> wareLocationDbBase = new DbBase<WareLocation>();
                BaseResult<string> baseResult = new BaseResult<string>();
                WareLocationTrayManager wareLocationTrayManager = new WareLocationTrayManager(_trayStateService,
                _wareLocationService, _wareLocationLockHisService);

                MovestockManager movestockManager = new MovestockManager(_missionService,
                _liuShuiHaoService, _wareLocationService, wareLocationTrayManager, _trayStateService, null, null);

                List<WareLocation> wareLocation = new List<WareLocation>();

                wareLocation = wareLocationDbBase.GetList(u => (u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.coldNitrogenLine1
                || u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.coldNitrogenLine2 || u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.hotNitrogenLine1
                || u.WareArea.WareAreaClass.AreaClass == WareAreaEntity.hotNitrogenLine2) && u.WareLocaState == WareLocaState.NoTray, true, DbMainSlave.Master);

                TiShengJiRunRecord tiShengJiRunRecord = new TiShengJiRunRecord();
                tiShengJiRunRecord = tiShengJiRunRecordService.GetList(u => u.Reserve1 == "0", true, DbMainSlave.Master).OrderBy(u => u.OrderTime).FirstOrDefault() ;
                if (wareLocation != null && wareLocation.Count != 0 && _tiShengJiInfo.TiShengJiState.F2DuiJieWei == TiShengJiStateEntity.SecFloorHadGood)
                {
                    foreach (WareLocation item in wareLocation)
                    {
                        
                        if (_tiShengJiInfo.TiShengJiState.F2DuiJieWei == TiShengJiStateEntity.SecFloorHadGood&& tiShengJiRunRecord!= null&& tiShengJiRunRecord.Reserve2 == item.Reserve1)
                        {
                            baseResult = movestockManager.MoveIn_Su(tiShengJiRunRecord.InsideTrayNo, tiShengJiRunRecord.TsjName, item.WareLocaNo, tiShengJiRunRecord.Reserve2 + "线程", "", "", GoodType.GoodTray, item.Reserve1, "上线",null,null);
                            if (baseResult.Code == 200)
                            {
                                tiShengJiRunRecord.Reserve1 = "1";
                                tiShengJiRunRecordService.Update(tiShengJiRunRecord);
                                tiShengJiRunRecordService.SaveChanges();
                            }
                        }
                        Logger.Default.Process(new Log(LevelType.Info,
                            $"ColdHotNitrogenOnLineThread:处理冷热氮检上线{tiShengJiRunRecord.InsideTrayNo}+{tiShengJiRunRecord.Reserve2}" + baseResult.Msg));
                    }
                }
            }

        }
    }
}

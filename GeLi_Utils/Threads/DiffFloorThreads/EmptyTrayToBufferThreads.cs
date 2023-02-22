using GeLi_Utils.Entity.StockEntity;
using GeLi_Utils.Entity.TiShengJiEntity;
using GeLi_Utils.Helpers;
using GeLi_Utils.Utils.AGVUtils;
using GeLiData_WMS;
using GeLiData_WMSUtils;
using GeLiService_WMS;
using GeLiService_WMS.Entity.AGVOrderEntity;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilsSharp.Shared.Standard;

namespace GeLi_Utils.Threads.DiffFloorThreads
{
    /// <summary>
    /// 空托盘运输到托盘缓冲区服务
    /// </summary>
    public class EmptyTrayToBufferThreads
    {

        TiShengJiInfo _tiShengJiInfo;

        public MyTask runTask;
        public MyTask stepTwofailTask;
       

       AGVOrderHelper agvOrderHelpers;

        
        ReckonSecondUtils reckonSecondUtils = new ReckonSecondUtils();
        LiuShuiHaoService liuShuiHaoService = new LiuShuiHaoService();
        AGVMissionFloorService floorService = new AGVMissionFloorService();
        AGVMissionService missionService = new AGVMissionService();
        WareLocationService  wareLocationService = new WareLocationService();
        TiShengJiInfoService tiShengJiInfoService = new TiShengJiInfoService();

        MovestockManager movestockManager = null;
       


        TrayStateService trayStateService = new TrayStateService();
        TiShengJiRunRecordService tiShengJiRunRecordService = new TiShengJiRunRecordService();
        RabbitMQUtils rabbitMqUtils = new RabbitMQUtils();
        RedisHelper redisHelper = new RedisHelper();
        DateTime dtime = DateTime.Now;
        DateTime firstTime = DateTime.Now;
        string runningKey = string.Empty;
        bool stoped = false;
        //Expression<Func<AGVMissionInfo_Floor, bool>> run_expression;
        public EmptyTrayToBufferThreads(TiShengJiInfo tiShengJiInfo)
        {
            var AGVServerIP = ConfigurationManager.AppSettings["AGVIPAndPort"].ToString();
            _tiShengJiInfo = tiShengJiInfo;
            runningKey = $"IsEmptyTrayToBufferTaskStop:{_tiShengJiInfo.TsjName}";
            agvOrderHelpers = new AGVOrderHelper(AGVServerIP);
            //状态获取
            // tiShengJiHelper = new TiShengJiHelper(tiShengJiInfo);
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
                            $"{_tiShengJiInfo.TsjName}空托盘运输到托盘缓冲区层线程已暂停"));
                        stoped = true;
                    }
            

                    Thread.Sleep(5000);
                    return;
                }
                if (stoped)
                {
                    Logger.Default.Process(new Log(LevelType.Info,
                           $"{_tiShengJiInfo.TsjName}空托盘运输到托盘缓冲区线程已启动"));
                    stoped = false;
                }
                dtime = DateTime.Now.AddDays(-1);

                //空托盘运输到托盘缓冲区
                EmptyTrayToBuffer1F();
               

            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                       $"EmptyTrayToBufferThreads:\r\n{ex.ToString()}"));
            }
        }
        private void EmptyTrayToBuffer1F()
        {
            using (redisHelper.CreateLock($"Lock_EmptyTray{_tiShengJiInfo.TsjName}",
                TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(30),
                TimeSpan.FromMilliseconds(500)))
            {
                List<TiShengJiInfo> TiShengJiInfo1s = new List<TiShengJiInfo>();
                DbBase<TiShengJiInfo> TiShengJiInfoDbBase = new DbBase<TiShengJiInfo>();
                //List <AGVMissionInfo> kongTuoAGVMissionInfos = new List<AGVMissionInfo>();
                List<AGVMissionInfo> AGVMissionInfos = new List<AGVMissionInfo>();

                AGVMissionInfos = missionService.GetList(u => u.IsFloor == 0 && u.RunState != StockState.RunState_Success
                && u.RunState != StockState.RunState_Cancel && u.RunState != StockState.RunState_Error && (u.SendState == StockState.SendState_Success || u.SendState == string.Empty)
                && u.RunState != StockState.RunState_RunFail && u.RunState != StockState.RunState_SendFail
                && u.Mark == MissionType.MoveOutNull_TSJ
                , true, DbMainSlave.Master);
                TiShengJiInfo1s = TiShengJiInfoDbBase.GetAll();
                if (TiShengJiInfo1s != null)
                {
                    foreach (var item in TiShengJiInfo1s)
                    {
                        //对每一台提升机进行判断是否能发起AGV搬运空托任务。
                        if (item.TiShengJiState == null)
                        {
                            return;
                        }
                        if (item.TiShengJiState.F1DuiJieWei == TiShengJiStateEntity.OneFloorHadGood)
                        {
                            if (AGVMissionInfos != null && AGVMissionInfos.Count == 0)
                            {
                                movestockManager = new MovestockManager(missionService, liuShuiHaoService, wareLocationService, tiShengJiInfoService);

                                List<WareLocation> wareLocations = movestockManager.GetWls(EmptyTrayToBufferType.GeLi_1Lou, EmptyTrayToBufferType.KongTuo).Where(u => u.WareLocaState == EmptyTrayToBufferType.WareLocation_NULL).OrderBy(u => u.ID).ToList();
                                if (wareLocations != null && wareLocations.Count == 0)
                                {
                                    continue;
                                }
                                BaseResult<string> baseResult = movestockManager.MoveOutTiShengJi(null, item.TsjName, wareLocations.FirstOrDefault().WareLocaNo, EmptyTrayToBufferType.UserID, null, null, GoodType.EmptyTray, EmptyTrayToBufferType.processName, null);
                                // OrderResult result = agvOrderHelpers.SendOrder(kongTuoAGVMissionone);
                                Logger.Default.Process(new Log(LevelType.Info, item.TsjName + "空托搬运到缓存区执行：" + baseResult.Code.ToString() + ":" + baseResult.Msg.ToString()));

                            }
                        }



                    }

                }

            }
            
        }

       
    }
}

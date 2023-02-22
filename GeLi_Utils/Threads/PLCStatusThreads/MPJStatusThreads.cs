using GeLi_Utils.Entity.MaPanJiStateEntity;
using GeLi_Utils.Entity.StockEntity;
using GeLi_Utils.Helpers;
using GeLi_Utils.Services.WMS;
using GeLiData_WMS;
using GeLiData_WMS.Dao;
using GeLiData_WMSUtils;
using GeLiService_WMS;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Utils;
using GeLiService_WMS.Utils.RedisUtils;
using GeLiService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsSharp.Shared.Standard;

namespace GeLi_Utils.Threads.PLCStatusThreads
{
    public class MPJStatusThreads
    {
        MaPanJiInfo _maPanJiInfo;
        //ConcurrentQueue<AGVMissionInfo> concurrentQueue = new ConcurrentQueue<AGVMissionInfo>();
        // AGVMissionService _agvMissionService = new AGVMissionService();
       // AGVOrderHelper aGVOrderHelper;
        //string waitRun = "等待执行";
        public MyTask myTask;
        ReckonSecondUtils reckonSecondUtils = new ReckonSecondUtils();
        LiuShuiHaoService liuShuiHaoService = new LiuShuiHaoService();
        AGVMissionFloorService floorService = new AGVMissionFloorService();
        AGVMissionService missionService = new AGVMissionService();
        MaPanJiInfoService _maPanJiInfoService = new MaPanJiInfoService();
        MaPanJiStateService _maPanJiStateService = new MaPanJiStateService();
        WareLocationService wareLocationService = new WareLocationService();
        MovestockManager movestockManager = null;
        RedisHelper redisHelper = new RedisHelper();

        //启动线程前要传入仓库名
        public MPJStatusThreads(MaPanJiInfo maPanJiInfo)
        {
            _maPanJiInfo = maPanJiInfo;
           // aGVOrderHelper = new AGVOrderHelper("http://" + _maPanJiInfo.AGVServerIP);
            //_agvMissionService = agvMissionService;
            myTask = new MyTask(new Action(Run),
                        3, true).StartTask();
        }

        public void Run()
        {
            try
            {
                using (redisHelper.CreateLock($"OutKongTuoMaPanJiLock:{_maPanJiInfo.MpjIp}:{_maPanJiInfo.MpjPort}",
                 TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(30),
                 TimeSpan.FromMilliseconds(500)))
                {
                    MaPanJiInfo list = _maPanJiInfoService.GetList(u=>u.MpjName==_maPanJiInfo.MpjName, true, DbMainSlave.Master).FirstOrDefault();
                    DateTime dtime = DateTime.Now.AddDays(-110);
                    //查找当天前24小时同层的任务，提升机当作仓库库位
                    List<MaPanJiState> aGVAlarmLogList = new List<MaPanJiState>();
                    DbBase<MaPanJiState> aGVAlarmLogdbBase = new DbBase<MaPanJiState>();
                    var ManPanmissionAll = missionService.GetList(u => u.Mark == MissionType.MoveOutMaPanJi && u.SendState == StockState.SendState_Success
                     && u.RunState != StockState.RunState_Success
                         && u.RunState != StockState.RunState_Error && u.RunState != StockState.RunState_Cancel
                         && u.RunState != StockState.RunState_RunFail && u.RunState != StockState.RunState_SendFail);
                    if (list != null)
                    {
                        MaPanJiHelper maPanJiHelper = new MaPanJiHelper(list.MpjIp, list.MpjPort);
                        maPanJiHelper.GetAndSaveState();

                        if (list.MaPanJiState != null && list.MaPanJiState.Reserve1 == MaPanJiStateSummarize.FullEmptyTray && ManPanmissionAll != null & ManPanmissionAll.Count == 0)
                        {

                            movestockManager = new MovestockManager(missionService, liuShuiHaoService, wareLocationService, null, null, _maPanJiInfoService, _maPanJiStateService);

                            List<WareLocation> wareLocations = movestockManager.GetWls(EmptyTrayToBufferType.GeLi_2Lou, EmptyTrayToBufferType.AllKongTuo).Where(u => u.WareLocaState == EmptyTrayToBufferType.WareLocation_NULL).OrderBy(u => u.ID).ToList();
                            if (wareLocations != null && wareLocations.Count == 0 || wareLocations == null)
                            {
                                return;
                            }

                            BaseResult<string> baseResult = movestockManager.MoveOutMaPanJi(null, list.MpjName, wareLocations.OrderBy(u => u.ID).FirstOrDefault().WareLocaNo, EmptyTrayToBufferType.UserID, null, null, GoodType.EmptyTray, EmptyTrayToBufferType.processName, null);

                            Logger.Default.Process(new Log(LevelType.Info, list.MpjName + "空托搬运到缓存区执行：" + baseResult.Code.ToString() + ":" + baseResult.Msg.ToString()));

                        }
                    }

                }
            }
            catch (Exception ex)
            {

                Logger.Default.Process(new Log(LevelType.Error,
                             ex.ToString()));
            }

            // MaPanJiHelper
        }
    }
}

using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Helper.WMS;
using NanXingService_WMS.Services;
using NanXingService_WMS.Services.WMS;
using NanXingService_WMS.Services.WMS.AGV;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Managers
{
    public class InstockManager
    {
        TrayStateService _trayStateService;
        WareLocationService _wareLocationService;
        LiuShuiHaoService _liuShuiHaoService;
        AGVMissionService _agvMissionService;
        RedisHelper redisHelper = new RedisHelper();
        //StockPlanService _stockPlanService;
        WareLocationLockHisService _wareLoactionLockHisService;
        InstockHelper instockHelper;
        public InstockManager(TrayStateService trayStateService,
            WareLocationService wareLocationService, LiuShuiHaoService liuShuiHaoService
            , AGVMissionService agvMissionService,WareLocationLockHisService wareLoactionLockHisService)
        {
            _trayStateService = trayStateService;
            _wareLocationService = wareLocationService;
            _liuShuiHaoService = liuShuiHaoService;
            _agvMissionService = agvMissionService;
            _wareLoactionLockHisService = wareLoactionLockHisService;
            instockHelper = new InstockHelper(_wareLocationService, wareLoactionLockHisService);
        }

        /// <summary>
        /// 条码获取产品信息
        /// </summary>
        /// <param name="trayNo">托盘条码</param>
        /// <returns></returns>
        public TrayState GetInfoByTrayNo(string trayNo)
        {
            return _trayStateService.GetIQueryable(u => u.TrayNO == trayNo,true,DbMainSlave.Master
                ).FirstOrDefault();
        }


        /// <summary>
        /// 获取楼层入库位数据
        /// </summary>
        /// <param name="wlNo">仓位号<param>
        /// <param name="position">楼层</param>
        /// <returns></returns>
        public List<WareLocation> GetInStartWls(string wlNo, string postion)
        {
            //当仓位号不为空则直接获取该仓位号仓位信息
            //当仓位号为空，则获取楼层的所有入库位信息
            List<WareLocation> list = !string.IsNullOrEmpty(wlNo) ?
                _wareLocationService.GetList(u =>
                u.IsOpen == 1 && u.WareLocaNo == wlNo
                && u.WareArea.WareAreaClass.AreaClass == AreaClassType.RuKuArea, true)
                : _wareLocationService.GetList(u =>
                u.IsOpen == 1 && u.WareArea.WareHouse.WHName == postion
                && u.WareArea.WareAreaClass.AreaClass == AreaClassType.RuKuArea, true);
            return list;
        }

        //获取目标仓位
        public WareLocation GetWL(string trayNo, string position,string user)
        {
            TrayState ts = GetInfoByTrayNo(trayNo);
            return instockHelper.GetInstockWL(ts.batchNo, ts.proname, position, user, false);
        }

        /// <summary>
        /// 入库操作
        /// </summary>
        /// <param name="startNo">起始仓位位置</param>
        /// <param name="TrayNo">入库托盘号</param>
        /// <param name="userID">操作人</param>
        /// <param name="positionID">目标仓库</param>
        /// <param name="remark">备注：string.Empty 或 回转测试</param>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="endNo">终点仓位位置，如果为string.Empty则自动寻找仓位</param>
        /// <param name="isHuiZhuan">是否回转测试</param>
        /// <returns></returns>
        public bool Instock(string startNo, string TrayNo, string userID,
            string position, string remark, ref string errorMsg, string endNo = "", bool isHuiZhuan = false)
        {

            List<WareLocation> startWls = null;
            if (!isHuiZhuan)
                startWls = GetInStartWls(startNo, position);
            else
                startWls = _wareLocationService.GetIQueryable(u=>u.WareLocaNo==startNo,
                    true, DbMainSlave.Master).ToList();
            if (startWls == null || startWls.Count == 0)
            {
                errorMsg = StockResult.InstockError_StartWLError;
                return false;
            }
            TrayState ts = GetInfoByTrayNo(TrayNo);
            
            //仓位表查询、任务表查询
            if (!isHuiZhuan && ts.WareLocation!=null)
            {
                errorMsg = StockResult.InstockError_TrayHasInError;
                return false;
            }
            DateTime dateTime = DateTime.Now.AddDays(-1);
            //判断该条码是否在未完成的任务中
            AGVMissionInfo missionInfo = _agvMissionService.GetIQueryable(
                u => u.OrderTime >= dateTime && u.TrayNo == TrayNo && 
                !(u.RunState == StockState.RunState_Success || u.RunState == StockState.RunState_Cancel
                || u.RunState == StockState.RunState_RunFail || u.RunState == StockState.RunState_SendFail)
                ,true).FirstOrDefault();
            if (missionInfo != null)
            {
                errorMsg = StockResult.InstockError_TrayInMissionError;
                return false;
            }
            //判断目标仓位是否存在、是否有产品
            WareLocation endWl =
                endNo == string.Empty ?
                instockHelper.GetInstockWL(ts.batchNo, ts.proname, position, userID, true)
                : _wareLocationService.GetByWLNo(endNo, true);
           
            if (endWl == null)
            {
                errorMsg = endNo == string.Empty ?
                    StockResult.InstockError_FindEndWLAutoError
                    : StockResult.InstockError_FindEndWLSRError;
                return false;
            }
            else if (endWl.TrayState_ID != null)
            {
                errorMsg = StockResult.InstockError_EndWLHasTrayError;
                return false;
            }

            if (endWl.WareLocaState == WareLocaState.PreIn
                     || endWl.WareLocaState == WareLocaState.PreOut)
            {
                errorMsg = StockResult.InstockError_EndWLIsUseError;
                return false;
            }
            if (endWl.WareLocaState!= WareLocaState.PreIn)
            {
                string lieStr = $"{WareLocationService.wareLocker}:{endWl.WareLoca_Lie}";
                using (var redislock = redisHelper.CreateLock(lieStr, TimeSpan.FromSeconds(10),
                   TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
                {
                    WareLoactionLockHis wareLoactionLockHis = new WareLoactionLockHis();
                    wareLoactionLockHis.WareLocaNo = endWl.WareLocaNo;
                    wareLoactionLockHis.PreState = WareLocaState.PreIn;
                    wareLoactionLockHis.LockTime = DateTime.Now;
                    wareLoactionLockHis.Locker = userID;
                    _wareLoactionLockHisService.Insert(wareLoactionLockHis);
                    _wareLoactionLockHisService.SaveChanges();
                    _wareLocationService.UpdateByPlus(u => u.ID == endWl.ID,
                              u => new WareLocation
                              {
                                  WareLocaState = WareLocaState.PreIn,
                                  BatchNo = ts.batchNo,
                                  LockHis_ID = wareLoactionLockHis.ID
                              });
                }
                  
            }


            bool ret1 = startWls[0].WareArea.InstockRule == "优先使用小的仓位号";
            string lieName = startWls[0].WareLoca_Lie;
            var q0 = _wareLocationService.GetIQueryable(
               u => u.WareLoca_Lie == lieName
               , true);

            WareLocation startMiddleWl = (ret1 ? q0.OrderByDescending(u =>
                u.WareLoca_Index.Length).ThenByDescending(u => u.WareLoca_Index) :
                q0.OrderBy(u =>
                u.WareLoca_Index.Length).ThenBy(u => u.WareLoca_Index))
               .FirstOrDefault();

            bool ret2 = endWl.WareArea.InstockRule == "优先使用小的仓位号";
            var q1 = _wareLocationService.GetIQueryable(
               u => u.WareLoca_Lie == endWl.WareLoca_Lie
               , true);
            WareLocation endMiddleWl = (ret2 ? q1.OrderByDescending(u =>
                 u.WareLoca_Index.Length).ThenByDescending(u => u.WareLoca_Index) :
                q1.OrderBy(u =>
                u.WareLoca_Index.Length).ThenBy(u => u.WareLoca_Index))
                .FirstOrDefault();

            AGVMissionInfo agvMission = new AGVMissionInfo();
            agvMission.MissionNo = _liuShuiHaoService.GetAGVMissionNoLSH();
            agvMission.TrayNo = TrayNo;
            agvMission.StartPosition = startNo;
            agvMission.StartLocation = startWls[0].AGVPosition;

            agvMission.StartMiddlePosition = startMiddleWl.WareLocaNo;
            agvMission.StartMiddleLocation = startMiddleWl.AGVPosition;

            agvMission.EndMiddlePosition = endMiddleWl.WareLocaNo;
            agvMission.EndMiddleLocation = endMiddleWl.AGVPosition;

            agvMission.EndPosition = endWl.WareLocaNo;
            agvMission.EndLocation = endWl.AGVPosition;
            
            agvMission.Mark = !isHuiZhuan ? MissionType.InstockType
                : MissionType.MovestockType;
            agvMission.OrderTime = DateTime.Now;
            
            agvMission.AGVCarId = string.Empty;
            agvMission.userId = userID;
            agvMission.IsFloor =
                (startWls[0].WareArea.WareHouse.WHName
                == endWl.WareArea.WareHouse.WHName)
                ? 0 : 1;
            if (agvMission.IsFloor == 0)
                agvMission.WHName = startWls[0].WareArea.WareHouse.WHName;
            agvMission.OrderGroupId = agvMission.IsFloor==0
                ?string.Empty: endWl.WareLoca_Lie;
            agvMission.SendState = string.Empty;
            agvMission.RunState = string.Empty;
            agvMission.SendMsg = string.Empty;
            agvMission.StateMsg = string.Empty;
            agvMission.ModelProcessCode =
                //agvMission.IsFloor == 0 ?
                ////(startWls[0].AGVPosition.StartsWith("1") ? "711" : "2")
                //startWls[0].WareArea.WareHouse.AGVModelCode :
                 string.Empty;
            if (remark != string.Empty)
                agvMission.Remark = remark;

            if (_agvMissionService.Add(agvMission))
                return true;
            else
            {
                errorMsg = StockResult.InstockError_WriteMissionError;
                return false;
            }
        }
    }

    
}

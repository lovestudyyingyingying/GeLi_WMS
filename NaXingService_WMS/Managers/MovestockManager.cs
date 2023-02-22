using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.InstockEntity;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Helper.WMS;
using NanXingService_WMS.Services.WMS.AGV;
using NanXingService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UtilsSharp.Standard;

namespace NanXingService_WMS.Services.WMS
{
    public class MovestockManager
    {
        AGVMissionService _missionService ;
        LiuShuiHaoService _liuShuiHaoService ;
        WareLocationService _wareLocationService;
        WareLocationLockHisService _wareLocationLockHisService = new WareLocationLockHisService();
        InstockHelper _instockHelper;
        OutstockHelper _outstockHelper;
        RedisHelper redisHelper = new RedisHelper();
        public MovestockManager(AGVMissionService missionService, 
            LiuShuiHaoService liuShuiHaoService, WareLocationService wareLocationService)
        {
            _missionService = missionService;
            _liuShuiHaoService = liuShuiHaoService;
            _wareLocationService = wareLocationService;
            _instockHelper = new InstockHelper(_wareLocationService, _wareLocationLockHisService);
            _outstockHelper = new OutstockHelper(_wareLocationService, _wareLocationLockHisService);
        }

        #region 

        public BaseResult<string> Move(string TrayNo,string endPosition,
            string userID,string position,string remark)
        {
            DateTime dtime = DateTime.Now.AddDays(-1);
            var result = new BaseResult<string>();
            int runQ= _missionService.GetCount(u=> u.OrderTime > dtime && (u.TrayNo == TrayNo 
            || u.EndPosition== endPosition)
            &&
             //已下发到表 或者 执行中
             (u.SendState.Length == 0 ||
             (u.SendState.Length > 0

             && !(u.SendState == StockState.SendState_Success
                && u.RunState == StockState.RunState_Success)

             && u.RunState != StockState.RunState_RunFail
             && u.RunState != StockState.RunState_SendFail
             && u.RunState != StockState.RunState_Cancel)), true, DbMainSlave.Master);
            if (runQ>0)
            {
                result.SetError(StockResult.MovestockError_TrayInMissionError);
                return result;
            }

            WareLocation endWl=_wareLocationService.GetIQueryable(u => u.WareLocaNo == endPosition,
                true, DbMainSlave.Master).FirstOrDefault();
            if (endWl == null)
            {
                result.SetError(StockResult.MovestockError_FindEndWLSRError);
                return result;
            }

            if (endWl.WareLocaState == WareLocaState.PreIn || endWl.WareLocaState == WareLocaState.PreOut)
            {
                result.SetError(StockResult.MovestockError_EndWLIsUseError);
                return result;
            }
            if (endWl.TrayState!= null)
            {
                result.SetError(StockResult.MovestockError_EndWLHasTrayError);
                return result;
            }
            WareLocation startWl = _wareLocationService.GetIQueryable(
                u => u.TrayState.TrayNO  == TrayNo,
                true, DbMainSlave.Master).FirstOrDefault();

            if(startWl==null)
            {
                result.SetError(StockResult.MovestockError_TrayNoInstockError);
                return result;
            }
            if (startWl.WareLocaState == WareLocaState.PreIn || startWl.WareLocaState == WareLocaState.PreOut)
            {
                result.SetError(StockResult.MovestockError_EndWLIsUseError);
                return result;
            }

            bool ret1 = startWl.WareArea.InstockRule == "优先使用小的仓位号";

            var q0 = _wareLocationService.GetIQueryable(
               u => u.WareLoca_Lie == startWl.WareLoca_Lie
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
            agvMission.StartPosition = startWl.WareLocaNo;
            agvMission.StartLocation = startWl.AGVPosition;

            agvMission.StartMiddlePosition = startMiddleWl.WareLocaNo;
            agvMission.StartMiddleLocation = startMiddleWl.AGVPosition;

            agvMission.EndMiddlePosition = endMiddleWl.WareLocaNo;
            agvMission.EndMiddleLocation = endMiddleWl.AGVPosition;

            agvMission.EndPosition = endWl.WareLocaNo;
            agvMission.EndLocation = endWl.AGVPosition;

            agvMission.Mark = MissionType.MovestockType;
            agvMission.OrderTime = DateTime.Now;
            agvMission.OrderGroupId = string.Empty;
            agvMission.AGVCarId = string.Empty;
            agvMission.userId = userID;
            agvMission.IsFloor =
                (startWl.WareArea.WareHouse.WHName
                == endWl.WareArea.WareHouse.WHName)
                ? 0 : 1;
            if (agvMission.IsFloor == 0)
                agvMission.WHName = startWl.WareArea.WareHouse.WHName;
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
                agvMission.Remark = remark;

            if (_missionService.Add(agvMission))
            {
                _wareLocationService.UpdateByPlus(u => u.ID == endWl.ID,
                    u => new WareLocation
                    { WareLocaState = WareLocaState.PreIn });
                _wareLocationService.UpdateByPlus(u => u.ID == startWl.ID,
                   u => new WareLocation
                   { WareLocaState = WareLocaState.PreOut });
                result.SetOk();
                return result;
            }
            else
            {
                result.SetError(StockResult.MovestockError_WriteMissionError); 
                return result;
            }
        }
        #endregion

        #region
        public BaseResult<string> MoveBatchNo(string batchNo,string proname, string endPosition,
            string userID, string position,int boxCount, string remark,string isHuanCun)
        {
            //获取到本楼层所有去除预进预出批次的仓位
            List<WareLocation> startWls = GetAllWlsByBatchNo(batchNo, position,
                proname, isHuanCun=="1");
            startWls= GetLieCount(startWls, boxCount, userID, 1);
            //分配
            List<WareLocation> endWls=
            _instockHelper.GetInstockWLs(batchNo, proname, endPosition, userID, startWls.Count);

            AGVMissionInfo agvMission = null;
            DataTable dt = _missionService.ClassToDataTable(typeof(AGVMissionInfo));
            for (int i = 0; i < startWls.Count; i++)
            {
                if (agvMission == null)
                {
                    agvMission = new AGVMissionInfo();
                    agvMission.Mark = MissionType.MovestockType;
                    agvMission.OrderTime = DateTime.Now;
                    agvMission.userId = userID;
                    agvMission.OrderGroupId = string.Empty;
                    agvMission.AGVCarId = string.Empty;
                    agvMission.SendState = string.Empty;
                    agvMission.RunState = string.Empty;
                    agvMission.SendMsg = string.Empty;
                    agvMission.StateMsg = string.Empty;
                    agvMission.ModelProcessCode = string.Empty;
                }
                bool ret1 = startWls[i].WareArea.InstockRule == "优先使用小的仓位号";

                string lieName1 = startWls[i].WareLoca_Lie;
                var q0 = _wareLocationService.GetIQueryable(
                   u => u.WareLoca_Lie == lieName1
                   , true);

                WareLocation startMiddleWl = (ret1 ? q0.OrderByDescending(u =>
                    u.WareLoca_Index.Length).ThenByDescending(u => u.WareLoca_Index) :
                    q0.OrderBy(u =>
                    u.WareLoca_Index.Length).ThenBy(u => u.WareLoca_Index))
                   .FirstOrDefault();

                bool ret2 = endWls[i].WareArea.InstockRule == "优先使用小的仓位号";
                string lieName2 = endWls[i].WareLoca_Lie;
                var q1 = _wareLocationService.GetIQueryable(
                   u => u.WareLoca_Lie == lieName2
                   , true);
                WareLocation endMiddleWl = (ret2 ? q1.OrderByDescending(u =>
                     u.WareLoca_Index.Length).ThenByDescending(u => u.WareLoca_Index) :
                    q1.OrderBy(u =>
                    u.WareLoca_Index.Length).ThenBy(u => u.WareLoca_Index))
                    .FirstOrDefault();
                agvMission.IsFloor =
                        (startWls[i].WareArea.WareHouse.WHName ==
                        endWls[i].WareArea.WareHouse.WHName)
                        ? 0 : 1;
                if (agvMission.IsFloor == 0)
                {
                    agvMission.ModelProcessCode = string.Empty;
                    agvMission.WHName = !string.IsNullOrEmpty(position) ? position
                        : startWls[i].WareArea.WareHouse.WHName;
                    if (agvMission.Mark == MissionType.MoveOut_TSJ)
                        agvMission.Mark = MissionType.MovestockType;
                }
                agvMission.MissionNo = _liuShuiHaoService.GetAGVMissionNoLSH();
                agvMission.TrayNo = startWls[i].TrayState.TrayNO;
                agvMission.StartLocation = startWls[i].AGVPosition;
                agvMission.StartPosition = startWls[i].WareLocaNo;

                agvMission.StartMiddlePosition = startMiddleWl.WareLocaNo;
                agvMission.StartMiddleLocation = startMiddleWl.AGVPosition;

                agvMission.EndMiddlePosition = endMiddleWl.WareLocaNo;
                agvMission.EndMiddleLocation = endMiddleWl.AGVPosition;
                agvMission.OrderGroupId = agvMission.IsFloor == 0
                    ? endWls[i].WareLoca_Lie : string.Empty;

                agvMission.EndLocation = endWls[i].AGVPosition;
                agvMission.EndPosition = endWls[i].WareLocaNo;
                agvMission.Remark = remark;
                dt = _missionService.AddToDataTable(dt, agvMission);
            }
            _missionService.AddMany(dt);

            return null;
        }

        #endregion

        /// <summary>
        /// 获取该品种列排序从小到大的仓位，去除任务单中的出库位，与入库列
        /// </summary>
        /// <param name="sp">出入仓计划</param>
        /// <returns></returns>
        private List<WareLocation> GetAllWlsByBatchNo(string batchNo,string position,
            string proname,bool isHuanCun)
        {
            if (position == string.Empty)
                return null;

            //第一部
            #region 根据出库任务筛选出所有库位
            Expression<Func<WareLocation, bool>> exp = DbBaseExpand.True<WareLocation>();
            //判断是从 缓存区出库 还是 成品区出库
            //position 1L的03出库是从缓存区出库
            if(isHuanCun)
                exp = exp.And(u => u.WareArea.WareAreaClass.AreaClass == AreaClassType.HuanCunArea);
            else
                exp = exp.And(u => u.WareArea.WareAreaClass.AreaClass == AreaClassType.ChengPinArea);
            //判断范围是在1L还是2L
            if (position == string.Empty)
            {
                exp = exp.And(u => u.WareArea.WareHouse.WHName == "07一楼"
                        || u.WareArea.WareHouse.WHName == "07二楼");
            }
            else
            {
                exp = exp.And(u => u.WareArea.WareHouse.WHName == position);
            }

            exp = exp.And(u => u.TrayState != null && u.TrayState.batchNo == batchNo
               && u.TrayState.proname == proname);

            var sp_q = _wareLocationService.GetIQueryable(exp, true, DbMainSlave.Master);
            #endregion

            #region 获取被占用的 出库位，与入库列 ，即预进预出
            //排除任务单中的出库位，与入库列
            var lie_q = _wareLocationService.GetIQueryable(u => u.WareLocaState == WareLocaState.PreIn
              || u.WareLocaState == WareLocaState.PreOut, true, DbMainSlave.Master)
                .GroupBy(u => u.WareLoca_Lie);

            //var q= sp_q.Join(lie_q)
            #endregion

            var q = from a in sp_q
                    where !lie_q.Any(b => b.Key == a.WareLoca_Lie)
                    orderby (new
                    {
                        a.WareLoca_Lie.Length,
                        a.WareLoca_Lie,
                        a.WareLoca_Index,
                    })
                    select a;
            return q.ToList();
        }

        public List<WareLocation> GetLieCount(List<WareLocation> wldt,int boxCount,string userId,
           int wlType, bool isAutoOut = true)
        {



            if (wldt == null || wldt.Count == 0)
            {
                return null;
            }
            var list = wldt.GroupBy(u => new { u.WareArea.WareNo, u.WareLoca_Lie, u.WareArea.InstockRule })
                .Select(k => new LieState
                {
                    areaName = k.Key.WareNo,
                    lie = k.Key.WareLoca_Lie,
                    count = k.Sum(l => l.TrayState.OnlineCount),
                    InstockRule = k.Key.InstockRule
                }
                ).OrderBy(u => u.areaName.Length).ThenBy(u => u.areaName)
                .ThenBy(u => u.lie.Length).ThenBy(u => u.lie).ToList();


            int dcount1 = boxCount;
            //相减后的数
            int dcount2 = dcount1;
            //2、获取所有库区的数据
            List<WareLocation> newWls = new List<WareLocation>();
            List<WareLocation> sortList = null;
            string orderStr = OutstockHelper.SmallFirst;
            if (wlType == 2)
                orderStr = OutstockHelper.LargeFirst;
            foreach (var temp in list)
            {
                string lieStr = OutstockHelper.RedisStr + temp.lie;


                dcount1 = dcount1 - temp.count;
                //IEnumerable<WareLocation> sortQ = null;
                if (temp.InstockRule == orderStr)
                {
                    sortList = wldt.Where(u => u.WareLoca_Lie == temp.lie)
                        .OrderByDescending(u => u.WareLoca_Index).ToList();
                }
                else
                {
                    sortList = wldt.Where(u => u.WareLoca_Lie == temp.lie)
                        .OrderBy(u => u.WareLoca_Index).ToList();
                }
                //如果dcount>0,证明temp lie 的当前列的数量不满足需要，还要添加数量
                if (dcount1 > 0)
                {
                    newWls = newWls.Union(sortList).ToList();
                    //改变仓位状态为预出
                    if (isAutoOut)
                    {
                        _outstockHelper.ChangeProOut(sortList, userId);
                    }
                    dcount2 = dcount1;
                }
                else
                {
                    List<WareLocation> wls = new List<WareLocation>();
                    //最后一列
                    foreach (WareLocation wl in sortList)
                    {
                        dcount2 = dcount2 - wl.TrayState.OnlineCount;
                        newWls.Add(wl);
                        wls.Add(wl);
                        if (dcount2 <= 0)
                        {
                            break;
                        }
                    }
                    //改变仓位状态为预出
                    if (isAutoOut)
                    {
                        _outstockHelper.ChangeProOut(wls, userId);
                    }
                    break;
                    //dcount1 = dcount2;
                }

            }
            return newWls;
        }
    }
}

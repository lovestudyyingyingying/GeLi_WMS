using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.InstockEntity;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Services.WMS;
using NanXingService_WMS.Services.WMS.AGV;
using NanXingService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Helper.WMS
{
    public class OutstockHelper
    {
        WareLocationService _wareLocationService;
        WareLocationLockHisService _wareLocationLockHisService;


        RedisHelper redisHelper = new RedisHelper();
        public OutstockHelper(WareLocationService wareLocationService
            , WareLocationLockHisService wareLocationLockHisService)
        {
            this._wareLocationService = wareLocationService;
            _wareLocationLockHisService = wareLocationLockHisService;

        }

        //string ChuKuArea = "出库区";

        public static string SmallFirst = "优先使用小的仓位号";
        public static string LargeFirst = "优先使用大的仓位号";
        //string StateNormal = "空";
        public static string RedisStr = "OutStock";

        #region 出库功能



        #endregion


        #region 出仓通用货物起点仓位
        /// <summary>
        /// 根据出库计划获取
        /// </summary>
        /// <param name="sp">出库计划</param>
        /// <param name="isAutoOut">是否自动设置预出状态</param>
        /// <returns>出库计划仓位</returns>
        public List<WareLocation> GetOutStockProWl(StockPlan sp, bool isAutoOut = true)
        {
            List<WareLocation> outList = null;
            using (redisHelper.CreateLock(RedisStr))
            {
                List<WareLocation> list = GetAllWlsByStockPlan(sp);

                //Task<DataTable> task = _wareLocationService.ClassToDataTableAsync(typeof(WareLocation));

                outList = GetLieCount(list, sp, 1, isAutoOut);


            }
            return outList;
        }
        /// <summary>
        /// 根据仓库与是否缓存区获取产品与数量
        /// </summary>
        /// <param name="position"></param>
        /// <param name="isHuanCun"></param>
        /// <returns></returns>
        public List<StockProItem> GetStockProItem(string position, bool isHuanCun)
        {
            #region 根据仓库名称筛选出所有有货物的库位
            Expression<Func<WareLocation, bool>> exp = DbBaseExpand.True<WareLocation>();
            if (isHuanCun)
                exp = exp.And(u => u.WareArea.WareAreaClass.AreaClass == AreaClassType.HuanCunArea);
            else
            {
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
            }
            //
            exp = exp.And(u => u.TrayState != null && u.WareLocaState == WareLocaState.HasTray && u.IsOpen == 1);
            var allTrayState_Q = _wareLocationService.GetIQueryable(exp, true, DbMainSlave.Master);
            #endregion

            #region 将所有库位进行分类
            List<StockProItem> itemList = allTrayState_Q.GroupBy(u => new
            {
                u.TrayState.batchNo,
                u.TrayState.proname,
                u.TrayState.spec,
                u.TrayState.probiaozhun
            }).Select(u => new StockProItem
            {
                BatchNo = u.Key.batchNo,
                Spec = u.Key.spec,
                Biaozhun = u.Key.batchNo,
                Proname = u.Key.proname,
                Count_All = u.Sum(item => item.TrayState.OnlineCount),
                Count_Useable = 0
            }).ToList();

            //计算可用数量
            itemList.ForEach((item) =>
            {
                item.Count_Useable =
                    allTrayState_Q.Where(u => u.TrayState.batchNo == item.BatchNo &&
                    u.TrayState.proname == item.Proname &&
                    u.TrayState.spec == item.Spec &&
                    u.TrayState.probiaozhun == item.Biaozhun && u.WareLocaState == WareLocaState.HasTray)
                    .Count();
            });
            return itemList;
            #endregion

        }

        /// <summary>
        /// 获取该品种列排序从小到大的仓位，去除任务单中的出库位，与入库列
        /// </summary>
        /// <param name="sp">出入仓计划</param>
        /// <returns></returns>
        private List<WareLocation> GetAllWlsByStockPlan(StockPlan sp)
        {
            //第一部
            #region 根据出库任务筛选出所有库位
            Expression<Func<WareLocation, bool>> exp = DbBaseExpand.True<WareLocation>();
            //判断是从 缓存区出库 还是 成品区出库
            //position 1L的03出库是从缓存区出库
            if (sp.mark == "03" && sp.position == "07一楼")
                exp = exp.And(u => u.WareArea.WareAreaClass.AreaClass == AreaClassType.HuanCunArea);
            else
            {
                exp = exp.And(u => u.WareArea.WareAreaClass.AreaClass == AreaClassType.ChengPinArea);
                //判断范围是在1L还是2L
                if (sp.position == string.Empty)
                {
                    exp = exp.And(u => u.WareArea.WareHouse.WHName == "07一楼"
                            || u.WareArea.WareHouse.WHName == "07二楼");
                }
                else
                {
                    exp = exp.And(u => u.WareArea.WareHouse.WHName == sp.position);
                }
            }

            exp = exp.And(u => u.TrayState != null && u.TrayState.batchNo == sp.batchNo
                && u.TrayState.probiaozhun == sp.probiaozhun
                && u.TrayState.spec == sp.spec && u.TrayState.proname == sp.proname);

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
        #endregion

        #region 出仓通用终点仓位
        /// <summary>
        /// 得到终点仓位集合
        /// </summary>
        /// <param name="stockPlan"></param>
        /// <returns></returns>
        public List<WareLocation> GetTargetWls(StockPlan stockPlan, int count, bool isAutoOut = true)
        {
            //通过Mark判断是否跨楼层计划，03为同楼层、05为跨楼层
            //03为出库到出仓位置，05为出库到缓存位置
            if (stockPlan.mark == MissionType.OutstockType)
            {
                return _wareLocationService.GetIQueryable(
                    u => u.IsOpen == 1
                    && u.WareArea.WareHouse.WHName == stockPlan.position
                    && u.WareArea.WareAreaClass.AreaClass == AreaClassType.ChuKuArea
                    , true, DbMainSlave.Master).OrderBy(u => u.AGVPosition)
                    .Take(count).ToList();
            }
            else if (stockPlan.mark == MissionType.MoveOut_TSJ)
            {
                //获取缓存位置的空位
                var all_q = _wareLocationService.GetIQueryable(
                    u => u.IsOpen == 1
                    && u.WareArea.WareAreaClass.AreaClass == AreaClassType.HuanCunArea
                    && u.TrayState == null && u.WareLocaState == WareLocaState.NoTray
                    , true, DbMainSlave.Master);

                //#region 获取被占用的 出库位，与入库列 ，即预进预出和占用
                //排除任务单中的出库位，与入库列
                var lie_q = _wareLocationService.GetIQueryable(
                    u => u.WareLocaState == WareLocaState.PreIn || u.WareLocaState == WareLocaState.PreOut
                    || u.WareLocaState == WareLocaState.HasTray, true, DbMainSlave.Master)
                    .GroupBy(u => u.WareLoca_Lie);

                //var q= sp_q.Join(lie_q)
                //#endregion
                //排除预进预出占用的所有缓存位
                var q = (from a in all_q
                         where !lie_q.Any(b => b.Key == a.WareLoca_Lie)
                         orderby (new
                         {
                             a.WareArea.WareNo,
                             a.WareLoca_Lie.Length,
                             a.WareLoca_Lie,
                             a.WareLoca_Index,
                         })
                         select a).Take(count).ToList();
                //List<WareLocation> qList= GetLieCount(q.ToList(), stockPlan, 2);
                //改变仓位状态为预进
                if (isAutoOut)
                {
                    //DataTable wareLockDt = new DataTable();
                    WareLoactionLockHis wareLoactionLockHis = null;
                    foreach (WareLocation temp in q)
                    {
                        temp.WareLocaState = WareLocaState.PreIn;
                        temp.BatchNo = stockPlan.batchNo;
                        wareLoactionLockHis = new WareLoactionLockHis();
                        wareLoactionLockHis.WareLocaNo = temp.WareLocaNo;
                        wareLoactionLockHis.PreState = WareLocaState.PreIn;
                        wareLoactionLockHis.LockTime = DateTime.Now;
                        wareLoactionLockHis.Locker = stockPlan.planUser;
                        _wareLocationLockHisService.Insert(wareLoactionLockHis);
                        _wareLocationLockHisService.SaveChanges();
                        temp.LockHis_ID = wareLoactionLockHis.ID;
                    }
                    //DataTable dt= await task;
                    DataTable dt = _wareLocationService.ConvertToDataTable(q);
                    _wareLocationService.BatchUpdateData(dt, "WareLocation");
                    //_wareLocationLockHisService.SetDataTableToTable(wareLockDt, "WareLocationLockHis");

                }
                return q;
            }
            else
                return null;
        }

        #endregion

        #region 通用方法
        /// <summary>
        /// 从所有仓位中选出满足任务条件数量的仓位（起点用）
        /// </summary>
        /// <param name="wldt">该产品所有仓位</param>
        /// <param name="sp">出库任务</param>
        /// <param name="wlType">仓位类型：1为出仓起点位，2为出仓终点位</param>
        public List<WareLocation> GetLieCount(List<WareLocation> wldt, StockPlan sp,
            int wlType, bool isAutoOut = true)
        {
            if (wldt == null || wldt.Count == 0)
            {
                return null;
            }
            var list = wldt.GroupBy(u => new { u.WareArea.WareNo, u.WareLoca_Lie, u.WareArea.InstockRule })
                .Select(k => new LieState
                {
                    areaName=k.Key.WareNo,
                    lie = k.Key.WareLoca_Lie,
                    count = k.Sum(l => l.TrayState.OnlineCount),
                    InstockRule = k.Key.InstockRule
                }
                ).OrderBy(u => u.areaName.Length).ThenBy(u => u.areaName)
                .ThenBy(u => u.lie.Length).ThenBy(u => u.lie).ToList();


            int dcount1 = (int)sp.count;
            //相减后的数
            int dcount2 = dcount1;
            //2、获取所有库区的数据
            List<WareLocation> newWls = new List<WareLocation>();
            List<WareLocation> sortList = null;
            string orderStr = SmallFirst;
            if (wlType == 2)
                orderStr = LargeFirst;
            foreach (var temp in list)
            {
                string lieStr = RedisStr + temp.lie;


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
                        ChangeProOut(sortList, sp.planUser);
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
                        ChangeProOut(wls, sp.planUser);
                    }

                    break;


                    //dcount1 = dcount2;
                }

            }
            return newWls;
        }

        public void ChangeProOut(List<WareLocation> outList, string userName)
        {
            DataTable wareLockDt = new DataTable();
            foreach (WareLocation temp in outList)
            {
                temp.WareLocaState = WareLocaState.PreOut;
                //temp.WareLocaState = temp.WareLocaState;
                WareLoactionLockHis wareLoactionLockHis = new WareLoactionLockHis();
                wareLoactionLockHis.WareLocaNo = temp.WareLocaNo;
                wareLoactionLockHis.PreState = WareLocaState.PreOut;
                wareLoactionLockHis.LockTime = DateTime.Now;
                wareLoactionLockHis.Locker = userName;
                wareLockDt = _wareLocationLockHisService.ParseInDataTable(wareLockDt, wareLoactionLockHis);
            }
            //DataTable dt= await task;
            DataTable dt = _wareLocationService.ConvertToDataTable(outList);
            _wareLocationService.BatchUpdateData(dt, "WareLocation");
            _wareLocationLockHisService.SetDataTableToTable(wareLockDt, "WareLoactionLockHis");
        }

        #endregion
    }
}

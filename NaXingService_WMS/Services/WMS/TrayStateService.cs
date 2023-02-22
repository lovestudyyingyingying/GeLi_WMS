using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.InstockEntity;
using NanXingService_WMS.Entity.StockEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services
{
    public class TrayStateService:DbBase<TrayState>
    {
        DbBase<TrayState> trayStateDao = new DbBase<TrayState>();

        #region 简单查询
        public TrayState GetByTrayNo(string prosn,bool isNoTracking=false
            , DbMainSlave dms = DbMainSlave.Slave)
        {
            return trayStateDao.GetList(u => u.TrayNO == prosn,isNoTracking, dms)
                .FirstOrDefault();
        }

        public TrayState GetByWL(string wl)
        {
            return trayStateDao.GetList(u => u.WareLocation!=null
                &&u.WareLocation.WareLocaNo == wl).FirstOrDefault();
        }

        public IQueryable<TrayState> GetQuery(Expression<Func<TrayState, bool>> whereLambda, bool isNoTracking = false,
            DbMainSlave dms = DbMainSlave.Slave, Expression<Func<TrayState, string>> ordering = null)
        {
            return trayStateDao.GetIQueryable(whereLambda,isNoTracking,dms,ordering);

        }
        #endregion

        #region 其他方法

        public DataTable ConvertDataTable<T>(IEnumerable<T> ts)
        {
            return trayStateDao.ConvertToDataTable(ts);
        }
        #endregion

        #region 根据批次查询库存产品信息与数量

        public DataTable GetInfoByBatchNo(string batchNo,string position, bool isHuanCun)
        {
            Expression<Func<TrayState, bool>> exp = DbBaseExpand.True<TrayState>();
            string sql_Area = AreaClassType.ChengPinArea;
            //string sql_Mark = "05";
            if (isHuanCun)
            {
                sql_Area = AreaClassType.HuanCunArea;
                //sql_Mark= "03";
            }
            //string sqlposition = $" and WareArea.WareHouse_ID ={position} ";
            if (position == string.Empty)
                exp = exp.And(u => (u.WareLocation.WareArea.WareHouse.WHName == "07一楼"
                            || u.WareLocation.WareArea.WareHouse.WHName == "07二楼"));
            else
            {
                exp = exp.And(u =>  u.WareLocation.WareArea.WareHouse.WHName == position);

            }

            exp = exp.And(u => u.batchNo == batchNo
                && u.WareLocation.WareArea.WareAreaClass.AreaClass == sql_Area
                );

            //1、获取所有
            var trayState_q = GetQuery(exp, true, DbMainSlave.Master)
                .GroupBy(u => new { u.batchNo, u.spec, u.proname, u.probiaozhun, u.color })
                .Select(k => new
                {
                    batchNo = k.Key.batchNo,
                    spec = k.Key.spec,
                    proname = k.Key.proname,
                    probiaozhun = k.Key.probiaozhun,
                    color= k.Key.color,
                    count = k.Sum(l => l.OnlineCount)
                });

            #region 获取被占用的 出库位，与入库列 ，即预进预出
            //排除任务单中的出库位，与入库列
            var batchNo_q = GetQuery(u => 
                u.WareLocation != null
                && u.WareLocation.WareLocaState == WareLocaState.PreIn
              || u.WareLocation.WareLocaState == WareLocaState.PreOut, true, DbMainSlave.Master)
                .GroupBy(u => u.batchNo).Select(k => new
                {
                    batchNo = k.Key,
                    count = k.Sum(l => l.OnlineCount)
                });
            var list = batchNo_q.ToList();
            //var q= sp_q.Join(lie_q)
            #endregion

            var q = from a in trayState_q
                    join b in batchNo_q
                    on a.batchNo equals b.batchNo
                    into JoinedTrayStateBatchNo
                    from c in JoinedTrayStateBatchNo.DefaultIfEmpty()
                    orderby (new
                    {
                        a.batchNo.Length,
                        a.batchNo,
                    })
                    select new StockProItem
                    {
                        BatchNo = a.batchNo,
                        Spec = a.spec,
                        Biaozhun = a.probiaozhun,
                        Proname = a.proname,
                        Color=a.color??string.Empty,
                        Count_All = a.count,
                        Count_Useable =  a.count - (c==null?0:c.count)
                    };
            var list1 = q.ToList();
            DataTable dt = ConvertDataTable<StockProItem>(q);
            return dt;
        }

        #endregion

        #region 添加

        //public void Add

        #endregion

    }
}

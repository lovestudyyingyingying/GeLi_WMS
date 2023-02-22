

using GeLiService_WMS.Entity.StockEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GeLiData_WMS;
using GeLiData_WMSUtils;
using GeLi_Utils.Entity.PDAApiEntity;

namespace GeLiService_WMS.Services
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

   

        #endregion

        #region 添加

        //public void Add

        public string AddByPda(PdaTray pdaTray)
        {
            string pdaTrayNo = pdaTray.分厂订单批次;
            var trayState = GetList(u => u.TrayNO == pdaTrayNo).FirstOrDefault();
            if (trayState != null)
            {

                return trayState.TrayNO;
            }
            else
            {
                TrayState trayState1 = new TrayState();
                trayState1.TrayNO = pdaTray.分厂订单批次;
                trayState1.optdate = DateTime.Parse(pdaTray.总装订单上线时间);
                trayState1.OnlineCount =int.Parse(pdaTray.数量);
                trayState1.itemno = pdaTray.物料编码;
                trayState1.proname = pdaTray.物料名称;
                trayState1.Reserve1 = pdaTray.总装订单;
                Insert(trayState1);
                SaveChanges();
                return trayState1.TrayNO;
            }
        }

        #endregion

    }
}

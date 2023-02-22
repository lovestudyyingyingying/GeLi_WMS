using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Helper.WMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services.WMS
{
    class OutstockService
    {
        DbBase<AGVMissionInfo> agvMissionDao = new DbBase<AGVMissionInfo>();
        DbBase<TrayState> trayStateDao = new DbBase<TrayState>();
        DbBase<WareLocation> wareLocationDao = new DbBase<WareLocation>();

        

        #region 查询可出库成品
        public void GetAllowOutTray()
        {
            //仓位状态：空、占用、预进、预出
            //1、获取所有
            //List<WareLocation> list= wareLocationDao.get

        }
        #endregion

        #region 出库操作
        HCWareLocationHelper hcWareLocationHelper =  new HCWareLocationHelper();


        public void RunOutstock(StockPlan sp)
        {

        }

        #endregion
    }
}

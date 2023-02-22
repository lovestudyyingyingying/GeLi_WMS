
using GeLiService_WMS.Entity.StockEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeLiData_WMSUtils;
using GeLiData_WMS;

namespace GeLiService_WMS.Services.WMS
{
    public class WareLocationLockHisService:DbBase<WareLoactionLockHis>
    {
        public bool UnLock(WareLocation wareLocation)
        {
            if (wareLocation == null)
                return true;
            if (wareLocation.LockHis_ID == null)
                return true;
           
            WareLoactionLockHis wareLoactionLockHis = FindById(wareLocation.LockHis_ID
                ,DbMainSlave.Master);
            wareLoactionLockHis.UnLockTime = DateTime.Now;
            Update(wareLoactionLockHis, new List<string>() { "UnLockTime" });
            return SaveChanges()>0;
        }

        public bool UnLockByPlus(WareLocation wareLocation)
        {
            if (wareLocation.LockHis_ID == null)
                return true;
            return UpdateByPlus(u => u.ID == wareLocation.LockHis_ID,
                u => new WareLoactionLockHis { UnLockTime = DateTime.Now })>0;
        }

    }
}

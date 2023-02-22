using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.StockEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace NanXingService_WMS.Services.WMS
{
    public class WareAreaService:DbBase<WareArea>
    {

        public IQueryable<WareAreaIndexData> GetIndexData(Expression<Func<WareArea, bool>> expression)
        {
            return SelectToQuery<WareAreaIndexData>(expression,
                u => new WareAreaIndexData()
                {
                    ID = u.ID,
                    WareNo = u.WareNo,
                    AreaClassName = u.WareAreaClass.AreaClass,
                    WareAreaState = u.WareAreaState??false
                }); 
        }

        public bool ChangeAreaState(int wareId,bool ret)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    WareArea area = FindById(wareId,DbMainSlave.Master);
                    List<WareLocation> wareLocations = area.WareLocation.ToList();
                    //bool ret = area.WareAreaState == null ? true : false;
                    foreach (WareLocation wareLocation in wareLocations)
                        wareLocation.IsOpen = ret?1:0;
                    
                    area.WareAreaState = ret;
                    SaveChanges();
                    tran.Complete();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }
    }
}

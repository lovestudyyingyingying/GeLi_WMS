using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.ProductEntity;
using NanXingService_WMS.Entity.StockEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services.APS
{
    public class ProductOrderlistsService : DbBase<ProductOrderlists>
    {

        public IQueryable<ProductOrderlistsIndexData> GetEditIndexData(Expression<Func<ProductOrderlists, bool>> exp)
        {
                   
            return SelectToQuery<ProductOrderlistsIndexData>(exp,
                u => new ProductOrderlistsIndexData()
                {
                    ID = u.ID,
                    ProductOrder_XuHao = u.ProductOrder_XuHao,
                    ItemName = u.ItemName,
                    DeliveryDate = u.ProPlanOrderlists.crmPlanList.DeliveryDate,
                    Spec = u.Spec,
                    Unit = u.Unit,
                    PcCount = u.PcCount,
                    ProCount = u.ProCount ?? 0,
                    NoWorkCount = (u.PcCount ?? 0) - (u.ProCount ?? 0),
                    BatchNo = u.BatchNo,
                    BoxNo = u.BoxNo,
                    BoxName = u.BoxName,
                    PlanDate = u.ProPlanOrderlists.PlanDate,
                    PlanProDate = u.PlanDate,
                    PlanProTime = u.PlanTime,
                    GiveDept = u.GiveDept,
                    Ingredients = u.Ingredients,
                    Chejianclass = u.Chejianclass,
                    Jingbanren = u.Jingbanren,
                    ProOrderList_State = u.ProOrderList_State,
                    PrintState = u.PrintState,
                    PlanOrder_XuHao = u.ProPlanOrderlists.PlanOrder_XuHao,
                    Newdate = u.Newdate ?? DateTime.Now,
                   

                    PlanTime = u.ProPlanOrderlists.PlanTime,
                    Priority = u.Priority,
                    Remark = u.Remark,
                    ProductState = u.ProOrderList_State,
                    HalfProState = u.ProPlanOrderlists.HalfProState,
                    BoxRemark = u.ProPlanOrderlists.BoxRemark
                }) ;
        }

       

    }
}

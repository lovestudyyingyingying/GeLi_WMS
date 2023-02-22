using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services.WMS
{
    public class StockPlanService: DbBase<StockPlan>
    {
        //DbBase<StockPlan> stockPlanDao = new DbBase<StockPlan>();

        public bool AddStockPlan(StockPlan stockPlan)
        {
            Insert(stockPlan);
            return SaveChanges()>0;
        }

        public void UpdateStockPlan(StockPlan stockPlan)
        {
            Update(stockPlan);
            SaveChanges();
        }


    }
}

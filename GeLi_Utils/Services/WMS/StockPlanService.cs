using GeLiData_WMS;
using GeLiData_WMSUtils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Services.WMS
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

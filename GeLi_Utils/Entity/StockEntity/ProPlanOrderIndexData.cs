using GeLiService_WMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.StockEntity
{
    public class ProPlanOrderIndexData
    {

        public int ID { get; set; }
        public string HalfProState { get; set; }
        public string PlanOrder_XuHao { get; set; }
        public string Remark { get; set; }
        public string Chejianclass { get; set; }
        public string ItemNo { get; set; }
        public string Biaozhun { get; set; }
        public string ProductRecipe { get; set; }
        
        public string ItemName { get; set; }
        public string ProUnit { get; set; }
        public decimal? PCCount { get; set; }
        public string Spec { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? PlanProDate { get; set; }
        public string PlanProTime { get; set; }
        public string YanBanCount { get; set; }

        public string YanBanUnit { get; set; }

        public DateTime? PlanDate { get; set; }
        public string Reserve1 { get; set; }
        
        public string PlanTime { get; set; }

        public decimal? PcCount { get; set; }
        public string Unit { get; set; }
        public string BoxNo { get; set; }

        public string BoxName { get; set; }

        public string BoxRemark { get; set; }
        public string JingBanRen { get; set; }

        public string PlanProRen { get; set; }

        public DateTime? PushProDate { get; set; }
        public string Ingredients { get; set; }
        public string GiveDept { get; set; }
        public string Productlocation { get; set; }
        public string PlanOrder_State { get; set; }

        public string ProListNo { get; set; }
        //public List<ProductOrderlists> ProductOrderlists { get; set; }

    }
}

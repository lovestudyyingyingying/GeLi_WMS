using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.ProductEntity
{
    public class ProductOrderEntity
    {
        //ID: 0,
        //    OrderNo: '',
        //    Count_ALL: 0,
        //    Count_HasPro: 0,
        //    Precent_HasPro: 0,
        //    Count_NoPro: 0,
        public int ID { get; set; }
        public string OrderNo { get; set; }
        public string CheJianName { get; set; }
        public decimal Count_ALL { get; set; }
        public decimal Count_HasPro { get; set; }
        public decimal Precent_HasPro { get; set; }
        public decimal Count_NoPro { get; set; }
        public List<ProductOrderListEntity> OrderList { get; set; }

    }

    public class ProductOrderListEntity
    {
        //ClientName: "广东宏正自动识别技术有限公司"
        //Count_HasPro: 0
        //Count_NoPro: 50
        //ID: 14
        //ItemName: "开心果"
        //PlanCount: 50
        //PlanTime: "3月14日"    
        public int ID { get; set; }
        public string ClientName { get; set; }
        public string ItemName { get; set; }
        public string PlanTime { get; set; }
        public decimal PlanCount { get; set; }
        public decimal Count_HasPro { get; set; }
        public decimal Count_NoPro { get; set; }
       
    }
}

using NanXingData_WMS.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.ProductEntity
{
    public class ProductIndexData
    {
        public int ID { get; set; }
        public string ProductOrderNo { get; set; }
        public decimal? YanBanCount { get; set; }
        public decimal? ProYanBanCount { get; set; }
        
        public string YanBanUnit { get; set; }
        public string ItemName { get; set; }
        public string Priority { get; set; }
        public decimal PcCount { get; set; }
        public decimal NoWorkCount { get; set; }
        public decimal WorkCount { get; set; }
        public decimal Present_Work { get; set; }
        public string Remark { get; set; }
        public string Workshops { get; set; }
        public DateTime Newdate { get; set; }
        public DateTime? PlanDate { get; set; }
        public string PlanTime { get; set; }
        public string JingBanRen { get; set; }

        public DateTime DeliveryDate { get; set; }
        public string PrintState { get; set; }
        public string ProductState { get; set; }
        public List<ProductOrderlists> ProductOrderlists { get; set; }

    }
}

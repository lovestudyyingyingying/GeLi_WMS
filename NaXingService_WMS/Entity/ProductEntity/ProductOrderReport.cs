using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.ProductEntity
{
    public  class ProductOrderReport
    {
        public int ID { get; set; }
        public string OrderNo { get; set; }
        public string BoxNo { get; set; }
        public string PlanDate { get; set; }
        public string CheJianName { get; set; }
        public string ItemName { get; set; }
        public string Spec { get; set; }
        public string ClientName { get; set; }
        public string Unit { get; set; }
        public decimal PcCount { get; set; }
        public decimal ProCount { get; set; }
        public decimal NoWorkCount { get; set; }
        public decimal Precent_Pro { get; set; }
        public string Statue { get; set; }
        public string UploadBatch { get; set; }

    }
}

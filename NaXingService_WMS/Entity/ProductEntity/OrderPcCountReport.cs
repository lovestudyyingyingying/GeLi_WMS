using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.ProductEntity
{
    public class OrderPcCountReport
    {
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string CheJianName { get; set; }
        public string Unit { get; set; }
        public string Spec { get; set; }

        public decimal PcCount { get; set; }
        public decimal ProCount { get; set; }
        public decimal AllProCount { get; set; }

        public decimal ProPrecent { get; set; }
        public string ConvertRate { get; set; }

        public decimal PcCountOnKg { get; set; }
        public decimal ProCountOnKg { get; set; }
        public decimal AllProCountOnKg { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.InstockEntity
{
    public class StockProItem
    {
        /*
         批号=a.batchNo,
         型号=a.spec,
         标准=a.probiaozhun,
         品名=a.proname,
         理论箱数 = a.count,
         可用箱数 =c.count-a.count
         */
        public string BatchNo { get; set; }
        public string Spec { get; set; }
        public string Biaozhun { get; set; }
        public string Proname { get; set; }
        public string Color { get; set; }
        public int Count_All { get; set; }

        public int Count_Useable { get; set; }


    }
}

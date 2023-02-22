using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.ProductEntity
{
    public class ProOrderAddCount
    {
        public int OrderID { get; set; }
        public int Count { get; set; }
        public string UploadBatch { get; set; }
        public string ControlUser { get; set; }
    }
}

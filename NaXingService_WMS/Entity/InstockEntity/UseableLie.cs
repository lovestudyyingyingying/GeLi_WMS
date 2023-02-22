using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.InstockEntity
{
    public class UseableLie
    {
        public string WHName { get; set; }
        public string WareAreaName { get; set; }
        public string WareLocation_Lie { get; set; }
        public string BatchNo { get; set; }
        public int NullWLCount { get; set; }
        public string InstockRule { get; set; }

    }
}

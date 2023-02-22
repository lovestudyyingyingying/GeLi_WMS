using GeLiService_WMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.StockEntity
{
    public class LieHasNullCount
    {
        public string WareAreaName { get; set; }
        public string WareLoca_Lie { get; set; }
        public string WareLocaState { get; set; }
        public string BatchNo { get; set; }
        public string ProName { get; set; }
        public string WareHouse { get; set; }
        public string InstockRule{ get; set; }
        public int Count { get; set; }

    }
}

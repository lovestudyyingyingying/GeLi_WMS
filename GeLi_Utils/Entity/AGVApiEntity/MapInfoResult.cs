using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.AGVApiEntity
{

    public class MapInfoResult
    {
        public int code { get; set; }
        public string msg { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public int type { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public outEdges[] outEdges { get; set; }
        public string name { get; set; }
    }

    public class outEdges
    {
        public int nid { get; set; }
    }

}

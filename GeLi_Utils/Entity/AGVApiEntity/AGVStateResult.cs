using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.AGVApiEntity
{

    public class AGVStateResult
    {
        public int code { get; set; }
        public string msg { get; set; }
        public data[] data { get; set; }
    }

    public class data
    {
        public int id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int theta { get; set; }
        public int cargo { get; set; }
        public int soc { get; set; }
    }

}

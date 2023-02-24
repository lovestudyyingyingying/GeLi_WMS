using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 单元测试
{

    public class Class1
    {
        public int code { get; set; }
        public string msg { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public Outedge[] outEdges { get; set; }
    }

    public class Outedge
    {
        public int nid { get; set; }
        public object maxV { get; set; }
        public object laserD { get; set; }
        public object laserLD { get; set; }
        public object laserRD { get; set; }
        public object obsRunType { get; set; }
        public object runMode { get; set; }
        public object curvePoints { get; set; }
    }

}

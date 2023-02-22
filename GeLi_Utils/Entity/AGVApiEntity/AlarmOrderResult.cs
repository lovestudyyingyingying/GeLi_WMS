using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.AGVApiEntity
{

    public class AlarmOrderResult
    {
        public int code { get; set; }
        public string msg { get; set; }
        public AlarmData[] data { get; set; }
    }

    public class AlarmData
    {
        public string alarm_code { get; set; }
        public string detail { get; set; }
        public int grade { get; set; }
        public int id { get; set; }
        public long time { get; set; }
    }

}

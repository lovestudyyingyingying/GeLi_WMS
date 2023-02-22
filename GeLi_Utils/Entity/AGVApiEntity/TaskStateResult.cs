using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.AGVApiEntity
{

    public class TaskStateResult
    {
        public int code { get; set; }
        public Data data { get; set; }
        public string msg { get; set; }
    }

    public class Data
    {
        public int agvNo { get; set; }
        public int status { get; set; }
        public string taskId { get; set; }
    }

}

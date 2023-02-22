using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.AGVOrderEntity
{
    
        public class DeviceApply
        {
            public string taskId { get; set; }
            public int agvNo { get; set; }
            public string action { get; set; }
            public int type { get; set; }
            public string deviceId { get; set; }
        }

    
}

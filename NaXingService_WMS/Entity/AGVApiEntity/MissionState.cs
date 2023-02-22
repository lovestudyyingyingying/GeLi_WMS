using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.AGVApiEntity
{
    public class MissionState
    {
        public string orderId { get; set; }

        public string qrContent { get; set; }

        public string deviceNum { get; set; }

        public int status { get; set; }

        public string processRate { get; set; }

        public string shelfNumber { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.AGVOrderEntity
{
    public class MissionStates
    {
        public string code { get; set; }
        public StateData data { get; set; }
    }

    public class StateData
    {
        public int areaId { get; set; }
        public int createTime { get; set; }
        public string fromSystem { get; set; }
        public int status { get; set; }
        public StateDataDetails[] taskOrderDetail { get; set; }
    }

    public class StateDataDetails
    {
        public string qrContent { get; set; }
        public string deviceNum { get; set; }
        public string status { get; set; }
        public int time { get; set; }
    }
}

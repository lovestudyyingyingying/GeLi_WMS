using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys.QueueInputEntitys
{
    public class CRMProState:QueueInputEntityBase
    {
        public string proOrderNo { get; set; }
        public string pcState { get; set; }
        public string proState { get; set; }

    }
}

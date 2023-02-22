using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity
{
    public  class CRMApplyState: QueueInputEntityBase
    {
        public string pcState { get; set; }

        public string pcReason { get; set; }

    }
}

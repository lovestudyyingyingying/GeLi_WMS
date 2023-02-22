using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys;

namespace NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity
{
    public class CRMPlanEntity: QueueInputEntityBase
    {
        //public string crmID { get; set; }
        public string planOrderNo { get; set; }
        public string planOrderNo_XuHao { get; set; }
        public string crmState { get; set; }
        public string pcCount { get; set; }
        public string pcUnit { get; set; }
        public string fzCount { get; set; }
        public string fzUnit { get; set; }
        public long pcTime { get; set; }
        public string batchNo { get; set; }

    }
}

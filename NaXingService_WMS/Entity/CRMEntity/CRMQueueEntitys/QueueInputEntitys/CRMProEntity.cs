using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity
{
    public class CRMProEntity: QueueInputEntityBase
    {
        public string planOrderNo_XuHao { get; set; }
        public string proOrderNo { get; set; }
        public string taskName { get; set; }
        public string proState { get; set; }
        public long planTime { get; set; }
        public string crmState { get; set; }
        public string updateUser { get; set; }

    }
}

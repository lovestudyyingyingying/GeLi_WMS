using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys;
using NanXingService_WMS.Entity.CRMItemEntity;
using NanXingService_WMS.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity
{
    public class CRMApplyChangeRequest
    {
        public string corpAccessToken { get; set; }
        public string corpId { get; set; }

        public string currentOpenUserId = "FSUID_9D013E3437E2C860E6E5B27BE5D52B6A";

        public bool triggerWorkFlow { get; set; }
        public Data data { get; set; }

        //public static CRMApplyChangeRequest CreateRequest(CRMTokenResult tokenResult)
        //{

        //}
    }
    public class Data
    {
        public QueueOutputEntityBase object_data { get; set; }
        public Data()
        {

        }
        public Data(QueueOutputEntityBase _DataBase)
        {
            object_data = _DataBase;
        }
    }
}

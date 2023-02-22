using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMEntity
{
    public class CRMWriteEntity 
    {
        public string WriteType { get; set; }

        public object WriteObject { get; set; }

        public CRMWriteEntity(string writeType, object writeObject)
        {
            WriteType = writeType;
            WriteObject = writeObject;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity
{
    class CRMResult
    {
        public string traceId { get; set; }
        public string errorDescription { get; set; }
        public string errorMessage { get; set; }
        public int errorCode { get; set; }

    }
}

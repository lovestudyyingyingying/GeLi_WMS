using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMItemEntity
{
    public class CRMTokenResult
    {
        public string corpAccessToken { get; set; }
        public string corpId { get; set; }
        public int expiresIn { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public string traceId { get; set; }
    }
}

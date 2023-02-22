using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMItemEntity
{
    public class CRMItemRequest
    {

        public string corpAccessToken { get; set; }
        public string corpId { get; set; }

        public string currentOpenUserId = "FSUID_9D013E3437E2C860E6E5B27BE5D52B6A";
        public RequestData data { get; set; }
    }
    

    public class RequestData
    {
        public Search_Query_Info search_query_info { get; set; }
        public string dataObjectApiName { get; set; }
    }

    public class Search_Query_Info
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public Filter[] filters { get; set; }
    }

    public class Filter
    {
        public string field_name { get; set; }
        public long[] field_values { get; set; }
        public string @operator { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.AGVOrderEntity
{
    public class OrderResult
    {
        public int code { get; set; }
        public string data { get; set; }
        public string desc { get; set; }

    }

    public class ResultStr
    {
        public static string success = "成功";
        public static string fail = "失败";

    }
}

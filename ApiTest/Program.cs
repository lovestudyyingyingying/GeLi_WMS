using GeLi_Utils.Utils.AGVUtils;
using GeLiService_WMS.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //  HttpUtils httpUtils = new HttpUtils();
            // var aa = new
            // {
            //     type = "capricorn",
            //     time = "today"
            // };

            //var aaa= httpUtils.GetHttpGet("https://api.vvhan.com/api/horoscope",aa);
            AGVOrderHelper aGVOrderHelper = new AGVOrderHelper("http://121.5.2.81:7001");
            var a =  aGVOrderHelper.GetMapInfo();


        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.AGVOrderEntity
{

    public class AGVDeviceState
    {
        public int code { get; set; }
        public Datum[] data { get; set; }
        public string desc { get; set; }
    }

    public class Datum
    {
        public string taskPath { get; set; }
        public string payLoad { get; set; }
        public string orderId { get; set; }
        public string shelfNum { get; set; }
        public string devicePosition { get; set; }
        public int[] devicePostionRec { get; set; }
        public string state { get; set; }
        public string deviceCode { get; set; }
        public string battery { get; set; }
        public string deviceName { get; set; }
        public int deviceStatus { get; set; }
    }


    

    //public class Datum
    //{
    //    public string payLoad { get; set; }
    //    public string devicePosition { get; set; }
    //    public int[] devicePostionRec { get; set; }
    //    public string state { get; set; }
    //    public string deviceCode { get; set; }
    //    public string battery { get; set; }
    //    public string deviceName { get; set; }
    //    public int deviceStatus { get; set; }
    //}
}

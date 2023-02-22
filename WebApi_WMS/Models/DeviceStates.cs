using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_WMS.Models
{
    public class DeviceStates
    {
        /// <summary>
        /// 状态码 
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 状态码 描述
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// 设备状态详情
        /// </summary>
        public List<DeviceStatesData> data { get; set; }

    }
}
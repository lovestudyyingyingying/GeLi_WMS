using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_WMS.Models
{
    public class Alarmlog
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceNum { get; set; }
        /// <summary>
        /// 报警描述
        /// </summary>
        public string alarmDesc { get; set; }
        /// <summary>
        /// 报警类型
        /// </summary>
        public int alarmType { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public int areaId { get; set; }
        /// <summary>
        /// 是否已读
        /// </summary>
        public int alarmReadFlag { get; set; }
        /// <summary>
        /// 报警位置
        /// </summary>
        public string channelDeviceId { get; set; }
       /// <summary>
       /// 报警系统
       /// </summary>
        public string alarmSource { get; set; }
        /// <summary>
        /// 建议处理
        /// </summary>
        public string channelName { get; set; }
        /// <summary>
        /// 报警日期
        /// </summary>
        public string alarmDate { get; set; }
        /// <summary>
        /// 接收报警时间
        /// </summary>
        //public DateTime recTime { get; set; }
        /// <summary>
        /// 设备序列号
        /// </summary>
        public string deviceName { get; set; }
        /// <summary>
        /// 报警等级，数字越高越严重
        /// </summary>
        public int alarmGrade { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace NanXingService_WMS.Entity.AGVApiEntity
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetType().Name}:[\r\n");
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(this.GetType()))
            {
                //Type proType = pd.PropertyType.Name == "Nullable`1" ? pd.PropertyType.GenericTypeArguments[0] : pd.PropertyType;
                sb.Append($"{pd.Name}:{pd.GetValue(this)}\r\n");
            }
            sb.Append($"]\r\n");
            return sb.ToString();
        }
    }
}
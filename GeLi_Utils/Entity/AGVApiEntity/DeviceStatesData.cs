﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace GeLiService_WMS.Entity.AGVApiEntity
{
    /// <summary>
    /// 设备状态详情
    /// </summary>
    public class DeviceStatesData
    {
        /// <summary>
        /// 设备序列号 
        /// </summary>
        public string deviceCode { get; set; }

        /// <summary>
        /// 设备负载状态 
        /// </summary>
        public string payLoad { get; set; }

        /// <summary>
        /// 设备所在二维码的 x,y 坐标，前边的值 是 x，后边的是 y 
        /// </summary>
        public int[] devicePostionRec { get; set; }

        /// <summary>
        /// 设备当前位置
        /// </summary>
        public string devicePosition { get; set; }

        /// <summary>
        /// 电池电量 
        /// </summary>
        public string battery { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName { get; set;}
        /// <summary>
        /// 设备状态：1：空闲， 3：初始化中，4： 任务中，2：故障， 0：离线，5：充电 中，7：升级中。 
        /// </summary>
        public int deviceStatus { get; set; }

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
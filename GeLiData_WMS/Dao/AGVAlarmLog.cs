namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Text;

    [Table("AGVAlarmLog")]
    public partial class AGVAlarmLog
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        [StringLength(50)]
        public string deviceNum { get; set; }

        /// <summary>
        /// 报警描述
        /// </summary>
        [StringLength(1000)]
        public string alarmDesc { get; set; }

        /// <summary>
        /// 报警类型
        /// </summary>
        public int? alarmType { get; set; }

        /// <summary>
        /// 报警位置
        /// </summary>
        public int? areaId { get; set; }

        /// <summary>
        /// 报警已处理标志
        /// </summary>
        public int? alarmReadFlag { get; set; }

        /// <summary>
        /// 通道设备ID
        /// </summary>
        [StringLength(50)]
        public string channelDeviceId { get; set; }

        /// <summary>
        /// 报警来源
        /// </summary>
        [StringLength(50)]
        public string alarmSource { get; set; }

        /// <summary>
        /// 设备名字
        /// </summary>
        [StringLength(256)]
        public string channelName { get; set; }

        /// <summary>
        /// 报警时间
        /// </summary>
        public DateTime? alarmDate { get; set; }

        /// <summary>
        /// 收到时间
        /// </summary>
        public DateTime recTime { get; set; }

        /// <summary>
        /// 设备名字
        /// </summary>
        [StringLength(50)]
        public string deviceName { get; set; }

        /// <summary>
        ///  报警等级
        /// </summary>
        public int? alarmGrade { get; set; }
        /// <summary>
        /// 保留字段1
        /// </summary>
        [StringLength(50)]
        public string Reserve1 { get; set; }
        /// <summary>
        /// 保留字段2
        /// </summary>
        [StringLength(50)]
        public string Reserve2 { get; set; }
        /// <summary>
        /// 保留字段3
        /// </summary>
        [StringLength(50)]
        public string Reserve3 { get; set; }
        /// <summary>
        /// 保留字段4
        /// </summary>
        [StringLength(50)]
        public string Reserve4 { get; set; }
        /// <summary>
        /// 保留字段5
        /// </summary>
        [StringLength(50)]
        public string Reserve5 { get; set; }

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

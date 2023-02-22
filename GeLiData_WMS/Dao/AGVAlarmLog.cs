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
        /// �豸���
        /// </summary>
        [StringLength(50)]
        public string deviceNum { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [StringLength(1000)]
        public string alarmDesc { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public int? alarmType { get; set; }

        /// <summary>
        /// ����λ��
        /// </summary>
        public int? areaId { get; set; }

        /// <summary>
        /// �����Ѵ����־
        /// </summary>
        public int? alarmReadFlag { get; set; }

        /// <summary>
        /// ͨ���豸ID
        /// </summary>
        [StringLength(50)]
        public string channelDeviceId { get; set; }

        /// <summary>
        /// ������Դ
        /// </summary>
        [StringLength(50)]
        public string alarmSource { get; set; }

        /// <summary>
        /// �豸����
        /// </summary>
        [StringLength(256)]
        public string channelName { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? alarmDate { get; set; }

        /// <summary>
        /// �յ�ʱ��
        /// </summary>
        public DateTime recTime { get; set; }

        /// <summary>
        /// �豸����
        /// </summary>
        [StringLength(50)]
        public string deviceName { get; set; }

        /// <summary>
        ///  �����ȼ�
        /// </summary>
        public int? alarmGrade { get; set; }
        /// <summary>
        /// �����ֶ�1
        /// </summary>
        [StringLength(50)]
        public string Reserve1 { get; set; }
        /// <summary>
        /// �����ֶ�2
        /// </summary>
        [StringLength(50)]
        public string Reserve2 { get; set; }
        /// <summary>
        /// �����ֶ�3
        /// </summary>
        [StringLength(50)]
        public string Reserve3 { get; set; }
        /// <summary>
        /// �����ֶ�4
        /// </summary>
        [StringLength(50)]
        public string Reserve4 { get; set; }
        /// <summary>
        /// �����ֶ�5
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

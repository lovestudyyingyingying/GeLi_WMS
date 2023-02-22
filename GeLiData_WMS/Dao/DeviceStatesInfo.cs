namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DeviceStatesInfo")]
    public partial class DeviceStatesInfo
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string deviceCode { get; set; }

        [StringLength(50)]
        public string payLoad { get; set; }

        [StringLength(50)]
        public string devicePostionRec { get; set; }

        [StringLength(50)]
        public string devicePosition { get; set; }

        [StringLength(50)]
        public string battery { get; set; }

        [StringLength(50)]
        public string deviceName { get; set; }

        public int? deviceStatusInt { get; set; }

        [StringLength(50)]
        public string deviceStatus { get; set; }

        public DateTime recTime { get; set; }

        [StringLength(10)]
        public string devicePostionX { get; set; }

        [StringLength(10)]
        public string devicePostionY { get; set; }
        /// <summary>
        /// ±£Áô×Ö¶Î1
        /// </summary>
        [StringLength(50)]
        public string Reserve1 { get; set; }
        /// <summary>
        /// ±£Áô×Ö¶Î2
        /// </summary>
        [StringLength(50)]
        public string Reserve2 { get; set; }
        /// <summary>
        /// ±£Áô×Ö¶Î3
        /// </summary>
        [StringLength(50)]
        public string Reserve3 { get; set; }
        /// <summary>
        /// ±£Áô×Ö¶Î4
        /// </summary>
        [StringLength(50)]
        public string Reserve4 { get; set; }
        /// <summary>
        /// ±£Áô×Ö¶Î5
        /// </summary>
        [StringLength(50)]
        public string Reserve5 { get; set; }
    }
}

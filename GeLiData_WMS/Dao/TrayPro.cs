namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrayPro")]
    public partial class TrayPro
    {
        [Key]
        public int ID { get; set; }

        public DateTime optdate { get; set; }

        public int TrayStateID { get; set; }

        [StringLength(60)]
        public string prosn { get; set; }

        public virtual TrayState TrayState { get; set; }
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
    }
}

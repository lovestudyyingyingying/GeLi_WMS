namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrayState")]
    public partial class TrayState
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TrayState()
        {
            TrayPro = new HashSet<TrayPro>();
        }
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string TrayNO { get; set; }

        public DateTime optdate { get; set; }

        public int OnlineCount { get; set; }

        [ForeignKey("WareLocation")]
        public int? WareLocation_ID { get; set; }

        [StringLength(50)]
        public string proname { get; set; }

        [StringLength(50)]
        public string itemno { get; set; }

        [StringLength(20)]
        public string spec { get; set; }

        [StringLength(100)]
        public string batchNo { get; set; }

        [StringLength(10)]
        public string boxName { get; set; }

        [StringLength(10)]
        public string color { get; set; }

        [StringLength(10)]
        public string probiaozhun { get; set; }

        [StringLength(20)]
        public string position { get; set; }

        public string remark { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrayPro> TrayPro { get; set; }

        //public virtual List<WareLocation> WareLocationList { get; set; }

        //[ForeignKey("WareLocation_ID")]
        public virtual WareLocation WareLocation { get;set;}
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

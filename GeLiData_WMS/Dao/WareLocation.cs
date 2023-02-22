namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WareLocation")]
    public partial class WareLocation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WareLocation()
        {
            //TrayState = new HashSet<TrayState>();
        }
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string WareLocaNo { get; set; }

        [Index]
        [StringLength(50)]
        public string WareLoca_Lie { get; set; }

        [StringLength(50)]
        public string WareLoca_Index { get; set; }

        public int? WareArea_ID { get; set; }

        public int? header_ID { get; set; }

        [StringLength(50)]
        public string AGVPosition { get; set; }

        [StringLength(10)]
        public string WareLocaState { get; set; }

        public int? LockHis_ID { get; set; }

        [StringLength(50)]
        public string BatchNo { get; set; }

        public int? TrayState_ID { get; set; }

        public int? IsOpen { get; set; }

        // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ForeignKey("TrayState_ID")]
        public virtual TrayState TrayState { get; set; }

        [ForeignKey("header_ID")]
        public virtual Users Users { get; set; }

        [ForeignKey("WareArea_ID")]
        public virtual WareArea WareArea { get; set; }

        [ForeignKey("LockHis_ID")]
        public virtual WareLoactionLockHis WareLoactionLockHis { get; set; }
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

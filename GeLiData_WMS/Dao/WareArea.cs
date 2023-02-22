namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WareArea")]
    public partial class WareArea
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WareArea()
        {
            WareLocation = new HashSet<WareLocation>();
        }
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string WareNo { get; set; }

        public int? War_ID { get; set; }

        public int? WareHouse_ID { get; set; }

        public bool? WareAreaState { get; set; }

        [StringLength(10)]
        public string InstockRule { get; set; }

        /// <summary>
        /// 用于区分冷热
        /// </summary>
        [StringLength(10)]
        public string protype { get; set; }

        [StringLength(10)]
        public string InstockWay { get; set; }

        public virtual WareAreaClass WareAreaClass { get; set; }

        public virtual WareHouse WareHouse { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WareLocation> WareLocation { get; set; }
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
    }
}

namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Menus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Menus()
        {
            Menus1 = new HashSet<Menus>();
        }
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string ImageUrl { get; set; }

        [StringLength(200)]
        public string NavigateUrl { get; set; }

        [StringLength(500)]
        public string Remark { get; set; }

        public int SortIndex { get; set; }

        public int? ParentID { get; set; }

        public int? ViewPowerID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Menus> Menus1 { get; set; }

        public virtual Menus Menus2 { get; set; }

        public virtual Powers Powers { get; set; }
    }
}

namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Depts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Depts()
        {
            Depts1 = new HashSet<Depts>();
            Users = new HashSet<Users>();
        }
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        public int SortIndex { get; set; }

        [StringLength(500)]
        public string Remark { get; set; }

        public int? ParentID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Depts> Depts1 { get; set; }

        public virtual Depts Depts2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Users> Users { get; set; }
    }
}

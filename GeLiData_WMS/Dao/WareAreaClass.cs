namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WareAreaClass")]
    public partial class WareAreaClass
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WareAreaClass()
        {
            WareArea = new HashSet<WareArea>();
        }
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string AreaClass { get; set; }

        public int? SortIndex { get; set; }

        public bool? IsOpen { get; set; }
        /// <summary>
        /// ������Ļ��֣�������Ϊ0��������Ϊ1��������Ϊ2
        /// </summary>
        [StringLength(50)]
        public string Remark { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WareArea> WareArea { get; set; }
        /// <summary>
        /// �Ͳ��߹����ı�ע
        /// </summary>
        [StringLength(50)]
        public string Reserve1 { get; set; }
        /// <summary>
        /// ��������ʹ�ã����ڹ�����һ������
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

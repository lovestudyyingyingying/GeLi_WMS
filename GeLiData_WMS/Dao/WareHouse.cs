namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WareHouse")]
    public partial class WareHouse
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public WareHouse()
        //{
        //    WareArea = new HashSet<WareArea>();
        //}
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string WHName { get; set; }

        [StringLength(50)]
        public string WHPosition { get; set; }

        public bool? WHState { get; set; }

        [StringLength(50)]
        public string Remark { get; set; }
        /// <summary>
        /// AGVͬ¥�����ģ��
        /// </summary>
        [StringLength(20)]
        public string AGVModelCode { get; set; }
        /// <summary>
        /// ¥��AGV�ķ�����
        /// </summary>
        [StringLength(20)]
        public string AGVServerIP { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<WareArea> WareArea { get; set; }
        public virtual List<AGVRunModel> AGVRunModel { get; set; }
        /// <summary>
        /// �����ֶ�1(����¥��)
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

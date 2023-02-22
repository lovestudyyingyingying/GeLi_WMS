namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FaHuoPlan")]
    public partial class FaHuoPlan
    {
        [Key]
        public int ID { get; set; }

        [StringLength(100)]
        public string danjutype { get; set; }

        [StringLength(100)]
        public string danjuno { get; set; }

        [StringLength(100)]
        public string itemno { get; set; }

        [StringLength(100)]
        public string itemname { get; set; }

        [StringLength(100)]
        public string spec { get; set; }

        [StringLength(100)]
        public string saleunit { get; set; }

        public decimal? salequt { get; set; }

        public decimal? outqut { get; set; }

        public decimal? boxnum { get; set; }

        public DateTime? fhdate { get; set; }
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

namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StockPlan")]
    public partial class StockPlan
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string PlanNo { get; set; }

        [StringLength(50)]
        public string proname { get; set; }

        [StringLength(50)]
        public string batchNo { get; set; }

        [StringLength(50)]
        public string probiaozhun { get; set; }

        [StringLength(50)]
        public string spec { get; set; }

        public decimal? count { get; set; }

        public DateTime plantime { get; set; }

        [StringLength(50)]
        public string planUser { get; set; }

        [StringLength(50)]
        public string states { get; set; }

        [StringLength(50)]
        public string color { get; set; }

        [StringLength(10)]
        public string mark { get; set; }

        [StringLength(20)]
        public string position { get; set; }
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

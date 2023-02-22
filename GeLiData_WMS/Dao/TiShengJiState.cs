namespace GeLiData_WMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TiShengJiState")]
   
    public partial class TiShengJiState
    {
        [Key]
        public int ID { get; set; }
        [StringLength(25)]
        public string TsjIp{ get; set; }

       public DateTime? InputTime { get; set; }

        [StringLength(20)]
        public string deviceState { get; set; }

        [StringLength(20)]
        public string carState { get; set; }

        [StringLength(20)]
        public string carTarget { get; set; }

        public int? CarCount { get; set; }

        public int? F1Count { get; set; }

        public int? F2Count { get; set; }

        public int? F3Count { get; set; }

        [StringLength(20)]
        public string CarState2 { get; set; }

        [StringLength(20)]
        public string F1State { get; set; }

        [StringLength(20)]
        public string F2State { get; set; }

        [StringLength(20)]
        public string F3State { get; set; }


        [StringLength(20)]
        public string F1DuiJieWei { get; set; }

        [StringLength(20)]
        public string F2DuiJieWei { get; set; }

        [StringLength(20)]
        public string F3DuiJieWei { get; set; }

        /// <summary>
        /// ÊÕµ½µÄ¼ÇÂ¼
        /// </summary>
        [StringLength(20)]
        public string OrderReceive { get; set; }
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

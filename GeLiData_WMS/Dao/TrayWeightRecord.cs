using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiData_WMS
{
    [Table("TrayWeightRecord")]
    public class TrayWeightRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(50)]
        public string Prosn { get; set; }
        [StringLength(50)]

        public string BatchNo { get; set; }

        [StringLength(50)]
        public string Position { get; set; }

        [StringLength(200)]
        public string Proname { get; set; }

        [StringLength(50)]
        public string Spec { get; set; }

        [StringLength(50)]
        public string Biaozhun { get; set; }

        [StringLength(10)]
        public string Result { get; set; }
        public Decimal TrayCount { get; set; }
        public Decimal TrayWeight { get; set; }

        [StringLength(100)]
        public string BoxName { get; set; }

        [StringLength(20)]
        public string Color { get; set; }

        [StringLength(50)]
        public string Itemno { get; set; }
        public DateTime RecTime { get; set; }

        public int? Rec_UserID { get; set; }
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

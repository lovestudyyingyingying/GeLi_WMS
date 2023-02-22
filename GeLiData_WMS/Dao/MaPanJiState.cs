using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiData_WMS.Dao
{
    [Table("MaPanJiState")]
    /// <summary>
    /// 提升机流水表
    /// </summary>
    public class MaPanJiState
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 输入时间
        /// </summary>
        public DateTime InputTime { get; set; }

        /// <summary>
        /// 处于叠板状态(M350)
        /// </summary>
        public bool IsDieBan { get; set; }

        /// <summary>
        /// 是否处于叠板状态(M351)
        /// </summary>
        public bool IsDieBanReadyAndAllowIn { get; set; }

        /// <summary>
        /// 是否正在叠板(M352)
        /// </summary>
        public bool IsDieBaning { get; set; }

        /// <summary>
        /// 是否叠板完成(M357)
        /// </summary>
        public bool IsDieBanEnd { get; set; }

        /// <summary>
        /// 是否处于拆板状态(M353)
        /// </summary>
        public bool IsChaiBan { get; set; }

        /// <summary>
        /// 是否处于叠板状态(M354)
        /// </summary>
        public bool IsChaiBanReadyAndAllowIn { get; set; }

        /// <summary>
        /// 是否正在叠板(M355)
        /// </summary>
        public bool IsChaiBaning { get; set; }

        /// <summary>
        /// 是否叠板完成(M356)
        /// </summary>
        public bool IsChaiBanEnd { get; set; }

        /// <summary>
        /// 库内数量(数量)
        /// </summary>
        public int BanNum { get; set; }

        /// <summary>
        /// 状态总结
        /// </summary>
        [StringLength(100)]
        public string Reserve1 { get; set; }
        [StringLength(100)]
        public string Reserve2 { get; set; }
        [StringLength(100)]
        public string Reserve3 { get; set; }
        [StringLength(100)]
        public string Reserve4 { get; set; }
        [StringLength(100)]
        public string Reserve5 { get; set; }


    }
}

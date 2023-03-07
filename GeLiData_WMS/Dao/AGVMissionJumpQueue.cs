using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiData_WMS.Dao
{
    /// <summary>
    /// 任务插队表
    /// </summary>
    public class AGVMissionJumpQueue
    {
        [Key]
        public int ID { get; set; }

        [StringLength(20)]
        public string MissionNo { get; set; }

        public DateTime? InsertTime { get; set; }

        [StringLength(50)]
        public string TrayNo { get; set; }

        [StringLength(50)]
        public string Mark { get; set; }

        [StringLength(50)]
        public string StartLocation { get; set; }

        [StringLength(50)]
        public string StartPosition { get; set; }

        /// <summary>
        /// 目标区域
        /// </summary>
        [StringLength(50)]
        public string TargetArea { get; set; }


        [StringLength(10)]
        public string userId { get; set; }


     
        /// <summary>
        /// 是否处理并发送成功
        /// </summary>
        public bool? IsSendSuccess { get; set; }

        public DateTime? SendTime { get; set; }
        /// <summary>
        /// 为它分配了什么点位
        /// </summary>
        [StringLength(50)]
        public string EndLocation { get; set; }

        [StringLength(50)]
        public string EndPosition { get; set; }

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

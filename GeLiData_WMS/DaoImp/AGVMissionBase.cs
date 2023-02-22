using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiData_WMSImp
{
    public class AGVMissionBase
    {
        [Key]
        public int ID { get; set; }

        [StringLength(20)]
        public string MissionNo { get; set; }

        public DateTime? OrderTime { get; set; }

        [StringLength(50)]
        public string TrayNo { get; set; }

        [StringLength(50)]
        public string Mark { get; set; }

        [StringLength(50)]
        public string StartLocation { get; set; }

        [StringLength(50)]
        public string StartPosition { get; set; }

        [StringLength(50)]
        public string StartMiddleLocation { get; set; }

        [StringLength(50)]
        public string StartMiddlePosition { get; set; }

        [StringLength(50)]
        public string EndLocation { get; set; }

        [StringLength(50)]
        public string EndPosition { get; set; }

        [StringLength(50)]
        public string EndMiddleLocation { get; set; }

        [StringLength(50)]
        public string EndMiddlePosition { get; set; }

        [StringLength(50)]
        public string SendState { get; set; }

        [StringLength(100)]
        public string SendMsg { get; set; }

        [StringLength(100)]
        public string StateMsg { get; set; }

        [StringLength(50)]
        public string RunState { get; set; }

        public DateTime? StateTime { get; set; }

        public int? StockPlan_ID { get; set; }

        [StringLength(255)]
        public string OrderGroupId { get; set; }

        [StringLength(20)]
        public string ModelProcessCode { get; set; }

        [StringLength(20)]
        public string AGVCarId { get; set; }

        [StringLength(10)]
        public string userId { get; set; }

        public int? MissionFloor_ID { get; set; }

        [StringLength(50)]
        public string Remark { get; set; }

        /// <summary>
        /// 节点时间
        /// </summary>
        public DateTime? NodeTime { get; set; }

        public int? IsFloor { get; set; }

        /// <summary>
        /// 发送AGV的地址
        /// </summary>
        [StringLength(50)]
        public string SendAGVPoStr { get; set; }

        /// <summary>
        /// 请求API返回的地址
        /// </summary>
        [StringLength(50)]
        public string ApiReturnPoStr { get; set; }
        /// <summary>
        /// 保留字段1(当前工序名)
        /// </summary>
        [StringLength(50)]
        public string Reserve1 { get; set; }
        /// <summary>
        /// 保留字段2(暂存下任务时的起始仓库)
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

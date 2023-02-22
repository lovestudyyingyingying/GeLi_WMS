using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GeLiData_WMS
{
    [Table("StockRecord")]
    public class StockRecord
    {
        [Key]
        public int ID { get; set; }

        //任务号、条码号、产品名称、批次号、数量、
        //方式（自动出仓、自动进仓、同层进仓、同层出库、手动出仓、人工干预）
        //下发时间、完成时间、起点仓位、目标仓位
        //下发人员、执行AGV
        [StringLength(20)]
        public string MissionNo { get; set; }

        [StringLength(50)]
        public string TrayNo { get; set; }

        [StringLength(100)]
        public string ProName { get; set; }

        [StringLength(50)]
        public string BatchNo { get; set; }

        public decimal ProCount { get; set; }

        /// <summary>
        /// 出入库类型：只记录出仓、进仓 or  调拨
        /// </summary>
        [StringLength(20)]
        public string StockType { get; set; }

        /// <summary>
        /// 方式（自动出仓、自动进仓、同层进仓、同层出库、同层调拨、跨层调拨、手动出仓、手动进仓、任务失败-人工干预）
        /// </summary>
        [StringLength(30)]
        public string StockTypeDesc { get; set; }

        //下发时间、完成时间、起点仓位、目标仓位
        [StringLength(20)]
        public string StartLocation { get; set; }

        [StringLength(20)]
        public string EndLocation { get; set; }

        public DateTime OrderTime { get; set; }

        public DateTime FinishTime { get; set; }

        public DateTime RecordTime { get; set; }

        //下发人员、执行AGV
        [StringLength(10)]
        public string OrderUser { get; set; }
        [StringLength(20)]
        public string OrderAGV { get; set; }
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

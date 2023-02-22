using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.ProPlanEntity
{
    public  class ProPlanEditDto_Small
    {
        public int ID { get; set; }

        public string PlanOrderNo { get; set; }
        public DateTime? Newdate { get; set; }

        public DateTime? Moddate { get; set; }
        /// <summary>
        /// 合并时间
        /// </summary>
        public DateTime? Optdate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// 排产单类型：大包装排产单/小包装排产单
        /// </summary>

        public string PositionClass { get; set; }

        public string Priority { get; set; }
        public string Remark { get; set; }


        public string mergeCellsValue { get; set; }
        public string mergeCells { get; set; }


        public string Workshops { get; set; }

        public string OrderNo { get; set; }

        //记录成品信息

        public string Itemno { get; set; }
        public string Itemno2 { get; set; }
        public string ItemName { get; set; }

        public string Spec { get; set; }

        public string Color { get; set; }

        public string ProductionClass { get; set; }

        public string Unit { get; set; }

        public decimal? PcCount { get; set; }
        public decimal? PcCount_03_Tank { get; set; }
        public decimal? PcCount_03_Bag { get; set; }
        public decimal? PcCount_03_Box { get; set; }
        public decimal? PcCount_07_Tank { get; set; }
        public decimal? PcCount_07_Bag { get; set; }
        public decimal? PcCount_07_Box { get; set; }

        public DateTime? PlanDate { get; set; }

        public string BatchNo { get; set; }

        public string Biaozhun { get; set; }

        public string Jingbanren { get; set; }

        public string Clientname { get; set; }

        public long? CRMPlanList_ID { get; set; }

        public string BoxNo { get; set; }

        public string BoxName { get; set; }

        public string BoxRemark { get; set; }
    }
}

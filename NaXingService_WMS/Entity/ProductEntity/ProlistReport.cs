using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.ProductEntity
{
    /// <summary>
    /// 生产明细报表
    /// </summary>
    public class ProlistReport
    {
        public int ID { get; set; }
        public int Index { get; set; }
        public string ProductOrder_XuHao { get; set; }
        public string ProductOrderNo { get; set; }
        public string ProPlanOrder_XuHao { get; set; }
        public string YanBanCount { get; set; }
        public string YanBanUnit { get; set; }

        public string Itemno { get; set; }
        public string ItemName { get; set; }
        public string CheJianName { get; set; }
        public string CheJianNameStr { get { return ConvertCheJian(); } }

        public string Spec { get; set; }
        
        public DateTime? ApplyTime { get; set; }
        public string ApplyTimeStr { get { return ApplyTime.HasValue ? ApplyTime.Value.ToString("MM-dd") : string.Empty; } }
        public DateTime? PlanDate { get; set; }
        public string PlanDateStr { get { return PlanDate.HasValue ? PlanDate.Value.ToString("MM-dd") : string.Empty; } }

        public DateTime? DeliveryDate { get; set; }
        public string DeliveryDateStr { get { return DeliveryDate.HasValue ? DeliveryDate.Value.ToString("MM-dd") : string.Empty; } }

        public DateTime? PlanProDate { get; set; }
        public string PlanProDateStr { get { return PlanProDate.HasValue ? PlanProDate.Value.ToString("MM-dd") : string.Empty; } }

        public DateTime? FinishTime { get; set; }
        public string FinishTimeStr { get { return FinishTime.HasValue ? FinishTime.Value.ToString
                    ("MM-dd") : string.Empty; } }


        public string PcCQ { get; set; }
        public string JFCQ { get; set; }

        public string ConvertRate { get; set; }

        public decimal? PcCount { get; set; }
        public decimal? PCCount { get; set; }
        public decimal? ProCount { get; set; }
        public decimal? PcCountOnKg { get; set; }
        public decimal? ProCountOnKg { get; set; }
        public decimal? PCCountOnKg { get; set; }
        
        public string PCUnit { get; set; }
        public string Unit { get; set; }
        public decimal WCL { get; set; }
        public string WCLStr { get { return this.WCL.ToString("F0")+"%"; } }
        public string ProOrderList_State { get; set; }

        public string PlanProTime { get; set; }

        public string UploadRen { get; set; }

        public DateTime? UploadTime { get; set; }
        public string ProductOrder_State { get; set; }

        private string ConvertCheJian()
        {
            if (CheJianName == "03小包装-小袋")
                return "03小袋";
            else if (CheJianName == "03小包装-罐")
                return "03罐";
            else if (CheJianName == "03小包装-每日坚果")
                return "03坚果";
            else if(CheJianName == "07小包装-小袋")
                return "07小袋";
            else if (CheJianName == "07小包装-罐")
                return "07罐";
            else if (CheJianName == "07小包装-每日坚果")
                return "07坚果";
            return string.Empty;
        }
    }
}

using NanXingData_WMS.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity
{
    public class CRMApplyIndexData
    {
        /// <summary>
        /// 
        /// </summary>
        public long ID { get; set; }
        public string CRMApplyNo { get; set; }
        public string HalfProState { get; set; }
        public string CRMApplyNo_Xuhao { get; set; }
        public string ClientName { get; set; }
        public string Reserve9 { get; set; }
        public string Reserve10 { get; set; }
        
        public string Ingredients { get; set; }
        public string GiveDept { get; set; }
        public string Reserve11 { get; set; }
        
        public string ApplicantName { get; set; }
        /// <summary>
        /// 下推日期
        /// </summary>
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// 交付日期
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        public string DeliveryDateStr
        {
            get
            {
                return DeliveryDate.HasValue? DeliveryDate.Value.Date.ToString("yyyy-MM-dd"):string.Empty;
            }
        }
        public DateTime? ApplyTime { get; set; }
        public string ApplyNoState { get; set; }
        public string EmergencyDegree { get; set; }
        public string ItemNo { get; set; }
        public string Itemno { get; set; }
        public string ItemName { get; set; }
        public string CRMApplyList_InCode { get; set; }
        public string PlanTime { get; set; }
        public string Spec { get; set; }
        public int OrderCount { get; set; }
        public string Unit { get; set; }
        public string InventoryCount { get; set; }
      
        public string Biaozhun { get; set; }
        public string ProductRecipe { get; set; }
        public string BoxNo { get; set; }
        public string BoxName { get; set; }
        public string BoxRemark { get; set; }

        public string Remark { get; set; }
        public string ApplicantDept { get; set; }
        public string Reserve1 { get; set; }
        public string YanBanCount { get; set; }
        public string YanBanUnit { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
        public List<ProPlanOrderlists> ProPlanOrderlists { get; set; }


        //产品别名
        public string InName { get; set; }
        //原料名
        public string MaterialItem { get; set; }


        //排产单号
        public string PlanOrder_XuHao { get; set; }
        //排产数量
        public decimal? PcCount { get; set; }

        //排产单位
        public string PcUnit { get; set; }

        //批号
        public string BatchNo { get; set; }
        //排产状态
        public string crmListStatus { get; set; }
        //排产日期
        public DateTime? PlanDate { get; set; }

        public DateTime? PlanNewdate { get; set; }

        //排产人员
        public string Jingbanren { get; set; }

        //public string ListStr  { 
        //    get{
        //        //return InitList();
        //    } 
        //}
        
        private string InitList()
        {
            //this.ListStr = string.Empty;
            string[] list = new string[ProPlanOrderlists.Count];
            for(int i=0;i< ProPlanOrderlists.Count; i++)
            {
                list[i] = ProPlanOrderlists[i].PlanOrder_XuHao;
            }
            return string.Join(",", list);
        }
    }
}
// ProOrderCountReport-SmallBox-03
// ProOrderListReport-SmallBox
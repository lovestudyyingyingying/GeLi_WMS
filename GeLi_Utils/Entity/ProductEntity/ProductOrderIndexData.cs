using GeLiService_WMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity
{
    public class ProductOrderIndexData
    {
        public int ID { get; set; }
        public string PlanOrderNo { get; set; }
        public string YanBanCount { get; set; }
        public string YanBanUnit { get; set; }
        public string Reserve2 { get; set; }
        public string ClientName { get; set; }
        public string Spec { get; set; }
        public string Reserve1 { get; set; }
        public string ApplyNoState { get; set; }
        public DateTime? Newdate { get; set; }

        public DateTime? Moddate { get; set; }

        public DateTime? Optdate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? PlanDate { get; set; }


        public string PositionClass { get; set; }
        public string Remark { get; set; }
        public string orderNo { get; set; }
        public string mergeCells { get; set; }
        public string mergeCellsValue { get; set; }
        public string Workshops { get; set; }
        public string WorkshopsValue { get; set; }
        public decimal NoWorkCount { get; set; }
        public string PrintState { get; set; }
        public string ProductState { get; set; }
        public string JingBanRen { get; set; }
        public string CRMXuHao { get; set; }
        public string CRMHeadNo { get; set; }
        
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<ProPlanOrderlists> ProPlanOrderlists { get; set; }
        
        /// <summary>
        /// 该排产单所有的产品名称
        /// </summary>
        public string ItemNameStr
        {
            get
            {
                return InitItemNameStr();
            }
            set
            {
                InitItemNameStr();
            }
        }
        public string InitItemNameStr()
        {
            if(ProPlanOrderlists!=null && ProPlanOrderlists.Count>0)
            {
                List<string> list = new List<string>();
                for (int i = 0; i < ProPlanOrderlists.Count; i++)
                {
                    if (!list.Contains(ProPlanOrderlists[i].ItemName))
                    {
                        list.Add(ProPlanOrderlists[i].ItemName);
                    }
                }
                return string.Join(",", list.ToArray());
            }
            return string.Empty;
        }



       
    }
}

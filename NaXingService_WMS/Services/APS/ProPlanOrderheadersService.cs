using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity;
using NanXingService_WMS.Entity.ProductEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services
{
    public class ProPlanOrderheaderService:DbBase<ProPlanOrderheaders>
    {
        /// <summary>
        /// 查询排产单表头数据
        /// </summary>
        /// <param name="whereLambda">筛选条件</param>
        /// <returns>IQueryable数据</returns>
        public IQueryable<ProductOrderIndexData> QueryIndex(Expression<Func<ProPlanOrderheaders, bool>> whereLambda)
        {
            //return proOrderHeadService.GetIQueryable(whereLambda);

            var list = SelectToList(whereLambda, u => new ProductOrderIndexData
            {
                ID = u.ID,
                PlanOrderNo = u.PlanOrderNo,
                Newdate = u.Newdate,
                Moddate = u.Moddate,
                Optdate = u.Optdate,
                PositionClass = u.PositionClass,
                Remark = u.Remark,
                mergeCells = u.mergeCells,
                mergeCellsValue = u.mergeCellsValue,
                Workshops = u.Workshops ?? string.Empty,
                DeliveryDate = u.crmPlanList != null ? u.crmPlanList.DeliveryDate : null,
                //WorkshopsValue = u.WorkshopsValue,
                ProPlanOrderlists = u.ProPlanOrderlists,
                ItemNameStr = string.Empty,
                NoWorkCount = 0,
                PrintState = "未打印",
                Spec = u.Spec,
                ApplyNoState = u.crmPlanList.ApplyNoState,

                Reserve2 = u.Reserve2,
                ClientName = u.Clientname,
                Reserve1 = u.crmPlanList.CRMPlanHead.Reserve1,
                ProductState = string.Empty,
                CRMHeadNo = u.crmPlanList != null ? u.crmPlanList.CRMPlanHead.CRMApplyNo : string.Empty,
                CRMXuHao = u.crmPlanList != null ? u.crmPlanList.CRMApplyNo_Xuhao : string.Empty,
                YanBanUnit = u.ProPlanOrderlists.FirstOrDefault().Reserve2
            }); 
            list.ForEach((item) =>
            {
                item.InitItemNameStr();

                decimal allcount = 0;
                decimal allYBcount = 0;
                item.Remark=item.Remark ?? string.Empty;
                //string proState = string.Empty;
                item.ProPlanOrderlists.ForEach((orderItem) =>
                {
                    if (orderItem.ProPlanOrderheaders.DeliveryDate!=null&& item.DeliveryDate==null)
                    {
                        item.DeliveryDate = orderItem.ProPlanOrderheaders.DeliveryDate;
                    }
                    if (string.IsNullOrEmpty(item.JingBanRen))
                        item.JingBanRen = orderItem.Jingbanren;
                    if (item.PlanDate==null)
                        item.PlanDate = orderItem.PlanDate;
                    if (orderItem.PlanOrder_State != "已删除")
                    {
                        decimal noworkcount = orderItem.PcCount ?? 0 - (decimal)(orderItem.ProductOrderlists == null|| orderItem.ProductOrderlists.Count == 0 ? 0 : orderItem.ProductOrderlists[0].ProCount ?? 0);
                        allcount += orderItem.PcCount ?? 0;
                        item.NoWorkCount += noworkcount < 0 ? 0 : ((int)noworkcount);

                        decimal danBanCount = Convert.ToDecimal(orderItem.Reserve1) - (decimal)(orderItem.ProductOrderlists == null || orderItem.ProductOrderlists.Count == 0 ? 0 : Convert.ToDecimal(orderItem.ProductOrderlists[0].TemplateNum)); 
                         allYBcount += Convert.ToDecimal(danBanCount) < 0 ? 0 : Convert.ToDecimal(danBanCount);
                        item.YanBanCount = (allYBcount).ToString();

                        //if (orderItem.ProductOrderlists != null && orderItem.ProductOrderlists.Count > 0)
                        //{
                        //    ret = false;
                        //}
                       
                    }
                    
                    //else if (orderItem.PlanOrder_State != "已下推")
                    //{
                    //    index++;
                    //}
                });
                //if (index==0)
                //    item.ProductState = "已删除";
                //else if (ret)
                //    item.ProductState = "已排产";
                //else if (item.NoWorkCount == 0)
                //    item.ProductState = "已完成";
                //else if (allcount == item.NoWorkCount)
                //    item.ProductState = "已下推";
                //else
                //    item.ProductState = "生产中";
            });
            var q = list.AsQueryable();
            return q;
        }
    }
}

using AutoMapper;
using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.ProductEntity;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace NanXingService_WMS.Services.APS
{
    public class ProPlanOrderlistsService:DbBase<ProPlanOrderlists>
    {
        ProPlanOrderlistsService _planListsService;
        
        ProductOrderlistsService productListsService;
        
        RedisHelper redisHelper = new RedisHelper();
    

        public IQueryable<ProPlanOrderIndexData> GetIndexData(Expression<Func<ProPlanOrderlists, bool>> exp)
        {
            return SelectToQuery(exp,
                u => new ProPlanOrderIndexData
                {
                    ID = u.ID,
                    PlanOrder_XuHao = u.PlanOrder_XuHao,
                    Chejianclass = u.Chejianclass,
                    ItemName = u.ItemName,
                    DeliveryDate = u.ProPlanOrderheaders.DeliveryDate,
                    ProductOrderlists = u.ProductOrderlists,
                    Spec = u.Spec,
                    PlanDate = u.PlanDate,
                    PlanTime = u.PlanTime,
                    JingBanRen = u.Jingbanren,
                    PcCount = ((int?)u.PcCount),
                    Unit = u.Unit,
                    Remark = u.Remark,
                    BoxNo = u.BoxNo ?? string.Empty,
                    BoxName = u.BoxName ?? string.Empty,
                    BoxRemark = u.BoxRemark ?? string.Empty,
                    PlanOrder_State = u.PlanOrder_State,
                    HalfProState = u.HalfProState,
                    Reserve1 = u.crmPlanList.CRMPlanHead.Reserve1,
                    ItemNo = u.Itemno,
                    Biaozhun = u.crmPlanList.Biaozhun == null ? u.Biaozhun : u.crmPlanList.Biaozhun,
                    ProductRecipe = u.crmPlanList.ProductRecipe,
                    YanBanUnit = u.Reserve2,
                    YanBanCount = u.Reserve1

                }) ;
        }


        public List<PrintData_SmallBox> GetPrintItem(
            Expression<Func<ProPlanOrderlists,bool>> exp, 
            Expression<Func<ProPlanOrderlists, string>> ordering,
            string path)
        {
            var q = SelectToList(exp,
               u => new PrintData_SmallBox
               {
                   ID=u.ID,
                   PlanOrderNo = u.ProPlanOrderheaders.PlanOrderNo,
                   PlanOrder_XuHao = u.PlanOrder_XuHao,
                   ItemNo = u.Itemno,
                   ItemName = u.ItemName,
                   Spec = u.Spec,
                   Unit = u.Unit,
                   BoxNo = u.BoxNo,
                   BoxName = u.BoxName,
                   ClientName = u.crmPlanList == null ? string.Empty : u.crmPlanList.CRMPlanHead.ClientName,
                   PcCount = (decimal)u.PcCount,
                   NoWorkCount = (decimal)u.PcCount - 0,
                   NoWorkCount_OrderHeader = 0,
                   Priority=u.Priority,
                   Workshops=u.ProPlanOrderheaders.Workshops,
                   Remark=u.Remark,
                   PlanDate=u.PlanDate??DateTime.Now,
                   DeliveryDate = u.crmPlanList.DeliveryDate ?? DateTime.Now,
                   ImageUrl= "~/images/" + u.ProPlanOrderheaders.PlanOrderNo+".jpg"
               }) ;
            var qSum=q.GroupBy(u => u.PlanOrderNo).Select((u,g)=>new {
                PlanOrderNo= u.Key,
                NoWorkCount_OrderHeader= u.Sum(item=>(int)item.NoWorkCount)
            }).ToList();

            q.ForEach((item) =>
            {
                if (!File.Exists(path+item.PlanOrderNo+ ".jpg"))
                {
                    QRCodeHandler.CreateQRCode(item.PlanOrderNo, "Byte", 5, 0, "H", path + item.PlanOrderNo + ".jpg", false, string.Empty);
                }
                //decimal allCount= item.
                //decimal noworkcount = item.PcCount - item.ProPlanOrderlists.Production.Count;

                item.NoWorkCount_OrderHeader = qSum.Where(
                    u=>u.PlanOrderNo==item.PlanOrderNo).FirstOrDefault()
                    .NoWorkCount_OrderHeader;

                if (item.NoWorkCount == 0)
                    item.ProductState = "已完成";
                else if (item.PcCount == item.NoWorkCount)
                    item.ProductState = "未生产";
                else
                    item.ProductState = "生产中";
            });
            //string Path2 = "http://192.168.1.118:8019/images/" + lbProsn.Text + ".jpg";

            //List<PrintData_SmallBox> list= (List<PrintData_SmallBox>)AutoMapperHelper.MapTo<ProPlanOrderlists, PrintData_SmallBox>(q);
            return q;
        }
        // <summary>
        /// 显示CRM下推的申请单页面
        /// </summary>
        /// <param name="crmwhereLambda"></param>
        /// <param name="prowhereLambda"></param>
        /// <returns></returns>
        public IQueryable<ProPlanOrderIndexData> QueryIndex(
            Expression<Func<ProPlanOrderlists, bool>> planwhereLambda = null,
            Expression<Func<ProductOrderlists, bool>> productwhereLambda = null,
            Expression<Func<ProPlanOrderIndexData, bool>> cIndexWhereLambda = null)
        {
            ProPlanOrderlistsService proPlanOrderlistsService = new ProPlanOrderlistsService();
            _planListsService = proPlanOrderlistsService;
            ProductOrderlistsService productOrderlistsService = new ProductOrderlistsService();
            productListsService = productOrderlistsService;
            var planListQ = planwhereLambda == null ? _planListsService.GetAllQueryable(null)
                : _planListsService.GetIQueryable(planwhereLambda);
            var PROListQ = productwhereLambda == null ? productListsService.GetAllQueryable(null)
                : productListsService.GetIQueryable(productwhereLambda);


            var q = from a in planListQ
                    join b in PROListQ
                    on a.ID equals (b.ProPlanOrderlists_ID)
                    into ab
                    from abi in ab.DefaultIfEmpty()
                    select new ProPlanOrderIndexData
                    {
                        ID = a.ID,
                        PlanOrder_XuHao = a.PlanOrder_XuHao,
                        Chejianclass = a.Chejianclass,
                        ItemName = a.ItemName,
                        DeliveryDate = a.ProPlanOrderheaders.DeliveryDate,
                        ProductOrderlists = a.ProductOrderlists,
                        Spec = a.Spec,
                        PlanDate = a.PlanDate,
                        PlanTime = a.PlanTime,
                        JingBanRen = a.Jingbanren,
                        PcCount = a.PcCount,
                        Unit = a.Unit,
                        Remark = a.Remark,
                        BoxNo = a.BoxNo ?? string.Empty,
                        BoxName = a.BoxName ?? string.Empty,
                        BoxRemark = a.BoxRemark ?? string.Empty,
                        PlanOrder_State = a.PlanOrder_State,
                        HalfProState = a.HalfProState,

                        Productlocation = abi.Reserve1,
                        PCCount = abi.PcCount,
                        ProUnit = abi.Unit,
                        Ingredients = a.Ingredients,
                        GiveDept = a.GiveDept,

                        
                        
                    };
            return q;

        }

    }
}

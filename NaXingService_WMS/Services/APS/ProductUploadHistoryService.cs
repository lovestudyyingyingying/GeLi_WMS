using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsSharp;

namespace NanXingService_WMS.Services.APS
{
    public class ProductUploadHistoryService:DbBase<ProductUploadHistory>
    {
        public bool AddHis(ProductOrderlists productOrderlists,int count,string uploadUser,
            string updateBatch,string liushuihao)
        {
            ProductUploadHistory productUploadHistory = new ProductUploadHistory();
            MapperHelper<ProductOrderlists, ProductUploadHistory>.Map(productOrderlists, productUploadHistory);
            productUploadHistory.ID = 0;
            productUploadHistory.Newdate = DateTime.Now;
            productUploadHistory.Moddate = DateTime.Now;
            productUploadHistory.ProCount = count;
            productUploadHistory.UploadUser = uploadUser.Contains(":")?uploadUser.Split(':')[0]: uploadUser;
            productUploadHistory.ModUser = uploadUser.Contains(":") ? uploadUser.Split(':')[1] : uploadUser;
            productUploadHistory.UploadBatch = $"{updateBatch}-{liushuihao}";
            productUploadHistory.LiuShuiHao = liushuihao;

            Insert(productUploadHistory);
            SaveChanges();
            return true;
        }

        public int GetAllCount(string orderNo)
        {
            if (!Any(u => u.ProductOrder_XuHao == orderNo))
            {
                return 0;
            }
            return (int)GetList(u => u.ProductOrder_XuHao == orderNo, true, DbMainSlave.Master).Sum(u => u.ProCount);
        }
    }
}

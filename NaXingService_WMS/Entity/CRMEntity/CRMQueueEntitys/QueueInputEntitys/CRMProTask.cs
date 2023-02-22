using NanXingData_WMS.Dao;
using NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity;
using NanXingService_WMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys;

namespace NanXingService_WMS.Entity.CRMEntity
{
    public class CRMProTask: QueueInputEntityBase
    {
        //public string crm_ID { get; set; }

        public string proTaskNo { get; set; }
        public string taskName { get; set; }

        public string startTime { get; set; }

        public string endTime { get; set; }

        public decimal count { get; set; }

        public string unit { get; set; }   

        public string  updateUserID { get; set; }

        //public string  updateUserName { get; set; }

        public static CRMProTask CreateByProlists(ProductOrderlists productOrderlists,string userName)
        {
            return new CRMProTask()
            {
                proTaskNo=productOrderlists.ProductOrder_XuHao,
                crm_ID = productOrderlists.ProPlanOrderlists.crmPlanList.CRMApplyList_InCode,
                taskName = productOrderlists.Chejianclass,
                startTime = UnixDateTImeUtils.ConvertDateTimeInt(productOrderlists.StartTime ?? DateTime.Now).ToString(),
                endTime = UnixDateTImeUtils.ConvertDateTimeInt(productOrderlists.FinishTime??DateTime.Now).ToString(),
                unit = productOrderlists.Unit,
                count= productOrderlists.ProCount??0,
                updateUserID= userName,
            };
        }
    }
}

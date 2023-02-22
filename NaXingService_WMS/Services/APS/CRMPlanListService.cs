using NanXingData_WMS.DaoUtils;
using NanXingData_WMS.Dao;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services.APS
{
    public class CRMPlanListService:DbBase<CRMPlanList>

    {
        public string ChangeRemark(string remark, string reserve2, string reserve3)
        {
            if (string.IsNullOrEmpty(reserve2) && !string.IsNullOrEmpty(reserve3))
            {
                remark = "产期限制" + reserve3 + "天;\n" + remark;
            }
            else if (string.IsNullOrEmpty(reserve3) && !string.IsNullOrEmpty(reserve2))
            {
                remark = "最早日期" + reserve2 + ";\n" + remark;
            }
            else if (!string.IsNullOrEmpty(reserve3) && !string.IsNullOrEmpty(reserve2))
            {
                remark = "最早日期" + reserve2 + ";\n" + "产期限制" + reserve3 + "天;\n" + remark;
            }
            return remark;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.ProductEntity
{
    /// <summary>
    /// 排产计划单状态：已排产（已排产）、已下推、生产中、生产完成、生产异常、已删除
    /// </summary>
    public class ProOrderState
    {

        //public static string HasPlan = "已排产";
        public static string NoWork = "未生产";

        public static string HasProduct = "生产中";

        public static string Finish = "已完成";
        public static string ErrorFinish = "异常";

        public static string ProductError = "异常";
        public static string HasDeleted = "已删除";

    }
}

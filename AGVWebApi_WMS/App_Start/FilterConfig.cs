using System.Web;
using System.Web.Mvc;

namespace NX_WMS_TM_ApiNet
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace NX_WMS_TM_ApiNet.Filter
{
    public class ExceptionFilter: ExceptionFilterAttribute
    {
        log4net.ILog log = log4net.LogManager.GetLogger("HCController");
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            log.Info(actionExecutedContext.Exception);
          
        }
    }
}
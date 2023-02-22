using NX_WMS_TM_ApiNet.AuthAttributes;
using NX_WMS_TM_ApiNet.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NX_WMS_TM_ApiNet
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            config.EnableCors(new EnableCorsAttribute("localhost:44320,*", "*", "*"));
            config.Filters.Add(new ExceptionFilter());
            // Web API 路由
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Filters.Add(new IdentityBasicAuthentication());
        }
    }
}

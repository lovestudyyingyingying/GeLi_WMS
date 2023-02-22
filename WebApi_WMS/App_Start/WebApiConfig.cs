
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi_WMS.Utils;

namespace WebApi_WMS
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //跨域配置
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            // Web API 配置和服务
            config.Formatters.Add(new PlainTextTypeFormatter());
            // Web API 路由
            config.MapHttpAttributeRoutes();
            //if (!formatter.SupportedMediaTypes.Any(mt => mt.MediaType == "text/plain"))
            //    formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            //config.Formatters.Add(formatter);
            config.Filters.Add(new ExceptionFilter());
            //config.Formatters.Add(new PlainTextTypeFormatter());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
        }
    }
}

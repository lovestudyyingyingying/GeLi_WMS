using GeLiData_WMS;
using GeLiService_WMS.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using HttpContext = System.Web.HttpContext;

namespace AGVWebApi_WMS.Controllers
{
    [RoutePrefix("pda")]
    public class PDALoginContoller : ApiController
    {
        log4net.ILog log = log4net.LogManager.GetLogger("PDAlogin");
        public string Options()
        {
            return null;
        }

        [HttpPost]
        [HttpGet]
        [Route("dologin")]
        public void dologin( HttpContext context)
        {
            //Dictionary<string, string> jsonDict = GetDicInJson(context);
            //string loginId = jsonDict["name"];
            //string passw = jsonDict["password"];
            //Users user = userService.GetByName(loginId);

            ////context.Response.ContentType = "text/plain";

            //if (PasswordUtil.ComparePasswords(user.Password, passw))
            //{
            //    if (!Convert.ToBoolean(user.Enabled))
            //    {
            //        context.Response.Write("用户未启用，请联系管理员！");
            //    }
            //    else
            //    {
            //        // 登录成功
            //        //logger.Info(String.Format("登录成功：用户“{0}”", user.Name));
            //        context.Response.Write("success");
            //        return;
            //    }
            //}
            //else
            //{
            //    context.Response.Write("fail");
            //}
        }
    }
}
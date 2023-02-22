using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using JWT;
using JWT.Serializers; 
using System.Text;
using System.Net;
using System.Net.Http;
using NX_WMS_TM_ApiNet.Models;
using NX_WMS_TM_ApiNet.Common;
using System.Security.Principal;
using System.Security.Claims;

namespace NX_WMS_TM_ApiNet.AuthAttributes
{
    /// <summary>
    /// 身份认证拦截器
    /// </summary>
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        log4net.ILog log = log4net.LogManager.GetLogger("AuthorizeAttribute");

        /// <summary>
        /// 指示指定的控件是否已获得授权
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //var user = (ClaimsPrincipal)User;
            //var dic = new Dictionary<string, object>();
            //foreach (var userClaim in user.Claims)
            //{
            //    dic.Add(userClaim.Type, userClaim.Value);
            //}
            //actionContext.ControllerContext.RequestContext.Principal.
           //List<string> rolename;
           //string userid = "";
           //foreach (var queryNameValuePair in actionContext.Request.GetQueryNameValuePairs())
           //{
           //    if (queryNameValuePair.Key == "role")
           //    {
           //        rolename = queryNameValuePair.Value.ToString();
           //    }
           //    if (queryNameValuePair.Key == "userid")
           //    {
           //        rolename = queryNameValuePair.Value.ToString();
           //    }
           //}
           //// 这是base.IsAuthorized里的逻辑
           //IPrincipal principal = actionContext.ControllerContext.RequestContext.Principal;
           //return principal != null && principal.Identity != null
           //    && principal.Identity.IsAuthenticated &&
           //    (
           //        rolename.Any<string>(new Func<string, bool>(principal.IsInRole)))
           //     );

           // (rolename.Length <= 0 || ((IEnumerable<string>)this._usersSplit).Contains<string>(principal.Identity.Name, (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase))
           //&&
           //(this.rolename.Length <= 0 || ((IEnumerable<string>)this._rolesSplit).Any<string>(new Func<string, bool>(principal.IsInRole)))
           //前端请求api时会将token存放在名为"auth"的请求头中
           var authHeader = from t in actionContext.Request.Headers where t.Key == "token" select t.Value.FirstOrDefault();
            bool flag = false;
            log.Info(authHeader);
            if (authHeader != null)
            {
                  string secretKey =  System.Configuration.ConfigurationManager.AppSettings["seckey"].ToString();//加密秘钥
                string token = authHeader.FirstOrDefault();//获取token
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        byte[] key = Encoding.UTF8.GetBytes(secretKey);
                        IJsonSerializer serializer = new JsonNetSerializer();
                        IDateTimeProvider provider = new UtcDateTimeProvider();
                        IJwtValidator validator = new JwtValidator(serializer, provider);
                        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                        IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
                        //解密
                        var json = decoder.DecodeToObject<AuthInfo>(token, key, verify: true);


                        





                        //var jwtHelper = new JWTHelper();
                        //var json = jwtHelper.DecodeToObject<AuthInfo>(token, secretKey, out bool isValid, out string errMsg);
                        if (json != null)
                        {
                            //判断口令过期时间
                            if (json.ExpiryDateTime < DateTime.Now)
                            {
                                flag = false;
                            }
                            else
                            {
                                IPrincipal principal = actionContext.ControllerContext.RequestContext.Principal;

                                List<string> rolename = new List<string>();
                                foreach (var roleItem in json.Roles.ToString().Split(','))
                                {
                                    rolename.Add(roleItem);
                                }
                                flag = principal != null && principal.Identity != null
                                    && principal.Identity.IsAuthenticated &&
                                    (
                                        (((IEnumerable<string>)rolename).Any<string>(new Func<string, bool>(principal.IsInRole)))
                                     );
                                //log.Info(json.Roles + ":::"+principal.Identity.Name + ":::" + flag.ToString());
                            }
                           
                            actionContext.RequestContext.RouteData.Values.Add("token", json);
                            log.Info("flag;:::" + flag);
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Info(ex.ToString());
                        return false;
                    }
                }
            }
            return flag;
        }

        protected  bool IsAuthorized_bak(HttpActionContext actionContext)
        {
            //前端请求api时会将token存放在名为"auth"的请求头中
            var authHeader = from t in actionContext.Request.Headers where t.Key == "token" select t.Value.FirstOrDefault();
            log.Info(authHeader);
            if (authHeader != null)
            {
                string secretKey = System.Configuration.ConfigurationManager.AppSettings["seckey"].ToString();//加密秘钥
                string token = authHeader.FirstOrDefault();//获取token
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        byte[] key = Encoding.UTF8.GetBytes(secretKey);
                        IJsonSerializer serializer = new JsonNetSerializer();
                        IDateTimeProvider provider = new UtcDateTimeProvider();
                        IJwtValidator validator = new JwtValidator(serializer, provider);
                        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                        IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
                        //解密
                        var json = decoder.DecodeToObject<AuthInfo>(token, key, verify: true);
                        //var jwtHelper = new JWTHelper();
                        //var json = jwtHelper.DecodeToObject<AuthInfo>(token, secretKey, out bool isValid, out string errMsg);
                        if (json != null)
                        {
                            //判断口令过期时间
                            if (json.ExpiryDateTime < DateTime.Now)
                            {
                                return false;
                            }
                            actionContext.RequestContext.RouteData.Values.Add("token", json);
                            return true;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        log.Info(ex.ToString());
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 处理授权失败的请求
        /// </summary>
        /// <param name="actionContext"></param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var erModel = new
            {
                Success = "false",
                ErrorCode = "401"
            };
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, erModel, "application/json");
        }

        /// <summary>
        ///  为操作授权时调用
        /// </summary>
        /// <param name="actionContext"></param>
        //public override void OnAuthorization(HttpActionContext actionContext)
        //{

        //}
    }
}
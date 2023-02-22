using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApi_WMS.Models;

namespace WebApi_WMS.Filter
{
    public class ApiAuthorizeAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// 指示指定的控件是否已获得授权
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected bool IsAuthorized(HttpActionContext actionContext)
        {
            //前端请求api时会将token存放在名为"auth"的请求头中
            var authHeader = from t in actionContext.Request.Headers where t.Key == "Authorization" select t.Value.FirstOrDefault();
            if (authHeader != null)
            {
                string secretKey = ConfigurationManager.AppSettings["SecurityKey"].ToString();//加密秘钥
                string token = authHeader.FirstOrDefault().Split(' ')[1];//获取token
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        byte[] key = Encoding.UTF8.GetBytes(secretKey);
                        IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                        IJsonSerializer serializer = new JsonNetSerializer();
                        IDateTimeProvider provider = new UtcDateTimeProvider();
                        IJwtValidator validator = new JwtValidator(serializer, provider);
                        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                        IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                        //AuthInfo authInfo = JWT.JsonWebToken.DecodeToObject<AuthInfo>(token, key);
                        //解密
                        var json = decoder.DecodeToObject(token, key, verify: true);
                        if (json != null)
                        {
                            object timeStamp = null;
                            json.TryGetValue("exp", out timeStamp);

                            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                            long lTime = ((long)timeStamp * 10000000);
                            TimeSpan toNow = new TimeSpan(lTime);
                            DateTime targetDt = dtStart.Add(toNow);
                            //判断口令过期时间
                            if (targetDt < DateTime.Now)
                            {
                                return false;
                            }
                            actionContext.RequestContext.RouteData.Values.Add("auth", json);
                            return true;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
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
        protected void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var erModel = new
            {
                Description = "未经许可的访问(Unauthorized)"
            };
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, erModel, "application/json");
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var attr = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>();
            bool isAnonymous = attr.Any(a => a is AllowAnonymousAttribute);
            if (!isAnonymous)
            {
                var rq = actionContext.Request.Properties;

                var authorization = actionContext.Request.Headers.Authorization;
                if (authorization == null)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    string ResultMessage;//需要解析的消息
                    string Payload;//获取负载
                    var result = ValidateJWT(authorization.Parameter, out Payload, out ResultMessage); //TokenManager.ValidateToken(authorization.Scheme);
                    if (!result)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, ResultMessage);
                    }
                }

            }
        }

        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="token">token值</param>
        /// <param name="payload">token还原后的字典</param>
        /// <param name="message">验证结果</param>
        /// <returns></returns>
        public static bool ValidateJWT(string token, out string payload, out string message)
        {
            bool isValidted = false;
            payload = "";
            
            try
            {
                
                string secret = ConfigurationManager.AppSettings["SecurityKey"].ToString();//加密秘钥
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtValidator validator = new JwtValidator(serializer, provider);//用于验证JWT的类
                                                                                 //  IJwtAlgorithm algorithm= new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);//用于解析JWT的类
                payload = decoder.Decode(token, secret, verify: true);

                isValidted = true;

                message = "验证成功";
            }
            catch (TokenExpiredException)//当前时间大于负载过期时间（负荷中的exp），会引发Token过期异常
            {
                message = "过期了！";
            }
            catch (SignatureVerificationException)//如果签名不匹配，引发签名验证异常
            {
                message = "签名错误！";
            }
            return isValidted;
        }
    }
}
using JWTWebApiNet.AuthAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http; 

namespace JWTWebApiNet.Controllers
{
    [RoutePrefix("api/UserInfo")]
    [ApiAuthorize]
    public class UserInfoController : ApiController
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserInfo")]
        public string GetUserInfo()
        {
            var userInfo = new
            {
                UserName = "tom",
                Tel = "123456789",
                Address = "testddd"
            };
            return JsonConvert.SerializeObject(userInfo);
        }
        [HttpGet]
        [Route("GetInfo"), Authorize(Roles = "admin,client")]
        public string GetInfo()
        {
            var userInfo = new
            {
                UserName = "ccccccccccccc",
                Tel = "aaaaaaaaaaaaaaa",
                Address = "bbbbbbbbbbbbbbbb"
            };
            return JsonConvert.SerializeObject(userInfo);
        }
        [HttpGet]
        [Route("Info"), Authorize(Roles = "client")]
        public string Info()
        {
            var userInfo = new
            {
                UserName = "1111111111",
                Tel = "22222222222",
                Address = "33333333333333"
            };
            return JsonConvert.SerializeObject(userInfo);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("test"), Authorize(Roles = "test")]
        public string test()
        {
            var userInfo = new
            {
                UserName = "1111111111",
                Tel = "22222222222",
                Address = "33333333333333"
            };
            return JsonConvert.SerializeObject(userInfo);
        }
    }
}
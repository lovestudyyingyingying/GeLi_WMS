using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NX_WMS_TM_ApiNet.Models
{
    public class TokenInfo
    {
        /// <summary>
     /// 是否成功
     /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; } 
    }
}
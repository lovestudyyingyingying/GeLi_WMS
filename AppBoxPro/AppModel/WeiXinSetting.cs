using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeLiPage_WMS
{
    /// <summary>
    /// 微信设置
    /// </summary>
    public class WeiXinSetting
    {
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? expiraiton_time { get; set; }

    }
}
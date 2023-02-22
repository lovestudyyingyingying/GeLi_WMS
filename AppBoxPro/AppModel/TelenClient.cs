using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GeLiPage_WMS
{
    public class TelenClient : IKeyID
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [StringLength(20)]
        public string Number { get; set; }



        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(50)]
        public string Name { get; set; }

        //分级代码
        [StringLength(50)]
        public string levelCode { get; set; }

        //客户经理
        public string clientManager { get; set; }

        /// <summary>
        /// 实施人员
        /// </summary>
        public string implementer { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public string gendan { get; set; }

        /// <summary>
        /// 服务有效期
        /// </summary>
        public DateTime? serverTime { get; set; }

        /// <summary>
        /// 服务条件
        /// </summary>
        [StringLength(50)]
        public string condiction { get; set; }

        [StringLength(50)]
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        [StringLength(50)]
        /// <summary>
        /// 传真
        /// </summary>
        public string fax { get; set; }

        [StringLength(50)]
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }

        [StringLength(50)]
        /// <summary>
        /// 开户银行
        /// </summary>
        public string bank { get; set; }

        [StringLength(50)]
        public string Eamil { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// openid
        /// </summary>
        public string openid { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeLiPage_WMS
{
    public class ServiceOrder : IKeyID
    {
        [Key]
        public int ID { get; set; }

        public virtual User user { get; set; }

        //客户
        public virtual TelenClient telenClient { get; set; }

        //联系人
        public string contact { get; set; }

        //地址
        public string address { get; set; }

        /// <summary>
        /// 服务需求描述
        /// </summary>
        public string order_desc{ get; set; }

        /// <summary>
        /// 服务性质
        /// </summary>
        public string nature { get; set; }
        
        /// <summary>
        /// 培训描述
        /// </summary>
        public string train_desc { get; set; }

        /// <summary>
        /// 培训岗位对象
        /// </summary>
        public string train_instance { get; set; }

        /// <summary>
        /// 设备信息登记
        /// </summary>
        public string equip_info { get; set; }

        /// <summary>
        /// 解决问题内容
        /// </summary>
        public string soulv_content { get; set; }

        /// <summary>
        /// 服务评价
        /// </summary>
        public string evaluate { get; set; }

        /// <summary>
        /// 下达日期
        /// </summary>
        public DateTime? input_date { get; set; }

        /// <summary>
        /// 完成日期
        /// </summary>
        public DateTime? solve_date { get; set; }

        /// <summary>
        /// 状态  01-已下达，02-处理完成
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public int? grade { get; set; }

        /// <summary>
        /// 照片名称
        /// </summary>
        public string imagename { get; set; }
    }
}
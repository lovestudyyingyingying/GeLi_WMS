using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_WMS.Models
{
    public class HuoJiaState
    {
        /// <summary>
        /// 货架编号
        /// </summary>
        public string shelfNum { get; set; }
        /// <summary>
        /// 货架所在二维码位置，如果 货架在 RCS 系统无位置， 返回-1，同时下面字段没有 值。 
        /// </summary>
        public string shelfCurrStation  { get; set; }
        /// <summary>
        /// 货架 x 坐标 
        /// </summary>
        public int posX { get; set; }
        /// <summary>
        /// 货架 y 坐标 
        /// </summary>
        public int posY { get; set; }
        /// <summary>
        /// 货架类型 
        /// </summary>
        public string shelfType { get; set; }

        /// <summary>
        /// 货架角度 
        /// </summary>
        public string angle { get; set; }

    }
}
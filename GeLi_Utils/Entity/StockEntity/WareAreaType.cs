using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.StockEntity
{
    public class WareAreaType
    {
        /// <summary>
        /// 产线区
        /// </summary>
        public static string ProductionLine = "0";

        /// <summary>
        /// 缓存区(不是叉车拼音，是缓存英文)
        /// </summary>
        public static string CacheArea = "1";

        /// <summary>
        /// 空托区
        /// </summary>
        public static string EmptyArea = "2";

        /// <summary>
        /// 码盘区
        /// </summary>
        public static string MaPanArea = "3";
    }
}

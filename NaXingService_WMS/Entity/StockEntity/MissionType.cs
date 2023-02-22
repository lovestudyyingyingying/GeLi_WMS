using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.StockEntity
{
    public class MissionType
    {
        /// <summary>
        /// 02为终点位绑定货物
        /// </summary>
        public static string InstockType = "02";
        /// <summary>
        /// 03为起始位解绑货物
        /// </summary>
        public static string OutstockType = "03";
        /// <summary>
        /// 04为起始位解绑货物，为终点位绑定货物
        /// </summary>
        public static string MovestockType = "04";
        /// <summary>
        /// 05为跨楼层出库到缓存区的标识，StockPlan分解为出库指令时使用
        /// 为终点位绑定货物
        /// </summary>
        public static string MoveOut_TSJ = "05";
        /// <summary>
        /// 跨楼层入仓从入库起始位搬到提升机,入仓用
        /// 完成时没有其他操作，多用于入仓第一阶段使用
        /// </summary>
        public static string MoveIn_TSJ = "06";

    }
}

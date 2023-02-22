using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.StockEntity
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


        /// <summary>
        /// 下发从提升机运输空托到缓冲区的任务标识
        /// </summary>
        public static string MoveOutNull_TSJ = "07";

        /// <summary>
        /// 物料上线
        /// 完成任务时终点解绑（缓存区）
        /// </summary>
        public static string GoodOnline = "10";

        /// <summary>
        ///物料下线到产线
        ///下任务时起点绑定，完成任务时终点解绑
        /// </summary>
        public static string GoodOfflineInChanXian = "11";

        /// <summary>
        /// 物料下线到缓存
        /// 下任务时起点绑定，完成任务时终点绑定（相当于调拨）
        /// </summary>
        public static string GoodOfflineInHuanCun = "12";

        /// <summary>
        /// 送货去码盘机码盘
        /// </summary>
        public static string MoveToMaPanJi = "13";

        /// <summary>
        /// 将若干个码盘完毕的空托搬离码盘机
        /// </summary>
        public static string MoveOutMaPanJi = "14";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.MaPanJiStateEntity
{
    public class MaPanJiStateSummarize
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        public static string original = "初始状态";

        /// <summary>
        /// 允许叠板
        /// </summary>
        public static string Idle = "允许叠板";

        /// <summary>
        /// 满盘
        /// </summary>
        public static string FullEmptyTray = "满盘";

        /// <summary>
        /// 下发叠板任务
        /// </summary>
        public static string DeliverTask = "下发叠板任务";

        /// <summary>
        /// 任务中允许进入
        /// </summary>
        public static string TaskingCanIn = "任务中允许进入";

        /// <summary>
        /// 正在叠板
        /// </summary>
        public static string DieBaning = "正在叠板";
    }
}

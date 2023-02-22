using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.AGVApiEntity
{
    public class AgvManPanJiErrorState
    {
        public static string CanotGetSatate = "无法获取码盘机状态";
        public static string MaPanJiNoInDieBan = "码盘机未处于叠板任务";
        public static string MaPanJiReadying = "码盘机准备中请等待";
        public static string MaPanJiDieBanIng = "码盘机正在叠板";
    }
}

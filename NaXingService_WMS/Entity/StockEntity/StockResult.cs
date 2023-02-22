using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsSharp.Standard;

namespace NanXingService_WMS.Entity.StockEntity
{
    public class StockResult
    {

        #region 进仓失败错误信息

        public static string InstockError_StartWLError = "进仓失败:没有该起始仓位，请重新扫描起始仓位或选择起始仓位";

        public static string InstockError_TrayHasInError = "进仓失败:该产品条码已进仓，请更换产品重试";

        public static string InstockError_TrayInMissionError = "进仓失败:该产品条码已在任务中，请更换产品重试";

        public static string InstockError_FindEndWLAutoError = "进仓失败:没有合适的仓位存放该货物，请仓库管理员查询库存情况";

        public static string InstockError_FindEndWLSRError = "进仓失败:找不到用户输入的仓位，请重新输入或重新扫描产品获取最新可用仓位";

        public static string InstockError_EndWLIsUseError = "进仓失败:目标仓位正在出库或进仓，请重新输入仓位或重新扫描产品获取最新可用仓位";

        public static string InstockError_EndWLHasTrayError = "进仓失败:目标仓位已有产品，请重新输入仓位或重新扫描产品获取最新可用仓位";

        public static string InstockError_WriteMissionError = "进仓失败:进仓指令写入失败，请检查服务器是否正常连接";
        #endregion

        #region 迁移失败错误信息

        public static string MovestockError_TrayNoInstockError = "调拨失败:该产品条码还没进仓";

        public static string MovestockError_TrayInMissionError = "调拨失败:该产品条码或终点仓位已在任务中，请更换产品或终点仓位重试";

        public static string MovestockError_FindEndWLSRError = "调拨失败:找不到用户输入的仓位，请重新输入";

        public static string MovestockError_EndWLIsUseError = "调拨失败:目标仓位正在出库或进仓，请重新输入仓位";

        public static string MovestockError_EndWLHasTrayError = "调拨失败:目标仓位已有产品，请重新输入仓位";

        public static string MovestockError_WriteMissionError = "进仓失败:进仓指令写入失败，请检查服务器是否正常连接";

        #endregion

        public static int GetBaseStateCode(string errorMsg)
        {
            if (errorMsg == InstockError_WriteMissionError|| errorMsg == MovestockError_WriteMissionError)
                return BaseStateCode.网络异常;
            return BaseStateCode.数据验证不通过;
        }
    }
}

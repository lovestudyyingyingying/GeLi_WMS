using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.StockEntity
{
    public class StockState
    {
        //总任务 ： SendState空--》已分类--》步骤一--》步骤二--》成功(发送失败)
        //          RunState 空--》空    --》空    --》空    --》已完成(已取消、执行失败)

        //分任务 :  SendState空 --》成功（发送失败）
        //          RunState 空 --》已下发--》运行中--》等待确认--》已完成(已取消、执行失败、发送失败)

        //发送状态
        public static string SendState_Emtpy = string.Empty;
        public static string SendState_Group = "已分类";
        public static string SendState_BzOne = "步骤一";
        public static string SendState_BzTwo = "步骤二";
        public static string SendState_Success = "成功";
        public static string SendState_Fail = "失败";


        //执行状态
        public static string RunState_Emtpy = string.Empty;
        public static string RunState_HasSend = "已下发";
        public static string RunState_Running = "运行中";
        public static string RunState_WaitRun = "等待确认";

        public static string RunState_Success = "已完成";
        public static string RunState_Cancel = "已取消";
        public static string RunState_RunFail = "执行失败";
        public static string RunState_SendFail = "发送失败";
        
        public static bool IsFinish(string runState)
        {
            return (runState == RunState_Success || runState == RunState_Cancel
                || runState == RunState_RunFail || runState == RunState_SendFail);
        }
    }
}

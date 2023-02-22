using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.MaPanJiStateEntity
{
    public class MaPanJiErrorState
    {
        public static string MaxTray = "栈板已达上限，无法执行叠板操作";
        public static string TrayOver = "栈板校正后超出";
        public static string MinTray = "栈板已达下限，无法执行拆板操作";
        public static string TrayNoOut = "栈板未取出，无法继续拆板";
        public static string TrayAtBottom = "底部有栈板，请取出栈板后完成复位";
        public static string CylinderOriginWarn = "校正气缸原点报警";
        public static string CylinderMoveWarn = "校正气缸动点报警";
        public static string GearCylinderOriginWarn = "插齿气缸原点报警";
        public static string GearCylinderMoveWarn = "插齿气缸动点报警";
        public static string SuddenStopWarn = "急停报警";
        public static string CanInitialization = "设备有报警，不能进行初始化";
        public static string TransducerWarn = "变频器报警";
        public static string LeftAndRightTooFast = "左右运行速度差异过大";
        public static string NoDeviceRunning = "设备没有运行，请先启动设备";
    }
}

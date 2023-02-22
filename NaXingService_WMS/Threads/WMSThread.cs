using NanXingService_WMS.Threads.DiffFloorThreads;
using NanXingService_WMS.Threads.SameFloorThreads;
using NanXingService_WMS.Utils.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Threads
{
    /// <summary>
    /// WMSThread管理中心，只有两个功能，开始，停止
    /// </summary>
    public class WMSThread
    {
        #region 变量

        GroupMissionThread groupMissionThread = new GroupMissionThread();

        SameFloorFactory sameFloorThread = new SameFloorFactory();
        DiffFloorFactory diffFloorThread = new DiffFloorFactory();

        WareLieStateThread wareLieStateThread1;
        WareLieStateThread wareLieStateThread2;

        RabbitMQUtils rabbitMQUtils = new RabbitMQUtils();

        #endregion

        #region 启动

        public void StartTask()
        {

        }


        #endregion 启动

        #region 关闭

        public void CloseTask()
        {

        }

        #endregion 关闭

    }
}

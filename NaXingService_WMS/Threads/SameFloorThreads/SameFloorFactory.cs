using NanXingData_WMS.Dao;
using NanXingService_WMS.Services;
using NanXingService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NanXingService_WMS.Threads.SameFloorThreads
{
    /// <summary>
    /// 同楼层执行线程工厂
    /// </summary>
    public class SameFloorFactory
    {
        ConcurrentDictionary<string, SameFloorRunThread> taskDic = 
            new ConcurrentDictionary<string, SameFloorRunThread>();
        WareHouseService wareHouseService = new WareHouseService();
        //定时执行
        Timer timer;
        public void Start(double RepeatTime=600000)
        {
            Control();
            //定时执行
            timer = new Timer();
            timer.Enabled = true;
            timer.Interval = RepeatTime;//执行间隔时间,单位为毫秒
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);
        }
        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Default.Process(new Log(LevelType.Info,
                  $"同楼层线程池判断。。。"));
            try
            {
                Control();
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error, "SameFloorThread:\r\n"+ex.ToString()));
                //Close();
                //Start(1000);
            }

        }
        /// <summary>
        /// 本工厂主要工作：
        /// 生产同楼层执行线程，并定时检测是否正常运行
        /// </summary>
        public void Control()
        {
            List<WareHouse> list = wareHouseService.GetAll(true);
            foreach (WareHouse item in list)
            {
                if (!taskDic.Keys.Contains(item.WHName))
                {
                    SameFloorRunThread sameFloorRunThread = new SameFloorRunThread(item);
                    taskDic.TryAdd(item.WHName, sameFloorRunThread);
                    Logger.Default.Process(new Log(LevelType.Info,
                    $"SameFloorRunThread:{item.WHName}开启同楼层执行线程。。。"));
                }
            }
            foreach (string temp in taskDic.Keys)
            {
                if (!list.Any(u=>u.WHName==temp))
                {
                    SameFloorRunThread sameFloorRunThread = null;
                    taskDic.TryGetValue(temp, out sameFloorRunThread);
                    if(sameFloorRunThread.myTask!=null)
                        sameFloorRunThread.myTask.CloseTask();
                    taskDic.TryRemove(temp,out sameFloorRunThread);
                }
            }
        }

        public void Close()
        {
            if (timer != null)
                timer.Stop();
            timer.Dispose();
            foreach (var temp in taskDic.Values)
            {
                temp.myTask.CloseTask();
            }
            taskDic.Clear();
        }
    }
}

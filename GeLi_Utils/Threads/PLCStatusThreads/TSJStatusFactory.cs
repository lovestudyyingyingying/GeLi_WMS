using GeLiData_WMS;
using GeLiData_WMSUtils;
using GeLiService_WMS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GeLi_Utils.Threads.PLCStatusThreads
{
    public class TSJStatusFactory
    {
        ConcurrentDictionary<string, TSJStatusThreads> taskDic =
            new ConcurrentDictionary<string, TSJStatusThreads>();
        DbBase<TiShengJiInfo> tiShengJiInfoDbBase = new DbBase<TiShengJiInfo>();
        //对仓库数据表进行增删改查的工具
        // WareHouseService wareHouseService = new WareHouseService();
        //定时执行
        Timer timer;


        public void Start(double RepeatTime = 30000) //30秒执行一次
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
                  $"提升机状态及其故障状态获取程池判断。。。"));
            try
            {
                Control();//尝试开启所有线程
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error, "TSJStatusFactory:\r\n" + ex.ToString()));
                //Close();
                //Start(1000);
            }

        }

        /// <summary>
        /// 本工厂主要工作：
        /// 生成码AGV和码盘机故障信息获取执行线程，并定时检测是否正常运行
        /// </summary>
        public void Control()
        {
            //拿了所有仓库表
            List<TiShengJiInfo> list = tiShengJiInfoDbBase.GetAll(true);


            foreach (TiShengJiInfo item in list)
            {
                //判断线程字典里是否包含，不包含则
                if (!taskDic.Keys.Contains(item.TsjName))
                {
                    TSJStatusThreads tSJStatusThreads = new TSJStatusThreads(item);
                    // SameFloorRunThread sameFloorRunThread = new SameFloorRunThread(item);//当线程被实例化后已经开始循环了，此处传入仓库名
                    taskDic.TryAdd(item.TsjName, tSJStatusThreads);//加到线程字典里
                    Logger.Default.Process(new Log(LevelType.Info,
                    $"MPJStatusThreads:{item.TsjName}开启提升机状态及其故障状态获取执行线程。。。"));
                }

            }

            foreach (string temp in taskDic.Keys)//对字段的名字循环   （保险）
            {
                if (!list.Any(u => u.TsjName == temp))
                {
                    TSJStatusThreads tSJStatusThreads = null;
                    taskDic.TryGetValue(temp, out tSJStatusThreads);
                    if (tSJStatusThreads.myTask != null)
                        tSJStatusThreads.myTask.CloseTask();
                    taskDic.TryRemove(temp, out tSJStatusThreads);
                }
            }
        }

        //关闭线程工厂的方法
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

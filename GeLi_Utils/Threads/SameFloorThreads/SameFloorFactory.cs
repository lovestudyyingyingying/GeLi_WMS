using GeLi_Utils.Threads.SameFloorThreads;
using GeLiData_WMS;
using GeLiService_WMS.Services;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using System.Timers;

namespace GeLiService_WMS.Threads.SameFloorThreads
{
    /// <summary>
    /// 同楼层执行线程工厂
    /// </summary>
    public class SameFloorFactory
    {
        //定义一个线程字典，一开始为空
        ConcurrentDictionary<string, SameFloorRunThread> taskDic = 
            new ConcurrentDictionary<string, SameFloorRunThread>();

        //对仓库数据表进行增删改查的工具
        WareHouseService wareHouseService = new WareHouseService();
        //定时执行
        Timer timer;

        
        public void Start(double RepeatTime=600000) //600秒执行一次
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
                Control();//尝试开启所有线程
                DryProductionLineThread dryProductionLineThread = new DryProductionLineThread();


                
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
            //拿了所有仓库表
            List<WareHouse> list = wareHouseService.GetList(null,true,GeLiData_WMSUtils.DbMainSlave.Master);
           
         
            foreach (WareHouse item in list)
            {
                //判断线程字典里是否包含，不包含则
                if (!taskDic.Keys.Contains(item.WHName))
                {
                    SameFloorRunThread sameFloorRunThread = new SameFloorRunThread(item);//当线程被实例化后已经开始循环了，此处传入仓库名
                    taskDic.TryAdd(item.WHName, sameFloorRunThread);//加到线程字典里
                    Logger.Default.Process(new Log(LevelType.Info,
                    $"SameFloorRunThread:{item.WHName}开启同楼层执行线程。。。"));
                }
            }

            foreach (string temp in taskDic.Keys)//对字段的名字循环   （保险）
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

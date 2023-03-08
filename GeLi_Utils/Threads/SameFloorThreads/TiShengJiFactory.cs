using GeLiData_WMS;
using GeLiService_WMS;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeLi_Utils.Threads.SameFloorThreads
{
     public class TiShengJiFactory
    {
        TiShengJiInfoService tiShengJiInfoService = new TiShengJiInfoService();
        ConcurrentDictionary<string, ColdHotNitrogenOnLineThread> taskDic_OnLine =
            new ConcurrentDictionary<string, ColdHotNitrogenOnLineThread>();
        ConcurrentDictionary<string, ColdHotCacheThread> taskDic_Cache =
            new ConcurrentDictionary<string, ColdHotCacheThread>();
        ConcurrentDictionary<string, Socket> socketDic =
            new ConcurrentDictionary<string, Socket>();
        public MyTask myTask;


        public TiShengJiFactory()
        {

        }

        public void StartNew()
        {
            if (myTask != null)
                Close();
            Thread.Sleep(2000);
            myTask = new MyTask(new Action(Run), 600, true)
                .StartTask();
        }

        public void Run()
        {
            // try
            // {
            //定时读取提升机表
            List<TiShengJiInfo> list = tiShengJiInfoService.GetAll();
            list.ForEach(temp => {

                if (!taskDic_OnLine.Keys.Contains(temp.TsjName))
                {

                    //新建提升机任务
                    ColdHotNitrogenOnLineThread coldHotNitrogenOnLineThread = new ColdHotNitrogenOnLineThread(temp);
                    //tiShengJiThread.RunControl();
                    taskDic_OnLine.TryAdd(temp.TsjName, coldHotNitrogenOnLineThread);

                    Logger.Default.Process(new Log(LevelType.Info,
                    $"ColdHotNitrogenOnLineThread:{temp.TsjName}开启处理提升机缓存执行线程。。。"));
                }
                if (!taskDic_Cache.Keys.Contains(temp.TsjName))
                {

                    //新建提升机任务
                    ColdHotCacheThread coldHotCacheThread = new ColdHotCacheThread(temp);
                    //tiShengJiThread.RunControl();
                    taskDic_Cache.TryAdd(temp.TsjName, coldHotCacheThread);

                    Logger.Default.Process(new Log(LevelType.Info,
                    $"ColdHotCacheThread:{temp.TsjName}开启处理冷热缓存区执行线程。。。"));
                }

            });

            // }
            //catch (Exception ex)
            //{
            //    Logger.Default.Process(new Log(LevelType.Error, "DiffFloorThread:\r\n"+ ex.ToString()));
            //    //Close();
            //    //Start(1000);
            //}

        }
        public void Close()
        {
            if (myTask != null)
                myTask.CloseTask();
            foreach (var temp in taskDic_OnLine.Values)
            {
                //temp.tiShengJiHelper.CloseTcp();
                temp.myTask.CloseTask();
            }
            foreach (var temp in taskDic_Cache.Values)
            {
                //temp.tiShengJiHelper.CloseTcp();
                temp.myTask.CloseTask();
            }
            taskDic_OnLine.Clear();
            taskDic_Cache.Clear();
        }
    }
}

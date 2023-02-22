using GeLiData_WMS;
using GeLiService_WMS;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Threads.DiffFloorThreads;
using GeLiService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeLi_Utils.Threads.DiffFloorThreads
{
    /// <summary>
    /// 空托盘运输到托盘缓冲区工厂
    /// </summary>
    public class EmptyTrayToBufferFactory
    {
        TiShengJiInfoService tiShengJiInfoService = new TiShengJiInfoService();
        ConcurrentDictionary<string, EmptyTrayToBufferThreads> taskDic =
            new ConcurrentDictionary<string, EmptyTrayToBufferThreads>();
        ConcurrentDictionary<string, Socket> socketDic =
            new ConcurrentDictionary<string, Socket>();
        public MyTask myTask;

        public static readonly string oneStr = "-1";
        public static readonly string twoStr = "-2";

        public EmptyTrayToBufferFactory()
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
            try
            {
                //定时读取提升机表
                List<TiShengJiInfo> list = tiShengJiInfoService.GetAll();
                list.ForEach(temp => {

                    if (!taskDic.Keys.Contains(temp.TsjName))
                    {

                        //新建提升机任务
                        EmptyTrayToBufferThreads emptyTrayToBufferThreads = new EmptyTrayToBufferThreads(temp);
                        //tiShengJiThread.RunControl();
                        taskDic.TryAdd(temp.TsjName, emptyTrayToBufferThreads);

                        Logger.Default.Process(new Log(LevelType.Info,
                        $"EmptyTrayToBufferRunThreads:{temp.TsjName}开启搬运空托执行线程。。。"));
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error, "EmptyTrayToBufferThreads:\r\n" + ex.ToString()));
                //Close();
                //Start(1000);
            }

        }
        public void Close()
        {
            if (myTask != null)
                myTask.CloseTask();
            foreach (var temp in taskDic.Values)
            {
               // temp.tiShengJiHelper.CloseTcp();
                temp.runTask.CloseTask();
            }
            taskDic.Clear();
        }
    }
}


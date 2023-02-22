using NanXingData_WMS.Dao;
using NanXingService_WMS.Services.WMS.AGV;
using NanXingService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanXingService_WMS.Threads.DiffFloorThreads
{
    /// <summary>
    /// 跨楼层线程工厂类
    /// </summary>
    public class DiffFloorFactory
    {
        TiShengJiInfoService tiShengJiInfoService = new TiShengJiInfoService();
        ConcurrentDictionary<string, TiShengJiThread> taskDic =
            new ConcurrentDictionary<string, TiShengJiThread>();
        ConcurrentDictionary<string, Socket> socketDic =
            new ConcurrentDictionary<string, Socket>();
        public MyTask myTask;

        public static readonly string oneStr = "-1";
        public static readonly string twoStr = "-2";

        public DiffFloorFactory()
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
        DataTable dataTable = null;
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
                        TiShengJiThread tiShengJiThread = new TiShengJiThread(temp);
                        //tiShengJiThread.RunControl();
                        taskDic.TryAdd(temp.TsjName, tiShengJiThread);
                        
                        Logger.Default.Process(new Log(LevelType.Info,
                        $"DiffFloorRunThread:{temp.TsjName}开启跨楼层执行线程。。。"));
                    }
                });
                
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error, "DiffFloorThread:\r\n"+ ex.ToString()));
                //Close();
                //Start(1000);
            }
            
        }
        public void Close() 
        {
            if (myTask!=null)
                myTask.CloseTask();
            foreach (var temp in taskDic.Values)
            {
                temp.tiShengJiHelper.CloseTcp();
                temp.runTask.CloseTask();
            }
            taskDic.Clear();
        }
    }
}

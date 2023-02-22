using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.ThreadUtils
{
    /// <summary>
    /// 线程池帮助类
    /// ————————————————
    /// 版权声明：本文为CSDN博主「我可是森森呢」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
    /// 原文链接：https://blog.csdn.net/qq_42690327/article/details/122607912
    /// </summary>
    // 线程池优点：
    // 1、提高资源利用率
    // 线程池可以重复利用已经创建了的线程
    // 2、提高响应速度
    // 因为当线程池中的线程没有超过线程池的最大上限时，有的线程处于等待分配任务状态，当任务到来时，无需创建线程就能被执行。
    // 3、具有可管理性
    // 线程池会根据当前系统特点对池内的线程进行优化处理，减少创建和销毁线程带来的系统开销。
    // ————————————————
    /// 版权声明：本文为CSDN博主「psply」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
    /// 原文链接：https://blog.csdn.net/azahoopxkuzb78795/article/details/68946290
    /// 

    public class ThreadPoolHelper
    {
        private List<Task> ThreadList = new List<Task>();//线程队列
        private int RunningThreadNum;//正在执行的任务数
        private int MaxThreadNum;//最大线程数
        private int index;//取值下标
        /// <summary>
        /// 
        /// for(int i = 0; i < 10; i++) {
        ///        MyClass cl = new MyClass(i);//自己定义的类
        ///        Task t = new Task(() => {
        ///           cl.Strat();//创建线程执行类中的方法
        ///       });
        ///       pool.AddTask(t);//将线程放入线程池
        ///    }
        ///    pool.RunAndWait();//开始运行子线程并等待线程运行结束
        /// </summary>
        /// <param name="MaxThreadNum"></param>
        public ThreadPoolHelper(int MaxThreadNum)
        {   
            //创建固定数目线程池
            this.MaxThreadNum = MaxThreadNum;
            RunningThreadNum = 1;
            index = 0;
        }
        //添加线程
        public void AddTask(Task newTask)
        {
            ThreadList.Add(newTask);
        }
        //执行线程并等待完成
        public void RunAndWait()
        {
            while (index < ThreadList.Count)
            {
                if (RunningThreadNum <= MaxThreadNum)
                {
                    Task t = ThreadList[index];//取出任务
                    Task.WhenAll(t).ContinueWith((s) => {
                        RunningThreadNum--;//执行完就--
                    });
                    t.Start();//执行
                    RunningThreadNum++;//计数+1
                    index++;
                }
                else
                {
                    //任务队列塞满了
                    continue;
                }
            }
            //等待全部执行完成
            foreach (Task t in ThreadList)
            {
                t.Wait();
            }
        }
    }
}

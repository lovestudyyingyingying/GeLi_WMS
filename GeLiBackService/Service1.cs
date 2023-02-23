using GeLi_Utils.Threads.DiffFloorThreads;
using GeLi_Utils.Threads.FaultCollection;
using GeLi_Utils.Threads.PLCStatusThreads;
using GeLiService_WMS;
using GeLiService_WMS.Threads;
using GeLiService_WMS.Threads.DiffFloorThreads;
using GeLiService_WMS.Threads.SameFloorThreads;
using GeLiService_WMS.Utils.ThreadUtils;
using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace GeLiBackService
{
    public partial class Service1 : ServiceBase
    {

        GroupMissionThread groupMissionThread = new GroupMissionThread();
        SameFloorFactory sameFloorThread = new SameFloorFactory();
        DiffFloorFactory diffFloorThread = new DiffFloorFactory();
        EmptyTrayToBufferFactory emptyTrayToBufferFactory = new EmptyTrayToBufferFactory();
        AGVAndMPJFaulysFactory aGVAndMPJFaulysFactory = new AGVAndMPJFaulysFactory();
        MPJStatusFactory mPJStatusFactory = new MPJStatusFactory();
        TSJStatusFactory tSJStatusFactory = new TSJStatusFactory();


       // RabbitMQUtils rabbitMQUtils = new RabbitMQUtils();


        MyTask groupTask;
        public Service1()
        {
            InitializeComponent();
        }
  
        protected override void OnStart(string[] args)
        {
            try
            {

                Logger.Default.Process(new Log(LevelType.Info,
                   $"服务开启。。。"));


              
                //任务分类线程
                groupTask = new MyTask(new Action(groupMissionThread.Control), 1, true)
                    .StartTask();
                //同楼层执行线程
                sameFloorThread.Start();
                //跨楼层执行线程
                diffFloorThread.StartNew();
                //码盘机状态获取执行线程
                mPJStatusFactory.Start();
                //AGV和码盘机故障检测执行线程
                aGVAndMPJFaulysFactory.Start();
                //空托搬运到空托缓存区执行线程
                emptyTrayToBufferFactory.StartNew();
                //提升机状态获取执行线程
                tSJStatusFactory.Start();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Logger.Default.Process(new Log(LevelType.Error,
                   ex.ToString()));
            }
        }

        protected override void OnStop()
        {
            Logger.Default.Process(new Log(LevelType.Error,
                    "关闭服务"));
            try
            {
                groupTask.CloseTask();

            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    "关闭分类线程出现错误\r\n" + ex.ToString()));
            }


            try
            {
                sameFloorThread.Close();
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    "关闭同楼层执行线程出现错误\r\n" + ex.ToString()));
            }
            try
            {
                diffFloorThread.Close();
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    "关闭跨楼层执行线程出现错误\r\n" + ex.ToString()));
            }


            try
            {
                mPJStatusFactory.Close();
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    "关闭码盘机状态获取执行线程出现错误\r\n" + ex.ToString()));
            }
            try
            {
                aGVAndMPJFaulysFactory.Close();
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    "关闭AGV和码盘机故障检测执行线程出现错误\r\n" + ex.ToString()));
            }

            try
            {
                emptyTrayToBufferFactory.Close();
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    "关闭空托搬运到空托缓存区执行线程出现错误\r\n" + ex.ToString()));
            }
            try
            {
                tSJStatusFactory.Close();
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    "关闭提升机状态获取执行线程出现错误\r\n" + ex.ToString()));
            }
        }

    }
}

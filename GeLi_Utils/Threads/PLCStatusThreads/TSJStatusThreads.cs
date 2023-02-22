using GeLi_Utils.Helpers;
using GeLiData_WMS;
using GeLiData_WMSUtils;
using GeLiService_WMS;
using GeLiService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Threads.PLCStatusThreads
{
    public class TSJStatusThreads
    {
        TiShengJiInfo _tiShengJiInfo;
        //ConcurrentQueue<AGVMissionInfo> concurrentQueue = new ConcurrentQueue<AGVMissionInfo>();
        // AGVMissionService _agvMissionService = new AGVMissionService();
        // AGVOrderHelper aGVOrderHelper;
        //string waitRun = "等待执行";
        public MyTask myTask;


        //启动线程前要传入仓库名
        public TSJStatusThreads(TiShengJiInfo tiShengJiInfo)
        {
            _tiShengJiInfo = tiShengJiInfo;
            // aGVOrderHelper = new AGVOrderHelper("http://" + _maPanJiInfo.AGVServerIP);
            //_agvMissionService = agvMissionService;
            myTask = new MyTask(new Action(Run),
                        3, true).StartTask();
        }

        public void Run()
        {
            try
            {
                DateTime dtime = DateTime.Now.AddDays(-110);
                //查找当天前24小时同层的任务，提升机当作仓库库位
                List<TiShengJiState> tiShengJiStateList = new List<TiShengJiState>();
                DbBase<TiShengJiState> aGVAlarmLogdbBase = new DbBase<TiShengJiState>();


                TiShengJiHelper tiShengJiHelper = new TiShengJiHelper(_tiShengJiInfo.TsjIp, _tiShengJiInfo.TsjPort);
                tiShengJiHelper.ReadTiShengJiState();
            }
            catch (Exception ex)
            {

                Logger.Default.Process(new Log(LevelType.Error,
                             ex.ToString()));
            }

            // MaPanJiHelper
        }
    }
}

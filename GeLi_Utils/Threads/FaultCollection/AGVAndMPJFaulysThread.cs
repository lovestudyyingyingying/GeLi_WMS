using GeLi_Utils.Entity.AGVApiEntity;
using GeLi_Utils.Helpers;
using GeLi_Utils.Utils.AGVUtils;
using GeLiData_WMS;
using GeLiData_WMS.Dao;
using GeLiData_WMSUtils;
using GeLiService_WMS;
using GeLiService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Threads.FaultCollection
{
    public class AGVAndMPJFaulysThread
    {
        MaPanJiInfo _maPanJiInfo;
        //ConcurrentQueue<AGVMissionInfo> concurrentQueue = new ConcurrentQueue<AGVMissionInfo>();
       // AGVMissionService _agvMissionService = new AGVMissionService();
        AGVOrderHelper aGVOrderHelper;
        //string waitRun = "等待执行";
        public MyTask myTask;
        

        //启动线程前要传入仓库名
        public AGVAndMPJFaulysThread(MaPanJiInfo maPanJiInfo)
        {
            var AGVServerIP = ConfigurationManager.AppSettings["AGVIPAndPort"].ToString();
            _maPanJiInfo = maPanJiInfo;
            aGVOrderHelper = new AGVOrderHelper(AGVServerIP);
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
                List<AGVAlarmLog> aGVAlarmLogList = new List<AGVAlarmLog>();
                DbBase<AGVAlarmLog> aGVAlarmLogdbBase = new DbBase<AGVAlarmLog>();

                AlarmOrderResult alarmOrderResult = aGVOrderHelper.GetAgvAlarms();
                if (alarmOrderResult!=null&&alarmOrderResult.data != null && alarmOrderResult.data.Count() != 0)
                {

                    if (alarmOrderResult.code == 200)
                    {
                        for (int i = 0; i < alarmOrderResult.data.Count(); i++)
                        {
                            aGVAlarmLogList[i].deviceNum = alarmOrderResult.data[i].id.ToString();
                            aGVAlarmLogList[i].alarmGrade = alarmOrderResult.data[i].grade;
                            aGVAlarmLogList[i].alarmDesc = alarmOrderResult.data[i].detail;
                            aGVAlarmLogList[i].alarmDate = Convert.ToDateTime(alarmOrderResult.data[i].time);
                            aGVAlarmLogList[i].Reserve1 = alarmOrderResult.data[i].alarm_code;
                            aGVAlarmLogList[i].deviceName = "AGV" + aGVAlarmLogList[i].deviceNum;
                            aGVAlarmLogList[i].recTime = DateTime.Now;

                            Logger.Default.Process(new Log(LevelType.Info,
                            "AGV" + aGVAlarmLogList[i].deviceNum + aGVAlarmLogList[i].alarmGrade + aGVAlarmLogList[i].alarmDesc
                            + aGVAlarmLogList[i].Reserve1 + "AGV故障记录获取成功"));
                        }
                        aGVAlarmLogdbBase.InsertRange(aGVAlarmLogList);
                        aGVAlarmLogdbBase.SaveChanges();

                    }
                    else
                    {
                        Logger.Default.Process(new Log(LevelType.Error,
                            $"SameFloorRunThread:{_maPanJiInfo.MpjName}码盘机故障记录获取失败"));
                    }

                }
                MaPanJiHelper maPanJiHelper = new MaPanJiHelper(_maPanJiInfo.MpjIp, _maPanJiInfo.MpjPort);
                maPanJiHelper.CheckAndSaveError();
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

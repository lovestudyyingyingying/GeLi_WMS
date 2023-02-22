using NanXingData_WMS.Dao;
using NanXingService_WMS.Entity.AGVOrderEntity;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Helper.WMS;
using NanXingService_WMS.Services;
using NanXingService_WMS.Services.WMS;
using NanXingService_WMS.Utils.AGVUtils;
using NanXingService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Threads.SameFloorThreads
{
    public class SameFloorRunThread
    {
        WareHouse _wareHouse;
        //ConcurrentQueue<AGVMissionInfo> concurrentQueue = new ConcurrentQueue<AGVMissionInfo>();
        AGVMissionService _agvMissionService=new AGVMissionService();
        AGVOrderUtils agvOrderUtils;
        //string waitRun = "等待执行";
        public MyTask myTask;
        public SameFloorRunThread(WareHouse wareHouse)
        {
            _wareHouse = wareHouse;
            agvOrderUtils = new AGVOrderUtils(_wareHouse.AGVServerIP);
            //_agvMissionService = agvMissionService;
            myTask = new MyTask(new Action(Run),
                        3, true).StartTask();
        }

        public void Run()
        {
            DateTime dtime = DateTime.Now.AddDays(-1);
            List<AGVMissionInfo> list = _agvMissionService.GetIQueryable(u =>
                     u.OrderTime >= dtime && u.IsFloor == 0 
                     && u.WHName == _wareHouse.WHName && u.SendState== StockState.SendState_Group,
                    true, NanXingData_WMS.DaoUtils.DbMainSlave.Master)
                .OrderBy(u=>u.ID).ToList();
            if (list.Count > 0)
            {
                foreach (AGVMissionInfo mission in list)
                {

                    if (mission.WHName == "07一楼" && mission.Mark == MissionType.MovestockType)
                        mission.OrderGroupId = mission.EndMiddlePosition;
                    OrderResult result = agvOrderUtils.SendOrder(mission);
                    //Logger.Default.Process(new Log(LevelType.Info,"同楼层任务执行："+ result.ToString()));

                    if (result.code == 1000)
                        mission.SendState = ResultStr.success;
                    else
                        mission.SendState = ResultStr.fail;
                }

                DataTable dataTable = _agvMissionService.ConvertToDataTable(list);
                _agvMissionService.UpdateMany(dataTable);
            }
        }
       
    }
}

using NanXingData_WMS.Dao;
using NanXingService_WMS.Entity.SensorEntity;
using NanXingService_WMS.Services;
using NanXingService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Threads.DAQ
{
    public class DaqThread
    {
        SensorDataService sensorDataService2 = new SensorDataService();
        SensorDataService sensorDataService3 = new SensorDataService();
        FloorBase floor2 = new FloorBase("192.168.10.210", 8001, "2");
        //FloorBase floor2 = new FloorBase("127.0.0.1", 60000, "2");
        FloorBase floor3 = new FloorBase("192.168.10.215", 8001, "3");

        MyTask floor2Task = null;
        MyTask floor3Task = null;


        public DaqThread()
        {
            floor2.dataTable = sensorDataService2.sensorData_dt.Clone();
            floor3.dataTable = sensorDataService3.sensorData_dt.Clone();
        }

        public void Start(int waitSecond)
        {
            floor2Task=new MyTask(new Action(() => sensorDataService2.MainControl(floor2)),
                waitSecond, true,null, new Action(() => sensorDataService2.Close())).StartTask();
            floor3Task=new MyTask(new Action(() => sensorDataService3.MainControl(floor3)),
                waitSecond, true, null, new Action(() => sensorDataService3.Close())).StartTask();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.AGVOrderEntity
{
    public class TaskOrderDetails
    {
        public string TaskPath { get; set; }
        public string ShelfNumber { get; set; }
        public string StackerStorageNum { get; set; }
        public string AssignRobotIds { get; set; }

        public TaskOrderDetails()
        {
        }

        public TaskOrderDetails(string taskPath, string shelfNumber, string stackerStorageNum, string assignRobotIds)
        {
            TaskPath = taskPath;
            ShelfNumber = shelfNumber;
            StackerStorageNum = stackerStorageNum;
            AssignRobotIds = assignRobotIds;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.AGVApiEntity
{
    public class MissionState
    {
        public string taskId { get; set; }

        /// <summary>
        /// 目标点
        /// </summary>
        public string targetPoint { get; set; }

        public int agvNo { get; set; }

        public int status { get; set; }

        //public string processRate { get; set; }

        //public string shelfNumber { get; set; }
    }
}

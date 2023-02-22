using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeLiPage_WMS.Entity
{
    public class AGVMissionInfoDto
    {
        public  int ID { get; set; }
        
        public string MissionNo { get; set; }

        public DateTime OrderTime { get; set; }

        
        public string TrayNo { get; set; }

        
        public string Mark { get; set; }

        
        public string StartLocation { get; set; }

        
        public string StartPosition { get; set; }



        
        public string EndLocation { get; set; }

        
        public string EndPosition { get; set; }

        
        public string SendState { get; set; }

        
        public string SendMsg { get; set; }

        
        public string StateMsg { get; set; }

        
        public string RunState { get; set; }

        public DateTime StateTime { get; set; }

        public DateTime NodeTime { get; set; }



        public string OrderGroupId { get; set; }

        
        public string ModelProcessCode { get; set; }

        
        public string AGVCarId { get; set; }


        public string userId { get; set; }

        
        public string Remark { get; set; }


     
        /// <summary>
        /// 保留字段1(当前工序名)
        /// </summary>
        
        public string Reserve1 { get; set; }
        /// <summary>
        /// 保留字段2(暂存下任务时的起始仓库)
        /// </summary>
        
        public string Reserve2 { get; set; }
    }
}
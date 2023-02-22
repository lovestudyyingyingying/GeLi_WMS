using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity
{
    /// <summary>
    /// 任务x： 名称、开始时间、完成单位、完成数量、更新人、更新人ID、结束时间
    /// </summary>
    public class OrderTask
    {
        /// <summary>
        /// 任务序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>    
        public long StartTime { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public int FinishCount { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        public int FinishUnit { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>    
        public long EndTime { get; set; }
        /// <summary>
        /// 更新人ID
        /// </summary>    
        public string ModUserID { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>    
        public string ModUserName { get; set; }

    }
}

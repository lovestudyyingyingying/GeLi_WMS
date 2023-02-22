using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.AGVOrderEntity
{
    public class MissionOrder
    {
        /// <summary>
        /// 任务流程模板编号
        /// </summary>
        public string modelProcessCode { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public string priority { get; set; }
        /// <summary>
        /// 来源系统
        /// </summary>
        public string fromSystem { get; set; }
        /// <summary>
        /// 任务编号，不能重复
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        ///任务组 ID，如果同一组的任务， 则下发后将按照下发顺序执 行，直至该组任务完成。 
        /// </summary>
        public string orderGroupId { get; set; }
        /// <summary>
        /// 任务详情
        /// </summary>
        //public TaskOrderDetails TaskOrderDetail { get; set; }


        public List<TaskOrderDetails> taskOrderDetail { get; set; }

        public MissionOrder()
        {
        }

        public MissionOrder(string modelProcessCode, string priority, string fromSystem, string orderId, List<TaskOrderDetails> taskOrderDetail)
        {
            this.modelProcessCode = modelProcessCode;
            this.priority = priority;
            this.fromSystem = fromSystem;
            this.orderId = orderId;
            this.taskOrderDetail = taskOrderDetail;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.AGVOrderEntity
{
    public class OrderID
    {
        public OrderID()
        {
        }
        public OrderID(string orderId)
        {
            this.orderId = orderId;
        }

        public string orderId { get; set; }
    }
}

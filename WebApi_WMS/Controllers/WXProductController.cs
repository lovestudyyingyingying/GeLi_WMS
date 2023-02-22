using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using WebApi_WMS.Filter;
using System.Diagnostics;
using NanXingService_WMS.Entity;

namespace WebApi_WMS.Controllers
{
    [RoutePrefix("api/WX_Product")]
    public class WXProductController : BaseController
    {
        //获取排产单号
        [ApiAuthorize]
        [HttpPost]
        [WebMethod]
        [Route("GetProductOrder")]
        public object GetProductOrder([FromBody] ProOrderNo request)
        {
            RunResult<object> runResult = ProductOrderManager.GetOrderProCount(request.OrderNo);

            //从数据库获取订单
            Debug.WriteLine(request);
            return runResult;
        }

        //获取排产单号
        [ApiAuthorize]
        [HttpPost]
        [WebMethod]
        [Route("AddCount")]
        public object AddOrderCount([FromBody] ProOrderAddCount request)
        {
            RunResult<string> runResult = ProductOrderManager.UpdateOrderAddProCount
                (request.OrderID, request.addCount);

            //从数据库获取订单
            Debug.WriteLine(request);
            return runResult;
        }

        //获取排产单号
        [ApiAuthorize]
        [HttpPost]
        [WebMethod]
        [Route("UpdateCount")]
        public object UpdateOrderCount([FromBody] ProOrderAddCount request)
        {
            RunResult<string> runResult = ProductOrderManager.UpdateOrderProData(request.OrderID, request.addCount);

            //从数据库获取订单
            Debug.WriteLine(runResult.ToString());
            return runResult;
        }



        public class ProOrderNo
        {
            public string OrderNo { get; set; }
        }

        public class ProOrderAddCount
        {
            public int OrderID { get; set; }
            public int addCount { get; set; }

        }

    }
}
using NX_WMS_TM_ApiNet.Models;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using System.Text;
using FromBodyAttribute = System.Web.Http.FromBodyAttribute;

namespace AGVWebApi_WMS.Controllers
{
    [RoutePrefix("api")]
   
    public class AGVController : ApiController
    {
        log4net.ILog log = log4net.LogManager.GetLogger("AGVController");
        public string Options()
        {
            return null;
        }



      /// <summary>
      /// 获取标签记录数据给WMS
      /// </summary>
      /// <param name="Barcode"></param>
      /// <returns></returns>
        [HttpPost]
        [HttpGet]
        [Route("feedbackTask")]
        public HttpResponseMessage feedbackTask([FromBody]object Request)
        {

            // log.Info("GetBarCodeMessage:::::::");
            // log.Info(Barcode.ToString());
            //if(string.IsNullOrEmpty(Barcode))
            // {
            //     return new HttpResponseMessage { Content = new StringContent("没有传入标签", Encoding.GetEncoding("UTF-8"), "application/json") };
            // }

            // GeLiData_WMS.DaoUtils.DbBase<GeLiData_WMS.Dao.PrintRecordLists> dbBase = new GeLiData_WMS.DaoUtils.DbBase<GeLiData_WMS.Dao.PrintRecordLists>();
            // var printRecordList = dbBase.GetList(u => u.BarcodeNumber == Barcode,false,GeLiData_WMS.DaoUtils.DbMainSlave.Master);
            // if (printRecordList.Count==0)
            // {
            //     return new HttpResponseMessage { Content = new StringContent("没有该标签", Encoding.GetEncoding("UTF-8"), "application/json") };

            // }
            // TinyMapper.Bind<List<PrintRecordLists>, List<PrintRecordListsDto>>();
            // var printRecordListDto = TinyMapper.Map<List<PrintRecordListsDto>>(printRecordList);
            //var back = JsonConvert.SerializeObject(printRecordListDto);
            // return new HttpResponseMessage { Content = new StringContent(back, Encoding.GetEncoding("UTF-8"), "application/json") };
            return null;

        }

       
    }
}
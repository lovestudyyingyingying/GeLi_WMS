
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Services;
//using WebApi_WMS.Models;


using System.Threading.Tasks;
using System.Web;
using System.Configuration;

using GeLiService_WMS.Entity;
using GeLiService_WMS.Entity.AGVApiEntity;
using GeLi_Utils.Entity.AGVOrderEntity;
using GeLiService_WMS.Entity.AGVOrderEntity;
using GeLi_Utils.Entity.AGVApiEntity;
using OrderResult = GeLi_Utils.Entity.AGVApiEntity.OrderResult;
using GeLiService_WMS;

namespace WebApi_WMS.Controllers
{
    [RoutePrefix("api/AGV")]
    public class AGVController : BaseController
    {

        public AGVController()
        {

        }

        /// <summary>
        /// 接收任务状态接口
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("feedbackTask")]
        public RunResult<string> MissionStates([FromBody] MissionState ms)
        {
            try
            {
                Logger.Default.Process(new Log(LevelType.Debug, "AGV_APi_feedbackTask" + JsonConvert.SerializeObject(ms)));
                return AGVApiManager.UpdateMissionStates(ms);
            }
            catch (Exception e)
            {
                return new RunResult<string>() { code=999 , desc="失败",message=e.ToString() };
            }
        }


        /// <summary>
        /// 码盘机进出许可
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("deviceApply")]
        public OrderResult DeviceApply([FromBody] DeviceApply ms)
        {
            try
            {
                Logger.Default.Process(new Log(LevelType.Debug, "AGV_APi_deviceApply" + JsonConvert.SerializeObject(ms)));
                if (ms.type == 1)
                    return movestockManager.AgvMoveInMaPanJi(ms);
                if (ms.type == 2)
                    return movestockManager.AgvMoveOutMaPanJi(ms);

                return new OrderResult() { msg = "未知操作类型", code = 999 };
            }
            catch(Exception e)
            {

                return new OrderResult() { code=999,msg=e.Message.ToString() };
            }

        }



       

    }
}

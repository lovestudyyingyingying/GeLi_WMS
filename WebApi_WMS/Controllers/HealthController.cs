using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Services;

namespace WebApi_WMS.Controllers
{
    public class HealthController : BaseController
    {
        /// <summary>
        /// 接收任务状态接口
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        [WebMethod]
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        public async Task<object> MissionStates()
        {
            Task task= MissionService.FirstOrDefaultAsync();
            await task;
            return Ok();
        }
    }
}

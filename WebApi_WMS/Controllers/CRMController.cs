using NanXingData_WMS.Dao;
using NanXingService_WMS.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Services;
using WebApi_WMS.Models;
using WebApi_WMS.Filter;
using System.IO;
using System.Text;
using System.Threading;
using NanXingService_WMS.Utils.RedisUtils;

namespace WebApi_WMS.Controllers
{
    /// <summary>
    /// CRM调用接口
    /// </summary>
    [RoutePrefix("api/CRM")]
    public class CRMController : BaseController
    {
        /// <summary>
        /// 插入CRM任务单
        /// </summary>
        /// <param name="crmpPlanList">多个任务单列表</param>
        /// <returns>插入结果</returns>
        [ApiAuthorize]
        [HttpPost]
        [WebMethod]
        [Route("CRMPlanApply")]
        public async Task<ApiResult<string>> AddCRMPlanApply([FromBody] CRMPlanWriter crmpPlanList)
        {
            //Thread.Sleep(10000);
            ApiResult<string> apiResult = new ApiResult<string>();
           
            try
            {
                var ret = await crmPlanManager.AddOrUpdateCRMApply(crmpPlanList);

                if (ret.message== ResultMsg.success) {
                    apiResult.code = 1000;
                    apiResult.message = "success";
                    apiResult.data = string.Empty;
                }
                else
                {
                    apiResult.code = 1001;
                    apiResult.message = "fail";
                    apiResult.data = ret.message;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                apiResult.code = 1001;
                apiResult.message = "fail";
                apiResult.data = ex.ToString();
            }

            return apiResult;
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [ApiAuthorize]
        [HttpPost]
        [WebMethod]
        [Route("UpdateApplyFiles")]
        public async Task<ApiResult<string>> UpdateApplyFiles(string CRMID, string fileName)
        {
            ApiResult<string> apiResult = new ApiResult<string>();
            try
            {
                var request = System.Web.HttpContext.Current.Request;
                
                bool ret = await crmPlanManager.SaveCRMFile(CRMID,fileName,request);
                if (ret)
                {
                    apiResult.code = 1000;
                    apiResult.message = "success";
                    apiResult.data = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                apiResult.code = 1001;
                apiResult.message = "fail";
                apiResult.data = ex.ToString();
            }

            return apiResult;
        }

        [HttpPost]
        [HttpGet]
        [WebMethod]
        [Route("WarmUp")]
        public async Task<ApiResult<string>> WarmUp()
        {
            var apiResult = new ApiResult<string>();
            var task = crmFilesService.FirstOrDefaultAsync();
            await task;
            apiResult.code = 1000;
            apiResult.message = "success";
            apiResult.data = string.Empty;
            return apiResult;
        }

    }
}

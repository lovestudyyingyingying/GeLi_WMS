using NanXingService_WMS.Entity.CRMEntity;
using NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity;
using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys;
using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys.QueueOutputEntitys;
using NanXingService_WMS.Entity.CRMItemEntity;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.AutoMapper;
using NanXingService_WMS.Utils.RedisUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilsSharp.Standard;

namespace NanXingService_WMS.Helper.APS
{
    public class CRMApiHelper
    {
        RedisHelper redisHelper = new RedisHelper();
        HttpUtils httpUtils = new HttpUtils();

        string apiKey = "WriteApi";

        #region token

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns>CRM的Token</returns>Utils.RedisUtils.RedisHelper.RedisHelper() 
        public CRMTokenResult GetToken()
        {
            string keyName = "CRM:CRMToken";
            //1 先从Redis中查找，如果没有则调用认证接口，将Token存放在Redis中
            CRMTokenResult crmToken = redisHelper.StringGet<CRMTokenResult>(keyName);
            if (crmToken == null)
            {
                using (var locker = redisHelper.CreateLock("ItemUpdate", TimeSpan.FromSeconds(10)))
                {
                    crmToken = redisHelper.StringGet<CRMTokenResult>(keyName);
                    if (crmToken == null)
                    {
                        CRMTokenRequest crtRequest = new CRMTokenRequest();
                        string result = httpUtils.HttpPost("https://open.fxiaoke.com/cgi/corpAccessToken/get/V2",
                            crtRequest, typeof(CRMTokenRequest));
                        crmToken = JsonConvert.DeserializeObject<CRMTokenResult>(result);
                        //将Token存放在Redis中
                        redisHelper.StringSet(keyName, crmToken, DateTime.Now.AddSeconds(crmToken.expiresIn));
                    }

                }
            }
            return crmToken;
        }

        #endregion

        # region 物料信息接口
        /// <summary>
        /// 组成物料信息接口请求信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="lastModTime"></param>
        /// <returns>物料信息</returns>
        private CRMItemRequest CreateItemRequest(long lastModTime,int startIndex,int readCount)
        {
            CRMTokenResult token = GetToken();
            CRMItemRequest crmItemRequest = new CRMItemRequest();
            crmItemRequest.corpId = token.corpId;
            crmItemRequest.corpAccessToken = token.corpAccessToken;

            Filter filter = new Filter();
            filter.field_name = "last_modified_time";
            filter.field_values = new long[] { lastModTime };
            filter.@operator = "GT";

            Search_Query_Info query_Info = new Search_Query_Info();
            query_Info.offset = startIndex;
            query_Info.limit = readCount;
            query_Info.filters = new Filter[1] { filter };

            RequestData requestData = new RequestData();
            requestData.search_query_info = query_Info;
            requestData.dataObjectApiName = "ProductObj";

            crmItemRequest.data = requestData;
            return crmItemRequest;
        }
        /// <summary>
        /// 调用物料信息接口
        /// </summary>
        /// <param name="lastModTime"></param>
        /// <returns>物料信息</returns>
        public CRMItemResult GetItemFromApi(long lastModTime,int startIndex,int 
            readCount)
        {
            //第一步：获取Token
            //long startTime= UnixDateTImeUtils.ConvertDateTimeInt(DateTime.Now);
            //第二步：组成Request
            Debug.WriteLine($"startIndex:{startIndex}----readCount:{readCount}");
            CRMItemRequest itemRequest = CreateItemRequest(lastModTime, startIndex, readCount);
            string result = httpUtils.HttpPost("https://open.fxiaoke.com/cgi/crm/v2/data/query", itemRequest, typeof(CRMItemRequest));
            
            CRMItemResult itemResult = JsonConvert.DeserializeObject<CRMItemResult>(result); ;
            //List<ItemInfoDatalist> itemInfoDatalists = new List<ItemInfoDatalist>();
            //int startIndex = 0;
            //int readCount = 2000;
            //for (int i = 0; i < lastCount;i+= readCount) 
            //{
            //    startIndex=startIndex+i;
               
            //    if (itemResult != null && itemResult.data != null)
            //    {
            //        itemInfoDatalists.AddRange(itemResult.data.dataList);
            //        //lastCount = itemResult.data.total;
            //    }
            //}
            if (itemResult != null && itemResult.data.dataList!= null)
                return itemResult;
            else
                return null;
            //RedisCacheHelper.Add("last_modified_time", startTime, DateTime.Now.AddYears(2));

        }
        // <summary>
        /// 调用物料信息接口
        /// </summary>
        /// <param name="lastModTime"></param>
        /// <returns>物料信息</returns>
        public async Task<ItemInfoDatalist[]> GetItemFromApiAsync(long lastModTime, int startIndex, int
            readCount)
        {
            //第一步：获取Token
            //long startTime= UnixDateTImeUtils.ConvertDateTimeInt(DateTime.Now);
            //第二步：组成Request
            CRMItemRequest itemRequest = CreateItemRequest(lastModTime, startIndex, readCount);
            string result = await httpUtils.HttpPostAsync("https://open.fxiaoke.com/cgi/crm/v2/data/query", itemRequest, typeof(CRMItemRequest));
            CRMItemResult itemResult = JsonConvert.DeserializeObject<CRMItemResult>(result); ;
            if (itemResult != null && itemResult.data.dataList != null)
                return itemResult.data.dataList.ToArray();
            else
                return null;
            //RedisCacheHelper.Add("last_modified_time", startTime, DateTime.Now.AddYears(2));

        }
        #endregion

        #region 获取CRM申请单明细信息

        private CRMApplyDetailRequest GetDetailRequest(string crmID)
        {
            CRMTokenResult token = GetToken();
            CRMApplyDetailRequest crmDetailRequest = new CRMApplyDetailRequest();
            crmDetailRequest.corpId = token.corpId;
            crmDetailRequest.corpAccessToken = token.corpAccessToken;
            crmDetailRequest.currentOpenUserId = "FSUID_9D013E3437E2C860E6E5B27BE5D52B6A";
            DetailFilter filter = new DetailFilter();
            filter.field_name = "_id";
            filter.field_values = crmID;
            filter.@operator = "EQ";

            DetailSearch_Query_Info query_Info = new DetailSearch_Query_Info();
            query_Info.offset = 0;
            query_Info.limit = 200;
            query_Info.filters = new DetailFilter[1] { filter };

            CRMApplyDetailRequestData requestData = new CRMApplyDetailRequestData();
            requestData.dataObjectApiName = "object_S1KIF__c";
            requestData.search_query_info = query_Info;

            crmDetailRequest.data = requestData;
            return crmDetailRequest;
        }


        public DetailDatalist[] GetCRMDetailFromApi(string crmID)
        {

            //第二步：组成Request
            CRMApplyDetailRequest itemRequest = GetDetailRequest(crmID);
            Debug.WriteLine(JsonConvert.SerializeObject(itemRequest));
            string result = httpUtils.HttpPost("https://open.fxiaoke.com/cgi/crm/custom/v2/data/query", 
                itemRequest, typeof(CRMApplyDetailRequest));
            CRMApplyDetail itemResult = JsonConvert.DeserializeObject<CRMApplyDetail>(result);

            if (itemResult != null && itemResult.data != null)
                return itemResult.data.dataList;
            else
                return null;
            //RedisCacheHelper.Add("last_modified_time", startTime, DateTime.Now.AddYears(2));

        }



        #endregion 获取CRM申请单明细信息

        #region 回写CRM申请单的排产状态
        //public BaseResult<string> WriteCRMApplyPush(CRMPlanEntity planList)
        //{
        //    var result = new BaseResult<string>();
        //    CRMTokenResult token = GetToken();
        //    CRMApplyChangeRequest entity = new CRMApplyChangeRequest();
        //    entity.corpId = token.corpId;
        //    entity.corpAccessToken = token.corpAccessToken;
        //    entity.triggerWorkFlow = false;
        //    Entity.CRMEntity.CRMAppleNoEntity.Data data = new Entity.CRMEntity.CRMAppleNoEntity.Data();
        //    QueueOutputEntityBase object_Data = AutoMapperUtils.mapper.Map<CRMEntityPushPlan>(planList);
        //    data.object_data = object_Data;
        //    entity.data = data;
        //    string httpResult = httpUtils.HttpPost("https://open.fxiaoke.com/cgi/crm/custom/v2/data/update",
        //        entity, typeof(CRMApplyChangeRequest));
        //    CRMResult CRMResult = JsonConvert.DeserializeObject<CRMResult>(httpResult);
        //    if (CRMResult.errorCode == 0)
        //        result.SetOk();
        //    else
        //        result.SetError(CRMResult.errorMessage);
        //    return result;

        //}

        #endregion

        #region 回写修改CRM申请单的状态

        public BaseResult<string> WriteCRMApply<T, TK>(TK proTask)
           where T : QueueOutputEntityBase where TK : QueueInputEntityBase
        {
            using (redisHelper.CreateLock($"{apiKey}",
                TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(30),
                TimeSpan.FromMilliseconds(500)))
            {
                var result = new BaseResult<string>();
                CRMTokenResult token = GetToken();
                CRMApplyChangeRequest entity = new CRMApplyChangeRequest();
                entity.corpId = token.corpId;
                entity.corpAccessToken = token.corpAccessToken;
                entity.triggerWorkFlow = true;
                Entity.CRMEntity.CRMAppleNoEntity.Data data = new Entity.CRMEntity.CRMAppleNoEntity.Data();

                //data.object_data = (T)object_Base.GetObjectTest(proTask);
                data.object_data = AutoMapperUtils.mapper.Map<T>(proTask);

                entity.data = data;
                string httpResult = httpUtils.HttpPost("https://open.fxiaoke.com/cgi/crm/custom/v2/data/update",
                    entity, typeof(CRMApplyChangeRequest));
                CRMResult CRMResult = JsonConvert.DeserializeObject<CRMResult>(httpResult);
                if (CRMResult.errorCode == 0)
                    result.SetOk();
                else if (CRMResult.errorCode == 50009)
                {
                    result.SetOk(CRMResult.errorMessage);
                }
                else
                    result.SetError(CRMResult.errorMessage);
                return result;
            }
        }

        public BaseResult<string> WriteCRMApply(CRMEntityAll cRMEntityAll)
        {
            using (redisHelper.CreateLock($"{apiKey}",
                TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(30),
                TimeSpan.FromMilliseconds(500)))
            {
                var result = new BaseResult<string>();
                CRMTokenResult token = GetToken();
                CRMApplyChangeRequest entity = new CRMApplyChangeRequest();
                entity.corpId = token.corpId;
                entity.corpAccessToken = token.corpAccessToken;
                entity.triggerWorkFlow = true;
                Entity.CRMEntity.CRMAppleNoEntity.Data data = new Entity.CRMEntity.CRMAppleNoEntity.Data();

                //data.object_data = (T)object_Base.GetObjectTest(proTask);
                data.object_data = cRMEntityAll;

                entity.data = data;
                string httpResult = httpUtils.HttpPost("https://open.fxiaoke.com/cgi/crm/custom/v2/data/update",
                    entity, typeof(CRMApplyChangeRequest));
                CRMResult CRMResult = JsonConvert.DeserializeObject<CRMResult>(httpResult);
                if (CRMResult.errorCode == 0)
                    result.SetOk();
                else if (CRMResult.errorCode == 50009)
                {
                    result.SetOk(CRMResult.errorMessage);
                }
                else
                    result.SetError(CRMResult.errorMessage);
                return result;
            }
        }


        #endregion

    }

}

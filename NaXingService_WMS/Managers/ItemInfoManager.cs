using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.CRMItemEntity;
using NanXingService_WMS.Helper.APS;
using NanXingService_WMS.Services.APS;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.AutoMapper;
using NanXingService_WMS.Utils.RedisUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services
{
    public class ItemInfoManager
    {
        ItemInfoService itemInfoService = new ItemInfoService();
        RedisHelper stringCacheRedisHelper= new RedisHelper();
        CRMApiHelper crmApiHelper = new CRMApiHelper();
        

        #region 从CRM更新物料数据
        Dictionary<string, string> nameDic = null;

        public void StartUpdate()
        {
            //分布式缓存锁
            using (var locker= stringCacheRedisHelper.CreateLock("ItemUpdate",TimeSpan.FromSeconds(300)))
            {
                try
                {
                    ParseItems();
                    Logger.Default.Process(new Log(LevelType.Info,
                    $"ItemThread:获取物料信息。。。"));
                }
                catch (Exception ex)
                {
                    Logger.Default.Process(new Log(LevelType.Error,
                        "ItemThread:\r\n" + ex.ToString()));
                }
            }
        }

        string Key_lastModTime = "ItemInfo:LastModTime";
        /// <summary>
        /// 分解物料信息进行数据操作
        /// </summary>
        public void ParseItems()
        {
            //最后修改时间
            long lastModTime = stringCacheRedisHelper.StringGet<long>(Key_lastModTime);
            DateTime? lastModDateTime = null;
            if (lastModTime == 0)
            {
                lastModDateTime = GetMAXModTime();
                lastModTime = lastModDateTime == null ? 1632931200000: UnixDateTImeUtils.ConvertDateTimeInt(lastModDateTime.Value);
            }
            //if(lastModTime==0)
            //DateTime lastModTime=
            //1、获取所有修改日期之后的数据
            //lastModTime
            int startIndex = 0;
            int readCount = 500;
            int lastCount = 0;
           
            for (int i=0;i< lastCount/ readCount + 2; i++)
            {
               
                CRMItemResult result = crmApiHelper.GetItemFromApi(lastModTime, startIndex, readCount);
                if (result == null)
                    return;
               
                lastCount = result.data.total;
                startIndex = readCount * i;
                ItemInfoDatalist[] dataList = result.data.dataList;
                if (dataList != null && dataList.Length > 0)
                {
                    ItemInfo[] itemInfo = AutoMapperUtils.mapper.Map<ItemInfo[]>(dataList);

                    List<ItemInfo> itemInfos = itemInfo.ToList();
                    itemInfoService.InsertOrUpdateListByDataTable(itemInfos, new Expression<Func<ItemInfo, object>>[] { u => u.CRMID });


                    //获取最大的lastModTime存入Redis
                    //stringCacheRedisHelper.StringSet(Key_lastModTime, dataList.Max(u => u.last_modified_time), DateTime.Now.AddYears(1));
                    itemInfoService.SaveChanges();
                }
            }
            
            stringCacheRedisHelper.StringSet(Key_lastModTime, lastModTime, DateTime.Now.AddYears(1));
            
                
        }

        private void AddItemInfo(ItemInfoDatalist[] dataList)
        {
            DataTable dataTable_ADD = null;
            dataTable_ADD = itemInfoService.ClassToDataTable(typeof(ItemInfoDatalist));
            //全部转为新增数据
            foreach (ItemInfoDatalist temp in dataList)
            {
                dataTable_ADD = itemInfoService.ParseInDataTable(dataTable_ADD, temp);
            }
            if (!dataTable_ADD.Columns.Contains("UpdateTime"))
            {
                dataTable_ADD.Columns.Add("UpdateTime");
                foreach (DataRow temp in dataTable_ADD.Rows)
                {
                    temp["UpdateTime"] = DateTime.Now.ToString("G");
                }
            }
            itemInfoService.SetDataTableToTable(dataTable_ADD, "ItemInfo", ConvertDatalistsToDataTable());
            stringCacheRedisHelper.StringSet(Key_lastModTime, dataList.Max(u => u.last_modified_time), DateTime.Now.AddYears(1));
        }

        private Dictionary<string, string> ConvertDatalistsToDataTable()
        {
            if (nameDic==null)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("name", "ItemNo");
                dic.Add("CRMID", "CRMID");
                dic.Add("product_code", "ItemName");
                dic.Add("product_spec", "Spec");
                dic.Add("field_1n4aG__c", "MainUtil");
                dic.Add("field_owUk6__c", "SlaveUtil");
                dic.Add("field_p5rBp__c", "ConvertRate");
                dic.Add("CreateTime", "CreateTime");
                dic.Add("ModTime_CRM", "ModTime_CRM");
                dic.Add("UpdateTime", "UpdateTime");
                nameDic = dic;
            }
            return nameDic;
        }

        private DateTime? GetMAXModTime()
        {
            DateTime? dateTime=
            itemInfoService.Query<DateTime?>("select max(ModTime_CRM) from ItemInfo",new List<System.Data.SqlClient.SqlParameter>(),DbMainSlave.Master).FirstOrDefault() ;
            return dateTime;
        }
        
        #endregion
    }
}

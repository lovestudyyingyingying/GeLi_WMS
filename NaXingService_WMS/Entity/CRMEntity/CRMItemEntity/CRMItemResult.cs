using NanXingService_WMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMItemEntity
{
    public class CRMItemResult
    {
        public string traceId { get; set; }
        public Data data { get; set; }
        public string errorDescription { get; set; }
        public string errorMessage { get; set; }
        public int errorCode { get; set; }
    }

    public class Data
    {
        public ItemInfoDatalist[] dataList { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
    }

    public class ItemInfoDatalist
    {
        public string field_p5rBp__c { get; set; }
        public bool is_saleable { get; set; }
        public string field_ap54t__c { get; set; }
        public string extend_obj_data_id { get; set; }
        public Created_By__R created_by__r { get; set; }
        public int total_num { get; set; }
        public string barcode { get; set; }
        
        public string field_k52pf__c { get; set; }
        public string lock_status { get; set; }
        public Data_Own_Department__R data_own_department__r { get; set; }
        public string is_giveaway { get; set; }
        public long create_time { get; set; }
        public string field_owUk6__c { get; set; }
        public string version { get; set; }
        public string[] created_by { get; set; }
        public Relevant_Team[] relevant_team { get; set; }
        public string[] data_own_department { get; set; }
        public string field_MBWww__c { get; set; }
        public string field_1n4aG__c { get; set; }
        public string name { get; set; }
        public string _id { get; set; }
        public Data_Own_Department__L[] data_own_department__l { get; set; }
        public string field_bE9px__c { get; set; }
        public string product_status { get; set; }
        public string product_code { get; set; }
        public long on_shelves_time { get; set; }
        public bool is_deleted { get; set; }
        public Owner__L[] owner__l { get; set; }
        public string relevant_team__r { get; set; }
        public Owner__R owner__r { get; set; }
        public string[] owner { get; set; }
        public bool is_package { get; set; }
        public long last_modified_time { get; set; }
        public string life_status { get; set; }
        public Last_Modified_By__L[] last_modified_by__l { get; set; }
        public Created_By__L[] created_by__l { get; set; }
        public string[] last_modified_by { get; set; }
        public string record_type { get; set; }
        public Last_Modified_By__R last_modified_by__r { get; set; }
        public string product_spec { get; set; }
        public string category { get; set; }


        public DateTime CreateTime { 
            get {
                return UnixDateTImeUtils.ConvertIntDateTime(create_time);
            } 
        }
        public DateTime ModTime_CRM
        {
            get
            {
                return UnixDateTImeUtils.ConvertIntDateTime(last_modified_time);
            }
        }
        public string CRMID
        {
            get
            {
                return _id;
            }
        }
    }


    public class Created_By__R
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Data_Own_Department__R
    {
        public string deptName { get; set; }
        public string deptId { get; set; }
        public int status { get; set; }
    }

    public class Owner__R
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Last_Modified_By__R
    {
        public string name { get; set; }
        public string id { get; set; }
        public string picAddr { get; set; }
        public string description { get; set; }
        public string dept { get; set; }
        public string supervisorId { get; set; }
        public long modifyTime { get; set; }
        public string post { get; set; }
        public long createTime { get; set; }
        public string phone { get; set; }
        public string nickname { get; set; }
        public string tenantId { get; set; }
        public string email { get; set; }
        public int status { get; set; }
    }

    public class Relevant_Team
    {
        public string[] teamMemberEmployee { get; set; }
        public string teamMemberType { get; set; }
        public string teamMemberRole { get; set; }
        public string teamMemberPermissionType { get; set; }
    }

    public class Data_Own_Department__L
    {
        public string deptId { get; set; }
        public string deptName { get; set; }
        public int status { get; set; }
    }

    public class Owner__L
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Last_Modified_By__L
    {
        public string id { get; set; }
        public string name { get; set; }
        public string tenantId { get; set; }
        public string picAddr { get; set; }
        public string email { get; set; }
        public string nickname { get; set; }
        public string supervisorId { get; set; }
        public string phone { get; set; }
        public string description { get; set; }
        public int status { get; set; }
        public long createTime { get; set; }
        public long modifyTime { get; set; }
        public string dept { get; set; }
        public string post { get; set; }
    }

    public class Created_By__L
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}

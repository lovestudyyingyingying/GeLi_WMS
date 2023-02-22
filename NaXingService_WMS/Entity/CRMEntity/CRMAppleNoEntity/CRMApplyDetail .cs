using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity
{


    public class CRMApplyDetailRequest
    {
        public string corpAccessToken { get; set; }
        public string corpId { get; set; }
        public string currentOpenUserId { get; set; }
        public CRMApplyDetailRequestData data { get; set; }
    }

    public class CRMApplyDetailRequestData
    {
        public string dataObjectApiName { get; set; }
        public DetailSearch_Query_Info search_query_info { get; set; }
    }

    public class DetailSearch_Query_Info
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public DetailFilter[] filters { get; set; }
    }

    public class DetailFilter
    {
        public string field_name { get; set; }
        public string field_values { get; set; }
        public string @operator { get; set; }
    }


    public class CRMApplyDetail
    {
        public string traceId { get; set; }
        public DetailData data { get; set; }
        public string errorDescription { get; set; }
        public string errorMessage { get; set; }
        public int errorCode { get; set; }
    }


    public class DetailData
    {
        public DetailDatalist[] dataList { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
    }

    public class DetailDatalist
    {
        public long field_p5sN5__c { get; set; }
        public string field_bMaSx__c__r { get; set; }
        public long field_s0ryw__c { get; set; }
        public string field_B1F7t__c__r { get; set; }
        public string owner_department_id { get; set; }
        public string field_Luo72__c { get; set; }
        public string field_0Uw1j__c { get; set; }
        public Data_Own_Department__R data_own_department__r { get; set; }
        public string field_0h244__c { get; set; }
        public string field_bMaSx__c { get; set; }
        public long field_abRIo__c { get; set; }
        public string field_n2r72__c__relation_ids { get; set; }
        public string version { get; set; }
        public string field_1rrOy__c { get; set; }
        public long field_l8aa2__c { get; set; }
        public Data_Own_Department__L[] data_own_department__l { get; set; }
        public long field_614ed__c { get; set; }
        public string field_rHKfQ__c { get; set; }
        public string field_dTK9q__c { get; set; }
        public long field_orBwH__c { get; set; }
        public string field_A2i6w__c { get; set; }
        public string field_cmFG8__c { get; set; }
        public string field_6A7cc__c { get; set; }
        public string field_S24fz__c { get; set; }
        public string field_1ouek__c { get; set; }
        public string field_4yUJU__c { get; set; }
        public string field_610Kh__c { get; set; }
        public long field_9a1pa__c { get; set; }
        public long last_modified_time { get; set; }
        public string life_status { get; set; }
        public string field_69b3E__c { get; set; }
        public string field_4iY04__c { get; set; }
        public string field_rb734__c { get; set; }
        public string field_s9KMk__c { get; set; }
        public string field_w2lSP__c { get; set; }
        public string field_OK9ER__c { get; set; }
        public string field_5iPu6__c { get; set; }
        public long field_8hUjG__c { get; set; }
        public long field_40rQb__c { get; set; }
        public string field_PUHit__c { get; set; }
        public string order_by { get; set; }
        public string field_r67M2__c { get; set; }
        public string field_C7Vg6__c { get; set; }
        public string field_r5Tqq__c { get; set; }
        public string field_7nsNU__c { get; set; }
        public string field_75GaJ__c { get; set; }
        public string field_B1F7t__c__relation_ids { get; set; }
        public string field_pl8w1__c { get; set; }
        public string field_L12kn__c { get; set; }
        public long field_spgPb__c { get; set; }
        public Created_By__R created_by__r { get; set; }
        public string field_w5och__c { get; set; }
        public object[] field_80085__c { get; set; }
        public long field_B1wK3__c { get; set; }
        public long field_mzspU__c { get; set; }
        public int total_num { get; set; }
        public string owner_department { get; set; }
        public string field_n2r72__c { get; set; }
        public string field_zMmYu__c { get; set; }
        public string field_ntVuf__c { get; set; }
        public string field_7gsp0__c { get; set; }
        public string lock_status { get; set; }
        public long create_time { get; set; }
        public string field_bv6S7__c { get; set; }
        public string field_rEdHq__c { get; set; }
        public string field_OBFvG__c { get; set; }
        public string field_31flP__c { get; set; }
        public string[] created_by { get; set; }
        public string[] data_own_department { get; set; }
        public string field_I54DT__c { get; set; }
        public string field_yAnwj__c { get; set; }
        public string field_2i1l9__c { get; set; }
        public string name { get; set; }
        public string _id { get; set; }
        public string field_Vm3lG__c { get; set; }
        public string field_rJ2F0__c { get; set; }
        public string field_MLRho__c { get; set; }
        public string field_n2r72__c__r { get; set; }
        public string field_j42uv__c { get; set; }
        public string field_B1F7t__c { get; set; }
        public string field_d2AW2__c { get; set; }
        public long field_2F5zV__c { get; set; }
        public long field_2xK4R__c { get; set; }
        public string field_evz13__c { get; set; }
        public string field_1wc9U__c { get; set; }
        public bool is_deleted { get; set; }
        public string field_8ktK8__c { get; set; }
        public Owner__L[] owner__l { get; set; }
        public long field_m1OPt__c { get; set; }
        public string field_ewHJQ__c { get; set; }
        public Owner__R owner__r { get; set; }
        public string field_bMaSx__c__relation_ids { get; set; }
        public string[] owner { get; set; }
        public string field_9iYem__c { get; set; }
        public string field_RnAI2__c { get; set; }
        public string field_1tqvO__c { get; set; }
        public Last_Modified_By__L[] last_modified_by__l { get; set; }
        public string field_1kR5w__c { get; set; }
        public Created_By__L[] created_by__l { get; set; }
        public string field_f117d__c { get; set; }
        public string[] last_modified_by { get; set; }
        public string record_type { get; set; }
        public Last_Modified_By__R last_modified_by__r { get; set; }
        public string field_loso1__c { get; set; }
        public string field_goHWR__c { get; set; }
        public string field_teU84__c { get; set; }
        public string field_4V8kp__c { get; set; }
        public long field_1rL3W__c { get; set; }
        public string field_qKHGz__c { get; set; }

        public string field_070o1__c { get; set; }
        public string field_zPc3r__c { get; set; }
        public string field_6mobW__c { get; set; }
        public string field_9am1y__c { get; set; }
        public string field_yJik8__c { get; set; }
        public string field_f2A0g__c { get; set; }
        public string field_e3d3W__c { get; set; }

        public string field_0zFL1__c { get; set; }
        public string field_70cFm__c { get; set; }
        public string field_hbTdR__c { get; set; }
        public string field_g4z0y__c { get; set; }
        public string field_UE26m__c { get; set; }
        public string field_cg0ac__c { get; set; }
        public string field_t64Wa__c { get; set; }

        public string field_xqV0O__c { get; set; }
        public string field_Yyg67__c { get; set; }
        public string field_1yTW1__c { get; set; }
        public string field_4xcUU__c { get; set; }
        public string field_n6fUQ__c { get; set; }
        public string field_1gETl__c { get; set; }
        public string field_2oZ0S__c { get; set; }

    }

    public class Data_Own_Department__R
    {
        public string deptName { get; set; }
        public string deptId { get; set; }
        public string parentId { get; set; }
        public int status { get; set; }
    }

    public class Created_By__R
    {
        public string picAddr { get; set; }
        public string description { get; set; }
        public string dept { get; set; }
        public string supervisorId { get; set; }
        public long modifyTime { get; set; }
        public string post { get; set; }
        public long createTime { get; set; }
        public string phone { get; set; }
        public string name { get; set; }
        public string nickname { get; set; }
        public string tenantId { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public int status { get; set; }
    }

    public class Owner__R
    {
        public string picAddr { get; set; }
        public string description { get; set; }
        public string dept { get; set; }
        public string supervisorId { get; set; }
        public long modifyTime { get; set; }
        public string post { get; set; }
        public long createTime { get; set; }
        public string phone { get; set; }
        public string name { get; set; }
        public string nickname { get; set; }
        public string tenantId { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public int status { get; set; }
    }

    public class Last_Modified_By__R
    {
        public string picAddr { get; set; }
        public string description { get; set; }
        public string dept { get; set; }
        public string supervisorId { get; set; }
        public long modifyTime { get; set; }
        public string post { get; set; }
        public long createTime { get; set; }
        public string phone { get; set; }
        public string name { get; set; }
        public string nickname { get; set; }
        public string tenantId { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public int status { get; set; }
    }

    public class Data_Own_Department__L
    {
        public string parentId { get; set; }
        public string deptId { get; set; }
        public string deptName { get; set; }
        public int status { get; set; }
    }

    public class Owner__L
    {
        public string id { get; set; }
        public string tenantId { get; set; }
        public string name { get; set; }
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

    public class Last_Modified_By__L
    {
        public string id { get; set; }
        public string tenantId { get; set; }
        public string name { get; set; }
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
        public string tenantId { get; set; }
        public string name { get; set; }
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
}

using NanXingData_WMS.DaoUtils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services
{
    public class LiuShuiHaoService : DbBase
    {
        //执行存储过程，返回流水号
        //Type t = typeof(string);
        List<SqlParameter> sqlParms = new List<SqlParameter>(1);

        private string ProLeft = "1";
        private string TrayLeft = "2";
        private string PlanOrderLeft = "P";
        private string SCOrderLeft = "S";

        /// <summary>
        /// 得到打印托盘的流水号
        /// </summary>
        /// <returns></returns>
        public string GetTrayLSH()
        {
            //sqlParms.Clear();
            List<SqlParameter> sqlParameter =new List<SqlParameter>(1) { new SqlParameter("@MaintainCate", "PrintTest") };

            return TrayLeft + QueryOne<string>("exec GetSeq_2 @MaintainCate", sqlParameter, DbMainSlave.Master);
        }
        /// <summary>
        /// 得到排产单的流水号
        /// </summary>
        /// <returns></returns>
        public string GetPlanOrderLSH()
        {
            sqlParms.Clear();
            sqlParms.Add(new SqlParameter("@MaintainCate", "PlanOrder"));
            return PlanOrderLeft + QueryOne<string>("exec GetSeq_1 @MaintainCate", sqlParms, DbMainSlave.Master);
        }

        /// <summary>
        /// 得到排产单的流水号
        /// </summary>
        /// <returns></returns>
        public string GetProOrderLSH()
        {
            sqlParms.Clear();
            sqlParms.Add(new SqlParameter("@MaintainCate", "ProOrder"));
            return SCOrderLeft + QueryOne<string>("exec GetSeq_1 @MaintainCate", sqlParms, DbMainSlave.Master);
        }

        public string GetOutStockNoLSH()
        {
            sqlParms.Clear();
            sqlParms.Add(new SqlParameter("@MaintainCate", "Out"));
            return QueryOne<string>("exec GetSeq @MaintainCate", sqlParms, DbMainSlave.Master);
        }

        public string GetAGVMissionNoLSH()
        {
            sqlParms.Clear();
            sqlParms.Add(new SqlParameter("@MaintainCate", "MissionNo"));
            return QueryOne<string>("exec GetSeq @MaintainCate", sqlParms, DbMainSlave.Master);
        }

        public string GetUploadBatchLSH()
        {
            sqlParms.Clear();
            sqlParms.Add(new SqlParameter("@MaintainCate", "UploadBatch"));
            return QueryOne<string>("exec GetSeq_3 @MaintainCate", sqlParms, DbMainSlave.Master);
        }
        /// <summary>
        /// 得到打印托盘的流水号
        /// </summary>
        /// <returns></returns>
        public string GetProsn()
        {
            //sqlParms.Clear();
            List<SqlParameter> sqlParameter = new List<SqlParameter>(1) { new SqlParameter("@MaintainCate", "Production") };

            return ProLeft + QueryOne<string>("exec GetSeq_4 @MaintainCate", sqlParameter, DbMainSlave.Master);
        }
    }
}

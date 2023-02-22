using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.RedisUtils;
using SQLHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services
{
    public class AGVMissionService:DbBase<AGVMissionInfo>
    {
        DbBase<AGVMissionInfo> agvMissionDao = new DbBase<AGVMissionInfo>();
        static string tableName = "AGVMissionInfo";
        RedisHelper stringCacheRedisHelper = new RedisHelper();

        #region 查询
        private static string NoRunID = "NoRunID";

        //private static string NoRunSql = @"SELECT TOP 1000 [ID]
        //          ,[MissionNo]
        //          ,[OrderTime]
        //          ,[TrayNo]
        //          ,[Mark]
        //          ,[StartLocation]
        //          ,[StartPosition]
        //          ,[EndLocation]
        //          ,[EndPosition]
        //          ,[SendState]
        //          ,[SendMsg]
        //          ,[StateMsg]
        //          ,[RunState]
        //          ,[StateTime]
        //          ,[StockPlan_ID]
        //          ,[OrderGroupId]
        //          ,[AGVCarId]
        //          ,[userId]
        //          ,[MissionFloor_ID]
        //          ,[Remark]
        //          ,[IsFloor]
        //    FROM [NanXingGuoRen_CangKu_New].[dbo].[AGVMissionInfo]
        //    where ID>@ID";

        /// <summary>
        /// 获取未执行的最大ID
        /// </summary>
        /// <returns></returns>
        private int GetMaxNoRunID()
        {
            //RedisHelper stringRedisHelper = new RedisHelper();
            int maxId = stringCacheRedisHelper.StringGet<int>(NoRunID);
            if (maxId == 0) 
            {
                DateTime dateTime = DateTime.Now.AddDays(-1);
                maxId = agvMissionDao.GetMax(u => u.ID,u=>u.OrderTime>= dateTime && 
                u.SendState.Length==0 && u.RunState.Length==0);
                stringCacheRedisHelper.StringSet(NoRunID ,maxId,DateTime.Now.AddDays(1));
            }
            return maxId;
        }

        /// <summary>
        /// 获取未分类的线程
        /// </summary>
        /// <returns></returns>
        public List<AGVMissionInfo> GetNoSplitMission()
        {

            //int ID= GetMaxNoRunID();
            //List<AGVMissionInfo> list=agvMissionDao.GetList(u => u.ID > ID, true);
            //int maxId = list.Max(u => u.ID);
            //RedisCacheHelper.Add(NoRunID, maxId, DateTime.Now.AddDays(1));

            //return list;
            DateTime dtime = DateTime.Now.AddDays(-1);
            return agvMissionDao.GetList(u => u.OrderTime> dtime
            && u.SendState == string.Empty, true);
        }

        public AGVMissionInfo GetByID(int ID, DbMainSlave dms = DbMainSlave.Slave)
        {
            return agvMissionDao.FindById(ID, dms);
        }


        public IQueryable<AGVMissionInfo> GetQuery(Expression<Func<AGVMissionInfo, bool>> whereLambda,
           bool isNoTracking = false, DbMainSlave dms = DbMainSlave.Slave
           , Expression<Func<AGVMissionInfo, string>> ordering = null)
        {
            return agvMissionDao.GetIQueryable(whereLambda, isNoTracking,  dms, ordering);
        }

        #endregion

        #region 增加
        DataTable dt = null;
        public bool Add(AGVMissionInfo agvMissionInfo)
        {
            if (dt == null)
                dt = agvMissionDao.ClassToDataTable(typeof(AGVMissionInfo));
            else
                dt.Clear();
            dt = agvMissionDao.ParseInDataTable(dt, agvMissionInfo);
            return agvMissionDao.SetDataTableToTable(dt, tableName);
        }

        public void AddMany(DataTable dtable)
        {
            agvMissionDao.SetDataTableToTable(dtable, tableName);
        }
        #endregion

        #region 修改
        

        //public void Update(AGVMissionInfo agvMissionInfo)
        //{
        //    if (dt == null)
        //        dt = agvMissionDao.ClassToDataTable(typeof(AGVMissionInfo));
        //    else
        //        dt.Clear();
        //    dt = agvMissionDao.ParseInDataTable(dt, agvMissionInfo);
        //    agvMissionDao.BatchUpdateData(dt, tableName);
        //}

        public bool UpdateMany(DataTable dtable)
        {
            return agvMissionDao.BatchUpdateData(dtable, tableName);
        }
        #endregion

        #region 删除
        //public void Delete(AGVMissionInfo agvMissionInfo)
        //{
        //    if (dt == null)
        //        dt = agvMissionDao.ClassToDataTable(typeof(AGVMissionInfo));
        //    else
        //        dt.Clear();
        //    dt = agvMissionDao.ParseInDataTable(dt, agvMissionInfo);
        //    agvMissionDao.BatchUpdateData(dt, tableName);
        //}
        #endregion

        #region 其他
        public DataTable ConvertToTable()
        {
            return agvMissionDao.ClassToDataTable(typeof(AGVMissionInfo));
        }

        public DataTable AddToDataTable(DataTable dtable, AGVMissionInfo agvMissionInfo)
        {
            return agvMissionDao.ParseInDataTable(dtable, agvMissionInfo);
        }

        #endregion
    }
}

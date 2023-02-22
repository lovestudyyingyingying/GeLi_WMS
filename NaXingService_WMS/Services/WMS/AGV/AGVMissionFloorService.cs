using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Utils;
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
    public class AGVMissionFloorService:DbBase<AGVMissionInfo_Floor>
    {
        static string tableName = "AGVMissionInfo_Floor";
        #region 查询
        //private static string NoRunID = "NoRunID";

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

        ///// <summary>
        ///// 获取未执行的最大ID
        ///// </summary>
        ///// <returns></returns>
        //private int GetMaxNoRunID()
        //{
        //    int maxId = RedisCacheHelper.Get<int>(NoRunID);
        //    if (maxId == 0) 
        //    {
        //        DateTime dateTime = DateTime.Now.AddDays(-1);
        //        maxId = GetMax(u => u.ID,u=>u.OrderTime>= dateTime && 
        //        u.SendState.Length==0 && u.RunState.Length==0);
        //        RedisCacheHelper.Add(NoRunID,maxId,DateTime.Now.AddDays(1));
        //    }
        //    return maxId;
        //}

        //public List<AGVMissionInfo_Floor> GetNoRunMission()
        //{
        //    //SqlParameter sqlParameter = new SqlParameter();
        //    //sqlParameter.ParameterName = "ID";
        //    //sqlParameter.Value = GetMaxNoRunID();
        //    int ID= GetMaxNoRunID();
        //    return GetList(u => u.ID > ID, true);
        //    //return Query<AGVMissionInfo>(NoRunSql,
        //    //    new List<SqlParameter>() { sqlParameter },DbMainSlave.Master);
        //}

        //public List<AGVMissionInfo_Floor> GetList(Expression<Func<AGVMissionInfo_Floor, bool>> whereLambda,
        //    bool isNoTracking = false, DbMainSlave dms = DbMainSlave.Slave
        //    , Expression<Func<AGVMissionInfo_Floor, string>> ordering = null)
        //{
        //    return GetList(whereLambda, isNoTracking, dms,ordering);
        //}

        public IQueryable<AGVMissionInfo_Floor> GetQuery(Expression<Func<AGVMissionInfo_Floor, bool>> whereLambda,
           bool isNoTracking = false, DbMainSlave dms = DbMainSlave.Slave
           , Expression<Func<AGVMissionInfo_Floor, string>> ordering = null)
        {
            return GetIQueryable(whereLambda, isNoTracking, dms,ordering);
        }

        public List<AGVMissionInfo_Floor> GetTsjList(string tsjName)
        {
            return GetList(u=>u.EndPosition== tsjName || u.StartLocation== tsjName,
                true, DbMainSlave.Master);
        }

        public AGVMissionInfo_Floor GetByID(int ID, DbMainSlave dms = DbMainSlave.Slave)
        {
            return FindById(ID, dms);
        }


        #endregion

        #region 增加
        DataTable dt = null;
        public void Add(AGVMissionInfo_Floor agvMissionInfo)
        {
            if (dt == null)
                dt = ClassToDataTable(typeof(AGVMissionInfo_Floor));
            else
                dt.Clear();
            dt = ParseInDataTable(dt, agvMissionInfo);
            SetDataTableToTable(dt, tableName);
        }

        public void AddMany(DataTable dtable)
        {
            SetDataTableToTable(dtable, tableName);
        }
        #endregion

        #region 修改
        

        //public void Update(AGVMissionInfo_Floor agvMissionInfo)
        //{
        //    if (dt == null)
        //        dt = ClassToDataTable(typeof(AGVMissionInfo_Floor));
        //    else
        //        dt.Clear();
        //    dt = ParseInDataTable(dt, agvMissionInfo);
        //    BatchUpdateData(dt, tableName);
        //}

        public bool UpdateMany(DataTable dtable)
        {
            return BatchUpdateData(dtable, tableName);
        }
        #endregion

        #region 删除
        //public void Delete(AGVMissionInfo_Floor agvMissionInfo)
        //{
        //    if (dt == null)
        //        dt = ClassToDataTable(typeof(AGVMissionInfo_Floor));
        //    else
        //        dt.Clear();
        //    dt = ParseInDataTable(dt, agvMissionInfo);
        //    BatchUpdateData(dt, tableName);
        //}
        #endregion

        #region 其他
        public DataTable ConvertToTable()
        {
            return ClassToDataTable(typeof(AGVMissionInfo_Floor));
        }

        public DataTable AddToDataTable(DataTable dt, AGVMissionInfo_Floor agvMissionInfo)
        {
            return ParseInDataTable(dt, agvMissionInfo);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GeLiPage_WMS
{
    public class DBHelper
    {
        public static readonly string connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        #region ExecuteNonQuery命令
        /// <summary>  
        /// 对数据库执行增、删、改命令  
        /// </summary>  
        /// <param name="safeSql">T-Sql语句</param>  
        /// <returns>受影响的记录数</returns>  
        public static int ExecuteNonQuery(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                SqlTransaction trans = Connection.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(safeSql, Connection);
                    cmd.Transaction = trans;

                    if (Connection.State != ConnectionState.Open)
                    {
                        Connection.Open();
                    }
                    int result = cmd.ExecuteNonQuery();
                    trans.Commit();
                    return result;
                }
                catch
                {
                    trans.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>  
        /// 对数据库执行增、删、改命令  
        /// </summary>  
        /// <param name="sql">T-Sql语句</param>  
        /// <param name="values">参数数组</param>  
        /// <returns>受影响的记录数</returns>  
        public static int ExecuteNonQuery(string sql, SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                SqlTransaction trans = Connection.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, Connection);
                    cmd.Transaction = trans;
                    cmd.Parameters.AddRange(values);
                    if (Connection.State != ConnectionState.Open)
                    {
                        Connection.Open();
                    }
                    int result = cmd.ExecuteNonQuery();
                    trans.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return 0;
                }
            }
        }
        #endregion

        #region ExecuteScalar命令
        /// <summary>  
        /// 查询结果集中第一行第一列的值  
        /// </summary>  
        /// <param name="safeSql">T-Sql语句</param>  
        /// <returns>第一行第一列的值</returns>  
        public static int ExecuteScalar(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }

        /// <summary>  
        /// 查询结果集中第一行第一列的值  
        /// </summary>  
        /// <param name="sql">T-Sql语句</param>  
        /// <param name="values">参数数组</param>  
        /// <returns>第一行第一列的值</returns>  
        public static int ExecuteScalar(string sql, SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.Parameters.AddRange(values);
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }
        #endregion

        #region ExecuteReader命令
        /// <summary>  
        /// 创建数据读取器  
        /// </summary>  
        /// <param name="safeSql">T-Sql语句</param>  
        /// <param name="Connection">数据库连接</param>  
        /// <returns>数据读取器对象</returns>  
        public static SqlDataReader ExecuteReader(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
        }

        /// <summary>  
        /// 创建数据读取器  
        /// </summary>  
        /// <param name="sql">T-Sql语句</param>  
        /// <param name="values">参数数组</param>  
        /// <param name="Connection">数据库连接</param>  
        /// <returns>数据读取器</returns>  
        public static SqlDataReader ExecuteReader(string sql, SqlParameter[] values, SqlConnection Connection)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.Parameters.AddRange(values);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        #endregion

        #region ExecuteDataTable命令
        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataTable  
        /// </summary>  
        /// <param name="type">命令类型(T-Sql语句或者存储过程)</param>  
        /// <param name="safeSql">T-Sql语句或者存储过程的名称</param>  
        /// <param name="values">参数数组</param>  
        /// <returns>结果集DataTable</returns>  
        public static DataTable ExecuteDataTable(CommandType type, string safeSql, params SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                cmd.CommandType = type;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds.Tables[0];
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataTable  
        /// </summary>  
        /// <param name="safeSql">T-Sql语句</param>  
        /// <returns>结果集DataTable</returns>  
        public static DataTable ExecuteDataTable(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex)
                {

                }
                return ds.Tables[0];
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataTable  
        /// </summary>  
        /// <param name="sql">T-Sql语句</param>  
        /// <param name="values">参数数组</param>  
        /// <returns>结果集DataTable</returns>  
        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.CommandTimeout = 0;
                cmd.Parameters.AddRange(values);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds.Tables[0];
            }
        }
        #endregion

        #region GetDataSet命令
        /// <summary>  
        /// 取出数据  
        /// </summary>  
        /// <param name="safeSql">sql语句</param>  
        /// <param name="tabName">DataTable别名</param>  
        /// <param name="values"></param>  
        /// <returns></returns>  
        public static DataSet GetDataSet(string safeSql, string tabName, params SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);

                if (values != null)
                    cmd.Parameters.AddRange(values);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(ds, tabName);
                }
                catch (Exception ex)
                {

                }
                return ds;
            }
        }
        #endregion

        #region ExecureData 命令
        /// <summary>  
        /// 批量修改数据  
        /// </summary>  
        /// <param name="ds">修改过的DataSet</param>  
        /// <param name="strTblName">表名</param>  
        /// <returns></returns>  
        public static int ExecureData(DataSet ds, string strTblName)
        {
            try
            {
                //创建一个数据库连接  
                using (SqlConnection Connection = new SqlConnection(connectionString))
                {
                    if (Connection.State != ConnectionState.Open)
                        Connection.Open();

                    //创建一个用于填充DataSet的对象  
                    SqlCommand myCommand = new SqlCommand("SELECT * FROM " + strTblName, Connection);
                    SqlDataAdapter myAdapter = new SqlDataAdapter();
                    //获取SQL语句，用于在数据库中选择记录  
                    myAdapter.SelectCommand = myCommand;

                    //自动生成单表命令，用于将对DataSet所做的更改与数据库更改相对应  
                    SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(myAdapter);

                    return myAdapter.Update(ds, strTblName);  //更新ds数据  
                }

            }
            catch (Exception err)
            {
                throw err;
            }
        }

        #endregion

        #region 执行存储过程
        /// <summary>  
        ///  
        /// </summary>  
        /// <returns></returns>  
        public static DataTable ExecuteStoredProcedure(string storedName, params SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                SqlCommand cmd = new SqlCommand(storedName, Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                try
                {
                    da.Fill(dt);
                }
                catch
                {

                }
                return dt;
            }
        }
        #endregion

        #region 一次性把DataTable的数据插入到数据库
        /// <summary>  
        /// 
        /// </summary>   
        /// <param name="source">数据源</param>  
        /// <returns> true-成功，false-失败</returns> 
        /// 
        public static bool AddDataTableToDB(DataTable dt)
        {
                using (SqlConnection Connection = new SqlConnection(connectionString))
                {
                Connection.Open();
                    using (SqlBulkCopy copy = new SqlBulkCopy(Connection))
                    {
                    try
                    {
                        copy.DestinationTableName = "DR_Productinfor";  //指定服务器上目标表的名称 
                        copy.BatchSize = dt.Rows.Count;
                        copy.ColumnMappings.Add("Spec", "Spec");//第一个是dt的字段名，第二个是数据库的
                        copy.ColumnMappings.Add("Quantity", "Quantity");
                        copy.ColumnMappings.Add("Banhou", "Banhou");
                        copy.ColumnMappings.Add("RetQuantity", "RetQuantity");
                        copy.ColumnMappings.Add("Differ", "Differ");
                        copy.ColumnMappings.Add("ProjectNo", "ProjectNo");
                        copy.ColumnMappings.Add("Remark", "Remark");
                        copy.WriteToServer(dt);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                    }
                }
        }


        #endregion
    }
}
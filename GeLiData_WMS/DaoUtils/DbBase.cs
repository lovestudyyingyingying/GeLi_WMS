using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using GeLiData_WMS.Extensions;


namespace GeLiData_WMSUtils
{
    public class DbBase
    {
        //是否读写分离(可以配置在配置文件中)
        private static readonly bool IsReadWriteSeparation = true;
        //RedisHelper stringCacheRedisHelper = new RedisHelper();
        #region EF上下文对象(主库)

        protected DbContext MasterDb => _masterDb.Value;
        private readonly Lazy<DbContext> _masterDb = new Lazy<DbContext>(() => new DbContextFactory().GetWriteDbContext());

        #endregion EF上下文对象(主库)

        #region EF上下文对象(从库)

        protected DbContext SlaveDb => IsReadWriteSeparation ? _slaveDb.Value : _masterDb.Value;
        private readonly Lazy<DbContext> _slaveDb = new Lazy<DbContext>(() => new DbContextFactory().GetReadDbContext());

        #endregion EF上下文对象(从库)




        #region 自定义其他方法
        public void Init()
        {
            Query<string>("select '1'", new List<SqlParameter>(0), DbMainSlave.Master);
            Query<string>("select '1'", new List<SqlParameter>(0));
        }

        /// <summary>
        /// 执行存储过程或自定义sql语句--返回集合(自定义返回类型)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public List<TModel> Query<TModel>(string sql, List<SqlParameter> parms, DbMainSlave dms = DbMainSlave.Slave, CommandType cmdType = CommandType.Text)
        {
            //存储过程（exec getActionUrlId @name,@ID）
            if (cmdType == CommandType.StoredProcedure)
            {
                StringBuilder paraNames = new StringBuilder();
                foreach (var sqlPara in parms)
                {
                    paraNames.Append($" @{sqlPara},");
                }
                sql = paraNames.Length > 0 ? $"exec {sql} {paraNames.ToString().Trim(',')}" : $"exec {sql} ";
            }
            var list = (dms == DbMainSlave.Slave ? SlaveDb : MasterDb)
                .Database.SqlQuery<TModel>(sql, parms.ToArray());
            var enityList = list.ToList();
            return enityList;
        }
        /// <summary>
        /// 执行存储过程或自定义sql语句--返回集合(自定义返回类型)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public TModel QueryOne<TModel>(string sql, List<SqlParameter> parms, DbMainSlave dms = DbMainSlave.Slave, CommandType cmdType = CommandType.Text)
        {
            //存储过程（exec getActionUrlId @name,@ID）
            if (cmdType == CommandType.StoredProcedure)
            {
                StringBuilder paraNames = new StringBuilder();
                foreach (var sqlPara in parms)
                {
                    paraNames.Append($" @{sqlPara},");
                }
                sql = paraNames.Length > 0 ? $"exec {sql} {paraNames.ToString().Trim(',')}" : $"exec {sql} ";
            }
            var list = (dms == DbMainSlave.Slave ? SlaveDb : MasterDb)
                .Database.SqlQuery<TModel>(sql, parms.ToArray());
            var enityList = list.FirstOrDefault();
            return enityList;
        }
        /// <summary>
        /// 自定义语句和存储过程的增删改--返回影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public int Execute(string sql, List<SqlParameter> parms, CommandType cmdType = CommandType.Text)
        {
            //存储过程（exec getActionUrlId @name,@ID）
            if (cmdType == CommandType.StoredProcedure)
            {
                StringBuilder paraNames = new StringBuilder();
                foreach (var sqlPara in parms)
                {
                    paraNames.Append($" @{sqlPara},");
                }
                sql = paraNames.Length > 0 ?
                    $"exec {sql} {paraNames.ToString().Trim(',')}" :
                    $"exec {sql} ";
            }
            int ret = MasterDb.Database.ExecuteSqlCommand(sql, parms.ToArray());
            return ret;
        }

        /// <summary>
        /// 将对象数据插入DataTable中
        /// </summary>
        /// <param name="dt">原DataTable</param>
        /// <param name="obj">实例</param>
        /// <returns>新的DataTable</returns>
        public DataTable ParseInDataTable<T>(DataTable dt, T obj)
        {
            DataRow dr = dt.NewRow();
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(T));
            PropertyDescriptor pd2 = null;
            foreach (DataColumn dc in dt.Columns)
            {
                pd2 = pdc.Find(dc.ColumnName, true);

                if (pd2 != null && pd2.GetValue(obj) != null)
                    dr[dc.ColumnName] = pd2.GetValue(obj);
                else
                    dr[dc.ColumnName] = DBNull.Value;
            }
            dt.Rows.Add(dr);
            return dt;
        }
        /// <summary>
        /// 将类转换成空的dataTable
        /// </summary>
        /// <param name="t">类</param>
        /// <returns>dataTable</returns>
        public DataTable ClassToDataTable(Type t)
        {
            DataTable dataTable = new DataTable();
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(t))
            {
                Type proType = pd.PropertyType.Name == "Nullable`1" ? pd.PropertyType.GenericTypeArguments[0] : pd.PropertyType;

                //ICollection
                if (!proType.FullName.Contains("Collection") && proType.FullName.StartsWith("System"))
                {
                    if (pd.Name == "AGVMissionInfo_Floor")
                        Debug.WriteLine("");
                    //Debug.WriteLine(pd.PropertyType.GenericTypeArguments[0]);
                    if (pd.Name.StartsWith("_"))
                        dataTable.Columns.Add(pd.Name.Substring(1, pd.Name.Length - 1), proType);
                    else
                        dataTable.Columns.Add(pd.Name, proType);
                }
            }
            return dataTable;
        }

        /// <summary>
        /// 将实例对象的数据存入新的实例中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="type2"></param>
        /// <returns></returns>
        public object ParseValue<T>(T obj, Type type2, bool isICollection = true)
        {
            object retObj = type2.Assembly.CreateInstance(type2.FullName);
            Type type = typeof(T);

            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(type2);
            PropertyDescriptor pd2 = null;
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(type))
            {
                Type proType = pd.PropertyType.Name == "Nullable`1" ? pd.PropertyType.GenericTypeArguments[0] : pd.PropertyType;
                //如果isICollection为false则判断是否包含
                bool ret = (!isICollection ?
                    (!proType.FullName.Contains("Collection") && proType.FullName.StartsWith("System")) : isICollection);
                if (pdc.Contains(pd) && ret)
                {
                    if (pd.PropertyType.Name.Contains("DeliveryDate"))
                        Debug.WriteLine(pd.GetValue(obj));
                    object value = pd.GetValue(obj);
                    pd2 = pdc.Find(pd.Name, true);
                    if (pd2 != null)
                        pd2.SetValue(retObj, value);
                    pd2 = null;
                }
            }
            return retObj;
        }

        /// <summary>
        /// 将类转换成空的dataTable
        /// </summary>
        /// <param name="t">类</param>
        /// <returns>dataTable</returns>
        public async Task<DataTable> ClassToDataTableAsync(Type t)
        {
            DataTable dataTable = new DataTable();
            await Task.Factory.StartNew(() =>
            {
                foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(t))
                {
                    Type proType = pd.PropertyType.Name == "Nullable`1" ? pd.PropertyType.GenericTypeArguments[0] : pd.PropertyType;
                    //ICollection
                    if (!proType.FullName.Contains("Collection") && proType.FullName.StartsWith("System"))
                    {
                        //Debug.WriteLine(pd.PropertyType.GenericTypeArguments[0]);
                        if (pd.Name.StartsWith("_"))
                            dataTable.Columns.Add(pd.Name.Substring(1, pd.Name.Length - 1), proType);
                        else
                            dataTable.Columns.Add(pd.Name, proType);
                    }
                }
            });
            return dataTable;
        }

        /// <summary>
        /// Linq结果转DataTable 
        /// </summary>
        /// <typeparam name = "T" ></ typeparam >
        /// < param name="enumerable">Linq结果</param>
        /// <returns>DataTable</returns>
        public DataTable ConvertToDataTable<T>(IEnumerable<T> enumerable)
        {
            try
            {
                var dataTable = new DataTable();
                if (string.Empty.GetType() != typeof(T))
                {
                    foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(typeof(T)))
                    {
                        Type proType = pd.PropertyType.Name == "Nullable`1" ? pd.PropertyType.GenericTypeArguments[0] : pd.PropertyType;
                        if (!proType.FullName.Contains("Collection") && proType.FullName.StartsWith("System"))
                        {
                            //Debug.WriteLine(pd.PropertyType.GenericTypeArguments[0]);
                            if (pd.Name.StartsWith("_"))
                                dataTable.Columns.Add(pd.Name.Substring(1, pd.Name.Length - 1), proType);
                            else
                                dataTable.Columns.Add(pd.Name, proType);
                        }
                        //Debug.WriteLine(pd.Name);
                        //dataTable.Columns.Add(pd.Name);
                    }
                    foreach (T item in enumerable)
                    {
                        var Row = dataTable.NewRow();

                        foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(typeof(T)))
                        {
                            if (dataTable.Columns.Contains(pd.Name))
                            {
                                if (pd.GetValue(item) == null)
                                    Row[pd.Name] = DBNull.Value;
                                else
                                    Row[pd.Name] = pd.GetValue(item);
                            }

                        }
                        dataTable.Rows.Add(Row);
                    }
                }
                else
                {
                    dataTable.Columns.Add("column1", string.Empty.GetType());
                    foreach (T item in enumerable)
                    {
                        var Row = dataTable.NewRow();
                        Row[0] = item;
                        dataTable.Rows.Add(Row);
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }


        }

        /// <summary>
        /// 用法：using（）{ try { .Commit()}catch(.Rollback();)}
        /// </summary>
        /// <returns></returns>
        public DbContextTransaction GetTransaction()
        {
            return MasterDb.Database.BeginTransaction();
        }

        /// <summary>
        /// 用法：using（）{ try { .Commit()}catch(.Rollback();)}
        /// </summary>
        /// <returns></returns>
        public void CloseTransaction()
        {
            MasterDb.Database.CurrentTransaction.Dispose();
        }

        #endregion 自定义其他方法

        #region DataTable 批量操作数据库
        /// <summary>
        /// 批量插入（优先推荐） SqlBulkCopy
        /// </summary>
        /// <param name="source"></param>
        /// <param name="tableName"></param>
        /// <param name="isAllEqual">是否datatable中的列名与数据库列名都一一对应</param>
        /// <param name="dic">datatable中的列名，数据库列名</param>
        /// <returns></returns>
        /// bool isAllEqual=true,
        public bool SetDataTableToTable(DataTable source, string tableName, Dictionary<string, string> dic = null)
        {
            //DateTime time1 = DateTime.Now;
            SqlTransaction tran = null;//声明一个事务对象  
            try
            {
                using (SqlConnection conn = new SqlConnection(MasterDb.Database.Connection.ConnectionString))
                {
                    conn.Open();//打开链接  
                    using (tran = conn.BeginTransaction())
                    {
                        using (SqlBulkCopy copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran))
                        {
                            copy.DestinationTableName =
                                tableName.Contains("dbo.") ? tableName :$"dbo.{tableName}";           //指定服务器上目标表的名称
                                                                             //rt.WriteLog(source.Rows.Count.ToString());

                            #region 进行字段映射
                            if (dic == null || dic.Count() == 0)
                            {
                                foreach (DataColumn temp in source.Columns)
                                {
                                    copy.ColumnMappings.Add(temp.ColumnName, temp.ColumnName);
                                    Debug.WriteLine(temp.ColumnName);
                                }
                            }
                            else
                            {
                                //从dic中获取datatabel的列名与数据库字段名
                                foreach (KeyValuePair<string, string> temp in dic)
                                    copy.ColumnMappings.Add(temp.Key, temp.Value);
                            }

                            #endregion
                            copy.BulkCopyTimeout = 600;
                            copy.WriteToServer(source); //执行把DataTable中的数据写入DB  
                            tran.Commit(); //提交事务  
                            conn.Close();//关闭链接 

                            //rt.WriteLog("插入数据库时间：" + rt.ReckonSeconds(time1, time2));
                            return true;                                        //返回True 执行成功！  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                if (null != tran)
                    tran.Rollback();
                return false;//返回False 执行失败！  
            }
        }

        public Task<bool> SetToTableAsync(DataTable source, string tableName, Dictionary<string, string> dic = null)
        {
            return Task.Run<bool>(() =>
            {
                return SetDataTableToTable(source, tableName, dic);
            });
        }
        /// <summary>
        /// 批量更新数据（推荐）适用于多个数据不同修改列的修改方式
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableName"></param>
        /// <param name="BatchSize">每次往返处理的行数</param>
        /// <returns></returns>
        public bool BatchUpdateData(DataTable table, string tableName, int BatchSize = 5000)
        {
            table.TableName = tableName;
            DataSet ds = new DataSet();
            ds.Tables.Add(table.Copy());
            string _tableName = tableName;
            int result = 0;
            using (SqlConnection sqlconn = new SqlConnection(MasterDb.Database.Connection.ConnectionString))
            {
                sqlconn.Open();
                bool ret = false;
                //使用加强读写锁事务   
                SqlTransaction tran = sqlconn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {

                    ds.Tables[0].AcceptChanges();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        //所有行设为修改状态   
                        dr.SetModified();
                    }
                    //为Adapter定位目标表   

                    SqlCommand cmd = new SqlCommand(string.Format("select * from {0} where {1}", _tableName, " 1=2"), sqlconn, tran);
                    //cmd.Transaction = tran;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    SqlCommandBuilder sqlCmdBuilder = new SqlCommandBuilder(da);
                    sqlCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    da.AcceptChangesDuringUpdate = false;
                    StringBuilder columnsUpdateSqlBuilder = new StringBuilder();
                    //string columnsUpdateSql =string.Empty;
                    SqlParameter[] paras = new SqlParameter[table.Columns.Count];
                    //int parasIndex = 0;
                    //需要更新的列设置参数是,参数名为"@+列名"
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        //此处拼接要更新的列名及其参数值
                        if (i > 0)
                            columnsUpdateSqlBuilder.Append("[" + table.Columns[i].ColumnName + "]" + "=@" + table.Columns[i].ColumnName + ",");
                        if (table.Columns[i].DataType.Name == "DateTime")
                        {
                            paras[i] = new SqlParameter("@" + table.Columns[i].ColumnName, SqlDbType.DateTime, 23, table.Columns[i].ColumnName);
                        }
                        else if (table.Columns[i].DataType.Name == "Int64")
                        {
                            paras[i] = new SqlParameter("@" + table.Columns[i].ColumnName, SqlDbType.NVarChar, 19, table.Columns[i].ColumnName);
                        }
                        else
                        {
                            paras[i] = new SqlParameter("@" + table.Columns[i].ColumnName, SqlDbType.NVarChar, 2000, table.Columns[i].ColumnName);
                        }
                    }
                    if (columnsUpdateSqlBuilder.Length > 0)
                    {
                        //此处去掉拼接处最后一个","
                        columnsUpdateSqlBuilder = columnsUpdateSqlBuilder.Remove(columnsUpdateSqlBuilder.Length - 1, 1);
                    }
                    //此处生成where条件语句
                    string limitSql = ("[" + table.Columns[0].ColumnName + "]" + "=@" + table.Columns[0].ColumnName);
                    SqlCommand updateCmd = new SqlCommand(string.Format(" UPDATE [{0}] SET {1} WHERE {2} ", _tableName, columnsUpdateSqlBuilder.ToString(), limitSql));
                    Debug.WriteLine(string.Format(" UPDATE [{0}] SET {1} WHERE {2} ", _tableName, columnsUpdateSqlBuilder.ToString(), limitSql));
                    //不修改源DataTable   
                    updateCmd.UpdatedRowSource = UpdateRowSource.None;
                    da.UpdateCommand = updateCmd;
                    da.UpdateCommand.Parameters.AddRange(paras);
                    da.UpdateCommand.Transaction = tran;
                    //da.UpdateCommand.Parameters.Add("@" + table.Columns[0].ColumnName, table.Columns[0].ColumnName);
                    //每次往返处理的行数
                    da.UpdateBatchSize = BatchSize;
                    result = da.Update(ds, _tableName);
                    ds.AcceptChanges();
                    tran.Commit();
                    ret = true;

                }
                catch (Exception ex)
                {
                    ret = false;
                    tran.Rollback();
                    Debug.WriteLine(ex.ToString());
                    throw ex;
                }
                finally
                {
                    sqlconn.Dispose();
                    sqlconn.Close();

                }
                return ret;

            }

        }//new DBHelpers().MultiUpdateData(temtable, "*", "infor");


        public bool BatchDeleteData(DataTable table, string tableName)
        {
            bool ret = false;
            if (table == null || table.Rows.Count == 0)
                return false;
            lock (this)//处理并发情况(分布式情况)
            {
                table.TableName = tableName;
                DataSet ds = new DataSet();
                ds.Tables.Add(table.Copy());
                string _tableName = tableName;
                Stopwatch watch = Stopwatch.StartNew();
                using (SqlConnection sqlconn = new SqlConnection(MasterDb.Database.Connection.ConnectionString))
                {
                    sqlconn.Open();
                    watch.Stop();
                    Debug.WriteLine($"开启连接耗时：{watch.ElapsedMilliseconds}");
                    //使用加强读写锁事务   
                    SqlTransaction tran = sqlconn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        SqlCommand cmd = new SqlCommand(string.Format("select * from {0} where {1}", _tableName, " 1=2"), sqlconn);
                        SqlDataAdapter myAdapter = new SqlDataAdapter(cmd);
                        SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(myAdapter);

                        table.AcceptChanges();
                        for (int i = table.Rows.Count - 1; i >= 0; i--)
                            table.Rows[i].Delete();
                        //myAdapter.InsertCommand = myCommandBuilder.GetInsertCommand();
                        //myAdapter.UpdateCommand = myCommandBuilder.GetUpdateCommand();

                        myAdapter.DeleteCommand = myCommandBuilder.GetDeleteCommand();

                        myAdapter.DeleteCommand.Transaction = tran;
                        myAdapter.Update(ds, _tableName);
                        ds.AcceptChanges();
                        tran.Commit();
                        ret = true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Debug.WriteLine(ex.ToString());
                        throw ex;
                    }
                    finally
                    {
                        tran.Dispose();
                        sqlconn.Dispose();
                        sqlconn.Close();
                    }
                }
            }
            return ret;
        }

        public bool BatchInsertOrUpdate(DataTable addDataTable, DataTable updateDataTable, string tableName, out string exp, int BatchSize = 5000)
        {
            exp = string.Empty;
            SqlTransaction tran = null;//声明一个事务对象  

            using (SqlConnection conn = new SqlConnection(MasterDb.Database.Connection.ConnectionString))
            {
                conn.Open();//打开链接  
                using (tran = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {

                        #region 批量新增
                        if (addDataTable != null && addDataTable.Rows.Count > 0)
                        {
                            using (SqlBulkCopy copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran))
                            {
                                copy.DestinationTableName = tableName;           //指定服务器上目标表的名称
                                                                                 //rt.WriteLog(source.Rows.Count.ToString());

                                #region 进行字段映射
                                foreach (DataColumn temp in addDataTable.Columns)
                                {
                                    copy.ColumnMappings.Add(temp.ColumnName, temp.ColumnName);
                                    //Debug.WriteLine(temp.ColumnName);
                                }

                                #endregion
                                copy.BulkCopyTimeout = 600;
                                copy.BatchSize = BatchSize;
                                copy.WriteToServer(addDataTable); //执行把DataTable中的数据写入DB
                                                                  //返回True 执行成功！  
                            }

                        }

                        #endregion 批量新增

                        if (updateDataTable != null && updateDataTable.Rows.Count > 0)
                        {


                            #region 批量修改
                            //批量插入
                            updateDataTable.TableName = tableName;
                            DataSet ds = new DataSet();
                            ds.Tables.Add(updateDataTable.Copy());
                            ds.Tables[0].AcceptChanges();
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                //所有行设为修改状态   
                                dr.SetModified();
                            }
                            //为Adapter定位目标表   

                            SqlCommand cmd = new SqlCommand(string.Format("select * from {0} where {1}", tableName, " 1=2"), conn, tran);
                            //cmd.Transaction = tran;
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            SqlCommandBuilder sqlCmdBuilder = new SqlCommandBuilder(da);
                            sqlCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                            da.AcceptChangesDuringUpdate = false;
                            StringBuilder columnsUpdateSqlBuilder = new StringBuilder();
                            //string columnsUpdateSql =string.Empty;
                            SqlParameter[] paras = new SqlParameter[updateDataTable.Columns.Count];
                            //int parasIndex = 0;
                            //需要更新的列设置参数是,参数名为"@+列名"
                            for (int i = 0; i < updateDataTable.Columns.Count; i++)
                            {
                                //此处拼接要更新的列名及其参数值
                                if (i > 0)
                                    columnsUpdateSqlBuilder.Append("[" + updateDataTable.Columns[i].ColumnName + "]" + "=@" + updateDataTable.Columns[i].ColumnName + ",");
                                if (updateDataTable.Columns[i].DataType.Name == "DateTime")
                                {
                                    paras[i] = new SqlParameter("@" + updateDataTable.Columns[i].ColumnName, SqlDbType.DateTime, 23, updateDataTable.Columns[i].ColumnName);
                                }
                                else if (updateDataTable.Columns[i].DataType.Name == "Int64")
                                {
                                    paras[i] = new SqlParameter("@" + updateDataTable.Columns[i].ColumnName, SqlDbType.NVarChar, 19, updateDataTable.Columns[i].ColumnName);
                                }
                                else
                                {
                                    paras[i] = new SqlParameter("@" + updateDataTable.Columns[i].ColumnName, SqlDbType.NVarChar, 2000, updateDataTable.Columns[i].ColumnName);
                                }
                            }
                            if (columnsUpdateSqlBuilder.Length > 0)
                            {
                                //此处去掉拼接处最后一个","
                                columnsUpdateSqlBuilder = columnsUpdateSqlBuilder.Remove(columnsUpdateSqlBuilder.Length - 1, 1);
                            }
                            //此处生成where条件语句
                            string limitSql = ("[" + updateDataTable.Columns[0].ColumnName + "]" + "=@" + updateDataTable.Columns[0].ColumnName);
                            SqlCommand updateCmd = new SqlCommand(string.Format(" UPDATE [{0}] SET {1} WHERE {2} ", updateDataTable, columnsUpdateSqlBuilder.ToString(), limitSql));
                            Debug.WriteLine(string.Format(" UPDATE [{0}] SET {1} WHERE {2} ", tableName, columnsUpdateSqlBuilder.ToString(), limitSql));
                            //不修改源DataTable   
                            updateCmd.UpdatedRowSource = UpdateRowSource.None;
                            da.UpdateCommand = updateCmd;
                            da.UpdateCommand.Parameters.AddRange(paras);
                            da.UpdateCommand.Transaction = tran;
                            //da.UpdateCommand.Parameters.Add("@" + table.Columns[0].ColumnName, table.Columns[0].ColumnName);
                            //每次往返处理的行数
                            da.UpdateBatchSize = BatchSize;
                            da.Update(ds, tableName);
                            ds.AcceptChanges();

                            #endregion 批量修改
                        }

                        tran.Commit(); //提交事务  
                        conn.Close();//关闭链接 

                        //rt.WriteLog("插入数据库时间：" + rt.ReckonSeconds(time1, time2));
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        exp = ex.ToString();
                        if (null != tran)
                            tran.Rollback();
                        return false;//返回False 执行失败！  
                    }
                }
            }

        }

        #endregion
    }

    /// <summary>
    /// mssql数据库 数据层 父类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbBase<T> : DbBase where T : class, new()
    {
        #region INSERT

        /// <summary>
        /// 新增 实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void Insert(T model)
        {
            MasterDb.Set<T>().Add(model);
        }

        /// <summary>
        /// 普通批量插入
        /// </summary>
        /// <param name="datas"></param>
        public void InsertRange(List<T> datas)
        {
            MasterDb.Set<T>().AddRange(datas);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="datas"></param>
        public void InsertBulk(IQueryable<T> query)
        {
            EFBulkInsert.BulkInsertExtension.BulkInsert(MasterDb, query);
        }

        public async Task<T> InsertAsync(T t)
        {
            //ReadWriteEnum.Read;
            //确定链接---主库
            MasterDb.Set<T>().Add(t);
            await MasterDb.SaveChangesAsync();//写在这里  就不需要单独commit  不写就需要 
            return t;
        }

        #endregion INSERT

        #region DELETE

        /// <summary>
        /// 根据模型删除
        /// </summary>
        /// <param name="model">包含要删除id的对象</param>
        /// <returns></returns>
        public void Delete(T model)
        {
            MasterDb.Set<T>().Attach(model);
            MasterDb.Set<T>().Remove(model);
        }

        /// <summary>
        /// 批量删除及提交(推荐)
        /// </summary>
        /// <param name="whereLambda"></param>
        public int Delete(Expression<Func<T, bool>> whereLambda)
        {

            int count = MasterDb.Set<T>().Where(whereLambda).Delete();

            return count;
        }

        public async Task<int> DeleteAsync(Expression<Func<T, bool>> whereLambda)
        {
            //Stopwatch watch = Stopwatch.StartNew();
            Task<int> count = MasterDb.Set<T>().Where(whereLambda).DeleteAsync();
            //watch.Stop();
            //Console.WriteLine($"{count}条数据耗时：{watch.ElapsedMilliseconds}");
            return await count;
        }

        #endregion DELETE

        #region UPDATE

        /// <summary>
        /// 单个对象指定列修改
        /// </summary>
        /// <param name="model">要修改的实体对象</param>
        /// <param name="proNames">要修改的 属性 名称</param>
        /// <param name="isProUpdate"></param>
        /// <returns></returns>
        public void Update(T model, List<string> proNames, bool isProUpdate = true)
        {
            //将 对象 添加到 EF中
            MasterDb.Set<T>().Attach(model);
            var setEntry = ((IObjectContextAdapter)MasterDb).ObjectContext.ObjectStateManager.GetObjectStateEntry(model);
            //指定列修改
            if (isProUpdate)
            {
                foreach (string proName in proNames)
                {
                    setEntry.SetModifiedProperty(proName);
                }
            }
            //忽略类修改
            else
            {
                Type t = typeof(T);
                List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
                foreach (var item in proInfos)
                {
                    string proName = item.Name;
                    if (proNames.Contains(proName))
                    {
                        continue;
                    }
                    setEntry.SetModifiedProperty(proName);
                }
            }
        }

        /// <summary>
        /// 单个对象修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void Update(T model)
        {
            DbEntityEntry entry = MasterDb.Entry<T>(model);
            MasterDb.Set<T>().Attach(model);
            entry.State = EntityState.Modified;
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public void UpdateAll(List<T> models)
        {
            Stopwatch watch = Stopwatch.StartNew();
            foreach (var model in models)
            {
                DbEntityEntry entry = MasterDb.Entry(model);
                entry.State = EntityState.Modified;
            }
            Debug.WriteLine($"{models.Count}条数据设置为修改耗时：{watch.ElapsedMilliseconds}");

            MasterDb.SaveChanges();
            Debug.WriteLine($"{models.Count}条数据更新完成耗时：{watch.ElapsedMilliseconds}");
        }


        //public void UpdateAllTest(List<T> models)
        //{
        //    int count = MasterDb.Set<T>().Where(whereLambda).Update(updateExpression);

        //}
        /// <summary>
        /// 批量统一修改
        /// </summary>
        /// <param name="model">要修改的类</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="modifiedProNames"></param>
        /// <returns></returns>
        public void UpdateMany(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames)
        {
            //查询要修改的数据
            List<T> listModifing = MasterDb.Set<T>().Where(whereLambda).ToList();
            Type t = typeof(T);
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            Dictionary<string, PropertyInfo> dictPros = new Dictionary<string, PropertyInfo>();
            proInfos.ForEach(p =>
            {
                if (modifiedProNames.Contains(p.Name))
                {
                    dictPros.Add(p.Name, p);
                }
            });
            if (dictPros.Count <= 0)
            {
                throw new Exception("指定修改的字段名称有误或为空");
            }
            foreach (var item in dictPros)
            {
                PropertyInfo proInfo = item.Value;

                //取出 要修改的值
                object newValue = proInfo.GetValue(model, null);

                //批量设置 要修改 对象的 属性
                foreach (T oModel in listModifing)
                {
                    //为 要修改的对象 的 要修改的属性 设置新的值
                    proInfo.SetValue(oModel, newValue, null);
                }
            }
        }

        /// <summary>
        /// 批量更新(推荐)
        /// </summary>
        /// <param name="whereLambda">筛选条件</param>
        /// <param name="updateExpression">更新字段与值 u => new T { A="B" }</param>
        public int UpdateByPlus(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateExpression, bool isOpenTran = false)
        {
            //Stopwatch watch = Stopwatch.StartNew();
            //var temp =MasterDb.Database.Connection.BeginTransaction();
            //temp.Dispose();
            MasterDb.Configuration.EnsureTransactionsForFunctionsAndCommands = isOpenTran;
            return MasterDb.Set<T>().Where(whereLambda).Update(updateExpression);
            //watch.Stop();
            //Console.WriteLine($"{count}条数据耗时：{watch.ElapsedMilliseconds}");
        }
        public async Task<int> UpdateByPlusAsync(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateExpression)
        {
            return await MasterDb.Set<T>().Where(whereLambda).UpdateAsync(updateExpression);
        }
        #endregion UPDATE

        #region SELECT

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindById(dynamic id, DbMainSlave dms = DbMainSlave.Slave)
        {
            return dms == DbMainSlave.Slave ? SlaveDb.Set<T>().Find(id) : MasterDb.Set<T>().Find(id);
        }

        /// <summary>
        /// 获取默认一条数据，没有则为NULL
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public T FirstOrDefault(Expression<Func<T, bool>> whereLambda = null
            , bool isNoTracking = false, DbMainSlave dms = DbMainSlave.Slave)
        {

            var iQueryable =
                (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>();

            if (whereLambda == null)
            {
                return isNoTracking ? iQueryable.AsNoTracking().FirstOrDefault() :
                     iQueryable.FirstOrDefault();
            }
            return isNoTracking ? iQueryable.AsNoTracking().FirstOrDefault(whereLambda) :
                     iQueryable.FirstOrDefault(whereLambda);
        }
        /// <summary>
        /// 异步获取默认一条数据，没有则为NULL
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> whereLambda = null
            , bool isNoTracking = false, DbMainSlave dms = DbMainSlave.Slave)
        {
            var iQueryable =
               (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>();
            if (whereLambda == null)
            {
                return isNoTracking ? iQueryable.AsNoTracking().FirstOrDefaultAsync() :
                     iQueryable.FirstOrDefaultAsync();
            }else
                return isNoTracking ? iQueryable.AsNoTracking().FirstOrDefaultAsync(whereLambda) :
                         iQueryable.FirstOrDefaultAsync(whereLambda);
        }

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll(bool isNoTracking = false, Expression<Func<T, string>> ordering = null)
        {
            return ordering == null
                ? (isNoTracking ? SlaveDb.Set<T>().AsNoTracking().ToList() : SlaveDb.Set<T>().ToList())
                : (isNoTracking ? SlaveDb.Set<T>().OrderBy(ordering).AsNoTracking().ToList()
                                : SlaveDb.Set<T>().OrderBy(ordering).ToList());
        }

        public IQueryable<T> GetAllQueryable(Expression<Func<T, string>> ordering = null)
        {
            return ordering == null
                ? SlaveDb.Set<T>().AsQueryable()
                : SlaveDb.Set<T>().OrderBy(ordering).AsQueryable();
        }

        /// <summary>
        /// 异步获取全部数据
        /// </summary>
        /// <returns></returns>
        public Task<List<T>> GetAllAsync(Expression<Func<T, string>> ordering)
        {
            return ordering == null
                ? SlaveDb.Set<T>().ToListAsync()
                : SlaveDb.Set<T>().OrderBy(ordering).ToListAsync();
        }

        /// <summary>
        /// 带条件查询获取数据
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <param name="ordering"></param>
        /// <returns></returns>
        public List<T> GetList(Expression<Func<T, bool>> whereLambda, bool isNoTracking = false,
            DbMainSlave dms = DbMainSlave.Slave, Expression<Func<T, string>> ordering = null)
        {
            var iQueryable = whereLambda == null ?
                (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>() :
               (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>().Where(whereLambda);

            return ordering == null
                ? (isNoTracking ? iQueryable.AsNoTracking().ToList() : iQueryable.ToList())
                : (isNoTracking ? iQueryable.OrderBy(ordering).AsNoTracking().ToList()
                                : iQueryable.OrderBy(ordering).ToList());
        }

        /// <summary>
        /// 带条件查询获取数据
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <param name="ordering"></param>
        /// <returns></returns>
        public Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereLambda, bool isNoTracking = false, DbMainSlave dms = DbMainSlave.Slave,
            Expression<Func<T, string>> ordering = null)
        {
            var iQueryable = (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>().Where(whereLambda);
            return ordering == null
                ? (isNoTracking ? iQueryable.AsNoTracking().ToListAsync() : iQueryable.ToListAsync())
                : (isNoTracking ? iQueryable.OrderBy(ordering).AsNoTracking().ToListAsync()
                                : iQueryable.OrderBy(ordering).ToListAsync());
        }

        /// <summary>
        /// 带条件查询获取数据
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public IQueryable<T> GetIQueryable(
            Expression<Func<T, bool>> whereLambda, bool isNoTracking = false,
            DbMainSlave dms = DbMainSlave.Slave, Expression<Func<T, string>> ordering = null)
        {
            var iQueryable = whereLambda == null ?
                 (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>() :
                (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>().Where(whereLambda);

            return ordering == null
                ? (isNoTracking ? iQueryable.AsNoTracking() : iQueryable)
                : (isNoTracking ? iQueryable.OrderBy(ordering).AsNoTracking()
                                : iQueryable.OrderBy(ordering));
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="whereLambd"></param>
        /// <returns></returns>
        public int GetCount(Expression<Func<T, bool>> whereLambda = null,
            bool isNoTracking = false, DbMainSlave dms = DbMainSlave.Slave)
        {
            var iQueryable = whereLambda == null ?
                 (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>() :
                (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>().Where(whereLambda);
            return isNoTracking ? iQueryable.AsNoTracking().Count() : iQueryable.Count();
            //whereLambda == null ? SlaveDb.Set<T>().Count() : SlaveDb.Set<T>().Where(whereLambda).Count();
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="whereLambd"></param>
        /// <returns></returns>
        public Task<int> GetCountAsync(Expression<Func<T, bool>> whereLambd = null)
        {
            return whereLambd == null ? SlaveDb.Set<T>().CountAsync() : SlaveDb.Set<T>().Where(whereLambd).CountAsync();
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="whereLambd"></param>
        /// <returns></returns>
        public int GetMax(Expression<Func<T, int>> column, Expression<Func<T, bool>> whereLambd = null)
        {
            return whereLambd == null ? SlaveDb.Set<T>().Max(column) : SlaveDb.Set<T>().Where(whereLambd).Max(column);
        }

        /// <summary>
        /// 判断对象是否存在
        /// </summary>
        /// <param name="whereLambd"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<T, bool>> whereLambda = null,
            bool isNoTracking = false, DbMainSlave dms = DbMainSlave.Slave)
        {
            var iQueryable = whereLambda == null ?
                 (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>() :
                (dms == DbMainSlave.Slave ? SlaveDb : MasterDb).Set<T>().Where(whereLambda);
            return isNoTracking ? iQueryable.AsNoTracking().Any(): iQueryable.Any();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="rows">总条数</param>
        /// <param name="orderBy">排序条件（一定要有）</param>
        /// <param name="whereLambda">查询添加（可有，可无）</param>
        /// <param name="isOrder">是否是Order排序</param>
        /// <returns></returns>
        public List<T> Page<TKey>(int pageIndex, int pageSize, out int rows, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> whereLambda = null, bool isOrder = true)
        {
            IQueryable<T> data = isOrder ?
                SlaveDb.Set<T>().OrderBy(orderBy) :
                SlaveDb.Set<T>().OrderByDescending(orderBy);

            if (whereLambda != null)
            {
                data = data.Where(whereLambda);
            }
            rows = data.Count();

            return data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="rows">总条数</param>
        /// <param name="ordering">排序条件（一定要有）</param>
        /// <param name="whereLambda">查询添加（可有，可无）</param>
        /// <returns></returns>
        public List<T> Page(int pageIndex, int pageSize, out int rows, Expression<Func<T, string>> ordering, Expression<Func<T, bool>> whereLambda = null)
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            var data = SlaveDb.Set<T>().OrderBy(ordering).AsQueryable();
            if (whereLambda != null)
            {
                data = data.Where(whereLambda);
            }
            rows = data.Count();
            return data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        ///// <summary>
        ///// 查询转换
        ///// </summary>
        ///// <typeparam name="TDto"></typeparam>
        ///// <param name="whereLambda">筛选条件</param>
        ///// <param name="selector">每个字段的转换明细，格式 u=>new {ID=...}</param>
        ///// <returns></returns>
        public List<TDto> SelectToList<TDto>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TDto>> selector)
        {
            var iQueryable = whereLambda == null ?
                 SlaveDb.Set<T>() :
                SlaveDb.Set<T>().Where(whereLambda);

            return iQueryable.AsQueryable()
                .Select<T, TDto>(selector).ToList<TDto>();
        }

        ///// <summary>
        ///// 查询转换
        ///// </summary>
        ///// <typeparam name="TDto"></typeparam>
        ///// <param name="whereLambda">筛选条件</param>
        ///// <param name="selector">每个字段的转换明细，格式 u=>new {ID=...}</param>
        ///// <returns></returns>
        public IQueryable<TDto> SelectToQuery<TDto>(Expression<Func<T, bool>> whereLambda,
            Expression<Func<T, TDto>> selector)
        {
            var iQueryable = whereLambda == null ?
                 SlaveDb.Set<T>() :
                SlaveDb.Set<T>().Where(whereLambda);

            return iQueryable.AsQueryable()
                .Select<T, TDto>(selector);
        }

        #endregion SELECT

        #region ORTHER

        public void InsertOrUpdate(T entity)
        {
            var type = entity.GetType();
            var property = type.GetProperty("ID");
            var propValue = Convert.ToInt64(property.GetValue(entity));
            DbEntityEntry entry = MasterDb.Entry<T>(entity);
            MasterDb.Set<T>().Attach(entity);
            entry.State = EntityState.Modified;
            entry.State = propValue > 0 ? EntityState.Modified : EntityState.Added;
            MasterDb.Set<T>().AddOrUpdateExtension(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="ids"></param>
        //public void InsertOrUpdate(List<T> addEntitys, Dictionary<Int64,T> dicEntity)
        //{
        //    
        //}

        /// <summary>
        /// 效率太低
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="ids"></param>
        public void InsertOrUpdate<TK>(List<T> entitys, TK[] ids)
        {
            var type = typeof(T);
            long[] intpcid = Array.ConvertAll(ids, s => Convert.ToInt64(s));

            var property = type.GetProperty("ID");

            for (int index = 0; index < entitys.Count; index++)
            //foreach(T temp in entitys)
            {
                //var propValue = (int)property.GetValue(temp);
                DbEntityEntry entry = MasterDb.Entry<T>(entitys[index]);
                MasterDb.Set<T>().Attach(entitys[index]);
                //entry.State = EntityState.Modified;
                entry.State = intpcid[index] > 0 ? EntityState.Modified : EntityState.Added;
            }
            MasterDb.Set<T>().AddOrUpdateExtension(entitys);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name = "TK" ></ typeparam >
        /// < param name="entitys"></param>
        /// <param name = "ids" ></ param >
        public void InsertOrUpdateList(List<T> entitys,
             Expression<Func<T, object>>[] expressions)
        {
            //Stopwatch sw = Stopwatch.StartNew();
            var q = MasterDb.Set<T>().ToList();
            List<T> deleteIndex = new List<T>(); 
            for (int index = 0; index < entitys.Count; index++)
            {
                var EXP = DbBaseExpand.True<T>();

                foreach (var propertyExp in expressions)
                {
                    var member = (MemberExpression)propertyExp.Body;
                    string propertyName = member.Member.Name;

                    var temp = entitys[index].GetPropertyValue(propertyName);

                    var addExp = propertyExp.SelectorExpToBool(temp);
                    EXP = EXP.And(addExp);
                }
                Func<T, bool> func = EXP.Compile();
                Predicate<T> pred = func.Invoke;
                //var query = q.AsQueryable();
                if (q.Exists(pred))
                {
                    T obj= q.Find(pred);
                    var result = ObjectExtension.CompareType(entitys[index], obj, u => u.Name.ToUpper() != "ID"&& u.Name.ToUpper() != "UPDATETIME");
                    if (!result)
                    {
                        obj.ChangeByObject(entitys[index]);
                        //obj = (T)ParseValue(entitys[index], typeof(T));
                        entitys[index] = obj;
                        DbEntityEntry entry = MasterDb.Entry(entitys[index]);
                        MasterDb.Set<T>().Attach(entitys[index]);
                        //MasterDb.Set<T>().SeedEnumValues(u=>)
                        entry.State = EntityState.Modified;
                    }else
                    {
                        deleteIndex.Add(entitys[index]);
                    }
                }
                else
                {
                    DbEntityEntry entry = MasterDb.Entry(entitys[index]);
                    MasterDb.Set<T>().Attach(entitys[index]);
                    entry.State = EntityState.Added;
                }
            }
            entitys.RemoveAll(u => deleteIndex.Contains(u));

            //sw.Stop();
            //Debug.WriteLine("1261:"+sw.ElapsedMilliseconds);
            //sw.Restart();
            MasterDb.Set<T>().AddOrUpdateExtension(entitys);
            //sw.Stop();
            //Debug.WriteLine("1266:"+sw.ElapsedMilliseconds);
        }

        public void InsertOrUpdateListByDataTable(List<T> entitys,
             Expression<Func<T, object>>[] expressions)
        {
            //Stopwatch sw = Stopwatch.StartNew();
            var q = MasterDb.Set<T>().ToList();
            List<T> deleteIndex = new List<T>();
            List<T> editList=new List<T>();
            List<T> addList = new List<T>();

            for (int index = 0; index < entitys.Count; index++)
            {
                var EXP = DbBaseExpand.True<T>();

                foreach (var propertyExp in expressions)
                {
                    var member = (MemberExpression)propertyExp.Body;
                    string propertyName = member.Member.Name;

                    var temp = entitys[index].GetPropertyValue(propertyName);

                    var addExp = propertyExp.SelectorExpToBool(temp);
                    EXP = EXP.And(addExp);
                }
                Func<T, bool> func = EXP.Compile();
                Predicate<T> pred = func.Invoke;
                //var query = q.AsQueryable();
                if (q.Exists(pred))
                {
                    T obj = q.Find(pred);
                    var result = ObjectExtension.CompareType(entitys[index], obj, u => u.Name.ToUpper() != "ID" && u.Name.ToUpper() != "UPDATETIME");
                    if (!result)
                    {
                        obj.ChangeByObject(entitys[index]);
                        //obj = (T)ParseValue(entitys[index], typeof(T));
                        entitys[index] = obj;
                        editList.Add(obj);
                        //DbEntityEntry entry = MasterDb.Entry(entitys[index]);
                        //MasterDb.Set<T>().Attach(entitys[index]);
                        ////MasterDb.Set<T>().SeedEnumValues(u=>)
                        //entry.State = EntityState.Modified;
                    }
                    else
                    {
                        deleteIndex.Add(entitys[index]);
                    }
                }
                else
                {
                    addList.Add(entitys[index]);
                    //DbEntityEntry entry = MasterDb.Entry(entitys[index]);
                    //MasterDb.Set<T>().Attach(entitys[index]);
                    //entry.State = EntityState.Added;
                }
            }
            entitys.RemoveAll(u => deleteIndex.Contains(u));
            DataTable editDt = ConvertToDataTable(editList);
            DataTable addDt = ConvertToDataTable(addList);
            string exp = string.Empty;
            BatchInsertOrUpdate(addDt, editDt, typeof(T).Name,out exp);
            //sw.Stop();
            //Debug.WriteLine("1261:"+sw.ElapsedMilliseconds);
            //sw.Restart();
            //MasterDb.Set<T>().AddOrUpdateExtension(entitys);
            //sw.Stop();
            //Debug.WriteLine("1266:"+sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// 执行存储过程或自定义sql语句--返回集合
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public List<T> Query(string sql, List<SqlParameter> parms, DbMainSlave dms = DbMainSlave.Slave, CommandType cmdType = CommandType.Text)
        {
            return Query<T>(sql, parms, dms, cmdType);
        }

        public List<T> ConvertList(List<T> list)
        {
            List<T> list2 = new List<T>(list.Count);
            foreach (T temp in list)
            {
                list2.Add((T)ParseValue(temp, typeof(T), false));
            }
            return list2;
        }


        /// <summary>
        /// 提交保存
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return MasterDb.SaveChanges();
        }
      
        public async Task<int> SaveChangesAsync()
        {
            return await MasterDb.SaveChangesAsync();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void RollBackChanges()
        {
            var items = MasterDb.ChangeTracker.Entries().ToList();
            items.ForEach(o => o.State = EntityState.Unchanged);
        }




        #endregion ORTHER

    }


    public enum DbMainSlave
    {
        Master,
        Slave
    }
}

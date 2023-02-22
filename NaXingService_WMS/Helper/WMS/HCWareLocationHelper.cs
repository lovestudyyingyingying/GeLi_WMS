using SQLHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Helper.WMS
{
    /// <summary>
    /// 缓存库位帮助类
    /// </summary>
    public class HCWareLocationHelper
    {

        public async Task<DataTable> In1(int count)
        {
            return await Task.Run(() => In(count));
        }
        #region 进缓存区2
        /// <summary>
        /// 获取缓存区进仓顺序
        /// </summary>
        /// <param name="count">需要获取的仓位数量</param>
        /// <returns></returns>
        public DataTable In(int count)
        {
            //1、得到空列与数量
            DataTable dt = GetNullLie();
            //int count2 = 0;
            List<string> lieList = new List<string>();
            List<string> wlList = new List<string>();
            //2、将所有空位进行分组按列排序

            DataTable newDt = dt.Clone();
            newDt.Clear();
            int maxCount = 0;
            Dictionary<string, DataRow[]> dic = SortInLieDic(dt, out maxCount);

            List<DataRow[]> dicValue = dic.Values.ToList();
            //先排序再分数量，然后再分批
            for (int i = 0; i < dicValue.Count; i++)
            {
                for (int j = 0; j < maxCount; j++)
                {
                    if (dicValue[i].Length > 0)
                    {
                        newDt.ImportRow(dicValue[i][0]);
                        dicValue[i] = Remove(dicValue[i], 0);
                    }
                }
            }

            //分数量
            DataTable newDt2 = dt.Clone();
            newDt2.Clear();
            for (int i = 0; i < count; i++)
            {
                newDt2.ImportRow(newDt.Rows[i]);
            }

            int carCount = 2;
            maxCount = 0;
            //分批
            Dictionary<string, DataRow[]> dic2 = SortInLieDic(newDt2, out maxCount);
            List<DataRow[]> dicValue2 = dic2.Values.ToList();

            DataTable newDt3 = dt.Clone();
            newDt3.Clear();
            //同时跑的列数
            int runCount = dicValue2.Count() < carCount ? dicValue2.Count() : carCount;
            Debug.WriteLine(runCount);

            for (int i = 0; i < dicValue2.Count() / runCount + 1; i++)
            {
                for (int k = 0; k < maxCount; k++)
                {
                    for (int j = 0; j < runCount; j++)
                    {
                        if (i * runCount + j < dicValue2.Count())
                        {
                            int index = runCount * i + j;
                            //Debug.WriteLine(index);
                            if (dicValue2[index].Count() > 0)
                            {
                                newDt3.ImportRow(dicValue2[index][0]);
                                dicValue2[index] = Remove(dicValue2[index], 0);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < count / runCount + 1; i++)
            {
                for (int j = 0; j < runCount; j++)
                {
                    //Debug.WriteLine(i + "::" + j);
                    if (j < count && dicValue2[j].Count() > 0)
                    {
                        newDt3.ImportRow(dicValue2[j][0]);
                        dicValue2[j] = Remove(dicValue2[j], 0);
                    }
                }
            }

            return newDt3;
        }
        /// <summary>
        /// 将datatable按列的位置进行分组排序
        /// </summary>
        /// <param name="query">groupby 后的集合</param>
        /// <param name="dt">查询到的空位datatable</param>
        /// <param name="mCount">每列最大值</param>
        /// <returns></returns>
        private Dictionary<string, DataRow[]> SortInLieDic(DataTable dt, out int mCount)
        {
            //得到每一列的进出仓排序方式
            var query = from t in dt.AsEnumerable()
                        group t by new { t1 = t.Field<string>("lie"), t2 = t.Field<string>("InstockRule") } into m
                        select new
                        {
                            lie = m.Key.t1,
                            InstockRule = m.Key.t2,
                            //score = m.Sum(n => n.Field<decimal>("score"))
                        };
            int maxCount = 0;
            Dictionary<string, DataRow[]> dic = new Dictionary<string, DataRow[]>();

            if (query.ToList().Count > 0)
            {
                query.ToList().ForEach(q =>
                {
                    DataRow[] drs = dt.Select(string.Format("lie='{0}'", q.lie));
                    if (drs.Count() > maxCount)
                        maxCount = drs.Count();

                    if (q.InstockRule == "优先使用小的仓位号")
                    {
                        drs = drs.OrderBy(x => x["xuhao"]).ToArray();
                    }
                    else
                    {
                        drs = drs.OrderByDescending(x => x["xuhao"]).ToArray();
                    }
                    dic.Add(q.lie, drs);
                });
            }
            mCount = maxCount;
            return dic;
        }

        //and WareHouse_ID=3
        string sql = string.Format(@"
               select LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1)) lie,
                Right(a.WareLocaNo,len(a.WareLocaNo)-charindex('-',a.WareLocaNo,charindex('-',a.WareLocaNo)+1)) xuhao,AGVPosition,b.InstockRule 
                from WareLocation a,WareArea b
                where a.WareArea_ID=b.id  and charindex('-',a.WareLocaNo)>0 and IsOpen>0 and
                b.War_ID=5 and  WareHouse_ID=1 and 
                not exists(
                select LEFT(d.WareLocaNo,charindex('-',d.WareLocaNo,charindex('-',d.WareLocaNo)+1))
                 from WareLocation d,TrayState c where c.WareLocation_ID= d.ID and 
                 LEFT(a.WareLocaNo,charindex('-',a.WareLocaNo,charindex('-',a.WareLocaNo)+1))=
                 LEFT(d.WareLocaNo,charindex('-',d.WareLocaNo,charindex('-',d.WareLocaNo)+1)) )
                and NOT exists( select lie from
	             (SELECT LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1)) lie,AGVPosition FROM WareLocation,
             (select  EndLocation  from AGVMissionInfo where SendState='成功' AND Mark='02' and 
             (len(RunState)=0 or (RunState!='已完成' and RunState!='已取消' and RunState!='执行失败'and RunState!='发送失败')))A
	         WHERE WareLocation.AGVPosition=A.EndLocation)F WHERE lie= LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1)))
                group by LEFT(WareLocaNo,charindex('-',a.WareLocaNo,charindex('-',a.WareLocaNo)+1)),b.InstockRule ,AGVPosition,
                Right(a.WareLocaNo,len(a.WareLocaNo)-charindex('-',a.WareLocaNo,charindex('-',a.WareLocaNo)+1))
                order by LEFT(LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1)),charindex('-',LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1))))
                ,CONVERT(INT,left(right(LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1)),len(LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1)))
                -charindex('-',LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1)))),charindex('-',right(LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1))
                ,len(LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1)))-charindex('-',LEFT(WareLocaNo,charindex('-',WareLocaNo,charindex('-',WareLocaNo)+1)))))-1))
            ");

        public HCWareLocationHelper()
        {
            DbHelperSQL.connectionString = ConfigurationManager.ConnectionStrings["MainConnectionString"].ToString();

        }

        /// <summary>
        /// 得到空的列与对应的数量
        /// </summary>
        /// <returns></returns>
        private DataTable GetNullLie()
        {
            DataTable dt = DbHelperSQL.ReturnDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 返回select后的 的数据
        /// </summary>
        /// <param name="dt">原datatable</param>
        /// <param name="drs">datatable select后的行数组</param>
        /// <returns></returns>
        private DataTable SelectDt(DataTable dt, DataRow[] drs)
        {
            DataTable t = dt.Clone();
            t.Clear();
            foreach (DataRow row in drs)
                t.ImportRow(row);
            return t;
        }

        /// <summary>
        /// 删除数组某个元素
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="index">元素序号</param>
        /// <returns>新数组</returns>
        private DataRow[] Remove(DataRow[] array, int index)
        {
            int length = array.Length;
            DataRow[] result = new DataRow[length - 1];
            try
            {
                Array.Copy(array, result, index);
                Array.Copy(array, index + 1, result, index, length - index - 1);
                return result;
            }
            catch
            {
                return result;
            }
        }

        #endregion
    }
}

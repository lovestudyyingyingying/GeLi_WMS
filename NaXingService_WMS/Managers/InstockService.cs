using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.InstockEntity;
using NanXingService_WMS.Utils;

namespace NanXingService_WMS.Services
{
    /// <summary>
    /// 入仓服务，获取仓位
    /// </summary>
    public class InstockService
    { 
        DbBase<WareLocation> wareLocationDao = new DbBase<WareLocation>();
        private string InstockRuleAsc = "优先使用小的仓位号";
        private string RedisStr = "WLState:";

        #region 添加
        //public void I

        #endregion

        #region 查询
        /// <summary>
        /// 获取楼层入库位数据
        /// </summary>
        /// <param name="positionID">楼层ID</param>
        /// <returns></returns>
        public List<WareLocation> GetInstockStart(int positionID)
        {
            List<WareLocation> list= wareLocationDao.GetList(
                u => u.WareArea.WareHouse.ID == positionID
                  && u.WareArea.WareAreaClass.AreaClass == "入库位"
                  && u.IsOpen == 1, true);
            List<WareLocation> list2 = wareLocationDao.ConvertList(list);
            return list2;
        }

        /// <summary>
        /// 获取到所有空列的数据
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        private List<UseableLie> GetLieState(string batchNo)
        {
            
            //获取所有列的状态、列产品
            //1、获取07栋所有成品区仓位
            IQueryable<WareLocation> all_Q = wareLocationDao.GetIQueryable (u=>
               u.IsOpen==1 && u.WareArea.WareHouse.WHName.StartsWith("07")
            && u.WareArea.WareAreaClass.AreaClass == "成品区",false, DbMainSlave.Master, null);
           
            //2、获取仓位所有列中的数量
            var Q_Line = from a in all_Q
                         group a by
                         new
                         {
                             a.WareLoca_Lie,
                             a.WareLocaState,
                             batchNo = a.TrayState.batchNo ?? "",
                             a.WareArea.WareHouse,
                             a.WareArea.InstockRule
                         }
                    into g
                         select new
                         {
                             g.Key,
                             Count = g.Count(),
                         };
            //3、获取存在空位的列，获取占用的列，空列left join 左关联查询
            var Q_LineState = from a in
                                  (from a in Q_Line
                                   where a.Key.WareLocaState == "空"
                                   select new
                                   {
                                       a.Key,
                                       a.Count
                                   })
                              join b in (from a in Q_Line
                                         where a.Key.WareLocaState == "占用"
                                         select new
                                         {
                                             a.Key,
                                             a.Count
                                         })
                                on a.Key.WareLoca_Lie equals b.Key.WareLoca_Lie
                                into temp
                              from tt in temp.DefaultIfEmpty()
                              where tt.Key.batchNo ==string.Empty ||
                              tt.Key.batchNo == batchNo
                              orderby a.Count ascending,
                              a.Key.WareHouse.WHName ascending,
                              tt.Key.batchNo.Length descending
                              select new UseableLie
                              {
                                  WareLocation_Lie=a.Key.WareLoca_Lie,//列名
                                  BatchNo = tt.Key.batchNo,//列批号
                                  NullWLCount = a.Count,//空位数量
                                  InstockRule=a.Key.InstockRule//入库排序方式
                              };
            return Q_LineState.ToList();
            //Debug.WriteLine(1);

        }

        /// <summary>
        /// 获取入库的位置
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public WareLocation GetInstockWL(string batchNo)
        {
            List<UseableLie> nullLie = GetLieState(batchNo);
           
            WareLocation instockWl = null;
            int wlID = 0;
            //stopwatch.Restart();
            foreach (UseableLie temp in nullLie)
            {
                string lieStr = RedisStr + temp.WareLocation_Lie;
                //获取列的状态
                if (RedisCacheHelper.Incr(lieStr, DateTime.Now.AddHours(1)) == 1)
                {
                    //等于1证明没有占用
                    List<WareLocation> list=wareLocationDao.GetList(u => u.WareLoca_Lie == temp.WareLocation_Lie
                    ,true, DbMainSlave.Master);
                    list = list.Where(u => u.WareLocaState == "空").ToList();
                    if (list.Count>0)
                    {
                        //获取排序规则
                        if (temp.InstockRule == InstockRuleAsc)
                            wlID = list.OrderBy(u => u.WareLocaNo).ToList()[0].ID;
                        else
                            wlID = list.OrderByDescending(u => u.WareLocaNo).ToList()[0].ID;
                        
                        wareLocationDao.UpdateByPlus(u => u.ID == wlID,
                            u => new WareLocation { WareLocaState = "预进" });

                        
                        RedisCacheHelper.Remove(lieStr);
                        
                        break;
                    }
                    //else
                    //    RedisCacheHelper.Remove(lieStr);
                }
                else
                {
                    RedisCacheHelper.Remove(lieStr);
                    continue;
                }
                //如果REDIS中没有列状态缓存，则先直接从数据库中判断列状态，

                //并另起线程更新列状态到REDIS
            }
            return instockWl;
        }

        #endregion


        #region 其他

        public List<WareLocation> ConverToList(List<WareLocation> list)
        {
            List<WareLocation> wlList = new List<WareLocation>();
            wlList = wareLocationDao.ConvertList(list);
            return wlList;
        }
        #endregion
    }
}

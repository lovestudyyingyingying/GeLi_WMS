using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.InstockEntity;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Services.WMS;
using NanXingService_WMS.Services.WMS.AGV;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Helper.WMS
{
    public class InstockHelper
    {
        WareLocationService _wareLocationService;
        WareLocationLockHisService _wareLocationLockHisService;
        public static string InstockRuleAsc = "优先使用小的仓位号";
        //private string RedisStr = "WLState:";
        RedisHelper stringCacheRedisHelper = new RedisHelper();
        public InstockHelper(WareLocationService wareLocationService
            , WareLocationLockHisService wareLocationLockHisService)
        {
            _wareLocationService = wareLocationService;
            _wareLocationLockHisService = wareLocationLockHisService;
        }

        #region 查询
        /// <summary>
        /// 获取楼层入库位数据
        /// </summary>
        /// <param name="positionID">楼层ID</param>
        /// <returns></returns>
        public List<WareLocation> GetInstockStart(int positionID)
        {
            List<WareLocation> list = _wareLocationService.GetList(
                u => u.WareArea.WareHouse.ID == positionID
                  && u.WareArea.WareAreaClass.AreaClass == AreaClassType.RuKuArea
                  && u.IsOpen == 1, true);
            List<WareLocation> list2 = _wareLocationService.ConvertList(list);
            return list2;
        }

        /// <summary>
        /// 获取到所有空列的数据
        /// </summary>
        /// <param name="batchNo">批号</param>
        /// <param name="proName">产品名称</param>
        /// <param name="position">仓库名称，为空则是07全部成品仓位</param>
        /// <returns>可用的列集合</returns>
        public List<UseableLie> GetLieState(string batchNo,string proName,string position)
        {
            //获取所有列的状态、列产品
            //1、获取07栋所有成品区仓位
            Expression<Func< WareLocation,bool>> exp = DbBaseExpand.True<WareLocation>();
            if (string.IsNullOrEmpty(position))
                exp = exp.And(u => u.WareArea.WareHouse.WHName.StartsWith("07"));
            else
                exp = exp.And(u => u.WareArea.WareHouse.WHName == position);

            exp = exp.And(u =>
               u.IsOpen == 1 && u.WareArea.WareAreaClass.AreaClass == AreaClassType.ChengPinArea);

            List<LieHasNullCount> all_Q = _wareLocationService.GetIQueryable(exp
                , true, DbMainSlave.Master).Select(a => new LieHasNullCount
                {
                    WareAreaName = a.WareArea.WareNo,
                    WareLoca_Lie = a.WareLoca_Lie,
                    WareLocaState = a.WareLocaState,
                    BatchNo = a.TrayState == null ? string.Empty : a.TrayState.batchNo ?? string.Empty,
                    ProName = a.TrayState == null ? string.Empty : a.TrayState.proname ?? string.Empty,
                    WareHouse = a.WareArea.WareHouse.WHName,
                    InstockRule = a.WareArea.InstockRule
                }).ToList();
            //List<LieHasNullCount> q = all_Q.ToList();
            //grouyby之后一列会有1-2行数据，一行是空批次，一行是有批次
            //获取到有批次的列信息
            var q_Line = all_Q.Where(u=> u.BatchNo.Length>0)
                .GroupBy(a => new
            {
                a.WareAreaName,
                a.WareLoca_Lie,
                a.WareLocaState,
                a.BatchNo,
                a.ProName,
                a.WareHouse,
                a.InstockRule
            }).Select(g => new LieHasNullCount
            {
                WareAreaName=g.Key.WareAreaName,
                WareLoca_Lie=g.Key.WareLoca_Lie,
                WareLocaState=g.Key.WareLocaState,
                BatchNo=g.Key.BatchNo,
                ProName=g.Key.ProName,
                WareHouse=g.Key.WareHouse,
                InstockRule=g.Key.InstockRule,
                Count = g.Count(),
            }).ToList();

            //2、获取仓位所有列中的数量
            var Q_Line2 = from a in all_Q
                         group a by
                         new
                         {
                             a.WareAreaName,
                             a.WareLoca_Lie,
                             a.WareLocaState,
                             a.BatchNo,
                             a.ProName,
                             a.WareHouse,
                             a.InstockRule
                         }
                        into g
                         select new 
                         {
                             WareAreaName=g.Key.WareAreaName,
                             WareLoca_Lie = g.Key.WareLoca_Lie,
                             WareLocaState = g.Key.WareLocaState,
                             BatchNo = g.Key.BatchNo,
                             ProName = g.Key.ProName,
                             WareHouse = g.Key.WareHouse,
                             InstockRule = g.Key.InstockRule,
                             Count = g.Count(),
                         };
            var q_Line2 = Q_Line2.ToList();

            var Q_Line = q_Line.AsEnumerable();


            //3、获取存在空位的列，获取占用的列，空列left join 左关联查询
            var Q_LineState = from a in
                                 (from a in Q_Line2
                                  where a.WareLocaState == WareLocaState.NoTray
                                  select new
                                  {
                                      WareAreaName = a.WareAreaName,
                                      WareLoca_Lie = a.WareLoca_Lie,
                                      WareLocaState = a.WareLocaState,
                                      BatchNo = a.BatchNo,
                                      ProName = a.ProName,
                                      WareHouse = a.WareHouse,
                                      InstockRule = a.InstockRule,
                                      Count = a.Count
                                  })
                                  //所有有货物的的列
                              join b in (from a in Q_Line
                                             //where a.WareLocaState == WareLocaState.HasTray
                                         select new
                                         {
                                             WareAreaName = a.WareAreaName,
                                             WareLoca_Lie = a.WareLoca_Lie,
                                             WareLocaState = a.WareLocaState,
                                             BatchNo = a.BatchNo,
                                             ProName = a.ProName,
                                             WareHouse = a.WareHouse,
                                             InstockRule = a.InstockRule,
                                             Count = a.Count
                                         })
                                //所有有占用的列
                                on a.WareLoca_Lie equals b.WareLoca_Lie
                                into temp
                              from tt in temp.DefaultIfEmpty()
                              where tt == null || tt.BatchNo == null ||
                                     (tt.BatchNo == batchNo && tt.ProName == proName)
                              select new UseableLie
                              {
                                  WHName = a.WareHouse,
                                  WareAreaName=a.WareAreaName,//区名
                                  WareLocation_Lie = a.WareLoca_Lie,//列名
                                  BatchNo = tt!=null?tt.BatchNo:string.Empty,//列批号
                                  NullWLCount = a.Count,//空位数量
                                  InstockRule = a.InstockRule//入库排序方式
                              };
            //orderby a.Count ascending,
            //                 a.WareHouse ascending,
            //                 tt.BatchNo.Length descending

            //排序要求：1、有批号的在前，空仓位少的在前
            //          2、空批号的在后，列号小的在前
            //分开排序：1、有批号的排序+空批号的排序

            List<UseableLie> hasBatch = Q_LineState.Where(u => u.BatchNo.Length > 0).
                OrderBy(u => u.NullWLCount).ToList();
            List<UseableLie> noBatch = Q_LineState.Where(u => u.BatchNo.Length == 0).
                OrderBy(u => u.WHName).ThenBy(u => u.WareAreaName)
                .ThenBy(u => u.WareLocation_Lie.Length)
                .ThenBy(u=>u.WareLocation_Lie).ToList();
            List<UseableLie> returnList= hasBatch.Union(noBatch).ToList();

            return returnList;
            //Debug.WriteLine(1);

        }

        /// <summary>
        /// 获取入库的位置
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public WareLocation GetInstockWL(string batchNo,string proName,string position,
            string locker,bool updatePre=true)
        {
            //获取到包含该批次的列或完全空的列
            List<UseableLie> nullLie = GetLieState(batchNo,proName, position);

            WareLocation instockWl = null;
            int wlID = 0;
            //stopwatch.Restart();
            foreach (UseableLie temp in nullLie)
            {
                string lieStr = $"{WareLocationService.wareLocker}:{temp.WareLocation_Lie}";
                //获取列的状态
                //加锁判断该列是否被其他进程获取仓位
                using (var redislock = stringCacheRedisHelper.CreateLock(lieStr, TimeSpan.FromSeconds(10),
                    TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
                {
                    //等于1证明没有占用
                    List<WareLocation> list = _wareLocationService.GetList(
                        u => u.WareLoca_Lie == temp.WareLocation_Lie
                    && u.WareArea.WareAreaClass.AreaClass == AreaClassType.ChengPinArea
                    , true, DbMainSlave.Master);
                    if(list.Where(u=>u.WareLocaState== WareLocaState.PreOut).Count() > 0)
                    {
                        continue;
                    }
                    else 
                    {
                        List<WareLocation> preInList =
                            list.Where(u => u.WareLocaState == WareLocaState.PreIn).ToList();
                        if(preInList.Count() > 0 &&
                            preInList.Where(u => u.BatchNo == batchNo).Count() == 0)
                        {
                            continue;
                        }
                    }
                    list = list.Where(u => u.WareLocaState == WareLocaState.NoTray).ToList();
                    if (list.Count > 0)
                    {
                        //获取排序规则
                        if (temp.InstockRule == InstockRuleAsc)
                        {
                            instockWl = list.OrderBy(u => u.WareLocaNo).ToList()[0];
                            wlID = instockWl.ID;
                        }
                        else
                        {
                            instockWl = list.OrderByDescending(u => u.WareLocaNo).ToList()[0];
                            wlID = instockWl.ID;
                        }
                        //预锁仓位
                        if (updatePre)
                        {
                            
                            WareLoactionLockHis wareLoactionLockHis = new WareLoactionLockHis();
                            wareLoactionLockHis.WareLocaNo = instockWl.WareLocaNo;
                            wareLoactionLockHis.PreState = WareLocaState.PreIn;
                            wareLoactionLockHis.LockTime = DateTime.Now;
                            wareLoactionLockHis.Locker = locker;
                            _wareLocationLockHisService.Insert(wareLoactionLockHis);
                            _wareLocationLockHisService.SaveChanges();

                            _wareLocationService.UpdateByPlus(u => u.ID == wlID,
                               u => new WareLocation
                               {
                                   WareLocaState = WareLocaState.PreIn,
                                   BatchNo = batchNo,
                                   LockHis_ID = wareLoactionLockHis.ID
                               }); 
                        }

                        //stringCacheRedisHelper.KeyDelete(lieStr);
                        break;
                    }
                }
                continue;
                
            }
            return instockWl;
        }


        public List<WareLocation> GetInstockWLs(string batchNo, string proName, string position,
            string locker,int trayCount, bool updatePre = true)
        {
            List<UseableLie> nullLie = GetLieState(batchNo, proName, position);
            List<WareLocation> wareLocationList = new List<WareLocation>(trayCount);
            int lastCount = trayCount;
            foreach (UseableLie temp in nullLie)
            {
               
                string lieStr = $"{WareLocationService.wareLocker}:{temp.WareLocation_Lie}";
                //获取列的状态
                //加锁判断该列是否被其他进程获取仓位
                using (var redislock = stringCacheRedisHelper.CreateLock(lieStr, TimeSpan.FromSeconds(10),
                    TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(200)))
                {
                    List<WareLocation> list = _wareLocationService.GetList(
                        u => u.WareLoca_Lie == temp.WareLocation_Lie
                    && u.WareArea.WareAreaClass.AreaClass == AreaClassType.ChengPinArea
                    , true, DbMainSlave.Master);
                    if (list.Where(u => u.WareLocaState == WareLocaState.PreOut).Count() > 0)
                    {
                        continue;
                    }
                    else
                    {
                        List<WareLocation> preInList =
                            list.Where(u => u.WareLocaState == WareLocaState.PreIn).ToList();
                        if (preInList.Count() > 0 &&
                            preInList.Where(u => u.BatchNo == batchNo).Count() == 0)
                        {
                            continue;
                        }
                    }
                    list = list.Where(u => u.WareLocaState == WareLocaState.NoTray).ToList();

                    int takeCount = lastCount > list.Count ? list.Count: lastCount;
                    if (list.Count > 0)
                    {
                        List<WareLocation> lieLists = null;
                        //获取排序规则
                        if (temp.InstockRule == InstockRuleAsc)
                        {
                            lieLists = list.OrderBy(u => u.WareLocaNo).Take(takeCount).ToList();
                            wareLocationList = wareLocationList.Union(lieLists).ToList();
                        }
                        else
                        {
                            lieLists = list.OrderByDescending(u => u.WareLocaNo).Take(takeCount).ToList();
                            wareLocationList = wareLocationList.Union(lieLists).ToList();
                        }
                        lastCount = lastCount - takeCount;
                        //预锁仓位
                        if (updatePre)
                        {
                            List<WareLoactionLockHis> wareLoactionLockHisList = 
                                new List<WareLoactionLockHis>(lieLists.Count);
                            DateTime lockTime = DateTime.Parse(
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.F"));
                            foreach (var tempWl in lieLists)
                            {
                                WareLoactionLockHis wareLoactionLockHis = new WareLoactionLockHis();
                                wareLoactionLockHis.WareLocaNo = tempWl.WareLocaNo;
                                wareLoactionLockHis.PreState = WareLocaState.PreIn;
                                wareLoactionLockHis.LockTime = lockTime;
                                wareLoactionLockHis.Locker = locker;
                                wareLoactionLockHisList.Add(wareLoactionLockHis);
                            }
                           
                            _wareLocationLockHisService.InsertBulk(wareLoactionLockHisList.AsQueryable());
                            _wareLocationLockHisService.SaveChanges();
                            int[] ids = lieLists.Select(u => u.ID).ToArray();
                            string[] wareNoList = lieLists.Select(u => u.WareLocaNo).ToArray();

                            List<WareLoactionLockHis> lockHisList  =_wareLocationLockHisService.GetList(
                                    u => wareNoList.Contains(u.WareLocaNo)
                                    && u.LockTime== lockTime,true,DbMainSlave.Master);

                            foreach(var tempWl in lieLists)
                            {
                                WareLoactionLockHis wareLoactionLockHis= 
                                    lockHisList.Find(u => u.WareLocaNo == tempWl.WareLocaNo);
                                tempWl.LockHis_ID = wareLoactionLockHis.ID;
                                tempWl.WareLocaState = WareLocaState.PreIn;
                                tempWl.BatchNo = batchNo;
                            }

                            DataTable dataTable = _wareLocationService.ConvertToDataTable(lieLists);
                            _wareLocationService.BatchUpdateData(dataTable, "WareLocation");


                        }

                        //stringCacheRedisHelper.KeyDelete(lieStr);
                        
                    }


                }
            }
            return wareLocationList;
        }


        #endregion


        #region 其他

        public List<WareLocation> ConverToList(List<WareLocation> list)
        {
            List<WareLocation> wlList = new List<WareLocation>();
            wlList = _wareLocationService.ConvertList(list);
            return wlList;
        }
        #endregion

    }
}

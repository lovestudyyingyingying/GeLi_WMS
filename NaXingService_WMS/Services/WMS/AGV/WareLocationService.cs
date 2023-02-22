using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.InstockEntity;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Helper.WMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services.WMS.AGV
{
    public class WareLocationService: DbBase<WareLocation>
    {
        //DbBase<WareLocation> wareLocationDao = new DbBase<WareLocation>();
        public static string wareLocker = "WareLieLock";

        #region 简单查询

        public IQueryable<WareLocationIndexData> GetIndexData(Expression<Func<WareLocation, bool>> expression)
        {
            return SelectToQuery<WareLocationIndexData>(expression,
                u => new WareLocationIndexData()
                {
                    ID = u.ID,
                    AGVPosition = u.AGVPosition,
                    WareLocaNo = u.WareLocaNo,
                    WareArea_ID = u.WareArea_ID,
                    WareLocaState= u.WareLocaState,
                    TrayStateNo = u.TrayState==null?String.Empty:u.TrayState.TrayNO,
                    TrayStateBatchNo= u.TrayState == null ? String.Empty : u.TrayState.batchNo,
                    IsOpen=u.IsOpen??0
                });
        }



        /// <summary>
        /// 根据ID获取仓位信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public WareLocation GetByID(int ID)
        {
            return FindById(ID);
        }

        /// <summary>
        /// 根据仓位号获取仓位信息
        /// </summary>
        /// <param name="warelocaNo"></param>
        /// <returns></returns>
        public WareLocation GetByWLNo(string warelocaNo,bool isNoTracking=false, DbMainSlave dms = DbMainSlave.Slave)
        {
            return GetIQueryable(u=>u.WareLocaNo==warelocaNo,isNoTracking,dms).FirstOrDefault();
        }

        /// <summary>
        /// 根据AGV内部仓位号获取仓位信息
        /// </summary>
        /// <param name="agvPosition"></param>
        /// <returns></returns>
        public WareLocation GetByAGVPo(string agvPosition, bool isNoTracking = false, DbMainSlave dms = DbMainSlave.Slave)
        {
            return GetIQueryable(u => u.AGVPosition == agvPosition, isNoTracking, dms).FirstOrDefault();
        }
        #endregion

        #region 指定库位查询

        #region 可入 库位查询
       
        /// <summary>
        /// 获取成品区出库到缓存区的仓位
        /// </summary>
        /// <param name="count">成品区出库数量</param>
        /// <returns>缓存区仓位集合</returns>
        //public List<UseableLie> GetNullLies(string batchNo, string proName, string position)
        //{



        //}
        #endregion

        #region 可出 库位查询
        OutstockHelper outstockWLsHelper = null;
        /// <summary>
        /// 根据出仓计划获取出仓仓位，并预锁仓位（缓存区、成品区通用）
        /// </summary>
        /// <param name="sp">出库计划</param>
        /// <param name="isAutoOut">是否预锁仓位</param>
        /// <returns>出仓仓位</returns>
        //public List<WareLocation> GetWL_Out(StockPlan sp,bool isAutoOut=true)
        //{
        //    if (outstockWLsHelper == null)
        //        outstockWLsHelper = new OutstockHelper(this);

        //    return outstockWLsHelper.GetOutStockProWl(sp, isAutoOut);
        //}
        #endregion


        #endregion


        #region 更新
        //public void Update(WareLocation wareLocation)
        //{
        //    Update(wareLocation);
        //    SaveChanges();
        //}


        #endregion

    }
}

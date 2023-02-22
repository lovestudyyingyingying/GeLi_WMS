using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UtilsSharp;

namespace NanXingService_WMS.Services
{
    public class WareHouseService: DbBase<WareHouse>
    {

        #region query
        /// <summary>
        /// 获取仓库数据
        /// </summary>
        /// <param name="whereLambda">筛选条件</param>
        /// <returns>仓库数据IQueryable</returns>
        public IQueryable<WareHouse> GetWareHouses(Expression<Func<WareHouse, bool>> whereLambda = null)
        {
            return GetIQueryable(whereLambda);
        }

        public WareHouse FindById(int ID, DbMainSlave dms = DbMainSlave.Slave)
        {
            return FindById(ID, dms);
        }
      
        #endregion

        #region add
        /// <summary>
        /// 添加仓库
        /// </summary>
        /// <param name="wh">仓库实体</param>
        /// <returns>仓库数据IQueryable</returns>
        public int AddWareHouses(WareHouse wh)
        {
            Insert(wh);
            return SaveChanges();
        }
        #endregion

        #region update
        /// <summary>
        /// 修改整条仓库数据
        /// </summary>
        /// <param name="wh"></param>
        /// <returns></returns>
        public int UpdateWareHouses(WareHouse wh)
        {
            Update(wh);
            return SaveChanges();
        }
        #endregion

        #region delete
        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int DeleteWareHouses(int ID)
        {
            WareHouse wareHouse = FindById(ID);
            Delete(wareHouse);
            return SaveChanges();
        }
        #endregion
    }
}

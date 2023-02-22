using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services
{
    public class WorkshopProcessService: DbBase<WorkShopProcess>
    {
        DbBase<ProcessClass> processClassDao = new DbBase<ProcessClass>();
        //DbBase<WorkShopProcess> workshopProcessDao = new DbBase<WorkShopProcess>();



        #region query
        /// <summary>
        /// 工序类型IQueryable
        /// </summary>
        /// <param name="whereLambda">筛选条件</param>
        /// <returns></returns>
        public IQueryable<ProcessClass> GetProcessClassQuery(Expression<Func<ProcessClass, bool>> whereLambda = null)
        {
            return processClassDao.GetIQueryable(whereLambda);
        }


        
        /// <summary>
        /// 根据ID获取ProcessClass实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dms"></param>
        /// <returns></returns>
        public ProcessClass FindProcessClassById(int id, DbMainSlave dms = DbMainSlave.Slave)
        {
            return processClassDao.FindById(id, dms);
        }

        /// <summary>
        /// 工序车间IQueryable
        /// </summary>
        /// <param name="whereLambda">筛选条件</param>
        /// <returns></returns>
        public IQueryable<WorkShopProcess> GetWorkShopProcessQuery(Expression<Func<WorkShopProcess, bool>> whereLambda = null)
        {
            return GetIQueryable(whereLambda);
        }

        /// <summary>
        /// 根据ID获取WorkShopProcess实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dms"></param>
        /// <returns></returns>
        public WorkShopProcess FindWorkShopProcessById(int id, DbMainSlave dms = DbMainSlave.Slave)
        {
            return FindById(id, dms);
        }
        #endregion

        #region add
        /// <summary>
        /// 插入工序类型数据
        /// </summary>
        /// <param name="pc">工序类型实体</param>
        /// <returns></returns>
        public int AddProcessClass(ProcessClass pc)
        {
            processClassDao.Insert(pc);
            return processClassDao.SaveChanges();
        }
        /// <summary>
        /// 插入工序车间
        /// </summary>
        /// <param name="wsp">工序车间实体</param>
        /// <returns></returns>
        public int AddWorkShopProcess(WorkShopProcess wsp)
        {
            Insert(wsp);
            return SaveChanges();
        }

        #endregion

        #region update
        /// <summary>
        /// 更新工序类型
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public int UpdateProcessClass(ProcessClass pc)
        {
            processClassDao.Update(pc);
            return processClassDao.SaveChanges();
        }

        /// <summary>
        /// 更新工序车间
        /// </summary>
        /// <param name="wsp"></param>
        /// <returns></returns>
        public int UpdateWorkShopProcess(WorkShopProcess wsp)
        {
            Update(wsp);
            return SaveChanges();
        }
        #endregion

        #region delete
        /// <summary>
        /// 删除工序类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteProcessClass(int id)
        {
            ProcessClass pc = FindProcessClassById(id, DbMainSlave.Master);
            if (pc.workShopProcessList != null && pc.workShopProcessList.Count == 0)
            {
                processClassDao.Delete(pc);
                return processClassDao.SaveChanges();
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 删除工序车间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteWorkShopProcess(int id)
        {
            Delete(FindWorkShopProcessById(id, DbMainSlave.Master));
            return SaveChanges();
        }

        #endregion
    }
}

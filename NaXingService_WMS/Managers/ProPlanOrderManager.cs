using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity;
using NanXingService_WMS.Entity.CRMEntity;
using NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity;
using NanXingService_WMS.Entity.ProductEntity;
using NanXingService_WMS.Entity.ProPlanEntity;
using NanXingService_WMS.Services.APS;
using NanXingService_WMS.Threads.CRMThreads;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UtilsSharp;
using UtilsSharp.Standard;

namespace NanXingService_WMS.Services
{

    public class ProPlanOrderManager
    {
        ProPlanOrderheaderService proOrderHeadService =null;
        ProPlanOrderlistsService proOrderListService = null;
        CRMPlanListService crmpListService = null;
        //ProductOrderlistsService productOrderService = null;
        //string queueName = "ChangeCRM";
        //string queueName = "ChangeCRM";

        public ProPlanOrderManager(ProPlanOrderheaderService proOrderHeadService,
            ProPlanOrderlistsService proOrderListService, CRMPlanListService crmpListService)
        {
            this.proOrderHeadService = proOrderHeadService;
            this.proOrderListService = proOrderListService;
            this.crmpListService = crmpListService;
        }

        #region Query
        

        /// <summary>
        /// 根据ID获取排产单表头
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProPlanOrderheaders GetProOrder(int id, DbMainSlave dms=DbMainSlave.Slave)
        {   
            return proOrderHeadService.FindById(id, dms);
        }
        /// <summary>
        /// 根据ID获取排产单表体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProPlanOrderlists GetProList(int id)
        {
            return proOrderListService.FindById(id);
        }
        /// <summary>
        /// 根据条件获取排产单表体
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <param name="ordering"></param>
        /// <returns>排产单表体List</returns>
        public List<ProPlanOrderlists> GetList(Expression<Func<ProPlanOrderlists, bool>> whereLambda
             , Expression<Func<ProPlanOrderlists, string>> ordering, DbMainSlave dms = DbMainSlave.Slave)
        {
            return proOrderListService.GetList(whereLambda,false, dms, ordering);
        }

        public List<ProPlanOrderheaders> GetHeaders(Expression<Func<ProPlanOrderheaders, bool>> whereLambda,
             bool isNoTracking = false, DbMainSlave dms = DbMainSlave.Slave, Expression<Func<ProPlanOrderheaders, string>> ordering=null)
        {
            return proOrderHeadService.GetList(whereLambda, isNoTracking,  dms, ordering);
        }

        public ProPlanEditDto_Small GetPlanEdit_Small(int headerID)
        {
            var header=proOrderHeadService.FindById(headerID);
            ProPlanEditDto_Small editDto = new ProPlanEditDto_Small();
            MapperHelper<ProPlanOrderheaders, ProPlanEditDto_Small>.Map(header, editDto);
            //遍历List,将list的数据写入
            header.ProPlanOrderlists.ForEach((item) => {
                if (item.Chejianclass == "03小包装-小袋")
                    editDto.PcCount_03_Bag = item.PcCount??0;
                else if (item.Chejianclass == "03小包装-罐")
                    editDto.PcCount_03_Tank = item.PcCount ?? 0;
                else if (item.Chejianclass == "03小包装-每日坚果")
                    editDto.PcCount_03_Box = item.PcCount ?? 0;
                else if (item.Chejianclass == "07小包装-小袋")
                    editDto.PcCount_07_Bag = item.PcCount ?? 0;
                else if (item.Chejianclass == "07小包装-罐")
                    editDto.PcCount_07_Tank = item.PcCount ?? 0;
                else if (item.Chejianclass == "07小包装-每日坚果")
                    editDto.PcCount_07_Box = item.PcCount ?? 0;

                if (editDto.PlanDate == null)
                    editDto.PlanDate = item.PlanDate;
                if (string.IsNullOrEmpty(editDto.Priority))
                    editDto.Priority = item.Priority;
            });
            editDto.PcCount_03_Bag = editDto.PcCount_03_Bag ?? 0;
            editDto.PcCount_03_Tank = editDto.PcCount_03_Tank ?? 0;
            editDto.PcCount_03_Box = editDto.PcCount_03_Box ?? 0;
            editDto.PcCount_07_Bag = editDto.PcCount_07_Bag ?? 0;
            editDto.PcCount_07_Tank = editDto.PcCount_07_Tank ?? 0;
            editDto.PcCount_07_Box = editDto.PcCount_07_Box ?? 0;
            editDto.Itemno2 = editDto.Itemno;
            return editDto;
        }


        #endregion

        #region Add
        /// <summary>
        /// 新增排产单及明细
        /// </summary>
        /// <param name="pro">排产单示例</param>
        /// <returns>执行结果</returns>
        public int AddProOrder(ProPlanOrderheaders pro)
        {
            pro.ProPlanOrderlists.ForEach((item)=> {
                item.ProPlanOrderheaders_ID = pro.ID;
                item.ProPlanOrderheaders = pro;
            });
            proOrderHeadService.Insert(pro);
            proOrderListService.InsertRange(pro.ProPlanOrderlists);
            return proOrderHeadService.SaveChanges();
        }

        public bool AddProOrders(List<ProPlanOrderheaders> list,bool changeCrm=false)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    List<CRMPlanList> crmLists = new List<CRMPlanList>();
                    List<long> crmputLists = new List<long>();
                    foreach (var pro in list)
                    {
                        pro.ProPlanOrderlists.ForEach((item) => {
                            item.ProPlanOrderheaders_ID = pro.ID;
                            item.ProPlanOrderheaders = pro;

                            if (changeCrm)
                            {
                                CRMPlanList planList = crmpListService.FindById(pro.CRMPlanList_ID,DbMainSlave.Master);
                                planList.crmListStatus = PlanOrderState.HasPlan;

                                //CRMPlanEntity cRMPutItem = new CRMPlanEntity();
                                //cRMPutItem.planOrderNo = pro.PlanOrderNo;
                                //cRMPutItem.planOrderNo_XuHao = item.PlanOrder_XuHao;

                                //cRMPutItem.crm_ID = planList.CRMApplyList_InCode;
                                //cRMPutItem.fzCount = (pro.PcCount ?? 0).ToString();
                                //cRMPutItem.fzUnit= pro.Unit;
                                //cRMPutItem.pcUnit = "kg";
                                //decimal convertRate = string.IsNullOrEmpty(planList.ConvertRate) ? 0 : decimal.Parse(planList.ConvertRate);
                                //cRMPutItem.pcCount = ((pro.PcCount ?? 0) * convertRate).ToString();
                                //cRMPutItem.crmState = PlanOrderState.HasPlan;
                                //cRMPutItem.pcTime = UnixDateTImeUtils.ConvertDateTimeInt(DateTime.Now);
                                //cRMPutItem.batchNo = pro.BatchNo;
                                crmLists.Add(planList);
                                crmputLists.Add(pro.CRMPlanList_ID??0);
                            }



                        });
                        proOrderHeadService.Insert(pro);
                        proOrderListService.InsertRange(pro.ProPlanOrderlists);
                    }

                    proOrderHeadService.SaveChanges();
                    crmpListService.SaveChanges();
                    if (changeCrm && crmLists.Count > 0)
                    {
                        crmpListService.UpdateAll(crmLists);
                        
                        RabbitMQUtils.Send(CRMReturnWriteThread.queueName_CRMReturnWrite,
                            crmputLists);
                    }

                    tran.Complete();
                }
                catch (Exception ex)
                {
                    //执行错误处理
                    Logger.Default.Process(new Log(LevelType.Error, ex.ToString()));
                    return false;
                }
            }
            return true;
            
        }

        //public bool ChangeProOrders(List<long> ids,string statue)
        //{
        //    long[] idsArr = ids.ToArray();
        //    var list = crmpListService.GetList(u => idsArr.Contains(u.ID)&& u.CRMApplyList_InCode!=null
        //    , true, DbMainSlave.Master);

        //    using (TransactionScope tran = new TransactionScope())
        //    {
        //        try
        //        {
        //            List<CRMPlanList> crmLists = new List<CRMPlanList>();
        //            List<CRMPlanEntity> crmputLists = new List<CRMPlanEntity>();
        //            string crmState = string.Empty;

        //            foreach (var planList in list)
        //            {
        //                if (crmLists == null) crmLists = new List<CRMPlanList>(list.Count);
        //                if (crmState == string.Empty)
        //                    crmState = "已排产";
        //                //CRMPlanList planList = crmpListService.FindById(pro.CRMPlanList_ID, DbMainSlave.Master);
        //                planList.crmListStatus = crmState;

        //                CRMPlanEntity cRMPutItem = new CRMPlanEntity();
        //                cRMPutItem.crmID = planList.CRMApplyList_InCode;
        //                cRMPutItem.fzCount = (pro.PcCount ?? 0).ToString();
        //                cRMPutItem.fzUnit = pro.Unit;
        //                cRMPutItem.pcUnit = "kg";
        //                decimal convertRate = string.IsNullOrEmpty(planList.ConvertRate) ? (decimal)0 : decimal.Parse(planList.ConvertRate);
        //                cRMPutItem.pcCount = (pro.PcCount ?? 0 * convertRate).ToString();
        //                cRMPutItem.pcState = "已下推";
        //                cRMPutItem.pcTime = UnixDateTImeUtils.ConvertDateTimeInt(DateTime.Now);
        //                cRMPutItem.batchNo = pro.BatchNo;
        //                crmLists.Add(planList);
        //                crmputLists.Add(cRMPutItem);
        //            }

        //            proOrderHeadService.SaveChanges();
        //            crmpListService.SaveChanges();
        //            if (changeCrm && crmLists.Count > 0)
        //            {
        //                crmpListService.UpdateAll(crmLists);

        //                RabbitMQUtils.Send(queueName, crmputLists);
        //            }

        //            tran.Complete();
        //        }
        //        catch (Exception ex)
        //        {
        //            //执行错误处理
        //            Logger.Default.Process(new Log(LevelType.Error, ex.ToString()));
        //            return false;
        //        }
        //    }
        //    return true;

        //}

        public bool EditProOrders(ProPlanOrderheaders ProPlanOrderheaders,
            List<ProPlanOrderlists> addlist, List<ProPlanOrderlists> editlist)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    addlist.ForEach((item) => {
                        item.ProPlanOrderheaders_ID = ProPlanOrderheaders.ID;
                        item.ProPlanOrderheaders = ProPlanOrderheaders;
                    });
                    proOrderListService.InsertRange(addlist);

                    editlist.ForEach((item) => {
                        item.ProPlanOrderheaders_ID = ProPlanOrderheaders.ID;
                        item.ProPlanOrderheaders = ProPlanOrderheaders;
                    });
                    proOrderHeadService.Update(ProPlanOrderheaders);
                    proOrderListService.UpdateAll(editlist);
                    proOrderHeadService.SaveChanges();
                    proOrderListService.SaveChanges();
                    tran.Complete();
                }
                catch (Exception ex)
                {
                    //执行错误处理
                    Logger.Default.Process(new Log(LevelType.Error, ex.ToString()));
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 新增排产单及明细
        /// </summary>
        /// <param name="pro">排产单示例</param>
        /// <returns>执行结果</returns>
        public async Task<int> AddProOrderAsync(ProPlanOrderheaders pro)
        {
            pro.ProPlanOrderlists.ForEach((item) => {
                item.ProPlanOrderheaders_ID = pro.ID;
                item.ProPlanOrderheaders = pro;
            });
            proOrderHeadService.Insert(pro);
            proOrderListService.InsertRange(pro.ProPlanOrderlists);
            return await proOrderHeadService.SaveChangesAsync();
        }

        #endregion

        #region Update
        /// <summary>
        /// 单纯修改排产单及明细
        /// </summary>
        /// <param name="pro">排产单实例</param>
        /// <param name="headModColumnsList">排产单修改的列名集合</param>
        /// <returns>执行结果</returns>
        public int UpdateProOrder(ProPlanOrderheaders pro,List<string> headModColumnsList)
        {
            proOrderHeadService.Update(pro, headModColumnsList);
            proOrderListService.UpdateAll(pro.ProPlanOrderlists);
            return proOrderHeadService.SaveChanges();
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <returns></returns>
        public int UpdateOrderProData()
        {
            return 0;
        }

        #endregion

        #region Delete
        /// <summary>
        /// 移除排产单明细
        /// </summary>
        /// <param name="id">排产单明细ID</param>
        public bool DeleteProOrderList(int id)
        {

            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    ProPlanOrderheaders header = proOrderHeadService.FindById(id, DbMainSlave.Master);
                    List<ProPlanOrderlists> list = header.ProPlanOrderlists;
                    foreach(ProPlanOrderlists temp in list)
                        temp.PlanOrder_State = "已删除";
                    if (header.CRMPlanList_ID != null && header.CRMPlanList_ID > 0)
                    {
                        header.crmPlanList.crmListStatus = "未排产";
                        crmpListService.Update(header.crmPlanList, new List<string>(1) { "crmListStatus" });
                    }

                    //proOrderListService.Delete(u => u.ID == id);
                    proOrderListService.UpdateAll(list);
                    proOrderListService.SaveChanges();
                    crmpListService.SaveChanges();
                    tran.Complete();
                }
                catch (Exception ex)
                {
                    //执行错误处理
                    Logger.Default.Process(new Log(LevelType.Error, ex.ToString()));
                    return false;
                }
            }
            return true;
            
        }

        /// <summary>
        /// 移除整个排产单数据
        /// </summary>
        /// <param name="pro">排产单表头</param>
        /// <returns>执行结果</returns>
        public int DeleteProOrderAll(ProPlanOrderheaders pro)
        {
            proOrderListService.Delete(u => u.ProPlanOrderheaders_ID == pro.ID);
            proOrderHeadService.Delete(pro);
            return proOrderHeadService.SaveChanges();
        }

        /// <summary>
        /// 移除多个排产单数据
        /// </summary>
        /// <param name="pro">多个排产ID</param>
        /// <returns>执行结果</returns>
        public RunResult<string> DeleteProOrderAll(List<int> ids)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    int?[] ids2 = Array.ConvertAll<int, int?>(ids.ToArray(), s => s);
                   
                    //原CRM订单需要回滚状态
                    long?[] crmIDs = proOrderHeadService.GetIQueryable(u => ids2.Contains(u.ID)).
                        Select(u=>u.CRMPlanList_ID).ToArray();

                    List<CRMPlanList> list = crmpListService.GetList(u => crmIDs.Contains(u.ID),
                        false,DbMainSlave.Master);
                  
                    string column = "crmListStatus";
                    List<long> applyStates = new List<long>();
                   
                    foreach (var item in list)
                    {
                        if (item != null)
                        {
                            //CRMApplyState applyState = new CRMApplyState()
                            //{
                            //    crm_ID = item.CRMApplyList_InCode,
                            //    pcState= PlanOrderState.NoPlan,
                            //    pcReason= string.Empty
                            //};
                            //CRMWriteEntity writeEntity = new CRMWriteEntity(WriteTypeEnum.CRMDeletePlan.GetEnumDescription(), applyState);
                             applyStates.Add(item.ID);
                            item.crmListStatus = PlanOrderState.NoPlan;
                            crmpListService.Update(item, new List<string>(1) { column });
                        }
                    }
                    crmpListService.SaveChanges();
                    proOrderListService.Delete(u => ids2.Contains(u.ProPlanOrderheaders_ID));
                    proOrderHeadService.Delete(u => ids2.Contains(u.ID));

                    proOrderHeadService.SaveChanges();
                    //proOrderListService.Delete(u => u.ID == id);
                    
                    tran.Complete();
                    RabbitMQUtils.Send(CRMReturnWriteThread.queueName_CRMReturnWrite, applyStates);
                }
                catch (Exception ex)
                {
                    //执行错误处理
                    Logger.Default.Process(new Log(LevelType.Error, ex.ToString()));
                    return RunResult<string>.False(ex.ToString());
                }
            }
            return RunResult<string>.True();
        }

        public int Save()
        {
            return proOrderHeadService.SaveChanges();
        }
        #endregion
    }
}

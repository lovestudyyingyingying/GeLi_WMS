using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.CRMEntity;
using NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity;
using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys.QueueInputEntitys;
using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys.QueueOutputEntitys;
using NanXingService_WMS.Helper.APS;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.RabbitMQ;
using NanXingService_WMS.Utils.ThreadUtils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using UtilsSharp.Standard;

namespace NanXingService_WMS.Threads.CRMThreads
{
    public class CRMReturnWriteThread
    {
        public static string queueName_CRMPushPlan = "CRMPushPlan";
        public static string queueName_CRMPushPro = "CRMPushPro";

        public static string queueName_CRMChangeState = "CRMChangeState";
        public static string queueName_CRMProTask = "CRMProTask";
        public static string queueName_CRMProState = "CRMProState";

        public static string queueName_CRMDeleteTask = "CRMDeleteTask";

        public static string queueName_CRMReturnWrite = "CRMReturnWriteNew";


        CRMApiHelper crmApiHelper = new CRMApiHelper();
        RabbitMQUtils rabbitMQUtils = new RabbitMQUtils();

        MyTask pushPlanTask = null;
        MyTask pushProTask = null;

        MyTask changeTask = null;
        MyTask proTask = null;
        MyTask deleteTask = null;

        MyTask returnWriteTask = null;
        DbBase<CRMPlanList> crmListDbBase = new DbBase<CRMPlanList>();
        DbBase<Users> usersDbBase = new DbBase<Users>();
        /// <summary>
        /// 回写CRM
        /// 1、下推排产计划：排产单号、CRM 状态、排产数量、排产单位、辅助数量、辅助单位、排产时间、批次号
        /// 2、下推生产计划：生产单号、CRM 状态、任务车间、生产状态、计划时间、更新人
        /// 3、回写生产产量：生产单号、产量、生产单位、开始时间、结束时间、更新人
        /// 4、回写生产状态：生产单号、CRM 状态、生产状态
        /// 5、删除生产单  ：生产单号、生产状态、任务车间、产量、生产单位、开始时间、结束时间、更新人
        /// 6、删除排产计划：全部清空
        /// </summary>
        /// <param name="writeEntity"></param>
        public void ReturnWriteCRM(long ID)
        {
            //1、分发回写CRM类型
            //if (string.IsNullOrEmpty(writeEntity.WriteType))
            //{
            //    Logger.Default.Process(new Log(LevelType.Info, $"CRM回写类型不能为空：{writeEntity.WriteType}"));
            //    return;
            //}
            //else if (writeEntity.WriteType == WriteTypeEnum.CRMPushPlan.GetEnumDescription())
            //{
            //    CRMPlanEntity planList= JsonConvert.DeserializeObject<CRMPlanEntity>(writeEntity.WriteObject.ToString());
            //    WriteCRMPushPlan(planList);
            //}
            //else if (writeEntity.WriteType == WriteTypeEnum.CRMPushPro.GetEnumDescription())
            //{
                //CRMProEntity planList = JsonConvert.DeserializeObject<CRMProEntity>(writeEntity.WriteObject.ToString());
            //    WriteCRMPushPro(planList);
            //}
            //else if (writeEntity.WriteType == WriteTypeEnum.CRMChangeState.GetEnumDescription())
            //{
                //CRMApplyState planList = JsonConvert.DeserializeObject<CRMApplyState>(writeEntity.WriteObject.ToString());
            //    WriteCRMChangeState(planList);
            //}
            //else if (writeEntity.WriteType == WriteTypeEnum.CRMProState.GetEnumDescription())
            //{
            //    CRMProState planList = JsonConvert.DeserializeObject<CRMProState>(writeEntity.WriteObject.ToString());
            //    WriteCRMTaskState(planList);
            //}
            //else if (writeEntity.WriteType == WriteTypeEnum.CRMProTask.GetEnumDescription())
            //{
            //    CRMProTask planList = JsonConvert.DeserializeObject<CRMProTask>(writeEntity.WriteObject.ToString());
            //    WriteCRMProTask(planList);
            //}
            //else if (writeEntity.WriteType == WriteTypeEnum.CRMDeleteTask.GetEnumDescription())
            //{
            //    CRMProTask planList = JsonConvert.DeserializeObject<CRMProTask>(writeEntity.WriteObject.ToString());
            //    DeleteCRMProTask(planList);
            //}
            //else if (writeEntity.WriteType == WriteTypeEnum.CRMDeletePlan.GetEnumDescription())
            //{
               // CRMApplyState planList = JsonConvert.DeserializeObject<CRMApplyState>(writeEntity.WriteObject.ToString());
            //    DeleteCRMPlan(planList);
            //}

            
            CRMPlanList cRMPlanList = crmListDbBase.FindById(ID);
            CRMEntityAll cRMEntityAll = new CRMEntityAll();
            cRMEntityAll._id = cRMPlanList.CRMApplyList_InCode;
            var ConvertRate = string.IsNullOrEmpty(cRMPlanList.ConvertRate.Trim()) ? 0 : Convert.ToDecimal(cRMPlanList.ConvertRate.Trim());
            var ProPlanOrderheaders = cRMPlanList.ProPlanOrderheaders.FirstOrDefault();
            var ProPlanOrderlists = cRMPlanList.ProPlanOrderlists;
            if (ProPlanOrderheaders!=null)
            {
                cRMEntityAll.field_t1iXR__c = ProPlanOrderheaders.PlanOrderNo;
                cRMEntityAll.field_p5sN5__c = UnixDateTImeUtils.ConvertDateTimeInt(ProPlanOrderheaders.Newdate ?? DateTime.Now).ToString();
                cRMEntityAll.field_r5Tqq__c = ProPlanOrderheaders.Unit;
                cRMEntityAll.field_pl8w1__c = (ProPlanOrderheaders.PcCount ?? 0).ToString();
                cRMEntityAll.field_I54DT__c = ((ProPlanOrderheaders.PcCount ?? 0) * ConvertRate).ToString();
                cRMEntityAll.field_4yUJU__c = cRMPlanList.crmListStatus;
                cRMEntityAll.field_0h244__c = "Kg";
                for (int i = 0; i < ProPlanOrderlists.Count; i++)
                {
                    if (i == 0)
                    {
                        cRMEntityAll.field_w2lSP__c = usersDbBase.FirstOrDefault(U => U.ChineseName == ProPlanOrderlists[i].Jingbanren).Name + ":" + ProPlanOrderlists[i].Jingbanren;
                        cRMEntityAll.field_xqV0O__c = UnixDateTImeUtils.ConvertDateTimeInt(ProPlanOrderlists[i].PlanDate ?? DateTime.Now).ToString();
                        cRMEntityAll.field_MLRho__c = ProPlanOrderlists[i].Chejianclass;
                        cRMEntityAll.field_0zFL1__c = ProPlanOrderlists[i].PlanOrder_XuHao;
                        var productOrderFirst = ProPlanOrderlists[i].ProductOrderlists.FirstOrDefault();
                        if (productOrderFirst!=null)
                        {
                            var finishTime = productOrderFirst.FinishTime.HasValue
                               ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.FinishTime.Value).ToString() : string.Empty;
                            var startTime = productOrderFirst.StartTime.HasValue
                                   ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.StartTime.Value).ToString() : string.Empty;
                            cRMEntityAll.field_orBwH__c = finishTime;
                            cRMEntityAll.field_B1wK3__c = startTime;

                            cRMEntityAll.field_RnAI2__c = productOrderFirst.ProductOrder_XuHao;
                            cRMEntityAll.field_cmFG8__c = productOrderFirst.Unit;
                            cRMEntityAll.field_evz13__c = (productOrderFirst.ProCount ?? 0).ToString();
                            cRMEntityAll.field_070o1__c = productOrderFirst.ProOrderList_State;
                        }

                    }
                    else if (i == 1)
                    {
                        cRMEntityAll.field_rb734__c = usersDbBase.FirstOrDefault(U =>U.ChineseName == ProPlanOrderlists[i].Jingbanren).Name +":"+ ProPlanOrderlists[i].Jingbanren;
                        cRMEntityAll.field_Yyg67__c = UnixDateTImeUtils.ConvertDateTimeInt(ProPlanOrderlists[i].PlanDate ?? DateTime.Now).ToString();
                        cRMEntityAll.field_PUHit__c = ProPlanOrderlists[i].Chejianclass;
                        cRMEntityAll.field_70cFm__c = ProPlanOrderlists[i].PlanOrder_XuHao;
                        var productOrderFirst = ProPlanOrderlists[i].ProductOrderlists.FirstOrDefault();
                        if (productOrderFirst != null)
                        {
                            var finishTime = productOrderFirst.FinishTime.HasValue
                               ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.FinishTime.Value).ToString() : string.Empty;
                            var startTime = productOrderFirst.StartTime.HasValue
                                   ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.StartTime.Value).ToString() : string.Empty;
                            cRMEntityAll.field_8hUjG__c = finishTime;
                            cRMEntityAll.field_2xK4R__c = startTime;

                            cRMEntityAll.field_goHWR__c = productOrderFirst.ProductOrder_XuHao;
                            cRMEntityAll.field_7gsp0__c = productOrderFirst.Unit;
                            cRMEntityAll.field_31flP__c = (productOrderFirst.ProCount ?? 0).ToString();
                            cRMEntityAll.field_zPc3r__c = productOrderFirst.ProOrderList_State;
                        }
                    }
                    else if (i == 2)
                    {
                        cRMEntityAll.field_s9KMk__c = usersDbBase.FirstOrDefault(U => U.ChineseName == ProPlanOrderlists[i].Jingbanren).Name + ":" + ProPlanOrderlists[i].Jingbanren;
                        cRMEntityAll.field_1yTW1__c = UnixDateTImeUtils.ConvertDateTimeInt(ProPlanOrderlists[i].PlanDate ?? DateTime.Now).ToString();
                        cRMEntityAll.field_yAnwj__c = ProPlanOrderlists[i].Chejianclass;
                        cRMEntityAll.field_hbTdR__c = ProPlanOrderlists[i].PlanOrder_XuHao;
                        var productOrderFirst = ProPlanOrderlists[i].ProductOrderlists.FirstOrDefault();
                        if (productOrderFirst != null)
                        {
                            var finishTime = productOrderFirst.FinishTime.HasValue
                               ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.FinishTime.Value).ToString() : string.Empty;
                            var startTime = productOrderFirst.StartTime.HasValue
                                   ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.StartTime.Value).ToString() : string.Empty;
                            cRMEntityAll.field_l8aa2__c = finishTime;
                            cRMEntityAll.field_mzspU__c = startTime;

                            cRMEntityAll.field_4iY04__c = productOrderFirst.ProductOrder_XuHao;
                            cRMEntityAll.field_dTK9q__c = productOrderFirst.Unit;
                            cRMEntityAll.field_7nsNU__c = (productOrderFirst.ProCount ?? 0).ToString();
                            cRMEntityAll.field_6mobW__c = productOrderFirst.ProOrderList_State;
                        }
                    }
                    else if (i == 3)
                    {
                        cRMEntityAll.field_d2AW2__c = usersDbBase.FirstOrDefault(U => U.ChineseName == ProPlanOrderlists[i].Jingbanren).Name + ":" + ProPlanOrderlists[i].Jingbanren;
                        cRMEntityAll.field_4xcUU__c = UnixDateTImeUtils.ConvertDateTimeInt(ProPlanOrderlists[i].PlanDate ?? DateTime.Now).ToString();
                        cRMEntityAll.field_bv6S7__c = ProPlanOrderlists[i].Chejianclass;
                        cRMEntityAll.field_g4z0y__c = ProPlanOrderlists[i].PlanOrder_XuHao;
                        var productOrderFirst = ProPlanOrderlists[i].ProductOrderlists.FirstOrDefault();
                        if (productOrderFirst != null)
                        {
                            var finishTime = productOrderFirst.FinishTime.HasValue
                               ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.FinishTime.Value).ToString() : string.Empty;
                            var startTime = productOrderFirst.StartTime.HasValue
                                   ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.StartTime.Value).ToString() : string.Empty;
                            cRMEntityAll.field_40rQb__c = finishTime;
                            cRMEntityAll.field_614ed__c = startTime;

                            cRMEntityAll.field_8ktK8__c = productOrderFirst.ProductOrder_XuHao;
                            cRMEntityAll.field_A2i6w__c = productOrderFirst.Unit;
                            cRMEntityAll.field_r67M2__c = (productOrderFirst.ProCount ?? 0).ToString();
                            cRMEntityAll.field_9am1y__c = productOrderFirst.ProOrderList_State;
                        }
                    }
                    else if (i == 4)
                    {
                        cRMEntityAll.field_1ouek__c = usersDbBase.FirstOrDefault(U => U.ChineseName == ProPlanOrderlists[i].Jingbanren).Name + ":" + ProPlanOrderlists[i].Jingbanren;
                        cRMEntityAll.field_n6fUQ__c = UnixDateTImeUtils.ConvertDateTimeInt(ProPlanOrderlists[i].PlanDate ?? DateTime.Now).ToString();
                        cRMEntityAll.field_5iPu6__c = ProPlanOrderlists[i].Chejianclass;
                        cRMEntityAll.field_UE26m__c = ProPlanOrderlists[i].PlanOrder_XuHao;
                        var productOrderFirst = ProPlanOrderlists[i].ProductOrderlists.FirstOrDefault();
                        if (productOrderFirst != null)
                        {
                            var finishTime = productOrderFirst.FinishTime.HasValue
                               ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.FinishTime.Value).ToString() : string.Empty;
                            var startTime = productOrderFirst.StartTime.HasValue
                                   ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.StartTime.Value).ToString() : string.Empty;
                            cRMEntityAll.field_1rL3W__c = finishTime;
                            cRMEntityAll.field_m1OPt__c = startTime;

                            cRMEntityAll.field_ewHJQ__c = productOrderFirst.ProductOrder_XuHao;
                            cRMEntityAll.field_75GaJ__c = productOrderFirst.Unit;
                            cRMEntityAll.field_f117d__c = (productOrderFirst.ProCount ?? 0).ToString();
                            cRMEntityAll.field_yJik8__c = productOrderFirst.ProOrderList_State;
                        }
                    }
                    else if (i == 5)
                    {
                        cRMEntityAll.field_69b3E__c = usersDbBase.FirstOrDefault(U => U.ChineseName == ProPlanOrderlists[i].Jingbanren).Name + ":" + ProPlanOrderlists[i].Jingbanren;
                        cRMEntityAll.field_1gETl__c = UnixDateTImeUtils.ConvertDateTimeInt(ProPlanOrderlists[i].PlanDate ?? DateTime.Now).ToString();
                        cRMEntityAll.field_6A7cc__c = ProPlanOrderlists[i].Chejianclass;
                        cRMEntityAll.field_cg0ac__c = ProPlanOrderlists[i].PlanOrder_XuHao;
                        var productOrderFirst = ProPlanOrderlists[i].ProductOrderlists.FirstOrDefault();
                        if (productOrderFirst != null)
                        {
                            var finishTime = productOrderFirst.FinishTime.HasValue
                               ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.FinishTime.Value).ToString() : string.Empty;
                            var startTime = productOrderFirst.StartTime.HasValue
                                   ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.StartTime.Value).ToString() : string.Empty;
                            cRMEntityAll.field_2F5zV__c = finishTime;
                            cRMEntityAll.field_abRIo__c = startTime;

                            cRMEntityAll.field_rEdHq__c = productOrderFirst.ProductOrder_XuHao;
                            cRMEntityAll.field_rHKfQ__c = productOrderFirst.Unit;
                            cRMEntityAll.field_S24fz__c = (productOrderFirst.ProCount ?? 0).ToString();
                            cRMEntityAll.field_f2A0g__c = productOrderFirst.ProOrderList_State;
                        }
                    }
                    else if (i == 6)
                    {
                        cRMEntityAll.field_1wc9U__c = usersDbBase.FirstOrDefault(U => U.ChineseName == ProPlanOrderlists[i].Jingbanren).Name + ":" + ProPlanOrderlists[i].Jingbanren;
                        cRMEntityAll.field_2oZ0S__c = UnixDateTImeUtils.ConvertDateTimeInt(ProPlanOrderlists[i].PlanDate ?? DateTime.Now).ToString();
                        cRMEntityAll.field_qKHGz__c = ProPlanOrderlists[i].Chejianclass;
                        cRMEntityAll.field_t64Wa__c = ProPlanOrderlists[i].PlanOrder_XuHao;
                        var productOrderFirst = ProPlanOrderlists[i].ProductOrderlists.FirstOrDefault();
                        if (productOrderFirst != null)
                        {
                            var finishTime = productOrderFirst.FinishTime.HasValue
                               ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.FinishTime.Value).ToString() : string.Empty;
                            var startTime = productOrderFirst.StartTime.HasValue
                                   ? UnixDateTImeUtils.ConvertDateTimeInt(productOrderFirst.StartTime.Value).ToString() : string.Empty;
                            cRMEntityAll.field_spgPb__c = finishTime;
                            cRMEntityAll.field_s0ryw__c = startTime;

                            cRMEntityAll.field_loso1__c = productOrderFirst.ProductOrder_XuHao;
                            cRMEntityAll.field_0Uw1j__c = productOrderFirst.Unit;
                            cRMEntityAll.field_1tqvO__c = (productOrderFirst.ProCount ?? 0).ToString();
                            cRMEntityAll.field_e3d3W__c = productOrderFirst.ProOrderList_State;
                        }
                    }
                }


            }

            crmApiHelper.WriteCRMApply(cRMEntityAll);

        }

        public void Control()
        {   
            if(returnWriteTask!=null)
                returnWriteTask.CloseTask();
            Thread.Sleep(2000);
            returnWriteTask = rabbitMQUtils.Recevice(queueName_CRMReturnWrite,
                new Action<long>((item) =>
                {
                    ReturnWriteCRM(item);
                }));
        }

        public void WriteCRMPushPlan(CRMPlanEntity planList)
        {
            //抽取CRM所需数据
            if (string.IsNullOrEmpty(planList.crm_ID))
            {
                Logger.Default.Process(new Log(LevelType.Error,
                 $"排产任务状态失败：传过来的CRMID为空"));
                return;
            }
            //planList.crm_ID = "6295d5df42b30b0001f263ca";
            //var ret=crmApiHelper.WriteCRMApply<CRMEntityPushPlan, CRMPlanEntity>(planList);
            DetailDatalist[] list =
            crmApiHelper.GetCRMDetailFromApi(planList.crm_ID);

            if (list == null|| list.Length==0)
            {
                Logger.Default.Process(new Log(LevelType.Error,
               $"排产任务状态失败：没有找到{planList.crm_ID}该单号"));
                return;
            }
            DetailDatalist detailDatalist = list.First();

            BaseResult<string> ret = null;
            //int index = 0;
            if (string.IsNullOrEmpty(detailDatalist.field_0zFL1__c))
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPlan1, CRMPlanEntity>(planList);
            else if (string.IsNullOrEmpty(detailDatalist.field_70cFm__c))
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPlan2, CRMPlanEntity>(planList);
            else if (string.IsNullOrEmpty(detailDatalist.field_hbTdR__c))
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPlan3, CRMPlanEntity>(planList);
            else if (string.IsNullOrEmpty(detailDatalist.field_g4z0y__c))
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPlan4, CRMPlanEntity>(planList);
            else if (!string.IsNullOrEmpty(detailDatalist.field_UE26m__c))
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPlan5, CRMPlanEntity>(planList);
            else if (!string.IsNullOrEmpty(detailDatalist.field_cg0ac__c))
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPlan6, CRMPlanEntity>(planList);
            else if (!string.IsNullOrEmpty(detailDatalist.field_t64Wa__c))
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPlan7, CRMPlanEntity>(planList);
            if (ret == null)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    $"排产任务状态失败：没有找到{planList.crm_ID}--{planList.crmState}"));
                return;
            }
            else if (ret.Code!=200)
            {
                Logger.Default.Process(new Log(LevelType.Warn,
                    $"排产任务状态失败：{planList.crm_ID}--{planList.crmState}:" +
                    $"{(ret==null?string.Empty:ret.Msg)}"));
                throw (new Exception(ret.Msg));
            }
            Logger.Default.Process(new Log(LevelType.Info,
                  $"排产任务状态成功：{planList.crm_ID}--{planList.crmState}-{ret.Msg}"));
        }

        public void WriteCRMPushPro(CRMProEntity planList)
        {
            //抽取CRM所需数据
            if (string.IsNullOrEmpty(planList.crm_ID))
            {
                Logger.Default.Process(new Log(LevelType.Error,
                 $"下推任务状态失败：传过来的CRMID为空"));
                return;
            }
            DetailDatalist[] list =
            crmApiHelper.GetCRMDetailFromApi(planList.crm_ID);

            if (list == null || list.Length == 0)
            {
                Logger.Default.Process(new Log(LevelType.Error,
               $"下推任务状态失败：没有找到{planList.crm_ID}该单号"));
                return;
            }
            DetailDatalist detailDatalist = list.First();

            BaseResult<string> ret = null;
            int index = GetIndexOrEmptyIndex(detailDatalist, 1 , false , planList.planOrderNo_XuHao);


            if (index==1)
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPro1, CRMProEntity>(planList);
            else if (index==2)
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPro2, CRMProEntity>(planList);
            else if(index==3)
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPro3, CRMProEntity>(planList);
            else if (index==4)
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPro4, CRMProEntity>(planList);
            else if (index==5)
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPro5, CRMProEntity>(planList);
            else if (index==6)
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPro6, CRMProEntity>(planList);
            else if (index==7)
                ret = crmApiHelper.WriteCRMApply<CRMEntityPushPro7, CRMProEntity>(planList);

            //planList.crm_ID = "6295d5df42b30b0001f263ca";
            //var ret = crmApiHelper.WriteCRMApply<CRMEntityPushPro1, CRMProEntity>(planList);
            if (ret == null)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    $"下推任务状态失败：没有找到{planList.crm_ID}--{planList.crmState}"));
                return;
            }
            else  if (ret==null || ret.Code != 200)
            {
                Logger.Default.Process(new Log(LevelType.Warn,
                    $"下推任务状态失败：{planList.crm_ID}--{planList.crmState}:\r\n{ret.Msg}"));
                throw (new Exception(ret.Msg));
            }
            Logger.Default.Process(new Log(LevelType.Info,
                  $"下推任务状态成功：{planList.crm_ID}--{planList.crmState}-{ret.Msg}"));
        }

        public void WriteCRMChangeState(CRMApplyState applyState)
        {
            //抽取CRM所需数据
            if (string.IsNullOrEmpty(applyState.crm_ID))
            {
                Logger.Default.Process(new Log(LevelType.Error,
                 $"修改任务状态失败：传过来的CRMID为空"));

                return;
            }
            var ret = crmApiHelper.WriteCRMApply<CRMEntityChangeState, CRMApplyState>(applyState);
            if (ret == null)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    $"修改任务状态失败：没有找到{applyState.crm_ID}--{applyState}"));
                return;
            }else  if (ret == null || ret.Code != 200)
            {
                Logger.Default.Process(new Log(LevelType.Warn,
                    $"修改任务状态失败：{applyState.crm_ID}--{applyState.pcState}:{ ret.Msg}"));
                throw (new Exception(ret.Msg));
            }
            Logger.Default.Process(new Log(LevelType.Info,
                    $"修改任务状态成功：{applyState.crm_ID}--{applyState.pcState}-{ret.Msg}"));
        }

        public void WriteCRMTaskState(CRMProState proState)
        {
            //抽取CRM所需数据
            if (string.IsNullOrEmpty(proState.crm_ID))
            {
                Logger.Default.Process(new Log(LevelType.Error,
                 $"修改任务状态失败：传过来的CRMID为空"));
                return;
            }

            DetailDatalist[] list =
            crmApiHelper.GetCRMDetailFromApi(proState.crm_ID);

            if (list == null || list.Length == 0)
            {
                Logger.Default.Process(new Log(LevelType.Error,
               $"修改任务数量失败：没有找到{proState.crm_ID}该单号"));
                return;
            }
            DetailDatalist detailDatalist = list.First();

            BaseResult<string> ret = null;

            int index = GetIndexOrEmptyIndex(detailDatalist, 2, false, proState.proOrderNo);

            if (index==1)
                ret = crmApiHelper.WriteCRMApply<CRMEntityChangeState1, CRMProState>(proState);
            else if (index==2)
                ret = crmApiHelper.WriteCRMApply<CRMEntityChangeState2, CRMProState>(proState);
            else if (index==3)
                ret = crmApiHelper.WriteCRMApply<CRMEntityChangeState3, CRMProState>(proState);
            else if (index==4)
                ret = crmApiHelper.WriteCRMApply<CRMEntityChangeState4, CRMProState>(proState);
            else if (index==5)
                ret = crmApiHelper.WriteCRMApply<CRMEntityChangeState5, CRMProState>(proState);
            else if (index==6)
                ret = crmApiHelper.WriteCRMApply<CRMEntityChangeState6, CRMProState>(proState);
            else if (index==7)
                ret = crmApiHelper.WriteCRMApply<CRMEntityChangeState7, CRMProState>(proState);

            if (ret == null)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    $"修改任务状态失败：没有找到{proState.crm_ID}:{proState.proOrderNo}"));
                return;
            }
            else  if ( ret.Code != 200)
            {
                Logger.Default.Process(new Log(LevelType.Warn,
                    $"修改任务状态失败：{proState.crm_ID}--{proState.pcState}:{ret.Msg}"));
                throw (new Exception( ret.Msg));
            }
            Logger.Default.Process(new Log(LevelType.Info,
                    $"修改任务状态成功：{proState.crm_ID}--{proState.pcState}-{ret.Msg}"));
        }

        public void WriteCRMProTask(CRMProTask proTask)
        {
            //抽取CRM所需数据
            
            if (string.IsNullOrEmpty(proTask.crm_ID))
            {
                Logger.Default.Process(new Log(LevelType.Error,
               $"修改任务数量失败：传过来的CRMID为空"));
                return;
            }
            //proTask.crm_ID = "6295d5df42b30b0001f263ca";
            DetailDatalist[] list=
            crmApiHelper.GetCRMDetailFromApi(proTask.crm_ID);

            if (list == null|| list.Length==0)
            {
                Logger.Default.Process(new Log(LevelType.Error,
               $"修改任务数量失败：没有找到{proTask.crm_ID}该单号"));
                return;
            }
            DetailDatalist detailDatalist = list.First();
            int index = GetIndexOrEmptyIndex(detailDatalist, 2, false, proTask.proTaskNo);
            BaseResult<string> ret = null;
            //int index = 0;
            if (index==1)
                ret = crmApiHelper.WriteCRMApply< CRMEntityTask1,CRMProTask>(proTask);
            else if (index==2)
                ret = crmApiHelper.WriteCRMApply<CRMEntityTask2, CRMProTask>(proTask);
            else if (index==3)
                ret = crmApiHelper.WriteCRMApply<CRMEntityTask3, CRMProTask>(proTask);
            else if (index==4)
                ret = crmApiHelper.WriteCRMApply<CRMEntityTask4, CRMProTask>(proTask);
            else if (index==5)
                ret = crmApiHelper.WriteCRMApply<CRMEntityTask5, CRMProTask>(proTask);
            else if (index==6)
                ret = crmApiHelper.WriteCRMApply<CRMEntityTask6, CRMProTask>(proTask);
            else if (index==7)
                ret = crmApiHelper.WriteCRMApply<CRMEntityTask7, CRMProTask>(proTask);

            if (ret == null)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    $"修改任务数量失败：{proTask.crm_ID}没有找到{proTask.proTaskNo}该任务单号的位置"));
                return;
            }
            else if (ret.Code != 200)
            {
                Logger.Default.Process(new Log(LevelType.Warn,
                    $"修改任务数量失败：{proTask.crm_ID}-{proTask.taskName}:{ret.Msg}"));
                throw (new Exception(ret.Msg));
            }
            Logger.Default.Process(new Log(LevelType.Info,
                    $"修改任务数量成功：{proTask.crm_ID}-{proTask.taskName}-{proTask.count}"));
        }

        public void DeleteCRMProTask(CRMProTask proTask)
        {

            if (string.IsNullOrEmpty(proTask.crm_ID))
            {
                Logger.Default.Process(new Log(LevelType.Error,
               $"删除CRM生产任务失败：传过来的CRMID为空"));
                return;
            }
            //proTask.crm_ID = "6295d5df42b30b0001f263ca";
            DetailDatalist[] list =
            crmApiHelper.GetCRMDetailFromApi(proTask.crm_ID);

            if (list == null || list.Length == 0)
            {
                Logger.Default.Process(new Log(LevelType.Error,
               $"删除生产任务失败：没有找到{proTask.crm_ID}该单号"));
                return;
            }
            DetailDatalist detailDatalist = list.First();
            CRMProTask cRMProTask = new CRMProTask()
            {
                crm_ID = proTask.crm_ID,
                proTaskNo=string.Empty,
                taskName= string.Empty,
                startTime = "0",
                endTime = "0",
                count=0,
                unit=string.Empty,
                updateUserID=string.Empty,
            };
            BaseResult<string> ret = null;
            int index = GetIndexOrEmptyIndex(detailDatalist, 2, false, proTask.proTaskNo);
            //int index = 0;
            if (index==1)
            {
                ret = crmApiHelper.WriteCRMApply<CRMEntityDeletePro1, CRMProTask>(cRMProTask);
            }
            else if (index==2)
            {
                ret = crmApiHelper.WriteCRMApply<CRMEntityDeletePro2, CRMProTask>(cRMProTask);
            }
            else if (index==3)
            {
                ret = crmApiHelper.WriteCRMApply<CRMEntityDeletePro3, CRMProTask>(cRMProTask);
            }
            else if (index==4)
            {
                ret = crmApiHelper.WriteCRMApply<CRMEntityDeletePro4, CRMProTask>(cRMProTask);
            }
            else if (index==5)
            {
                ret = crmApiHelper.WriteCRMApply<CRMEntityDeletePro5, CRMProTask>(cRMProTask);
            }
            else if (index==6)
            {
                ret = crmApiHelper.WriteCRMApply<CRMEntityDeletePro6, CRMProTask>(cRMProTask);
            }
            else if (index==7)
            {
                ret = crmApiHelper.WriteCRMApply<CRMEntityDeletePro7, CRMProTask>(cRMProTask);
            }

            if (ret == null)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    $"删除生产任务失败：{proTask.crm_ID}没有找到{proTask.proTaskNo}该任务号的位置"));
                return;
            }
            else if (ret.Code != 200)
            {
                Logger.Default.Process(new Log(LevelType.Warn,
                    $"删除生产任务失败：{proTask.crm_ID}-任务{index}-{proTask.proTaskNo}"));
                throw (new Exception(ret.Msg));
            }
            Logger.Default.Process(new Log(LevelType.Info,
                    $"删除生产任务成功：{proTask.crm_ID}-任务{index}-{proTask.proTaskNo}"));
        }

        public void DeleteCRMPlan(CRMApplyState planList)
        {
            if (string.IsNullOrEmpty(planList.crm_ID))
            {
                Logger.Default.Process(new Log(LevelType.Error,
               $"删除CRM排产计划失败：传过来的CRMID为空"));
                return;
            }
            BaseResult<string> ret = crmApiHelper.WriteCRMApply<CRMEntityEmpty, CRMApplyState>(planList);

            if (ret == null)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    $"删除排产计划失败：没有找到{planList.crm_ID}"));
                return;
            }
            else if (ret.Code != 200)
            {
                Logger.Default.Process(new Log(LevelType.Warn,
                    $"删除排产计划失败：{planList.crm_ID}-{ret.Msg}"));
                throw (new Exception(ret.Msg));
            }
            Logger.Default.Process(new Log(LevelType.Info,
                    $"删除排产计划成功：{planList.crm_ID}-{ret.Msg}"));
        }

        public void CloseCRMThread()
        {
            if (pushPlanTask!=null)
                pushPlanTask.CloseTask();
            if (pushProTask != null)
                pushProTask.CloseTask();
            if (changeTask != null)
                changeTask.CloseTask();
            if (proTask != null)
                proTask.CloseTask();
            if (deleteTask != null)
                deleteTask.CloseTask();
            if (returnWriteTask!=null)
                returnWriteTask.CloseTask();
            rabbitMQUtils.CloseRabbitMQ();
        }

        private string[] proPlanXuHaoColumns=new string[]
        {
            "field_0zFL1__c",
            "field_70cFm__c",
            "field_hbTdR__c",
            "field_g4z0y__c",
            "field_UE26m__c",
            "field_cg0ac__c",
            "field_t64Wa__c",
        };
        private string[] proXuHaoColumns = new string[]
        {
            "field_RnAI2__c",
            "field_goHWR__c",
            "field_4iY04__c",
            "field_8ktK8__c",
            "field_ewHJQ__c",
            "field_rEdHq__c",
            "field_loso1__c",
        };

        /// <summary>
        /// 序号
        /// </summary>
        /// <param name="detailDatalist">CRM数据</param>
        /// <param name="selectType">1为 计划单号、2为 生产单号</pa
        /// ram>
        /// <param name="returnEmpty">是否 返回空值</param>
        /// <param name="orderNo">计划单号 或 生产单号</param>
        /// <returns></returns>
        public int GetIndexOrEmptyIndex(DetailDatalist detailDatalist,int selectType 
            ,bool returnEmpty ,string orderNo)
        {
            Type type = detailDatalist.GetType();
            string[] targetArr = null;
            //目标：1、如果已有该单号，要知道是在第几个，如果没有，要知道第几个任务为空
            if (selectType==1)
            {
                targetArr=proPlanXuHaoColumns;
            }else
                targetArr=proXuHaoColumns;
            int emptyIndex = 0;
            PropertyInfo[] propertyInfos = type.GetProperties()
            .Where(u => targetArr.Contains(u.Name)).ToArray();
            for (int i = 0; i< targetArr.Length; i++)
            {
                PropertyInfo propertyInfo=propertyInfos.First(u => u.Name==targetArr[i]);

                object obj=propertyInfo.GetValue(detailDatalist,null);
                if (obj!=null && obj.ToString()==orderNo)
                    return i+1;
                else if (returnEmpty && emptyIndex==0)
                    emptyIndex=i+1;

            }
            return emptyIndex;
        }
    }
    public enum WriteTypeEnum
    {
        [Description("CRMPushPlan")]
        CRMPushPlan = 01,
        [Description("CRMPushPro")]
        CRMPushPro = 02,
        [Description("CRMChangeState")]
        CRMChangeState = 03,
        [Description("CRMProState")]
        CRMProState = 04,
        [Description("CRMProTask")]
        CRMProTask = 05,
        [Description("CRMDeleteTask")]
        CRMDeleteTask = 06,
        [Description("CRMDeletePlan")]
        CRMDeletePlan = 07
        //public static string queueName_CRMPushPlan = "CRMPushPlan";
        //public static string queueName_CRMPushPro = "CRMPushPro";

        //public static string queueName_CRMChangeState = "CRMChangeState";
        //public static string queueName_CRMProTask = "CRMProTask";
        //public static string queueName_CRMDeleteTask = "CRMDeleteTask";
    }

   

}

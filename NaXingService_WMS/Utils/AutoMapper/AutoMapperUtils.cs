using AutoMapper;
using NanXingData_WMS.Dao;
using NanXingService_WMS.Entity.CRMEntity;
using NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity;
using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys;
using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys.QueueInputEntitys;
using NanXingService_WMS.Entity.CRMEntity.CRMQueueEntitys.QueueOutputEntitys;
using NanXingService_WMS.Entity.CRMItemEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.AutoMapper
{ /// <summary>
  /// AutoMapper扩展帮助类
  /// </summary>
    public static class AutoMapperUtils
    {
        static MapperConfiguration mapperConfiguration = null;

        public static IMapper mapper;
        static AutoMapperUtils()
        {
            var config = new MapperConfiguration(
                    cfg =>
                    {
                        cfg.AddProfile<WriteCRMProfile>();
                    }
                );
            mapper = config.CreateMapper();

            //Register();
        }

        public static MapperConfiguration Register()
        {
            if (mapperConfiguration != null)
                return mapperConfiguration;
            else
                return mapperConfiguration = new MapperConfiguration(
                c =>
                {
                    c.ReplaceMemberName("_id", "crm_ID");


                    c.ReplaceMemberName("field_5bS63__c", "proOrderNo");
                    c.ReplaceMemberName("field_u40ye__c", "planOrderNo");
                    c.ReplaceMemberName("field_G26Hm__c", "planOrderNo");


                    c.ReplaceMemberName("field_4yUJU__c", "pcState");
                    c.ReplaceMemberName("field_I54DT__c", "pcCount");
                    c.ReplaceMemberName("field_0h244__c", "pcUnit");
                    c.ReplaceMemberName("field_pl8w1__c", "fzCount");
                    c.ReplaceMemberName("field_r5Tqq__c", "fzUnit");
                    c.ReplaceMemberName("field_p5sN5__c", "pcTime");
                    c.ReplaceMemberName("field_Vm3lG__c", "batchNo");

                    c.ReplaceMemberName("field_MLRho__c", "taskName");
                    c.ReplaceMemberName("field_B1wK3__c", "startTime");
                    c.ReplaceMemberName("field_orBwH__c", "endTime");
                    c.ReplaceMemberName("field_evz13__c", "count");
                    c.ReplaceMemberName("field_cmFG8__c", "unit");
                    c.ReplaceMemberName("field_w2lSP__c", "updateUserID");
                    c.ReplaceMemberName("field_RnAI2__c", "updateUserName");

                    c.ReplaceMemberName("field_PUHit__c", "taskName");
                    c.ReplaceMemberName("field_2xK4R__c", "startTime");
                    c.ReplaceMemberName("field_8hUjG__c", "endTime");
                    c.ReplaceMemberName("field_31flP__c", "count");
                    c.ReplaceMemberName("field_7gsp0__c", "unit");
                    c.ReplaceMemberName("field_rb734__c", "updateUserID");
                    c.ReplaceMemberName("field_goHWR__c", "updateUserName");

                    c.ReplaceMemberName("field_yAnwj__c", "taskName");
                    c.ReplaceMemberName("field_mzspU__c", "startTime");
                    c.ReplaceMemberName("field_l8aa2__c", "endTime");
                    c.ReplaceMemberName("field_7nsNU__c", "count");
                    c.ReplaceMemberName("field_dTK9q__c", "unit");
                    c.ReplaceMemberName("field_s9KMk__c", "updateUserID");
                    c.ReplaceMemberName("field_4iY04__c", "updateUserName");

                    c.ReplaceMemberName("field_bv6S7__c", "taskName");
                    c.ReplaceMemberName("field_614ed__c", "startTime");
                    c.ReplaceMemberName("field_40rQb__c", "endTime");
                    c.ReplaceMemberName("field_r67M2__c", "count");
                    c.ReplaceMemberName("field_A2i6w__c", "unit");
                    c.ReplaceMemberName("field_d2AW2__c", "updateUserID");
                    c.ReplaceMemberName("field_8ktK8__c", "updateUserName");

                    c.ReplaceMemberName("field_5iPu6__c", "taskName");
                    c.ReplaceMemberName("field_m1OPt__c", "startTime");
                    c.ReplaceMemberName("field_1rL3W__c", "endTime");
                    c.ReplaceMemberName("field_f117d__c", "count");
                    c.ReplaceMemberName("field_75GaJ__c", "unit");
                    c.ReplaceMemberName("field_1ouek__c", "updateUserID");
                    c.ReplaceMemberName("field_ewHJQ__c", "updateUserName");

                    c.ReplaceMemberName("field_6A7cc__c", "taskName");
                    c.ReplaceMemberName("field_abRIo__c", "startTime");
                    c.ReplaceMemberName("field_2F5zV__c", "endTime");
                    c.ReplaceMemberName("field_S24fz__c", "count");
                    c.ReplaceMemberName("field_rHKfQ__c", "unit");
                    c.ReplaceMemberName("field_69b3E__c", "updateUserID");
                    c.ReplaceMemberName("field_rEdHq__c", "updateUserName");

                    c.ReplaceMemberName("field_qKHGz__c", "taskName");
                    c.ReplaceMemberName("field_s0ryw__c", "startTime");
                    c.ReplaceMemberName("field_spgPb__c", "endTime");
                    c.ReplaceMemberName("field_1tqvO__c", "count");
                    c.ReplaceMemberName("field_0Uw1j__c", "unit");
                    c.ReplaceMemberName("field_1wc9U__c", "updateUserID");
                    c.ReplaceMemberName("field_loso1__c", "updateUserName");

                }
                );
        }


    }

    public class WriteCRMProfile : Profile
    {
        //添加你的实体映射关系.
        public WriteCRMProfile()
        {
            #region 物料

            CreateMap<ItemInfoDatalist, ItemInfo>()
               //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
               .ForMember(dto => dto.ItemNo, opt => opt.MapFrom(info => info.name))
               .ForMember(dto => dto.CRMID, opt => opt.MapFrom(info => info.CRMID))
               .ForMember(dto => dto.ItemName, opt => opt.MapFrom(info => info.product_code))
               .ForMember(dto => dto.Spec, opt => opt.MapFrom(info => info.product_spec))
               .ForMember(dto => dto.MainUtil, opt => opt.MapFrom(info => info.field_1n4aG__c))
               .ForMember(dto => dto.SlaveUtil, opt => opt.MapFrom(info => info.field_owUk6__c))
               .ForMember(dto => dto.ConvertRate, opt => opt.MapFrom(info => info.field_p5rBp__c))
               .ForMember(dto => dto.CreateTime, opt => opt.MapFrom(info => info.CreateTime))
               .ForMember(dto => dto.ModTime_CRM, opt =>opt.MapFrom(info => info.ModTime_CRM))
               .ForMember(dto => dto.UpdateTime, opt => opt.MapFrom(info => DateTime.Now.ToString("G")))
                .ForMember(dto => dto.FBARCODE, opt => opt.MapFrom(info => info.barcode))
                .ForMember(dto => dto.ID, opt => opt.Ignore())
                .ForMember(dto => dto.InName, opt => opt.Ignore())
                .ForMember(dto => dto.MaterialItem, opt => opt.Ignore())
                .ForMember(dto => dto.Workshops, opt => opt.Ignore())
                .ForMember(dto => dto.ModUser_APS, opt => opt.Ignore())
                .ForMember(dto => dto.ModTime_APS, opt => opt.Ignore())

               ;
            #endregion 物料
            //UserInfoEntity转UserInfoDto.
            //CreateMap<CRMPlanEntity, CRMEntityPushPlan>()
            //    //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
            //    .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
            //    .ForMember(dto => dto.field_t1iXR__c, opt => opt.MapFrom(info => info.planOrderNo))
            //    .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.pcState))
            //    .ForMember(dto => dto.field_I54DT__c, opt => opt.MapFrom(info => info.pcCount))
            //    .ForMember(dto => dto.field_0h244__c, opt => opt.MapFrom(info => info.pcUnit))
            //    .ForMember(dto => dto.field_pl8w1__c, opt => opt.MapFrom(info => info.fzCount))
            //    .ForMember(dto => dto.field_r5Tqq__c, opt => opt.MapFrom(info => info.fzUnit))
            //    .ForMember(dto => dto.field_p5sN5__c, opt => opt.MapFrom(info => info.pcTime))
            //    .ForMember(dto => dto.field_Vm3lG__c, opt => opt.MapFrom(info => info.batchNo))
            //    ;

            #region 下推排产计划

            CreateMap<CRMPlanEntity, CRMEntityPushPlan1>()
               //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
               .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
               .ForMember(dto => dto.field_t1iXR__c, opt => opt.MapFrom(info => info.planOrderNo))
               .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
               .ForMember(dto => dto.field_I54DT__c, opt => opt.MapFrom(info => info.pcCount))
               .ForMember(dto => dto.field_0h244__c, opt => opt.MapFrom(info => info.pcUnit))
               .ForMember(dto => dto.field_pl8w1__c, opt => opt.MapFrom(info => info.fzCount))
               .ForMember(dto => dto.field_r5Tqq__c, opt => opt.MapFrom(info => info.fzUnit))
               .ForMember(dto => dto.field_p5sN5__c, opt => opt.MapFrom(info => info.pcTime))
               .ForMember(dto => dto.field_Vm3lG__c, opt => opt.MapFrom(info => info.batchNo))
               .ForMember(dto => dto.field_0zFL1__c, opt => opt.MapFrom(info => info.planOrderNo_XuHao))

               ;

            CreateMap<CRMPlanEntity, CRMEntityPushPlan2>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
              .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
              .ForMember(dto => dto.field_t1iXR__c, opt => opt.MapFrom(info => info.planOrderNo))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
              .ForMember(dto => dto.field_I54DT__c, opt => opt.MapFrom(info => info.pcCount))
              .ForMember(dto => dto.field_0h244__c, opt => opt.MapFrom(info => info.pcUnit))
              .ForMember(dto => dto.field_pl8w1__c, opt => opt.MapFrom(info => info.fzCount))
              .ForMember(dto => dto.field_r5Tqq__c, opt => opt.MapFrom(info => info.fzUnit))
              .ForMember(dto => dto.field_p5sN5__c, opt => opt.MapFrom(info => info.pcTime))
              .ForMember(dto => dto.field_Vm3lG__c, opt => opt.MapFrom(info => info.batchNo))
              .ForMember(dto => dto.field_70cFm__c, opt => opt.MapFrom(info => info.planOrderNo_XuHao))
              ;
            CreateMap<CRMPlanEntity, CRMEntityPushPlan3>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
              .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
              .ForMember(dto => dto.field_t1iXR__c, opt => opt.MapFrom(info => info.planOrderNo))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
              .ForMember(dto => dto.field_I54DT__c, opt => opt.MapFrom(info => info.pcCount))
              .ForMember(dto => dto.field_0h244__c, opt => opt.MapFrom(info => info.pcUnit))
              .ForMember(dto => dto.field_pl8w1__c, opt => opt.MapFrom(info => info.fzCount))
              .ForMember(dto => dto.field_r5Tqq__c, opt => opt.MapFrom(info => info.fzUnit))
              .ForMember(dto => dto.field_p5sN5__c, opt => opt.MapFrom(info => info.pcTime))
              .ForMember(dto => dto.field_Vm3lG__c, opt => opt.MapFrom(info => info.batchNo))
              .ForMember(dto => dto.field_hbTdR__c, opt => opt.MapFrom(info => info.planOrderNo_XuHao))
              ;
            CreateMap<CRMPlanEntity, CRMEntityPushPlan4>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
              .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
              .ForMember(dto => dto.field_t1iXR__c, opt => opt.MapFrom(info => info.planOrderNo))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
              .ForMember(dto => dto.field_I54DT__c, opt => opt.MapFrom(info => info.pcCount))
              .ForMember(dto => dto.field_0h244__c, opt => opt.MapFrom(info => info.pcUnit))
              .ForMember(dto => dto.field_pl8w1__c, opt => opt.MapFrom(info => info.fzCount))
              .ForMember(dto => dto.field_r5Tqq__c, opt => opt.MapFrom(info => info.fzUnit))
              .ForMember(dto => dto.field_p5sN5__c, opt => opt.MapFrom(info => info.pcTime))
              .ForMember(dto => dto.field_Vm3lG__c, opt => opt.MapFrom(info => info.batchNo))
              .ForMember(dto => dto.field_g4z0y__c, opt => opt.MapFrom(info => info.planOrderNo_XuHao))
              ;

            CreateMap<CRMPlanEntity, CRMEntityPushPlan5>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
              .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
              .ForMember(dto => dto.field_t1iXR__c, opt => opt.MapFrom(info => info.planOrderNo))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
              .ForMember(dto => dto.field_I54DT__c, opt => opt.MapFrom(info => info.pcCount))
              .ForMember(dto => dto.field_0h244__c, opt => opt.MapFrom(info => info.pcUnit))
              .ForMember(dto => dto.field_pl8w1__c, opt => opt.MapFrom(info => info.fzCount))
              .ForMember(dto => dto.field_r5Tqq__c, opt => opt.MapFrom(info => info.fzUnit))
              .ForMember(dto => dto.field_p5sN5__c, opt => opt.MapFrom(info => info.pcTime))
              .ForMember(dto => dto.field_Vm3lG__c, opt => opt.MapFrom(info => info.batchNo))
              .ForMember(dto => dto.field_UE26m__c, opt => opt.MapFrom(info => info.planOrderNo_XuHao))
              ;

            CreateMap<CRMPlanEntity, CRMEntityPushPlan6>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
              .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
              .ForMember(dto => dto.field_t1iXR__c, opt => opt.MapFrom(info => info.planOrderNo))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
              .ForMember(dto => dto.field_I54DT__c, opt => opt.MapFrom(info => info.pcCount))
              .ForMember(dto => dto.field_0h244__c, opt => opt.MapFrom(info => info.pcUnit))
              .ForMember(dto => dto.field_pl8w1__c, opt => opt.MapFrom(info => info.fzCount))
              .ForMember(dto => dto.field_r5Tqq__c, opt => opt.MapFrom(info => info.fzUnit))
              .ForMember(dto => dto.field_p5sN5__c, opt => opt.MapFrom(info => info.pcTime))
              .ForMember(dto => dto.field_Vm3lG__c, opt => opt.MapFrom(info => info.batchNo))
              .ForMember(dto => dto.field_cg0ac__c, opt => opt.MapFrom(info => info.planOrderNo_XuHao))
              ;

            CreateMap<CRMPlanEntity, CRMEntityPushPlan7>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
              .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
              .ForMember(dto => dto.field_t1iXR__c, opt => opt.MapFrom(info => info.planOrderNo))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
              .ForMember(dto => dto.field_I54DT__c, opt => opt.MapFrom(info => info.pcCount))
              .ForMember(dto => dto.field_0h244__c, opt => opt.MapFrom(info => info.pcUnit))
              .ForMember(dto => dto.field_pl8w1__c, opt => opt.MapFrom(info => info.fzCount))
              .ForMember(dto => dto.field_r5Tqq__c, opt => opt.MapFrom(info => info.fzUnit))
              .ForMember(dto => dto.field_p5sN5__c, opt => opt.MapFrom(info => info.pcTime))
              .ForMember(dto => dto.field_Vm3lG__c, opt => opt.MapFrom(info => info.batchNo))
              .ForMember(dto => dto.field_t64Wa__c, opt => opt.MapFrom(info => info.planOrderNo_XuHao))
              ;
            #endregion

            #region 回写CRM总状态 
            CreateMap<CRMApplyState, CRMEntityChangeState>()
                //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.pcState))
                .ForMember(dto => dto.field_G26Hm__c, opt => opt.MapFrom(info => info.pcReason))
                ;

            #endregion

            #region 回写CRM任务状态
            CreateMap<CRMProState, CRMEntityChangeState1>()
               //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
               .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
               .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.pcState))
               .ForMember(dto => dto.field_070o1__c, opt => opt.MapFrom(info => info.proState))
               ;

            CreateMap<CRMProState, CRMEntityChangeState2>()
               //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
               .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
               .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.pcState))
               .ForMember(dto => dto.field_zPc3r__c, opt => opt.MapFrom(info => info.proState))
               ;
            CreateMap<CRMProState, CRMEntityChangeState3>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
              .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.pcState))
              .ForMember(dto => dto.field_6mobW__c, opt => opt.MapFrom(info => info.proState))
              ;
            CreateMap<CRMProState, CRMEntityChangeState4>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
              .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.pcState))
              .ForMember(dto => dto.field_9am1y__c, opt => opt.MapFrom(info => info.proState))
              ;
            CreateMap<CRMProState, CRMEntityChangeState5>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
              .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.pcState))
              .ForMember(dto => dto.field_yJik8__c, opt => opt.MapFrom(info => info.proState))
              ;
            CreateMap<CRMProState, CRMEntityChangeState6>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
            .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
            .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.pcState))
            .ForMember(dto => dto.field_f2A0g__c, opt => opt.MapFrom(info => info.proState))
            ;
            CreateMap<CRMProState, CRMEntityChangeState7>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
            .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
            .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.pcState))
            .ForMember(dto => dto.field_e3d3W__c, opt => opt.MapFrom(info => info.proState))
            ;
            #endregion 回写CRM任务状态

            #region 下推生产计划
            CreateMap<CRMProEntity, CRMEntityPushPro1>()
             //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
             .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
             .ForMember(dto => dto.field_RnAI2__c, opt => opt.MapFrom(info => info.proOrderNo))
              .ForMember(dto => dto.field_MLRho__c, opt => opt.MapFrom(info => info.taskName))
             .ForMember(dto => dto.field_w2lSP__c, opt => opt.MapFrom(info => info.updateUser))
             .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
             .ForMember(dto => dto.field_070o1__c, opt => opt.MapFrom(info => info.proState))
             .ForMember(dto => dto.field_xqV0O__c, opt => opt.MapFrom(info => info.planTime));


            CreateMap<CRMProEntity, CRMEntityPushPro2>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
            .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
            .ForMember(dto => dto.field_goHWR__c, opt => opt.MapFrom(info => info.proOrderNo))
             .ForMember(dto => dto.field_PUHit__c, opt => opt.MapFrom(info => info.taskName))
             .ForMember(dto => dto.field_rb734__c, opt => opt.MapFrom(info => info.updateUser))
             .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
             .ForMember(dto => dto.field_zPc3r__c, opt => opt.MapFrom(info => info.proState))
             .ForMember(dto => dto.field_Yyg67__c, opt => opt.MapFrom(info => info.planTime))
            ;


            CreateMap<CRMProEntity, CRMEntityPushPro3>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
            .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
            .ForMember(dto => dto.field_4iY04__c, opt => opt.MapFrom(info => info.proOrderNo))
             .ForMember(dto => dto.field_yAnwj__c, opt => opt.MapFrom(info => info.taskName))
             .ForMember(dto => dto.field_s9KMk__c, opt => opt.MapFrom(info => info.updateUser))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
             .ForMember(dto => dto.field_6mobW__c, opt => opt.MapFrom(info => info.proState))
              .ForMember(dto => dto.field_1yTW1__c, opt => opt.MapFrom(info => info.planTime))

            ;


            CreateMap<CRMProEntity, CRMEntityPushPro4>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
            .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
            .ForMember(dto => dto.field_8ktK8__c, opt => opt.MapFrom(info => info.proOrderNo))
             .ForMember(dto => dto.field_bv6S7__c, opt => opt.MapFrom(info => info.taskName))
            .ForMember(dto => dto.field_d2AW2__c, opt => opt.MapFrom(info => info.updateUser))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
             .ForMember(dto => dto.field_9am1y__c, opt => opt.MapFrom(info => info.proState))
               .ForMember(dto => dto.field_4xcUU__c, opt => opt.MapFrom(info => info.planTime));

            CreateMap<CRMProEntity, CRMEntityPushPro5>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
            .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
            .ForMember(dto => dto.field_ewHJQ__c, opt => opt.MapFrom(info => info.proOrderNo))
             .ForMember(dto => dto.field_5iPu6__c, opt => opt.MapFrom(info => info.taskName))
             .ForMember(dto => dto.field_1ouek__c, opt => opt.MapFrom(info => info.updateUser))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
             .ForMember(dto => dto.field_yJik8__c, opt => opt.MapFrom(info => info.proState))
              .ForMember(dto => dto.field_n6fUQ__c, opt => opt.MapFrom(info => info.planTime));

            CreateMap<CRMProEntity, CRMEntityPushPro6>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
            .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
            .ForMember(dto => dto.field_rEdHq__c, opt => opt.MapFrom(info => info.proOrderNo))
             .ForMember(dto => dto.field_6A7cc__c, opt => opt.MapFrom(info => info.taskName))
            .ForMember(dto => dto.field_69b3E__c, opt => opt.MapFrom(info => info.updateUser))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
             .ForMember(dto => dto.field_f2A0g__c, opt => opt.MapFrom(info => info.proState))
             .ForMember(dto => dto.field_1gETl__c, opt => opt.MapFrom(info => info.planTime));

            CreateMap<CRMProEntity, CRMEntityPushPro7>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
            .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
            .ForMember(dto => dto.field_loso1__c, opt => opt.MapFrom(info => info.proOrderNo))
             .ForMember(dto => dto.field_qKHGz__c, opt => opt.MapFrom(info => info.taskName))
             .ForMember(dto => dto.field_1wc9U__c, opt => opt.MapFrom(info => info.updateUser))
              .ForMember(dto => dto.field_4yUJU__c, opt => opt.MapFrom(info => info.crmState))
             .ForMember(dto => dto.field_e3d3W__c, opt => opt.MapFrom(info => info.proState))
             .ForMember(dto => dto.field_2oZ0S__c, opt => opt.MapFrom(info => info.planTime));
            #endregion 下推生产计划

       

            #region 上报生产状态
            CreateMap<CRMProTask, CRMEntityTask1>()
               //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
               .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                .ForMember(dto => dto.field_RnAI2__c, opt => opt.MapFrom(info => info.proTaskNo))
                   .ForMember(dto => dto.field_MLRho__c, opt => opt.MapFrom(info => info.taskName))
                .ForMember(dto => dto.field_B1wK3__c, opt => opt.MapFrom(info => info.startTime))
                .ForMember(dto => dto.field_orBwH__c, opt => opt.MapFrom(info => info.endTime))
                .ForMember(dto => dto.field_evz13__c, opt => opt.MapFrom(info => info.count))
                .ForMember(dto => dto.field_cmFG8__c, opt => opt.MapFrom(info => info.unit))
                .ForMember(dto => dto.field_w2lSP__c, opt => opt.MapFrom(info => info.updateUserID))
                ;

            CreateMap<CRMProTask, CRMEntityTask2>()
               //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                    .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                     .ForMember(dto => dto.field_goHWR__c, opt => opt.MapFrom(info => info.proTaskNo))
                   .ForMember(dto => dto.field_PUHit__c, opt => opt.MapFrom(info => info.taskName))
                    .ForMember(dto => dto.field_2xK4R__c, opt => opt.MapFrom(info => info.startTime))
                    .ForMember(dto => dto.field_8hUjG__c, opt => opt.MapFrom(info => info.endTime))
                    .ForMember(dto => dto.field_31flP__c, opt => opt.MapFrom(info => info.count))
                    .ForMember(dto => dto.field_7gsp0__c, opt => opt.MapFrom(info => info.unit))
                    .ForMember(dto => dto.field_rb734__c, opt => opt.MapFrom(info => info.updateUserID));


            CreateMap<CRMProTask, CRMEntityTask3>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                  .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                   .ForMember(dto => dto.field_4iY04__c, opt => opt.MapFrom(info => info.proTaskNo))
            .ForMember(dto => dto.field_yAnwj__c, opt => opt.MapFrom(info => info.taskName))
                    .ForMember(dto => dto.field_mzspU__c, opt => opt.MapFrom(info => info.startTime))
                    .ForMember(dto => dto.field_l8aa2__c, opt => opt.MapFrom(info => info.endTime))
                    .ForMember(dto => dto.field_7nsNU__c, opt => opt.MapFrom(info => info.count))
                    .ForMember(dto => dto.field_dTK9q__c, opt => opt.MapFrom(info => info.unit))
                    .ForMember(dto => dto.field_s9KMk__c, opt => opt.MapFrom(info => info.updateUserID))
                    ;

            CreateMap<CRMProTask, CRMEntityTask4>()
              //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                    .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                    .ForMember(dto => dto.field_8ktK8__c, opt => opt.MapFrom(info => info.proTaskNo))
                    .ForMember(dto => dto.field_bv6S7__c, opt => opt.MapFrom(info => info.taskName))
                    .ForMember(dto => dto.field_614ed__c, opt => opt.MapFrom(info => info.startTime))
                    .ForMember(dto => dto.field_40rQb__c, opt => opt.MapFrom(info => info.endTime))
                    .ForMember(dto => dto.field_r67M2__c, opt => opt.MapFrom(info => info.count))
                    .ForMember(dto => dto.field_A2i6w__c, opt => opt.MapFrom(info => info.unit))
                    .ForMember(dto => dto.field_d2AW2__c, opt => opt.MapFrom(info => info.updateUserID))
                    ;

            CreateMap<CRMProTask, CRMEntityTask5>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                    .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                    .ForMember(dto => dto.field_ewHJQ__c, opt => opt.MapFrom(info => info.proTaskNo))
                    .ForMember(dto => dto.field_5iPu6__c, opt => opt.MapFrom(info => info.taskName))
                    .ForMember(dto => dto.field_m1OPt__c, opt => opt.MapFrom(info => info.startTime))
                    .ForMember(dto => dto.field_1rL3W__c, opt => opt.MapFrom(info => info.endTime))
                    .ForMember(dto => dto.field_f117d__c, opt => opt.MapFrom(info => info.count))
                    .ForMember(dto => dto.field_75GaJ__c, opt => opt.MapFrom(info => info.unit))
                    .ForMember(dto => dto.field_1ouek__c, opt => opt.MapFrom(info => info.updateUserID))
                  ;

            CreateMap<CRMProTask, CRMEntityTask6>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                    .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                    .ForMember(dto => dto.field_rEdHq__c, opt => opt.MapFrom(info => info.proTaskNo))
                    .ForMember(dto => dto.field_6A7cc__c, opt => opt.MapFrom(info => info.taskName))
                    .ForMember(dto => dto.field_abRIo__c, opt => opt.MapFrom(info => info.startTime))
                    .ForMember(dto => dto.field_2F5zV__c, opt => opt.MapFrom(info => info.endTime))
                    .ForMember(dto => dto.field_S24fz__c, opt => opt.MapFrom(info => info.count))
                    .ForMember(dto => dto.field_rHKfQ__c, opt => opt.MapFrom(info => info.unit))
                    .ForMember(dto => dto.field_69b3E__c, opt => opt.MapFrom(info => info.updateUserID))
                  ;

            CreateMap<CRMProTask, CRMEntityTask7>()
            //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                    .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                    .ForMember(dto => dto.field_loso1__c, opt => opt.MapFrom(info => info.proTaskNo))
                    .ForMember(dto => dto.field_qKHGz__c, opt => opt.MapFrom(info => info.taskName))
                    .ForMember(dto => dto.field_s0ryw__c, opt => opt.MapFrom(info => info.startTime))
                    .ForMember(dto => dto.field_spgPb__c, opt => opt.MapFrom(info => info.endTime))
                    .ForMember(dto => dto.field_1tqvO__c, opt => opt.MapFrom(info => info.count))
                    .ForMember(dto => dto.field_0Uw1j__c, opt => opt.MapFrom(info => info.unit))
                    .ForMember(dto => dto.field_1wc9U__c, opt => opt.MapFrom(info => info.updateUserID))
                  ;

            #endregion 上报生产状态

            #region 删除生产计划
            //产量、生产单位、开始时间、结束时间
            CreateMap<CRMProTask, CRMEntityDeletePro1>()
                .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                .ForMember(dto => dto.field_RnAI2__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_MLRho__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_B1wK3__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_orBwH__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_evz13__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_cmFG8__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_w2lSP__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_070o1__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_xqV0O__c, opt => opt.Ignore())
                ;

            //产量、生产单位、开始时间、结束时间
            CreateMap<CRMProTask, CRMEntityDeletePro2>()
                 .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                .ForMember(dto => dto.field_goHWR__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_PUHit__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_2xK4R__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_8hUjG__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_31flP__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_7gsp0__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_rb734__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_zPc3r__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_Yyg67__c, opt => opt.Ignore())
                ;

            //产量、生产单位、开始时间、结束时间
            CreateMap<CRMProTask, CRMEntityDeletePro3>()
                 .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                .ForMember(dto => dto.field_4iY04__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_yAnwj__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_mzspU__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_l8aa2__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_7nsNU__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_dTK9q__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_s9KMk__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_6mobW__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_1yTW1__c, opt => opt.Ignore())
                ;

            //产量、生产单位、开始时间、结束时间
            CreateMap<CRMProTask, CRMEntityDeletePro4>()
                 .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                .ForMember(dto => dto.field_8ktK8__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_bv6S7__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_614ed__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_40rQb__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_r67M2__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_A2i6w__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_d2AW2__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_9am1y__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_4xcUU__c, opt => opt.Ignore())
                ;


            //产量、生产单位、开始时间、结束时间
            CreateMap<CRMProTask, CRMEntityDeletePro5>()
                 .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                .ForMember(dto => dto.field_ewHJQ__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_5iPu6__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_m1OPt__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_1rL3W__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_f117d__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_75GaJ__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_1ouek__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_yJik8__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_n6fUQ__c, opt => opt.Ignore())
                ;

            //产量、生产单位、开始时间、结束时间
            CreateMap<CRMProTask, CRMEntityDeletePro6>()
                 .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                .ForMember(dto => dto.field_rEdHq__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_6A7cc__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_abRIo__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_2F5zV__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_S24fz__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_rHKfQ__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_69b3E__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_f2A0g__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_1gETl__c, opt => opt.Ignore())
                ;
            //产量、生产单位、开始时间、结束时间
            CreateMap<CRMProTask, CRMEntityDeletePro7>()
                 .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                .ForMember(dto => dto.field_loso1__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_qKHGz__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_s0ryw__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_spgPb__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_1tqvO__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_0Uw1j__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_1wc9U__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_e3d3W__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_2oZ0S__c, opt => opt.Ignore())
                ;
            #endregion   删除生产计划

            #region 删除排产计划
            CreateMap<CRMApplyState, CRMEntityEmpty>()
                  //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                  .ForMember(dto => dto._id, opt => opt.MapFrom(info => info.crm_ID))
                   .ForMember(dto => dto.field_t1iXR__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_4yUJU__c , opt => opt.MapFrom(info => info.pcState))
                    .ForMember(dto => dto.field_I54DT__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_0h244__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_pl8w1__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_r5Tqq__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_p5sN5__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_Vm3lG__c , opt => opt.Ignore())
                    //任务1
                    .ForMember(dto => dto.field_RnAI2__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_MLRho__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_B1wK3__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_orBwH__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_evz13__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_cmFG8__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_w2lSP__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_0zFL1__c, opt => opt.Ignore())
                    .ForMember(dto => dto.field_070o1__c, opt => opt.Ignore())
                    .ForMember(dto => dto.field_xqV0O__c, opt => opt.Ignore())
                    //任务2
                    .ForMember(dto => dto.field_goHWR__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_PUHit__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_2xK4R__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_8hUjG__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_31flP__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_7gsp0__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_rb734__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_70cFm__c, opt => opt.Ignore())
                    .ForMember(dto => dto.field_zPc3r__c, opt => opt.Ignore())
                    .ForMember(dto => dto.field_Yyg67__c, opt => opt.Ignore())
                    //任务3
                    .ForMember(dto => dto.field_4iY04__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_yAnwj__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_mzspU__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_l8aa2__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_7nsNU__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_dTK9q__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_s9KMk__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_hbTdR__c, opt => opt.Ignore())
                     .ForMember(dto => dto.field_6mobW__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_1yTW1__c, opt => opt.Ignore())
                    //任务4
                    .ForMember(dto => dto.field_8ktK8__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_bv6S7__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_614ed__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_40rQb__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_r67M2__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_A2i6w__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_d2AW2__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_g4z0y__c, opt => opt.Ignore())
                    .ForMember(dto => dto.field_9am1y__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_4xcUU__c, opt => opt.Ignore())
                    //任务5
                    .ForMember(dto => dto.field_ewHJQ__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_5iPu6__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_m1OPt__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_1rL3W__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_f117d__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_75GaJ__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_1ouek__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_UE26m__c, opt => opt.Ignore())
                     .ForMember(dto => dto.field_yJik8__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_n6fUQ__c, opt => opt.Ignore())
                    //任务6
                    .ForMember(dto => dto.field_rEdHq__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_6A7cc__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_abRIo__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_2F5zV__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_S24fz__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_rHKfQ__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_69b3E__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_cg0ac__c, opt => opt.Ignore())
                      .ForMember(dto => dto.field_f2A0g__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_1gETl__c, opt => opt.Ignore())
                    //任务7
                    .ForMember(dto => dto.field_loso1__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_qKHGz__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_s0ryw__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_spgPb__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_1tqvO__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_0Uw1j__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_1wc9U__c , opt => opt.Ignore())
                    .ForMember(dto => dto.field_t64Wa__c, opt => opt.Ignore())
                     .ForMember(dto => dto.field_e3d3W__c, opt => opt.Ignore())
                .ForMember(dto => dto.field_2oZ0S__c, opt => opt.Ignore())
                  ;
            #endregion  删除排产计划
        }
    }
}

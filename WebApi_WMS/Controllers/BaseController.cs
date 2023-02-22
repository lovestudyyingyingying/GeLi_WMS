
using System.Web;
using System.Web.Http;
using System.Runtime.Remoting.Messaging;
using WebApi_WMS.Models;
using WebApi_WMS.Utils;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Managers;
using System.Collections.Generic;
using System.Reflection;
using System;
using GeLi_Utils.Services.WMS;
using GeLiData_WMS;
using GeLiService_WMS.Utils.RedisUtils;

namespace WebApi_WMS.Controllers
{
    public class BaseController : ApiController
    {
        //public static GeLiGuoRen_WMSEntities1 DB2
        //{
        //    get
        //    {
        //        string keyName = "__GeLiGuoRen_WMSEntities";
        //        HttpContext context = HttpContext.Current;
        //        if (context == null)
        //        {
        //            context = HttpRuntime.Cache.Get("context") as HttpContext;
        //        }
        //        // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
        //        if (!context.Items.Contains(keyName))
        //        {
        //            context.Items[keyName] = new GeLiGuoRen_WMSEntities1();
        //        }
        //        return context.Items[keyName] as GeLiGuoRen_WMSEntities1;
        //    }
        //}
        protected WareAreaClassService wareAreaClassService
        {
            get
            {
                string keyName = "__GeLiChiJi_WareAreaClassService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new WareAreaClassService();
                }
                return context.Items[keyName] as WareAreaClassService;
            }
        }
        protected RedisHelper redisHelper
        {
            get
            {
                string keyName = "__GeLiChiJi_RedisHelper";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new RedisHelper();
                }
                return context.Items[keyName] as RedisHelper;
            }
        }

        protected MovestockManager movestockManager
        {
            get
            {
                string keyName = "__GeLiChiJi_MovestockManager";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new MovestockManager(agvMissionService, liuShuiHaoService,
                        WareLocationService,WareLocationTrayManager,trayStateService,maPanJiInfoService, maPanJiStateService);
                }
                return context.Items[keyName] as MovestockManager;
            }
        }

        protected AGVAlarmLogService aGVAlarmLogService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_AGVAlarmLogService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new AGVAlarmLogService();
                }
                return context.Items[keyName] as AGVAlarmLogService;
            }
        }

        protected MaPanJiStateService maPanJiStateService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_MaPanJiStateService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new MaPanJiStateService();
                }
                return context.Items[keyName] as MaPanJiStateService;
            }
        }

        protected MaPanJiInfoService maPanJiInfoService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_MaPanJiInfoService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new MaPanJiInfoService();
                }
                return context.Items[keyName] as MaPanJiInfoService;
            }
        }
       
        protected LiuShuiHaoService liuShuiHaoService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_LiuShuiHaoService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new LiuShuiHaoService();
                }
                return context.Items[keyName] as LiuShuiHaoService;
            }
        }
        protected AGVMissionService agvMissionService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_AGVMissionService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new AGVMissionService();
                }
                return context.Items[keyName] as AGVMissionService;
            }
        }
       
        protected WareLocationLockHisService wareLocationLockHisService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_WareLocationLockHisService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new WareLocationLockHisService();
                }
                return context.Items[keyName] as WareLocationLockHisService;
            }
        }
        protected TrayStateService trayStateService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_TrayStateService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new TrayStateService();
                }
                return context.Items[keyName] as TrayStateService;
            }
        }

        public static UserService UserService
        {
            get
            {
                string keyName = "__GeLiAPI_UserService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = CallContext.GetData("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new UserService();
                }
                return context.Items[keyName] as UserService;
            }
        }
        public static BaseService BaseService
        {
            get
            {
                string keyName = "__GeLiAPI_BaseService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new BaseService();
                }
                return context.Items[keyName] as BaseService;
            }
        }
        public static AGVRunModelService AGVRunModelService
        {
            get
            {
                string keyName = "__GeLiAPI_AGVRunModelService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new AGVRunModelService();
                }
                return context.Items[keyName] as AGVRunModelService;
            }
        }

        public static AGVMissionService MissionService
        {
            get
            {
                string keyName = "__GeLiAPI_AGVMissionService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get(keyName) as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new AGVMissionService();
                }
                return context.Items[keyName] as AGVMissionService;
            }
        }
        public static AGVMissionFloorService FloorService
        {
            get
            {
                string keyName = "__GeLiAPI_AGVMissionFloorService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new AGVMissionFloorService();
                }
                return context.Items[keyName] as AGVMissionFloorService;
            }
        }
        public static TrayStateService TrayStateService
        {
            get
            {
                string keyName = "__GeLiAPI_TrayStateService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new TrayStateService();
                }
                return context.Items[keyName] as TrayStateService;
            }
        }
        public static WareLocationService WareLocationService
        {
            get
            {
                string keyName = "__GeLiAPI_WareLocationService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new WareLocationService();
                }
                return context.Items[keyName] as WareLocationService;
            }
        }
        public static WareLocationLockHisService WareLocationLockHisService
        {
            get
            {
                string keyName = "__GeLiAPI_WareLocationLockHisService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new WareLocationLockHisService();
                }
                return context.Items[keyName] as WareLocationLockHisService;
            }
        }

        public static TiShengJiInfoService TiShengJiInfoService
        {
            get
            {
                string keyName = "__GeLiAPI_TiShengJiInfoService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new TiShengJiInfoService();
                }
                return context.Items[keyName] as TiShengJiInfoService;
            }
        }

        public static StockRecordService StockRecordService
        {
            get
            {
                string keyName = "__GeLiAPI_StockRecordService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new StockRecordService();
                }
                return context.Items[keyName] as StockRecordService;
            }
        }

        public static WareLocationTrayManager WareLocationTrayManager
        {
            get
            {
                string keyName = "__GeLiAPI_WareLocationTrayManager";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new WareLocationTrayManager(
                    TrayStateService, WareLocationService, WareLocationLockHisService);
                }
                return context.Items[keyName] as WareLocationTrayManager;
            }
        }

        public static AGVApiManager AGVApiManager
        {
            get
            {
                string keyName = "__GeLiAPI_AGVApiManager";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new AGVApiManager(MissionService, FloorService,
                    TrayStateService, WareLocationService, DeviceStatesService, AlarmLogService
                    , AGVRunModelService, WareLocationLockHisService, TiShengJiInfoService, StockRecordService, WareLocationTrayManager);
                }
                return context.Items[keyName] as AGVApiManager;
            }
        }

        protected WareHouseService wareHouseService
        {
            get
            {
                string keyName = "__GeLiAPI_WareHouseService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new WareHouseService();
                }
                return context.Items[keyName] as WareHouseService;
            }
        }

        public static DeviceStatesService DeviceStatesService
        {
            get
            {
                string keyName = "__GeLiAPI_DeviceStatesService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new DeviceStatesService();
                }
                return context.Items[keyName] as DeviceStatesService;
            }
        }
        public static AGVAlarmLogService AlarmLogService
        {
            get
            {
                string keyName = "__GeLiAPI_AGVAlarmLogService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                }
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new AGVAlarmLogService();
                }
                return context.Items[keyName] as AGVAlarmLogService;
            }
        }

        protected WareAreaService wareAreaService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_WareAreaService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new WareAreaService();
                }
                return context.Items[keyName] as WareAreaService;
            }
        }


    }


    //#region CRM
    //public static ProPlanOrderheaderService ProPlanOrderheaderService
    //{
    //    get
    //    {
    //        string keyName = "__GeLiAPI_ProPlanOrderheaderService";
    //        HttpContext context = HttpContext.Current;
    //        if (context == null)
    //        {
    //            context = HttpRuntime.Cache.Get("context") as HttpContext;
    //        }
    //        // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
    //        if (!context.Items.Contains(keyName))
    //        {
    //            context.Items[keyName] = new ProPlanOrderheaderService();
    //        }
    //        return context.Items[keyName] as ProPlanOrderheaderService;
    //    }
    //}

    //public static ProPlanOrderlistsService ProPlanOrderlistsService
    //{
    //    get
    //    {
    //        string keyName = "__GeLiAPI_ProPlanOrderlistsService";
    //        HttpContext context = HttpContext.Current;
    //        if (context == null)
    //        {
    //            context = HttpRuntime.Cache.Get("context") as HttpContext;
    //        }
    //        // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
    //        if (!context.Items.Contains(keyName))
    //        {
    //            context.Items[keyName] = new ProPlanOrderlistsService();
    //        }
    //        return context.Items[keyName] as ProPlanOrderlistsService;
    //    }
    //}
    //protected static CRMPlanHeadService crmPlanHeadService
    //{
    //    get
    //    {
    //        string keyName = "__GeLiAPI_CRMPlanHeadService";
    //        HttpContext context = HttpContext.Current;
    //        if (context == null)
    //        {
    //            context = HttpRuntime.Cache.Get("context") as HttpContext;
    //        }
    //        //http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
    //        if (!HttpContext.Current.Items.Contains(keyName))
    //        {
    //            CRMPlanHeadService temp = new CRMPlanHeadService();

    //            context.Items[keyName] = temp;
    //        }
    //        return HttpContext.Current.Items[keyName] as CRMPlanHeadService;
    //    }
    //}
    //protected static CRMPlanListService crmPlanListService
    //{
    //    get
    //    {
    //        string keyName = "__GeLiAPI_CRMPlanListService";
    //        HttpContext context = HttpContext.Current;
    //        if (context == null)
    //        {
    //            context = HttpRuntime.Cache.Get("context") as HttpContext;
    //        }
    //        //http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
    //        if (!HttpContext.Current.Items.Contains(keyName))
    //        {
    //            CRMPlanListService temp = new CRMPlanListService();

    //            context.Items[keyName] = temp;
    //        }
    //        return HttpContext.Current.Items[keyName] as CRMPlanListService;
    //    }
    //}
    //protected static CRMFilesService crmFilesService
    //{
    //    get
    //    {
    //        string keyName = "__GeLiAPI_CRMFilesService";
    //        HttpContext context = HttpContext.Current;
    //        if (context == null)
    //        {
    //            context = HttpRuntime.Cache.Get("context") as HttpContext;
    //        }
    //        //http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
    //        if (!HttpContext.Current.Items.Contains(keyName))
    //        {
    //            CRMFilesService temp = new CRMFilesService();

    //            context.Items[keyName] = temp;
    //        }
    //        return HttpContext.Current.Items[keyName] as CRMFilesService;
    //    }
    //}
    //public static ProPlanOrderManager ProPlanOrderManager
    //{
    //    get
    //    {
    //        string keyName = "__GeLiAPI_ProPlanOrderManager";
    //        HttpContext context = HttpContext.Current;
    //        if (context == null)
    //        {
    //            context = HttpRuntime.Cache.Get("context") as HttpContext;
    //        }
    //        // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
    //        if (!context.Items.Contains(keyName))
    //        {
    //            context.Items[keyName] = new ProPlanOrderManager(
    //                ProPlanOrderheaderService, ProPlanOrderlistsService, crmPlanListService);
    //        }
    //        return context.Items[keyName] as ProPlanOrderManager;
    //    }
    //}

    //protected static ProductOrderheadersService ProductOrderheadersService
    //{
    //    get
    //    {
    //        string keyName = "__GeLiAPI_ProductOrderheadersService";
    //        //http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
    //        if (!HttpContext.Current.Items.Contains(keyName))
    //        {
    //            ProductOrderheadersService temp = new ProductOrderheadersService();

    //            HttpContext.Current.Items[keyName] = temp;
    //        }
    //        return HttpContext.Current.Items[keyName] as ProductOrderheadersService;
    //    }
    //}

    //protected static ProductOrderlistsService ProductOrderlistsService
    //{
    //    get
    //    {
    //        string keyName = "__GeLiAPI_ProductOrderlistsService";
    //        //http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
    //        if (!HttpContext.Current.Items.Contains(keyName))
    //        {
    //            ProductOrderlistsService temp = new ProductOrderlistsService();

    //            HttpContext.Current.Items[keyName] = temp;
    //        }
    //        return HttpContext.Current.Items[keyName] as ProductOrderlistsService;
    //    }
    //}
    //public static ProductOrderManager ProductOrderManager
    //{
    //    get
    //    {
    //        string keyName = "__GeLiAPI_ProductOrderManager";
    //        HttpContext context = HttpContext.Current;
    //        if (context == null)
    //        {
    //            context = HttpRuntime.Cache.Get("context") as HttpContext;
    //        }
    //        // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
    //        if (!context.Items.Contains(keyName))
    //        {
    //            context.Items[keyName] = new ProductOrderManager(
    //                ProductOrderheadersService, ProductOrderlistsService, ProPlanOrderlistsService);
    //        }
    //        return context.Items[keyName] as ProductOrderManager;
    //    }
    //}

    //public static CRMPlanManager crmPlanManager
    //{
    //    get
    //    {
    //        string keyName = "__GeLiAPI_CRMPlanService";
    //        HttpContext context = HttpContext.Current;
    //        if (context == null)
    //        {
    //            context = HttpRuntime.Cache.Get("context") as HttpContext;
    //        }
    //        // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
    //        if (!context.Items.Contains(keyName))
    //        {
    //            context.Items[keyName] = new CRMPlanManager
    //                (crmPlanHeadService, crmPlanListService, ProPlanOrderlistsService, crmFilesService);
    //        }
    //        return context.Items[keyName] as CRMPlanManager;
    //    }
    //}

    //#endregion
}

using GeLiService_WMS.Managers;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
using GeLiService_WMS.Services.WMS.AGV;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace JWTWebApiNet.Controllers
{
    public class BaseApiController : ApiController
    {
        public virtual void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write("success");
        }

        public virtual bool IsReusable
        {
            get
            {
                return false;
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
        protected WareHouseService wareHouseService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_WareHouseService";
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
        protected WareLocationService wareLocationService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_WareLocationService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new WareLocationService();
                }
                return context.Items[keyName] as WareLocationService;
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

        protected AGVMissionFloorService agvMissionFloorService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_AGVMissionFloorService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new AGVMissionFloorService();
                }
                return context.Items[keyName] as AGVMissionFloorService;
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
        protected StockPlanService stockPlanService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_StockPlanService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new StockPlanService();
                }
                return context.Items[keyName] as StockPlanService;
            }
        }

        protected UserService userService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_UserService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new UserService();
                }
                return context.Items[keyName] as UserService;
            }
        }
        protected StockRecordService StockRecordService
        {
            get
            {
                string keyName = "__GeLiShouChiJi_StockRecordService";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new StockRecordService();
                }
                return context.Items[keyName] as StockRecordService;
            }
        }


        protected InstockManager instockManager
        {
            get
            {
                string keyName = "__GeLiShouChiJi_InstockManager";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new InstockManager(trayStateService, wareLocationService,
                        liuShuiHaoService, agvMissionService, wareLocationLockHisService);
                }
                return context.Items[keyName] as InstockManager;
            }
        }


        protected OutstockManager outstockManager
        {
            get
            {
                string keyName = "__GeLiShouChiJi_OutstockManager";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new OutstockManager(wareLocationService, stockPlanService
                            , liuShuiHaoService, agvMissionService, wareLocationLockHisService, StockRecordService);
                }
                return context.Items[keyName] as OutstockManager;
            }
        }
        protected MovestockManager movestockManager
        {
            get
            {
                string keyName = "__GeLiShouChiJi_MovestockManager";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new MovestockManager(agvMissionService, liuShuiHaoService,
                        wareLocationService);
                }
                return context.Items[keyName] as MovestockManager;
            }
        }

        protected WareLocationTrayManager WareLocationTrayManager
        {
            get
            {
                string keyName = "__GeLiShouChiJi_WareLocationTrayManager";
                HttpContext context = HttpContext.Current;
                if (context == null)
                    context = HttpRuntime.Cache.Get("context") as HttpContext;
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!context.Items.Contains(keyName))
                {
                    context.Items[keyName] = new WareLocationTrayManager(trayStateService, wareLocationService,
                        wareLocationLockHisService);
                }
                return context.Items[keyName] as WareLocationTrayManager;
            }
        }

        /// <summary>
        /// 从content里取出json数据，不能是数组
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Dic</returns>
        protected Dictionary<string, string> GetDicInJson(HttpContext context)
        {
            System.IO.Stream sm = context.Request.InputStream;
            int len = (int)sm.Length;
            byte[] inputByts = new byte[len];
            sm.Read(inputByts, 0, len);
            sm.Close();
            string data = Encoding.GetEncoding("utf-8").GetString(inputByts);
            Debug.WriteLine(data);
            Dictionary<string, string> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            return jsonDict;
        }
        //protected static GeLiGuoRen_WMSEntities4 DB2
        //{
        //    get
        //    {
        //        // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
        //        if (!HttpContext.Current.Items.Contains("__GeLiGuoRen_WMSEntities"))
        //        {
        //            HttpContext.Current.Items["__GeLiGuoRen_WMSEntities"] = new GeLiGuoRen_WMSEntities4();
        //        }
        //        return HttpContext.Current.Items["__GeLiGuoRen_WMSEntities"] as GeLiGuoRen_WMSEntities4;
        //    }
        //}

        /// <summary>
        /// 计算使用时间
        /// </summary>
        /// <param name="dt1">开始时间</param>
        /// <param name="dt2">结束时间</param>
        /// <returns>相隔的秒数</returns>
        public string ReckonSeconds(DateTime dt1, DateTime dt2)
        {
            TimeSpan ts = (dt2 - dt1).Duration();

            double second = 0;
            if (ts.Hours > 0)
            {
                second += ts.Hours * 3600;
            }
            if (ts.Minutes > 0)
            {
                second += ts.Minutes * 60;
            }
            second += ts.Seconds;
            second += (ts.Milliseconds * 0.001);

            return second.ToString();

        }
    }
}

using GeLiData_WMS;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Services;
using WebApi_WMS.Models.PDAModels;
using Newtonsoft.Json.Linq;
using UtilsSharp.Shared.Standard;
using GeLiData_WMSUtils;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS;
using GeLiCangKu;
using LevelType = GeLiService_WMS.LevelType;
using GeLiService_WMS.Services.WMS;
using Newtonsoft.Json;
using GeLi_Utils.Entity.PDAApiEntity;
using GeLiService_WMS.Utils.RedisUtils;
using Exceptionless.Logging;
using Microsoft.Extensions.Logging;
using System.Transactions;
using GeLi_Utils.Entity.StockEntity;
using GeLiData_WMS.Dao;
using GeLi_Utils.Services.WMS;

namespace WebApi_WMS.Controllers
{

    [RoutePrefix("api/PDA")]
    public class PDAController : BaseController
    {


        public PDAController()
        {

            //Console.WriteLine("SetContext");
        }

        #region 登录接口
        /// <summary>
        /// 手持机登录接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("login/DoLogin")]
        public object DoLogin([FromBody] DoLoginModels request)
        {
            try
            {
                Logger.Default.Process(new Log(LevelType.Debug, request.name + request.password));
                if (request == null)
                {
                    return new { success = false, message = "参数错误" };
                }
                string loginId = request.name;
                string passw = request.password;
                Logger.Default.Process(new Log("Info", "PDA_" + "DoLogin:" + $"用户{loginId}尝试登录"));
                Users user = UserService.GetByName(loginId);
                if (user == null)
                {
                    Logger.Default.Process(new Log("Info", "PDA_" + "DoLogin:" + $"用户{loginId}账号错误"));
                    return new { success = false, message = "账号错误" };

                }
                if (PasswordUtil.ComparePasswords(user.Password, passw))
                {
                    if (!Convert.ToBoolean(user.Enabled))
                    {
                        return new { success = false, message = "用户未启用请联系管理员" };
                    }
                    else
                    {
                        //log.Debug(String.Format("登录成功：用户“{0}”", user.Name));
                        // 登录成功
                        //logger.Info(String.Format("登录成功：用户“{0}”", user.Name));
                        Logger.Default.Process(new Log("Info", "PDA_" + "DoLogin:" + $"用户{user.Name}登录成功"));
                        return new { success = true, message = "成功" };

                    }
                }
                else
                {
                    return new { success = false, message = "失败" };
                }
            }
            catch (Exception ee)
            {
                Logger.Default.Process(new Log(LevelType.Error, "PDA_" + "DoLogin:" + ee.ToString()));

                return new { success = false, message = ee.Message };
            }
        }



        #endregion

        #region 下发AGV任务相关接口

        /// <summary>
        /// 调拨命令
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("moves/DiaoBoOrder")]
        public object DiaoBoOrder([FromBody] object request)
        {
            try
            {
                Logger.Default.Process(new Log(LevelType.Debug, "PDA_" + "DiaoBoOrder:" + request.ToString()));
                var dic = JObject.Parse(request.ToString());

                if (dic["endPo"] == null)
                    return new { success = false, message = "endPo不能为空" };
                if (dic["nowPre"] == null)
                    return new { success = false, message = "nowPre不能为空" };
                if (dic["startPo"] == null)
                    return new { success = false, message = "startPo不能为空" };
                if (dic["processName"] == null)
                    return new { success = false, message = "processName不能为空" };
                //string startPo = dic["startPo"];
                string endPo = dic["endPo"].ToString(); //终点
                string nowPre = dic["nowPre"].ToString(); //操作人
                string prosn = string.Empty;
                bool isPriority = dic["isPriority"] == null ? false : dic["isPriority"].ToString() == "1" ? true : false;
                if (dic["prosn"] != null)
                {

                    if (!string.IsNullOrEmpty(dic["prosn"].ToString()))
                    {
                        var pdaTray = JObject.Parse(dic["prosn"].ToString())
                        .ToObject<PdaTray>();
                        if (pdaTray.分厂订单批次 == null)
                            return new { success = false, message = "标签号不能为空" };
                        prosn = trayStateService.AddByPda(pdaTray);
                    }

                }
                string startPo = dic["startPo"].ToString();
                string processName = dic["processName"].ToString(); // 工序名
                //var entity = movestockManager.GetMissionType(processName);
                //if (entity == null)
                //    return new { success = false, message = "未知操作类型" };
                //var goodType = entity.goodType;
                //var moveType = entity.moveType;

                //下任务
                object result;

                //if (!entity.endArea.Contains("码盘"))
                if (processName == ProcessName.ZhangGuanKongTuoShangXian)
                    result = movestockManager.MoveIn(prosn, startPo, endPo, nowPre, string.Empty, string.Empty, GoodType.EmptyTray, processName, "上线");
                else if (processName == ProcessName.HongGanKongTuoXiaXian)
                    result = movestockManager.MoveIn(prosn, startPo, endPo, nowPre, string.Empty, string.Empty, GoodType.EmptyTray, processName, "下线");
                else if (processName == ProcessName.HanJieKongTuoShangXian)
                    result = movestockManager.MoveIn(prosn, startPo, endPo, nowPre, string.Empty, string.Empty, GoodType.EmptyTray, processName, "上线");
                else if (processName == ProcessName.HanJieDangBanShangXian)
                    result = movestockManager.MoveIn(prosn, startPo, endPo, nowPre, string.Empty, string.Empty, GoodType.EmptyTray, processName, "上线");
                else if (processName == ProcessName.ZhangGuanWuLiaoXiaXian && isPriority || processName == ProcessName.ChuiYangWuLiaoXiaXian && isPriority || processName == ProcessName.QieGeWuLiaoXiaXian && isPriority)
                    result = movestockManager.JumpQueue(prosn,startPo,endPo, nowPre);
                else if (processName == ProcessName.ZhangGuanWuLiaoXiaXian || processName == ProcessName.ChuiYangWuLiaoXiaXian || processName == ProcessName.QieGeWuLiaoXiaXian)

                {
                    string endPosition = movestockManager.SplitAreaToPosition(endPo);
                    result = movestockManager.MoveIn(prosn, startPo, endPosition, nowPre, string.Empty, string.Empty, GoodType.GoodTray, processName, "下线");

                }
                else
                    result = movestockManager.MoveToMaPanJi(startPo, endPo, nowPre, processName);
                //if (missionType == "下线")
                //    result = movestockManager.MoveOut(prosn, startPo, endPo, nowPre, position, string.Empty);

                return result;
            }
            catch (Exception ee)
            {
                Logger.Default.Process(new Log(LevelType.Error, "PDA_" + "DiaoBoOrder:" + ee.ToString()));
                return new { success = false, message = ee.Message };
            }
        }

        #region 可调度库位状态查询接口（起点终点）

        /// <summary>
        /// 根据工序名获取起点终点（区域或点位）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("instocks/GetInStartWL")]
        public object GetStartAndEndWareLocation([FromBody] object request)
        {
            try
            {
                Logger.Default.Process(new Log(LevelType.Debug, "PDA_" + "GetInStartWL:" + request.ToString()));
                var requestObj = JObject.Parse(request.ToString());


                if (requestObj["processName"] == null)
                    return new { success = false, message = "请输入操作名称" };

                var processName = requestObj["processName"].ToString();
                //if (requestObj["startRemark"] == null)
                //    return new { success = false, message = "请输入起点描述" };
                //if (requestObj["endRemark"] == null)
                //    return new { success = false, message = "请输入终点描述" };
                object result = null;
                //冷管还是热管
                var protype = requestObj["protype"].ToString();

                ProcessTypeParamService processTypeParamService = new ProcessTypeParamService();
                var processTypeParam = processTypeParamService.GetEntity(processName, protype);
                if (processTypeParam == null)
                    return new { success = false, message = "未能识别该工序名称" };

                if (processTypeParam.missionType == PDAGetAreaMissionType.PointToPoint)
                {
                    if(processTypeParam.processName==ProcessName.HongGanKongTuoXiaXian)
                        result = movestockManager.FindWearLocationStartAndEnd(processTypeParam.startPosition, processTypeParam.endPosition, processTypeParam.startArea, processTypeParam.endArea,true);
                    else
                    result = movestockManager.FindWearLocationStartAndEnd(processTypeParam.startPosition, processTypeParam.endPosition, processTypeParam.startArea, processTypeParam.endArea);
                }
                else  if (processTypeParam.missionType == PDAGetAreaMissionType.PointToArea)
                    result = movestockManager.FindStartPointAndEndArea(processTypeParam.startArea, processTypeParam.endRemark);



                //if (processName == ProcessName.ZhangGuanWuLiaoXiaXian)
                //{
                //    result = movestockManager.FindStartPointAndEndArea("胀管产线区", processName);
                //    //int positionID = Convert.ToInt32(position);
                //}
                //else if (processName == ProcessName.ZhangGuanKongTuoShangXian)
                //{
                //    if (protype == "冷管")
                //    {
                //        result = movestockManager.FindWearLocationStartAndEnd("格力1楼", "格力1楼", "冷胀烘空托缓存区", "胀管空托区");

                //    }
                //    else if (protype == "热管")
                //    {
                //        result = movestockManager.FindWearLocationStartAndEnd("格力1楼", "格力1楼", "热胀烘空托缓存区", "胀管空托区");
                //    }
                //}
                //else if (processName == ProcessName.HongGanKongTuoXiaXian)
                //{
                //    if (protype == "冷管")
                //    {
                //        result = movestockManager.FindWearLocationStartAndEnd("格力1楼", "格力1楼", "冷烘干空托区", "冷胀烘空托缓存区");

                //    }
                //    else if (protype == "热管")
                //    {
                //        result = movestockManager.FindWearLocationStartAndEnd("格力1楼", "格力1楼", "热烘干空托区", "热胀烘空托缓存区");
                //    }

                //}
                //else if (processName == ProcessName.ChuiYangWuLiaoXiaXian)
                //{
                //    if (protype == "冷管")
                //    {
                //        result = movestockManager.FindStartPointAndEndArea("冷吹氧产线区", "冷" + processName);

                //    }
                //    else if (protype == "热管")
                //    {
                //        result = movestockManager.FindStartPointAndEndArea("热吹氧产线区", "热" + processName);
                //    }
                //}
                //else if (processName == ProcessName.QieGeWuLiaoXiaXian)
                //{
                //    if (protype == "冷管")
                //    {
                //        result = movestockManager.FindStartPointAndEndArea("冷切割产线区", "冷" + processName);

                //    }
                //    else if (protype == "热管")
                //    {
                //        result = movestockManager.FindStartPointAndEndArea("热切割产线区", "热" + processName);
                //    }
                //}
                //else if (processName == ProcessName.HanJieKongTuoShangXian)
                //{
                //    if (protype == "冷管")
                //    {
                //        result = movestockManager.FindStartPointAndEndArea("冷焊接空托区", "冷" + processName);
                //    }
                //    else if (protype == "热管")
                //    {
                //        result = movestockManager.FindStartPointAndEndArea("热焊接空托区", "热" + processName);

                //    }

                //    //var startRemark= requestObj["startRemark"].ToString();
                //    //var endRemark = requestObj["endRemark"].ToString();

                //    //var entity = movestockManager.GetMissionType(processName,startRemark,endRemark);

                //    //if (entity == null)
                //    //    return new { success = false, message = "未能识别该操作" };
                //    //entity.protype = protype.ToString();
                //    //var result = movestockManager.FindWearLocationStartAndEnd(entity.startPosition, entity.endPosition, entity.startArea, entity.endArea, entity.protype, entity.missionType, entity.nextArea, entity.nextPosition);
                //    //int positionID = Convert.ToInt32(position);


                //}
                //else if (processName == ProcessName.HanJieDangBanShangXian)
                //{
                //    if (protype == "冷管")
                //    {
                //        result = movestockManager.FindStartPointAndEndArea("冷焊接挡板区", "冷" + processName);
                //    }
                //    else if (protype == "热管")
                //    {
                //        result = movestockManager.FindStartPointAndEndArea("热焊接挡板区", "热" + processName);

                //    }

                //}

                return result;
            }
            catch (Exception ee)
            {
                Logger.Default.Process(new Log(LevelType.Error, "PDA_" + "GetInStartWL:" + ee.ToString()));
                return new { success = false, message = ee.Message };

            }
        }

        #endregion

        #endregion

        #region 获取产品信息
        /// <summary>
        /// 获取产品信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("moves/GetProInfo")]
        public object GetProInfo([FromBody] object request)
        {

            Logger.Default.Process(new Log(LevelType.Debug, "PDA_" + "GetProInfo:" + request.ToString()));
            var jobject = JObject.Parse(request.ToString());
            string prosn = jobject["prosn"].ToString().Trim();
            //string startArea = jobject["startArea"].ToString();

            TrayState ts = trayStateService.GetByTrayNo(prosn.Trim());
            ts = (TrayState)trayStateService.ParseValue(ts, typeof(TrayState), false);
            if (ts.ID != 0)
            {

                Logger.Default.Process(new Log(LevelType.Info, ts.TrayNO));
                var result = new { success = true, message = "成功", data = ts };
                return result;
            }
            else
            {
                return new { success = false, message = "未找到该条码" };

            }
        }
        #endregion


        #region 报警接口
        /// <summary>
        /// 获取报警信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("warn/GetWarn")]
        public object GetWarn([FromBody] object request)
        {
            try
            {
                Logger.Default.Process(new Log(LevelType.Info, "PDA_" + "GetWarn:" + request.ToString()));
                var requestObj = JObject.Parse(request.ToString());
                string startWarnDateStr = requestObj["startWarnDate"].ToString();//拿到报警时间
                string endWarnDateStr = requestObj["endWarnDate"].ToString();//拿到报警时间

                DateTime startWarnDate = DateTime.Parse(startWarnDateStr);

                DateTime endWarnDate = DateTime.Parse(endWarnDateStr);

                var data = AGVApiManager.GetWarnInFor(startWarnDate, endWarnDate).Select(u =>
                 new { u.deviceName, u.alarmDate, u.alarmDesc });
                Logger.Default.Process(new Log(LevelType.Info, "PDA_" + "GetWarn:" + JsonConvert.SerializeObject(data)));
                return new
                {
                    success = true,
                    message = "成功",
                    data = data
                };
            }
            catch (Exception ee)
            {
                Logger.Default.Process(new Log(LevelType.Error, "PDA_" + "GetWarn:" + ee.ToString()));

                return new { success = false, message = ee.Message };
            }

        }


        /// <summary>
        /// 修改报警信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("warn/ChangeWarn")]
        public object ChangeWarn([FromBody] object request)
        {
            try
            {
                var requestObj = JObject.Parse(request.ToString());
                string message = requestObj["message"].ToString();
                var time = Convert.ToDateTime(requestObj["time"]);
                int id = Convert.ToInt32(requestObj["id"]);
                var alarmLog = AlarmLogService.FindById(id, DbMainSlave.Master);
                alarmLog.alarmDesc = message;
                alarmLog.alarmDate = time;
                AlarmLogService.Update(alarmLog);
                AlarmLogService.SaveChanges();
                return new
                {
                    success = true,
                    message = "成功",

                };
            }
            catch (Exception ee)
            {
                Logger.Default.Process(new Log(LevelType.Error, ee.ToString()));

                return new { success = false, message = ee.Message };
            }

        }
        #endregion

        #region 修改库位状态接口
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("location/ChangeWareLocation")]
        public object ChangeWareLocation([FromBody] object request)
        {
            try
            {
                Logger.Default.Process(new Log(LevelType.Debug, "PDA_" + "ChangeWareLocation:" + request.ToString()));
                PostWarelocation warelocation = new PostWarelocation();
                warelocation = JsonConvert.DeserializeObject<PostWarelocation>(request.ToString());

                if (warelocation == null)
                    return new { success = false, message = "参数错误" };

                return movestockManager.ChangeWareLocation(warelocation);


            }
            catch (Exception ee)
            {
                Logger.Default.Process(new Log(LevelType.Error, "PDA_" + "ChangeWareLocation:" + ee.ToString()));
                return new { success = false, message = ee.Message };
            }

        }


        /// <summary>
        /// 获取库位区域接口
        /// </summary>
        /// <param name = "request" ></ param >
        /// < returns ></ returns >
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("location/GetWareAreaClass")]
        public object GetWareAreaClass([FromBody] object request)
        {

            try
            {
                var wareAreaClassStr = wareAreaClassService.GetAllWareAreaClassName();
                return new { success = true, message = "成功", data = wareAreaClassStr };
            }
            catch (Exception ee)
            {
                Logger.Default.Process(new Log(LevelType.Error, ee.ToString()));
                return new { success = false, message = ee.Message };
            }
        }

        /// <summary>
        /// 按区域获取库位接口
        /// </summary>
        /// <param name = "request" ></ param >
        /// < returns ></ returns >
        [WebMethod]
        [AllowAnonymous]
        [HttpPost]
        [Route("location/GetWareLocationState")]
        public object GetWareLocationState([FromBody] object request)
        {

            try
            {
                var requestObj = JObject.Parse(request.ToString());
                if (requestObj["wareAreaClass"] == null)
                    return new { success = false, message = "请输入库位区域" };
                string wareAreaClass = requestObj["wareAreaClass"].ToString();
                var warelocationList = WareLocationService.GetByAreaClass(wareAreaClass);
                if (warelocationList == null)
                    return new { success = false, message = "未找到相关库位" };
                var dataresult = from a in warelocationList
                                 select new
                                 {
                                     name = a.WareLocaNo,
                                     state = a.WareLocaState,
                                     itemName = a.TrayState == null ? "" : a.TrayState.proname,
                                     trayNo = a.TrayState == null ? "" : a.TrayState.TrayNO,
                                 };
                return new { success = true, message = "成功", data = dataresult };
            }
            catch (Exception ee)
            {
                Logger.Default.Process(new Log(LevelType.Error, "PDA_" + "GetWareLocationState:" + ee.ToString()));
                return new { success = false, message = ee.Message };
            }
        }
        #endregion

    }
}

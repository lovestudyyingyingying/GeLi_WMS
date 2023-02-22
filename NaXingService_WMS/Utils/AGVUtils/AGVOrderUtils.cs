using NanXingData_WMS.Dao;
using NanXingService_WMS.Entity.AGVOrderEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.AGVUtils
{    
    /// <summary>
    /// AGV接口工具类
    /// </summary>
    public class AGVOrderUtils
    {
        HttpUtils httpUtils = new HttpUtils();
        string sendTaskURI = string.Empty;
        string getMissionStateURI = string.Empty;
        string cancelTaskURI = string.Empty;
        string getAGVStateURI = string.Empty;
        string continueTaskURI = string.Empty;

        
        public AGVOrderUtils(string ip)
        {
            //string ip = ConfigurationManager.AppSettings["AGVIP"];
            sendTaskURI = $"http://{ip}:8001/ics/taskOrder/addTask";
            getMissionStateURI= $"http://{ip}:7000/ics/out/task/getTaskOrderStatus";
            cancelTaskURI= $"http://{ip}:7000/ics/out/task/cancelTask";
            getAGVStateURI = $"http://{ip}:7000/ics/out/device/list/deviceInfo";
            continueTaskURI= $"http://{ip}:7000/ics/out/task/continueTask";
        }

        #region 发送指令

        MissionOrder mo = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mission">任务</param>
        public OrderResult SendOrder(AGVMissionInfo mission)
        {
            if (mo == null)
            {
                mo = new MissionOrder();
                //mo.modelProcessCode = mission.ModelProcessCode;
                mo.priority = "6";
                mo.fromSystem = "WMS";
                TaskOrderDetails taskOrderDetails = new TaskOrderDetails(
                    //mission.StartLocation.StartsWith("1") ?
                    //string.Format(pathFormat_1F, mission.StartLocation, mission.EndLocation):
                    mission.SendAGVPoStr
                    , string.Empty, string.Empty, string.Empty);
                List<TaskOrderDetails> list = new List<TaskOrderDetails>();
                list.Add(taskOrderDetails);
                mo.taskOrderDetail = list;
            }
            mo.modelProcessCode = mission.ModelProcessCode;
            mo.orderId = mission.MissionNo;
            mo.orderGroupId = mission.OrderGroupId;
            mo.taskOrderDetail[0].TaskPath =
            //mission.StartLocation.StartsWith("1") ?
            //        string.Format(pathFormat_1F, mission.StartLocation, mission.EndLocation) :
                     mission.SendAGVPoStr;

            return SendMission(mo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mission">任务</param>
        public OrderResult SendFloorOrder(AGVMissionInfo_Floor mission)
        {
            if (mo == null)
            {
                mo = new MissionOrder();
                mo.modelProcessCode = mission.ModelProcessCode;
                mo.priority = "6";
                mo.fromSystem = "WMS";
                TaskOrderDetails taskOrderDetails = new TaskOrderDetails(
                     //mission.StartLocation.StartsWith("1") ?
                     //string.Format(pathFormat_1F, mission.StartLocation, mission.EndLocation):
                     mission.SendAGVPoStr

                    , string.Empty, string.Empty, string.Empty);
                List<TaskOrderDetails> list = new List<TaskOrderDetails>();
                list.Add(taskOrderDetails);
                mo.taskOrderDetail = list;
            }
            mo.modelProcessCode = mission.ModelProcessCode;
            mo.orderId = mission.MissionNo;
            mo.orderGroupId = mission.OrderGroupId;
            mo.taskOrderDetail[0].TaskPath =
            //mission.StartLocation.StartsWith("1") ?
            //        string.Format(pathFormat_1F, mission.StartLocation, mission.EndLocation) :
                     mission.SendAGVPoStr;

            return SendMission(mo);
        }
        #endregion
        /// <summary>
        /// 发送AGV指令
        /// </summary>
        /// <param name="mission"></param>
        /// <returns>"{ \"code\": 1000, \"data\": \"3215435\",  \"desc\": \"请求成功\" }"</returns>
        private OrderResult SendMission(MissionOrder mission)
        {
            string json = JsonConvert.SerializeObject(mission);
            Logger.Default.Process(new Log(LevelType.Info, "发送AGV任务指令：" + sendTaskURI+
                "--指令数据:"+json));
            string result= httpUtils.HttpApi(sendTaskURI, json, HttpUtils.postType);
            return JsonConvert.DeserializeObject<OrderResult>(result);
        }
        /// <summary>
        /// 获取AGV中的任务状态
        /// </summary>
        /// <param name="orderId">任务ID</param>
        /// <returns>任务状态</returns>
        public MissionStates GetMissionState(string orderId)
        {
            OrderID obj = new OrderID(orderId);
            string json = JsonConvert.SerializeObject(obj);
            string result = httpUtils.HttpApi(getMissionStateURI, json, HttpUtils.postType);
            //result: {"code":1000,"data":{"areaId":1,"createTime":1629786327,"taskOrderDetail":[{"shelfNumber":"","time":1629787743,"status":9}],"fromSystem":"WMS","status":9},"desc":"success"}
            
            return JsonConvert.DeserializeObject<MissionStates>(result);
        }

        /// <summary>
        /// 发送取消AGV任务指令
        /// </summary>
        /// <param name="orderIds">多个AGV任务ID</param>
        /// <returns>"{ \"code\": 1000,  \"desc\": \"success\" }"</returns>
        public OrderResult CancelMission(string[] orderIds)
        {
            List<OrderID> list = new List<OrderID>(orderIds.Length);
            foreach (string temp in orderIds)
                list.Add(new OrderID(temp));
            string json = JsonConvert.SerializeObject(list);
            string result = httpUtils.HttpApi(cancelTaskURI, json, HttpUtils.postType);
            return JsonConvert.DeserializeObject<OrderResult>(result);
        }

        /// <summary>
        /// 主动获取AGV设备状态接口
        /// </summary>
        /// <param name="areaId">区域 Id</param>
        /// <param name="deviceType">0:agv</param>
        /// <param name="deviceCode">设备编号,不传查所有</param>
        /// <returns>AGV状态</returns>
        public AGVDeviceState GetAGVState(string areaId, string deviceType, string deviceCode)
        {
            string api = getAGVStateURI;
            string result = httpUtils.HttpApi(api,
                JsonConvert.SerializeObject(new { areaId = 1, deviceType = 0, deviceCode = deviceCode })
                , HttpUtils.postType);
            //result :{"code":1000,"data":[{"taskPath":"20000091,20000085","payLoad":"0.0","orderId":"3215439","shelfNum":"","devicePosition":"20000905","devicePostionRec":[-18415,-7765],"state":"InTask","deviceCode":"7E04B8DPA400003","battery":"84","deviceName":"0003","deviceStatus":4}],"desc":"success"}

            return JsonConvert.DeserializeObject<AGVDeviceState>(result);
        }

        /// <summary>
        /// 继续任务
        /// </summary>
        /// <param name="orderId">任务ID</param>
        public OrderResult ContinueTask(string orderId)
        {
            OrderID obj = new OrderID(orderId);
            string json = JsonConvert.SerializeObject(obj);
            string result = httpUtils.HttpApi(continueTaskURI, json, HttpUtils.postType);
            return JsonConvert.DeserializeObject<OrderResult>(result);
        }
    }
}

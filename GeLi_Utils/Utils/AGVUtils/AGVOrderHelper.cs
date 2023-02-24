using GeLi_Utils.Entity.AGVApiEntity;
using GeLiData_WMS;
using GeLiService_WMS;
using GeLiService_WMS.Entity.AGVOrderEntity;
using GeLiService_WMS.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpUtils = GeLiService_WMS.Utils.HttpUtils;
using OrderResult = GeLi_Utils.Entity.AGVApiEntity.OrderResult;

namespace GeLi_Utils.Utils.AGVUtils
{
    public class AGVOrderHelper
    {
        /// <summary>
        /// 基础ip   ip:port
        /// </summary>
        public string BaseUrl { get; set; }

       public  AGVOrderHelper(string baseUrl)
        {
            this.BaseUrl ="http://"+ baseUrl;
        }

        /// <summary>
        /// 发送任务指令
        /// </summary>
        /// <param name="mission"></param>
        /// <returns></returns>
        public OrderResult SendOrder(AGVMissionInfo mission)
        {
            var startPoint = mission.StartPosition;
            var endPoint = mission.EndLocation;
            List<string> targePoint = new List<string>() {startPoint,endPoint };
            return CreateTask(mission.MissionNo, targePoint,null);
        }

        /// <summary>
        /// 发送任务指令
        /// </summary>
        /// <param name="mission"></param>
        /// <returns></returns>
        public OrderResult SendFloorOrder(AGVMissionInfo_Floor mission)
        {
            Logger.Default.Process(new Log(LevelType.Error, "删除标签失败"));
            var startPoint = mission.StartLocation;
            var endPoint = mission.EndLocation;
            List<string> targePoint = new List<string>() { startPoint, endPoint };
            return CreateTask(mission.MissionNo, targePoint, null);
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="taskId">任务唯一标识</param>
        /// <param name="targetPoints">任务目标点</param>
        /// <param name="taskConfig">任务配置的id</param>
        /// <returns></returns>
        public OrderResult CreateTask(string taskId,List<string> targetPoints,int? taskConfig)
        {
            string url = BaseUrl + "/rpc/createTask";
            var requestParam = new
            {
                taskId = taskId,
                targetPoints = targetPoints,
                taskConfig = taskConfig
            };

            HttpUtils httpUtils = new HttpUtils();
            Logger.Default.Process(new Log(LevelType.Info, "发送AGV任务指令：" + requestParam.taskId +
                "--指令数据:" + JsonConvert.SerializeObject(requestParam)));
            var resultStr =  httpUtils.HttpPost(url, requestParam, null);

            if(string.IsNullOrEmpty(resultStr))
            {
                return null;
            }
         
               var  result = JsonConvert.DeserializeObject<OrderResult>(resultStr);
    
            return result;
             
        }

       /// <summary>
       /// 继续任务
       /// </summary>
       /// <param name="destText">正在执行等待任务的点位</param>
       /// <param name="taskId">任务唯一编号</param>
       /// <returns></returns>
        public OrderResult ContinueTask(string destText, string taskId=null)
        {
            string url = BaseUrl + "/rpc/continueTask";
            var requestParam = new
            {
                taskId = taskId,
                destText=destText,
            };

            HttpUtils httpUtils = new HttpUtils();
            Logger.Default.Process(new Log(LevelType.Info, "发送AGV继续指令编号：" + requestParam.taskId +
                "--指令数据:" + JsonConvert.SerializeObject(requestParam)));
            var resultStr = httpUtils.HttpPost(url, requestParam, null);

            if (string.IsNullOrEmpty(resultStr))
            {
                return null;
            }

            var result = JsonConvert.DeserializeObject<OrderResult>(resultStr);

            return result;
        }


        /// <summary>
        /// 取消任务(发送取消时，若设备无货，直接取消并释放设备；若设备有货，当传入targetPoint时，设备会把货放到指定的targetPoint，否则系统会把货放到原来的地方。)
        /// </summary>
        /// <param name="taskId">任务目标点</param>
        /// <param name="targetPoint">卸货目标点</param>
        /// <returns></returns>
        public OrderResult CancelTask(string taskId, string targetPoint = null)
        {
            string url = BaseUrl + "/rpc/cancelTask";
            var requestParam = new
            {
                taskId = taskId,
                targetPoint = targetPoint,
            };

            HttpUtils httpUtils = new HttpUtils();
            Logger.Default.Process(new Log(LevelType.Info, "发送AGV取消指令编号：" + requestParam.taskId +
                "--指令数据:" + JsonConvert.SerializeObject(requestParam)));
            var resultStr = httpUtils.HttpPost(url, requestParam, null);

            if (string.IsNullOrEmpty(resultStr))
            {
                return null;
            }

            var result = JsonConvert.DeserializeObject<OrderResult>(resultStr);

            return result;
        }





        /// <summary>
        /// AGV查询 (上层系统获取在线小车状态)
        /// </summary>
        /// <returns></returns>
        public AGVStateResult GetOnlineAgv()
        {
            string url = BaseUrl + "/rpc/getOnlineAgv";
            HttpUtils httpUtils = new HttpUtils();
            Logger.Default.Process(new Log(LevelType.Info, "获取AGV信息："));
            string resultStr =  httpUtils.GetHttpGet(url);
            return JsonConvert.DeserializeObject<AGVStateResult>(resultStr);
        }

        /// <summary>
        /// 任务查询 (上层系统获取任务状态)
        /// </summary>
        /// <returns></returns>
        public TaskStateResult TaskQuery(string taskId)
        {
            string url = BaseUrl + "/rpc/taskQuery";
            HttpUtils httpUtils = new HttpUtils();
            Logger.Default.Process(new Log(LevelType.Info, "查询任务编号："));
            var obj = new
            {
                taskId = taskId
            };
            string resultStr = httpUtils.GetHttpGet(url,obj);
            return JsonConvert.DeserializeObject<TaskStateResult>(resultStr);
        }

        /// <summary>
        /// 获得地图 (上层系统获取地图信息)
        /// </summary>
        /// <param name="mapId">地图编号</param>
        /// <returns></returns>
        public MapInfoResult GetMapInfo(string mapId=null)
        {
            string url = BaseUrl + "/rpc/getMapInfo";
            HttpUtils httpUtils = new HttpUtils();
            Logger.Default.Process(new Log(LevelType.Info, "查询任务编号："));
            var obj = new
            {
                mapId = mapId
            };
            string resultStr = httpUtils.GetHttpGet(url,obj);
            return JsonConvert.DeserializeObject<MapInfoResult>(resultStr);
        }

        /// <summary>
        /// 获取AGV故障信息查询
        /// </summary>
        /// <returns></returns>
        public AlarmOrderResult GetAgvAlarms()
        {
            string url = BaseUrl + "/rpc/getAgvAlarms";
            HttpUtils httpUtils = new HttpUtils();
            Logger.Default.Process(new Log(LevelType.Info, "发起查询AGV故障请求"));
            string resultStr =  httpUtils.GetHttpGet(url);
            return JsonConvert.DeserializeObject<AlarmOrderResult>(resultStr);
        }



    }
}

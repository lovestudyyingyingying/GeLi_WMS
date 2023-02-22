
using GeLiData_WMS;
using GeLiData_WMSImp;
using GeLiData_WMSUtils;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
using GeLiService_WMS.Threads.DiffFloorThreads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Helper.WMS
{
    public class AGVModelHelper
    {

        private AGVRunModelService _runModelService;
        AGVMissionService _agvMissionService;

        public AGVModelHelper(AGVRunModelService AGVRunModelService, AGVMissionService agvMissionService)
        {
            _runModelService = AGVRunModelService;
            _agvMissionService = agvMissionService;
        }

        /// <summary>
        /// 根据任务类型分配任务模板
        /// </summary>
        /// <param name="missionType"></param>
        public void ChangeModel(AGVMissionBase missionInfo,bool isSameFloor)
        {
            //三个筛选条件：楼层、提升机、搬进搬出
            string ModelName = string.Empty;
            string TsjName = string.Empty;
            string whHouse=string.Empty; 
            //string TsjName = string.Empty;
            Expression<Func<AGVRunModel,bool>> exp= DbBaseExpand.True<AGVRunModel>();
            //第一步：判断是否同楼层出入库
            //if (isSameFloor)
            //{
            //    //第一步：判断任务类型：物料上线、物料下线到产线、物料下线到缓存、进货去码盘机码盘、空托搬离码盘机
            //    if (missionInfo.Mark == MissionType.GoodOnline)
            //        ModelName = "物料上线";
            //    else if (missionInfo.Mark == MissionType.GoodOfflineInChanXian)
            //        ModelName = "物料下线到产线";
            //    else if (missionInfo.Mark == MissionType.GoodOfflineInHuanCun)
            //        ModelName = "物料下线到缓存";
            //    else if (missionInfo.Mark == MissionType.MoveToMaPanJi)
            //        ModelName = "进货去码盘机码盘";
            //    else if (missionInfo.Mark == MissionType.MoveOutMaPanJi)
            //        ModelName = "空托搬离码盘机";
            //    AGVMissionInfo missionInfo2 =missionInfo as AGVMissionInfo;
            //    exp = exp.And(u => u.AGVModelName == ModelName&& u.wareHouse.WHName== missionInfo2.WHName);
            //}
            //else
            //{
            //    //跨楼层任务
            //    //第一步：判断提升机
            //    //第二步：判断目的是搬进或搬出 提升机（任务-1为搬进提升机，-2为搬出提升机）
            //    if (missionInfo.MissionNo.EndsWith(DiffFloorFactory.oneStr))
            //    {
            //        ModelName = "搬进提升机";
            //        TsjName = missionInfo.EndPosition;
            //        if (missionInfo.StartLocation.StartsWith("1"))
            //            whHouse = "07一楼";
            //        else if (missionInfo.StartLocation.StartsWith("2"))
            //            whHouse = "07二楼";
            //    }
            //    else if (missionInfo.MissionNo.EndsWith(DiffFloorFactory.twoStr))
            //    {
            //        ModelName = "搬出提升机";
            //        TsjName = missionInfo.StartPosition;
            //        if (missionInfo.StartLocation.StartsWith("1"))
            //            whHouse = "07一楼";
            //        else if(missionInfo.StartLocation.StartsWith("2"))
            //            whHouse = "07二楼";
            //    }
                

            //    exp = exp.And(u => u.AGVModelName == ModelName&& u.TiShengJiInfo.TsjName==TsjName
            //    && u.wareHouse.WHName== whHouse);
            //}
            
            //var model=_runModelService.FirstOrDefault(exp,true,DbMainSlave.Master);

            //if (model == null)
            //{
            //    throw new Exception("找不到该模板");
            //}
            //missionInfo.ModelProcessCode = model.AGVModelCode;

           // string sendPath = model.SendOrderPath;
            //string msg = string.Empty;
            //msg = sendPath.Replace("{start}", missionInfo.StartLocation);
            //msg = msg.Replace("{startmiddle}", missionInfo.StartMiddleLocation);
            //msg = msg.Replace("{endmiddle}", missionInfo.EndMiddleLocation);
            //msg = msg.Replace("{end}", missionInfo.EndLocation);
            //missionInfo.SendAGVPoStr = msg;

            //string returnPath = model.ApiRuturnPath;
            //msg = string.Empty;
            //msg = returnPath.Replace("{start}", missionInfo.StartLocation);
            //msg = msg.Replace("{startmiddle}", missionInfo.StartMiddleLocation);
            //msg = msg.Replace("{endmiddle}", missionInfo.EndMiddleLocation);
            //msg = msg.Replace("{end}", missionInfo.EndLocation);
            //missionInfo.ApiReturnPoStr = msg;
            //Logger.Default.Process(new Log("Info", $"{missionInfo.ModelProcessCode }" +
            //    $"{missionInfo.SendAGVPoStr}{missionInfo.ApiReturnPoStr }"));
            //_agvMissionService.UpdateByPlus(u => u.ID==missionInfo.ID,
            //    u=>new AGVMissionInfo { 
            //        ModelProcessCode = ModelProcessCode,
            //        SendAGVPoStr= SendAGVPoStr,
            //        ApiReturnPoStr=ApiReturnPoStr,
            //        SendState = StockState.SendState_Group
            //});

        }
    }

    
    //public class AGVModel
    //{
    //    public string Model { get; set; }
    //}
}

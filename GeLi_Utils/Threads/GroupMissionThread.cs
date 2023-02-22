
using GeLiData_WMS;
using GeLiData_WMSUtils;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Helper.WMS;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Threads.DiffFloorThreads;
using GeLiService_WMS.Utils;
using GeLiService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeLiService_WMS.Threads
{
    /// <summary>
    /// AGV线程类，主要对AGVMission的数据进行分类，并调用对应线程进行处理
    /// </summary>
    public class GroupMissionThread
    {
        AGVMissionService agvMissionService = new AGVMissionService();
        TiShengJiInfoService tiShengJiInfoService = new TiShengJiInfoService();
        AGVMissionFloorService floorService = new AGVMissionFloorService();
        AGVRunModelService _agvRunModelService = new AGVRunModelService();
        ChooseTiShengJiHelper chooseTiShengJiHelper;
        AGVModelHelper modelHelper;
        RedisHelper redisHelper = new RedisHelper();
        public GroupMissionThread()
        {
            modelHelper = new AGVModelHelper(_agvRunModelService, agvMissionService);
            chooseTiShengJiHelper=new ChooseTiShengJiHelper(tiShengJiInfoService);
        }


        //string splitStr = "等待发送";
        /// <summary>
        /// 只管发送AGV指令
        /// </summary>
        public void Control()
        {
            
           
            //1、读取所有未分类的任务
            List<AGVMissionInfo> noSplitlist = agvMissionService.GetNoSplitMission();

            #region 同楼层任务
            //2、分发跨楼层与同楼层任务给不同的线程进行执行
            //3、同楼层直接发送AGV进行执行
            List<AGVMissionInfo> sameFloorMission = noSplitlist.Where(u => u.IsFloor == 0).ToList();
            DataTable dt = agvMissionService.ClassToDataTable(typeof(AGVMissionInfo));

            foreach (AGVMissionInfo temp in sameFloorMission)
            {
                //根据确定AGV模板，修改模板数据
               // modelHelper.ChangeModel(temp, true);
                temp.SendState = StockState.SendState_Group;
            }
            dt = agvMissionService.ConvertToDataTable(sameFloorMission);
            agvMissionService.UpdateMany(dt);

            #endregion

            #region 跨楼层任务
            //4、跨楼层划分成步骤任务存储在数据库与Redis中，另一个后台线程读取Redis发送命令
            List<AGVMissionInfo> differentFloorMission = noSplitlist.Where(u => u.IsFloor == 1).ToList();
            //另起线程进行分解及分配提升机，但是等待分解结束
            ParseFloor(differentFloorMission,dt);
            #endregion
        }

        public void ParseFloor(List<AGVMissionInfo> differentFloorMission,DataTable dt)
        {
            //分配提升机用队列进行判断，队列的数量、与队列的列名，
            //List<>
            //同一列的任务分配给同一个提升机
            //写入floor表，并回写AGVMission表SendState为已分类
            //获取所有提升机现有未完成的任务
            dt.Clear();
            
            foreach (AGVMissionInfo temp in differentFloorMission)
            {
                //string lieName = temp.EndLocation.Split('-')[0];
                ////第一步，判断是否有同列任务
                //TiShengJiInfo tiShengJiInfo = chooseTiShengJiHelper.ChooseTSJByHash(temp.EndLocation,
                //    lieName);
                TiShengJiInfo tiShengJiInfo = chooseTiShengJiHelper.ChooseTSJByCount(temp.MissionNo);
                if (tiShengJiInfo == null)
                {
                    Logger.Default.Process(new Log(LevelType.Info, $"{temp.MissionNo}没有能够分配的提升机"));
                    continue;
                }
                    
                //执行分配，
                Logger.Default.Process(new Log(LevelType.Info, $"分配提升机：{tiShengJiInfo.TsjName}"));
                MissionToTsj(tiShengJiInfo, temp);
                //Debug.WriteLine(tsjName);
                //并将上面的集合进行改变，方便下一个任务进行判断用
                
                temp.SendState = StockState.SendState_Group;
                temp.WHName = tiShengJiInfo.TsjName;
                
            }
            dt = agvMissionService.ConvertToDataTable(differentFloorMission);
            agvMissionService.UpdateMany(dt);
        }

        DataTable dTable = null;
        /// <summary>
        /// 任务分配提升机
        /// </summary>
        /// <param name="tsjInfo"></param>
        /// <param name="agvMission"></param>
        private void MissionToTsj(TiShengJiInfo tsjInfo,AGVMissionInfo agvMission)
        {

            //当进仓第二步骤成功才算成功进仓
            //当出仓第1步骤成功算出仓成功
            //所以
            //
            //进仓第二步骤为02、
            //
            //出仓\调拨第一步骤为03，
            //第二步骤为05

            //生成步骤1任务
            //第一步 ： 判断进出仓
            string Mark01 = string.Empty;
            string Mark02 = string.Empty;
            
            if(agvMission.Mark == MissionType.GoodOnline)
            {
                Mark01 = MissionType.MoveIn_TSJ;
                Mark02 = MissionType.MoveOut_TSJ;
            }
            //第一步 ： 根据楼层与提升机，判断模板与步骤1终点，步骤2起点
            string po01 = string.Empty;
            string po02 = string.Empty;

            if (agvMission.StartPosition.StartsWith("1"))
            {
                po01 = tsjInfo.TsjPosition_1F;//一楼提升机位置
            }
            else if (agvMission.StartPosition.StartsWith("2"))
            {
                po01 = tsjInfo.TsjPosition_2F; //二楼提升机位置
            }
           

            if (agvMission.EndPosition.StartsWith("1"))
            {
                po02 = tsjInfo.TsjPosition_1F;//一楼提升机位置
            }
            else if (agvMission.EndPosition.StartsWith("2"))
            {
                po02 = tsjInfo.TsjPosition_2F; //二楼提升机位置
            }
            

            //生成步骤1任务
            AGVMissionInfo_Floor floorMission = new AGVMissionInfo_Floor();
            floorMission.MissionNo = agvMission.MissionNo + DiffFloorFactory.oneStr;
            floorMission.OrderTime = DateTime.Now;
            floorMission.TrayNo = agvMission.TrayNo;
            floorMission.Mark = Mark01;
            floorMission.StartLocation = agvMission.StartLocation;
            floorMission.StartPosition = agvMission.StartPosition;
            floorMission.EndLocation = po01;
            floorMission.EndPosition = tsjInfo.TsjName;

            floorMission.StartMiddleLocation = agvMission.StartMiddleLocation;
            floorMission.StartMiddlePosition = agvMission.StartMiddlePosition;
            floorMission.EndMiddleLocation = agvMission.EndMiddleLocation;
            floorMission.EndMiddlePosition = agvMission.EndMiddlePosition;

            floorMission.SendState = string.Empty;
            floorMission.SendMsg = string.Empty;
            floorMission.StateMsg = string.Empty;
            floorMission.RunState = string.Empty;
            //floorMission.StockPlan_ID=string.Empty
            floorMission.OrderGroupId = string.Empty;
            floorMission.AGVCarId = string.Empty;
            floorMission.userId = agvMission.userId;
            floorMission.MissionFloor_ID = agvMission.ID;
            floorMission.ModelProcessCode = string.Empty;
            floorMission.TSJ_Name = tsjInfo.TsjName;
           // modelHelper.ChangeModel(floorMission, false);
            if (dTable == null)
                dTable = floorService.ClassToDataTable(typeof(AGVMissionInfo_Floor));
            dTable.Clear();
            dTable = floorService.ParseInDataTable(dTable, floorMission);


            //生成步骤2
            floorMission.MissionNo = agvMission.MissionNo + DiffFloorFactory.twoStr;
            floorMission.Mark = Mark02;

            floorMission.StartLocation = po02;
            floorMission.StartPosition = tsjInfo.TsjName;
            floorMission.EndLocation = agvMission.EndLocation;
            floorMission.EndPosition = agvMission.EndPosition;
            floorMission.StartMiddleLocation = po02;
            floorMission.StartMiddlePosition = tsjInfo.TsjName;
            floorMission.EndMiddleLocation = agvMission.EndMiddleLocation;
            floorMission.EndMiddlePosition = agvMission.EndMiddlePosition;

            //floorMission.ModelProcessCode = ModelCode02;
            floorMission.OrderGroupId = string.Empty;
            modelHelper.ChangeModel(floorMission, false);
            dTable = floorService.ParseInDataTable(dTable, floorMission);
            floorService.AddMany(dTable);
        }

      

    }
}

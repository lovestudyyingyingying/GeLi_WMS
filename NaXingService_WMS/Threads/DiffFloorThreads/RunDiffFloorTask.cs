using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.AGVOrderEntity;
using NanXingService_WMS.Entity.TiShengJiEntity;
using NanXingService_WMS.Services;
using NanXingService_WMS.Utils.AGVUtils;
using NanXingService_WMS.Utils.TishengjiUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanXingService_WMS.Threads.DiffFloorThreads
{
    public class RunDiffFloorTask
    {
        AGVMissionFloorService missionFloorService = new AGVMissionFloorService();
        AGVMissionService missionService = new AGVMissionService();
        AGVOrderUtils agvOrderUtils = new AGVOrderUtils();


        //readonly string hasSend = "成功";
        //readonly string hasSuccess = "已完成";
        //readonly string hasCancel = "已取消";

        //readonly string waitRun = "等待执行";
        //readonly string runFail = "执行失败";
        //readonly string sendFail = "发送失败";
        //readonly string sending = "发送失败";


        //readonly string F1 = "1";
        //readonly string F2 = "2";
        //readonly string F3 = "3";



        //TiShengJiDevice tsj = null;
        //TiShengJiHelper tsj_Helper = null;
        //DateTime dtStart = DateTime.Now;
        //public RunDiffFloorTask(TiShengJiDevice tsj, TiShengJiHelper tsj_Helper)
        //{
        //    this.tsj = tsj;
        //    this.tsj_Helper = tsj_Helper;
        //    this.dtStart = DateTime.Now;
        //}

        //public void RunControl()
        //{
        //    DateTime dt = DateTime.Now;
        //    DateTime fid = DateTime.Now.AddDays(-1);
        //    DataTable dtable = missionService.ConvertToTable();
        //    //DataTable dtable_Floor = missionFloorService.ConvertToTable();
        //    //1、等待确认的任务发送继续任务
        //    List<AGVMissionInfo_Floor> runlist = missionFloorService.GetList(
        //        u => u.SendState== hasSend && u.RunState == waitRun, true, DbMainSlave.Master);

        //    foreach(AGVMissionInfo_Floor temp in runlist)
        //    {
        //        if((temp.StartPosition.StartsWith(F1) && temp.StartPosition==tsj.tsj_1F
        //            && tsj_Helper.state.F1DuiJieWei == GunZhouState.HasBox)
        //            && (temp.StartPosition.StartsWith(F2) && temp.StartPosition == tsj.tsj_2F
        //            && tsj_Helper.state.F2DuiJieWei == GunZhouState.HasBox)

        //            &&(temp.EndPosition.StartsWith(F1) && temp.EndPosition == tsj.tsj_1F
        //            && tsj_Helper.state.F2DuiJieWei == GunZhouState.NoBox) 
        //            &&(temp.EndPosition.StartsWith(F2) && temp.EndPosition == tsj.tsj_2F
        //            && tsj_Helper.state.F2DuiJieWei == GunZhouState.NoBox))
        //        {
        //            //发送继续任务
        //            agvOrderUtils.ContinueTask(temp.MissionNo);
        //        }
        //    }

        //    //2、对已发送的跨楼层任务进行跟踪
        //    List<AGVMissionInfo> hasSendlist = missionService.GetQuery(
        //        u => u.OrderTime > fid && u.SendState == hasSend 
        //        && (u.AGVMissionInfo_Floor.StartPosition.StartsWith(tsj.tsj_Name)
        //        || u.AGVMissionInfo_Floor.EndPosition.StartsWith(tsj.tsj_Name))
        //        , true, DbMainSlave.Master)
        //        .OrderBy(u => u.ID).ToList();
        //    UpdateAMIF(hasSendlist);

        //    Thread.Sleep(1000);

        //    //判断提升机状态是否空闲
        //    if (tsj_Helper.state!=null && tsj_Helper.state.carState== TiShengCarState.NoJob)
        //    {
        //        //1、获取所有未完成的跨楼层任务
        //        List<AGVMissionInfo> list1 = missionService.GetQuery(u =>
        //                           u.OrderTime > fid
        //                        && u.RunState != runFail && u.RunState != sendFail
        //                        && u.RunState != hasSuccess && u.RunState !=hasCancel
        //                        && (u.AGVMissionInfo_Floor.StartPosition.StartsWith(tsj.tsj_Name)
        //        || u.AGVMissionInfo_Floor.EndPosition.StartsWith(tsj.tsj_Name))
        //                        , true, DbMainSlave.Master
        //                         ).OrderBy(u => u.ID).ToList();

        //        //2、判断第一阶段的任务个数
        //        List<AGVMissionInfo> stepA_list = list1.Where(u => (u.SendState == hasSuccess
        //                || u.SendState == sending)
        //                && (u.RunState == string.Empty || u.RunState == null)).ToList();

        //        if (stepA_list.Count() >= 2)
        //        {
        //            string startFloor = stepA_list[0].StartLocation.Substring(0, 1);
        //            int saId1 = stepA_list[0].ID;
        //            int saId2 = stepA_list[1].ID;
        //            List<AGVMissionInfo_Floor> sAAllList = missionFloorService.GetQuery(
        //                u => (u.StartPosition.StartsWith(tsj.tsj_Name)
        //                        || u.EndPosition.StartsWith(tsj.tsj_Name))&&
        //                        (u.MissionFloor_ID == saId1 || u.MissionFloor_ID == saId2), true, DbMainSlave.Master)
        //                .OrderByDescending(u => u.ID).ToList();

        //            List<AGVMissionInfo_Floor> sAList = sAAllList.Where(u=>u.SendState == hasSuccess
        //                || u.SendState == sending).ToList();
        //            List<AGVMissionInfo_Floor> sBList = sAAllList.Where(u => u.SendState == string.Empty)
        //                .ToList();
        //            //获取第一阶段已完成的数据做判断，数据库中
        //            if ((startFloor == F1 && tsj_Helper.state.F1Count == 2 && tsj_Helper.state.F1State == GunZhouState.PutBoxOK)
        //              || (startFloor == F2 && tsj_Helper.state.F2Count == 2 && tsj_Helper.state.F2State == GunZhouState.PutBoxOK)
        //              || (startFloor == F3 && tsj_Helper.state.F3Count == 2 && tsj_Helper.state.F3State == GunZhouState.PutBoxOK))
        //              //|| (sAList[0].RunState == "已完成" && sAList[1].RunState == "已完成"))
        //            {
        //                List<AGVMissionInfo> list = new List<AGVMissionInfo>(2);
        //                list.Add(stepA_list[0]);
        //                list.Add(stepA_list[1]);

        //                tsj_Helper.SendTSJOrder(stepA_list[0].StartLocation, stepA_list[0].EndLocation, list.Count);

        //                SendOrder(sBList[0]);
        //                SendOrder(sBList[1]);

        //                stepA_list[0].RunState = sending;
        //                stepA_list[1].RunState = sending;
        //                dtable = missionService.AddToDataTable(dtable, stepA_list[0]);
        //                dtable = missionService.AddToDataTable(dtable, stepA_list[1]);
        //                missionService.UpdateMany(dtable);
        //            }
        //        }
        //        else if (stepA_list.Count() == 1)
        //        {
        //            //有新的 未开始的跨楼层任务
        //            AGVMissionInfo temp = list1.Where(u => u.SendState == string.Empty 
        //            || u.SendState == null).
        //                OrderBy(u => u.ID).FirstOrDefault();
        //            List<AGVMissionInfo_Floor> floorTemp = missionFloorService.
        //                GetList(u => u.MissionNo.StartsWith( temp.MissionNo));

        //            string endFloor = stepA_list[0].EndLocation.Substring(0, 1);
        //            string startFloor = stepA_list[0].StartLocation.Substring(0, 1);
        //            double ts = double.Parse(ReckonSeconds(dtStart, DateTime.Now));

        //            //判断第一个子任务是否已完成
        //            int saId1 = stepA_list[0].ID;
        //            List<AGVMissionInfo_Floor> sAAllList = missionFloorService.GetQuery(u =>
        //              u.MissionFloor_ID == saId1,true, DbMainSlave.Master)
        //               .OrderByDescending(u => u.ID).ToList();

        //            List<AGVMissionInfo_Floor> sAList = sAAllList.Where(u => u.SendState == hasSuccess
        //                || u.SendState == sending).ToList();
        //            List<AGVMissionInfo_Floor> sBList = sAAllList.Where(u => u.SendState == string.Empty)
        //                .ToList();
        //            //有新任务并新任务与旧任务的目标起始楼层一致,且新任务与旧任务大于10秒
        //            if (temp != null && temp.StockPlan_ID == stepA_list[0].StockPlan_ID
        //               && temp.EndLocation.StartsWith(endFloor) 
        //               && temp.StartLocation.StartsWith(startFloor))
        //            {
        //                SendOrder(floorTemp[0]);
        //            }
        //            else if (
        //                        //没有新任务并等待时间大于20秒
        //                        //判断第一个子任务是否已完成
        //                        (temp == null && ts > 20 && sAList.Count > 0 && sAList[0].RunState == hasSuccess)||
        //                        //有新任务但目标楼层不一致
        //                        ((startFloor == F1 && tsj_Helper.state.F1Count == 1 && tsj_Helper.state.F1State == GunZhouState.PutBoxOK)
        //              || (startFloor == F2 && tsj_Helper.state.F2Count ==1 && tsj_Helper.state.F2State == GunZhouState.PutBoxOK)
        //              || (startFloor == F3 && tsj_Helper.state.F3Count == 1 && tsj_Helper.state.F3State == GunZhouState.PutBoxOK)))
        //            {
        //                tsj_Helper.SendTSJOrder(stepA_list[0].StartLocation, stepA_list[0].EndLocation, stepA_list.Count);
        //                SendOrder(sBList[0]);
                        
        //                stepA_list[0].RunState = sending;
        //                dtable = missionService.AddToDataTable(dtable, stepA_list[0]);
        //                missionService.UpdateMany(dtable);
        //            }
        //        }
        //        else if (stepA_list.Count() == 0)
        //        {
        //            List<AGVMissionInfo> temp = list1.Where(u => u.SendState == string.Empty).
        //                       OrderBy(u => u.ID).ToList();
        //            List<AGVMissionInfo> temp2 = list1.Where(u => u.SendState.Length > 0).
        //               OrderBy(u => u.ID).ToList();
        //            if (temp2.Count > 0 && temp.Count > 0 && temp[0].StartLocation.Substring(0, 1) != temp2[0].StartLocation.Substring(0, 1))
        //            {
        //                //等待上一批任务完成
                        
        //            }
        //            else if (temp.Count > 1
        //                && temp[0].StartLocation.Substring(0, 1) == temp[1].StartLocation.Substring(0, 1)
        //                && temp[0].EndLocation.Substring(0, 1) == temp[1].EndLocation.Substring(0, 1))
        //            {
        //                List<AGVMissionInfo_Floor> floorList = missionFloorService.GetList(
        //                    u => u.MissionNo == temp[0] + "-01"|| u.MissionNo == temp[1] + "-01");
        //                //判断前两个是否相等
        //                //SendOrder(temp[0], 1);
        //                //SendOrder(temp[1], 1);
        //                int saId1 = temp[0].ID;
        //                int saId2 = temp[1].ID;

        //                temp[0].SendState = sending;
        //                temp[1].SendState = sending;
        //                dtable = missionService.AddToDataTable(dtable, temp[0]);
        //                dtable = missionService.AddToDataTable(dtable, temp[1]);

        //                missionService.UpdateMany(dtable);
        //                SendOrder(floorList[0]);
        //                SendOrder(floorList[1]);
        //            }
        //            else if (temp.Count == 1)
        //            {
        //                AGVMissionInfo_Floor amif = missionFloorService.GetList(
        //                    u => u.MissionNo == temp[0] + "-01").FirstOrDefault();
        //                SendOrder(amif);
        //                dtStart = DateTime.Now;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// 更新跨楼层任务的状态
        ///// </summary>
        ///// <param name="hasSendlist"></param>
        //private void UpdateAMIF(List<AGVMissionInfo> hasSendlist)
        //{
        //    DataTable dt = missionService.ConvertToTable();
        //    foreach (AGVMissionInfo temp in hasSendlist)
        //    {
        //        DateTime dt1 = DateTime.Now.AddDays(-1);
        //        //根据进出仓来判断子任务的状态
        //        int ID = (int)temp.MissionFloor_ID;
        //        AGVMissionInfo_Floor ami = missionFloorService.GetByID(ID, DbMainSlave.Master);
        //        //条件1：已取消，总任务=已取消
        //        //条件2：已完成，判断是第一阶段则任务中，第二阶段则已完成
        //        //条件3：执行失败、下发失败
        //        if (ami.RunState != temp.RunState)
        //        {
        //            if (ami.RunState == hasCancel || ami.RunState == runFail || ami.RunState == sendFail)
        //            {
        //                temp.RunState = ami.RunState;
        //                dt = missionService.AddToDataTable(dt, temp);
        //            }
        //            else if (ami.RunState == hasSuccess)
        //            {
        //                if ((temp.Mark == StockType.MovestockType_Floor && ami.Mark == StockType.MovestockType_Floor)
        //                  || (temp.Mark == StockType.InstockType && ami.Mark == temp.Mark)
        //                  || (temp.Mark == StockType.MovestockType && ami.Mark == temp.Mark))
        //                {
        //                    temp.RunState = ami.RunState;
        //                    dt = missionService.AddToDataTable(dt, temp);
        //                }
        //            }
        //        }
        //    }

        //    missionService.UpdateMany(dt);
        //}
        //MissionOrder mo;
        //string pathFormat_2F = "{0},{1}";
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="orderId">任务ID</param>
        ///// <param name="path">"20000091,20000078"</param>
        ///// <param name="lineOrder">列名+流水号</param>
        //private OrderResult SendOrder(AGVMissionInfo_Floor mission)
        //{
        //    if (mo != null)
        //    {
        //        mo = new MissionOrder();
        //        mo.modelProcessCode = mission.ModelProcessCode;
        //        mo.priority = "6";
        //        mo.fromSystem = "WMS";
        //        TaskOrderDetails taskOrderDetails = new TaskOrderDetails(
        //            //mission.StartLocation.StartsWith("1") ?
        //            //string.Format(pathFormat_1F, mission.StartLocation, mission.EndLocation):
        //            string.Format(pathFormat_2F, mission.StartLocation, mission.EndLocation)

        //            , string.Empty, string.Empty, string.Empty);
        //        List<TaskOrderDetails> list = new List<TaskOrderDetails>();
        //        list.Add(taskOrderDetails);
        //        mo.taskOrderDetail = list;
        //    }
        //    mo.modelProcessCode = mission.ModelProcessCode;
        //    mo.orderId = mission.MissionNo;
        //    mo.orderGroupId = mission.OrderGroupId;
        //    mo.taskOrderDetail[0].TaskPath =
        //    //mission.StartLocation.StartsWith("1") ?
        //    //        string.Format(pathFormat_1F, mission.StartLocation, mission.EndLocation) :
        //            string.Format(pathFormat_2F, mission.StartLocation, mission.EndLocation);

        //    mission.SendState = hasSend;
        //    DataTable dataTable = missionFloorService.ConvertToTable();
        //    dataTable = missionFloorService.AddToDataTable(dataTable, mission);
        //    missionFloorService.UpdateMany(dataTable);

        //    return agvOrderUtils.SendMission(mo);
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

using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.TiShengJiEntity;
using NanXingService_WMS.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static NanXingService_WMS.Threads.GroupMissionThread;

namespace NanXingService_WMS.Threads.DiffFloorThreads
{
    public class ParseDiffFloorTask
    {
        AGVMissionFloorService missionFloorService = new AGVMissionFloorService();
        AGVMissionService missionService = new AGVMissionService();

        TiShengJiDevice tsj_1 = null;
        TiShengJiDevice tsj_2 = null;

        List<AGVMissionInfo_Floor> list1 = null;
        List<AGVMissionInfo_Floor> list2 = null;


        #region 读取分离跨楼层任务
        AGVMissionInfo mission = null;
        DataTable dt_floor = null;
        DataTable dt = null;
        readonly string hasCopy = "已拆分";
        //readonly string hasSend = "成功";
        //readonly string hasSuccess = "已完成";

        #endregion

        public ParseDiffFloorTask(TiShengJiDevice tsj_1,TiShengJiDevice tsj_2)
        {
            this.tsj_1 = tsj_1;
            this.tsj_2 = tsj_2;
        }

        #region 读取并分离跨楼层任务
        /// <summary>
        /// 跨楼层命令分解为两行命令，另一个线程读取命令行进行执行
        /// </summary>
        public void ReadControl()
        {
            if (dt_floor == null)
                dt_floor = missionFloorService.ConvertToTable();
            try
            {
                if (DifferentFloorThread.missionQueue.Any())
                {
                    //1.定时从队列中查询数据
                    DifferentFloorThread.missionQueue.TryDequeue(out mission);

                    int index = 0;
                    //2.拆分跨楼层任务
                    //OrderResult result = SendOrder(mission);
                    AGVMissionInfo_Floor[] arr = ParseFloorMission(mission,ref index);

                    //3.添加数据库
                    dt_floor.Clear();
                    dt_floor = missionFloorService.AddToDataTable(dt_floor, arr[0]);
                    dt_floor = missionFloorService.AddToDataTable(dt_floor, arr[1]);
                    missionFloorService.AddMany(dt);
                    //4.修改数据库原有命令行状态
                    mission.RunState = hasCopy;
                    missionService.Update(mission);
                    if (index == 1)
                        DifferentFloorThread.tsj_1_missionQueue.Add(mission);
                    else
                        DifferentFloorThread.tsj_2_missionQueue.Add(mission);
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private AGVMissionInfo_Floor[] ParseFloorMission(AGVMissionInfo floorMission,ref int index)
        {
            //GetTiShengJi();
            //tisj_1 = RedisCacheHelper.Get<TiShengJiState>($"tsj_{tisj_IP_1}");
            //tisj_2 = RedisCacheHelper.Get<TiShengJiState>($"tsj_{tisj_IP_2}");
            AGVMissionInfo_Floor[] arr = new AGVMissionInfo_Floor[2];

            //第一步：任务类型与列号，筛选出优先类型的提升机
            string[] lineArr = floorMission.EndPosition.Split('-');
            string lie = $"{lineArr[0]}-{lineArr[1]}";
            int tsj_Index = 1;
            int youxian_Index = 1;
            int notYouXian = 1;
            //筛选出优先类型的提升机
            if (tsj_1.tsj_YouXianType.Contains(floorMission.Mark))
            {
                youxian_Index = 1;
                notYouXian = 2;
            }
            else if (tsj_2.tsj_YouXianType.Contains(floorMission.Mark))
            {
                youxian_Index = 2;
                notYouXian = 1;
            }

            Expression<Func<AGVMissionInfo_Floor, bool>> exp = DbBaseExpand.True<AGVMissionInfo_Floor>();
            if (floorMission.Mark == StockType.InstockType)
                exp.And(u => u.EndPosition.StartsWith(lie));
            else if (floorMission.Mark == StockType.InstockType)
                exp.And(u => u.StartPosition.StartsWith(lie));

            //exp.And(u => u.EndPosition.StartsWith(lie) || u.StartPosition.StartsWith(lie));

            if (list1.Any(exp.Compile()))
                tsj_Index = 1;
            else if (list2.Any(exp.Compile()))
                tsj_Index = 2;
            else//如果没有提升机进行中的任务，可以当成新列
            {
                //if(youxian_Index==1)
                //优先不空闲，只有非优先空闲
                if(!(notYouXian==1? DifferentFloorThread.tsj_1_missionQueue: DifferentFloorThread.tsj_2_missionQueue).Any()
                    && (youxian_Index==1? DifferentFloorThread.tsj_1_missionQueue : DifferentFloorThread.tsj_2_missionQueue).Any())
                {
                    tsj_Index = notYouXian;
                }else
                    tsj_Index = youxian_Index;
            }
            arr[0] = GetMissionByBuZhou(floorMission, 1, tsj_Index);
            arr[1] = GetMissionByBuZhou(floorMission, 2, tsj_Index);

            index = tsj_Index;
            return arr;
        }

        private AGVMissionInfo_Floor GetMissionByBuZhou(AGVMissionInfo floorMission, int buzhou, int index)
        {
            TiShengJiDevice temp = index == 1 ? tsj_1 : tsj_2;

            string start = string.Empty;
            string startPo = string.Empty;

            string end = string.Empty;
            string endPo = string.Empty;

            string lineOrder = string.Empty;
            string modelCode = string.Empty;

            string trayNo = string.Empty;

            //当进仓第二步骤成功才算成功进仓
            //当出仓第1步骤成功算出仓成功
            //所以进仓第二步骤为02、出仓\调拨第一步骤为03，第二步骤为05
            string mark = string.Empty;
            if (buzhou == 1)
            {
                start = floorMission.StartLocation;
                startPo = floorMission.StartPosition;
                trayNo = floorMission.TrayNo;
                //搬进
                if (floorMission.StartLocation.StartsWith("1"))
                {
                    end = temp.tsj_1F;
                    endPo = temp.tsj_Name + "-1L";
                    modelCode = temp.tsj_MoveIn_1F;
                }
                else if (floorMission.StartLocation.StartsWith("2"))
                {
                    end = temp.tsj_2F;
                    endPo = temp.tsj_Name + "-2L";
                    modelCode = temp.tsj_MoveIn_2F;

                }
                else if (floorMission.StartLocation.StartsWith("3"))
                    end = temp.tsj_3F;
                lineOrder = end;
                if (floorMission.Mark == "05" || floorMission.Mark == "04")
                    mark = "03";
                else
                    mark = "06";
            }
            else if (buzhou == 2)
            {
                trayNo = string.Empty;
                //搬走
                if (floorMission.EndLocation.StartsWith("1"))
                {
                    start = temp.tsj_1F;
                    startPo = temp.tsj_Name + "-1L";
                    modelCode = temp.tsj_MoveOut_1F;
                }
                else if (floorMission.EndLocation.StartsWith("2"))
                {
                    start = temp.tsj_2F;
                    startPo = temp.tsj_Name + "-2L";
                    modelCode = temp.tsj_MoveOut_2F;
                }
                else if (floorMission.EndLocation.StartsWith("3"))
                    start = temp.tsj_3F;
                end = floorMission.EndLocation;
                endPo = floorMission.EndPosition;
                //mark = "06";
                string[] arr = floorMission.EndPosition.Split('-');
                lineOrder = $"{arr[0]}-{ arr[1]}";

                if (floorMission.Mark == "02" || floorMission.Mark == "04")
                    mark = floorMission.Mark;
                else
                    mark = "05";
            }


            //插入数据
            AGVMissionInfo_Floor newMission = new AGVMissionInfo_Floor();
            newMission.MissionNo = floorMission.MissionNo + "-" + buzhou;
            newMission.OrderTime = DateTime.Now;
            newMission.StartLocation = start.Trim();
            newMission.StartPosition = startPo;
            newMission.EndLocation = end.Trim();
            newMission.EndPosition = endPo;

            newMission.TrayNo = trayNo;
            newMission.Mark = mark;
            newMission.StockPlan_ID = Convert.ToInt32(floorMission.StockPlan_ID);
            newMission.SendState = string.Empty;
            newMission.RunState = string.Empty;

            newMission.userId = floorMission.userId;
            newMission.MissionFloor_ID = floorMission.ID;
            //newMission.SendState = "成功";
            //if (amif.StockNo == "回转")
            lineOrder = string.Empty;
            newMission.OrderGroupId = lineOrder;
            newMission.ModelProcessCode = modelCode;
            //temp.AGVMissionInfo.Add(mission);
            return newMission;
        }

        #endregion

    }
}

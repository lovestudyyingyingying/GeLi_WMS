using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Managers;
using NanXingService_WMS.Services;
using NanXingService_WMS.Services.APS;
using NanXingService_WMS.Services.WMS;
using NanXingService_WMS.Services.WMS.AGV;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.AGVUtils;
using NanXingService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilsSharp;

namespace NanXingService_WMS.Helper.WMS
{
    public class HuiZhuanHelper
    {
        LiuShuiHaoService liuShuiHaoService;
        //TrayStateService trayStateService;
        AGVMissionService missionService;
        InstockManager instockManager;
        OutstockManager outstockManager;
        WareLocationService wareLocationService;
        AGVOrderUtils agvUtils = null;
        MyTask myTask;

        public HuiZhuanHelper(LiuShuiHaoService liuShuiHaoService,
             AGVMissionService missionService
            ,InstockManager instockManager, OutstockManager outstockManager, WareLocationService wareLocationService)
        {
            this.liuShuiHaoService = liuShuiHaoService;
            //this.trayStateService = trayStateService;
            this.missionService = missionService;
            this.wareLocationService = wareLocationService;

            this.instockManager = instockManager;
            this.outstockManager = outstockManager;
        }

        public MyTask StartHuiZhuan(string trayNo1, string trayNo2, int index)
        {
            return myTask = new MyTask(() =>
            {
                HuiZhuanTest(trayNo1, trayNo2, index);
            }, 3, true,null, StopHuiZhuan)
                .StartTask();
        }

        public void HuiZhuanTest(string trayNo1, string trayNo2,int index)
        {
            string remarkStr = huiZhuanRemark + index.ToString();
            DateTime dtime = DateTime.Now.AddDays(-1);
            int count = missionService.GetCount(u => u.OrderTime > dtime
             && u.Remark == remarkStr &&
             //已下发到表 或者 执行中
             (u.SendState.Length == 0 ||
             (u.SendState.Length > 0

             &&!(u.SendState== StockState.SendState_Success 
                && u.RunState == StockState.RunState_Success)

             && u.RunState != StockState.RunState_RunFail
             && u.RunState != StockState.SendState_Fail
             && u.RunState != StockState.RunState_Cancel)), true, DbMainSlave.Master);
            DataTable dtable = missionService.ConvertToTable();
            Logger.Default.Process(new Log(LevelType.Info,
                $"数量：{count}"));
            if (count == 0)
            {
                dtable.Clear();
                //1、获取到条码位置
                //TrayState trayState1 = trayStateService.GetByTrayNo(trayNo1, true, DbMainSlave.Master);
                //TrayState trayState2 = trayStateService.GetByTrayNo(trayNo2, true, DbMainSlave.Master);

                WareLocation trayWL1 = wareLocationService.GetIQueryable(u => u.TrayState.TrayNO == trayNo1,
               true, DbMainSlave.Master).FirstOrDefault();
                WareLocation trayWL2 = wareLocationService.GetIQueryable(u => u.TrayState.TrayNO == trayNo2,
                true, DbMainSlave.Master).FirstOrDefault();

                WareLocation[] arr = new WareLocation[] { trayWL1, trayWL2 };
                if (agvUtils == null)
                {
                    agvUtils = new AGVOrderUtils(trayWL1.WareArea.WareHouse.AGVServerIP);
                }
                if (trayWL1.WareArea.WareAreaClass.AreaClass== AreaClassType.ChengPinArea)
                {
                    //成品区：出库到缓存区
                    HuiZhuan_Out(arr, dtable, remarkStr);
                }
                else if(trayWL1.WareArea.WareAreaClass.AreaClass == AreaClassType.HuanCunArea)
                {
                    //缓存区：调拨到缓存区
                    HuiZhuan_Move(arr, remarkStr);
                }
                
            }
        }
        string huiZhuanRemark = "回转测试";
        public void HuiZhuan_Out(WareLocation[] arr, DataTable dtable,string remarkStr)
        {
            StockPlan stockPlan = MapperHelper<WareLocation, StockPlan>.Map(arr[0]);
            stockPlan.PlanNo = liuShuiHaoService.GetOutStockNoLSH();
            stockPlan.count = arr[0].TrayState.OnlineCount + arr[1].TrayState.OnlineCount;
            stockPlan.plantime = DateTime.Now;
            stockPlan.planUser = "001";
            stockPlan.states = "0";
            stockPlan.mark = MissionType.MoveOut_TSJ;
            stockPlan.position = string.Empty;
            List<WareLocation> list = outstockManager.outstockHelper.GetTargetWls(stockPlan, 2);
            for (int i = 0; i < arr.Length; i++)
            {
                AGVMissionInfo missionInfo = new AGVMissionInfo();
                missionInfo.MissionNo = liuShuiHaoService.GetAGVMissionNoLSH();
                missionInfo.OrderTime = DateTime.Now;
                missionInfo.TrayNo = arr[i].TrayState.TrayNO;
                missionInfo.Mark = MissionType.MoveOut_TSJ;
                missionInfo.StartLocation = arr[i].AGVPosition;
                missionInfo.StartPosition = arr[i].WareLocaNo;
                missionInfo.EndLocation = list[i].AGVPosition;
                missionInfo.EndPosition = list[i].WareLocaNo;
                missionInfo.SendState = string.Empty;
                missionInfo.SendMsg = string.Empty;
                missionInfo.RunState = string.Empty;
                missionInfo.StateMsg = string.Empty;
                missionInfo.OrderGroupId = string.Empty;
                missionInfo.AGVCarId = string.Empty;
                missionInfo.IsFloor = 1;
                missionInfo.WHName = string.Empty;
                missionInfo.Remark = remarkStr;
                dtable = missionService.ParseInDataTable(dtable, missionInfo);
            }
            missionService.AddMany(dtable);
        }

        public void HuiZhuan_Move(WareLocation[] arr, string remarkStr)
        {
            Logger.Default.Process(new Log(LevelType.Info,
               $"从缓存区搬上二楼"));
            string msg = string.Empty;
            foreach (var temp in arr)
            {
                instockManager.Instock(temp.WareLocaNo,
                    temp.TrayState.TrayNO, string.Empty,string.Empty, remarkStr,ref msg,string.Empty, true);
            }
        }

        private void StopHuiZhuan()
        {
            if (agvUtils==null)
                return;
            DateTime dtime = DateTime.Now.AddDays(-1);
            List<AGVMissionInfo> q = missionService.GetList(u => u.OrderTime > dtime
            && u.Remark == huiZhuanRemark &&
            //已下发到表 或者 执行中
            (u.SendState.Length == 0 ||
            (u.SendState.Length > 0
            && u.RunState != StockState.RunState_RunFail
            && u.RunState != StockState.SendState_Fail
            && u.RunState != StockState.RunState_Success
            && u.RunState != StockState.RunState_Cancel))
            ,true,DbMainSlave.Master);
            string[] arr = new string[1];
            foreach (var temp in q)
            {
                foreach(AGVMissionInfo_Floor floorTemp in temp.AGVMissionInfo_Floor)
                {
                    if (floorTemp.SendState.Length > 0
                        && floorTemp.RunState != StockState.RunState_RunFail
                        && floorTemp.RunState != StockState.SendState_Fail
                        && floorTemp.RunState != StockState.RunState_Success
                        && floorTemp.RunState != StockState.RunState_Cancel)
                    {
                        arr[0] = floorTemp.MissionNo;
                        Task.Run(()=>agvUtils.CancelMission(arr));
                        Thread.Sleep(200);
                    }
                }
                
            }

        }
    }
}

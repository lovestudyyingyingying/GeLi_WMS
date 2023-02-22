using FineUIPro;
using GeLi_Utils.Services.WMS;
using GeLiData_WMS;
using GeLiPage_WMS.GeLiPage;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS.AGV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GeLiPage_WMS.admin
{
    public partial class _default : PageBase
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            WareAreaClassService wareAreaClassService = new WareAreaClassService();
            WareLocationService wareLocationService = new WareLocationService();
            var wareAreaClass = wareAreaClassService.GetAll();
            foreach (var item in wareAreaClass)
            {
                UserControlConnector ctrlConnector1 = new UserControlConnector();

                mainPanel.Items.Add(ctrlConnector1);

                var haduse = wareLocationService.GetIQueryable(u => u.WareArea.War_ID == item.ID && u.WareLocaState != WareLocaState.NoTray && u.IsOpen==1).Count().ToString();
                var noHaduse = wareLocationService.GetIQueryable(u => u.WareArea.War_ID == item.ID && u.WareLocaState == WareLocaState.NoTray && u.IsOpen == 1).Count().ToString();
                ctrlConnector1.Controls.Add(CreateANewWareArea(item.AreaClass, haduse,noHaduse));
            }
       
           

        }

        private WareAreaState CreateANewWareArea(string Title,string HadUse,string NoHadUse)
        {
            WareAreaState ctrl = LoadControl("~/GeLiPage/WareAreaState.ascx") as WareAreaState;
            ctrl.WareAreaTitle = Title;
            ctrl.HadUse = HadUse;
            ctrl.NoHadUse = NoHadUse;

            return ctrl;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            AGVAlarmLogService alarmLogService = new AGVAlarmLogService();
            AGVMissionService aGVMissionInfo = new AGVMissionService();
            var startDate = DateTime.Now.Date;
            var endDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            var mission = aGVMissionInfo.GetIQueryable(u => u.OrderTime >= startDate && u.OrderTime <= endDate);
            ImageLabelMain_UCTodayMission.Value= mission.Count().ToString();
            ImageLabelMain_UCTodayComplte.Value= mission.Where(u => u.RunState == StockState.RunState_Success).Count().ToString();
            ImageLabelMain_UCNoComplte.Value= mission.Where(u => u.RunState != StockState.RunState_Success).Count().ToString();
            ImageLabelMain_UCCancel.Value = mission.Where(u => u.RunState == StockState.RunState_Cancel).Count().ToString();
            ImageLabelMain_UC5.Value = mission.Where(u => u.RunState != StockState.RunState_Success).Count().ToString();
            ImageLabelMain_UCWarn.Value = alarmLogService.GetIQueryable(u => u.alarmDate >= startDate && u.alarmDate <= endDate).Count().ToString();
        }
    }
}

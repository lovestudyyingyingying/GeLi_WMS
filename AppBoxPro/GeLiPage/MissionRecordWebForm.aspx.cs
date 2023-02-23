using FineUIPro;
using GeLiData_WMS;
using GeLiData_WMSUtils;
using GeLiPage_WMS.Entity;
using GeLiService_WMS;
using GeLiService_WMS.Entity.AGVApiEntity;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Managers;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS.AGV;
using Nelibur.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GeLiPage_WMS.MissionRecordWebForm
{
    public partial class MissionRecordWebForm : PageBase
    {
        /// <summary>
        /// 本页面的浏览权限，空字符串表示本页面不受权限控制
        /// </summary>
        public override string ViewPower
        {
            get
            {
                return "MissionRecordWebForm_View";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LoadData();
                BindGrid1();
            }
        }

        private void LoadData()
        {
            DateTime startTime = DateTime.Now.AddDays(-1);
            DateTime endTime = DateTime.Now.AddDays(0);
            dp1.SelectedDate = DateTime.Parse(startTime.ToString("yyyy-MM-dd"));
            dp2.SelectedDate = DateTime.Parse(endTime.ToString("yyyy-MM-dd"));
        }

        private void BindGrid1()
        {
            AGVMissionService aGVMissionService = new AGVMissionService();

            DateTime dt1 = dp1.SelectedDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime dt2 = dp2.SelectedDate ?? DateTime.Now;
            dt2 = dt2.Date.AddDays(1).AddSeconds(-1);

            //筛选条件
            Expression<Func<AGVMissionInfo, bool>> expression = DbBaseExpand.True<AGVMissionInfo>();
            if (!string.IsNullOrEmpty(tbxMissionNo.Text))
                expression = expression.And(u => u.MissionNo.Contains(tbxMissionNo.Text));
            if(DropDownListName.SelectedValue!="全部")
                expression = expression.And(u => u.Reserve1== DropDownListName.SelectedValue);
            if (DropDownListRunState.SelectedValue != "全部")
            {
                if (DropDownListRunState.SelectedValue == "未完成")
                    expression = expression.And(u => string.IsNullOrEmpty(u.RunState));
                else
                    expression = expression.And(u => u.RunState == DropDownListRunState.SelectedValue);
            }
            if (DropDownSendState.SelectedValue != "全部")
            {
                if (DropDownSendState.SelectedValue == "未开始")
                    expression = expression.And(u => u.SendState == DropDownSendState.SelectedValue);
                else
                    expression = expression.And(u => u.SendState == DropDownSendState.SelectedValue);
            }
            if(!string.IsNullOrEmpty(tbxStart.Text))
                expression = expression.And(u => u.StartPosition.Contains(tbxStart.Text));
            if (!string.IsNullOrEmpty(tbxEnd.Text))
                expression = expression.And(u => u.EndPosition.Contains(tbxEnd.Text));

            expression = expression.And(
            u => u.OrderTime >= dt1 && u.OrderTime <= dt2);
            var agvMissionInfo = aGVMissionService.GetIQueryable(expression);
            Grid1.RecordCount = agvMissionInfo.Count();
            agvMissionInfo = SortAndPage<AGVMissionInfo>(agvMissionInfo, Grid1);


            var agvMissionInfoList = agvMissionInfo.ToList();
            TinyMapper.Bind<List<AGVMissionInfo>, List<AGVMissionInfoDto>>();
            var aGVMissionInfoDto = TinyMapper.Map<List<AGVMissionInfoDto>>(agvMissionInfoList);
            foreach (var item in aGVMissionInfoDto)
            {
                switch (item.Mark)
                {
                    case "10":
                        item.Mark = "物料上线";
                        break;
                    case "11":
                        item.Mark = "物料下线到产线";
                        break;
                    case "12":
                        item.Mark = "物料下线到缓存";
                        break;
                    case "13":
                        item.Mark = "码盘任务";
                        break;
                    case "14":
                        item.Mark = "满托搬离";
                        break;
                    case "":
                        item.Mark = "空托任务";
                        break;
                    default:
                        break;
                }
            }




            Grid1.DataSource = aGVMissionInfoDto;

            Grid1.DataBind();
        }


        #region 事件
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid1();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

            if (!CheckPower("MissionRecordWebForm_Cancel"))

            {

                CheckPowerFailWithAlert();

                return;

            }
            AGVMissionService aGVMissionService = new AGVMissionService();
            WareLocationService wareLocationService = new WareLocationService();
            int id = GetSelectedDataKeyID(Grid1);
            var agvMissionInfo = aGVMissionService.FindById(id, DbMainSlave.Master);
            if (agvMissionInfo.RunState == StockState.RunState_Success || agvMissionInfo.RunState == StockState.RunState_Cancel
               )
            {
                Alert.Show($"失败:{agvMissionInfo.MissionNo + agvMissionInfo.RunState}");
                return;
            }

            //取消任务:任务状态挂为已取消，库位起点终点改为占用和空，库位预锁解锁，货物状态释放
            //使用数据库事务来处理，有一方为失败全体失败
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    agvMissionInfo.SendState= StockState.RunState_Cancel;
                    agvMissionInfo.RunState = StockState.RunState_Cancel;
                    agvMissionInfo.NodeTime = DateTime.Now;
                    var startLocation = wareLocationService.GetByAGVPo(agvMissionInfo.StartLocation, false, DbMainSlave.Master);
                    var endLocation = wareLocationService.GetByAGVPo(agvMissionInfo.EndLocation, false, DbMainSlave.Master);

                    startLocation.WareLocaState = WareLocaState.HasTray;
                    startLocation.LockHis_ID = null;
                    startLocation.TrayState_ID = null;

                    endLocation.WareLocaState = WareLocaState.NoTray;
                    endLocation.LockHis_ID = null;
                    endLocation.TrayState_ID = null;
                    aGVMissionService.Update(agvMissionInfo);
                    aGVMissionService.SaveChanges();
                    wareLocationService.Update(startLocation);                 
                    wareLocationService.Update(endLocation);  
                    wareLocationService.SaveChanges();
                    tran.Complete();
                  

                }
                catch(Exception ee)
                {
                    Alert.Show("处理失败");
                    Logger.Default.Process(new Log(LevelType.Error, ee.ToString()));
                }
            }
            Alert.Show("成功");
            BindGrid1();

        }

        protected void btnFinish_Click(object sender, EventArgs e)
        {
            if (!CheckPower("MissionRecordWebForm_Finish"))

            {

                CheckPowerFailWithAlert();

                return;

            }

            AGVMissionService aGVMissionService = new AGVMissionService();
            int id = GetSelectedDataKeyID(Grid1);
            var agvMissionInfo = aGVMissionService.FindById(id, DbMainSlave.Master);
            if (agvMissionInfo.RunState == StockState.RunState_Success|| agvMissionInfo.RunState ==StockState.RunState_Cancel)
            {
                Alert.Show($"失败:{agvMissionInfo.MissionNo+agvMissionInfo.RunState}");
                return;
            }

            AGVApiManager aGVApiManager = new AGVApiManager(new AGVMissionService(), new AGVMissionFloorService(), new TrayStateService(), new WareLocationService(), new DeviceStatesService(), new GeLi_Utils.Services.WMS.AGVAlarmLogService(), new GeLiService_WMS.Services.WMS.AGVRunModelService(), new GeLiService_WMS.Services.WMS.WareLocationLockHisService(), new TiShengJiInfoService(), new GeLiService_WMS.Services.WMS.StockRecordService(), new WareLocationTrayManager(new TrayStateService(), new WareLocationService(), new GeLiService_WMS.Services.WMS.WareLocationLockHisService()));
            MissionState missionState = new MissionState() { taskId = agvMissionInfo.MissionNo, targetPoint = agvMissionInfo.EndLocation, agvNo = 0, status = 1011 };
          var result =   aGVApiManager.UpdateMissionStates(missionState);
            Alert.Show(result.message);
            BindGrid1();
        }

        protected void Grid1_Sort(object sender, FineUIPro.GridSortEventArgs e)
        {
            Grid1.SortDirection = e.SortDirection;
            Grid1.SortField = e.SortField;
            BindGrid1();
        }

        protected void Grid1_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            Grid1.PageIndex = e.NewPageIndex;
            BindGrid1();
        }
        #endregion
    }
}
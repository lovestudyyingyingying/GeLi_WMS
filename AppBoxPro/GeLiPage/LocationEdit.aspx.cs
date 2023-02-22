using System;
using System.Linq;
using FineUIPro;

using System.Linq.Expressions;
using GeLiPage_WMS;
using GeLiData_WMS;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiData_WMSUtils;
using GeLiService_WMS.Services;
using GeLiService_WMS.Managers;
using GeLiService_WMS.Services.WMS;

namespace GeLiPage_WMS.GeLiPage
{
    public partial class LocationEdit : PageBase
    {
        #region ViewPower

        /// <summary>
        /// 本页面的浏览权限，空字符串表示本页面不受权限控制
        /// </summary>
        public override string ViewPower
        {
            get
            {
                return "";
            }
        }

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            WareLocationService wareLocationService = new WareLocationService();
            BindGrid();
            btnClose.OnClientClick = ActiveWindow.GetHideReference();

            int id = GetQueryIntValue("id");
            
            WareLocation item= wareLocationService.FindById(id);
            if (item == null)
            {
                // 参数错误，首先弹出Alert对话框然后关闭弹出窗口
                Alert.Show("参数错误！", String.Empty, ActiveWindow.GetHideReference());
                return;
            }

            lab_WMSPosition.Text = item.WareLocaNo;
            lab_AGVPosition.Text = item.AGVPosition;
            lab_WMSLie.Text = item.WareLoca_Lie;
            lab_WMSLieBatchNo.Text = item.BatchNo;
            if (item.TrayState_ID != null)
            {
                ddlTrayNo.Value = item.TrayState_ID.ToString();
                ddlTrayNo.Text = item.TrayState.TrayNO;
                Grid1.SelectedRowID= item.TrayState_ID.ToString();
                labProName.Text = item.TrayState.proname;
                labBatchNo.Text = item.TrayState.batchNo;
                ddlState.SelectedValue = "占用";
            }
        }

        #endregion

        #region Events

        protected void btnSaveClose_Click(object sender, EventArgs e)
        {
            WareLocationLockHisService wareLocationLockHisService = new WareLocationLockHisService();
            WareLocationService wareLocationService = new WareLocationService();
            TrayStateService trayStateService = new TrayStateService();
            StockRecordService stockRecordService = new StockRecordService();
            WareLocationTrayManager wareLocationTrayManager = new WareLocationTrayManager(trayStateService,wareLocationService, wareLocationLockHisService);
            int id = GetQueryIntValue("id");

            WareLocation wareLocation= wareLocationService.FindById(id,DbMainSlave.Master);
            TrayState ts = null;
            if (wareLocation.TrayState != null && ddlTrayNo.Text.Trim().Length>0
                && wareLocation.TrayState.TrayNO!= ddlTrayNo.Text.Trim())
            {
                Alert.Show("该库位已绑定标签，请先将原有标签产品出库");
                return;
            }
            else if (ddlTrayNo.Text.Trim().Length > 0)
            {
                ts = trayStateService.GetByTrayNo(ddlTrayNo.Text, false, DbMainSlave.Master);
                if (ts.WareLocation!= null)
                {
                    Alert.Show("该标签已绑定库位，请先将该标签原有库位产品出库");
                    return;
                }
            }
            //1、原有空 最后有   3、原有有  最后空

            if ((wareLocation.TrayState==null && !string.IsNullOrEmpty(ddlTrayNo.Text.Trim()))
               || (wareLocation.TrayState != null && ddlTrayNo.Text.Trim().Length == 0))
            {
                if (ddlTrayNo.Text.Trim().Length ==0)
                    ts = wareLocation.TrayState;
                
                //if (wareLocation.TrayState != null)
                //{
                //    WareLocationTrayManager.ChangeTrayWareLocation(1, wareLocation, wareLocation.TrayState);
                //}
                bool ret = false;
                if(ddlTrayNo.Text.Trim().Length > 0)
                    ret = wareLocationTrayManager.ChangeTrayWareLocation(0, wareLocation, ts);
                else
                    ret = wareLocationTrayManager.ChangeTrayWareLocation(1, wareLocation, wareLocation.TrayState);

                if (ret)
                    stockRecordService.AddHandStockRecord(ts, wareLocation.WareLocaNo, GetIdentityName(),
                        DateTime.Now, ddlTrayNo.Text.Trim().Length > 0);
            }
            else if (wareLocation.WareLocaState != ddlState.SelectedValue)
            {
                if (ddlState.SelectedValue == "占用"|| ddlState.SelectedValue == "空")
                {
                    wareLocation.LockHis_ID = null;
                    wareLocation.TrayState_ID = null;
                }
                wareLocation.WareLocaState = ddlState.SelectedValue;
                
                
                wareLocationService.Update(wareLocation);
                wareLocationService.SaveChanges();
            }

            //FineUIPro.Alert.Show("保存成功！", String.Empty, FineUIPro.Alert.DefaultIcon, FineUIPro.ActiveWindow.GetHidePostBackReference());
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }

        #endregion

        private void BindGrid()
        {
            TrayStateService trayStateService = new TrayStateService();
            Expression<Func<TrayState, bool>> expression =
                expression = DbBaseExpand.True<TrayState>();
            if (!string.IsNullOrEmpty(ttbSearch.Text.Trim()))
            {
                expression= expression.And(u => u.TrayNO.Contains(ttbSearch.Text.Trim()));
            }
            var q = trayStateService.GetIQueryable(expression);
            Grid1.RecordCount = q.Count();
            q = SortAndPage(q, Grid1);
            Grid1.DataSource = q;
            Grid1.DataBind();
        }


        protected void ttbSearch_Trigger1Click(object sender, EventArgs e)
        {
            ttbSearch.Text = String.Empty;
            ttbSearch.ShowTrigger1 = false;

            BindGrid();

        }

        protected void ttbSearch_Trigger2Click(object sender, EventArgs e)
        {
            ttbSearch.ShowTrigger1 = true;

            BindGrid();
        }

        protected void ddlTrayNo_TextChanged(object sender, EventArgs e)
        {
            TrayStateService trayStateService = new TrayStateService();
            if (!string.IsNullOrEmpty(ddlTrayNo.Text))
            {
                TrayState ts = trayStateService.GetByTrayNo(ddlTrayNo.Text);
                labBatchNo.Text = ts.batchNo;
                labProName.Text = ts.proname;
                ddlState.SelectedValue = "占用";
            }
        }

        protected void Grid1_RowSelect(object sender, GridRowSelectEventArgs e)
        {
            TrayStateService trayStateService = new TrayStateService();
            int id = GetSelectedDataKeyID(Grid1);
            if (id > 0)
            {
                TrayState ts = trayStateService.FindById(id);
                labBatchNo.Text = ts.batchNo;
                labProName.Text = ts.proname;
                ddlState.SelectedValue = "占用";
            }
        }

        protected void btnClearTray_Click(object sender, EventArgs e)
        {
            ddlTrayNo.Text = String.Empty;
            ddlTrayNo.Value= String.Empty;
            labBatchNo.Text = String.Empty;
            labProName.Text = String.Empty;
            ddlState.SelectedValue = "空";
        }
    }
}

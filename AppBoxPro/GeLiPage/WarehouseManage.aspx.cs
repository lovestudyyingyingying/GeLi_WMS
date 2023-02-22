using FineUIPro;
using GeLi_Utils.Entity.StockEntity;
using GeLiData_WMS;
using GeLiData_WMSUtils;
using GeLiPage_WMS;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
using GeLiService_WMS.Services.WMS.AGV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GeLiPage_WMS.GeLiPage
{
    public partial class WarehouseManage : PageBase
    {

        /// <summary>
        /// 本页面的浏览权限，空字符串表示本页面不受权限控制
        /// </summary>
        public override string ViewPower
        {
            get
            {
                return "WarehouseManage_View";
            }
        }
        //key-列名  value-值
        static Dictionary<string, string> dictClickColsName = new Dictionary<string, string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dictClickColsName.Clear();
                LoadData();
            }
        }
        private void LoadData()
        {
            BindData();
            BindGrid2();
            BindGrid3();
            BindGrid1();
        }
        /// <summary>
        /// 库区
        /// </summary>
        private void BindGrid3()
        {
            WareAreaService wareAreaService = new WareAreaService();
            //筛选条件
            Expression<Func<WareArea, bool>> expression = DbBaseExpand.True<WareArea>(); ;
           // expression = expression.And(u => u.WareAreaClass.AreaClass != "等待区");
            int select_Id = GetSelectedDataKeyID(Grid2);
            if (select_Id > -1)
            {
                expression = expression.And(u => u.WareHouse_ID == select_Id);
            }

            var q = wareAreaService.GetIndexData(expression);

            Grid3.RecordCount = q.Count();
            q = SortAndPage(q, Grid3);
            Grid3.DataSource = q;
            Grid3.DataBind();


        }
        /// <summary>
        /// 仓库
        /// </summary>
        private void BindGrid2()
        {
            //筛选条件
            WareHouseService wareHouseService = new WareHouseService();
            var q =
            wareHouseService.GetAllQueryable(u => u.WHName);

            Grid2.RecordCount = q.Count();
            q = SortAndPage(q, Grid2);
            Grid2.DataSource = q;
            Grid2.DataBind();
        }
        /// <summary>
        /// 库位
        /// </summary>
        private void BindGrid1()
        {
            WareLocationService wareLocationService = new WareLocationService();
            //筛选条件
            Expression<Func<WareLocation, bool>> expression =
                expression = DbBaseExpand.True<WareLocation>();
            int select_Id = GetSelectedDataKeyID(Grid3);
            if (select_Id > -1)
            {
                expression = expression.And(u => u.WareArea_ID == select_Id);
            }
            if (!string.IsNullOrEmpty(tbx_WlNo.Text.Trim()))
            {
                string value = tbx_WlNo.Text.Trim().ToUpper();
                expression = expression.And(u => u.WareLocaNo.Contains(value));
            }
            if (!string.IsNullOrEmpty(tbx_TrayNo.Text.Trim()))
            {
                string value = tbx_TrayNo.Text.Trim().ToUpper();
                expression = expression.And(u => u.TrayState != null &&
                u.TrayState.TrayNO.Contains(value));
            }
            if (!string.IsNullOrEmpty(tbx_GuanJianZi.Text.Trim()))
            {
                string value = tbx_GuanJianZi.Text.Trim().ToUpper();
                expression = expression.And(u => (u.TrayState != null &&
                (u.TrayState.TrayNO.Contains(value) || u.TrayState.batchNo.Contains(value)))
                || u.WareLocaNo.Contains(value) || u.WareLocaState.Contains(value));
            }
            if (ddlIsOpen.SelectedValue.Length > 0)
            {
                int state = Convert.ToInt32(ddlIsOpen.SelectedValue);
                expression = expression.And(u => u.IsOpen == state);
            }
           // expression = expression.And(u => u.WareArea.WareAreaClass.AreaClass != "等待区");
            var q = wareLocationService.GetIndexData(expression);

            Grid1.RecordCount = q.Count();

            q = SortAndPage(q, Grid1);
            Grid1.DataSource = q;
            Grid1.DataBind();
        }
        /// <summary>
        /// 统计已用库位数量
        /// </summary>
        private void BindData()
        {
            WareLocationService wareLocationService = new WareLocationService();
            //1、统计已使用仓位数量，2、统计未使用仓位数量
            var q_All = wareLocationService.GetList(null);

            var q_All_1 = q_All.Where(u => u.WareArea.WareHouse.WHName == EmptyTrayToBufferType.GeLi_1Lou);

            var HasUseCount_1L = q_All_1.Count(u => u.WareLocaState == WareLocaState.HasTray);

            var q_All_2 = q_All.Where(u => u.WareArea.WareHouse.WHName == EmptyTrayToBufferType.GeLi_2Lou);

            var HasUseCount_2L = q_All_2.Count(u => u.WareLocaState == WareLocaState.HasTray);
            var NoUseCount_1L = q_All_1.Count() - HasUseCount_1L;
            var NoUseCount_2L = q_All_2.Count() - HasUseCount_2L;


            ImageLabel_UC1.Value = HasUseCount_1L.ToString();
            ImageLabel_UC2.Value = NoUseCount_1L.ToString();

            ImageLabel_UC3.Value = HasUseCount_2L.ToString();
            ImageLabel_UC4.Value = NoUseCount_2L.ToString();

        }
        protected void Grid2_RowSelect(object sender, FineUIPro.GridRowSelectEventArgs e)
        {
            BindGrid3();
        }

        protected void Grid3_PreRowDataBound(object sender, FineUIPro.GridPreRowEventArgs e)
        {
            bool ret = ((WareAreaIndexData)e.DataItem).WareAreaState;

            FineUIPro.LinkButtonField isOpenBtn = Grid3.FindColumn("ChangeOpenArea") as FineUIPro.LinkButtonField;

            string opened = "已启用";
            string closed = "未启用";

            if (ret)
            {
                isOpenBtn.Icon = Icon.Accept;
                isOpenBtn.Text = opened;
            }
            else
            {
                isOpenBtn.Icon = Icon.Cancel;
                isOpenBtn.Text = closed;
            }
        }

        protected void Grid3_RowCommand(object sender, FineUIPro.GridCommandEventArgs e)
        {
            WareAreaService wareAreaService = new WareAreaService();
            if (e.CommandName == "ChangeOpenArea")
            {
                object[] keys = Grid3.DataKeys[e.RowIndex];
                int area_ID = Convert.ToInt32(keys[0]);
                bool ret = Convert.ToBoolean(keys[1]);
                
                wareAreaService.ChangeAreaState(area_ID, !ret);

                BindGrid3();
                BindGrid1();

            }
        }

        protected void Grid1_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            Grid1.PageIndex = e.NewPageIndex;
            BindGrid1();
        }

        protected void Grid1_Sort(object sender, FineUIPro.GridSortEventArgs e)
        {
            Grid1.SortDirection = e.SortDirection;
            Grid1.SortField = e.SortField;
            BindGrid1();
        }

        protected void Grid1_RowDataBound(object sender, FineUIPro.GridRowEventArgs e)
        {
            object obj = ((WareLocationIndexData)e.DataItem).IsOpen;
            int ret = obj == null ? 0 : Convert.ToInt32(obj);
            FineUIPro.LinkButtonField isOpenBtn = Grid1.FindColumn("ChangeOpen") as FineUIPro.LinkButtonField;

            string opened = "已启用";
            string closed = "未启用";

            if (ret == 1)
            {
                isOpenBtn.Icon = Icon.Accept;
                isOpenBtn.Text = opened;
            }
            else
            {
                isOpenBtn.Icon = Icon.Cancel;
                isOpenBtn.Text = closed;
            }
        }

        protected void Grid1_PreRowDataBound(object sender, FineUIPro.GridPreRowEventArgs e)
        {
            object obj = ((WareLocationIndexData)e.DataItem).IsOpen;
            int ret = obj == null ? 0 : Convert.ToInt32(obj);
            FineUIPro.LinkButtonField isOpenBtn = Grid1.FindColumn("ChangeOpen") as FineUIPro.LinkButtonField;

            string opened = "已启用";
            string closed = "未启用";

            if (ret == 1)
            {
                isOpenBtn.Icon = Icon.Accept;
                isOpenBtn.Text = opened;
            }
            else
            {
                isOpenBtn.Icon = Icon.Cancel;
                isOpenBtn.Text = closed;
            }
        }

        protected void Grid1_RowCommand(object sender, FineUIPro.GridCommandEventArgs e)
        {
            WareLocationService wareLocationService = new WareLocationService();
            string id = GetSelectedDataKey(Grid1, 0);

            if (e.CommandName == "ChangeOpen")
            {
                object[] keys = Grid1.DataKeys[e.RowIndex];
                int ware_ID = Convert.ToInt32(keys[0]);
                int state = Convert.ToInt32(keys[1]);
                int change = state == 1 ? 0 : 1;
            
                WareLocation wareLocation = wareLocationService.FindById(ware_ID, DbMainSlave.Master);
                if (wareLocation.WareArea.WareAreaState == false&&change==1) //表示状态不相同
                {
                    Alert.Show($"请先启用{wareLocation.WareArea.WareNo}");
                    return;
                }
                    wareLocation.IsOpen = change;
                wareLocationService.SaveChanges();
                BindGrid3();
                BindGrid1();
            }
            else if (e.CommandName == "ChangeTrayNo")
            {

            }
        }

        protected void Window2_Close(object sender, FineUIPro.WindowCloseEventArgs e)
        {
            BindGrid1();
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            BindGrid1();
        }

        protected void Grid3_RowSelect(object sender, FineUIPro.GridRowSelectEventArgs e)
        {
            BindGrid1();
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            BindGrid1();
        }
    }
}
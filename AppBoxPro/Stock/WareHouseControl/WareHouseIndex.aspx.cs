using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using EntityFramework.Extensions;
using System.Linq.Expressions;
using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;

namespace GeLiPage_WMS.Stock.WareHouseControl
{
    public partial class WareHouseIndex : PageBase
    {
        #region ViewPower

        /// <summary>
        /// 本页面的浏览权限，空字符串表示本页面不受权限控制
        /// </summary>
        public override string ViewPower
        {
            get
            {
                return "WareHouseView";
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
            // 权限检查
            CheckPowerWithButton("WareHouseNew", btnNew);
            //CheckPowerDeleteWithButton(btnDeleteSelected);

            //ResolveDeleteButtonForGrid(btnDeleteSelected, Grid1);

            btnNew.OnClientClick = Window1.GetShowReference("~/Stock/WareHouseControl/WareHouse_new.aspx", "新增仓库");

            // 每页记录数
            Grid1.PageSize = ConfigHelper.PageSize;

            BindGrid();
        }

        private void BindGrid()
        {
            
            Expression<Func<WareHouse, bool>> expression = DbBaseExpand.True<WareHouse>();

            //在仓库名称中搜索
            string searchText = ttbSearchMessage.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                expression = expression.And(t => t.WHName.Contains(searchText));
            }
            var q = wareHouseService.GetWareHouses(expression);

            //在查询添加之后，排序和分页之前获取总记录数
            Grid1.RecordCount = q.Count();
            //排列和分页
            q = SortAndPage(q, Grid1);
            Grid1.DataSource = q;
            Grid1.DataBind();
        }

        #endregion

        #region Events

        protected void ttbSearchMessage_Trigger2Click(object sender, EventArgs e)
        {
            ttbSearchMessage.ShowTrigger1 = true;
            BindGrid();
        }

        protected void ttbSearchMessage_Trigger1Click(object sender, EventArgs e)
        {
            ttbSearchMessage.Text = String.Empty;
            ttbSearchMessage.ShowTrigger1 = false;
            BindGrid();
        }

        protected void Grid1_PreDataBound(object sender, EventArgs e)
        {
            // 数据绑定之前，进行权限检查
            CheckPowerWithWindowField("WareHouseEdit", Grid1, "editField");
            CheckPowerWithLinkButtonField("WareHouseDelete", Grid1, "deleteField");
        }


        protected void Grid1_Sort(object sender, GridSortEventArgs e)
        {
            Grid1.SortDirection = e.SortDirection;
            Grid1.SortField = e.SortField;
            BindGrid();
        }

        protected void Grid1_PageIndexChange(object sender, GridPageEventArgs e)
        {
            Grid1.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void Grid1_RowCommand(object sender, GridCommandEventArgs e)
        {
            //int titleID = GetSelectedDataKeyID(Grid1);

            //if (e.CommandName == "Delete")
            //{
            //    // 在操作之前进行权限检查
            //    if (!CheckPower("CoreTitleDelete"))
            //    {
            //        CheckPowerFailWithAlert();
            //        return;
            //    }

            //    //int userCount = DB2.WareHouse.Where(u => u..Any(t => t.ID == titleID)).Count();
            //    //if (userCount > 0)
            //    //{
            //    //    Alert.ShowInTop("删除失败！需要先清空拥有此职务的用户！");
            //    //    return;
            //    //}

            //    DB2.WareHouse.Where(t => t.ID == titleID).Delete();

            //    BindGrid();
            //}
        }

        protected void Window1_Close(object sender, EventArgs e)
        {
            BindGrid();
        }

        #endregion

    }
}

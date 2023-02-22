using EntityFramework.Extensions;
using FineUIPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NanXingGuoRen_WMS.Stock
{
    public partial class WareLocationIndex : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            btnNew.OnClientClick = Window1.GetShowReference("~/Stock/WareLocationNew.aspx", "新增库区信息");
            BindGrid();
        }

        private void BindGrid()
        {
            var q = from a in DB2.WareLocation
                    orderby a.WareLocaNo
                    select new
                    {
                        a.ID,
                        a.WareLocaNo,
                        a.WareArea.WareNo,
                        a.WareArea.WareAreaClass.AreaClass,
                        a.Users.ChineseName,
                        a.WareLocaState,
                        a.AGVPosition,
                    };
            Grid1.RecordCount = q.Count();
         //   q = SortAndPage(q.AsQueryable(), Grid1);
            Grid1.DataSource = q;
            Grid1.DataBind();
        }

        protected void Grid1_PreDataBound(object sender, EventArgs e)
        {

        }

        protected void Grid1_RowCommand(object sender, GridCommandEventArgs e)
        {
            int menuID = GetSelectedDataKeyID(Grid1);
            if (e.CommandName == "Delete")
            {
                DB2.WareLocation.Where(m => m.ID == menuID).Delete();
                BindGrid();
            }
        }

        protected void Window1_Close(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void Grid1_PageIndexChange(object sender, GridPageEventArgs e)
        {
            Grid1.PageIndex = e.NewPageIndex;
            BindGrid();
        }
    }
}
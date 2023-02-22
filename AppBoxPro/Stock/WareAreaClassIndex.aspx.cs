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
    public partial class WareAreaClassIndex : PageBase
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
            btnNew.OnClientClick = Window1.GetShowReference("~/Stock/WareAreaClassNew.aspx", "新增货位类型");
            BindGrid();
        }

        private void BindGrid()
        {
            var q = DB2.WareAreaClass.OrderBy(u=>u.SortIndex);
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

                DB2.WareAreaClass.Where(m => m.ID == menuID).Delete();

                BindGrid();
            }
        }

        protected void Window1_Close(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
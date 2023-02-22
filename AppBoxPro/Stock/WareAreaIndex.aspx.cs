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
    public partial class WareAreaIndex : PageBase
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
            btnNew.OnClientClick = Window1.GetShowReference("~/Stock/WareAreaNew.aspx", "新增库区信息");
            BindGrid();
        }

        private void BindGrid()
        {
            var q = from a in DB2.WareArea
                    orderby a.WareNo
                    select new
                    {
                        a.ID,
                        a.WareNo,
                        a.WareAreaClass.AreaClass,
                        a.WareHouse.WHName,
                        a.WareAreaState,
                    };
                    
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
                DB2.WareArea.Where(m => m.ID == menuID).Delete();
                BindGrid();
            }
        }

        protected void Window1_Close(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
using FineUIPro;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NanXingGuoRen_WMS.Stock
{
    public partial class WareAreaNew : PageBase
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
            btnClose.OnClientClick = ActiveWindow.GetHideReference();
            LoadWareHouse();
            LoadWareAreaClass();
        }

        private void LoadWareHouse()
        {
            var q = DB2.WareHouse;
            ddl_WareHouse.DataTextField = "WHName";
            ddl_WareHouse.DataValueField = "ID";
            ddl_WareHouse.DataSource = q;
            ddl_WareHouse.DataBind();
        }

        private void LoadWareAreaClass()
        {
            var q = DB2.WareAreaClass;
            ddl_WareAreaClass.DataTextField = "AreaClass";
            ddl_WareAreaClass.DataValueField = "ID";
            ddl_WareAreaClass.DataSource = q;
            ddl_WareAreaClass.DataBind();
        }

        private void SaveItem()
        {
            WareArea item = new WareArea();
            item.WareNo = tbxName.Text.Trim();
            //item.BigClass = tbxBigClass.SelectedText.Trim();
            int wareHouseId= Convert.ToInt32(ddl_WareHouse.SelectedValue);
            item.WareHouse = DB2.WareHouse.Where(u => u.ID == wareHouseId).FirstOrDefault();
            item.WareHouse_ID = wareHouseId;
            int wareAreaClassId = Convert.ToInt32(ddl_WareAreaClass.SelectedValue);
            item.War_ID = wareAreaClassId;
            item.WareAreaClass = DB2.WareAreaClass.Where(u => u.ID == wareAreaClassId).FirstOrDefault();
            item.WareAreaState = cbStates.Checked;
            //item. = tbxRemark.Text.Trim();

            DB2.WareArea.Add(item);
            DB2.SaveChanges();
        }

        protected void btnSaveClose_Click(object sender, EventArgs e)
        {
            SaveItem();
            //Alert.Show("添加成功！", String.Empty, ActiveWindow.GetHidePostBackReference());
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }
    }
}
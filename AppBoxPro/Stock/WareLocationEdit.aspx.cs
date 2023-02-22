using FineUIPro;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NanXingGuoRen_WMS.Stock
{
    public partial class WareLocationEdit : PageBase
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
            BindGrid();
        }

        private void BindGrid()
        {
            int id = GetQueryIntValue("id");
            WareLocation q = DB2.WareLocation.Where(u => u.ID == id).FirstOrDefault();
            ddl_WareArea.SelectedValue = q.WareArea_ID.ToString();
            ddl_UserName.SelectedValue = q.Users.ID.ToString();
            cbStates.Checked = q.WareLocaState??false;
            tbxNo.Text = q.WareLocaNo ;
            tbxAGVPosition.Text = q.AGVPosition;
        }

        private void LoadWareHouse()
        {
            
            var q = DB2.WareArea;
            ddl_WareArea.DataTextField = "WareNo";
            ddl_WareArea.DataValueField = "ID";
            ddl_WareArea.DataSource = q;
            ddl_WareArea.DataBind();
        }

        private void LoadWareAreaClass()
        {
            var q = DB2.Users.Where(u=>u.Depts.Name=="仓库部");
            ddl_UserName.DataTextField = "Name";
            ddl_UserName.DataValueField = "ID";
            ddl_UserName.DataSource = q;
            ddl_UserName.DataBind();
        }

        private void SaveItem()
        {
            int id = GetQueryIntValue("id");
            WareLocation item = DB2.WareLocation.Where(u => u.ID == id).FirstOrDefault();
            item.WareLocaNo = tbxNo.Text.Trim();
            item.AGVPosition = tbxAGVPosition.Text.Trim();

            //item.BigClass = tbxBigClass.SelectedText.Trim();
            int wareAreaId = Convert.ToInt32(ddl_WareArea.SelectedValue);
            item.WareArea = DB2.WareArea.Where(u => u.ID == wareAreaId).FirstOrDefault();
            item.WareArea_ID = wareAreaId;
            int userId = Convert.ToInt32(ddl_UserName.SelectedValue);
            //item.Users = wareAreaClassId;
            item.header_ID = userId;
            item.Users = DB2.Users.Where(u => u.ID == userId).FirstOrDefault();
            item.WareLocaState = cbStates.Checked;
            //item. = tbxRemark.Text.Trim();

            //DB2.WareLocation.Add(item);
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
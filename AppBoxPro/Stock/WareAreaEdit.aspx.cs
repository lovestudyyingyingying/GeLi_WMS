using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;


namespace NanXingGuoRen_WMS.Stock
{
    public partial class WareAreaEdit : PageBase
    {
        #region ViewPower

        /// <summary>
        /// 本页面的浏览权限，空字符串表示本页面不受权限控制
        /// </summary>
        public override string ViewPower
        {
            get
            {
                return "CoreTitleEdit";
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
            btnClose.OnClientClick = ActiveWindow.GetHideReference();

            int id = GetQueryIntValue("id");
            WareArea wareArea = DB2.WareArea.Find(id);
            if (wareArea == null)
            {
                // 参数错误，首先弹出Alert对话框然后关闭弹出窗口
                Alert.Show("参数错误！", String.Empty, ActiveWindow.GetHideReference());
                return;
            }
            LoadWareHouse();
            LoadWareAreaClass();
            tbxName.Text = wareArea.WareNo;
            ddl_WareAreaClass.SelectedValue = wareArea.WareAreaClass.ID.ToString();
            ddl_WareHouse.SelectedValue = wareArea.WareHouse.ID.ToString();
            cbStates.Checked = wareArea.WareAreaState ??false;
            //tbxRemark.Text = wareArea.Remark;
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

        #endregion

        #region Events

        protected void btnSaveClose_Click(object sender, EventArgs e)
        {
            int id = GetQueryIntValue("id");
            
            WareArea item = DB2.WareArea.Find(id);
            item.WareNo = tbxName.Text.Trim();
            //item.BigClass = tbxBigClass.SelectedText.Trim();
            int wareHouseId = Convert.ToInt32(ddl_WareHouse.SelectedValue);
            item.WareHouse = DB2.WareHouse.Where(u => u.ID == wareHouseId).FirstOrDefault();
            item.WareHouse_ID = wareHouseId;
            int wareAreaClassId = Convert.ToInt32(ddl_WareAreaClass.SelectedValue);
            item.War_ID = wareAreaClassId;
            item.WareAreaClass = DB2.WareAreaClass.Where(u => u.ID == wareAreaClassId).FirstOrDefault();
            item.WareAreaState = cbStates.Checked;
            DB2.SaveChanges();

            //FineUIPro.Alert.Show("保存成功！", String.Empty, FineUIPro.Alert.DefaultIcon, FineUIPro.ActiveWindow.GetHidePostBackReference());
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }

        #endregion

    }
}

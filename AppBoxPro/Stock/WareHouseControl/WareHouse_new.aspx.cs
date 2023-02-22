using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using NanXingData_WMS.Dao;

namespace GeLiPage_WMS.Stock.WareHouseControl
{
    public partial class WareHouse_new : PageBase
    {
        #region ViewPower

        /// <summary>
        /// 本页面的浏览权限，空字符串表示本页面不受权限控制
        /// </summary>
        public override string ViewPower
        {
            get
            {
                return "WareHouseNew";
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

        }

        #endregion

        #region Events

        private void SaveJobTitle()
        {
            WareHouse item = new WareHouse();
            item.WHName = tbxName.Text.Trim();
            item.WHPosition = tbxPosition.Text.Trim();
            item.WHState = cbStates.Checked;
            item.Remark = tbxRemark.Text.Trim();

            wareHouseService.AddWareHouses(item);
        }

        protected void btnSaveClose_Click(object sender, EventArgs e)
        {
            SaveJobTitle();

            //Alert.Show("添加成功！", String.Empty, ActiveWindow.GetHidePostBackReference());
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }
        #endregion

    }
}

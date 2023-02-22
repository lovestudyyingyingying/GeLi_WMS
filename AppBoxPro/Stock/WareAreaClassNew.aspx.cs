using FineUIPro;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NanXingGuoRen_WMS.Stock
{
    public partial class WareAreaClassNew : PageBase
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

        }

        private void SaveItem()
        {
            WareAreaClass item = new WareAreaClass();
            item.AreaClass = tbxName.Text.Trim();
            //item.BigClass = tbxBigClass.SelectedText.Trim();
            item.SortIndex = Convert.ToInt32(tbxSortIndex.Text.Trim());
            item.Remark = tbxRemark.Text.Trim();


            DB2.WareAreaClass.Add(item);
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
using FineUIPro;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace NanXingGuoRen_WMS.Stock
{
    public partial class WareAreaClassEdit : PageBase
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
            int ID = GetQueryIntValue("ID");
            WareAreaClass item = DB2.WareAreaClass.Where(u => u.ID == ID).FirstOrDefault();
            tbxName.Text = item.AreaClass;
            tbxSortIndex.Text = item.SortIndex.ToString();
            //tbxBigClass.Text = item.BigClass.Trim();
            tbxRemark.Text = item.Remark;
        }

        private void SaveItem()
        {
            int ID = GetQueryIntValue("ID");
            WareAreaClass item = DB2.WareAreaClass.Where(u => u.ID == ID).FirstOrDefault();
            item.AreaClass = tbxName.Text.Trim();
            //item.BigClass=tbxBigClass.SelectedText.Trim();
            item.SortIndex = Convert.ToInt32(tbxSortIndex.Text.Trim());
            item.Remark = tbxRemark.Text.Trim();
            DB2.SaveChanges();
        }

        protected void btnSaveClose_Click(object sender, EventArgs e)
        {
            SaveItem();
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }
    }
}
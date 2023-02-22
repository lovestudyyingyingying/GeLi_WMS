using FineUIPro;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NanXingGuoRen_WMS.ProductReport
{
    public partial class BatchNoWeightReport : PageBase
    {
        //key-列名  value-值
        static Dictionary<string, string> dictClickColsName = new Dictionary<string, string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dictClickColsName.Clear();
                LoadData();
            }
        }

        private void LoadData()
        {
            BindDDLClass();
            BindGrid1();
        }

        private void BindDDLClass()
        {
            //var q = from a in DB2.
            //        select a;

            //if (ddlBigClass.SelectedIndex >= 0)
            //{
            //    string value = ddlBigClass.SelectedValue;
            //    if (ddlBigClass.SelectedValue.Contains("其他"))
            //    {
            //        q = q.Where(u => u.BigClass == value || u.BigClass == null);
            //    }
            //    else
            //        q = q.Where(u => u.BigClass == value);

            //}
            //ddlClass.DataTextField = "Name";
            //ddlClass.DataValueField = "Name";

            //ddlClass.DataSource = q;
            //ddlClass.DataBind();
        }
        protected void ddlBigClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDDLClass();
            //if (ddlBigClass.SelectedValue.Contains("耗材"))
            //{
            //    cbx.Checked = true;
            //    shouhuoCount.Hidden = true;
            //}
            //else if (ddlBigClass.SelectedValue.Contains("硬件"))
            //{
            //    shouhuoCount.Hidden = false;
            //}
            //else
            //{
            //    cbx.Checked = false;
            //    shouhuoCount.Hidden = true;
            //}


            //length.Hidden = !cbx.Checked;
            //width.Hidden = !cbx.Checked;

            //high.Hidden = !cbx.Checked;
            //instockhigh.Hidden = !cbx.Checked;
            //notinstockhigh.Hidden = !cbx.Checked;
            BindGrid1();
            //cbx_CheckedChanged(sender, (CheckedEventArgs)e);
        }
        

        // --and proname like '%{0}%' and spec like '%{1}%' and probiaozhun like '%{2}%' and batchNo like '%{3}%'
        private void BindGrid1()
        {
            string dp1Str = string.Empty;
            string dp2Str = string.Empty;

            //string guanjianci = string.Empty;
            //if (tbxSearch.Text.Trim().Length>0) 
            // guanjianci = string.Format(
            //    @" where ( a.proname like '%{0}%' or a.spec like '%{0}%' or a.probiaozhun like '%{0}%' or a.batchNo like '%{0}%')"
            //    , tbxSearch.Text.Trim());


            var q = from a in DB2.TrayWeightRecord 
                    orderby a.RecTime descending
                    select new {
                a.ID
              ,a.position
              ,a.RecTime
              ,a.trayWeight
              ,a.trayCount
              ,a.result
              ,a.itemno
              ,a.batchNo
              ,a.proname
              ,a.spec
              ,a.prosn
              ,a.userID
              ,a.biaozhun
              ,a.color
              ,a.boxName
            };
            if (dp1.SelectedDate.HasValue && dp2.SelectedDate.HasValue)
            {
                DateTime dtStart = dp1.SelectedDate.Value;
                DateTime dtEnd = dp2.SelectedDate.Value.AddDays(1);
                q = q.Where(u => u.RecTime >= dtStart && u.RecTime <= dtEnd);
            }
            if (!string.IsNullOrEmpty(tbxProname.Text))
            {
                q = q.Where(u => u.proname.Contains( tbxProname.Text));
            }

            if (!string.IsNullOrEmpty(tbxBatchNo.Text))
            {
                q = q.Where(u => u.batchNo.Contains(tbxBatchNo.Text));
            }
            if (!string.IsNullOrEmpty(tbxSpec.Text))
            {
                q = q.Where(u => u.spec.Contains(tbxSpec.Text));
            }
            if (!string.IsNullOrEmpty(tbxBiaoZhun.Text))
            {
                q = q.Where(u => u.biaozhun.Contains(tbxBiaoZhun.Text));
            }
            if (!string.IsNullOrEmpty(tbxColor.Text))
            {
                q = q.Where(u => u.color.Contains(tbxColor.Text));
            }
            //DbHelperSQL.connectionString = ConfigurationManager.ConnectionStrings["Default"].ToString();

            //DataTable dt = DbHelperSQL.ReturnDataTable(sql);

            Grid1.RecordCount = q.Count();
            q = SortAndPage(q, Grid1);
            //dt = GetPagedDataTable(dt, Grid1);
            Grid1.DataSource = q;
            Grid1.DataBind();

        }

        protected void Grid1_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            Grid1.PageIndex = e.NewPageIndex;
            BindGrid1();
        }

        protected void Grid1_Sort(object sender, FineUIPro.GridSortEventArgs e)
        {
            Grid1.SortDirection = e.SortDirection;
            Grid1.SortField = e.SortField;
            BindGrid1();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid1();
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            Grid1.PageSize = 1000;
            BindGrid1();
            ExportExcel("入库明细-"+DateTime.Now.ToString("yyyyMMdd"), Grid1);
        }

        protected void Grid1_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            int s = GetSelectedDataKeyID( Grid1);
            //s[0] 选中的行id, s[1] 选中的ColumnID
            PageContext.RegisterStartupScript(Window1.GetShowReference("~/ProductionOrder/ProductionOrderEdit.aspx?id=" + s, "编辑", 1500, 900));
            //BindGrid1();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            //移除最后一个key
            if (dictClickColsName.Count > 0)
            {
                string lastKey = dictClickColsName.Last().Key;
                dictClickColsName.Remove(lastKey);
                BindGrid1();
            }
            else
            {
                BindGrid1();
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            dictClickColsName.Clear();
            BindGrid1();
        }

        protected void Grid1_RowCommand(object sender, GridCommandEventArgs e)
        {
            string prosn = GetSelectedDataKey(Grid1, 1);

            if (e.CommandName == "Print")
            {
                //Print(prosn);
                PageContext.RegisterStartupScript(Window1.GetShowReference("~/ProductionOrder/ProductionOrder_Print.aspx?prosn=" + prosn, "打印",900, 700));
            }
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            BindGrid1();
        }

        protected void btnInstockBatch_Click(object sender, EventArgs e)
        {
            if (Grid1.SelectedRowIndexArray.Length == 0)
            {
                ShowNotify("没有选中任何行");
                return;
            }
            string bigClass = string.Empty;
            bool ret = true;
            foreach (int rowIndex in Grid1.SelectedRowIndexArray)
            {
                if (bigClass == string.Empty)
                {
                    bigClass = Grid1.DataKeys[rowIndex][1].ToString();
                }
                else if (bigClass != Grid1.DataKeys[rowIndex][1].ToString())
                {
                    ret = false;
                    break;
                }
            }
            //Debug.WriteLine((bigClass == "耗材" && !Grid1.DataKeys[0][2].ToString().Contains("碳带") && !Grid1.DataKeys[0][2].ToString().Contains("标签")));
            if (ret)
            {
                List<int> ids = GetSelectedDataKeyIDs(Grid1);
                //把ids传过去
                if (bigClass == "硬件")
                {
                    PageContext.RegisterStartupScript(Window2.GetShowReference("~/Stock/PurchaseInstockNewPatchHardware.aspx?id=" + ids[0], "硬件类批量入库"));
                }
                else if (string.IsNullOrEmpty(bigClass) || bigClass == "其他" ||
                    (bigClass == "耗材" && !Grid1.DataKeys[0][2].ToString().Contains("碳带") && !Grid1.DataKeys[0][2].ToString().Contains("标签")))
                {
                    PageContext.RegisterStartupScript(Window2.GetShowReference("~/Stock/PurchaseInstockNewPatch.aspx?ids=" + String.Join(",", ids), "批量入库"));
                }
                else if (bigClass == "耗材")
                {
                    PageContext.RegisterStartupScript(Window2.GetShowReference("~/Stock/PurchaseInstockNewPatchLabel.aspx?ids=" + String.Join(",", ids), "标签、碳带类批量入库"));
                }
            }
        }

        protected void Window2_Close(object sender, WindowCloseEventArgs e)
        {
            BindGrid1();
        }

        protected void btnInstockBatchLabel_Click(object sender, EventArgs e)
        {
            if (Grid1.SelectedRowIndexArray.Length == 0)
            {
                ShowNotify("没有选中任何行");
                return;
            }


            //把ids传过去

            List<int> ids = GetSelectedDataKeyIDs(Grid1);

            PageContext.RegisterStartupScript(Window2.GetShowReference("~/Stock/PurchaseInstockNewPatchLabel.aspx?ids=" + String.Join(",", ids), "标签、碳带类批量入库"));
        }

        protected void btnInstcokBatchHardware_Click(object sender, EventArgs e)
        {
            if (Grid1.SelectedRowIndexArray.Length == 0)
            {
                ShowNotify("没有选中任何行");
                return;
            }

            if (Grid1.SelectedRowIndexArray.Length > 1)
            {
                ShowNotify("硬件类入库 请选中单行记录");
                return;
            }


            //把ids传过去

            int id = GetSelectedDataKeyID(Grid1);

            PageContext.RegisterStartupScript(Window2.GetShowReference("~/Stock/PurchaseInstockNewPatchHardware.aspx?id=" + id, "硬件类批量入库"));
        }

        protected void btnclose_Click(object sender, EventArgs e)
        {
            if (Grid1.SelectedRowIndexArray.Length == 0)
            {
                ShowNotify("没有选中任何行");
                return;
            }

            List<int> ids = GetSelectedDataKeyIDs(Grid1);
            foreach (int temp in ids) {
                //PO_Item po_Item = DB.po_items.Where(a => a.ID == temp).FirstOrDefault();
                //po_Item.isClose = 1;
                //DB.SaveChanges();
            }
            BindGrid1();
        }

        protected void btnunclose_Click(object sender, EventArgs e)
        {
            if (Grid1.SelectedRowIndexArray.Length == 0)
            {
                ShowNotify("没有选中任何行");
                return;
            }

            List<int> ids = GetSelectedDataKeyIDs(Grid1);
            foreach (int temp in ids)
            {
                //PO_Item po_Item = DB.po_items.Where(a => a.ID == temp).FirstOrDefault();
                //po_Item.isClose = 0;
                //DB.SaveChanges();
            }
            BindGrid1();
        }



        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid1();
        }

        protected void cbx2_CheckedChanged(object sender, CheckedEventArgs e)
        {

        }


       
    }
}
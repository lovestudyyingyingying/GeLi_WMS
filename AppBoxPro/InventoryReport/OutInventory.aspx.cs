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

namespace NanXingGuoRen_WMS.InventoryReport
{
    public partial class OutInventory : PageBase
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
            BindGrid1();
        }

        // --and proname like '%{0}%' and spec like '%{1}%' and probiaozhun like '%{2}%' and batchNo like '%{3}%'
        private void BindGrid1()
        {
            string dp1Str = string.Empty;
            string dp2Str = string.Empty;

            if (dp1.SelectedDate.HasValue)
                dp1Str = dp1.SelectedDate.Value.ToString("yyyy-MM-dd 00:00:00");
            else
                dp1Str = "2000-01-01 00:00:00";


            if (dp2.SelectedDate.HasValue)
                dp2Str = dp2.SelectedDate.Value.ToString("yyyy-MM-dd 23:59:59");
            else
                dp2Str = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");

            //string sql = string.Format(
            //    @"   select  OrderTime,StartLocation,EndLocation,RunState,
            //      SendState,MissionNo,a.TrayNo,Mark,StockPlan_ID,
            //      proname,batchNo,spec,color,probiaozhun,procount
            //      ,'箱' unit from
            //      (select  OrderTime,StartLocation,EndLocation,RunState,
            //      SendState,MissionNo,TrayNo,Mark,StockPlan_ID,AGVCarId
            //      from [NanXingGuoRen_WMS].[dbo].[AGVMissionInfo]
            //      where Mark='03' and RunState='已完成' )a
            //      left join 
            //      (select TrayNO,proname,ProductOrderlistsID,unit,isnull(spec,'')spec,
            //      batchNo,isnull(color,'')color,isnull(probiaozhun,'')probiaozhun,COUNT(*) procount
            //       from traystate,traypro,production 
            //      where TrayState.ID=TrayPro.TrayStateID
            //      and TrayPro.prosn=Production.prosn
            //      group by TrayNO,proname,ProductOrderlistsID,unit,spec,batchNo,color,probiaozhun)b 
            //      on a.TrayNo=b.TrayNO
            //      where orderTime between '{5}' and '{6}' and
            //        proname like '%{0}%' and spec like '%{1}%' and probiaozhun like '%{2}%' and batchNo like '%{3}%'
            //      and color like '%{4}%'
            //      order by ordertime desc",
            string sql = string.Format(@" select FaHuodanhao, probar,makedate,
                itemname,itemno,spec,lotno,danjianwt from dbo.probarlist1
                where makedate between '{0}' and '{1}' "
                ,dp1Str,dp2Str);
            //proname like '%{0}%' and spec like '%{1}%' and probiaozhun like '%{2}%' and batchNo like '%{3}%'
            //Debug.WriteLine(sql);
            if (tbxProname.Text.Trim().Length>0)
            {
                sql += $" and itemname like '%{tbxProname.Text.Trim()}%'";
            }
            if (tbxSpec.Text.Trim().Length > 0)
            {
                sql += $" and spec like '%{tbxSpec.Text.Trim()}%'";
            }
            if (tbxBiaoZhun.Text.Trim().Length > 0)
            {
                sql += $" and lotno like '%{tbxBiaoZhun.Text.Trim()}%'";
            }
            if (tbxBatchNo.Text.Trim().Length > 0)
            {
                sql += $" and FaHuodanhao like '%{tbxBatchNo.Text.Trim()}%'";
            }

            DbHelperSQL.connectionString = ConfigurationManager.ConnectionStrings["Default"].ToString();

            DataTable dt = DbHelperSQL.ReturnDataTable(sql);
           
            Grid1.RecordCount = dt.Rows.Count;
            dt = GetPagedDataTable(dt, Grid1);
            Grid1.DataSource = dt;
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
            ExportExcel("出库明细-"+DateTime.Now.ToString("yyyyMMdd"), Grid1);
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

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            BindGrid1();
        }

        protected void Window2_Close(object sender, WindowCloseEventArgs e)
        {
            BindGrid1();
        }
    }
}
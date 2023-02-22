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
    public partial class OnlineInventory : PageBase
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

            string guanjianci = string.Empty;
            if (tbxSearch.Text.Trim().Length>0) 
             guanjianci = string.Format(
                @" where ( a.proname like '%{0}%' or a.spec like '%{0}%' or a.probiaozhun like '%{0}%' or a.batchNo like '%{0}%')"
                , tbxSearch.Text.Trim());


            string sql = string.Format(
                @"select a.batchNo,a.spec,a.probiaozhun,a.prodate,allCount ,allCount-isnull(b.prepareCount,0) usCount,a.proname,a.color
                ,'箱' unit from
                (select batchNo ,spec ,probiaozhun ,
                convert(nvarchar,prodate,23) prodate,count(TrayState.OnlineCount) allCount,proname ,color
                from TrayState,TrayPro,Production,WareLocation
                where TrayState.ID=TrayPro.TrayStateID and TrayPro.prosn=production.prosn and WareLocation_ID=WareLocation.ID
                and len(proname) >0 and prodate between '{4}' and '{5}' 
                and proname like '%{0}%' and spec like '%{1}%' and probiaozhun like '%{2}%' and batchNo like '%{3}%'
                group by proname,spec,probiaozhun,batchNo,prodate,color
                )a
                left join (
                select c.proname ,c.probiaozhun,c.batchNo,c.color,c.spec,SUM(OnlineCount) prepareCount,e.prodate
                from  AGVMissionInfo a,StockPlan c,
                (select b.trayno,e.proname ,e.probiaozhun,e.batchNo,e.color,e.spec,e.prodate,COUNT(d.prosn) OnlineCount
                 from TrayState b,TrayPro d,Production e 
                where  b.ID=d.TrayStateID and d.prosn=e.prosn
                group by b.trayno,e.proname ,e.probiaozhun,e.batchNo,e.color,e.spec,e.prodate)e
                where a.mark='03' and a.TrayNo=e.TrayNO and  a.StockPlan_ID=c.ID
                and SendState='成功'  and (RunState !='已完成' and RunState!='已取消' and RunState!='执行失败'
                and RunState!='发送失败')
                GROUP BY 
                c.proname ,c.probiaozhun,c.batchNo,c.color,c.spec,e.prodate)
                b
                on a.proname=b.proname and a.spec=b.spec and a.batchNo=b.batchNo and a.probiaozhun=b.probiaozhun
                {6}
                ORDER BY prodate ASC,proname,spec,probiaozhun",
                tbxProname.Text.Trim(), tbxSpec.Text.Trim(), tbxBiaoZhun.Text.Trim(), tbxBatchNo.Text.Trim(),
                dp1Str,dp2Str,guanjianci);
            //proname like '%{0}%' and spec like '%{1}%' and probiaozhun like '%{2}%' and batchNo like '%{3}%'

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
            ExportExcel("库存报表-"+DateTime.Now.ToString("yyyyMMdd"), Grid1);
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
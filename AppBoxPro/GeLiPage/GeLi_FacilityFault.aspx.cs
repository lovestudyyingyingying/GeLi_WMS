using GeLiData_WMS;
using GeLiData_WMSUtils;
using GeLiPage_WMS;
using GeLiPage_WMS.DictornaryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GeLiPage_WMS.GeLiPage
{
    public partial class GeLi_FacilityFault : PageBase
    {
        /// <summary>
        /// 本页面的浏览权限，空字符串表示本页面不受权限控制
        /// </summary>
        public override string ViewPower
        {
            get
            {
                return "GeLi_FacilityFault-View";
            }
        }
        public static List<MagnifyingFindDictionary> keyValuePairsSearch = new List<MagnifyingFindDictionary>();
        //key-列名  value-值
        static Dictionary<string, string> dictClickColsName = new Dictionary<string, string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                keyValuePairsSearch.Clear();
                dictClickColsName.Clear();
                LoadData();
            }
        }
        private void LoadData()
        {
            dp1.SelectedDate = DateTime.Now.AddDays(-15);
            dp2.SelectedDate = DateTime.Now.AddDays(15);
            BindGrid1();
        }
        

        private void BindGrid1(string Excel = null)
        {

            
            string[] arr = null;
            
            Expression<Func<AGVAlarmLog, bool>> expression = DbBaseExpand.True<AGVAlarmLog>();
           
            DbBase<AGVAlarmLog> aGVAlarmLogDbBase = new DbBase<AGVAlarmLog>();
            if (DDL_DateType.SelectedValue.Trim()=="报警日期")
            {
                expression = expression.And(u => u.alarmDate != null && u.alarmDate >= dp1.SelectedDate.Value && u.alarmDate <= dp2.SelectedDate.Value);
            }
            else if (DDL_DateType.SelectedValue.Trim() == "接收日期")
            {
                expression = expression.And(u => u.recTime != null && u.recTime >= dp1.SelectedDate.Value && u.recTime <= dp2.SelectedDate.Value);
            }

            if (deviceFault.SelectedValue.Trim()=="全部")
            {
                arr = null;
            }
            else
            {
                arr = new string[1] { deviceFault.SelectedValue };
            }

            if (arr != null && arr.Length > 0)
            {
                
                for (int i = 0; i < arr.Length; i++)
                {
                    var alltemp = arr[i].Trim().ToString();
                    expression = expression.And(u => u.deviceName.Contains(alltemp));
                }
            }
                
            var aGVAlarmLogs = aGVAlarmLogDbBase.GetIQueryable(expression);


        
            if (!string.IsNullOrEmpty(faultDesc.Text))
                aGVAlarmLogs = aGVAlarmLogs.Where(u => u.alarmDesc!= null && u.alarmDesc.Contains(faultDesc.Text));

            
            Grid1.RecordCount = aGVAlarmLogs.Count();
            if (Excel != "导出")
            {
                aGVAlarmLogs = SortAndPage(aGVAlarmLogs, Grid1);
            }


            aGVAlarmLogs = TableBindGrid<AGVAlarmLog>(aGVAlarmLogs, Grid1, keyValuePairsSearch);


            Grid1.DataSource = aGVAlarmLogs;
            Grid1.DataBind();
        }
        protected void Grid1_Sort(object sender, FineUIPro.GridSortEventArgs e)
        {
            Grid1.SortDirection = e.SortDirection;
            Grid1.SortField = e.SortField;
            BindGrid1();
        }

        protected void Grid1_RowCommand(object sender, FineUIPro.GridCommandEventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid1();
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            List<int> selections = GetSelectedDataKeyIDs(Grid1);
            if (selections.Count() == 0)
            {
                Grid1.PageSize = Grid1.RecordCount;
                BindGrid1("导出");
            }
            else
            {

                Expression<Func<AGVAlarmLog, bool>> expression = DbBaseExpand.True<AGVAlarmLog>();
                DbBase<AGVAlarmLog> aGVAlarmLogDbBase = new DbBase<AGVAlarmLog>();
                expression = expression.And(u => selections.Contains(u.ID));

                List<AGVAlarmLog> q = aGVAlarmLogDbBase.GetList(expression);


                Grid1.DataSource = q;

                Grid1.DataBind();
            }
            string NowMonth = DateTime.Now.Month.ToString();
            if (NowMonth.Length < 2 && NowMonth.Length > 0)
            {
                NowMonth = "0" + NowMonth;
            }
            string NowDay = DateTime.Now.Day.ToString();
            if (NowDay.Length < 2 && NowDay.Length > 0)
            {
                NowDay = "0" + NowDay;
            }
            string NowHour = DateTime.Now.Hour.ToString();
            if (NowHour.Length < 2 && NowHour.Length > 0)
            {
                NowHour = "0" + NowHour;
            }
            string NowMinute = DateTime.Now.Minute.ToString();
            if (NowMinute.Length < 2 && NowMinute.Length > 0)
            {
                NowMinute = "0" + NowMinute;
            }
            string NowSecond = DateTime.Now.Second.ToString();
            if (NowSecond.Length < 2 && NowSecond.Length > 0)
            {
                NowSecond = "0" + NowSecond;
            }
            string NowTime = DateTime.Now.Year.ToString() + NowMonth
                + NowDay + "_" + NowHour
                + NowMinute + NowSecond;
            ExportExcel(Grid1.Title + NowTime, Grid1);
        }

        protected void Grid1_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            Grid1.PageIndex = e.NewPageIndex;
            BindGrid1();
        }

        protected void Grid1_RowDataBound(object sender, FineUIPro.GridRowEventArgs e)
        {

        }
    }
}
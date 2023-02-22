using FineUIPro;

using NanXingGuoRen_WMS.Business.Helper;
using Newtonsoft.Json.Linq;
using SQLHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NanXingGuoRen_WMS.ProductReport.TouLiao1
{
    public partial class TouLiaoNew : PageBase
    {
        private bool AppendToEnd = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //datepicker1.SelectedDate = DateTime.Now;
                // 删除选中单元格的客户端脚本
                string deleteScript = GetDeleteScript();

                // 新增数据初始值
                JObject defaultObj = new JObject();

                // 在第一行新增一条数据
                //btnNew.OnClientClick = Grid1.GetAddNewRecordReference(defaultObj, AppendToEnd);

                // 重置表格
                //btnReset.OnClientClick = Confirm.GetShowReference("确定要重置表格数据？", String.Empty, Grid1.GetRejectChangesReference(), String.Empty);

                // 删除选中行按钮
                //btnDelete.OnClientClick = Grid1.GetNoSelectionAlertReference("请至少选择一项！") + deleteScript;
                LoadExcel();
                LoadData();

                BindDDLClient();
            }
        }

        private void BindDDLClient()
        {
        }

        private void LoadExcel()
        {
           
        }

        private string GetDeleteScript()
        {
            return Confirm.GetShowReference("删除选中行？", String.Empty, MessageBoxIcon.Question, Grid2.GetDeleteSelectedRowsReference(), String.Empty);
        }

        private void LoadData()
        {
            //BindGrid2();
            BindGridDDLProvider();
        }

        private void BindGridDDLProvider()
        {
            var q = DB2.Depts.Where(u=>u.Depts2.Name=="生产部");
            ddl_Class.DataTextField = "Name";
            ddl_Class.DataValueField = "Name";
            ddl_Class.DataSource = q;
            ddl_Class.DataBind();
        }

       
        private string ReplaceTeShu(string str)
        {
            str = str.Replace(" ", ",");
            str = str.Replace("，", ",");
            str = str.Replace("、", ",");
            str = str.Replace("；", ",");
            str = str.Replace(";", ",");
            return str;
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            //新增数据
            List<Dictionary<string, object>> newAddedList = Grid2.GetNewAddedList();
           
            if (newAddedList.Count <= 0)
            {
                Alert.Show("没有新增的数据");
                return;
            }
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < newAddedList.Count; i++)
            {
                DateTime DT = DateTime.Parse(newAddedList[i]["RecTime"].ToString());
                sb.Append(@"insert Productiondt(prosn,prodate,[weight],reserve1,reserve2,lotno,grade,operator)" +
                $"values('{newAddedList[i]["prosn"].ToString()}','{DT.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss")}'," +
                $"'{newAddedList[i]["weight"].ToString()}','{newAddedList[i]["reserve1"].ToString()}'," +
                $"'{newAddedList[i]["reserve2"].ToString()}','{newAddedList[i]["lotno"].ToString()}'," +
                $"'{newAddedList[i]["grade"].ToString()}','{newAddedList[i]["operator"].ToString()}') ");

                TouLiaoRecord item = new TouLiaoRecord();

                if (newAddedList[i].ContainsKey("prosn")) item.prosn = newAddedList[i]["prosn"].ToString();
                if (newAddedList[i].ContainsKey("RecTime")) item.RecTime = DateTime.Parse( newAddedList[i]["RecTime"].ToString());
                if (newAddedList[i].ContainsKey("userID")) item.userID = newAddedList[i]["userID"].ToString();
                
                DB2.TouLiaoRecord.Add(item);
            }
            DbHelperSQL.connectionString = ConfigurationManager.ConnectionStrings["Default"].ToString();
            DbHelperSQL.ExecuteSql(sb.ToString(),60);
            DB2.SaveChanges();

            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //BindGrid2();
        }

       
        protected void ddl_Class_SelectedIndexChanged(object sender, EventArgs e)
        {
            

        }

       
    }
}
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NanXingGuoRen_WMS.ProductReport.BatchNoSC
{
    public partial class BatchNoSCNew : PageBase
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
            DataTable dt=ExcelUtils.ReadExcel(Server.MapPath(Request.ApplicationPath + "/res/files/排产工艺名.xls"));
            ddlname.DataTextField = "物料名称";
            ddlname.DataValueField = "物料名称";
            ddlname.DataSource = dt;
            ddlname.DataBind();
        }

        private string GetDeleteScript()
        {
            return Confirm.GetShowReference("删除选中行？", String.Empty, MessageBoxIcon.Question, Grid1.GetDeleteSelectedRowsReference(), String.Empty);
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

            if (Grid1.GetNewAddedList().Count <= 0)
            {
                Alert.Show("没有新增的数据");
                return;
            }
            
            //if (!datepicker1.SelectedDate.HasValue ||datepicker1.SelectedDate?.ToString("yyyy-MM-dd") == "1900-01-01")
            //{
            //    Alert.Show("请选择下单日期");
            //    return;
            //}
        
            //新增数据
            List<Dictionary<string, object>> newAddedList = Grid1.GetNewAddedList();
            //Debug.WriteLine(newAddedList[i]["clientname"].ToString());
            
            //Type t = typeof(string);
            //SqlParameter[] sqlParms = new SqlParameter[1];
            //sqlParms[0] = new SqlParameter("@MaintainCate", "Product");
            ////var result = DB2.Database.SqlQuery(t, "exec GetSeq @MaintainCate", sqlParms).Cast<string>().First();
            //var result = DB2.Database.SqlQuery(t, "exec GetSeq_1 @MaintainCate", sqlParms).Cast<string>().First();

            //Debug.WriteLine(DB2.Database.Connection.ConnectionString);
           

            for (int i = 0; i < newAddedList.Count; i++)
            {
                BatchNoPro item = new BatchNoPro();
                if (newAddedList[i].ContainsKey("prosn")) item.prosn = newAddedList[i]["prosn"].ToString();
                if (newAddedList[i].ContainsKey("prodate")) item.prodate = DateTime.Parse( newAddedList[i]["prodate"].ToString());
                if (newAddedList[i].ContainsKey("name")) item.proname = newAddedList[i]["name"].ToString();
                if (newAddedList[i].ContainsKey("itemno")) item.itemno = newAddedList[i]["itemno"].ToString();
                if (newAddedList[i].ContainsKey("class")) item.@class = newAddedList[i]["class"].ToString();
                if (newAddedList[i].ContainsKey("color")) item.color = newAddedList[i]["color"].ToString();
           
                if (newAddedList[i].ContainsKey("spec")) item.spec = newAddedList[i]["spec"].ToString();

                if (newAddedList[i].ContainsKey("unit")) item.unit = newAddedList[i]["unit"].ToString();

                
                if (newAddedList[i].ContainsKey("remark1"))
                {
                    string remark = newAddedList[i]["remark1"].ToString().Replace(";\r\n",";").Replace(";", ";\r\n");
                    item.remark1 = remark;
                }

             
                if (newAddedList[i].ContainsKey("ingredients")) item.ingredients = newAddedList[i]["ingredients"].ToString();
                if (newAddedList[i].ContainsKey("boxName")) item.boxName = newAddedList[i]["boxName"].ToString();
                if (newAddedList[i].ContainsKey("batchNo")) item.batchNo = newAddedList[i]["batchNo"].ToString();
                if (newAddedList[i].ContainsKey("boxNo")) item.boxNo = newAddedList[i]["boxNo"].ToString();
                if (newAddedList[i].ContainsKey("position")) item.position = newAddedList[i]["position"].ToString();

                //item.mark = "03";

                //item.chejianclass = ddlCheJian.Text.Trim();

                DB2.BatchNoPro.Add(item);
            }

            DB2.SaveChanges();

            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //BindGrid2();
        }

       
        protected void ddl_Class_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_Class.SelectedValue== "原料车间")
            {
                boxName.Hidden = true;
                boxNo.Hidden = true;
                //ddlCheJian.Hidden = true;
            }
            else if (ddl_Class.SelectedValue == "烘烤车间")
            {
                boxName.Hidden = true;
                boxNo.Hidden = true;
                //ddlCheJian.Hidden = false;
            }
            else if (ddl_Class.SelectedValue.Contains( "包装车间"))
            {
                boxName.Hidden = false;
                boxNo.Hidden = false;
                //ddlCheJian.Hidden = true;

            }

        }

       
    }
}
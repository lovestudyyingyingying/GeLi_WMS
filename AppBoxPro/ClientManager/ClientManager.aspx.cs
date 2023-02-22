using EntityFramework.Extensions;
using FineUIPro;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NanXingGuoRen_WMS.ClientManager
{
    public partial class ClientManager : PageBase
    {
        /// <summary>
        /// 本页面的浏览权限，空字符串表示本页面不受权限控制
        /// </summary>
        public override string ViewPower
        {
            get
            {
                return "";
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            ResolveDeleteButtonForGrid(btnDeleteSelected, Grid1);
            btnNew.OnClientClick = Window1.GetShowReference("~/clientmanager/clientmanager_new.aspx", "新增客户");
            // 每页记录数
            Grid1.PageSize = ConfigHelper.PageSize;
            ddlGridPageSize.SelectedValue = ConfigHelper.PageSize.ToString();

            //客户经理
            BindDDLClient();

            //实施人员
            BindDDLimplementer();

            //跟单
            BindDDLgendan();

            //绑定分级代码
            BindDDLCodeLevel();

            //绑定服务条件
            BindDDLCondiction();


            BindGrid();
        }

        private void BindDDLCondiction()
        {
            var q = DB.telenClients.Where(u => u.condiction != null).Select(u => u.condiction).Distinct().ToList();

            ddlcondiction.DataSource = q;
            ddlcondiction.DataBind();
        }

        private void BindDDLCodeLevel()
        {
            var q = DB.telenClients.Select(u => u.levelCode).Distinct().ToList();

            ddlCodeLevel.DataSource = q;
            ddlCodeLevel.DataBind();
        }

        private void BindDDLgendan()
        {
            var q = from a in DB.Users
                    select new
                    {
                        a.ID,
                        a.ChineseName
                    };
            ddlgendan.DataTextField = "ChineseName";
            ddlgendan.DataValueField = "ChineseName";
            ddlgendan.DataSource = q;
            ddlgendan.DataBind();
        }

        private void BindDDLimplementer()
        {
            var q = from a in DB.Users
                    select new
                    {
                        a.ID,
                        a.ChineseName
                    };
            ddlshishirenyuan.DataTextField = "ChineseName";
            ddlshishirenyuan.DataValueField = "ChineseName";
            ddlshishirenyuan.DataSource = q;
            ddlshishirenyuan.DataBind();
        }

        private void BindDDLClient()
        {
            var q = from a in DB.Users
                    select new
                    {
                        a.ID,
                        a.ChineseName
                    };
            ddlkehujingli.DataTextField = "ChineseName";
            ddlkehujingli.DataValueField = "ChineseName";
            ddlkehujingli.DataSource = q;
            ddlkehujingli.DataBind();
        }



        private void BindGrid()
        {
            IQueryable<TelenClient> q = DB.telenClients;

            if (tbxname.Text != "")
            {
                q = q.Where(u => u.Name.Contains(tbxname.Text));

            }
            if (tbxnumber.Text != "")
            {
                q = q.Where(u => u.Number==tbxnumber.Text);

            }

            Grid1.RecordCount = q.Count();
            q = SortAndPage(q, Grid1);
            Grid1.DataSource = q;
            Grid1.DataBind();
        }

        protected void Grid1_PageIndexChange(object sender, GridPageEventArgs e)
        {
            Grid1.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void Grid1_Sort(object sender, GridSortEventArgs e)
        {
            Grid1.SortDirection = e.SortDirection;
            Grid1.SortField = e.SortField;
            BindGrid();
        }

        protected void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            
            // 从每个选中的行中获取ID（在Grid1中定义的DataKeyNames）
            List<int> ids = GetSelectedDataKeyIDs(Grid1);
            //Debug.WriteLine(ids[0]);
            DB.telenClients.Where(u => ids.Contains(u.ID)).Delete();


            // 重新绑定表格
            BindGrid();
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.CloseArgument))
            {
                JObject dataObj = null;

                try
                {
                    dataObj = JObject.Parse(e.CloseArgument);
                }
                catch (Exception)
                {
                    // nothing
                }

                if (dataObj != null)
                {
                    string rowid = dataObj.Value<string>("rowid");
                    string nickname = dataObj.Value<string>("nickname");
                    string openid = dataObj.Value<string>("openid");


                    JObject columnValues = new JObject();
                    columnValues.Add("nickname", nickname);
                    columnValues.Add("openid", openid);

                    // cancelEdit用来取消编辑
                    string updateCellScript = String.Format("F('{0}').cancelEdit();F('{0}').updateCellValue('{1}',{2});", Grid1.ClientID, rowid, columnValues);

                    PageContext.RegisterStartupScript(updateCellScript);



                }
            }
        }


        protected void ddlGridPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            Grid1.PageSize = Convert.ToInt32(ddlGridPageSize.SelectedValue);

            BindGrid();
        }

        protected void tbxEditorName_TriggerClick(object sender, EventArgs e)
        {
            string[] selectedCell = Grid1.SelectedCell;
            if (selectedCell != null)
            {
                PageContext.RegisterStartupScript(Window1.GetShowReference("weixinuser.aspx?rowid=" + selectedCell[0]));
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Dictionary<int ,Dictionary<string,object>> modifiedDict=  Grid1.GetModifiedDict();


            foreach(int rowIndex in modifiedDict.Keys)
            {
                int rowID = int.Parse(Grid1.DataKeys[rowIndex][0].ToString());

                UpdateDataRow(modifiedDict[rowIndex], rowID);

            }
            BindGrid();
        }

        private void UpdateDataRow(Dictionary<string, object> dictionary, int rowID)
        {
            //if (dictionary.ContainsKey("nickname"))
            //{
            //    DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { nickname = dictionary["nickname"].ToString() });
            //}

            //if (dictionary.ContainsKey("openid"))
            //{
            //    DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { openid = dictionary["openid"].ToString() });
            //}


            if (dictionary.ContainsKey("Number"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { Number = dictionary["Number"].ToString() });
            }

            if (dictionary.ContainsKey("Name"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { Name = dictionary["Name"].ToString() });
            }

            if (dictionary.ContainsKey("levelCode"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { levelCode = dictionary["levelCode"].ToString() });
            }

            if (dictionary.ContainsKey("clientManager"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { clientManager = dictionary["clientManager"].ToString() });
            }

            if (dictionary.ContainsKey("implementer"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { implementer = dictionary["implementer"].ToString() });
            }

            if (dictionary.ContainsKey("gendan"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { gendan = dictionary["gendan"].ToString() });
            }

            if (dictionary.ContainsKey("serverTime"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { serverTime =DateTime.Parse( dictionary["serverTime"].ToString()) });
            }

            if (dictionary.ContainsKey("condiction"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { condiction = dictionary["condiction"].ToString() });
            }

            if (dictionary.ContainsKey("remark"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { remark = dictionary["remark"].ToString() });
            }

            if (dictionary.ContainsKey("fax"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { fax = dictionary["fax"].ToString() });
            }

            if (dictionary.ContainsKey("address"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { address = dictionary["address"].ToString() });
            }

            if (dictionary.ContainsKey("bank"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { bank = dictionary["bank"].ToString() });
            }

            if (dictionary.ContainsKey("Eamil"))
            {
                DB.telenClients.Where(u => u.ID == rowID).Update(u => new TelenClient { Eamil = dictionary["Eamil"].ToString() });
            }

        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
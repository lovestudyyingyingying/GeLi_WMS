using FineUIPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NanXingGuoRen_WMS.ClientManager
{
    public partial class ClientManager_New :PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                btnClose.OnClientClick = ActiveWindow.GetHideReference();
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
            }
        }

        private void BindDDLCondiction()
        {
            var q = DB.telenClients.Where(u=>u.condiction!=null).Select(u => u.condiction).Distinct().ToList();

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
            ddlgendan.DataValueField = "ID";
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
            ddlshishirenyuan.DataValueField = "ID";
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
            ddlkehujingli.DataValueField = "ID";
            ddlkehujingli.DataSource = q;
            ddlkehujingli.DataBind();
        }

        protected void btnSaveClose_Click(object sender, EventArgs e)
        {
            TelenClient item = new TelenClient();
            item.Number = tbxNumber.Text.Trim();
            item.Name = tbxName.Text.Trim();
            item.levelCode = ddlCodeLevel.SelectedText;
            item.clientManager = ddlkehujingli.SelectedText;
            item.implementer = ddlshishirenyuan.SelectedText;
            item.gendan = ddlgendan.SelectedText;
            item.condiction = ddlcondiction.SelectedText;
            item.remark = tbxremark.Text.Trim();
            item.fax = tbxfax.Text.Trim();
            item.address = tbxaddress.Text.Trim();
            item.bank = tbxbank.Text.Trim();
            item.Eamil = tbxemail.Text.Trim();
            if (dpDate.SelectedDate.HasValue)
            {
                item.serverTime = dpDate.SelectedDate.Value;
            }
            DB.telenClients.Add(item);
            DB.SaveChanges();

            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }
    }
}
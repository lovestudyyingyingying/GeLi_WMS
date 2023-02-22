using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;

namespace GeLiPage_WMS
{
    public partial class register : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDDLDept();
            }
        }

        private void BindDDLDept()
        {
            var q = from a in DB.Depts
                    select a;
            ddlDept.DataTextField = "Name";
            ddlDept.DataValueField = "ID";

            ddlDept.DataSource = q;
            ddlDept.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = tbxname.Text.Trim();
            bool isExist = DB.Users.Any(u => u.Name == name);
            if (isExist == true)
            {
                Alert.Show("已存在该用户");
                return;
            }
            else
            {
                User item = new User();
                item.Name = tbxname.Text.Trim();
                item.Password = PasswordUtil.CreateDbPassword(tbxpassword.Text.Trim());
                int deptid =int.Parse( ddlDept.SelectedValue);
                Dept dept = DB.Depts.Where(u => u.ID == deptid).FirstOrDefault();
                item.Dept = dept;
                item.ChineseName = tbxchinesename.Text.Trim();
                item.Gender = rblGender.SelectedValue;
                item.Email = tbxEmail.Text.Trim();
                item.Enabled = true;
                item.CreateTime = DateTime.Now;

                DB.Users.Add(item);
                DB.SaveChanges();

                Alert.Show("注册成功", "提示", MessageBoxIcon.Success, "okscript()");
            }
        }
    }
}
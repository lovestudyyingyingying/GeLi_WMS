using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GeLiPage_WMS.admin
{
    public partial class role_user_addnew : PageBase
    {
        #region ViewPower

        /// <summary>
        /// 本页面的浏览权限，空字符串表示本页面不受权限控制
        /// </summary>
        public override string ViewPower
        {
            get
            {
                return "CoreRoleUserNew";
            }
        }

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
            else
            {
                // 页面回发时要同步选中的行数据到隐藏字段（在触发控件事件之前）
                //Debug.WriteLine(43);
                //SyncSelectedRowIndexArrayToHiddenField(hfSelectedIDS, Grid1);
            }
        }

        private void LoadData()
        {
            btnClose.OnClientClick = ActiveWindow.GetHideReference();

            int id = GetQueryIntValue("id");
            Role current = DB.Roles.Find(id);
            if (current == null)
            {
                // 参数错误，首先弹出Alert对话框然后关闭弹出窗口
                Alert.Show("参数错误！", String.Empty, ActiveWindow.GetHideReference());
                return;
            }

            // 每页记录数
            Grid1.PageSize = ConfigHelper.PageSize;
            ddlGridPageSize.SelectedValue = ConfigHelper.PageSize.ToString();


            BindGrid();
        }


        private void BindGrid()
        {
            Debug.WriteLine(Grid1.SelectedRowIDArray==null);
            if (Grid1.SelectedRowIDArray!=null)
            {
                Debug.WriteLine("选中行的ID列表为：" + string.Join(", ", Grid1.SelectedRowIDArray));
            }
            IQueryable<User> q = DB.Users;

            // 在名称中搜索
            string searchText = ttbSearchMessage.Text.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            }

            q = q.Where(u => u.Name != "admin");

            // 排除已经属于本角色的用户
            int roleID = GetQueryIntValue("id");
            q = q.Where(u => u.Roles.All(r => r.ID != roleID));
            
            // 在查询添加之后，排序和分页之前获取总记录数
            Grid1.RecordCount = q.Count();

            // 排列和分页
            q = SortAndPage<User>(q, Grid1);

            Grid1.DataSource = q;
            Grid1.DataBind();

            // 重新绑定表格数据之后，更新选中行
            UpdateSelectedRowIndexArray(hfSelectedIDS, Grid1);
        }

        #endregion

        #region Events

        protected void btnSaveClose_Click(object sender, EventArgs e)
        {
            int roleID = GetQueryIntValue("id");

            // 从每个选中的行中获取ID（在Grid1中定义的DataKeyNames）
            List<int> ids = GetSelectedIDsFromHiddenField(hfSelectedIDS);

            Role role = DB.Roles.Include(r => r.Users)
                .Where(r => r.ID == roleID)
                .FirstOrDefault();


            foreach (int userID in ids)
            {
                User user = Attach<User>(userID);
                role.Users.Add(user);
            }
            DB.SaveChanges();

            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }

       
        protected void ttbSearchMessage_Trigger2Click(object sender, EventArgs e)
        {
            ttbSearchMessage.ShowTrigger1 = true;
            BindGrid();
        }

        protected void ttbSearchMessage_Trigger1Click(object sender, EventArgs e)
        {
            ttbSearchMessage.Text = String.Empty;
            ttbSearchMessage.ShowTrigger1 = false;
            BindGrid();
        }

        protected void Grid1_Sort(object sender, GridSortEventArgs e)
        {
			Grid1.SortDirection = e.SortDirection;
            Grid1.SortField = e.SortField;
            BindGrid();
        }

        protected void Grid1_PageIndexChange(object sender, GridPageEventArgs e)
        {
            SyncSelectedRowIndexArrayToHiddenField(hfSelectedIDS, Grid1, e);
            Grid1.PageIndex = e.NewPageIndex;
            BindGrid();
        }


        protected void ddlGridPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {

            Grid1.PageSize = Convert.ToInt32(ddlGridPageSize.SelectedValue);

            BindGrid();
        }

        #endregion


    }
}

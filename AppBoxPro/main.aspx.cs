using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

using Newtonsoft.Json.Linq;
using FineUIPro;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;

namespace GeLiPage_WMS
{
    public partial class main : PageBase
    {
        #region Page_Init

        protected void Page_Init(object sender, EventArgs e)
        {
            // 工具栏上的帮助菜单
            JArray ja = JArray.Parse(ConfigHelper.HelpList);
            foreach (JObject jo in ja)
            {
                MenuButton menuItem = new MenuButton();
                menuItem.EnablePostBack = false;
                menuItem.Text = jo.Value<string>("Text");
                menuItem.Icon = IconHelper.String2Icon(jo.Value<string>("Icon"), true);
              
                menuItem.OnClientClick = String.Format("addExampleTab('{0}','{1}','{2}')", jo.Value<string>("ID"), ResolveUrl(jo.Value<string>("URL")), jo.Value<string>("Text"));

                btnHelp.Menu.Items.Add(menuItem);
            }

            // 用户可见的菜单列表
            List<Menu> menus = ResolveUserMenuList();
            if (menus.Count == 0)
            {
                Response.Write("系统管理员尚未给你配置菜单！");
                Response.End();

                return;
            }


            // 注册客户端脚本，服务器端控件ID和客户端ID的映射关系
            //JObject ids = GetClientIDS(regionPanel, regionTop, mainTabStrip, txtUser,
            //    txtOnlineUserCount, txtCurrentTime, btnRefresh);
            //ids.Add("userName", GetIdentityName());
            //ids.Add("userIP", Request.UserHostAddress);
            //ids.Add("onlineUserCount", GetOnlineCount());

            if (ConfigHelper.MenuType == "accordion")
            {
                Accordion accordionMenu = InitAccordionMenu(menus);
                //ids.Add("treeMenu", accordionMenu.ClientID);
                //ids.Add("menuType", "accordion");
            }
            else
            {
                Tree treeMenu = InitTreeMenu(menus);
                //ids.Add("treeMenu", treeMenu.ClientID);
                //ids.Add("menuType", "menu");
            }

            //string idsScriptStr = String.Format("window.DATA={0};", ids.ToString(Newtonsoft.Json.Formatting.None));
            //PageContext.RegisterStartupScript(idsScriptStr);
        }

        //private JObject GetClientIDS(params ControlBase[] ctrls)
        //{
        //    JObject jo = new JObject();
        //    foreach (ControlBase ctrl in ctrls)
        //    {
        //        jo.Add(ctrl.ID, ctrl.ClientID);
        //    }

        //    return jo;
        //}

        #region InitAccordionMenu

        /// <summary>
        /// 创建手风琴菜单
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        private Accordion InitAccordionMenu(List<Menu> menus)
        {
            Accordion accordionMenu = new Accordion();
            accordionMenu.ID = "accordionMenu";
            accordionMenu.EnableFill = true;
            accordionMenu.ShowBorder = false;
            accordionMenu.ShowHeader = false;
            leftPanel.Items.Add(accordionMenu);


            foreach (var menu in menus.Where(m => m.Parent == null))
            {
                AccordionPane accordionPane = new AccordionPane();
                accordionPane.Title = menu.Name;
                accordionPane.Layout = LayoutType.Fit;
                accordionPane.ShowBorder = false;
                accordionPane.BodyPadding = "2px 0 0 0";
               
                Tree innerTree = new Tree();
                innerTree.ShowBorder = false;
                innerTree.ShowHeader = false;
                innerTree.EnableIcons = true;
                innerTree.AutoScroll = true;
                
                // 生成树
                int nodeCount = ResolveMenuTree(menus, menu, innerTree.Nodes);
                if (nodeCount > 0)
                {
                    accordionPane.Items.Add(innerTree);
                    accordionMenu.Items.Add(accordionPane);
                }

            }

            return accordionMenu;
        }

        #endregion

        #region InitTreeMenu

        /// <summary>
        /// 创建树菜单
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        private Tree InitTreeMenu(List<Menu> menus)
        {
            Tree treeMenu = new Tree();
            treeMenu.ID = "treeMenu";
            //treeMenu.EnableArrows = true;
            treeMenu.ShowBorder = false;
            treeMenu.ShowHeader = false;
            treeMenu.EnableIcons = true;
            treeMenu.AutoScroll = true;
            leftPanel.Items.Add(treeMenu);

            // 生成树
            ResolveMenuTree(menus, null, treeMenu.Nodes);

            // 展开第一个树节点
            treeMenu.Nodes[0].Expanded = true;
            //treeMenu.Nodes[1].Expanded = true;
            //treeMenu.Nodes[2].Expanded = true;
            //treeMenu.Nodes[3].Expanded = true;
            return treeMenu;
        }

        /// <summary>
        /// 生成菜单树
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="parentMenuId"></param>
        /// <param name="nodes"></param>
        private int ResolveMenuTree(List<Menu> menus, Menu parentMenu, FineUIPro.TreeNodeCollection nodes)
        {
            int count = 0;
            foreach (var menu in menus.Where(m => m.Parent == parentMenu))
            {
                FineUIPro.TreeNode node = new FineUIPro.TreeNode();

                nodes.Add(node);
                count++;
                
                node.Text = menu.Name;
                node.IconUrl = menu.ImageUrl;
                if (!String.IsNullOrEmpty(menu.NavigateUrl))
                {
                    if (menu.Name != "欠货报表")
                    {
                        node.EnableClickEvent = false;
                        node.NavigateUrl = ResolveUrl(menu.NavigateUrl);
                    }
                    
                    //node.OnClientClick = String.Format("addTab('{0}','{1}','{2}')", node.NodeID, ResolveUrl(menu.NavigateUrl), node.Text.Replace("'", ""));
                }

                if (node.Text == "欠货报表")
                {

                    node.OnClientClick = @"window.open('2DReport/2DReport.aspx', '_blank');";

                }

                if (menu.IsTreeLeaf)
                {
                    node.Leaf = true;

                    // 如果是叶子节点，但是不是超链接，则是空目录，删除
                    if (String.IsNullOrEmpty(menu.NavigateUrl) && node.Text != "欠货报表")
                    {
                        nodes.Remove(node);
                        count--;
                    }
                }
               
                else
                {
                    //node.SingleClickExpand = true;

                    int childCount = ResolveMenuTree(menus, menu, node.Nodes);

                    // 如果是目录，但是计算的子节点数为0，可能目录里面的都是空目录，则要删除此父目录
                    if (childCount == 0 && String.IsNullOrEmpty(menu.NavigateUrl))
                    {
                        nodes.Remove(node);
                        count--;
                    }
                }
                
            }

            return count;
        }

        #endregion

        #region ResolveUserMenuList

        // 获取用户可用的菜单列表
        private List<Menu> ResolveUserMenuList()
        {
            // 当前登陆用户的权限列表
            List<string> rolePowerNames = GetRolePowerNames();

            // 当前用户所属角色可用的菜单列表
            List<Menu> menus = new List<Menu>();
            
            //foreach (var temp in rolePowerNames)
            //    Debug.WriteLine(temp);

            foreach (Menu menu in MenuHelper.Menus)
            {
                // 如果此菜单不属于任何模块，或者此用户所属角色拥有对此模块的权限
                if (menu.ViewPower == null || rolePowerNames.Contains(menu.ViewPower.Name))
                {
                    menus.Add(menu);
                }
            }

            return menus;
        } 

        #endregion

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            System.Web.UI.WebControls.HyperLink link = topPanel.FindControl("linkSystemTitle") as System.Web.UI.WebControls.HyperLink;
            if (link != null)
            {
                link.Text = ConfigHelper.Title;
            }
            string username= GetIdentityName();

            var q = DB.Users.Where(u => u.Name == username).FirstOrDefault();

            btnUserName.Text = q.ChineseName;

        }


        #endregion

        #region Events

        protected void btnExit_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            FormsAuthentication.RedirectToLoginPage();
        }

        #endregion
    }
}

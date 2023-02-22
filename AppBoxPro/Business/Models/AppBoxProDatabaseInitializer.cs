using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace GeLiPage_WMS
{
    public class GeLiPage_WMSDatabaseInitializer : 
        CreateDatabaseIfNotExists<GeLiPage_WMSContext>
        //DropCreateDatabaseIfModelChanges<GeLiPage_WMSContext>   DropCreateDatabaseAlways<GeLiPage_WMSContext>  DropCreateDatabaseIfModelChanges<GeLiPage_WMSContext>
    {
        protected override void Seed(GeLiPage_WMSContext context)
        {
            GetConfigs().ForEach(c => context.Configs.Add(c));
            GetDepts().ForEach(d => context.Depts.Add(d));
            GetUsers().ForEach(u => context.Users.Add(u));

            GetRoles().ForEach(r => context.Roles.Add(r));
            GetPowers().ForEach(p => context.Powers.Add(p));
            GetTitles().ForEach(t => context.Titles.Add(t));

            context.SaveChanges();
            // 添加菜单时需要指定ViewPower，所以上面需要先保存到数据库
            GetMenus(context).ForEach(m => context.Menus.Add(m));
        }

        private static List<Menu> GetMenus(GeLiPage_WMSContext context)
        {
            var menus = new List<Menu> {
                 new Menu
                        {
                            Name = "移动仓储",
                            SortIndex = 0,
                            Remark = "一级菜单",
                            //ImageUrl = "~/res/icon/tag_blue.png",
                            Children=new List<Menu>{
                                 new Menu
                            {
                            Name = "标签打印",
                            SortIndex = 1,
                            Remark = "二级菜单",
                            NavigateUrl = "~/ydcc/LabelPrint.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "InStockView").FirstOrDefault<Power>()
                             },
                                new Menu
                            {
                            Name = "进出仓查询",
                            SortIndex = 2,
                            Remark = "二级菜单",
                            NavigateUrl = "~/ydcc/stocksearch.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "InStockView").FirstOrDefault<Power>()
                             },
                                 new Menu
                            {
                            Name = "盘点查询",
                            SortIndex = 3,
                            Remark = "二级菜单",
                            NavigateUrl = "~/ydcc/pandiansearch.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "PanDianView").FirstOrDefault<Power>()
                             }
                          }
                      },
                new Menu
                {
                    Name = "系统管理",
                    SortIndex = 1,
                    Remark = "顶级菜单",
                    Children = new List<Menu> {
                        new Menu
                        {
                            Name = "用户管理",
                            SortIndex = 10,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/user.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreUserView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "职称管理",
                            SortIndex = 20,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/title.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreTitleView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "职称用户管理",
                            SortIndex = 30,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/title_user.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreTitleUserView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "部门管理",
                            SortIndex = 40,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/dept.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreDeptView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "部门用户管理",
                            SortIndex = 50,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/dept_user.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreDeptUserView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "角色管理",
                            SortIndex = 60,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/role.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreRoleView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "角色用户管理",
                            SortIndex = 70,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/role_user.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreRoleUserView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "权限管理",
                            SortIndex = 80,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/power.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CorePowerView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "角色权限管理",
                            SortIndex = 90,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/role_power.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreRolePowerView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "菜单管理",
                            SortIndex = 100,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/menu.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreMenuView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "在线统计",
                            SortIndex = 110,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/online.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreOnlineView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "系统配置",
                            SortIndex = 120,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/config.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreConfigView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "用户设置",
                            SortIndex = 130,
                            Remark = "二级菜单",
                            NavigateUrl = "~/admin/profile.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png"
                        }
                    }
                }
            };

            return menus;
        }


        private static List<Title> GetTitles()
        {
            var titles = new List<Title>()
            {
                new Title() 
                {
                    Name = "总经理"
                },
                new Title() 
                {
                    Name = "部门经理"
                },
                new Title() 
                {
                    Name = "高级工程师"
                },
                new Title() 
                {
                    Name = "工程师"
                }
            };

            return titles;
        }

        private static List<Power> GetPowers()
        {
            var powers = new List<Power>
            {
                new Power
                {
                    Name = "CoreUserView",
                    Title = "浏览用户列表",
                    GroupName = "CoreUser"
                },
                new Power
                {
                    Name = "CoreUserNew",
                    Title = "新增用户",
                    GroupName = "CoreUser"
                },
                new Power
                {
                    Name = "CoreUserEdit",
                    Title = "编辑用户",
                    GroupName = "CoreUser"
                },
                new Power
                {
                    Name = "CoreUserDelete",
                    Title = "删除用户",
                    GroupName = "CoreUser"
                },
                new Power
                {
                    Name = "CoreUserChangePassword",
                    Title = "修改用户登陆密码",
                    GroupName = "CoreUser"
                },
                new Power
                {
                    Name = "CoreRoleView",
                    Title = "浏览角色列表",
                    GroupName = "CoreRole"
                },
                new Power
                {
                    Name = "CoreRoleNew",
                    Title = "新增角色",
                    GroupName = "CoreRole"
                },
                new Power
                {
                    Name = "CoreRoleEdit",
                    Title = "编辑角色",
                    GroupName = "CoreRole"
                },
                new Power
                {
                    Name = "CoreRoleDelete",
                    Title = "删除角色",
                    GroupName = "CoreRole"
                },
                new Power
                {
                    Name = "CoreRoleUserView",
                    Title = "浏览角色用户列表",
                    GroupName = "CoreRoleUser"
                },
                new Power
                {
                    Name = "CoreRoleUserNew",
                    Title = "向角色添加用户",
                    GroupName = "CoreRoleUser"
                },
                new Power
                {
                    Name = "CoreRoleUserDelete",
                    Title = "从角色中删除用户",
                    GroupName = "CoreRoleUser"
                },
                new Power
                {
                    Name = "CoreOnlineView",
                    Title = "浏览在线用户列表",
                    GroupName = "CoreOnline"
                },
                new Power
                {
                    Name = "CoreConfigView",
                    Title = "浏览全局配置参数",
                    GroupName = "CoreConfig"
                },
                new Power
                {
                    Name = "CoreConfigEdit",
                    Title = "修改全局配置参数",
                    GroupName = "CoreConfig"
                },
                new Power
                {
                    Name = "CoreMenuView",
                    Title = "浏览菜单列表",
                    GroupName = "CoreMenu"
                },
                new Power
                {
                    Name = "CoreMenuNew",
                    Title = "新增菜单",
                    GroupName = "CoreMenu"
                },
                new Power
                {
                    Name = "CoreMenuEdit",
                    Title = "编辑菜单",
                    GroupName = "CoreMenu"
                },
                new Power
                {
                    Name = "CoreMenuDelete",
                    Title = "删除菜单",
                    GroupName = "CoreMenu"
                },
                new Power
                {
                    Name = "CoreLogView",
                    Title = "浏览日志列表",
                    GroupName = "CoreLog"
                },
                new Power
                {
                    Name = "CoreLogDelete",
                    Title = "删除日志",
                    GroupName = "CoreLog"
                },
                new Power
                {
                    Name = "CoreTitleView",
                    Title = "浏览职务列表",
                    GroupName = "CoreTitle"
                },
                new Power
                {
                    Name = "CoreTitleNew",
                    Title = "新增职务",
                    GroupName = "CoreTitle"
                },
                new Power
                {
                    Name = "CoreTitleEdit",
                    Title = "编辑职务",
                    GroupName = "CoreTitle"
                },
                new Power
                {
                    Name = "CoreTitleDelete",
                    Title = "删除职务",
                    GroupName = "CoreTitle"
                },
                new Power
                {
                    Name = "CoreTitleUserView",
                    Title = "浏览职务用户列表",
                    GroupName = "CoreTitleUser"
                },
                new Power
                {
                    Name = "CoreTitleUserNew",
                    Title = "向职务添加用户",
                    GroupName = "CoreTitleUser"
                },
                new Power
                {
                    Name = "CoreTitleUserDelete",
                    Title = "从职务中删除用户",
                    GroupName = "CoreTitleUser"
                },
                new Power
                {
                    Name = "CoreDeptView",
                    Title = "浏览部门列表",
                    GroupName = "CoreDept"
                },
                new Power
                {
                    Name = "CoreDeptNew",
                    Title = "新增部门",
                    GroupName = "CoreDept"
                },
                new Power
                {
                    Name = "CoreDeptEdit",
                    Title = "编辑部门",
                    GroupName = "CoreDept"
                },
                new Power
                {
                    Name = "CoreDeptDelete",
                    Title = "删除部门",
                    GroupName = "CoreDept"
                },
                new Power
                {
                    Name = "CoreDeptUserView",
                    Title = "浏览部门用户列表",
                    GroupName = "CoreDeptUser"
                },
                new Power
                {
                    Name = "CoreDeptUserNew",
                    Title = "向部门添加用户",
                    GroupName = "CoreDeptUser"
                },
                new Power
                {
                    Name = "CoreDeptUserDelete",
                    Title = "从部门中删除用户",
                    GroupName = "CoreDeptUser"
                },
                new Power
                {
                    Name = "CorePowerView",
                    Title = "浏览权限列表",
                    GroupName = "CorePower"
                },
                new Power
                {
                    Name = "CorePowerNew",
                    Title = "新增权限",
                    GroupName = "CorePower"
                },
                new Power
                {
                    Name = "CorePowerEdit",
                    Title = "编辑权限",
                    GroupName = "CorePower"
                },
                new Power
                {
                    Name = "CorePowerDelete",
                    Title = "删除权限",
                    GroupName = "CorePower"
                },
                new Power
                {
                    Name = "CoreRolePowerView",
                    Title = "浏览角色权限列表",
                    GroupName = "CoreRolePower"
                },
                new Power
                {
                    Name = "CoreRolePowerEdit",
                    Title = "编辑角色权限",
                    GroupName = "CoreRolePower"
                },
                new Power
                {
                    Name = "PanDianView",
                    Title = "盘点浏览权限",
                    GroupName = "PanDian"
                },
                new Power
                {
                    Name = "PanDianNew",
                    Title = "盘点新增权限",
                    GroupName = "PanDian"
                },
                new Power
                {
                    Name = "PanDianEdit",
                    Title = "盘点编辑权限",
                    GroupName = "PanDian"
                },
                new Power
                {
                    Name = "PanDianDelete",
                    Title = "盘点删除权限",
                    GroupName = "PanDian"
                },
                new Power
                {
                    Name = "InstockView",
                    Title = "进仓浏览权限",
                    GroupName = "Instock"
                },
                new Power
                {
                    Name = "InstockNew",
                    Title = "进仓新增权限",
                    GroupName = "Instock"
                },
                new Power
                {
                    Name = "InstockEdit",
                    Title = "进仓编辑权限",
                    GroupName = "Instock"
                },
                new Power
                {
                    Name = "InstockDelete",
                    Title = "进仓删除权限",
                    GroupName = "Instock"
                },
                new Power
                {
                    Name = "OutstockView",
                    Title = "出仓浏览权限",
                    GroupName = "Outstock"
                },
                new Power
                {
                    Name = "OutstockNew",
                    Title = "出仓新增权限",
                    GroupName = "Outstock"
                },
                new Power
                {
                    Name = "OutstockEdit",
                    Title = "出仓编辑权限",
                    GroupName = "Outstock"
                },
                new Power
                {
                    Name = "OutstockDelete",
                    Title = "出仓删除权限",
                    GroupName = "Outstock"
                }
            };

            return powers;
        }

        private static List<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                 new Role()
                {
                    Name = "超级管理员",
                    Remark = ""
                },
                new Role()
                {
                    Name = "公司管理员",
                    Remark = ""
                },
                new Role()
                {
                    Name = "部门经理",
                    Remark = ""
                },
                new Role()
                {
                    Name = "盘点人员",
                    Remark = ""
                }
            };

            return roles;
        }

        private static List<User> GetUsers()
        {
            string[] USER_NAMES = { "男", "童光喜", "男", "方原柏" };
            string[] EMAIL_NAMES = { "qq.com", "gmail.com", "163.com", "126.com", "outlook.com", "foxmail.com" };

            var users = new List<User>();
            var rdm = new Random();

            for (int i = 0, count = USER_NAMES.Length; i < count; i += 2)
            {
                string gender = USER_NAMES[i];
                string chineseName = USER_NAMES[i + 1];
                string userName = "user" + i.ToString();

                users.Add(new User
                {
                    Name = userName,
                    Gender = gender,
                    Password = PasswordUtil.CreateDbPassword(userName),
                    ChineseName = chineseName,
                    Email = userName + "@" + EMAIL_NAMES[rdm.Next(0, EMAIL_NAMES.Length)],
                    Enabled = true,
                    CreateTime = DateTime.Now
                });
            }

            // 添加超级管理员
            users.Add(new User
            {
                Name = "admin",
                Gender = "男",
                Password = PasswordUtil.CreateDbPassword("admin"),
                ChineseName = "超级管理员",
                Email = "admin@examples.com",
                Enabled = true,
                CreateTime = DateTime.Now
            });

            return users;
        }


        private static List<Dept> GetDepts()
        {
            var depts = new List<Dept> { 
                new Dept
                {
                    Name = "研发部",
                    SortIndex = 1,
                    Remark = "顶级部门",
                    Children = new List<Dept> { 
                        new Dept
                        {
                            Name = "开发部",
                            SortIndex = 1,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "测试部",
                            SortIndex = 2,
                            Remark = "二级部门"
                        }
                    }
                },
                new Dept
                {
                    Name = "销售部",
                    SortIndex = 2,
                    Remark = "顶级部门",
                    Children = new List<Dept> { 
                        new Dept
                        {
                            Name = "直销部",
                            SortIndex = 1,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "渠道部",
                            SortIndex = 2,
                            Remark = "二级部门"
                        }
                    }
                },
                new Dept
                {
                    Name = "客服部",
                    SortIndex = 3,
                    Remark = "顶级部门",
                    Children = new List<Dept> { 
                        new Dept
                        {
                            Name = "实施部",
                            SortIndex = 1,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "售后服务部",
                            SortIndex = 2,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "大客户服务部",
                            SortIndex = 3,
                            Remark = "二级部门"
                        }
                    }
                },
                new Dept
                {
                    Name = "财务部",
                    SortIndex = 4,
                    Remark = "顶级部门"
                },
                new Dept
                {
                    Name = "行政部",
                    SortIndex = 5,
                    Remark = "顶级部门",
                    Children = new List<Dept> { 
                        new Dept
                        {
                            Name = "人事部",
                            SortIndex = 1,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "后勤部",
                            SortIndex = 2,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "运输部",
                            SortIndex = 3,
                            Remark = "二级部门",
                            Children = new List<Dept>{
                                new Dept{
                                    Name = "省内运输部",
                                    SortIndex = 1,
                                    Remark = "三级部门",
                                },
                                new Dept{
                                    Name = "国内运输部",
                                    SortIndex = 2,
                                    Remark = "三级部门",
                                },
                                new Dept{
                                    Name = "国际运输部",
                                    SortIndex = 3,
                                    Remark = "三级部门",
                                }
                            }
                        }
                    }
                }
            };

            return depts;
        }

        private static List<Config> GetConfigs()
        {
            var configs = new List<Config> {
                new Config
                {
                    ConfigKey = "Title",
                    ConfigValue = "移动仓储后台管理系统",
                    Remark = "网站的标题"
                },
                new Config
                {
                    ConfigKey = "PageSize",
                    ConfigValue = "20",
                    Remark = "表格每页显示的个数"
                },
                new Config
                {
                    ConfigKey = "MenuType",
                    ConfigValue = "tree",
                    Remark = "左侧菜单样式"
                },
                new Config
                {
                    ConfigKey = "Theme",
                    ConfigValue = "Cupertino",
                    Remark = "网站主题"
                },
                new Config
                {
                    ConfigKey = "HelpList",
                    ConfigValue = "[{\"Text\":\"万年历\",\"Icon\":\"Calendar\",\"ID\":\"wannianli\",\"URL\":\"~/admin/help/wannianli.htm\"},{\"Text\":\"科学计算器\",\"Icon\":\"Calculator\",\"ID\":\"jisuanqi\",\"URL\":\"~/admin/help/jisuanqi.htm\"},{\"Text\":\"系统帮助\",\"Icon\":\"Help\",\"ID\":\"help\",\"URL\":\"~/admin/help/help.htm\"}]",
                    Remark = "帮助下拉列表的JSON字符串"
                }
            };

            return configs;
        }

    }
}
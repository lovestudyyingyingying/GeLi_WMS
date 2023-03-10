using System;
using System.Collections.Generic;
using System.Web;

using System.Linq;
using System.Data.Entity;
using System.Diagnostics;

namespace GeLiPage_WMS
{
    public class MenuHelper
    {
        #region 菜单
        private static List<Menu> _menus;

        public static List<Menu> Menus
        {
            get
            {
                if (_menus == null)
                {
                    InitMenus();
                }
                return _menus;
            }
        }

        public static void Reload()
        {
            _menus = null;
        }

        private static void InitMenus()
        {
            _menus = new List<Menu>();
            
            List<Menu> dbMenus = PageBase.DB.Menus.Include(m => m.ViewPower).OrderBy(m => m.SortIndex).ToList();
            Debug.WriteLine(dbMenus);
            ResolveMenuCollection(dbMenus, null, 0);
        }


        private static int ResolveMenuCollection(List<Menu> dbMenus, Menu parentMenu, int level)
        {
            int count = 0;

            foreach (var menu in dbMenus.Where(m => m.Parent == parentMenu))
            {
                count++;

                _menus.Add(menu);
                menu.TreeLevel = level;
                menu.IsTreeLeaf = true;
                menu.Enabled = true;

                level++;
                int childCount = ResolveMenuCollection(dbMenus, menu, level);
                if (childCount != 0)
                {
                    menu.IsTreeLeaf = false;
                }
                level--;
            }

            return count;
        }
        #endregion

    }
}

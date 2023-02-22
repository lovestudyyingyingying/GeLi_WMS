using System;
using System.Web.Security;
using System.Web.UI;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Web;
using FineUIPro;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Data;

using System.Data.Entity;

using System.Web.UI.WebControls;
using System.Linq.Dynamic;
using TextBox = FineUIPro.TextBox;

using AppBoxPro;
using GeLiPage_WMS.DictornaryEntity;

using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
using GeLiService_WMS.Services.WMS.AGV;

namespace GeLiPage_WMS
{
    public class PageBase : System.Web.UI.Page
    {
        #region 只读静态变量
       
        // Session key
        private static readonly string SK_ONLINE_UPDATE_TIME = "OnlineUpdateTime";
        //private static readonly string SK_USER_ROLE_ID = "UserRoleId";

        private static readonly string CHECK_POWER_FAIL_PAGE_MESSAGE = "您无权访问此页面！";
        private static readonly string CHECK_POWER_FAIL_ACTION_MESSAGE = "您无权进行此操作！";

        #endregion


        #region 实用函数

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        public virtual void ShowNotify(string message)
        {
            ShowNotify(message, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon)
        {
            Notify n = new Notify();
            n.Target = Target.Top;
            n.Message = message;
            n.MessageBoxIcon = messageIcon;
            n.PositionX = Position.Center;
            n.PositionY = Position.Top;
            n.DisplayMilliseconds = 3000;
            n.ShowHeader = false;


            n.Show();
        }


        /// <summary>
        /// 获取回发的参数
        /// </summary>
        /// <returns></returns>
        public string GetRequestEventArgument()
        {
            return Request.Form["__EVENTARGUMENT"];
        }
        #endregion

        #region 实体上下文

        public static GeLiPage_WMSContext DB
        {
            get
            {
                // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
                if (!HttpContext.Current.Items.Contains("__GeLiPage_WMSContext"))
                {
                    GeLiPage_WMSContext temp= new GeLiPage_WMSContext();
                    //temp.Database.Log = s => {
                    //    Debug.WriteLine(s);
                    //};
                    HttpContext.Current.Items["__GeLiPage_WMSContext"] = temp;
                }
                return HttpContext.Current.Items["__GeLiPage_WMSContext"] as GeLiPage_WMSContext;
            }
        }

        //public static GeLiPage_WMSEntities DB2
        //{
        //    get
        //    {
        //        // http://stackoverflow.com/questions/6334592/one-dbcontext-per-request-in-asp-net-mvc-without-ioc-container
        //        if (!HttpContext.Current.Items.Contains("__GeLiPage_WMSEntities"))
        //        {
        //            GeLiPage_WMSEntities temp = new GeLiPage_WMSEntities();

        //            temp.Database.Log = s => {
        //                Debug.WriteLine(s);
        //            };

        //            HttpContext.Current.Items["__GeLiPage_WMSEntities"] = temp;
        //        }
        //        return HttpContext.Current.Items["__GeLiPage_WMSEntities"] as GeLiPage_WMSEntities;
        //    }
        //}

        #endregion

        #region Redis
      

        #endregion

        #region Service(存放在HttpContext.Current中)
        //ASP.NET会为每个请求分配一个线程，HttpContext.Current存在于每个请求的线程中
        //当调用Service时才新建，而且一个请求只新建一次，减少内存占用

       
        #endregion

        #region Service-Old

        //protected ProductOrderService pos = new ProductOrderService();
        /*
        protected ProPlanOrderheaderService ProPlanOrderheaderService = new ProPlanOrderheaderService();
        protected ProPlanOrderlistsService ProPlanOrderlistsService = new ProPlanOrderlistsService();
        protected CRMPlanListService cpls = new CRMPlanListService();

        protected CRMPlanService cps = new CRMPlanService();
        protected LiuShuiHaoService lshs = new LiuShuiHaoService();
        protected UserService us = new UserService();
        protected WareHouseService whs = new WareHouseService();
        protected WorkshopProcessService wsps = new WorkshopProcessService();
        protected ItemService itemService = new ItemService();

        protected ProductOrderManager pos
        {
            get
            {
                return new ProductOrderManager(ProPlanOrderheaderService, ProPlanOrderlistsService, cpls);
            }
        }*/
        #endregion

        #region 浏览权限

        /// <summary>
        /// 本页面的浏览权限，空字符串表示本页面不受权限控制
        /// </summary>
        public virtual string ViewPower
        {
            get
            {
                return String.Empty;
            }
        }

        #endregion
        

        #region 页面初始化

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // 此用户是否有访问此页面的权限
            if (!CheckPowerView())
            {
                CheckPowerFailWithPage();
                return;
            }

            // 设置主题
            if (PageManager.Instance != null)
            {
                PageManager.Instance.Theme = (Theme)Enum.Parse(typeof(Theme), ConfigHelper.Theme, true);
            }

            
            UpdateOnlineUser(User.Identity.Name);

            // 设置页面标题
            Page.Title = ConfigHelper.Title;

            if(!IsPostBack)
            {
                //创建cookie
                HttpCookie cookie = HttpContext.Current.Request.Cookies["ClientId"];

                if (cookie == null)
                {
                    // 如果客户端没有 "ClientId" cookie，则创建一个新的 cookie
                    cookie = new HttpCookie("ClientId");
                    cookie.Value = Guid.NewGuid().ToString();
                    cookie.Expires = DateTime.Now.AddDays(1);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }

        #endregion

        #region 请求参数

        /// <summary>
        /// 获取查询字符串中的参数值
        /// </summary>
        protected string GetQueryValue(string queryKey)
        {
            return Request.QueryString[queryKey];
        }


        /// <summary>
        /// 获取查询字符串中的参数值
        /// </summary>
        protected int GetQueryIntValue(string queryKey)
        {
            int queryIntValue = -1;
            try
            {
                queryIntValue = Convert.ToInt32(Request.QueryString[queryKey]);
            }
            catch (Exception)
            {
                // TODO
            }

            return queryIntValue;
        }

        #endregion

        #region 表格相关

        #region 表格导出
        public void ExportExcel(string filename, Grid grid)
        {
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Write(GetGridTableHtml(grid));
            Response.End();
        }

        #region 多表头处理

        /// <summary>
        /// 处理多表头的类
        /// </summary>
        public class MultiHeaderTable
        {
            // 包含 rowspan，colspan 的多表头，方便生成 HTML 的 table 标签
            public List<List<object[]>> MultiTable = new List<List<object[]>>();
            // 最终渲染的列数组
            public List<GridColumn> Columns = new List<GridColumn>();


            public void ResolveMultiHeaderTable(GridColumnCollection columns)
            {
                List<object[]> row = new List<object[]>();
                foreach (GridColumn column in columns)
                {
                    object[] cell = new object[4];
                    cell[0] = 1;    // rowspan
                    cell[1] = 1;    // colspan
                    cell[2] = column;
                    cell[3] = null;

                    row.Add(cell);
                }

                ResolveMultiTable(row, 0);

                ResolveColumns(row);
            }

            private void ResolveColumns(List<object[]> row)
            {
                foreach (object[] cell in row)
                {
                    GroupField groupField = cell[2] as GroupField;
                    if (groupField != null && groupField.Columns.Count > 0)
                    {
                        List<object[]> subrow = new List<object[]>();
                        foreach (GridColumn column in groupField.Columns)
                        {
                            subrow.Add(new object[]
                            {
                                1,
                                1,
                                column,
                                groupField
                            });
                        }

                        ResolveColumns(subrow);
                    }
                    else
                    {
                        Columns.Add(cell[2] as GridColumn);
                    }
                }

            }

            private void ResolveMultiTable(List<object[]> row, int level)
            {
                List<object[]> nextrow = new List<object[]>();

                foreach (object[] cell in row)
                {
                    GroupField groupField = cell[2] as GroupField;
                    if (groupField != null && groupField.Columns.Count > 0)
                    {
                        // 如果当前列包含子列，则更改当前列的 colspan，以及增加父列（向上递归）的colspan
                        cell[1] = Convert.ToInt32(groupField.Columns.Count);
                        PlusColspan(level - 1, cell[3] as GridColumn, groupField.Columns.Count - 1);

                        foreach (GridColumn column in groupField.Columns)
                        {
                            nextrow.Add(new object[]
                            {
                                1,
                                1,
                                column,
                                groupField
                            });
                        }
                    }
                }

                MultiTable.Add(row);

                // 如果当前下一行，则增加上一行（向上递归）中没有子列的列的 rowspan
                if (nextrow.Count > 0)
                {
                    PlusRowspan(level);

                    ResolveMultiTable(nextrow, level + 1);
                }
            }

            private void PlusRowspan(int level)
            {
                if (level < 0)
                {
                    return;
                }

                foreach (object[] cells in MultiTable[level])
                {
                    GroupField groupField = cells[2] as GroupField;
                    if (groupField != null && groupField.Columns.Count > 0)
                    {
                        // ...
                    }
                    else
                    {
                        cells[0] = Convert.ToInt32(cells[0]) + 1;
                    }
                }

                PlusRowspan(level - 1);
            }

            private void PlusColspan(int level, GridColumn parent, int plusCount)
            {
                if (level < 0)
                {
                    return;
                }

                foreach (object[] cells in MultiTable[level])
                {
                    GridColumn column = cells[2] as GridColumn;
                    if (column == parent)
                    {
                        cells[1] = Convert.ToInt32(cells[1]) + plusCount;

                        PlusColspan(level - 1, cells[3] as GridColumn, plusCount);
                    }
                }
            }

        }
        #endregion


        private string GetGridTableHtml(Grid grid)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<meta http-equiv=\"Content-Type\" content=\"application/vnd.ms-excel;charset=utf-8\"/>");


            sb.Append("<table cellspacing=\"0\" rules=\"all\" border=\"1\" style=\"border-collapse:collapse;\">");

            sb.Append("<tr>");
            foreach (GridColumn column in grid.Columns)
            {
                sb.AppendFormat("<td>{0}</td>", column.HeaderText);
            }
            sb.Append("</tr>");

            foreach (GridRow row in grid.Rows)
            {
                sb.Append("<tr>");
                foreach (object value in row.Values)
                {
                    string html = value.ToString();
                    if (html.StartsWith(Grid.TEMPLATE_PLACEHOLDER_PREFIX))
                    {
                        // 模板列
                        string templateID = html.Substring(Grid.TEMPLATE_PLACEHOLDER_PREFIX.Length);
                        Control templateCtrl = row.FindControl(templateID);
                        html = GetRenderedHtmlSource(templateCtrl);
                    }
                    else
                    {
                        // 处理CheckBox
                        if (html.Contains("f-grid-static-checkbox"))
                        {
                            if (!html.Contains("f-checked"))
                            {
                                html = "×";
                            }
                            else
                            {
                                html = "√";
                            }
                        }

                        // 处理图片
                        if (html.Contains("<img"))
                        {
                            string prefix = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
                            html = html.Replace("src=\"", "src=\"" + prefix);
                        }
                    }

                    //日期
                    if (html.Contains("-"))
                    {
                        sb.Append("<td style='vnd.ms-excel.numberformat:yyyy-mm-dd'>" + html + "</td>");
                    }
                    //数字
                    else
                    {
                        sb.Append("<td style='vnd.ms-excel.numberformat:#,##0.0000'>" + html + "</td>");
                    }
                }
                sb.Append("</tr>");
            }


            sb.Append("</table>");

            return sb.ToString();
        }

        private string GetRenderedHtmlSource(Control ctrl)
        {
            if (ctrl != null)
            {
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                    {
                        ctrl.RenderControl(htw);

                        return sw.ToString();
                    }
                }
            }
            return String.Empty;
        }


        #endregion

        protected int GetSelectedDataKeyID(Grid grid)
        {
            int id = -1;
            int rowIndex = grid.SelectedRowIndex;
            if (rowIndex >= 0)
            {
                id = Convert.ToInt32(grid.DataKeys[rowIndex][0]);
            }
            return id;
        }

        protected string GetSelectedDataKey(Grid grid, int dataIndex)
        {
            string data = String.Empty;
            int rowIndex = grid.SelectedRowIndex;
            if (rowIndex >= 0)
            {
                data = grid.DataKeys[rowIndex][dataIndex].ToString();
            }
            return data;
        }

        /// <summary>
        /// 获取表格选中项DataKeys的第一个值，并转化为整型列表
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        protected List<int> GetSelectedDataKeyIDs(Grid grid)
        {
            List<int> ids = new List<int>();
            foreach (int rowIndex in grid.SelectedRowIndexArray)
            {
                ids.Add(Convert.ToInt32(grid.DataKeys[rowIndex][0]));
            }

            return ids;
        }

        /// <summary>
        /// 获取表格选中项DataKeys的第一个值，并转化为字符串
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        protected List<string> GetSelectedDataKeyIDsString(Grid grid)
        {
            List<string> ids = new List<string>();
            foreach (int rowIndex in grid.SelectedRowIndexArray)
            {
                ids.Add(grid.DataKeys[rowIndex][0].ToString());
            }

            return ids;
        }
        protected object[] GetSelectedAllDataKeys(Grid grid, int dataIndex)
        {
            object[] datas = new string[grid.SelectedRowIndexArray.Length];
            //int rowIndex = grid.SelectedRowIndex;
            try
            {
                for (int i = 0; i < grid.SelectedRowIndexArray.Length; i++)
                {
                    datas[i] = grid.DataKeys[grid.SelectedRowIndexArray[i]][dataIndex];
                }
                return datas;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// datatable 分页
        /// </summary>
        /// <param name="pageIndex">页面序号</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="source">原datatable</param>
        /// <returns>分页后的数据datatable</returns>
        protected DataTable GetPagedDataTable(DataTable source, Grid grid)
        {
            int rowbegin = grid.PageIndex * grid.PageSize;
            int rowend = (grid.PageIndex + 1) * grid.PageSize;

            string sortField = grid.SortField;
            string sortDirection = grid.SortDirection;

            DataView view2 = source.DefaultView;

            view2.Sort = String.Format("{0} {1}", sortField, sortDirection);
            DataTable table = view2.ToTable();

            DataTable paged = table.Clone();

            if (rowend > source.Rows.Count)
            {
                rowend = source.Rows.Count;
            }

            for (int i = rowbegin; i < rowend; i++)
            {
                paged.ImportRow(table.Rows[i]);
            }

            return paged;
        }

        /// <summary>
        /// 返回select后的 的数据
        /// </summary>
        /// <param name="dt">原datatable</param>
        /// <param name="drs">datatable select后的行数组</param>
        /// <returns></returns>
        protected DataTable SelectDt(DataTable dt, DataRow[] drs)
        {
            DataTable t = dt.Clone();
            t.Clear();
            foreach (DataRow row in drs)
                t.ImportRow(row);
            return t;
        }

        public string GetCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["ClientId"];
            string cookiename = string.Empty;
            if (cookie != null)
            {
                cookiename = cookie.Value;
                // Do something with the clientId value
            }
            else
            {
                //创建cookie
             

                
                    // 如果客户端没有 "ClientId" cookie，则创建一个新的 cookie
                    cookie = new HttpCookie("ClientId");
                    cookie.Value = Guid.NewGuid().ToString();
                    cookie.Expires = DateTime.Now.AddDays(1);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                cookiename = cookie.Value;

            }
            return cookiename;
        }

        /// <summary>
        /// 双击筛选里点击后退对字典的筛除
        /// </summary>
        /// <param name="htClickColsName"></param>
        /// <param name="tbxname"></param>
        protected void BackSelect(FineUIPro.TextBox tbxname, List<MagnifyingFindDictionary> keyValuePairsSearch)
        {
            string[] s = tbxname.Text.Split(new string[] { "AND" }, StringSplitOptions.RemoveEmptyEntries );


            if (s == null || s.Length == 0)
            {
                return;
            }
            string cookie = GetCookie();
            keyValuePairsSearch.Remove(keyValuePairsSearch.Where(u=>u.Key==s[s.Length - 1]&&u.cooike==cookie).First());
            UpdateCol(tbxname,keyValuePairsSearch);

        }

        /// <summary>
        /// 根据字典更新前端textbox文字,noselect不给默认全选
        /// </summary>
        /// <param name="htClickColsName"></param>
        /// <param name="tbxname"></param>
        /// <param name="dictionaryname"></param>
        /// <param name="noselect"></param>
        protected void UpdateCol(FineUIPro.TextBox tbxname, List<MagnifyingFindDictionary> keyValuePairsSearch)
        {
            tbxname.Readonly = true;
            tbxname.Text = "";
            HttpCookie cookie = HttpContext.Current.Request.Cookies["ClientId"];
            string cookiename = string.Empty;
            if (cookie != null)
            {
                cookiename = cookie.Value;
                // Do something with the clientId value
            }
            foreach (var de in keyValuePairsSearch.Where(u=>u.cooike==cookiename).ToList())
            {
                tbxname.Text += de.Key + "AND";

            }
        }

        /// <summary>
        /// 双击将选中行的列名和ID添加到字典中
        /// </summary>
        /// <param name="Grid1"></param>
        /// <param name="htClickColsName"></param>
        /// <param name="e"></param>
        protected void TableSelectAddDic(Grid Grid1, GridRowClickEventArgs e, TextBox textBox, List<MagnifyingFindDictionary> keyValuePairsSearch)
        {
            string[] s = Grid1.SelectedCell;
            string cookie = GetCookie();
            
            for (int i = 0; i < Grid1.Columns.Count; i++)
            {
                var key = Grid1.Columns.Where(x => x.ColumnID == s[1]).Select(x => x.HeaderText).FirstOrDefault();

                if (s[1] == Grid1.Columns[i].ColumnID && keyValuePairsSearch.Where(u=>u.Key==key&&u.cooike==cookie).Count()==0)
                {
                    MagnifyingFindDictionary magnifyingFindDictionary = new MagnifyingFindDictionary() { Key= Grid1.Columns[i].HeaderText ,Value = Grid1.Rows[e.RowIndex].Values[i].ToString() 
                    ,cooike=cookie};
                     keyValuePairsSearch.Add(magnifyingFindDictionary);
                    break;
                }
            }
            UpdateCol(textBox,keyValuePairsSearch);

        }

        /// <summary>
        /// 显示放大镜表和列宽自动调整（所有列显示）
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="e"></param>
        public void TableShowStyle(Grid grid, GridRowEventArgs e)
        {
            foreach (GridColumn column in grid.Columns)
            {
                if (column.ColumnID!= "viewFJ")
                {
                    e.CellCssClasses[column.ColumnIndex] = "showsearch";
                }
                //"f-grid-cell-uneditable";
                //int headlength = Encoding.Default.GetByteCount(column.HeaderText) * 10;
                //int strlength = Encoding.Default.GetByteCount(e.Values[column.ColumnIndex].ToString().Replace(" ", "")) * 10;
                //if (column.Width.Value < strlength && strlength > headlength)
                //{
                //    column.Width = Unit.Pixel(strlength);
                //}
            }

        }

        /// <summary>
        /// 绑定数据时对bindgrid的改动
        /// </summary>
        public IQueryable<T> TableBindGrid<T>(IQueryable<T> q, Grid grid, List<MagnifyingFindDictionary> keyValuePairsSearch)
        {
            string cookie = GetCookie();
            foreach (var de in keyValuePairsSearch.Where(u=>u.cooike==cookie).ToList())
            {
                var Key = grid.Columns.Where(u => u.HeaderText == de.Key).Select(u => u.ColumnID).FirstOrDefault();
                var filters = new Filter[] { new Filter { Key = Key, Value = de.Value.ToString(), Contract = "=" } };
                //var expr = DbBaseExpand.SelectorExpToBool<SendDto>(de.Key.ToString(), de.Value.ToString());
                q = q.Where(filters);  //得到最终结果  

            }
            return q;
        }

        /// <summary>
        /// 绑定数据时对bindgrid的改动(当类型为datatable时)
        /// </summary>
        public DataTable TableBindGrid(DataTable q, Grid grid, List<MagnifyingFindDictionary> keyValuePairsSearch)
        {
            string cookie = GetCookie();
            foreach (var de in keyValuePairsSearch.Where(u => u.cooike == cookie).ToList())
            {
                //得到列的id
                var Key = grid.Columns.Where(u => u.HeaderText == de.Key).Select(u => u.ColumnID).FirstOrDefault();
                //var filters = new Filter[] { new Filter { Key = Key, Value = de.Value.ToString(), Contract = "=" } };
                //var expr = DbBaseExpand.SelectorExpToBool<SendDto>(de.Key.ToString(), de.Value.ToString());
                //q = q.Where(filters);  //得到最终结果  
               
                q = RemoveDataTable(q,Key,"<>",de.Value);
            }
            return q;
        }

        /// <summary>
        /// 选择符合条件的行
        /// </summary>
        /// <returns></returns>
        public DataTable RemoveDataTable(DataTable dts, string coloumname, string condition, string value)
        {

            DataRow[] foundRow;
            foundRow = dts.Select(coloumname + condition + $"'{value}'");
            foreach (DataRow row in foundRow)
            {
                dts.Rows.Remove(row);
            }

            return dts;
        }
        #endregion

        #region EF相关

        // 排序
        protected IQueryable<T> Sort<T>(IQueryable<T> q, FineUIPro.Grid grid)
        {
            if (Session["SortArray"] != null)
            {
                var fieldArray = Session["SortArray"] as string[];
                Session["SortArray"] = null;
                return q.SortByArray(fieldArray);
            }
            if (!string.IsNullOrEmpty(grid.SortField) && grid.SortField.Length > 0)
            {
                return q.SortByArray(new string[2] { grid.SortField , grid.SortDirection });
            }
            else if (grid.SortFieldArray != null && grid.SortFieldArray.Length > 0)
            {
                return q.SortByArray(grid.SortFieldArray)??q;
            }
            else
                return q.SortBy("ID ASC"); 

        }

        // 排序后分页
        protected IQueryable<T> SortAndPage<T>(IQueryable<T> q, FineUIPro.Grid grid)
        {
            if (grid.PageIndex >= grid.PageCount && grid.PageCount >= 1)
            {
                grid.PageIndex = grid.PageCount - 1;
            }
            grid.PageSize = ConfigHelper.PageSize;
            grid.ShowHeader = false;
            var backq = Sort(q, grid);
            return backq.Skip(grid.PageIndex * grid.PageSize).Take(grid.PageSize);
        }


        // 附加实体到数据库上下文中（首先在Local中查找实体是否存在，不存在才Attach，否则会报错）
        // http://patrickdesjardins.com/blog/entity-framework-4-3-an-object-with-the-same-key-already-exists-in-the-objectstatemanager
        protected T Attach<T>(int keyID) where T : class, IKeyID, new()
        {
            T t = DB.Set<T>().Local.Where(x => x.ID == keyID).FirstOrDefault();
            if (t == null)
            {
                t = new T { ID = keyID };
                DB.Set<T>().Attach(t);
            }
            return t;
        }

        // 向现有实体集合中添加新项
        protected void AddEntities<T>(ICollection<T> existItems, int[] newItemIDs) where T : class,  IKeyID, new()
        {
            foreach (int roleID in newItemIDs)
            {
                T t = Attach<T>(roleID);
                existItems.Add(t);
            }
        }

        // 替换现有实体集合中的所有项
        // http://stackoverflow.com/questions/2789113/entity-framework-update-entity-along-with-child-entities-add-update-as-necessar
        protected void ReplaceEntities<T>(ICollection<T> existEntities, int[] newEntityIDs) where T : class,  IKeyID, new()
        {
            if (newEntityIDs.Length == 0)
            {
                existEntities.Clear();
            }
            else
            {
                int[] tobeAdded = newEntityIDs.Except(existEntities.Select(x => x.ID)).ToArray();
                int[] tobeRemoved = existEntities.Select(x => x.ID).Except(newEntityIDs).ToArray();

                AddEntities<T>(existEntities, tobeAdded);

                existEntities.Where(x => tobeRemoved.Contains(x.ID)).ToList().ForEach(e => existEntities.Remove(e));
                //foreach (int roleID in tobeRemoved)
                //{
                //    existEntities.Remove(existEntities.Single(r => r.ID == roleID));
                //}
            }
        }

        // http://patrickdesjardins.com/blog/validation-failed-for-one-or-more-entities-see-entityvalidationerrors-property-for-more-details-2
        // ((System.Data.Entity.Validation.DbEntityValidationException)$exception).EntityValidationErrors

        #endregion

        #region 在线用户相关

        protected void UpdateOnlineUser(string username)
        {
            DateTime now = DateTime.Now;
            object lastUpdateTime = Session[SK_ONLINE_UPDATE_TIME];
            if (lastUpdateTime == null || (Convert.ToDateTime(lastUpdateTime).Subtract(now).TotalMinutes > 5))
            {
                // 记录本次更新时间
                Session[SK_ONLINE_UPDATE_TIME] = now;

                Online online = DB.Onlines.Where(o => o.User.Name == username).FirstOrDefault();
                if (online != null)
                {
                    online.UpdateTime = now;
                    DB.SaveChanges();
                }
            }
        }

        protected void RegisterOnlineUser(User user)
        {
            Online online = DB.Onlines.Where(o => o.User.ID == user.ID).FirstOrDefault();

            // 如果不存在，就创建一条新的记录
            if (online == null)
            {
                online = new Online();
                DB.Onlines.Add(online);
            }
            DateTime now = DateTime.Now;
            online.User = user;
            online.IPAdddress = Request.UserHostAddress;
            online.LoginTime = now;
            online.UpdateTime = now;

            DB.SaveChanges();

            // 记录本次更新时间
            Session[SK_ONLINE_UPDATE_TIME] = now;

        }

        /// <summary>
        /// 在线人数
        /// </summary>
        /// <returns></returns>
        protected int GetOnlineCount()
        {
            DateTime lastM = DateTime.Now.AddMinutes(-15);
            return DB.Onlines.Where(o => o.UpdateTime > lastM).Count();
        }

        #endregion

        #region 当前登录用户信息

        // http://blog.163.com/zjlovety@126/blog/static/224186242010070024282/
        // http://www.cnblogs.com/gaoshuai/articles/1863231.html
        /// <summary>
        /// 当前登录用户的角色列表
        /// </summary>
        /// <returns></returns>
        protected List<int> GetIdentityRoleIDs()
        {
            List<int> roleIDs = new List<int>();

            if (User.Identity.IsAuthenticated)
            {
                FormsAuthenticationTicket ticket = ((FormsIdentity)User.Identity).Ticket;
                string userData = ticket.UserData;

                foreach (string roleID in userData.Split(','))
                {
                    if (!String.IsNullOrEmpty(roleID))
                    {
                        roleIDs.Add(Convert.ToInt32(roleID));
                    }
                }
            }

            return roleIDs;
        }

        /// <summary>
        /// 当前登录用户名
        /// </summary>
        /// <returns></returns>
        protected string GetIdentityName()
        {
            if (User.Identity.IsAuthenticated)
            {
                return User.Identity.Name;
            }
            return String.Empty;
        }

        /// <summary>
        /// 当前登录用户名
        /// </summary>
        /// <returns></returns>
        protected string GetIdentityChineseName()
        {
            if (User.Identity.IsAuthenticated)
            {
                string chineseName = string.Empty;
                if (Session["ChineseName"] == null)
                {
                    User user = DB.Users.Where(o => o.Name == User.Identity.Name).FirstOrDefault();

                    Session["ChineseName"] = user.ChineseName;
                    chineseName = user.ChineseName;
                }
                else
                    chineseName = Session["ChineseName"].ToString();

                return chineseName;
            }
            return String.Empty;
        }



        /// <summary>
        /// 创建表单验证的票证并存储在客户端Cookie中
        /// </summary>
        /// <param name="userName">当前登录用户名</param>
        /// <param name="roleIDs">当前登录用户的角色ID列表</param>
        /// <param name="isPersistent">是否跨浏览器会话保存票证</param>
        /// <param name="expiration">过期时间</param>
        protected void CreateFormsAuthenticationTicket(string userName, string roleIDs, bool isPersistent, DateTime expiration)
        {
            // 创建Forms身份验证票据
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                userName,                       // 与票证关联的用户名
                DateTime.Now,                   // 票证发出时间
                expiration,                     // 票证过期时间
                isPersistent,                   // 如果票证将存储在持久性 Cookie 中（跨浏览器会话保存），则为 true；否则为 false。
                roleIDs                         // 存储在票证中的用户特定的数据
             );

            // 对Forms身份验证票据进行加密，然后保存到客户端Cookie中
            string hashTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashTicket);
            cookie.HttpOnly = true;
            // 1. 关闭浏览器即删除（Session Cookie）：DateTime.MinValue
            // 2. 指定时间后删除：大于 DateTime.Now 的某个值
            // 3. 删除Cookie：小于 DateTime.Now 的某个值
            if (isPersistent)
            {
                cookie.Expires = expiration;
            }
            else
            {
                cookie.Expires = DateTime.MinValue;
            }
            Response.Cookies.Add(cookie);
        }


        #endregion

        #region 当前登陆IP地址
        protected string GetLocalIp()
        {
            string ipAddress = string.Empty;
            if (Context.Request.ServerVariables["HTTP_VIA"] != null) // using proxy 
            {
                ipAddress = Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString(); // Return real client IP. 
            }
            else// not using proxy or can't get the Client IP 
            {
                ipAddress = Context.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP. 
            }
            return ipAddress;
        }
        #endregion

        #region 权限检查
        

        /// <summary>
        /// 检查当前用户是否拥有当前页面的浏览权限
        /// 页面需要先定义ViewPower属性，以确定页面与某个浏览权限的对应关系
        /// </summary>
        /// <returns></returns>
        protected bool CheckPowerView()
        {
            return CheckPower(ViewPower);
        }

        /// <summary>
        /// 检查当前用户是否拥有某个权限
        /// </summary>
        /// <param name="powerType"></param>
        /// <returns></returns>
        protected bool CheckPower(string powerName)
        {
            // 如果权限名为空，则放行
            if (String.IsNullOrEmpty(powerName))
            {
                return true;
            }

            // 当前登陆用户的权限列表
            List<string> rolePowerNames = GetRolePowerNames();
            if (rolePowerNames.Contains(powerName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前登录用户拥有的全部权限列表
        /// </summary>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        protected List<string> GetRolePowerNames()
        {
            // 将用户拥有的权限列表保存在Session中，这样就避免每个请求多次查询数据库
            if (Session["UserPowerList"] == null)
            {
                Debug.WriteLine("第一次请求获取权限");
                List<string> rolePowerNames = new List<string>();
                //LogInfo(GetIdentityName());
                // 超级管理员拥有所有权限
                if (GetIdentityName() == "admin"||GetIdentityName()=="000")
                {
                    rolePowerNames = DB.Powers.Select(p => p.Name).ToList();
                    
                }
                else
                {
                    List<int> roleIDs = GetIdentityRoleIDs();

                    foreach (var role in DB.Roles.Include(r => r.Powers).Where(r => roleIDs.Contains(r.ID)))
                    { 
                        foreach (var power in role.Powers)
                        {
                            if (!rolePowerNames.Contains(power.Name))
                            {
                                rolePowerNames.Add(power.Name);
                            }
                        }
                    }
                }

                Session["UserPowerList"] = rolePowerNames;
            }
            return (List<string>)Session["UserPowerList"];
        }

        #endregion

        #region 权限相关

        protected void CheckPowerFailWithPage()
        {
            Response.Write(CHECK_POWER_FAIL_PAGE_MESSAGE);
            Response.End();
        }

        protected void CheckPowerFailWithButton(FineUIPro.Button btn)
        {
            btn.Enabled = false;
            btn.ToolTip = CHECK_POWER_FAIL_ACTION_MESSAGE;
        }

        protected void CheckPowerFailWithLinkButtonField(FineUIPro.Grid grid, string columnID)
        {
            Debug.WriteLine(columnID);
            FineUIPro.LinkButtonField btn = grid.FindColumn(columnID) as FineUIPro.LinkButtonField;
            btn.Enabled = false;
            btn.ToolTip = CHECK_POWER_FAIL_ACTION_MESSAGE;
        }

        protected void CheckPowerFailWithWindowField(FineUIPro.Grid grid, string columnID)
        {
            FineUIPro.WindowField btn = grid.FindColumn(columnID) as FineUIPro.WindowField;
            btn.Enabled = false;
            btn.ToolTip = CHECK_POWER_FAIL_ACTION_MESSAGE;
        }

        protected void CheckPowerFailWithAlert()
        {
            PageContext.RegisterStartupScript(Alert.GetShowInTopReference(CHECK_POWER_FAIL_ACTION_MESSAGE));
        }

        protected void CheckPowerWithButton(string powerName, FineUIPro.Button btn)
        {
            if (!CheckPower(powerName))
            {
                CheckPowerFailWithButton(btn);
            }
        }

        protected void CheckPowerWithLinkButtonField(string powerName, FineUIPro.Grid grid, string columnID)
        {
            if (!CheckPower(powerName))
            {
                CheckPowerFailWithLinkButtonField(grid, columnID);
            }
        }

        protected void CheckPowerWithWindowField(string powerName, FineUIPro.Grid grid, string columnID)
        {
            if (!CheckPower(powerName))
            {
                CheckPowerFailWithWindowField(grid, columnID);
            }
        }

        /// <summary>
        /// 为删除Grid中选中项的按钮添加提示信息
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="grid"></param>
        protected void ResolveDeleteButtonForGrid(FineUIPro.Button btn, Grid grid)
        {
            ResolveDeleteButtonForGrid(btn, grid, "确定要删除选中的{0}项记录吗？");
        }

        protected void ResolveDeleteButtonForGrid(FineUIPro.Button btn, Grid grid, string confirmTemplate)
        {
            ResolveDeleteButtonForGrid(btn, grid, "请至少应该选择一项记录！", confirmTemplate);
        }

        protected void ResolveDeleteButtonForGrid(FineUIPro.Button btn, Grid grid, string noSelectionMessage, string confirmTemplate)
        {
            // 点击删除按钮时，至少选中一项
            btn.OnClientClick = grid.GetNoSelectionAlertInParentReference(noSelectionMessage);
            btn.ConfirmText = String.Format(confirmTemplate, "&nbsp;<span class=\"highlight\"><script>" + grid.GetSelectedCountReference() + "</script></span>&nbsp;");
            btn.ConfirmTarget = Target.Top;
        }

        #endregion
        /// <summary>
        /// 跳转到高级排序
        /// </summary>
        public void GetHighOrderSortUrl(Grid Grid1, Window Window1)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var headerTextList = Grid1.Columns.Select(u => u.HeaderText).Where(u => u != "序号" && u != "行号").ToList();
            foreach (var header in headerTextList)
            {
                dic.Add(header, Grid1.Columns.Where(u => u.HeaderText == header).Select(u => u.ColumnID).FirstOrDefault());
            }
            Session["AllColumn"] = dic;
            string[] localPathSplit = Request.Url.LocalPath.Split('/');
            string localPath = localPathSplit[localPathSplit.Length - 1];

            PageContext.RegisterStartupScript(Window1.GetShowReference($"~/AdvancedFilteringWebForm/AdvancedFilteringWebForm.aspx?path={localPath}"));
        }
        #region 产品版本

        public string GetProductVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}", v.Major, v.Minor);
        }

        #endregion

        #region 隐藏字段相关

        /// <summary>
        /// 从隐藏字段中获取选择的全部ID列表
        /// </summary>
        /// <param name="hfSelectedIDS"></param>
        /// <returns></returns>
        public List<int> GetSelectedIDsFromHiddenField(FineUIPro.HiddenField hfSelectedIDS)
        {
            JArray idsArray = new JArray();

            string currentIDS = hfSelectedIDS.Text.Trim();
            if (!String.IsNullOrEmpty(currentIDS))
            {
                idsArray = JArray.Parse(currentIDS);
            }
            else
            {
                idsArray = new JArray();
            }
            return new List<int>(idsArray.ToObject<int[]>());
        }

        /// <summary>
        /// 跨页保持选中项 - 将表格当前页面选中行对应的数据同步到隐藏字段中
        /// </summary>
        /// <param name="hfSelectedIDS"></param>
        /// <param name="grid"></param>
        public void SyncSelectedRowIndexArrayToHiddenField(FineUIPro.HiddenField hfSelectedIDS, Grid grid, GridPageEventArgs e)
        {
            List<int> ids = GetSelectedIDsFromHiddenField(hfSelectedIDS);

            List<int> selectedRows = new List<int>();
            if (grid.SelectedRowIndexArray != null && grid.SelectedRowIndexArray.Length > 0)
            {
                selectedRows = new List<int>(grid.SelectedRowIndexArray);
            }

            if (grid.IsDatabasePaging)
            {
                int count = 0;
                if (e.OldPageIndex<e.NewPageIndex)
                {
                    count =grid.PageSize;
                }
                else
                {
                    if(e.OldPageIndex * grid.PageSize<= grid.RecordCount)
                        count = grid.RecordCount - e.OldPageIndex * grid.PageSize;
                    else
                        count= grid.PageSize;
                }
                for (int i = 0; i < count; i++)
                {
                    int id = Convert.ToInt32(grid.DataKeys[i][0]);
                    if (selectedRows.Contains(i))
                    {
                        if (!ids.Contains(id))
                        {
                            ids.Add(id);
                        }
                    }
                    else
                    {
                        if (ids.Contains(id))
                        {
                            ids.Remove(id);
                        }
                    }
                }
            }
            else
            {
                int startPageIndex = grid.PageIndex * grid.PageSize;
                for (int i = startPageIndex, count = Math.Min(startPageIndex + grid.PageSize, grid.RecordCount); i < count; i++)
                {
                    int id = Convert.ToInt32(grid.DataKeys[i][0]);
                    if (selectedRows.Contains(i - startPageIndex))
                    {
                        if (!ids.Contains(id))
                        {
                            ids.Add(id);
                        }
                    }
                    else
                    {
                        if (ids.Contains(id))
                        {
                            ids.Remove(id);
                        }
                    }
                }
            }

            hfSelectedIDS.Text = new JArray(ids).ToString(Formatting.None);
        }

        /// <summary>
        /// 跨页保持选中项 - 根据隐藏字段的数据更新表格当前页面的选中行
        /// </summary>
        /// <param name="hfSelectedIDS"></param>
        /// <param name="grid"></param>
        public void UpdateSelectedRowIndexArray(FineUIPro.HiddenField hfSelectedIDS, Grid grid)
        {
            List<int> ids = GetSelectedIDsFromHiddenField(hfSelectedIDS);

            List<int> nextSelectedRowIndexArray = new List<int>();
            if (grid.IsDatabasePaging)
            {
                for (int i = 0, count = Math.Min(grid.PageSize, (grid.RecordCount - grid.PageIndex * grid.PageSize)); i < count; i++)
                {
                    int id = Convert.ToInt32(grid.DataKeys[i][0]);
                    if (ids.Contains(id))
                    {
                        nextSelectedRowIndexArray.Add(i);
                    }
                }
            }
            else
            {
                int nextStartPageIndex = grid.PageIndex * grid.PageSize;
                for (int i = nextStartPageIndex, count = Math.Min(nextStartPageIndex + grid.PageSize, grid.RecordCount); i < count; i++)
                {
                    int id = Convert.ToInt32(grid.DataKeys[i][0]);
                    if (ids.Contains(id))
                    {
                        nextSelectedRowIndexArray.Add(i - nextStartPageIndex);
                    }
                }
            }
            grid.SelectedRowIndexArray = nextSelectedRowIndexArray.ToArray();
        }

        #endregion

        #region 模拟树的下拉列表

        protected List<T> ResolveDDL<T>(List<T> mys) where T : ICustomTree, ICloneable, IKeyID, new()
        {
            return ResolveDDL<T>(mys, -1, true);
        }

        protected List<T> ResolveDDL<T>(List<T> mys, int currentId) where T : ICustomTree, ICloneable, IKeyID, new()
        {
            return ResolveDDL<T>(mys, currentId, true);
        }


        // 将一个树型结构放在一个下列列表中可供选择
        protected List<T> ResolveDDL<T>(List<T> source, int currentID, bool addRootNode) where T : ICustomTree, ICloneable, IKeyID, new()
        {
            List<T> result = new List<T>();

            if (addRootNode)
            {
                // 添加根节点
                T root = new T();
                root.Name = "--根节点--";
                root.ID = -1;
                root.TreeLevel = 0;
                root.Enabled = true;
                result.Add(root);
            }

            foreach (T item in source)
            {
                T newT = (T)item.Clone();
                result.Add(newT);

                // 所有节点的TreeLevel加一
                if (addRootNode)
                {
                    newT.TreeLevel++;
                }
            }

            // currentId==-1表示当前节点不存在
            if (currentID != -1)
            {
                // 本节点不可点击（也就是说当前节点不可能是当前节点的父节点）
                // 并且本节点的所有子节点也不可点击，你想如果当前节点跑到子节点的子节点，那么这些子节点就从树上消失了
                bool startChileNode = false;
                int startTreeLevel = 0;
                foreach (T my in result)
                {
                    if (my.ID == currentID)
                    {
                        startTreeLevel = my.TreeLevel;
                        my.Enabled = false;
                        startChileNode = true;
                    }
                    else
                    {
                        if (startChileNode)
                        {
                            if (my.TreeLevel > startTreeLevel)
                            {
                                my.Enabled = false;
                            }
                            else
                            {
                                startChileNode = false;
                            }
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region 日志记录

        protected void LogInfo(string message)
        {
            DB.Logss.Add(new Logss
            {
                Level = "Info",
                Message = message,
                LogTime = DateTime.Now
            });
            DB.SaveChanges();

        }

        #endregion

    }
}

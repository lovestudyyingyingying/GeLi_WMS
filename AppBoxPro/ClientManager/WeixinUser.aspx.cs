using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace NanXingGuoRen_WMS.ClientManager
{
    public partial class WeixinUser : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnClose.OnClientClick = ActiveWindow.GetHideReference();
                BindGrid1();
            }
        }

        private async void BindGrid1()
        {
            string access_token = "";
            using (var db = new AppContext())
            {

                WeiXinSetting q = db.weiXinSettings.Where(u => u.key == "ACCESS_TOKEN").FirstOrDefault();
                access_token = q.value;
            }
            //获取 tagid 为2 的用户openid
            //var client = new RestClient("https://api.weixin.qq.com");

            //var request = new RestRequest("cgi-bin/user/tag/get", Method.POST);
            //request.AddParameter("access_token", access_token, ParameterType.QueryString);

            //postinfo postinfo = new postinfo();
            //postinfo.tagid = 2;
            //postinfo.next_openid = "";
            //var json = JsonConvert.SerializeObject(postinfo);

            //request.AddParameter("application/json", json, ParameterType.RequestBody);


            //IRestResponse<Result> response = client.Execute<Result>(request);

            //获取所有用户的openid

            var client = new RestClient("https://api.weixin.qq.com");

            var request = new RestRequest("cgi-bin/user/get", Method.Get);
            request.AddParameter("access_token", access_token, ParameterType.QueryString);
            request.AddParameter("next_openid", "", ParameterType.QueryString);

            RestResponse<Result> response = await client.ExecuteAsync<Result>(request);

            //所有的openid
            List<string> openidlist = response.Data.data.openid;


            //批量获取 所有 用户基本信息
            GetStatUser(openidlist, access_token);



        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="openidlist">用户列表</param>
        /// <param name="access_token">access_token</param>
        private async void GetStatUser(List<string> openidlist, string access_token)
        {
            var client = new RestClient("https://api.weixin.qq.com");
            var request = new RestRequest("cgi-bin/user/info/batchget", Method.Post);
            request.AddParameter("access_token", access_token, ParameterType.QueryString);

            BaseUser baseUser = new BaseUser();
            List<user_list> user_Lists = new List<user_list>();
            foreach (string openid in openidlist)
            {
                user_list item = new user_list();
                item.openid = openid;
                item.lang = "zh_CN";
                user_Lists.Add(item);
            }

            baseUser.user_list = user_Lists;
            request.AddParameter("application/json", JsonConvert.SerializeObject(baseUser), ParameterType.RequestBody);

            RestResponse<user_info> user_info = await client.ExecuteAsync<user_info>(request);

            var q = user_info.Data.user_info_list;

            var linqlist = from s in q
                           select new user_info_list
                           {
                               nickname = s.nickname,
                               openid = s.openid
                           };

            // 在用户名称中搜索
            string searchText = tbxsearch.Text.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                linqlist = linqlist.Where(u=>u.nickname != null&&u.nickname.Contains(searchText));

            }

            Grid1.RecordCount = linqlist.Count();

            

            Grid1.DataSource = linqlist;
            Grid1.DataBind();

        }

        protected void Grid1_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            SelectGridRow();
        }

        protected void btnSaveClose_Click(object sender, EventArgs e)
        {
            SelectGridRow();
        }


        private void SelectGridRow()
        {
            string nickname = Grid1.DataKeys[Grid1.SelectedRowIndex][0].ToString();
            string openid  = Grid1.DataKeys[Grid1.SelectedRowIndex][1].ToString();

            JObject dataObj = new JObject();
            dataObj.Add("rowid", Request.QueryString["rowid"]);
            dataObj.Add("nickname",nickname);
            dataObj.Add("openid", openid);

            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference(dataObj.ToString(Newtonsoft.Json.Formatting.None)));
        }

        [Serializable]
        public class postinfo
        {
            public int tagid { get; set; }

            public string next_openid { get; set; }
        }

        public class Result
        {
            public int count { get; set; }

            public data data { get; set; }

            public string next_openid { get; set; }
        }

        public class data
        {
            public List<string> openid { get; set; }
        }



        /// <summary>
        /// 
        /// </summary>
        public class BaseUser
        {
            public List<user_list> user_list { get; set; }
        }

        public class user_list
        {
            public string openid { get; set; }
            public string lang { get; set; }
        }

        /// <summary>
        /// 返回结果
        /// </summary>

        public class user_info
        {
            public List<user_info_list> user_info_list { get; set; }
        }

        public class user_info_list
        {
            /// <summary>
            /// 是否关注改订阅号
            /// </summary>
            public string subscribe { get; set; }

            public string openid { get; set; }

            public string nickname { get; set; }
        }

        protected void btnsearche_Click(object sender, EventArgs e)
        {
            BindGrid1();
        }

    }
}
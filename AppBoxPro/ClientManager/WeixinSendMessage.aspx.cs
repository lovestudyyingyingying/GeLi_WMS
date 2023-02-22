using FineUIPro;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NanXingGuoRen_WMS.ClientManager
{
    public partial class WeixinSendMessage : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {

        }

        protected void btnsend_Click(object sender, EventArgs e)
        {
            string access_token = "";
            using (var db = new AppContext())
            {

                WeiXinSetting qq = db.weiXinSettings.Where(u => u.key == "ACCESS_TOKEN").FirstOrDefault();
                access_token = qq.value;
            }

            string openid = GetQueryValue("openid");

            var client = new RestClient("https://api.weixin.qq.com");
            var request = new RestRequest("cgi-bin/message/template/send", Method.Post);
            request.AddParameter("access_token", access_token, ParameterType.QueryString);

            //模板消息id
            string template_id = "caDPtcxg9N8DPNneIZZXYYDWHJHXcARdgt4AY0CxX3Y";

            var q = new
            {
                touser = openid,
                template_id = template_id,
                url = "http://telen.cn",
                data = new
                {
                    first = new
                    {
                        value = tbxfirst.Text,
                        color = ""
                    },
                    keyword1 = new
                    {
                        value = keyword1.Text,
                        color = ""
                    },
                    keyword2 = new
                    {
                        value = keyword2.SelectedValue,
                        color = ""
                    },
                    keyword3 = new
                    {
                        value = keyword3.Text,
                        color = ""
                    },
                    keyword4 = new
                    {
                        value = keyword4.Text,
                        color = ""
                    },
                    keyword5 = new
                    {
                        value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        color = ""
                    },
                    remark = new
                    {
                        value = tbxremark.Text,
                        color=""
                    }
                }
            };

            string jsonstr = JsonConvert.SerializeObject(q);

            request.AddParameter("application/json", jsonstr, ParameterType.RequestBody);


            //返回数据
  //          {
  //              "errcode":0,
  //   "errmsg":"ok",
  //   "msgid":200228332
  //}
            var user_info = client.ExecuteAsync(request);
        }


    }
}
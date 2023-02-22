using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Quartz;
using SQLHelper;
using GeLiPage_WMS;
using RestSharp;
using EntityFramework.Extensions;
using System.Threading.Tasks;

namespace GeLiPage_WMS.AppModel
{
    public class WeixinToken : IJob
    {
        public class Token
        {
            public string access_token { get; set; }

            public string expires_in { get; set; }
        }
        public async void Execute(IJobExecutionContext context)
        {
            try
            {
                var client = new RestClient("https://api.weixin.qq.com");

                var request = new RestRequest("cgi-bin/token", Method.Get);
                request.AddParameter("grant_type", "client_credential");
                request.AddParameter("appid", ConfigurationManager.AppSettings["appid"].ToString());
                request.AddParameter("secret", ConfigurationManager.AppSettings["appsecret"].ToString());

                /*                                
               //方式1：按指定的格式返回响应文本
               IRestResponse response = client.Execute(request);
               var content = response.Content;//按指定的格式返回响应文本，如json
               */

                RestResponse<Token> response = await client.ExecuteAsync<Token>(request);
                
                using (var db = new AppContext())
                {
                    if (db.weiXinSettings.Any(u => u.key == "ACCESS_TOKEN"))
                    {
                        //如果存在，则更新
                        db.weiXinSettings.Where(u => u.key == "ACCESS_TOKEN").Update(
                            u => new WeiXinSetting { value = response.Data.access_token,
                                expiraiton_time = DateTime.Now.AddHours(2) });


                    }
                    else
                    {
                        //新增
                        WeiXinSetting item = new WeiXinSetting();
                        item.key = "ACCESS_TOKEN";
                        item.value = response.Data.access_token;
                        item.expiraiton_time = DateTime.Now.AddHours(2);
                        db.weiXinSettings.Add(item);
                        db.SaveChanges();

                    }
                }

            }
            catch
            {

            }
        }
    }
}
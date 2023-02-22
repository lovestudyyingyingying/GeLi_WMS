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

namespace GeLiPage_WMS.AppModel
{
    public class WriteText: IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            DbHelperSQL.connectionString = ConfigurationManager.ConnectionStrings["Default"].ToString();
            DataTable dt=  DbHelperSQL.ReturnDataTable("select d.proname,d.spec,d.quantity ,ISNULL( b.kucun,0 ) as kucun from YW_Lableinfor d left join (select a.prosn,SUM(a.q1)-SUM(a.q2) as 'kucun'  from   (select   prosn,case processno when '02' then quantity else 0 end as 'q1',case processno when '03' then  quantity else 0 end as 'q2' from YW_ProcessRec ) as a   group by prosn) as b on d.prosn = b.prosn");
            // zuidikucun 最低库存   //kuncun 库存

            string mes = "";
            foreach(DataRow row in dt.Rows)
            {
                if (double.Parse(row["quantity"].ToString()) >= double.Parse(row["kucun"].ToString()))
                {
                    mes += "名称:" + row["proname"].ToString() + "规格:" + row["spec"].ToString() + "现在库存为:" + row["kucun"].ToString() + ",已低于等于库存数警戒线" + row["quantity"].ToString() + "\r\n";
                }
            }

            if (mes!="")
            {
                string email = ConfigHelper.Mail;

                string[] emails = email.Split(',');
                for(int i = 0; i < emails.Length; i++)
                {
                    SendEmail("库存预警提示", mes,emails[i]);
                }
            }
        }

        public void SendEmail(string title, string mes,string address)
        {
            MailMessage mailMessage = new MailMessage();
            //发件人
            mailMessage.From = new MailAddress("291477662@qq.com");
            //收件人
            mailMessage.To.Add(new MailAddress(address));
            //邮件标题。
            mailMessage.Subject = title;
            //邮件内容。
            mailMessage.Body = mes;
            //实例化一个SmtpClient类。
            SmtpClient client = new SmtpClient();
            //在这里我使用的是qq邮箱，所以是smtp.qq.com，如果你使用的是126邮箱，那么就是smtp.126.com。
            client.Host = "smtp.qq.com";
            //使用安全加密连接。
            client.EnableSsl = true;
            //不和请求一块发送。
            client.UseDefaultCredentials = false;
            //验证发件人身份(发件人的邮箱，邮箱里的生成授权码);
            //gaczbswgbdbxbjei
            client.Credentials = new NetworkCredential("291477662@qq.com", "gaczbswgbdbxbjei");
            //发送
            client.Send(mailMessage);
        }



    }
}
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.IO;
using System.Data.Entity;
using Quartz;
using Quartz.Impl;
using GeLiPage_WMS.AppModel;
using System.Configuration;

namespace GeLiPage_WMS
{
    public class Global : System.Web.HttpApplication
    {
        //调度器
        IScheduler scheduler;
        //调度工厂
        ISchedulerFactory factory;

        protected void Application_Start(object sender, EventArgs e)
        {
            Database.SetInitializer(new GeLiPage_WMSDatabaseInitializer());
            //Database.SetInitializer<GeLiPage_WMSContext>(null);


            
            //创建一个调度器
            factory = new StdSchedulerFactory();
            scheduler = factory.GetScheduler();
            scheduler.Start();
            //创建一个任务
            //IJobDetail job = JobBuilder.Create<WriteText>().WithIdentity("job1", "group1").Build();

            //创建一个触发器,定义了什么时间任务开始或每隔多久执行一次
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithCronSchedule(ConfigurationManager.AppSettings["warningtime"].ToString())
                
                //.StartAt(runTime)
                .Build();//

            //自动刷新token

            IJobDetail job2 = JobBuilder.Create<WeixinToken>().WithIdentity("job2", "group2").Build();

            //创建一个触发器,定义了什么时间任务开始或每隔多久执行一次 1.5小时执行一次
            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group2")
                .WithCronSchedule(ConfigurationManager.AppSettings["tokentime"].ToString())

                //.StartAt(runTime)
                .Build();//


            //将任务与触发器添加到调度器中
            //scheduler.ScheduleJob(job, trigger);

            //将任务与触发器添加到调度器中
            //scheduler.ScheduleJob(job2, trigger2);
            //开始执行
            //scheduler.Start();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }
        
        protected virtual void Application_EndRequest()
        {
            var context = HttpContext.Current.Items["__GeLiPage_WMSContext"] as GeLiPage_WMSContext;
            if (context != null)
            {
                context.Dispose();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            if (scheduler != null)
            {
                //是否等待任务的完成再结束
                scheduler.Shutdown(true);
            }
        }
    }
}

using GeLiService_WMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GeLiBackService
{
     static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
         static void Main()
        {
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Service1()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                      ex.ToString()));
            }
            
           
        }
    }
}

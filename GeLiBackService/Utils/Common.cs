using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiBackService.Utils
{
    public static class Common
    {
        public static void WriteLogs(string content)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string LogName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace.Split('.')[0];
            string[] sArray = path.Split(new string[] { LogName }, StringSplitOptions.RemoveEmptyEntries);
            string aa = sArray[0] + "\\" + LogName + "Log\\";
            path = aa;
            if (!string.IsNullOrEmpty(path))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";//
                if (!File.Exists(path))
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                }
                if (File.Exists(path))
                {
                    StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "----" + content + "\r\n");
                    sw.Close();
                }
            }
        }
    }
}

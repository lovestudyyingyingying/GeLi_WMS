using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils
{
    public class ReckonSecondUtils
    {
        /// <summary>
        /// 计算使用时间
        /// </summary>
        /// <param name="dt1">开始时间</param>
        /// <param name="dt2">结束时间</param>
        /// <returns>相隔的秒数</returns>
        public double ReckonSeconds(DateTime dt1, DateTime dt2)
        {
            TimeSpan ts = (dt2 - dt1).Duration();

            double second = 0;
            if (ts.Hours > 0)
            {
                second += ts.Hours * 3600;
            }
            if (ts.Minutes > 0)
            {
                second += ts.Minutes * 60;
            }
            second += ts.Seconds;
            second += (ts.Milliseconds * 0.001);

            return second;

        }

    }
}

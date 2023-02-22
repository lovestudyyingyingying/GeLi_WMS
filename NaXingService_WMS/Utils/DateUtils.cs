using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils
{
    public class DateUtils
    {
        public static DateTime[] GetAllDates(DateTime sDate,DateTime eDate)
        {
            List<DateTime> li = new List<DateTime>();
         
            while (sDate <= eDate)
            {
                li.Add(sDate.Date);
                sDate = sDate.AddDays(1);
            }
            DateTime[] timearr = li.ToArray();//两个日期之间所有的日期
            return timearr;
        }
        public static DateTime[] GetAllHours(DateTime sDate, DateTime eDate)
        {
            List<DateTime> li = new List<DateTime>();

            while (sDate <= eDate)
            {
                li.Add(sDate);
                sDate = sDate.AddHours(1);
            }
            DateTime[] timearr = li.ToArray();//两个日期之间所有的分钟
            return timearr;
        }


        public static DateTime[] GetAllMinutes(DateTime sDate, DateTime eDate)
        {
            List<DateTime> li = new List<DateTime>();

            while (sDate <= eDate)
            {
                li.Add(sDate);
                sDate = sDate.AddMinutes(1);
            }
            DateTime[] timearr = li.ToArray();//两个日期之间所有的分钟
            return timearr;
        }

    }
}

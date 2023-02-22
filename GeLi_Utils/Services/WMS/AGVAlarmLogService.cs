using GeLiData_WMS;
using GeLiData_WMSUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Services.WMS
{
    public class AGVAlarmLogService:DbBase<AGVAlarmLog>
    {
        public IQueryable<AGVAlarmLog> GetLogByTime(DateTime dateTime,DateTime dateTimeEnd)
        {
            return GetIQueryable(u => u.alarmDate >= dateTime&&u.alarmDate<=dateTimeEnd).OrderByDescending(u=>u.ID);

        }
    }
}

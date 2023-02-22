using GeLiData_WMS;
using GeLiData_WMSUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Services.WMS
{
    public class WareAreaClassService:DbBase<WareAreaClass>
    {
        public List<string> GetAllWareAreaClassName()
        {
            return GetAllQueryable().Select(u => u.AreaClass).ToList();
        }
    }
}

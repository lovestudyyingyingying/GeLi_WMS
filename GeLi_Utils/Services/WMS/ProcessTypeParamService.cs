using GeLiData_WMS.Dao;
using GeLiData_WMSUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace GeLi_Utils.Services.WMS
{
    public class ProcessTypeParamService : DbBase<ProcessTypeParam>
    {
        public ProcessTypeParam GetEntity(string processName ,string protype)
        {
          return  GetIQueryable(u => u.protype == protype && u.processName == processName,true,DbMainSlave.Master).FirstOrDefault();

        }
    }
}

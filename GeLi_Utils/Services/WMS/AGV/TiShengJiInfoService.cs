using GeLiData_WMS;
using GeLiData_WMSUtils;
using System.Linq;

namespace GeLiService_WMS.Services.WMS.AGV
{
    public class TiShengJiInfoService:DbBase<TiShengJiInfo>
    {
       public TiShengJiInfo GetInfoByIp(string ip)
        {
            return GetIQueryable(u=>u.TsjIp==ip,true,DbMainSlave.Master).FirstOrDefault();
        }
    }
}

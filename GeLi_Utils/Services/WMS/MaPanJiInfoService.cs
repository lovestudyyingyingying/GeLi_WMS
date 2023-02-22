using GeLiData_WMS.Dao;
using GeLiData_WMSUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Services.WMS
{
    public class MaPanJiInfoService:DbBase<MaPanJiInfo>
    {

        /// <summary>
        /// 获取码盘机状态
        /// </summary>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public string GetMaPanJiStateByIp(string Ip)
        {
            var mapanji = GetIQueryable(u => u.MpjIp == Ip,true,DbMainSlave.Master).FirstOrDefault();
            return mapanji == null ? string.Empty : (mapanji.MaPanJiState == null ? string.Empty : mapanji.MaPanJiState.Reserve1);
        }

        /// <summary>
        /// 获取码盘机
        /// </summary>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public MaPanJiInfo GetMaPanJiEntityByIp(string Ip)
        {
           
            return  GetIQueryable(u => u.MpjIp == Ip,true,DbMainSlave.Master).FirstOrDefault();
        }

        /// <summary>
        /// 获取码盘机状态
        /// </summary>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public string GetMaPanJiStateByMaPanJiName(string name)
        {
            var mapanji = GetIQueryable(u => u.MpjName == name).FirstOrDefault();
            if(mapanji.IsError)
                return "码盘机故障";
            return mapanji == null ? string.Empty : (mapanji.MaPanJiState == null ? string.Empty : mapanji.MaPanJiState.Reserve1);
        }
    }
}

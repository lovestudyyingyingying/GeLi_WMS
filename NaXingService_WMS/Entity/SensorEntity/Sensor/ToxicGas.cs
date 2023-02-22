using NanXingService_WMS.Utils.SensorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.SensorEntity
{
    /// <summary>
    /// 有毒气体传感器
    /// </summary>
    public class ToxicGas : IEquipment, ISensor<float>
    {
        /// <summary>
        /// 设备站号
        /// </summary>
        public int StationAddress { get; set; }

        public string EquipmentName { get => "有毒气体传感器"; }
        public string ReceiveMessage { get; set; }
        public byte[] CommandCode { get => SensorTools.GetDatagramBytes("01 03 00 00 00 01 84 0A"); }

        public float GetResult()
        {
            try
            {
                return SensorTools.CalculateHexToInt(ReceiveMessage.Substring(6, 4));
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                ReceiveMessage=String.Empty;
            }
            
        }
    }
}

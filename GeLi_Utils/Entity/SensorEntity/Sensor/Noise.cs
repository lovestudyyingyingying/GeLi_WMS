using GeLiService_WMS.Utils.SensorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.SensorEntity
{
    /// <summary>
    /// 噪声传感器
    /// </summary>
    public class Noise : IEquipment, ISensor<float>
    {
        public int StationAddress { get; set; }
        public string EquipmentName { get => "噪声传感器"; }
        public string ReceiveMessage { get; set; }
        public byte[] CommandCode { get => SensorTools.GetDatagramBytes("13 03 00 00 00 01 87 78"); }

        /// <summary>
        /// 量程上限
        /// </summary>
        private float URV { get => 150f; }

        /// <summary>
        /// 量程下限
        /// </summary>
        private float LRV { get => 0f; }

        public float GetResult()
        {
            try
            {
                float tx= SensorTools.CalculateHexToInt(ReceiveMessage.Substring(6, 4));
              
                return tx>URV?URV:tx;
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

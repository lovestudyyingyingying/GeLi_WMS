using GeLiService_WMS.Utils.SensorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.SensorEntity
{
    /// <summary>
    /// 电表
    /// </summary>
    public class Ammeter : IEquipment, ISensor<float[]>
    {
        public int StationAddress { get; set; }
        public string EquipmentName { get => "电表"; }
        public string ReceiveMessage { get; set; }
        public byte[] CommandCode { get => SensorTools.GetDatagramBytes("0B 03 00 10 00 03 04 A4"); }

        /// <summary>
        /// 量程上限
        /// </summary>
        private float URV { get => 80f; }

        /// <summary>
        /// 量程下限
        /// </summary>
        private float LRV { get => 0f; }

        public float[] GetResult()
        {
            float[] resultArr;
            try
            {
                resultArr = new float[3];

                for (int i = 0; i < resultArr.Length; i++)
                {
                    string str = ReceiveMessage.Substring(6 + i * 4, 4);
                    resultArr[i] = CalculateResult(str);
                }
            }
            catch (Exception)
            {
                resultArr = new float[] { 0f, 0f, 0f };
            }
            ReceiveMessage=String.Empty;
            return resultArr;
        }

        /// <summary>
        /// 计算数据
        /// </summary>
        /// <param name="strNum">读取到的数据字符串</param>
        /// <returns></returns>
        private float CalculateResult(string strNum)
        {
            float result = CalculateFormula(SensorTools.CalculateHexToInt(strNum), this.URV, this.LRV);
            return result;
        }

        /// <summary>
        /// 电流数据计算公式
        /// </summary>
        /// <param name="data">读取到的数据</param>
        /// <param name="h">量程上限</param>
        /// <param name="l">量程下限</param>
        /// <returns></returns>
        private float CalculateFormula(float data, float h, float l)
        {
            return data * (h - l) / 10000 - Math.Abs(l);
        }
    }
}

using GeLiService_WMS.Utils.SensorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.SensorEntity
{
    /// <summary>
    /// 温控器
    /// </summary>
    public class Thermostat : IEquipment, ISensor<float>
    {
        public string EquipmentName { get => "温控器"; }
        public string ReceiveMessage { get; set; }
        public byte[] CommandCode { get => GetCmdCode(); }

        /// <summary>
        /// 温控器站地址
        /// </summary>
        public int StationAddress { get; set; }

        /// <summary>
        /// 获取读取数据报文
        /// </summary>
        /// <returns></returns>
        private byte[] GetCmdCode()
        {
            string code = "0300000002";
            string stationAddress = $"{this.StationAddress:x2}";
            code = GetSpace(stationAddress + code + GetCheckSum(stationAddress, code)).Trim();

            return SensorTools.GetDatagramBytes(code);
        }

        /// <summary>
        /// 计算CRC16校验码
        /// </summary>
        /// <param name="stationAddress">站地址</param>
        /// <param name="controlCode">控制码</param>
        /// <returns></returns>
        private string GetCheckSum(string stationAddress, string controlCode)
        {
            string code = stationAddress + controlCode;
            var strArr = Regex.Matches(code, ".{2}").Cast<Match>().Select(m => m.Value);
            byte[] pDataBytes = new byte[strArr.Count()];
            int temp = 0;
            foreach (var item in strArr)
            {
                pDataBytes[temp] = Convert.ToByte(item, 16);
                temp++;
            }
            ushort crc = 0xffff;
            ushort polynom = 0xA001;

            for (int i = 0; i < pDataBytes.Length; i++)
            {
                crc ^= pDataBytes[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x01) == 0x01)
                    {
                        crc >>= 1;
                        crc ^= polynom;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            string checkSum = crc.ToString("x4").ToUpper();
            int length = checkSum.Length;

            if (length > 4)
            {
                checkSum = checkSum.Substring(length - 5, 4);
            }

            checkSum = checkSum.Substring(2, 2) + checkSum.Substring(0, 2);

            return checkSum;
        }

        /// <summary>
        /// 添加空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string GetSpace(string str)
        {
            return string.Join(" ", Regex.Matches(str, @"..").Cast<Match>().ToList());
        }

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
        }
    }
}
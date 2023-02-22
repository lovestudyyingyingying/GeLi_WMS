using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.SensorUtils
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class SensorTools
    {
        /// <summary>
        /// 16进制转换为10进制计算方法
        /// </summary>
        /// <param name="hexNbr">需要计算的十六进制数</param>
        /// <returns></returns>
        public static float CalculateHexToInt(string hexNbr)
        {
            int number = 0;

            for (int i = 0; i < hexNbr.Length; i++)
            {
                int nbr = int.Parse(hexNbr[i].ToString(), System.Globalization.NumberStyles.HexNumber);
                string[] arr = new string[2];
                int pow = Convert.ToInt32(Math.Pow(16, 3 - i));

                number += nbr * pow;
            }

            float reslut = (float)number / 10;

            return reslut;
        }

        /// <summary>
        /// 获取报文数组
        /// </summary>
        /// <param name="datagram">报文</param>
        /// <returns></returns>
        public static byte[] GetDatagramBytes(string datagram)
        {
            string[] strArray = datagram.Split(' ');

            int byteBufferLength = strArray.Length;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i] == "")
                {
                    byteBufferLength--;
                }
            }

            byte[] byteBuffer = new byte[byteBufferLength];
            int ii = 0;
            for (int i = 0; i < strArray.Length; i++)
            {
                int decNum = 0;
                if (strArray[i] == "")
                {
                    continue;
                }
                else
                {
                    decNum = Convert.ToInt32(strArray[i], 16);
                }

                try
                {
                    byteBuffer[ii] = Convert.ToByte(decNum);
                }
                catch (System.Exception)
                {
                    return null;
                }

                ii++;
            }

            return byteBuffer;
        }

        //string sql = @"select 列名称 from 表名称";

        
        ////1、从类 里面拿到 所有的 属性 数组
        ////2、遍历所有的属性 
        //string sql2 = @"select 属性1、2、3、4、5  from 表名称";
    }
}

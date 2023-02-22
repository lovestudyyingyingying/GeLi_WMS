using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.PlcUtils
{
    class PLCInstance
    {
        private PLCInstance() { }

        /// <summary>
        /// PLC单例
        /// </summary>
        public static PLCInstance Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        /// <summary>
        /// 防止调用此类静态方法时，创建新的实例
        /// </summary>
        private class Nested
        {
            internal static readonly PLCInstance instance = null;
            static Nested()
            {
                instance = new PLCInstance();
            }
        }

        /// <summary>
        /// 私有PLC单例对象
        /// </summary>
        private static Plc plcObj = new Plc(CpuType.S7200Smart, "192.168.0.61", 0, 1);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public float GetPLCData()
        {
            float data;
            try
            {
                if (ConnectToPLC())
                {
                    data = GetDieselOilFlow();

                    Disconnect();
                }
                else
                {
                    data = 0f;
                    Logger.Default.Process(new Log("Warn", $"PLC has disconnected."));

                }

            }
            catch (Exception e)
            {

                data = 0f;
                Logger.Default.Process(new Log("Warn", $" {e.ToString()} "));

            }
            //当前获取数据为柴油流量
            return data;

        }

        /// <summary>
        /// 写入数据
        /// </summary>
        public void SetPLCData()
        {
            ConnectToPLC();

            //写入数据为Q1.7位取反，控制报警器开关
            bool alarmStatus = (bool)plcObj.Read("Q1.7");
            plcObj.Write("Q1.7", !alarmStatus);

            Disconnect();
        }

        /// <summary>
        /// 连接至PLC并返回连接状态
        /// </summary>
        /// <returns></returns>
        private bool ConnectToPLC()
        {
            try
            {
                plcObj.Open();
                return plcObj.IsConnected ? true : false;
            }
            catch (Exception)
            {

                return false;
            }

        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void Disconnect()
        {
            plcObj.Close();
        }

        /// <summary>
        /// 获取柴油流量（升）
        /// </summary>
        /// <returns></returns>
        private float GetDieselOilFlow()
        {
            //VD1092单位为升，VD1100单位为毫升
            float result = (float)plcObj.Read(DataType.DataBlock, 1, 1092, VarType.Real, 1);

            return result;
        }
    }
}

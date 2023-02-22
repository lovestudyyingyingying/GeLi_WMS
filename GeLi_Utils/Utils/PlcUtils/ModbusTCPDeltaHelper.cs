/*****************************************************
The MIT License (MIT)
Copyright (c) 2006 Scott Alexander, 2015 Dmitry Turin
see Docs/LICENSE.txt
******************************************************/

using HslCommunication.ModBus;
using NModbus;
using NModbus.Extensions.Enron;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GeLiService_WMS
{
    public class ModbusTCPDeltaHelper
    {


        public string Ip { get; set; }

        public int Port { get; set; }

        ModbusTcpNet modbusTcpNet;

        /// <summary>
        /// Creates ModbusTCP connection
        /// </summary>
        /// <param name="ip">controller ip address</param>
        /// <param name="port">端口，默认传502</param>
        /// <param name="connectionUpdateTime">time in milliseconds used as connection check period</param>
        public ModbusTCPDeltaHelper(string ip, int port)
        {
            modbusTcpNet = new ModbusTcpNet(ip, port);
            Ip = ip;
            Port = port;
        }
        /// <summary>
        /// 读取线圈状态，返回空或者结果
        /// </summary>
        /// <param name="startAddress"></param>
        /// <returns></returns>
        public bool? ReadBoolean(string startAddress)
        {
            ushort formatStartAddress = GetAddressIntValue(startAddress);




            var result = modbusTcpNet.ReadBool(formatStartAddress.ToString());
            if (result.IsSuccess)
            {
                return result.Content;
            }
            else
            {
                return null;
            }


        }

        /// <summary>
        /// 读取连续线圈状态
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<bool> ReadManyBoolean(string startAddress, ushort num)
        {
            ushort formatStartAddress = GetAddressIntValue(startAddress);
            var r = modbusTcpNet.ReadCoil(formatStartAddress.ToString(), num);
            if (r.IsSuccess)
            {
                return r.Content.ToList();

            }
            else
            {
                return null;
            }

        }


        /// <summary>
        /// 读取C区数量
        /// </summary>
        /// <param name="startAddress"></param>
        /// <returns></returns>
        public int? ReadDecimal(string startAddress)
        {
            ushort formatStartAddress = GetAddressIntValue(startAddress);

            var r = modbusTcpNet.ReadInt32(formatStartAddress.ToString());
            if (r.IsSuccess)
            {
                return r.Content;
            }
            else
            {
                return null;
            }


        }



        public bool WriteBoolean(string startAddress, bool value)
        {
            ushort formatStartAddress = GetAddressIntValue(startAddress);
            return modbusTcpNet.WriteCoil(formatStartAddress.ToString(), value).IsSuccess;
        }

        public bool WriteDecimal(string startAddress, short value)
        {
            ushort formatStartAddress = GetAddressIntValue(startAddress);
            return modbusTcpNet.WriteOneRegister(startAddress, value).IsSuccess;
        }

        private readonly string[] negativeSigns = { "8", "9", "A", "B", "C", "D", "E", "F" };
        public string GetSignInt(string hex)
        {
            //is number positive or negative?
            bool isNegativeNumber = false;
            foreach (var n in negativeSigns)
            {
                if (n == hex.Substring(0, 1))
                {
                    isNegativeNumber = true;
                    break;
                }
            }

            //find 2s component of hex number; first 4 symbols equal F means number is negative
            int result;
            if (isNegativeNumber)
            {
                uint intVal = Convert.ToUInt32(hex, 16);
                uint twosComp = ~intVal + 1;
                string h = string.Format("{0:X}", twosComp);

                result = int.Parse(h.Substring(4), NumberStyles.HexNumber);
                if (h.Substring(0, 4) == "FFFF")
                    result = -result;
            }
            else
            {
                result = int.Parse(hex, NumberStyles.HexNumber);
            }

            return result.ToString();
        }

        public ushort GetHexFromInt(int value)
        {
            string hex = value.ToString("X8");
            return ushort.Parse(hex.Substring(4), NumberStyles.HexNumber);
        }

        public void Subscribe(string startAddress, Action<string> function, int mstime)
        {
            var timer = new System.Timers.Timer(mstime);
            timer.Elapsed += (sender, args) => function(startAddress);
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        //this method needs revision, because it works only M, D and T registers
        //(see Docs/AH-EMC_Modbus_Addresses.pdf)
        private ushort GetAddressIntValue(string startAddress)
        {
            int intValue = 0;
            string mark = startAddress.Substring(0, 1);
            string address = startAddress.Substring(1);

            switch (mark)
            {
                case "M":
                    intValue = int.Parse(address) + 2048;
                    break;
                case "D":
                    intValue = int.Parse(address) + 0;
                    break;
                case "T":
                    intValue = int.Parse(address) + 57344;
                    break;
                case "C":
                    intValue = int.Parse(address) + 3584;
                    break;
            }

            string hex = intValue.ToString("X4");
            return ushort.Parse(hex, NumberStyles.HexNumber);
        }

        public void DisConnect ()
        {
            modbusTcpNet.ConnectClose();
            modbusTcpNet.Dispose();

        }

    }
}

/*****************************************************
The MIT License (MIT)
Copyright (c) 2006 Scott Alexander, 2015 Dmitry Turin
see Docs/LICENSE.txt
******************************************************/

using NModbus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GeLiService_WMS
{
	public class ModbusTCPDeltaHelper
    {
		private  IModbusMaster master;
		private bool isClientConnected;
		private bool isClientConnected_old;

		public Action<bool> SetConnectionStatus;

		public string Ip { get; }

		public int Port { get; }

		/// <summary>
		/// If connection doesn't loss returns true
		/// </summary>
		public bool IsClientConnected
		{
			get { return isClientConnected; }
			set
			{
				isClientConnected = value;
				if (isClientConnected != isClientConnected_old)
				{
					SetConnectionStatus?.Invoke(isClientConnected);
					isClientConnected_old = isClientConnected;
				}
			}
		}

		/// <summary>
		/// Creates ModbusTCP connection
		/// </summary>
		/// <param name="ip">controller ip address</param>
		/// <param name="port">connection port, default value 502</param>
		/// <param name="connectionUpdateTime">time in milliseconds used as connection check period</param>
		public ModbusTCPDeltaHelper(string ip, int port, int connectionUpdateTime)
		{
			Ip = ip;
			Port = port;

			//create new connection
			IsClientConnected = false;
			var conn = CreateConnection();

			//timer to read some register and check connection state
			var timer = new System.Timers.Timer(connectionUpdateTime);
			timer.Elapsed += (sender, args) => ConnectionCommander("M0");
			timer.AutoReset = true;
			timer.Enabled = true;
			master.Transport.ReadTimeout = 500;
			isClientConnected = conn;
		}
		/// <summary>
		/// 读取线圈状态，返回空或者结果
		/// </summary>
		/// <param name="startAddress"></param>
		/// <returns></returns>
		public bool? ReadBoolean(string startAddress)
		{
			ushort formatStartAddress = GetAddressIntValue(startAddress);

			

			try
			{
				bool[] coils = master.ReadCoils(0, formatStartAddress, 1);
			var	result = coils[0];
				return result;
			}
			catch
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
        public List<bool> ReadManyBoolean(string startAddress,ushort num)
        {
            ushort formatStartAddress = GetAddressIntValue(startAddress);

            

            try
            {
                bool[] coils = master.ReadCoils(0, formatStartAddress, num);
                var result = coils.ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }


        public async Task<string> ReadBooleanAsync(string startAddress)
		{
			ushort formatStartAddress = GetAddressIntValue(startAddress);

			string result = "";

			try
			{
				bool[] coils = await master.ReadCoilsAsync(0,formatStartAddress, 1);
				result = coils[0].ToString();
				return result;
			}
			catch
			{
				return result;
			}
		}

		public string ReadDecimal(string startAddress)
		{
			ushort formatStartAddress = GetAddressIntValue(startAddress);

			string hex;
			try
			{
				var registers = master.ReadHoldingRegisters(1, formatStartAddress, 1);
				hex = registers[0].ToString("X4");
				return GetSignInt(hex);
			}
			catch
			{
				return "";
			}
		}

		public async Task<string> ReadDecimalAsync(string startAddress)
		{
			ushort formatStartAddress = GetAddressIntValue(startAddress);

			string hex;
			try
			{
				var registers = await master.ReadHoldingRegistersAsync(1, formatStartAddress, 1);
				hex = registers[0].ToString("X4");
				return GetSignInt(hex);
			}
			catch
			{
				return "";
			}
		}

		public void WriteBoolean(string startAddress, bool value)
		{
			ushort formatStartAddress = GetAddressIntValue(startAddress);
			try
			{
				master.WriteSingleCoil(0,formatStartAddress, value);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		public void WriteDecimal(string startAddress, int value)
		{
			ushort formatStartAddress = GetAddressIntValue(startAddress);

			ushort[] data = { 0 };
			data[0] = GetHexFromInt(value);
			try
			{
				master.WriteMultipleRegisters(1, formatStartAddress, data);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
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

		private bool CreateConnection()
		{
            var factory = new ModbusFactory();
            TcpClient Client = new TcpClient();
			var result = Client.BeginConnect(Ip, Port, null, null);
			master = factory.CreateMaster(Client);

			return result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
		}

		private async void ConnectionCommander(string address)
		{
			var connVar = await ReadBooleanAsync(address);
			if (connVar.Length > 0)
			{
				IsClientConnected = true;
			}
			else
			{
				IsClientConnected = false;
				var connectionSuccess = CreateConnection();
				if (!connectionSuccess)
				{
					Console.WriteLine("Connection failed");
				}
			}
		}
	}
}

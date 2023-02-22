using AsyncTCPClient;
using HslCommunication;
using HslCommunication.Profinet.Siemens;
using Microsoft.Extensions.Hosting;
using NanXingData_WMS.Dao;
using NanXingService_WMS.Services;
using NanXingService_WMS.Services.APS;
using NanXingService_WMS.Utils.TishengjiUtils;
using NX_WorkshopData.TCPClient;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Geom;

namespace NanXingService_WMS.Threads.DAQ
{
    /// <summary>
    /// 数据采集：
    /// 获取计数器数量，对production 表进行数据插入
    /// </summary>
    public class ProductionThread
    {
        LiuShuiHaoService _liuShuiHaoService = new LiuShuiHaoService();
        ProductionService _productionService = new ProductionService();

        public void Control2L()
        {
            OpenTcp("192.168.10.210", 8003);
        }
        public void Control3L()
        {
            if (S200Smart_siemensTcpNet == null 
                || connect_S200 == null || !connect_S200.IsSuccess)
            {
                ConnectPLC("192.168.0.61", 102);
                //ConnectPLC("127.0.0.1", 102);
            }
            else
            {
                ParseMsg();
            }
        }

        #region 2L计数器采集

        AsyncClient ClientTcp;
        public void ParseMsg2L(string strMsg)
        {
            Production production = new Production();
            production.prosn=_liuShuiHaoService.GetProsn();
            production.prodate=DateTime.Now;
            production.proname="test";
            production.itemno="test";
            production.position="2L大包装车间";
            _productionService.Insert(production);
            _productionService.SaveChanges();


        }
        #region 打开关闭TCP连接
        public AsyncClient OpenTcp(string ip, int port)
        {
            ClientTcp = new AsyncClient(port, ip,
                 MsgException, MsgConnected,
                 MsgDisconnected, MsgReceived);

            return ClientTcp;
        }
       
        /// <summary>
        /// 客户端异常事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgException(object sender, TcpServerExceptionOccurredEventArgs e)
        {
            Logger.Default.Process(new Log("Error", $"TCP server {e.ToString()}exception occurred, {e.Exception.Message}."));

            //MessageBox.Show($"TCP server { e.ToString()}exception occurred, { e.Exception.Message}.");
        }
        /// <summary>
        /// 连接事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgConnected(object sender, TcpServerConnectedEventArgs e)
        {
            Logger.Default.Process(new Log(LevelType.Info, $"{e.Address}TCP server Online"));
            //f1.ChangeStatus(3, "已连接");
            //WriteToTcp(TiShengOrder.Init);
            //Thread.Sleep(100);
            //WriteToTcp(TiShengOrder.Check);
        }

        void NetworkResponseClientTcpOnFaildConnect()
        {
            Debug.WriteLine("失败");
            //f1.ChangeStatus(3, "连接失败");
            //WriteToTcp(TiShengOrder.Init);
            //Thread.Sleep(100);
            //WriteToTcp(TiShengOrder.Check);
        }
        private void MsgDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            Logger.Default.Process(new Log(LevelType.Info, $"{e.Address}TCP server DisConnected"));

            //f1.ChangeStatus(3, "连接断开");
            //WriteToTcp(TiShengOrder.Init);
            //Thread.Sleep(100);
            //WriteToTcp(TiShengOrder.Check);
        }

        /// <summary>
        /// 传感器客户端接收消息事件二楼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            //IPEndPoint iPEndPoint = (IPEndPoint)e.TcpClient.Client.RemoteEndPoint;
            lock (e.TcpClient.Client.RemoteEndPoint)
            {
                IPEndPoint iPEndPoint = (IPEndPoint)e.TcpClient.Client.RemoteEndPoint;
                try
                {
                    
                    string str = Encoding.Default.GetString(e.Datagram);
                    //Debug.WriteLine($"{DateTime.Now.ToString("G")}收到:{str}");
                    Logger.Default.Process(new Log(LevelType.Info, $"{DateTime.Now.ToString("G")}{iPEndPoint.Address}:{iPEndPoint.Port}收到:\r\n{str}"));
                    ParseMsg2L(str);

                }
                catch(DbEntityValidationException ex)
                {
                    StringBuilder stringBuilder = new StringBuilder(); 
                    foreach(var exTemp in ex.EntityValidationErrors)
                    {
                        stringBuilder.AppendLine(string.Join(",",exTemp.ValidationErrors.SelectMany(u=>u.ErrorMessage)));
                    }

                    Logger.Default.Process(new Log(LevelType.Error, $"{iPEndPoint.Address}{iPEndPoint.Port}\r\n{stringBuilder.ToString()}"));
                }
                catch(Exception ex)
                {
                    Logger.Default.Process(new Log(LevelType.Error, $"{iPEndPoint.Address}{iPEndPoint.Port}\r\n{ex}"));
                }
            }
        }

        public void CloseTcp()
        {
            if (ClientTcp.client != null && ClientTcp.client.Connected)
            {
                ClientTcp.client.Close();
                ClientTcp.client.Dispose();
                ClientTcp.client = null;
                ClientTcp = null;
            }
        }

        #endregion

        #region 读写指令

        public void WriteToTcp(string msg)
        {
            ClientTcp.client.Send(msg);
            Debug.WriteLine($"{DateTime.Now.ToString("G")}发送:{msg}");
        }

        void ReceiveTcpMsg(string ip, string strMsg)
        {
            Debug.WriteLine($"{DateTime.Now.ToString("G")}收到:{strMsg}");
            ParseMsg2L( strMsg);
        }

        #endregion


        #endregion 2L计数器采集



        #region 3L计数器采集
        #region 通信连接
        /// <summary>
        /// 200PLC连接的
        /// </summary>
        SiemensS7Net S200Smart_siemensTcpNet = null;

        /// <summary>
        /// 200PLC连接的状态
        /// </summary>
        OperateResult connect_S200 = null;
        /// <summary>
        /// 连接S7-200 SmartPLC
        /// </summary>
        /// <param name="pi"></param>
        private void ConnectPLC(string ip,int port)
        {
            if (S200Smart_siemensTcpNet != null)
            {
                S200Smart_siemensTcpNet.ConnectClose();
                S200Smart_siemensTcpNet = null;
            }
            //读取PLC信息
            S200Smart_siemensTcpNet = new SiemensS7Net(SiemensPLCS.S200Smart);
            S200Smart_siemensTcpNet.IpAddress = ip;
            S200Smart_siemensTcpNet.Port = port;
            connect_S200 = S200Smart_siemensTcpNet.ConnectServer();
            Logger.Default.Process(new Log(LevelType.Info,
                $"PLC连接 {connect_S200.IsSuccess}:{connect_S200.Message}"));
            
        }

        public void CloseConnect()
        {
            if (S200Smart_siemensTcpNet != null)
            {
                S200Smart_siemensTcpNet?.ConnectClose();
                S200Smart_siemensTcpNet = null;
            }
        }

        #endregion

        #region 数据解析

        int passCounter = 0;
        bool[] isPassingArr = new bool[8];
        int[] countArray = new int[8];
        public void ParseMsg()
        {
            try
            {
                short[] values=ReadShort("C0", 1);
                Logger.Default.Process(new Log(LevelType.Info,
                    $"获取C0:{values[0]}"));
                if(values[0]>=30000)
                {
                    S200Smart_siemensTcpNet.Write("Q1.1", true);
                    Thread.Sleep(100);
                    S200Smart_siemensTcpNet.Write("Q1.1", false);
                }

                Production production = new Production();
                production.prosn=_liuShuiHaoService.GetProsn();
                production.prodate=DateTime.Now;
                production.proname="test";
                production.itemno="test";
                production.position="3L大包装车间";
                _productionService.Insert(production);
                _productionService.SaveChanges();

            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var exTemp in ex.EntityValidationErrors)
                {
                    stringBuilder.AppendLine(string.Join(",", exTemp.ValidationErrors.SelectMany(u => u.ErrorMessage)));
                }

                Logger.Default.Process(new Log(LevelType.Error, $"PLC 3L大包装报错\r\n{stringBuilder.ToString()}"));
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    $"{DateTime.Now.ToString("G")}发生错误:{ex}"));
            }
        }

        #endregion

        #region S7读取转换
        /// <summary>
        /// 批量读取PLC数据
        /// </summary>
        /// <param name="readWrite"></param>
        /// <param name="addStart">起始地址</param>
        /// <param name="lengthStr">读取长度</param>
        /// <returns>返回字节格式的字符串</returns>
        public string ReadBatch(HslCommunication.Core.IReadWriteNet siemensTcpNet, string addStart, string lengthStr)
        {
            try
            {
                OperateResult<byte[]> read = siemensTcpNet.Read(addStart, ushort.Parse(lengthStr));
                if (read.IsSuccess)
                {
                    return HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content);
                }
                else
                {
                    //MessageBox.Show("Read Failed：" + read.ToMessageShowString());
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Read Failed：" + ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// 字节转二进制并获取对应点位是否为1
        /// </summary>
        /// <param name="value">16进制字节字符串</param>
        /// <param name="index">点位</param>
        /// <returns>二进制字符串</returns>
        private bool GetBitBool(string value, int index)
        {
            char line = Convert.ToString(Convert.ToInt32(value, 16), 2).PadLeft(8, '0')[7 - index];
            return line == '1' ? true : false;
        }

        private short[] ReadShort(string address,int readCount)
        {
            if (readCount%2>0)
                readCount+=1;

            //string result_States = ReadBatch(S200Smart_siemensTcpNet, "C0", "2");
            string result_States = ReadBatch(S200Smart_siemensTcpNet, address, readCount.ToString());

            Debug.WriteLine(result_States);
            byte[] v1Arr = new byte[result_States.Length / 2];
            for (int i = 0; i < result_States.Length / 2; i++)
            {
                v1Arr[i] = Convert.ToByte(result_States.Substring(i * 2, 2), 16);
            }
            int count = v1Arr.Length >> 1;
            short[] si = new short[readCount];
            for (int i = 0; i < readCount; i++)
            {
                si[i] = (short)(v1Arr[i * 2] << 8 | v1Arr[2 * i + 1] & 0xff);
            }

            return si;

        }

        #endregion
        #endregion
    }
}

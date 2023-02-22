using NanXingData_WMS.Dao;
using NX_WorkshopData.TCPClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using AsyncTCPClient;
using System.Net;
using NanXingService_WMS.Services.WMS.AGV;
using NanXingService_WMS.Entity.TiShengJiEntity;
using NanXingService_WMS.Utils.RedisUtils;
using System.Net.Sockets;

namespace NanXingService_WMS.Utils.TishengjiUtils
{
    public class TiShengJiHelper
    {
        /// <summary>
        /// 网络心跳包，用于客户端与服务端通讯，客戶端連接對象
        /// </summary>
        //public SocketClientManager ClientTcp = null;
        RedisHelper stringCacheRedisHelper = new RedisHelper();
        TiShengJiStateService tsjService = new TiShengJiStateService();

        /// <summary>
        /// 传感器客户端
        /// </summary>
        public AsyncClient ClientTcp;
        //Form1 f1 = null;

        public TiShengJiState state = null;
        string tsjName;

        //TiShengJiService tsjService;
        public TiShengJiHelper(TiShengJiDevice tsjDevice)
        {
            tsjName=tsjDevice.tsj_Name;
            //this.tsjService = tsjService;
            OpenTcp(tsjDevice.tsj_IP, tsjDevice.tsj_Port);
        }

        public TiShengJiHelper(TiShengJiInfo tsjDevice)
        {
            //this.tsjService = tsjService;
            OpenTcp(tsjDevice.TsjIp, tsjDevice.TsjPort);
            

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
            Logger.Default.Process(new Log("Error", $"TCP server { e.ToString()}exception occurred, { e.Exception.Message}."));

            //MessageBox.Show($"TCP server { e.ToString()}exception occurred, { e.Exception.Message}.");
        }
        /// <summary>
        /// 连接事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgConnected(object sender, TcpServerConnectedEventArgs e)
        {
            Debug.WriteLine("Online");
            //f1.ChangeStatus(3, "已连接");
            WriteToTcp(TiShengOrder.Init);
            Thread.Sleep(100);
            WriteToTcp(TiShengOrder.Check);
        }

        void NetworkResponseClientTcpOnFaildConnect()
        {
            Debug.WriteLine("失败");
            //f1.ChangeStatus(3, "连接失败");
            WriteToTcp(TiShengOrder.Init);
            Thread.Sleep(100);
            WriteToTcp(TiShengOrder.Check);
        }
        private void MsgDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            Debug.WriteLine("断开");
            //f1.ChangeStatus(3, "连接断开");
            WriteToTcp(TiShengOrder.Init);
            Thread.Sleep(100);
            WriteToTcp(TiShengOrder.Check);
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
                string str = System.Text.Encoding.Default.GetString(e.Datagram);
                Debug.WriteLine($"{DateTime.Now.ToString("G")}收到:{str}");

                ParseMsg(iPEndPoint.Address.ToString() + "_" + iPEndPoint.Port.ToString()
                , str);
            }
        }

        public void CloseTcp()
        {
            if (ClientTcp.client != null && ClientTcp.client.Connected)
            {
                ClientTcp.client.Close();
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
            ParseMsg(ip, strMsg);
        }

        #endregion

        #region 数据解析
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        private void ParseMsg(string ip, string msg)
        {
            msg = msg.Trim();
            if (msg.Length == 40)
            {
                try
                {
                    string[] msgArr = new string[msg.Length / 2];
                    for (int i = 0; i < msgArr.Length; i++)
                    {
                        msgArr[i] = msg.Substring(2 * i, 2);
                    }

                    TiShengJiState tsjs = new TiShengJiState();

                    tsjs.TsjIp = ip;
                    tsjs.InputTime = DateTime.Now;
                    //1、获取是否故障
                    if (msgArr[3] == TiShengReceive.State00)
                        tsjs.deviceState = DeviceState.Normal;
                    else if (msgArr[3] == TiShengReceive.State01)
                        tsjs.deviceState = DeviceState.Warning;
                    else if (msgArr[3] == TiShengReceive.State02)
                        tsjs.deviceState = DeviceState.Stop;
                    else if (msgArr[3] == TiShengReceive.State03)
                        tsjs.deviceState = DeviceState.Pause;

                    //2、获取升降小车状态
                    if (msgArr[6] == TiShengReceive.State00)
                    {
                        tsjs.carState = TiShengCarState.NoJob;
                        tsjs.carTarget = string.Empty;
                    }
                    else if (msgArr[6] == TiShengReceive.State01)
                    {
                        tsjs.carState = TiShengCarState.Running;
                        if (int.Parse(msgArr[9]) > 0)
                            tsjs.carTarget = msgArr[9].Substring(1, 1) + "L";
                        else if (int.Parse(msgArr[12]) > 0)
                            tsjs.carTarget = msgArr[12].Substring(1, 1) + "L";
                        else if (int.Parse(msgArr[15]) > 0)
                            tsjs.carTarget = msgArr[15].Substring(1, 1) + "L";
                    }

                    //3、有多少的货物在
                    tsjs.CarCount = int.Parse(msgArr[5]);
                    tsjs.F1Count = int.Parse(msgArr[8]);
                    tsjs.F2Count = int.Parse(msgArr[11]);
                    tsjs.F3Count = int.Parse(msgArr[14]);

                    if (msgArr[4] == TiShengReceive.State00)
                        tsjs.CarState2 = "无料箱";
                    else if (msgArr[4] == TiShengReceive.State01)
                        tsjs.CarState2 = "进货中";
                    else if (msgArr[4] == TiShengReceive.State02)
                        tsjs.CarState2 = "出货中";
                    else if (msgArr[4] == TiShengReceive.State03)
                        tsjs.CarState2 = "料箱到位";

                    tsjs.F1State = ParseGunTongState(msgArr, 7);
                    tsjs.F2State = ParseGunTongState(msgArr, 10);
                    tsjs.F3State = ParseGunTongState(msgArr, 13);
                    tsjs.F1DuiJieWei = ParseDuiJieDianState(msgArr, 16);
                    tsjs.F2DuiJieWei = ParseDuiJieDianState(msgArr, 17);
                    tsjs.F3DuiJieWei = ParseDuiJieDianState(msgArr, 18);

                    tsjs.OrderReceive = msgArr[19] == "01" ? "接收到数据" : "没有接收到";

                    if(!IsEqual(state, tsjs))
                        tsjService.AddState(tsjs);


                    if(tsjs.deviceState == DeviceState.Warning)
                        stringCacheRedisHelper.StringSet($"IsFloorTaskStop:{tsjName}", 1);
                    stringCacheRedisHelper.StringSet($"TSJ-State:{tsjs.TsjIp}", tsjs,DateTime.Now.AddSeconds(5));
                    state = tsjs;
                    //if (Form1.tsjs1 == null || !IsEqual(Form1.tsjs1, tsjs))
                    //{
                    //    DB4.TiShengJiState.Add(tsjs);
                    //    DB4.SaveChanges();
                    //}

                    //f1.ChangeStatus(2, string.Empty, tsjs);
                }
                catch (Exception ex)
                {
                    Logger.Default.Process(new Log("Info", msg));
                    Logger.Default.Process(new Log("Info", ex.ToString()));

                    //throw ex;
                    WriteToTcp(TiShengOrder.Init);
                    Thread.Sleep(100);
                    WriteToTcp(TiShengOrder.Check);
                }

            }
        }

        string[] timeName = { "ID", "InputTime" };
        private bool IsEqual<T>(T ts1, T ts2)
        {
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(typeof(T)))
            {
                if (!(pd.GetValue(ts1) ?? string.Empty).Equals(pd.GetValue(ts2) ?? string.Empty)
                    && !timeName.Contains(pd.Name))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 解析滚筒上货物的状态
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string ParseGunTongState(string[] arr, int index)
        {
            string msg = string.Empty;
            if (arr[index] == TiShengReceive.State00)
                msg = GunZhouState.NoBox;
            else if (arr[index] == TiShengReceive.State01)
                msg = GunZhouState.InBox;
            else if (arr[index] == TiShengReceive.State02)
                msg = GunZhouState.OutBox;
            else if (arr[index] == TiShengReceive.State03)
                msg = GunZhouState.PutBoxOK;
            else if (arr[index] == TiShengReceive.State04)
                msg = GunZhouState.PushBoxOK;
            return msg;
        }


        private string ParseDuiJieDianState(string[] arr, int index)
        {
            string msg = string.Empty;
            if (arr[index] == TiShengReceive.State00)
                msg = GunZhouState.NoBox;
            else if (arr[index] == TiShengReceive.State01)
                msg = GunZhouState.HasBox;
            return msg;
        }


        string F1 = "1";
        string F2 = "2";
        string F3 = "3";
        /// <summary>
        /// 发送提升机命令
        /// </summary>
        /// <param name="startPo">仓位起始点</param>
        /// <param name="endPo">仓位结束点</param>
        /// <param name="count">数量</param>
        public void SendTSJOrder(string startPo, string endPo, int count)
        {
            Logger.Default.Process(new Log("Info", "发送提升机指令"));
            string sendMsg = string.Empty;
            if (count == 1)
            {
                if (startPo.StartsWith(F1) && endPo.StartsWith(F2))
                    sendMsg = TiShengOrder.F1T2On1;
                else if (startPo.StartsWith(F1) && endPo.StartsWith(F3))
                    sendMsg = TiShengOrder.F1T3On1;
                else if (startPo.StartsWith(F2) && endPo.StartsWith(F1))
                    sendMsg = TiShengOrder.F2T1On1;
                else if (startPo.StartsWith(F2) && endPo.StartsWith(F3))
                    sendMsg = TiShengOrder.F2T3On1;
                else if (startPo.StartsWith(F3) && endPo.StartsWith(F1))
                    sendMsg = TiShengOrder.F3T1On1;
                else if (startPo.StartsWith(F3) && endPo.StartsWith(F2))
                    sendMsg = TiShengOrder.F3T2On1;

            }
            else if (count == 2)
            {
                if (startPo.StartsWith(F1) && endPo.StartsWith(F2))
                    sendMsg = TiShengOrder.F1T2On2;
                else if (startPo.StartsWith(F1) && endPo.StartsWith(F3))
                    sendMsg = TiShengOrder.F1T3On2;
                else if (startPo.StartsWith(F2) && endPo.StartsWith(F1))
                    sendMsg = TiShengOrder.F2T1On2;
                else if (startPo.StartsWith(F2) && endPo.StartsWith(F3))
                    sendMsg = TiShengOrder.F2T3On2;
                else if (startPo.StartsWith(F3) && endPo.StartsWith(F1))
                    sendMsg = TiShengOrder.F3T1On2;
                else if (startPo.StartsWith(F3) && endPo.StartsWith(F2))
                    sendMsg = TiShengOrder.F3T2On2;
            }
            if(sendMsg !=string.Empty)
                WriteToTcp(sendMsg);

            Logger.Default.Process(new Log(LevelType.Info, $"发送提升机命令:{sendMsg}"));
            int index = 0;
            while(true)
            {
                Thread.Sleep(200);
                if (state.deviceState != DeviceState.Normal)
                    break;
                else if (state.carState== TiShengCarState.Running)
                    break;
                index++;
                if (index == 25)
                {
                    Logger.Default.Process(new Log(LevelType.Warn,
                       $"{tsjName}提升机发送{startPo}-{endPo}搬货任务重试-第一次"));
                    WriteToTcp(TiShengOrder.Init);
                    Thread.Sleep(100);
                    WriteToTcp(sendMsg);
                   
                }
                if (index == 50)
                {
                    Logger.Default.Process(new Log(LevelType.Warn,
                       $"{tsjName}提升机发送{startPo}-{endPo}搬货任务重试-第二次"));
                    WriteToTcp(TiShengOrder.Init);
                    Thread.Sleep(100);
                    WriteToTcp(sendMsg);
                }
                if (index > 75)
                {
                    Logger.Default.Process(new Log(LevelType.Warn, 
                        $"{tsjName}提升机没有响应搬货任务"));
                    break;
                }
            }

            WriteToTcp(TiShengOrder.Init);
            Thread.Sleep(100);
            WriteToTcp(TiShengOrder.Check);


        }
        #endregion

    }

    public class TiShengCarState
    {
        public static string NoJob = "空闲中";
        public static string Running = "任务中";
    }
    public class DeviceState
    {
        public static string Normal = "正常";
        public static string Warning = "故障";
        public static string Stop = "急停";
        public static string Pause = "暂停";

    }

    /// <summary>
    /// 提升机状态变量
    /// </summary>
    public class TiShengReceive
    {
        public static string State00 = "00";
        public static string State01 = "01";
        public static string State02 = "02";
        public static string State03 = "03";
        public static string State04 = "04";
        public static string State05 = "05";

    }
    /// <summary>
    /// 提升机命令变量
    /// </summary>
    public class TiShengOrder
    {
        public static string Init = "0000000000000000";
        public static string Check = "0100000000000000";


        public static string F1T2On2 = "0102020000000001";
        public static string F1T3On2 = "0103020000000001";
        public static string F2T1On2 = "0100000102000001";
        public static string F2T3On2 = "0100000302000001";
        public static string F3T1On2 = "0100000000010201";
        public static string F3T2On2 = "0100000000020201";

        public static string F1T2On1 = "0102010000000001";
        public static string F1T3On1 = "0103010000000001";
        public static string F2T1On1 = "0100000101000001";
        public static string F2T3On1 = "0100000301000001";
        public static string F3T1On1 = "0100000000010101";
        public static string F3T2On1 = "0100000000020101";
    }

    /// <summary>
    /// 滚筒状态变量
    /// </summary>
    public class GunZhouState
    {
        public static string HasBox = "有料箱";
        public static string NoBox = "无料箱";
        public static string InBox = "进货中";
        public static string OutBox = "出货中";
        public static string PutBoxOK = "叉车放料箱到位";
        public static string PushBoxOK = "升降机出料箱到位";
    }
}

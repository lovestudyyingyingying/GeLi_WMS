using AsyncTCPClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NX_WorkshopData.TCPClient
{
    /// <summary>
    /// 异步TCP客户端类
    /// </summary>
    public class AsyncClient
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        private IPEndPoint remoteEP { get; set; }

        /// <summary>
        /// 客户端对象
        /// </summary>
        public AsyncTcpClient client { get; set; }

        /// <summary>
        /// 异常事件回调函数
        /// </summary>
        private Action<object, TcpServerExceptionOccurredEventArgs> client_ServerExceptionOccurred;

        /// <summary>
        /// 连接事件回调函数
        /// </summary>
        private Action<object, TcpServerConnectedEventArgs> client_ServerConnected;

        /// <summary>
        /// 断开连接事件回调函数
        /// </summary>
        private Action<object, TcpServerDisconnectedEventArgs> client_ServerDisconnected;

        /// <summary>
        /// 接收消息回调函数
        /// </summary>
        private Action<object, TcpDatagramReceivedEventArgs<byte[]>> client_DatagramReceived;

        /// <summary>
        /// 异步TCP客户端构造函数
        /// </summary>
        /// <param name="port">服务器端口号</param>
        /// <param name="serverExceptionOccurred">异常回调函数</param>
        /// <param name="serverConnected">连接回调函数</param>
        /// <param name="serverDisconnected">断开连接回调函数</param>
        /// <param name="datagramReceived">接收消息回调函数</param>
        public AsyncClient(
            int port,
            string IP,
            Action<object, TcpServerExceptionOccurredEventArgs> serverExceptionOccurred,
            Action<object, TcpServerConnectedEventArgs> serverConnected,
            Action<object, TcpServerDisconnectedEventArgs> serverDisconnected,
            Action<object, TcpDatagramReceivedEventArgs<byte[]>> datagramReceived)
        {
            //this.remoteEP = new IPEndPoint(IPAddress.Parse("192.168.10.210"), port);
            this.remoteEP = new IPEndPoint(IPAddress.Parse(IP), port);
            this.client = new AsyncTcpClient(IPAddress.Parse(IP), port);
            this.client_ServerExceptionOccurred = serverExceptionOccurred;
            this.client_ServerConnected = serverConnected;
            this.client_ServerDisconnected = serverDisconnected;
            this.client_DatagramReceived = datagramReceived;

            Init();
        }

        /// <summary>
        /// 初始化客户端
        /// 为各个事件注册回调函数
        /// </summary>
        private void Init()
        {
            client.Encoding = Encoding.UTF8;
            client.ServerExceptionOccurred +=
              new EventHandler<TcpServerExceptionOccurredEventArgs>(client_ServerExceptionOccurred);
            client.ServerConnected +=
              new EventHandler<TcpServerConnectedEventArgs>(client_ServerConnected);
            client.ServerDisconnected +=
              new EventHandler<TcpServerDisconnectedEventArgs>(client_ServerDisconnected);
            client.DatagramReceived +=
              new EventHandler<TcpDatagramReceivedEventArgs<byte[]>>(client_DatagramReceived);
            client.Connect();
        }
    }
}

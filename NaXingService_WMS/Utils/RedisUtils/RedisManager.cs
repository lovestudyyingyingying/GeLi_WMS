using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.RedisUtils
{
    public class RedisManager
    {
        private static string Constring = RedisConfig.Config();
        private static readonly object locker = new object();
        private static ConnectionMultiplexer instance;
        private static readonly Dictionary<string, ConnectionMultiplexer> Concache = new Dictionary<string, ConnectionMultiplexer>();

        /// <summary>
        /// 单例模式获取redis连接实例
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = GetManager();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 从缓存中获取
        /// </summary>
        /// <param name="constr"></param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConForMap(string constr)
        {
            if (!Concache.ContainsKey(constr))
                Concache[constr] = GetManager(constr);
            return Concache[constr];
        }

        private static ConnectionMultiplexer GetManager(string constr = null)
        {
            constr = constr ?? Constring;
            var connect = ConnectionMultiplexer.Connect(constr);

            #region 注册事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;
            #endregion

            return connect;
        }
        #region Redis事件
        /// <summary>
        /// 内部异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Logger.Default.Process(new Log(LevelType.Error, "内部异常：" + e.Exception.Message));
        }

        /// <summary>
        /// 集群更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Logger.Default.Process(new Log(LevelType.Info, "新集群：" + e.NewEndPoint + "旧集群：" + e.OldEndPoint));
        }

        /// <summary>
        /// 配置更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Logger.Default.Process(new Log(LevelType.Info,"配置更改：" + e.EndPoint));
        }

        /// <summary>
        /// 错误事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Logger.Default.Process(new Log(LevelType.Error, "异常信息：" + e.Message));
        }

        /// <summary>
        /// 重连错误事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Logger.Default.Process(new Log(LevelType.Error, "重连错误" + e.EndPoint));
        }

        /// <summary>
        /// 连接失败事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Logger.Default.Process(new Log(LevelType.Error, 
                "连接异常" + e.EndPoint + "，类型为" + e.FailureType + (e.Exception == null ?string.Empty 
                : ("，异常信息是" + e.Exception.Message))));
        }
        #endregion
    }
}

using EasyNetQ;
using NanXingService_WMS.Utils.ThreadUtils;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.RabbitMQ
{
    public class RabbitMQUtils
    {
        static ConnectionFactory _connectionFactory;
        static IConnection _connection;
        static Dictionary<string, IModel> sendModelCollection = new Dictionary<string, IModel>();
        static Dictionary<string, IModel> receviceModelCollection = new Dictionary<string, IModel>();
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object Locker = new object();
        //public static RabbitBus bus;

        static RabbitMQUtils()
        {
            CreateConnection();
        }

        
        private static IConnection CreateConnection()
        {
            if (_connection != null)
            {
                return _connection;
            }
            lock (Locker)
            {
                if (_connectionFactory == null)
                {
                    string HostName = ConfigurationManager.AppSettings["RabbitMQ-Server"];
                    string UserName = ConfigurationManager.AppSettings["RabbitMQ-UserName"];
                    string Password = ConfigurationManager.AppSettings["RabbitMQ-Password"];

                    _connectionFactory = new ConnectionFactory();
                    _connectionFactory.HostName = HostName;
                    _connectionFactory.UserName = UserName;
                    _connectionFactory.Password = Password;
                }
                _connection = _connectionFactory.CreateConnection();
                return _connection;
            }
        }

        private static IModel GetSendChannel(string queueName)
        {

            if (sendModelCollection.ContainsKey(queueName))
            {
                var channel = sendModelCollection[queueName];
                if (channel.IsOpen)
                    return channel;
            }
            lock (Locker)
            {
                if (sendModelCollection.ContainsKey(queueName))
                {
                    if (sendModelCollection[queueName].IsOpen)
                        return sendModelCollection[queueName];
                }
                var channel = CreateWorkQueue(queueName);
                sendModelCollection.Add(queueName, channel);
                return channel;
            }
        }



        private static IModel CreateWorkQueue(string queueName)
        {
            var channel = _connection.CreateModel();
            channel.BasicNacks += (sender, e) =>
            {
                //生产者发送消息到broker（服务器）后失败被生产者的listener监听到，就走无应答方法
                Logger.Default.Process(new Log(LevelType.Error, 
                    $" --没有成功发送到broker服务器{queueName}-- "));
            };
            channel.BasicAcks += (sender, e) =>
            {
                //有应答
                Logger.Default.Process(new Log(LevelType.Info,
                    $" --成功发送到broker服务器{queueName}-- "));
            };
            bool durable = true;//队列持久化
            channel.QueueDeclare(queueName, durable, false, false, null);
            // 4 指定我们的消息投递模式: 消息的确认模式
            channel.ConfirmSelect();
            return channel;
        }

        public static void Send<T>(string queueName, T obj)
        {
            var channel = GetSendChannel( queueName);
            string message = JsonConvert.SerializeObject(obj);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;//消息持久化

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(string.Empty, queueName, properties, body);
            Console.WriteLine(" set {0}", message);

          
        }
        public static void Send<T>(string queueName, List<T> obj)
        {
            var channel = GetSendChannel(queueName);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            foreach (T temp in obj)
            {
                string message = JsonConvert.SerializeObject(temp);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(string.Empty, queueName, properties, body);
                Console.WriteLine(" set {0}", message);
            }
            
        }

        public IModel GetReceviceChannel(string queueName)
        {

            if (receviceModelCollection.ContainsKey(queueName))
            {
                var channel = receviceModelCollection[queueName];
                if (channel.IsOpen)
                    return channel;
            }
            lock (Locker)
            {
                if (receviceModelCollection.ContainsKey(queueName))
                {
                    if (receviceModelCollection[queueName].IsOpen)
                        return sendModelCollection[queueName];
                }
                var channel = CreateWorkQueue(queueName);
                channel.BasicQos(0, 1, false);
                receviceModelCollection.Add(queueName, channel);
                return channel;
            }
        }


        public MyTask Recevice<T>(string queueName, Action<T> action)
        {
            return new MyTask(() => { Thread.Sleep(5000); }
            , 10, true, () =>
            { 
                var channel = GetReceviceChannel(queueName);
                channel.QueueDeclare(queueName, true, false, false, null);
                //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
                channel.BasicQos(0, 1, false);

                //在队列上定义一个消费者
                var consumer = new EventingBasicConsumer(channel);
                //var consumer = new query(channel);


                consumer.Received += (model, ea) =>
                {
                    var msgBody = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Logger.Default.Process(new Log(LevelType.Info, string.Format("***接收时间:{0}，消息内容：{1}",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msgBody)));

                    Debug.WriteLine(" [x] Done");
                    try
                    {
                        T obj = JsonConvert.DeserializeObject<T>(msgBody);
                        action.Invoke(obj);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        Logger.Default.Process(new Log(LevelType.Info,
                            $"{queueName}队列消息处理成功:\r\n数据内容:{msgBody}"));
                    }
                    catch (Exception ex)
                    {
                        Logger.Default.Process(new Log(LevelType.Error, 
                            $"{queueName}队列消息处理失败:\r\n数据内容:{msgBody}\r\n错误信息:{ex.ToString()}" ));
                    }
                };
                channel.BasicConsume(queueName, autoAck: false, consumer);


            }, () => {
                if (receviceModelCollection.ContainsKey(queueName))
                {
                    receviceModelCollection[queueName].Close();
                    receviceModelCollection.Remove(queueName);  
                }
            }).StartTask();
        }

        public void CloseRabbitMQ()
        {
            foreach(var temp in sendModelCollection.Values)
            {
                temp.Close();
            }
            foreach (var temp in receviceModelCollection.Values)
            {
                temp.Close();
            }
            _connection.Close();
        }

    }
}

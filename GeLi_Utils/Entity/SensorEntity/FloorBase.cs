using GeLiService_WMS.Extensions;
using NLog.Targets;
using GeLiService_WMS.TCPClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.SensorEntity
{
    /// <summary>
    /// 楼层设备类
    /// </summary>
    public class FloorBase
    {
        public string ip { get; set; }
        public int port { get; set; }

        /// <summary>
        /// 传感器客户端
        /// </summary>
        public AsyncClient sensorClient;
        /// <summary>
        /// 接收消息缓存字段
        /// </summary>
        public string sensorMsgFloor = string.Empty;
        public StringBuilder sbuilder = new StringBuilder();
        public DataTable dataTable { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        public string Floor { get; set; }
        /// <summary>
        /// 电表
        /// </summary>
        public Ammeter Ammeter { get; set; }

        /// <summary>
        /// 温湿度表
        /// </summary>
        public Humiture Humiture { get; set; }

        /// <summary>
        /// 噪声表
        /// </summary>
        public Noise Noise { get; set; }

        /// <summary>
        /// 有毒气体表
        /// </summary>
        public ToxicGas ToxicGas { get; set; }

        /// <summary>
        /// 温控1
        /// </summary>
        public Thermostat Thermostat1 { get; set; }

        /// <summary>
        /// 温控2
        /// </summary>
        public Thermostat Thermostat2 { get; set; }

        /// <summary>
        /// 温控3
        /// </summary>
        public Thermostat Thermostat3 { get; set; }

        /// <summary>
        /// 温控4
        /// </summary>
        public Thermostat Thermostat4 { get; set; }

        /// <summary>
        /// 温控5
        /// </summary>
        public Thermostat Thermostat5 { get; set; }

        /// <summary>
        /// 温控6
        /// </summary>
        public Thermostat Thermostat6 { get; set; }
        /// <summary>
        /// 柴油流量
        /// </summary>
        public float DieselOilFlow { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FloorBase(string ip,int port,string floor)
        {
            this.ip = ip;
            this.port = port;
            //this.dataTable = dt.Clone();
            this.Floor = floor;
            this.Ammeter = new Ammeter();
            this.Humiture = new Humiture();
            this.Noise = new Noise();
            this.Thermostat1 = new Thermostat();
            this.Thermostat2 = new Thermostat();
            this.Thermostat3 = new Thermostat();
            this.Thermostat4 = new Thermostat();
            this.Thermostat5 = new Thermostat();
            this.Thermostat6 = new Thermostat();
            this.ToxicGas = new ToxicGas();

            Ammeter.StationAddress=11;
            Humiture.StationAddress=18;
            Noise.StationAddress=19;
            ToxicGas.StationAddress=1;
            //依次设置温控表的站地址
            this.Thermostat1.StationAddress = 5;
            this.Thermostat2.StationAddress = 6;
            this.Thermostat3.StationAddress = 7;
            this.Thermostat4.StationAddress = 8;
            this.Thermostat5.StationAddress = 9;
            this.Thermostat6.StationAddress = 10;
            InitIEquipment();
        }
        private Dictionary<int, IEquipment> dic = new Dictionary<int, IEquipment>();
        private void InitIEquipment()
        {
            var type = this.GetType();
            var BaseType = typeof(IEquipment);

            //var types = type.Assembly.GetTypes()
            //        .Where(a => BaseType.IsAssignableFrom(a) && a != BaseType
            //                    && a.BaseType == BaseType).ToList();
            PropertyInfo[] propertyInfos = this.GetType().GetProperties().Where(
                u => u.PropertyType.GetInterfaces().Contains(BaseType)).ToArray();

            
            foreach (var temp in propertyInfos)
            {
                IEquipment obj = temp.GetValue(this) as IEquipment;

                if (!dic.ContainsKey(obj.StationAddress))
                {
                    dic.Add(obj.StationAddress, obj);
                }
            }
        }

        /// <summary>
        /// 将返回的数据按机台号分发给设备
        /// </summary>
        /// <param name="iPEndPoint">连接IP</param>
        /// <param name="buff">数据</param>
        public void SetValue(IPEndPoint iPEndPoint ,byte[] buff)
        {
            int station = buff[0];

            IEquipment equipment = dic[station];
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in buff)
            {
                stringBuilder.Append(item.ToString("x2"));
            }
            Logger.Default.Process(new Log(LevelType.Info, $"接收：{iPEndPoint.Address}{iPEndPoint.Port}:{equipment.EquipmentName}:{equipment.StationAddress}\r\n{stringBuilder}"));
            equipment.ReceiveMessage=stringBuilder.ToString();
        }


        private List<IEquipment> properties { get { return dic.Values.ToList(); } }
        /// <summary>
        /// 发送本楼层所有设备的命令
        /// </summary>
        public void SendCommand()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var iEquipment in properties)
            {
                stringBuilder.Clear();
                
                foreach (var item in iEquipment.CommandCode)
                {
                    stringBuilder.Append(item.ToString("x2"));
                }
                Logger.Default.Process(new Log(LevelType.Info,
                    $"184:发送{sensorClient.client.Addresses}:{sensorClient.client.Port}:{iEquipment.EquipmentName}:{iEquipment.StationAddress}:\r\n{stringBuilder}"));
                sensorClient.client.Send(iEquipment.CommandCode);
                Thread.Sleep(200);
            }

        }


    }
}

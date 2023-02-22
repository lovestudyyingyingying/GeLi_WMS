using AsyncTCPClient;
using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingData_WMS.Extensions;
using NanXingService_WMS.Entity.SensorEntity;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.PlcUtils;
using Newtonsoft.Json;
using NX_WorkshopData.TCPClient;
using S7.Net.Types;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DateTime = System.DateTime;

namespace NanXingService_WMS.Services
{
    public class SensorDataService
    {
        public SensorDataService(){
            sensorData_dt = sensorDataDao.ClassToDataTable(typeof(SensorData));
        }
        DbBase<SensorData> sensorDataDao = new DbBase<SensorData>();
        string _tableName = "SensorData";
        #region 数据库操作
        public async Task<bool> AddSensorDataAsync(DataTable dt,SensorData sensorData)
        {
            //DataTable dt = sensorDataDao.ClassToDataTable(typeof(SensorData));
            dt.Clear();
            dt = sensorDataDao.ParseInDataTable(dt,sensorData);
            return await sensorDataDao.SetToTableAsync(dt, _tableName);
        }
        public bool AddSensorData(DataTable dt, SensorData sensorData)
        {
            //DataTable dt = sensorDataDao.ClassToDataTable(typeof(SensorData));
            dt.Clear();
            dt = sensorDataDao.ParseInDataTable(dt, sensorData);
            return sensorDataDao.SetDataTableToTable(dt, _tableName);
        }
        public IQueryable<SensorData> GetQuery(Expression<Func<SensorData, bool>> whereLambda = null)
        {
            return sensorDataDao.GetIQueryable(whereLambda);
        }

        public List<SensorData> GetList(Expression<Func<SensorData, bool>> whereLambda = null)
        {
            return sensorDataDao.GetList(whereLambda);
        }


        public List<AgvTempData> GetAgvThermostatList(DateTime sDateTime)
        {
            List<SensorData> list = GetList(u => u.RefleshTime >= sDateTime && u.Floor=="3");

            var dataQuery = list.AsQueryable();
            var agvQuery = dataQuery.GroupBy(u => u.RefleshTime.Value.ToString("yyyy-MM-dd HH:mm:00"))
                .Select(u => new
                {
                    rfTime= u.Key,
                    agvT1 = u.Average(k => k.ChamberTemp1),
                    agvT2 = u.Average(k => k.StackTemp1),

                    agvT3 = u.Average(k => k.ChamberTemp2),
                    agvT4 = u.Average(k => k.StackTemp2),

                    agvT5 = u.Average(k => k.ChamberTemp3),
                    agvT6 = u.Average(k => k.StackTemp3),

                });
            var dateQuery = DateUtils.GetAllMinutes(sDateTime, DateTime.Now).AsQueryable();

            var query = from a in dateQuery
                        join b in agvQuery
                        on a.ToString("yyyy-MM-dd HH:mm:00") equals b.rfTime
                        into c
                        from g in c.DefaultIfEmpty()
                        select new AgvTempData{
                            rtime = a.ToString("HH:mm"),
                            agvT1 = g == null ?
                            (a < DateTime.Now ? "0" : string.Empty) : g.agvT1.ToString("f2"),
                            agvT2 = g == null ?
                            (a < DateTime.Now ? "0" : string.Empty) : g.agvT2.ToString("f2"),
                            agvT3 = g == null ?
                            (a < DateTime.Now ? "0" : string.Empty) : g.agvT3.ToString("f2"),
                            agvT4 = g == null ?
                            (a < DateTime.Now ? "0" : string.Empty) : g.agvT4.ToString("f2"),
                            agvT5 = g == null ?
                            (a < DateTime.Now ? "0" : string.Empty) : g.agvT5.ToString("f2"),
                            agvT6 = g == null ?
                            (a < DateTime.Now ? "0" : string.Empty) : g.agvT6.ToString("f2"),
                        };

            return query.ToList();
        }

        public List<AgvTempData> GetAgvRanQiList()
        {
            DateTime sdateTime = DateTime.Now.Date;
            List<SensorData> list = GetList(u => u.RefleshTime >= sdateTime && u.Floor == "3");

            var dataQuery = list.AsQueryable();
            var agvQuery = dataQuery.GroupBy(u => u.RefleshTime.Value.ToString("yyyy-MM-dd HH:mm:00"))
                .Select(u => new
                {
                    rfTime = u.Key,
                    agvT1 = u.Average(k => k.DieselOilFlow??0),
                });

            var dateQuery = DateUtils.GetAllMinutes(sdateTime, DateTime.Now.Date.AddDays(1)
                .AddMinutes(-1)).AsQueryable();
            var query = from a in dateQuery
                        join b in agvQuery
                        on a.ToString("yyyy-MM-dd HH:mm:00") equals b.rfTime
                        into c
                        from g in c.DefaultIfEmpty()
                        select new AgvTempData
                        {
                            rtime = a.ToString("HH:mm"),
                            agvT1 = g == null ? 
                            (a<DateTime.Now ?"0": string.Empty) : g.agvT1.ToString("f2"),
                        };
            return query.ToList();
        }

        public List<AgvTempData> GetNengHaoList(DateTime sDateTime)
        {
            //DateTime sdateTime = DateTime.Now.AddMinutes();
            List<SensorData> list = GetList(u => u.RefleshTime >= sDateTime && u.Floor == "3");

            var dataQuery = list.AsQueryable();
            var agvQuery = dataQuery.GroupBy(u => u.RefleshTime.Value.ToString("yyyy-MM-dd HH:mm:00"))
                .Select(u => new
                {
                    rfTime = u.Key,
                    agvT1 = u.Average(k => k.Ammeter1),
                });

            var dateQuery = DateUtils.GetAllMinutes(sDateTime, DateTime.Now).AsQueryable();
            var query = from a in dateQuery
                        join b in agvQuery
                        on a.ToString("yyyy-MM-dd HH:mm:00") equals b.rfTime
                        into c
                        from g in c.DefaultIfEmpty()
                        select new AgvTempData
                        {
                            rtime = a.ToString("HH:mm"),
                            agvT1 = g == null ?
                            (a < DateTime.Now ? "0" : string.Empty) : g.agvT1.ToString("f2"),
                        };
            return query.ToList();
        }

        public SensorData GetLastSensorData()
        {
            return sensorDataDao.GetIQueryable(u => u.Floor == "3").OrderByDescending(u=>u.ID).FirstOrDefault();
        }
        #endregion


        #region 传感器读取

        #region 变量
        FloorBase floor = null;
        ////FloorBase floor2 = new FloorBase("127.0.0.1", 60000, "2");
        //FloorBase floor3 = new FloorBase("192.168.10.215", 8001,"3");

        //SensorData data = null;
        /// <summary>
        /// 传感器客户端三楼
        /// </summary>
        //AsyncClient sensorClientF3;

        public DataTable sensorData_dt =null;
        #endregion
        /// <summary>
        /// 读取传感器入口
        /// </summary>
        public void StartRead()
        {
        //    floor2.dataTable = sensorData_dt.Clone();
        //    floor3.dataTable = sensorData_dt.Clone();

        //    while (true)
        //    {
        //        Stopwatch watch = Stopwatch.StartNew();

        //        //主要操作
        //        MainControl(floor2);
        //        watch.Stop();
        //        Logger.Default.Info($"2L数据采集完成：{watch.ElapsedMilliseconds}");

        //        watch.Restart();
        //        MainControl(floor3);
        //        watch.Stop();
        //        Logger.Default.Info($"3L数据采集完成：{watch.ElapsedMilliseconds}");

        //        Thread.Sleep(5000);
        //    }
        }


        public void Close()
        {
            if (floor.sensorClient!= null && floor.sensorClient.client.Connected)
            {
                floor.sensorClient.client.Close();
                floor.sensorClient.client.Dispose();
            }
        }

        public void MainControl(FloorBase floor)
        {
            this.floor = floor;
            //1、检查连接是否已连接
            if (floor.sensorClient == null || !floor.sensorClient.client.Connected)
            {
                floor.sensorClient = new AsyncClient(floor.port, floor.ip, MsgException, MsgConnected, MsgDisconnected, sensorFloor_Received);
            }
            if (floor.sensorClient != null || floor.sensorClient.client.Connected)
            {
                //等待传感器数据读取完成
                SensorData data = ParseData(floor);
                //3、写入数据库
                AddSensorData(floor.dataTable, data);
                
            }
        }
        /// <summary>
        /// 获取传感器数据实体
        /// </summary>
        /// <param name="floor"></param>
        private SensorData ParseData(FloorBase floor)
        {
            floor.SendCommand();
            Thread.Sleep(2000);
            SensorData data = new SensorData();
            data.Floor = floor.Floor;
            //获取数据
            data.RefleshTime = DateTime.Now;

            float[] ammeterResult = floor.Ammeter.GetResult();
            data.Ammeter1 = ammeterResult[0];
            data.Ammeter2 = ammeterResult[1];
            data.Ammeter3 = ammeterResult[2];

            float[] humitureReslut = floor.Humiture.GetResult();
            data.Humidity = humitureReslut[0];
            data.Temperature = humitureReslut[1];

            data.Noise = floor.Noise.GetResult();
            data.ToxicGas = floor.ToxicGas.GetResult();

            data.StackTemp1 = floor.Thermostat1.GetResult();
            data.ChamberTemp1 = floor.Thermostat2.GetResult();
            data.StackTemp2 = floor.Thermostat3.GetResult();
            data.ChamberTemp2 = floor.Thermostat4.GetResult();
            data.StackTemp3 = floor.Thermostat5.GetResult();
            data.ChamberTemp3 = floor.Thermostat6.GetResult();
            if (floor.Floor == "3")
                data.DieselOilFlow = PLCInstance.Instance.GetPLCData();

            //Logger.Default.Process(new Log(LevelType.Info, "253：：\r\n"+JsonConvert.SerializeObject(data)));
            return data;

        }

        /// <summary>
        /// 判断新数据是否正确
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        /// <returns></returns>
        private bool IsDataRight(SensorData oldData, SensorData newData)
        {
            if (oldData == null)
            {
                return true;
            }
            if (newData.Temperature > 45)
                return false;
            if (newData.Humidity > 100)
                return false;
            if (newData.Noise > 150)
                return false;
            if (Math.Abs(oldData.StackTemp1 - newData.StackTemp1) > 2)
                return false;
            if (Math.Abs(oldData.ChamberTemp1 - newData.ChamberTemp1) > 2)
                return false;
            if (Math.Abs(oldData.StackTemp2 - newData.StackTemp2) > 2)
                return false;
            if (Math.Abs(oldData.ChamberTemp2 - newData.ChamberTemp2) > 2)
                return false;
            if (Math.Abs(oldData.StackTemp3 - newData.StackTemp3) > 2)
                return false;
            if (Math.Abs(oldData.ChamberTemp3 - newData.ChamberTemp3) > 2)
                return false;
            return true;
        }
        
        #endregion

        #region MsgEvent

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
        /// 客户端连接事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgConnected(object sender, TcpServerConnectedEventArgs e)
        {
            Logger.Default.Process(new Log("Info", $"TCP server {e.ToString()} has connected."));

            //MessageBox.Show($"TCP server {e.ToString()} has connected.");
        }

        /// <summary>
        /// 客户端断开连接事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            //MessageBox.Show($"TCP server {e.ToString()} has disconnected.");
            Logger.Default.Process(new Log("Warn", $"TCP server {e.ToString()} has disconnected."));
            Thread.Sleep(5000);
            
            floor.sensorClient = new AsyncClient(floor.port, floor.ip, MsgException, MsgConnected, MsgDisconnected, sensorFloor_Received);
        }
        /// <summary>
        /// 传感器客户端接收消息事件二楼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sensorFloor_Received(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            lock (e.TcpClient.Client.RemoteEndPoint)
            {
                IPEndPoint iPEndPoint = (IPEndPoint)e.TcpClient.Client.RemoteEndPoint;
                try
                {
                    floor.SetValue(iPEndPoint, e.Datagram);
                }
                catch (Exception ex)
                {
                    Logger.Default.Process(new Log(LevelType.Error, $"{iPEndPoint.Address}{iPEndPoint.Port}\r\n{ex}"));
                }
            }
                
        }
        /// <summary>
        /// 楼层传感器数据转换方法
        /// </summary>
        /// <param name="floorBase"></param>
        /// <param name="buff"></param>
        private void ParseFloor(FloorBase floorBase,byte[] buff )
        {
            floorBase.sensorMsgFloor = string.Empty;
            floorBase.sbuilder.Clear();
            foreach (var item in buff)
            {
                floorBase.sbuilder.Append(item.ToString("x2"));
            }
            //Logger.Default.Process(new Log(LevelType.Info, $"404:{floorBase.sbuilder}"));
            floorBase.sensorMsgFloor = floorBase.sbuilder.ToString();
        }
        #endregion
    }
}

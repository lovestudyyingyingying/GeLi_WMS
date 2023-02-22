using NanXingData_WMS.Dao;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.TiShengJiEntity
{
    /// <summary>
    /// 提升机配置数据类
    /// </summary>
    public class TiShengJiDevice
    {
        //public ConcurrentQueue<AGVMissionInfo_Floor> ts = new ConcurrentQueue<AGVMissionInfo_Floor>();

        public string tsj_Name { get; set; }
        public string[] tsj_YouXianType { get; set; }

        public string tsj_IP { get; set; }
        public int tsj_Port { get; set; }

        /// <summary>
        /// 仓位号
        /// </summary>
        public string tsj_1F  { get; set; }
        public string tsj_2F  { get; set; }
        public string tsj_3F  { get; set; }

        /// <summary>
        /// 任务模板
        /// </summary>
        public string tsj_MoveIn_1F  { get; set; }
        public string tsj_MoveIn_2F  { get; set; }
        public string tsj_MoveIn_3F  { get; set; }

        /// <summary>
        /// 任务模板
        /// </summary>
        public string tsj_MoveOut_1F  { get; set; }
        public string tsj_MoveOut_2F  { get; set; }
        public string tsj_MoveOut_3F  { get; set; }

        public TiShengJiDevice(int index)
        {
            InitConfig(index);
        }

        public void InitConfig(int index)
        {
            tsj_Name = "tsj-" + index;

            tsj_IP = ConfigurationManager.AppSettings[tsj_Name + "-IP"];
            tsj_Port = int.Parse(ConfigurationManager.AppSettings[tsj_Name + "-Port"]);

            tsj_1F = ConfigurationManager.AppSettings[tsj_Name + "-1L"].ToString();
            tsj_2F = ConfigurationManager.AppSettings[tsj_Name + "-2L"].ToString();
            tsj_3F = ConfigurationManager.AppSettings[tsj_Name + "-3L"].ToString();

            tsj_MoveIn_1F = ConfigurationManager.AppSettings[tsj_Name + "-1L-MoBanMoveIn"].ToString();
            tsj_MoveIn_2F = ConfigurationManager.AppSettings[tsj_Name + "-2L-MoBanMoveIn"].ToString();
            tsj_MoveOut_1F = ConfigurationManager.AppSettings[tsj_Name + "-1L-MoBanMoveOut"].ToString();
            tsj_MoveOut_2F = ConfigurationManager.AppSettings[tsj_Name + "-2L-MoBanMoveOut"].ToString();
        }


    }
}

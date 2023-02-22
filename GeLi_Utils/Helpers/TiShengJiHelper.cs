using GeLi_Utils.Services.WMS;
using GeLiData_WMS;
using GeLiService_WMS;
using GeLiService_WMS.Entity.AGVApiEntity;
using GeLiService_WMS.Services.WMS.AGV;
using HslCommunication.Profinet.Melsec;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Helpers
{
    public class TiShengJiHelper
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public static string[] PLCErrorRegister = new string[] {   "D1004", "D1005", "D1006" };
        public static string PLCStateRegister = "D1000";
        public static string PLCMoveStateRegister = "D1001";
        private MelsecMcNet melsec_net;

        public TiShengJiHelper(string Ip, int port)
        {
            melsec_net = new MelsecMcNet(Ip, port);
        }

        public bool CompareTiShengJiStateIsChange(TiShengJiInfo tiShengJiInfo,string firstFloorState, string secondFloorState,string tiShengJiMoveState,string errorState)
        {
            if(tiShengJiInfo.TiShengJiState==null)
                return false;
            if(tiShengJiInfo.TiShengJiState.F1DuiJieWei==firstFloorState)
                return false;
            if (tiShengJiInfo.TiShengJiState.F2DuiJieWei == secondFloorState)
                return false;
            if (tiShengJiInfo.TiShengJiState.carState == tiShengJiMoveState)
                return false;
            if (tiShengJiInfo.TiShengJiState.deviceState == errorState)
                return false;

            return true;

        }

        public void ReadTiShengJiState()
        {
            bool isConnect = melsec_net.ConnectServer().IsSuccess;
            if (!isConnect)
            {
                return;
            }

            var MissionState = melsec_net.ReadInt32(PLCStateRegister);
            var PLCMoveState=melsec_net.ReadInt32(PLCMoveStateRegister);
            var plcErrorState = melsec_net.ReadInt32(PLCErrorRegister[0], 3);
            if (null != MissionState && MissionState.IsSuccess&& PLCMoveState!=null&& PLCMoveState.IsSuccess)
            {
                int D1000 = MissionState.Content;//从数组中取得D1000的值；
                int D1001 = PLCMoveState.Content;//从数组中取得D1001的值；
                int[] errorRegister = plcErrorState.Content;
                Logger.Default.Process(new Log(LevelType.Debug, D1000.ToString()));
                string stateToBinary = Convert.ToString(D1000, 2).PadLeft(3,'0');
                string state = stateToBinary.Substring(stateToBinary.Length - 3, 3);
                if (string.IsNullOrEmpty(state))
                {
                    Logger.Default.Process(new Log(LevelType.Error, "采集的状态字符为空"));

                    return;
                }
                string firstFloorState = string.Empty;
                string secondFloorState = string.Empty; 
                string tiShengJiMoveState = string.Empty;
                string errorState = string.Empty;
                //采集并对状态进行判断
                if (state[2]=='1'&& state[0] == '0') //对应的state[2]对应D1000.1(允许线头上件)，state[1]对应D1000.2(线尾请求下线，state[0]对应D1000.3(线头请求返件)
                    firstFloorState = TiShengState.AllowUpMission;
                else if (state[0] == '1' && state[2] == '0')
                    firstFloorState = TiShengState.OneFloorHadGood;
                else
                    firstFloorState = TiShengState.OneFloorWorking;
                if (state[1] == '1')
                    secondFloorState = TiShengState.SecFloorHadGood;
                else
                    secondFloorState = TiShengState.SecFloorHadNoGood;

                if (D1001 == 1)
                    tiShengJiMoveState = TiShengState.MotorForward;
                else if(D1001 == 2)
                    tiShengJiMoveState = TiShengState.MotorReverse;

                if (errorRegister[0] == 1)
                    errorState = DeviceState.Warn1;
                else if (errorRegister[0] == 2)
                    errorState = DeviceState.Warn1;
                else if (errorRegister[1] == 1)
                    errorState = DeviceState.Warn2;
                else if (errorRegister[1] == 2)
                    errorState = DeviceState.Warn3;
                else if (errorRegister[1] == 3)
                    errorState = DeviceState.Warn4;
                else if (errorRegister[2] == 1)
                    errorState = DeviceState.Warn5;
                else if (errorRegister[2] == 2)
                    errorState = DeviceState.Warn6;
                else if (errorRegister[2] == 3)
                    errorState = DeviceState.Warn7;
                else
                    errorState = DeviceState.Normal;

                TiShengJiInfoService tiShengJiInfoService = new TiShengJiInfoService();
                TiShengJiStateService tiShengJiStateService = new TiShengJiStateService();
                var tiShengJiInfo = tiShengJiInfoService.GetInfoByIp(melsec_net.IpAddress);
                if (tiShengJiInfo == null)
                {
                    Logger.Default.Process(new Log(LevelType.Error,$"数据库中不存在{melsec_net.IpAddress}"));
                    return;
                }
                //表示有变化
                if(CompareTiShengJiStateIsChange(tiShengJiInfo,firstFloorState,secondFloorState,tiShengJiMoveState,errorState))
                {
                    TiShengJiState tiShengJiState = new TiShengJiState();
                    tiShengJiState.TsjIp = melsec_net.IpAddress;
                    tiShengJiState.InputTime = DateTime.Now;
                    tiShengJiState.deviceState = errorState;
                    tiShengJiState.carState = tiShengJiMoveState;
                    tiShengJiState.F1DuiJieWei = firstFloorState;
                    tiShengJiState.F2DuiJieWei = secondFloorState;
                    if(errorState!=DeviceState.Normal)
                    {
                        AGVAlarmLog aGVAlarmLog = new AGVAlarmLog();
                        aGVAlarmLog.deviceNum = tiShengJiInfo.TsjName;
                        aGVAlarmLog.alarmDesc = errorState;
                        aGVAlarmLog.alarmDate = DateTime.Now;
                        aGVAlarmLog.alarmDate = DateTime.Now;
                        AGVAlarmLogService aGVAlarmLogService = new AGVAlarmLogService();
                        aGVAlarmLogService.Insert(aGVAlarmLog);
                        aGVAlarmLogService.SaveChanges();
                    }
                    tiShengJiStateService.Insert(tiShengJiState);
                    tiShengJiStateService.SaveChanges();
                    tiShengJiInfo.TiShengJiState_ID = tiShengJiState.ID;
                    tiShengJiInfoService.UpdateByPlus(u => u.ID == tiShengJiInfo.ID, u => new TiShengJiInfo { TiShengJiState_ID = tiShengJiState.ID });
                    tiShengJiInfoService.SaveChanges();
                }
                

            }
        }

    }

    public class TiShengState
    {
        public static string AllowUpMission = "一楼上件";
        public static string OneFloorHadGood = "一楼接货";
        public static string OneFloorWorking = "一楼忙碌";
        public static string SecFloorHadGood = "二楼接货";
        public static string SecFloorHadNoGood = "二楼无货";
    
        public static string MotorForward = "电机正转";
        public static string MotorReverse = "电机反转";
    }

    public class DeviceState
    {
        public static string Normal = "正常";
        public static string Warn1 = "线体链条变频器报警";
        public static string Warn2 = "线体转台卡料报警";
        public static string Warn3 = "线体转台变频器报警";
        public static string Warn4 = "线体转台急停报警";
        public static string Warn5 = "提升机卡料报警";
        public static string Warn6 = "提升机变频器报警";
        public static string Warn7 = "提升机急停报警";

    }
}

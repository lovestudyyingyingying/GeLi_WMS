using GeLi_Utils.Entity.MaPanJiStateEntity;
using GeLi_Utils.Services.WMS;
using GeLiData_WMS;
using GeLiData_WMS.Dao;
using GeLiData_WMSUtils;
using GeLiService_WMS;
using GeLiService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Helpers
{
    public class MaPanJiHelper
    {
        public static string startPoint = "M350";
        public static string CanSendPoint = "M357";
        public static string BanCount = "C216";
        public static string ErrorStartPoint = "M150";
        ModbusTCPDeltaHelper modbusTCPDeltaHelper;
        RedisHelper redisHelper = new RedisHelper();
        static Dictionary<int, string> errorLookup = new Dictionary<int, string>
{
    {0, MaPanJiErrorState.MaxTray }               ,
    {1, MaPanJiErrorState.TrayOver}     ,
    {2, MaPanJiErrorState.MinTray           }     ,
    {3, MaPanJiErrorState.TrayNoOut         }     ,
    {4, MaPanJiErrorState.TrayAtBottom      }     ,
    {5, MaPanJiErrorState.CylinderOriginWarn} ,
     {6, MaPanJiErrorState.CylinderMoveWarn
    }      ,
    {7, MaPanJiErrorState.GearCylinderOriginWarn
} ,
    { 8, MaPanJiErrorState.GearCylinderMoveWarn  } ,
    { 9, MaPanJiErrorState.SuddenStopWarn        } ,
    { 10, MaPanJiErrorState.CanInitialization     },
    { 11, MaPanJiErrorState.TransducerWarn        },
    { 12, MaPanJiErrorState.LeftAndRightTooFast   },
    { 13, MaPanJiErrorState.NoDeviceRunning },
            // 其他位置的映射
        };
        public MaPanJiHelper(string Ip, int port)
        {
            modbusTCPDeltaHelper = new ModbusTCPDeltaHelper(Ip, port);

        }

        private bool CompareIsSame(List<bool> bools, int banNum, MaPanJiState state)
        {
            if (state.IsDieBan == bools[0] &&
            state.IsDieBanReadyAndAllowIn == bools[1] &&
            state.IsDieBaning == bools[2] &&
            state.IsChaiBan == bools[3] &&
            state.IsChaiBanReadyAndAllowIn == bools[4] &&
            state.IsChaiBaning == bools[5] &&
            state.IsChaiBanEnd == bools[6] &&
            state.IsDieBanEnd == bools[7] && state.BanNum == banNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据返回的状态得到状态简介
        /// </summary>
        /// <param name="bools"></param>
        /// <param name="banNum"></param>
        /// <returns></returns>
        public string GetStateStr(List<bool> bools, int banNum)
        {
            string str = string.Empty;
            bools.ForEach((item) =>
            {
                if (item)
                    str += "1";
                else
                    str += "0";
            }
            );
            //转换为二进制
            if (str == "00000000" && banNum < 10)
                return MaPanJiStateSummarize.original;
            else if (str == "10000000" && banNum < 10)
                return MaPanJiStateSummarize.DeliverTask;
            else if (str == "01000000" && banNum < 10)
                return MaPanJiStateSummarize.TaskingCanIn;
            else if (str == "00100000" && banNum < 10)
                return MaPanJiStateSummarize.DieBaning;
            else if (str == "00000001" && banNum < 10)
                return MaPanJiStateSummarize.Idle;
            else if (str == "00000001" && banNum == 10)
                return MaPanJiStateSummarize.FullEmptyTray;
            else
                return str;

        }



        /// <summary>
        /// 读取状态:放在循环里执行
        /// </summary>
        public void GetAndSaveState()
        {
            using (redisHelper.CreateLock($"MaPanJiLock:{modbusTCPDeltaHelper.Ip}:{modbusTCPDeltaHelper.Port}",
                TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(30),
                TimeSpan.FromMilliseconds(500)))
            {
                MaPanJiStateService maPanJiStateService = new MaPanJiStateService();
                //读取线圈状态
                List<bool> state = modbusTCPDeltaHelper.ReadManyBoolean(startPoint, 8);

                if (state == null)
                    return;
                int? num = modbusTCPDeltaHelper.ReadDecimal(BanCount);
                if (num == null)
                    return;


                DbBase<MaPanJiInfo> dbBase = new DbBase<MaPanJiInfo>();
                var maPanJiInfo = dbBase.GetIQueryable(u => u.MpjIp == modbusTCPDeltaHelper.Ip, true, DbMainSlave.Master).FirstOrDefault();
                string stateStr = GetStateStr(state, num.Value);
                //表示还没有绑定状态，直接绑定
                if (maPanJiInfo.MaPanJiState == null)
                {
                    maPanJiInfo.MaPanJiState_ID = maPanJiStateService.CreatMaPanJiState(state, num.Value, stateStr);

                }
                else
                {
                    var maPanJiState = maPanJiInfo.MaPanJiState; // 拿到旧的状态
                    if (CompareIsSame(state, num.Value, maPanJiState))
                    {
                        //如果跟旧数据相同
                    }
                    else
                    {
                        maPanJiInfo.MaPanJiState_ID = maPanJiStateService.CreatMaPanJiState(state, num.Value, stateStr);

                    }
                }
                bool iserror = false;
                if (!CheackWetherNoError(modbusTCPDeltaHelper))
                    iserror = true;
                dbBase.UpdateByPlus(u => u.ID == maPanJiInfo.ID, u => new MaPanJiInfo { MaPanJiState_ID = maPanJiInfo.MaPanJiState_ID, IsError = iserror });
                dbBase.SaveChanges();
                Logger.Default.Process(new Log(LevelType.Info,
                                maPanJiInfo.MpjName + maPanJiInfo.MaPanJiState_ID + maPanJiInfo.MaPanJiState.Reserve1));
            }
        }

        /// <summary>
        /// 发送码盘任务，在发送AGV信号时一并发送 (port传502) 下发任务
        /// </summary>
        public  bool SendMissionToMaPanJi(string Ip, int port)
        {
            using (redisHelper.CreateLock($"MaPanJiLock:{modbusTCPDeltaHelper.Ip}:{modbusTCPDeltaHelper.Port}",
              TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(30),
              TimeSpan.FromMilliseconds(500)))
            {
                ModbusTCPDeltaHelper modbusTCPDeltaHelper = new ModbusTCPDeltaHelper(Ip, port);
                //判断点位
                
                bool canWrite1 = modbusTCPDeltaHelper.ReadBoolean(CanSendPoint) == null ? false : modbusTCPDeltaHelper.ReadBoolean(CanSendPoint).Value;
                bool canWrite2 = false;
                if (!canWrite1)//表示码盘机357=0
                {
                    var statePoint = modbusTCPDeltaHelper.ReadManyBoolean(startPoint, 3);
                    if(statePoint!=null&&statePoint.Where(u=>u).Count()==0)
                    {
                        canWrite2 = true;
                    }
                }
                //判断码盘机是否故障
                var IsOk = CheackWetherNoError(modbusTCPDeltaHelper);
                if ((canWrite1||canWrite2) && IsOk)
                    return modbusTCPDeltaHelper.WriteBoolean(startPoint, true);
                else
                    return false;
            }
        }

        private static bool CheackWetherNoError(ModbusTCPDeltaHelper modbusTCPDeltaHelper)
        {

            return modbusTCPDeltaHelper.ReadManyBoolean(ErrorStartPoint, 14).Where(u => u == true).Count() == 0;
        }

        /// <summary>
        /// 开始码盘任务，在接收AGV进入许可后写入 (port传502)
        /// </summary>
        public  bool StartMaPan(string Ip, int port)
        {
            using (redisHelper.CreateLock($"MaPanJiLock:{modbusTCPDeltaHelper.Ip}:{modbusTCPDeltaHelper.Port}",
              TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(30),
              TimeSpan.FromMilliseconds(500)))
            {
                ModbusTCPDeltaHelper modbusTCPDeltaHelper = new ModbusTCPDeltaHelper(Ip, port);
                bool canWrite = modbusTCPDeltaHelper.ReadBoolean("M351") == null ? false : modbusTCPDeltaHelper.ReadBoolean("M351").Value;
                var IsOk = CheackWetherNoError(modbusTCPDeltaHelper);
                if (canWrite && IsOk)
                    return modbusTCPDeltaHelper.WriteBoolean("M352", true);
                else
                    return false;
            }
        }


        public void CheckAndSaveError()
        {
            
                DbBase<AGVAlarmLog> dbBaseAGVAlarmLog = new DbBase<AGVAlarmLog>();
                List<bool> errorPoint = modbusTCPDeltaHelper.ReadManyBoolean(ErrorStartPoint, 14);
                var maPanJiInfoService = new MaPanJiInfoService();
                if (errorPoint == null)
                    return;
                var isOk = CheackWetherNoError(modbusTCPDeltaHelper); // 表示没有错误
                var mapanState = maPanJiInfoService.GetMaPanJiEntityByIp(modbusTCPDeltaHelper.Ip);
                if (mapanState == null)
                    return;
                if (isOk)
                {
                    //表示没有错误
                    if (mapanState.IsError)
                    {
                     
                        maPanJiInfoService.UpdateByPlus(u => u.ID == mapanState.ID, u => new MaPanJiInfo { IsError = false });
                        maPanJiInfoService.SaveChanges();
                        Logger.Default.Process(new Log(LevelType.Info,
                       mapanState.ID + mapanState.MpjName+ $"码盘机状态更改为成功"));
                    }

                }
                else
                {
                    var errorStrings = errorPoint
                    .Select((error, index) => new { error, index })
                    .Where(x => x.error)
                    .Select(x => errorLookup[x.index])
                    .ToList();
                    //MaPanJiStateService maPanJiStateService = new MaPanJiStateService();


                    List<AGVAlarmLog> alarmLogList = new List<AGVAlarmLog>();

                    foreach (var item in errorStrings)
                    {
                        AGVAlarmLog alarmLog = new AGVAlarmLog();
                        alarmLog.deviceName = mapanState.MpjName;
                        alarmLog.alarmDesc = item;
                        //alarmLog.areaId = mapanState.MpjPosition;
                        alarmLog.alarmReadFlag = 0;
                        alarmLog.channelDeviceId = mapanState.MpjName;
                        alarmLog.alarmSource = mapanState.MpjName;
                        alarmLog.channelName = mapanState.MpjPosition;
                        alarmLog.recTime = DateTime.Now;
                        alarmLog.alarmDate = DateTime.Now;
                        alarmLog.alarmGrade = 0;
                        alarmLogList.Add(alarmLog);
                        Logger.Default.Process(new Log(LevelType.Info,
                       alarmLog.deviceName+ alarmLog.alarmDate+ alarmLog.alarmDesc+$"码盘机故障记录获取成功"));
                    }
                    //mapanState.IsOpen = false;
                    maPanJiInfoService.UpdateByPlus(u => u.ID == mapanState.ID, u => new MaPanJiInfo { IsError = true });
                    maPanJiInfoService.SaveChanges();
                    dbBaseAGVAlarmLog.InsertRange(alarmLogList);
                    dbBaseAGVAlarmLog.SaveChanges();
                }
           
            
            
        }
        /// <summary>
        /// 改写码盘机PLC的板内数量为0
        /// </summary>
        public bool WriteTrayNumToZero()
        {
            using (redisHelper.CreateLock($"MaPanJiLock:{modbusTCPDeltaHelper.Ip}:{modbusTCPDeltaHelper.Port}",
              TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(30),
              TimeSpan.FromMilliseconds(500)))
            {

                if (CheackWetherNoError(modbusTCPDeltaHelper))//检查码盘机是否为错误状态
                {
                    if (modbusTCPDeltaHelper.ReadBoolean("M352").Value) //如果正在工作
                        return false;
                    return modbusTCPDeltaHelper.WriteDecimal(BanCount, 0);
                }
                else
                    return false;


            }
        }
        public void DisConnect ()
        {
            modbusTCPDeltaHelper.DisConnect();
        }
    }
}

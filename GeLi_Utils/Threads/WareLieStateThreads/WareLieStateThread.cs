
using GeLiData_WMS;
using GeLiData_WMSUtils;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Managers;
using GeLiService_WMS.Services;
using GeLiService_WMS.Services.WMS;
using GeLiService_WMS.Services.WMS.AGV;
using GeLiService_WMS.Utils.RabbitMQ;
using GeLiService_WMS.Utils.RedisUtils;
using GeLiService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GeLiService_WMS.Threads
{
    public class WareLieStateThread
    {
        public static string queueName = "CheckWareLieState";
        RedisHelper redisHelper = new RedisHelper();
        AGVMissionService agvMissionService = new AGVMissionService();
        WareLocationService _wareLocationService = new WareLocationService();
        
        WareLocationLockHisService _wareLoactionLockHisService=new WareLocationLockHisService();

        RabbitMQUtils _RabbitMQUtils;
        MyTask myTask = null;

        public WareLieStateThread(RabbitMQUtils RabbitMQUtils)
        {
            _RabbitMQUtils = RabbitMQUtils;
        }


        public void Run()
        {
            myTask=_RabbitMQUtils.Recevice(queueName,
                new Action<string>((item) =>
                {
                    Control(item);
                }));
        }
        

        private void Control(string lieName)
        {
            string lockName = $"{WareLocationService.wareLocker}:{lieName}";
            using (redisHelper.CreateLock(lockName, TimeSpan.FromSeconds(15),
                TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(300)))
            {
                DateTime dateTime = DateTime.Now.AddDays(-1);

                bool ret_Lie = _wareLocationService.Any(u => 
                    u.WareLoca_Lie==lieName &&
                    (u.WareLocaState == WareLocaState.PreIn
                 || u.WareLocaState == WareLocaState.PreOut),true,DbMainSlave.Master);
                if (!ret_Lie)
                    return;
                bool ret_Mission = agvMissionService.Any(u => u.OrderTime >= dateTime
                  && (u.EndPosition.StartsWith(lieName) || u.StartPosition.StartsWith(lieName))
                     && (u.RunState != StockState.RunState_Success
                    && u.RunState != StockState.RunState_Cancel
                    && u.RunState != StockState.RunState_RunFail
                    && u.RunState != StockState.RunState_SendFail)
                     , true, DbMainSlave.Master);

                if (!ret_Mission)
                {
                    int[] ids=_wareLocationService.GetList(
                        u => u.WareLoca_Lie == lieName &&
                        (u.WareLocaState == WareLocaState.PreIn
                        || u.WareLocaState == WareLocaState.PreOut),
                        true, DbMainSlave.Master).Select(u=>u.ID).ToArray();
                    if (ids.Length == 0)
                        return;
                    
                    List<WareLocation> wareLocations =
                        _wareLocationService.GetList(u => ids.Contains(u.ID), false, DbMainSlave.Master);
                    string[] wlNo = wareLocations.Select(u => u.WareLocaNo).ToArray();
                    Logger.Default.Process(new Log(LevelType.Info,
                               $"预进预出状态的仓位，{string.Join(",", wlNo)}"));
                    using (TransactionScope tran = new TransactionScope())
                    {
                        try
                        {
                            foreach(WareLocation wareLocation in wareLocations)
                            {
                                wareLocation.WareLocaState = WareLocaState.NoTray;
                                _wareLoactionLockHisService.UnLock(wareLocation);
                            }
                            _wareLocationService.UpdateAll(wareLocations);
                            _wareLocationService.SaveChanges();
                            _wareLoactionLockHisService.SaveChanges();
                            tran.Complete();
                        }
                        catch(Exception ex)
                        {
                            Logger.Default.Process(new Log(LevelType.Error,
                                $"预进预出状态修改事务失败，{ex.ToString()}"));
                            throw ex;
                        }
                    }
                }
            }
        }

        public void CloseTask()
        {
            if (myTask != null)
                myTask.CloseTask();
        }
    }
}

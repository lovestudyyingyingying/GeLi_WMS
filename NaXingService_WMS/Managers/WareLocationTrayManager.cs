using NanXingData_WMS.Dao;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Services;
using NanXingService_WMS.Services.WMS;
using NanXingService_WMS.Services.WMS.AGV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace NanXingService_WMS.Managers
{
    public class WareLocationTrayManager
    {
        TrayStateService _trayStateService;
        WareLocationService _wareLocationService;
        WareLocationLockHisService _wareLoactionLockHisService;
        StockRecordService _stockRecordService;

        public WareLocationTrayManager(TrayStateService trayStateService,
            WareLocationService wareLocationService, WareLocationLockHisService wareLoactionLockHisService)
        {
            _trayStateService = trayStateService;
            _wareLocationService = wareLocationService;
            _wareLoactionLockHisService = wareLoactionLockHisService;
            //_stockRecordService = stockRecordService;
        }



        /// <summary>
        /// 绑定库位+货物
        /// </summary>
        /// <param name="type">0为绑定，1为解绑</param>
        /// <param name="wareLocation"></param>
        /// <param name="trayState"></param>
        public bool ChangeTrayWareLocation(int type, WareLocation wareLocation, TrayState trayState)
        {
            bool ret = false;
            //进仓操作,为TrayState、WareLocation绑定，为WareLock解绑
            if (type == 0)
            {
                ret = BindTrayWarelocation(wareLocation, trayState);

            }
            else if (type == 1)
            {
                ret = UnBindTrayWarelocation(wareLocation, trayState);
            }

            return ret;
        }

        private bool BindTrayWarelocation(WareLocation wareLocation, TrayState trayState)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    _wareLoactionLockHisService.UnLock(wareLocation);

                    wareLocation.WareLocaState = WareLocaState.HasTray;
                    wareLocation.TrayState_ID = trayState.ID;
                    wareLocation.TrayState = trayState;
                    wareLocation.LockHis_ID = null;
                    _wareLocationService.Update(wareLocation);
                    _wareLocationService.SaveChanges();

                    trayState.WareLocation_ID = wareLocation.ID;
                    trayState.WareLocation = wareLocation;
                    _trayStateService.Update(trayState);
                    _trayStateService.SaveChanges();

                    tran.Complete();
                    Logger.Default.Process(new Log(LevelType.Info,
                       $"{wareLocation.WareLocaNo}:绑定货物{trayState.TrayNO}:ID {trayState.ID}"));



                    return true;
                }
                catch (Exception ex)
                {
                    //执行错误处理
                    Logger.Default.Process(new Log(LevelType.Error, ex.ToString()));
                    return false;
                }
            }
        }


        private bool UnBindTrayWarelocation(WareLocation wareLocation, TrayState trayState)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    _wareLoactionLockHisService.UnLock(wareLocation);
                    string changeWl = string.Empty;
                    string trayNo = string.Empty;
                    string ID = string.Empty;
                    if (wareLocation != null)
                    {
                        wareLocation.WareLocaState = WareLocaState.NoTray;
                        wareLocation.TrayState_ID = null;
                        wareLocation.LockHis_ID = null;
                        _wareLocationService.Update(wareLocation);
                        _wareLocationService.SaveChanges();
                        changeWl = wareLocation.WareLocaNo;
                    }

                    if (trayState.WareLocation_ID != null)
                    {
                        trayState.WareLocation_ID = null;
                        _trayStateService.Update(trayState) ;
                        _trayStateService.SaveChanges();
                        trayNo = trayState.TrayNO;
                        ID = trayState.ID.ToString();
                    }

                    tran.Complete();
                    Logger.Default.Process(new Log(LevelType.Info,
                       $"{changeWl}:解绑货物{trayNo}:ID {ID}"));
                    return true;
                }
                catch (Exception ex)
                {
                    //执行错误处理
                    Logger.Default.Process(new Log(LevelType.Error, ex.ToString()));
                    return false;
                }
            }
        }

        private bool UnBindTrayWarelocationWithoutTran(WareLocation wareLocation, TrayState trayState,List<BingWareTrayLog> loglist)
        {
            try
            {
                _wareLoactionLockHisService.UnLock(wareLocation);
                BingWareTrayLog bingWareTrayLog = new BingWareTrayLog();
                if (wareLocation != null)
                {
                    wareLocation.WareLocaState = WareLocaState.NoTray;
                    wareLocation.TrayState_ID = null;
                    wareLocation.LockHis_ID = null;
                    //_wareLocationService.Update(wareLocation);
                   
                    bingWareTrayLog.changeWl = wareLocation.WareLocaNo;
                }

                if (trayState.WareLocation_ID != null)
                {
                    trayState.WareLocation_ID = null;
                    //_trayStateService.Update(trayState);
                    bingWareTrayLog.trayNo = trayState.TrayNO;
                    bingWareTrayLog.trayStateID = trayState.ID;
                }
                if(bingWareTrayLog.trayNo.Length>0 && 
                    loglist.Any(u => u.trayNo == bingWareTrayLog.trayNo))
                {
                    BingWareTrayLog temp = loglist.Find(u => u.trayNo == bingWareTrayLog.trayNo);
                    if (temp.changeWl.Length==0)
                        loglist.Remove(temp);
                    if (bingWareTrayLog.changeWl.Length > 0)
                        loglist.Add(bingWareTrayLog);
                }
                else if (bingWareTrayLog.changeWl.Length>0 &&
                    loglist.Any(u => u.changeWl == bingWareTrayLog.changeWl))
                {
                    BingWareTrayLog temp = loglist.Find(u => u.changeWl == bingWareTrayLog.changeWl);
                    if (temp.trayNo.Length == 0)
                        loglist.Remove(temp);
                    if(bingWareTrayLog.trayNo.Length>0)
                        loglist.Add(bingWareTrayLog);
                }
                else
                    loglist.Add(bingWareTrayLog);
                return true;
            }
            catch
            {
                return false;
            }
        }

      
        public bool UnBindTrayWarelocationByBatchNo(string batchNo,string orderUser)
        {
            List<WareLocation> warelist = _wareLocationService.GetList(
                        u => u.TrayState!=null 
                        && u.TrayState.batchNo == batchNo 
                        && u.WareArea.WareAreaClass.AreaClass== AreaClassType.ChengPinArea
                    , false, NanXingData_WMS.DaoUtils.DbMainSlave.Master);
            List<TrayState> traystatelist = _trayStateService.GetList(
                        u => u.batchNo == batchNo 
                        && u.WareLocation!=null 
                        && u.WareLocation.WareArea.WareAreaClass.AreaClass == AreaClassType.ChengPinArea
                    , false, NanXingData_WMS.DaoUtils.DbMainSlave.Master);
            List<BingWareTrayLog> loglist= new List<BingWareTrayLog>();
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    foreach (WareLocation wareLocation in warelist)
                    {
                        UnBindTrayWarelocationWithoutTran(wareLocation, wareLocation.TrayState, loglist);
                    }
                    foreach (TrayState trayState in traystatelist)
                    {
                        UnBindTrayWarelocationWithoutTran(trayState.WareLocation, trayState, loglist);
                    }
                    if (loglist.Count > 0)
                    {
                        if (_stockRecordService == null)
                            _stockRecordService = new StockRecordService();
                        _wareLocationService.UpdateAll(warelist);
                        _trayStateService.UpdateAll(traystatelist);
                        _wareLocationService.SaveChanges();
                        _wareLocationService.SaveChanges();

                        int[] IDs = loglist.Select(u => u.trayStateID).ToArray();

                        List<TrayState> trayStates = _trayStateService.GetList(u=>IDs.Contains(u.ID)
                            , true, NanXingData_WMS.DaoUtils.DbMainSlave.Master);

                        foreach (BingWareTrayLog traylog in loglist)
                        {
                            if (traylog.trayStateID > 0)
                            {
                                TrayState trayState = trayStates.Find(u => u.ID == traylog.trayStateID);
                                _stockRecordService.AddHandStockRecord(trayState, traylog.changeWl, orderUser,
                                    DateTime.Now, false);
                            }
                        }
                    }
                    tran.Complete();
                    
                }
                catch(Exception ex)
                {
                    Logger.Default.Process(new Log(LevelType.Error,
                                  $"手动批量出库失败:失败批次{batchNo}:操作人员{orderUser}\r\n {ex.ToString()}"));
                    return false;
                }
                foreach (BingWareTrayLog traylog in loglist)
                    Logger.Default.Process(new Log(LevelType.Info,
                        $"{traylog.changeWl}:解绑货物{traylog.trayNo}:ID {traylog.trayStateID}"));
                return true;
            }
        }

    }
}

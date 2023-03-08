
using GeLi_Utils.Entity.MaPanJiStateEntity;
using GeLi_Utils.Helpers;
using GeLi_Utils.Services.WMS;
using GeLi_Utils.Utils.AGVUtils;
using GeLiData_WMS;
using GeLiData_WMS.Dao;
using GeLiData_WMSUtils;
using GeLiService_WMS.Entity.AGVOrderEntity;
using GeLiService_WMS.Entity.StockEntity;
using GeLiService_WMS.Services;
using GeLiService_WMS.Utils.ThreadUtils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using OrderResult = GeLi_Utils.Entity.AGVApiEntity.OrderResult;

namespace GeLiService_WMS.Threads.SameFloorThreads
{
    /// <summary>
    /// 检测总任务表的线程（姚）
    /// </summary>
    public class SameFloorRunThread
    {
        WareHouse _wareHouse;
        //ConcurrentQueue<AGVMissionInfo> concurrentQueue = new ConcurrentQueue<AGVMissionInfo>();
        
        AGVOrderHelper aGVOrderHelper;
        
        //string waitRun = "等待执行";
        public MyTask myTask;
        //启动线程前要传入仓库名
        public SameFloorRunThread(WareHouse wareHouse)
        {
            var AGVServerIP = ConfigurationManager.AppSettings["AGVIPAndPort"].ToString();
            _wareHouse = wareHouse;
            aGVOrderHelper = new AGVOrderHelper(AGVServerIP);
            //_agvMissionService = agvMissionService;
            myTask = new MyTask(new Action(Run),
                        3, true).StartTask();
        }

        public void Run()
        {
            AGVMissionService _agvMissionService = new AGVMissionService();
            MaPanJiInfoService maPanJiInfoService = new MaPanJiInfoService();
            MaPanJiInfo maPanJiInfo = new MaPanJiInfo();
            DateTime dtime = DateTime.Now.AddDays(-110);
            //查找当天前24小时同层的任务，提升机当作仓库库位
            List<AGVMissionInfo> list = _agvMissionService.GetIQueryable(u =>
                     u.OrderTime >= dtime && u.IsFloor == 0 //同层
                     && u.WHName == _wareHouse.WHName /*跟传入的仓库一致*/ && u.SendState== StockState.SendState_Group, //筛选发送状态为已分类的任务
                    true, GeLiData_WMSUtils.DbMainSlave.Master)
                .OrderBy(u=>u.ID).ToList();
            List<AGVMissionInfo> MaPanJilist = _agvMissionService.GetIQueryable(u =>
                    u.OrderTime >= dtime && u.IsFloor == 0 //同层
                    && u.WHName == _wareHouse.WHName /*跟传入的仓库一致*/ && u.Mark == MissionType.MoveToMaPanJi
                    && u.SendState == StockState.SendState_Success && u.RunState!= StockState.RunState_Success
                    && u.RunState != StockState.RunState_Error && u.RunState != StockState.RunState_Cancel 
                    && u.RunState != StockState.RunState_RunFail && u.RunState != StockState.RunState_SendFail, //筛选发送状态为已分类的任务
                   true, GeLiData_WMSUtils.DbMainSlave.Master)
               .OrderBy(u => u.ID).ToList();
           
            if (list.Count>0)//
            {
                foreach (AGVMissionInfo mission in list)
                {
                   
                    
                    if (mission.Mark==MissionType.MoveToMaPanJi)
                    {
                        continue;
                    }
                    OrderResult result = aGVOrderHelper.SendOrder(list[0]);
                    //Logger.Default.Process(new Log(LevelType.Info,"跨楼层任务执行："+ result.ToString()));

                    if (result.code == 200)
                        list[0].SendState = ResultStr.success;
                    // list[0].SendState = ResultStr.success;
                    //else
                    //    list[0].SendState = ResultStr.fail;
                }
                DataTable dataTable = _agvMissionService.ConvertToDataTable(list);
                _agvMissionService.UpdateMany(dataTable);
                
                var moveToMaPanJi= list.Where(u => u.Mark == MissionType.MoveToMaPanJi).FirstOrDefault();
                if (moveToMaPanJi != null&& MaPanJilist != null&& MaPanJilist.Count()==0)
                {
            

                    maPanJiInfo = maPanJiInfoService.GetList(u => u.MpjName == moveToMaPanJi.EndPosition,true,DbMainSlave.Master).FirstOrDefault(); 
                    if (maPanJiInfo != null&& maPanJiInfo.MaPanJiState != null)
                    {
                        
                        if (maPanJiInfo.MaPanJiState.Reserve1== MaPanJiStateSummarize.Idle||
                        maPanJiInfo.MaPanJiState.Reserve1 == MaPanJiStateSummarize.original)
                        {
                            MaPanJiHelper maPanJiHelper = new MaPanJiHelper(maPanJiInfo.MpjIp, maPanJiInfo.MpjPort);
                            bool trueorfalse = maPanJiHelper.SendMissionToMaPanJi(maPanJiInfo.MpjIp, maPanJiInfo.MpjPort);
                            if(trueorfalse==true)
                            {
                                moveToMaPanJi.SendState = ResultStr.success;
                                _agvMissionService.Update(moveToMaPanJi);
                                _agvMissionService.SaveChanges();
                            }
                                
                        }
                        
                    }
                }
                
                
                
            }

            var MaPanJilistGoTo = MaPanJilist.Where(u => u.RunState == StockState.RunState_AgvOutMaPan).FirstOrDefault();
            if (MaPanJilistGoTo != null )
            {
                maPanJiInfo = maPanJiInfoService.GetList(u => u.MpjName == MaPanJilistGoTo.EndPosition, true, DbMainSlave.Master).FirstOrDefault(); ;
                if (maPanJiInfo != null && maPanJiInfo.MaPanJiState != null)
                {
                    MaPanJiHelper maPanJiHelper = new MaPanJiHelper(maPanJiInfo.MpjIp, maPanJiInfo.MpjPort);
                    bool trueorfalse = maPanJiHelper.StartMaPan(maPanJiInfo.MpjIp,maPanJiInfo.MpjPort);
                    if (trueorfalse == true)
                    {
                        MaPanJilistGoTo.RunState = StockState.RunState_Success;
                        _agvMissionService.Update(MaPanJilistGoTo);
                        _agvMissionService.SaveChanges();
                    }
                }
            }
            //处理满盘任务
            List<AGVMissionInfo> MaPanJiGoOutlist = _agvMissionService.GetIQueryable(u =>
                   u.OrderTime >= dtime && u.IsFloor == 0 //同层
                   && u.WHName == _wareHouse.WHName /*跟传入的仓库一致*/ && u.Mark == MissionType.MoveOutMaPanJi
                   && u.SendState == StockState.SendState_Success && u.RunState != StockState.RunState_Success
                   && u.RunState != StockState.RunState_Error && u.RunState != StockState.RunState_Cancel
                   && u.RunState != StockState.RunState_RunFail && u.RunState != StockState.RunState_SendFail, //筛选发送状态为已分类的任务
                  true, GeLiData_WMSUtils.DbMainSlave.Master)
              .OrderBy(u => u.ID).ToList();

            var MaPanJiGoOut = MaPanJiGoOutlist.Where(u => u.RunState == StockState.RunState_AgvOutMaPan).FirstOrDefault();
            if (MaPanJiGoOutlist != null&& MaPanJiGoOut!=null)
            {
                

                maPanJiInfo = maPanJiInfoService.GetList(u => u.MpjName == MaPanJiGoOut.StartPosition, true, DbMainSlave.Master).FirstOrDefault(); ;
                if (maPanJiInfo != null)
                {
                    MaPanJiHelper maPanJiHelper = new MaPanJiHelper(maPanJiInfo.MpjIp, maPanJiInfo.MpjPort);
                    bool trueorfalse = maPanJiHelper.WriteTrayNumToZero();
                    if (trueorfalse == true)
                    {
                        MaPanJiGoOut.RunState = StockState.RunState_MaPanSet0;
                        _agvMissionService.Update(MaPanJiGoOut);
                        _agvMissionService.SaveChanges();
                    }

                }
            }

        }
       
    }
}

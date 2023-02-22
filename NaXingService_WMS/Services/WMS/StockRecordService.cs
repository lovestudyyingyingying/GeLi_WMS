using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Services.WMS;
using NanXingService_WMS.Threads.DiffFloorThreads;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services.WMS
{
    public class StockRecordService : DbBase<StockRecord>
    {
        //情况一：任务成功或任务失败，但是货物已搬起
        //成功：直接记录起点、终点、类型
        //失败：记录为 出仓---任务失败人工干预

        //情况二：手动出仓、手动进仓

        public bool AddStockRecord(AGVMissionInfo missionInfo
            , TrayState trayState, string oldWareLoca)
        {
            StockRecord stockRecord = GetStockRecord(missionInfo, trayState, oldWareLoca);
            Insert(stockRecord);
            return SaveChanges() > 0;
        }

        public bool AddHandStockRecord(TrayState trayState, string warelocation,
            string orderUser, DateTime orderTime, bool IsIn)
        {
            StockRecord stockRecord = GetStockRecordByHand(trayState, warelocation, orderUser,
                orderTime, IsIn);
            Insert(stockRecord);
            return SaveChanges() > 0;
        }


        private StockRecord GetStockRecord(AGVMissionInfo missionInfo
            , TrayState trayState, string oldWareLoca)
        {
            StockRecord stockRecord = new StockRecord();

            stockRecord.TrayNo = missionInfo.TrayNo;
            stockRecord.StartLocation = oldWareLoca;
            //同楼层或跨楼层的失败
            stockRecord.MissionNo = missionInfo.MissionNo;
            stockRecord.BatchNo = trayState.batchNo;
            stockRecord.ProName = trayState.proname;
            stockRecord.ProCount = trayState.OnlineCount;
            stockRecord.OrderTime = missionInfo.OrderTime ?? DateTime.Now;
            stockRecord.OrderUser = missionInfo.userId;
            stockRecord.RecordTime = DateTime.Now;
            string stockType = string.Empty;
            bool isMissionSuccess = missionInfo.RunState == StockState.RunState_Success;
            stockRecord.EndLocation = isMissionSuccess ?
                    missionInfo.EndPosition : string.Empty;
            if (missionInfo.IsFloor == 0)
            {
                //同楼层失败的时候，要看是否搬起货物，搬起货物则帮起点取消绑定，
                //有起点，无终点，类型为人工干预
                //没有搬起货物则是失败

                stockRecord.OrderAGV = missionInfo.AGVCarId;
                stockRecord.FinishTime = missionInfo.StateTime ?? DateTime.Now;

                stockRecord.StockTypeDesc = GetStockTypeDesc(missionInfo.Mark, true, ref stockType
                     , isMissionSuccess);
                stockRecord.StockType = stockType;
            }
            else
            {
                var twofloorTask = missionInfo.AGVMissionInfo_Floor.Find(
                    u => u.MissionNo.EndsWith(DiffFloorFactory.twoStr));

                stockRecord.OrderAGV = twofloorTask.AGVCarId;
                stockRecord.FinishTime = twofloorTask.StateTime ?? DateTime.Now;
                stockRecord.StockTypeDesc = GetStockTypeDesc(missionInfo.Mark, false, ref stockType
                    , isMissionSuccess);
                stockRecord.StockType = stockType;
            }

            //跨楼层任务/同楼层任务
            //任务成功、失败

            return stockRecord;
        }

        public StockRecord GetStockRecordByHand(TrayState trayState, string warelocation,
            string orderUser, DateTime orderTime, bool IsIn)
        {
            StockRecord stockRecord = new StockRecord();
            stockRecord.TrayNo = trayState.TrayNO;
            stockRecord.ProName = trayState.proname;
            stockRecord.BatchNo = trayState.batchNo;
            stockRecord.ProCount = trayState.OnlineCount;
            stockRecord.StartLocation = IsIn ? string.Empty : warelocation;
            stockRecord.EndLocation = IsIn ? warelocation : string.Empty;

            stockRecord.StockTypeDesc = IsIn ? StockTypeDesc.InstockType_HandControl.ToDescription() :
                StockTypeDesc.OutstockType_HandControl.ToDescription();
            stockRecord.StockType = IsIn ? StockType.In : StockType.Out;
            stockRecord.MissionNo = string.Empty;

            stockRecord.OrderTime = orderTime;
            stockRecord.FinishTime = orderTime;
            stockRecord.RecordTime = DateTime.Now;
            stockRecord.OrderUser = orderUser;
            stockRecord.OrderAGV = string.Empty;
            //跨楼层任务/同楼层任务
            //任务成功、失败

            return stockRecord;
        }

        private string GetStockTypeDesc(string mark, bool isSameFloor, ref string stockType,
            bool missionSuccess = true)
        {
            if (!missionSuccess)
            {
                stockType = StockType.Out;
                return StockTypeDesc.AGVMissionFail.ToDescription();
            }

            if (isSameFloor)
            {
                if (mark == MissionType.InstockType)
                {
                    stockType = StockType.In;
                    return StockTypeDesc.InstockType_SameFloor.ToDescription();
                }
                else if (mark == MissionType.OutstockType)
                {
                    stockType = StockType.Out;
                    return StockTypeDesc.OutstockType_SameFloor.ToDescription();
                }
                else if (mark == MissionType.MovestockType)
                {
                    stockType = StockType.Move;
                    return StockTypeDesc.MovestockType_SameFloor.ToDescription();
                }
            }
            else
            {
                if (mark == MissionType.InstockType)
                {
                    stockType = StockType.In;
                    return StockTypeDesc.InstockType_DiffFloor.ToDescription();
                }
                if (mark == MissionType.MoveOut_TSJ)
                {
                    stockType = StockType.Move;
                    return StockTypeDesc.OutstockType_DiffFloor.ToDescription();
                }
                else if (mark == MissionType.MovestockType)
                {
                    stockType = StockType.Move;
                    return StockTypeDesc.MovestockType_DiffFloor.ToDescription();
                }
            }
            return string.Empty;
        }
        
    }

    public class StockType
    {
        public static string In = "进仓";
        public static string Out = "出仓";
        public static string Move = "调拨";
    }

    public enum StockTypeDesc
    {
        [Description("同层进仓")]
        InstockType_SameFloor,
        [Description("同层出仓")]
        OutstockType_SameFloor,
        [Description("同层调拨")]
        MovestockType_SameFloor,

        [Description("跨楼层进仓")]
        InstockType_DiffFloor,
        [Description("跨楼层调拨-缓存")]
        OutstockType_DiffFloor,
        [Description("跨楼层调拨")]
        MovestockType_DiffFloor,
        [Description("手动出仓")]
        OutstockType_HandControl,
        [Description("手动进仓")]
        InstockType_HandControl,

        [Description("任务失败-人工干预")]
        AGVMissionFail

        
    }
}

public static class Extension
{

    public static string ToDescription(this StockTypeDesc myEnum)
    {
        Type type = typeof(StockTypeDesc);
        FieldInfo info = type.GetField(myEnum.ToString());
        DescriptionAttribute descriptionAttribute = info.GetCustomAttributes(typeof(DescriptionAttribute), true)[0] as DescriptionAttribute;
        if (descriptionAttribute != null)
            return descriptionAttribute.Description;
        else
            return type.ToString();
    }
}

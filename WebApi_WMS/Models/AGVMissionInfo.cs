//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WCSApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AGVMissionInfo
    {
        public int ID { get; set; }
        public string AreaClass { get; set; }
        public Nullable<int> SortIndex { get; set; }
        public Nullable<System.DateTime> OrderTime { get; set; }
        public string StockNo { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string RunState { get; set; }
        public string StateMsg { get; set; }
        public string SendState { get; set; }
        public string SendMsg { get; set; }
        public string MissionNo { get; set; }
        public string TrayNo { get; set; }
        public string Mark { get; set; }
        public Nullable<int> StockPlan_ID { get; set; }
        public string OrderGroupId { get; set; }
        public string AGVCarId { get; set; }
        public string userId { get; set; }
    
        public virtual StockPlan StockPlan { get; set; }
    }
}

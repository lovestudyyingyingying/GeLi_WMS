namespace GeLiData_WMS
{
    using GeLiData_WMSImp;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AGVMissionInfo")]
    public partial class AGVMissionInfo: AGVMissionBase
    {
        [Key]
        public new int ID { get; set; }
        /// <summary>
        /// 提升机名称，分类用
        /// </summary>
        [StringLength(20)]
        public string WHName { get; set; }

     

        [ForeignKey("MissionFloor_ID")]
        public virtual List<AGVMissionInfo_Floor> AGVMissionInfo_Floor { get; set; }
    }
}

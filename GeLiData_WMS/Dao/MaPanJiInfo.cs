using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiData_WMS.Dao
{
    [Table("MaPanJiInfo")]

    /// <summary>
    /// 码盘机信息
    /// </summary>
    public class MaPanJiInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(20)]
        public string MpjName { get; set; }

        [StringLength(25)]
        public string MpjIp { get; set; }

        public int MpjPort { get; set; }

        public DateTime InputTime { get; set; }

        [StringLength(200)]
        public string Floors { get; set; }

        [StringLength(30)]
        public string MpjPosition { get; set; }

        public bool IsOpen { get; set; }

        public bool IsError { get; set; }
        [StringLength(25)]
        public string AGVServerIP { get; set; }

        [StringLength(100)]
        public string Reserve1 { get; set; }
        [StringLength(100)]
        public string Reserve2 { get; set; }
        [StringLength(100)]
        public string Reserve3 { get; set; }
        [StringLength(100)]
        public string Reserve4 { get; set; }
        [StringLength(100)]
        public string Reserve5 { get; set; }

        public int? MaPanJiState_ID { get; set; }

        [ForeignKey("MaPanJiState_ID")]
        public virtual MaPanJiState MaPanJiState { get; set; }
    }
}

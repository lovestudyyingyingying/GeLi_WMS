using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiData_WMS.Dao
{

    [Table("ProcessTypeParam")]
    public class ProcessTypeParam
    {
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        [StringLength(50)]
        public string processName { get; set; }
        /// <summary>
        /// 起点楼层
        /// </summary>
        [StringLength(50)]
        public string startPosition { get; set; }
        /// <summary>
        /// 终点楼层
        /// </summary>
        [StringLength(50)]
        public string endPosition { get; set; }
        /// <summary>
        /// 起始区域
        /// </summary>
        [StringLength(50)]
        public string startArea { get; set; }
        /// <summary>
        /// 终点区域
        /// </summary>
        [StringLength(50)]
        public string endArea { get; set; }
        /// <summary>
        /// 冷管或热管
        /// </summary>
        [StringLength(50)]
        public string protype { get; set; }
        /// <summary>
        /// 点对点，点对区，区对区，区对点
        /// </summary>
        [StringLength(50)]
        public string missionType { get; set; }
        [StringLength(50)]
        public string nextArea { get; set; }
        [StringLength(50)]
        public string nextPosition { get; set; }
        /// <summary>
        /// 货物类型0，1; 0代表空托，1代表物料
        /// </summary>
        [StringLength(50)]
        public string goodType { get; set; }
        /// <summary>
        /// 移动类型：上线或者下线
        /// </summary>
        [StringLength(50)]
        public string moveType { get; set; }


        /// <summary>
        /// 起点备注
        /// </summary>
        [StringLength(50)]
        public string strartRemark { get; set; }


        /// <summary>
        /// 终点备注
        /// </summary>
        [StringLength(50)]
        public string endRemark { get; set; }
    }
}

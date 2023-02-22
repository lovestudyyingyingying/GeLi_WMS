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

        [StringLength(50)]
        public string processName { get; set; }
        [StringLength(50)]
        public string startPosition { get; set; }
        [StringLength(50)]
        public string endPosition { get; set; }
        [StringLength(50)]
        public string startArea { get; set; }
        [StringLength(50)]
        public string endArea { get; set; }
        [StringLength(50)]
        public string protype { get; set; }
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.PDAApiEntity
{
    public class WareLocationParamEntity
    {
        public string startPosition { get; set; }
        public string endPosition { get; set; }
        public string startArea { get; set; }
        public string endArea { get; set; }
        public string protype { get; set; }
        public string missionType { get; set; }
        public object nextArea { get; set; }
        public object nextPosition { get; set; }
    }
}

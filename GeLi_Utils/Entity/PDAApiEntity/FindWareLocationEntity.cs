using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.PDAApiEntity
{

    public class FindWareLocationEntity
    {
        public PostWarelocation[] startWareLocation { get; set; }
        public PostWarelocation[] endWareLocation { get; set; }
    }

    public class PostWarelocation
    {
        public string name { get; set; }
        public string state { get; set; }
    }

   

}

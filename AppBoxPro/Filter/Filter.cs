using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBoxPro
{
    public class Filter
    {
        public string Key { get; set; } //过滤的关键字  
        public string Value { get; set; } //过滤的值  
        public string Contract { get; set; }// 过滤的约束 比如：'<' '<=' '>' '>=' 'like'等  
    }
}

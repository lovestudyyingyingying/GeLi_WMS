using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.StockEntity
{
    public class TiShengJiMission
    {
        public string TsjName { get; set; }
        public List<string> Lie { get; set; }
        public int LieCount { get; set; }
        public Dictionary<string,int[]> LieIndexs { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetType().Name}:[\r\n");
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(this.GetType()))
            {
                //Type proType = pd.PropertyType.Name == "Nullable`1" ? pd.PropertyType.GenericTypeArguments[0] : pd.PropertyType;
                sb.Append($"{pd.Name}:{pd.GetValue(this)}\r\n");
            }
            sb.Append($"]\r\n");
            return sb.ToString();
        }

    }
}

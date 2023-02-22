using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity.ProductEntity
{
    public class ProKanBanChart
    {
        public string CheJianName { get; set; }
        public List<string> XData { get; set; }
        public List<string> YData1 { get; set; }
        public List<string> YData2 { get; set; }

        public ProKanBanChart(string CheJianName)
        {
            this.CheJianName=CheJianName;
            XData = new List<string>();
            YData1 = new List<string>();
            YData2 = new List<string>();

        }

    }
}

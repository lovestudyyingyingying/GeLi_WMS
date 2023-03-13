using GeLi_Utils.Entity.PDAApiEntity;
using GeLiService_WMS.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PdaWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }



        private void btnGetPoint_Click(object sender, RoutedEventArgs e)
        {
            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();
            string url = "http://" + baseUrl + "/api/PDA/instocks/GetInStartWL";
            HttpUtils httpUtils = new HttpUtils();
            if (cbxProcessNameFirst.Text == "")
                return;
           
            object obj = new { processName = cbxProcessNameFirst.Text, protype = cbxProtyName.Text,gongXu=cbxGongXu.Text,production=cbxProduct.Text };
            string result = httpUtils.HttpPost(url, obj, null);

            if (string.IsNullOrEmpty(result))
                return;
            var resultobj=JsonConvert.DeserializeObject<GetPiontResult>(result);

            if(!resultobj.success)
            {
                MessageBox.Show(resultobj.message);
                return;
            }    
            else
            {
                cbxStart.ItemsSource = resultobj.data.startWareLocation.Select(u=>u.name);
                if(cbxStart.ItemsSource != null)
                    cbxStart.SelectedIndex = 0;
                cbxEnd.ItemsSource = resultobj.data.endWareLocation.Select(u=>u.name);
                  if (cbxEnd.ItemsSource != null)
                     cbxEnd.SelectedIndex = 0;
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();
            string url = "http://" + baseUrl + "/api/PDA/moves/DiaoBoOrder";
            if (cbxStart.Text == "")
                return;
            if (cbxEnd.Text == "")
                return;
            object obj = new { startPo = cbxStart.Text, endPo = cbxEnd.Text , nowPre ="admin", processName =cbxProcessNameFirst.Text,gongXu = cbxGongXu.Text, prosn = cbxProduct.Text, production = cbxProduct.Text };
            HttpUtils httpUtils = new HttpUtils();
            string result = httpUtils.HttpPost(url, obj, null);
            var resultobj =  JsonConvert.DeserializeObject<GetPiontResult>(result);
            if(resultobj.success)
            {
                MessageBox.Show("下发成功");
            }
            else
            {
                MessageBox.Show("下发失败" + resultobj.message);
            }
        }

        private void btnChaSend_Click(object sender, RoutedEventArgs e)
        {
            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();
            string url = "http://" + baseUrl + "/api/PDA/moves/DiaoBoOrder";
            if (cbxStart.Text == "")
                return;
            if (cbxEnd.Text == "")
                return;
            object obj = new { startPo = cbxStart.Text, endPo = cbxEnd.Text, nowPre = "admin", processName = cbxProcessNameFirst.Text, isPriority="1", gongXu = cbxGongXu.Text, prosn = cbxProduct.Text };
            HttpUtils httpUtils = new HttpUtils();
            string result = httpUtils.HttpPost(url, obj, null);
            var resultobj = JsonConvert.DeserializeObject<GetPiontResult>(result);
            if (resultobj.success)
            {
                MessageBox.Show("下发成功");
            }
            else
            {
                MessageBox.Show("下发失败" + resultobj.message);
            }
        }
    }


    public class GetPiontResult
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Startwarelocation[] startWareLocation { get; set; }
        public Endwarelocation[] endWareLocation { get; set; }
    }

    public class Startwarelocation
    {
        public string name { get; set; }
        public string state { get; set; }
    }

    public class Endwarelocation
    {
        public string name { get; set; }
        public string state { get; set; }
    }

}

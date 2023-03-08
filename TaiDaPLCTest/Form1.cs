using System;

using System.Globalization;

using System.Windows.Forms;
using GeLi_Utils.Helpers;
using GeLiService_WMS;
using GeLiService_WMS.Threads;
using GeLiService_WMS.Utils.ThreadUtils;
using HslCommunication;
using HslCommunication.ModBus;

namespace TaiDaPLCTest
{
    public partial class Form1 : Form
    {
        private ModbusTCPDeltaHelper DelTaPLC;
        /// <summary>
        /// 发送间隔？
        /// 
        /// </summary>
        private int connectionUpdateTime = 100;
        public static bool IsConnected { get; set; }
        public Form1()
        {
            InitializeComponent();
        }
      

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                MaPanJiHelper maPanJiHelper = new MaPanJiHelper(tbxIP.Text, 502);
                //DelTaPLC = new ModbusTCPDeltaHelper(tbxIP.Text, 502, connectionUpdateTime);
                //DelTaPLC.SetConnectionStatus += (bool connectresult) => { IsConnected = connectresult; };
                //if(DelTaPLC.IsClientConnected)
                //{
                //    MessageBox.Show("ok");

                //}
                //else
                //{
                //    MessageBox.Show("no ok");
                //}
                //    //MessageBox.Show("连接成功");
       
            }
            catch
            {
                MessageBox.Show("连接失败");
            }
        }


        //this method needs revision, because it works only M, D and T registers
        //(see Docs/AH-EMC_Modbus_Addresses.pdf)
        private ushort GetAddressIntValue(string startAddress)
        {
            int intValue = 0;
            string mark = startAddress.Substring(0, 1);
            string address = startAddress.Substring(1);

            switch (mark)
            {
                case "M":
                    intValue = int.Parse(address) + 2048;
                    break;
                case "D":
                    intValue = int.Parse(address) + 0;
                    break;
                case "T":
                    intValue = int.Parse(address) + 57344;
                    break;
                case "C":
                    intValue = int.Parse(address) + 3584;
                    break;
            }

            string hex = intValue.ToString("X4");
            return ushort.Parse(hex, NumberStyles.HexNumber);
        }

        private void btnReadOne_Click(object sender, EventArgs e)
        {
            string startAddress = "M350";
            tbxResult.Clear();
           
            ModbusTcpNet modbusTcpNet = new ModbusTcpNet(tbxIP.Text);
            OperateResult result = modbusTcpNet.ConnectServer();
            if (!result.IsSuccess)
            {
                MessageBox.Show("连接失败！");
                return;
            }

           
            var DeltaToModBus =  GetAddressIntValue(startAddress);
            var r = modbusTcpNet.ReadCoil(DeltaToModBus.ToString(), 8);

            for (int i = 0; i < r.Content.Length; i++)
            {
                tbxResult.AppendText(r.Content[i].ToString()+"\r\n");

            }
            //string a = DelTaPLC.ReadBoolean(startAddress);
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            DelTaPLC.WriteBoolean("M350",true);
        }

        private void btnGetNum_Click(object sender, EventArgs e)
        {
          var result =   DelTaPLC.ReadDecimal("C216");
            tbxResult.Text = result.ToString();
          
        }

        private void btnHslModbus_Click(object sender, EventArgs e)
        {

            ModbusTcpNet modbusTcpNet = new ModbusTcpNet(tbxIP.Text);
            OperateResult result = modbusTcpNet.ConnectServer();
            if (!result.IsSuccess)
            {
                MessageBox.Show("连接失败！");
                return;
            }

            tbxResult.Text = modbusTcpNet.ReadInt32(GetAddressIntValue("C216").ToString()).Content.ToString() ;
        }

        private void btnWriteSingle_Click(object sender, EventArgs e)
        {
            ModbusTcpNet modbusTcpNet = new ModbusTcpNet(tbxIP.Text);
            OperateResult result = modbusTcpNet.ConnectServer();
            if (!result.IsSuccess)
            {
                MessageBox.Show("连接失败！");
                return;
            }
            var needwrite = GetAddressIntValue(tbxAddress.Text).ToString();

            var r = modbusTcpNet.WriteCoil(needwrite, true);
            MessageBox.Show(r.Message);

        }

        private void btnwriteFalse_Click(object sender, EventArgs e)
        {
            ModbusTcpNet modbusTcpNet = new ModbusTcpNet(tbxIP.Text);
            OperateResult result = modbusTcpNet.ConnectServer();
            if (!result.IsSuccess)
            {
                MessageBox.Show("连接失败！");
                return;
            }
            var needwrite = GetAddressIntValue(tbxAddress.Text).ToString();

            var r = modbusTcpNet.WriteCoil(needwrite, false);
            MessageBox.Show(r.Message);
        }

        private void btncontitnue_Click(object sender, EventArgs e)
        {
            
            ModbusTcpNet modbusTcpNet = new ModbusTcpNet(tbxIP.Text);
            OperateResult result = modbusTcpNet.ConnectServer();
            if (!result.IsSuccess)
            {
                MessageBox.Show("连接失败！");
                return;
            }
            btncontitnue.Text = "关闭读取";
            MyTask myTask = new MyTask(() =>
            {
                tbxResult.Invoke(new Action(() => tbxResult.Clear()));
                var DeltaToModBus = GetAddressIntValue("M350");
                var r = modbusTcpNet.ReadCoil(DeltaToModBus.ToString(), 8);

                for (int i = 0; i < r.Content.Length; i++)
                {
                    tbxResult.Invoke(new Action(() => tbxResult.AppendText(r.Content[i].ToString() + "\r\n")));

                }
                tbxResult.Invoke(new Action(() => tbxResult.AppendText(modbusTcpNet.ReadInt32(GetAddressIntValue("C216").ToString()).Content.ToString())));
            }, 1, true);
            myTask.StartTask();
        }

        private void btnHelperTest_Click(object sender, EventArgs e)
        {
            MaPanJiHelper maPanJiHelper = new MaPanJiHelper("121.201.123.117", 502);
            MyTask myTask = new MyTask(maPanJiHelper.GetAndSaveState, 3, true);
            myTask.StartTask();
            MyTask myTask1 = new MyTask(maPanJiHelper.CheckAndSaveError, 10, true);
            myTask1.StartTask();
            //maPanJiHelper.GetAndSaveState();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GroupMissionThread groupMissionThread = new GroupMissionThread();
            groupMissionThread.Control();
        }
    }
}

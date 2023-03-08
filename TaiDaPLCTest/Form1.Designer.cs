namespace TaiDaPLCTest
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tbxIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbxResult = new System.Windows.Forms.TextBox();
            this.btnReadOne = new System.Windows.Forms.Button();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnGetNum = new System.Windows.Forms.Button();
            this.btnHslModbus = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btncontitnue = new System.Windows.Forms.Button();
            this.btnwriteFalse = new System.Windows.Forms.Button();
            this.btnWriteSingle = new System.Windows.Forms.Button();
            this.tbxAddress = new System.Windows.Forms.TextBox();
            this.btnHelperTest = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbxIP
            // 
            this.tbxIP.Location = new System.Drawing.Point(107, 61);
            this.tbxIP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbxIP.Name = "tbxIP";
            this.tbxIP.Size = new System.Drawing.Size(220, 28);
            this.tbxIP.TabIndex = 0;
            this.tbxIP.Text = "192.168.1.5";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(243, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(410, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "广东宏正自动识别有限公司测试项目";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "IP地址：";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(336, 61);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(112, 35);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // tbxResult
            // 
            this.tbxResult.Location = new System.Drawing.Point(22, 131);
            this.tbxResult.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbxResult.Multiline = true;
            this.tbxResult.Name = "tbxResult";
            this.tbxResult.Size = new System.Drawing.Size(424, 178);
            this.tbxResult.TabIndex = 4;
            // 
            // btnReadOne
            // 
            this.btnReadOne.Location = new System.Drawing.Point(8, 30);
            this.btnReadOne.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReadOne.Name = "btnReadOne";
            this.btnReadOne.Size = new System.Drawing.Size(153, 35);
            this.btnReadOne.TabIndex = 5;
            this.btnReadOne.Text = "读取一次";
            this.btnReadOne.UseVisualStyleBackColor = true;
            this.btnReadOne.Click += new System.EventHandler(this.btnReadOne_Click);
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(458, 56);
            this.btnWrite.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(112, 35);
            this.btnWrite.TabIndex = 6;
            this.btnWrite.Text = "写入M350";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnGetNum
            // 
            this.btnGetNum.Location = new System.Drawing.Point(568, 56);
            this.btnGetNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGetNum.Name = "btnGetNum";
            this.btnGetNum.Size = new System.Drawing.Size(112, 35);
            this.btnGetNum.TabIndex = 7;
            this.btnGetNum.Text = "读取数量";
            this.btnGetNum.UseVisualStyleBackColor = true;
            this.btnGetNum.Click += new System.EventHandler(this.btnGetNum_Click);
            // 
            // btnHslModbus
            // 
            this.btnHslModbus.Location = new System.Drawing.Point(8, 73);
            this.btnHslModbus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnHslModbus.Name = "btnHslModbus";
            this.btnHslModbus.Size = new System.Drawing.Size(153, 121);
            this.btnHslModbus.TabIndex = 8;
            this.btnHslModbus.Text = "HslModbus读取板内数量";
            this.btnHslModbus.UseVisualStyleBackColor = true;
            this.btnHslModbus.Click += new System.EventHandler(this.btnHslModbus_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btncontitnue);
            this.groupBox1.Controls.Add(this.btnwriteFalse);
            this.groupBox1.Controls.Add(this.btnWriteSingle);
            this.groupBox1.Controls.Add(this.tbxAddress);
            this.groupBox1.Controls.Add(this.btnReadOne);
            this.groupBox1.Controls.Add(this.btnHslModbus);
            this.groupBox1.Location = new System.Drawing.Point(458, 131);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(300, 341);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // btncontitnue
            // 
            this.btncontitnue.Location = new System.Drawing.Point(170, 30);
            this.btncontitnue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btncontitnue.Name = "btncontitnue";
            this.btncontitnue.Size = new System.Drawing.Size(83, 164);
            this.btncontitnue.TabIndex = 12;
            this.btncontitnue.Text = "连续读取";
            this.btncontitnue.UseVisualStyleBackColor = true;
            this.btncontitnue.Click += new System.EventHandler(this.btncontitnue_Click);
            // 
            // btnwriteFalse
            // 
            this.btnwriteFalse.Location = new System.Drawing.Point(169, 251);
            this.btnwriteFalse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnwriteFalse.Name = "btnwriteFalse";
            this.btnwriteFalse.Size = new System.Drawing.Size(104, 37);
            this.btnwriteFalse.TabIndex = 11;
            this.btnwriteFalse.Text = "写入失败";
            this.btnwriteFalse.UseVisualStyleBackColor = true;
            this.btnwriteFalse.Click += new System.EventHandler(this.btnwriteFalse_Click);
            // 
            // btnWriteSingle
            // 
            this.btnWriteSingle.Location = new System.Drawing.Point(169, 204);
            this.btnWriteSingle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnWriteSingle.Name = "btnWriteSingle";
            this.btnWriteSingle.Size = new System.Drawing.Size(104, 40);
            this.btnWriteSingle.TabIndex = 10;
            this.btnWriteSingle.Text = "写入成功";
            this.btnWriteSingle.UseVisualStyleBackColor = true;
            this.btnWriteSingle.Click += new System.EventHandler(this.btnWriteSingle_Click);
            // 
            // tbxAddress
            // 
            this.tbxAddress.Location = new System.Drawing.Point(8, 203);
            this.tbxAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxAddress.Name = "tbxAddress";
            this.tbxAddress.Size = new System.Drawing.Size(152, 28);
            this.tbxAddress.TabIndex = 9;
            // 
            // btnHelperTest
            // 
            this.btnHelperTest.Location = new System.Drawing.Point(161, 390);
            this.btnHelperTest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnHelperTest.Name = "btnHelperTest";
            this.btnHelperTest.Size = new System.Drawing.Size(114, 82);
            this.btnHelperTest.TabIndex = 10;
            this.btnHelperTest.Text = "Helper测试";
            this.btnHelperTest.UseVisualStyleBackColor = true;
            this.btnHelperTest.Click += new System.EventHandler(this.btnHelperTest_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(45, 401);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 61);
            this.button1.TabIndex = 11;
            this.button1.Text = "WMS线程测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 540);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnHelperTest);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnGetNum);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.tbxResult);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxIP);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox tbxResult;
        private System.Windows.Forms.Button btnReadOne;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnGetNum;
        private System.Windows.Forms.Button btnHslModbus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnWriteSingle;
        private System.Windows.Forms.TextBox tbxAddress;
        private System.Windows.Forms.Button btnwriteFalse;
        private System.Windows.Forms.Button btncontitnue;
        private System.Windows.Forms.Button btnHelperTest;
        private System.Windows.Forms.Button button1;
    }
}


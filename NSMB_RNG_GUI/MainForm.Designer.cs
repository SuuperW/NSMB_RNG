using System.Windows.Forms;

namespace NSMB_RNG_GUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtMAC = new System.Windows.Forms.TextBox();
            this.pbxMAC = new System.Windows.Forms.PictureBox();
            this.chkMini = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFirst7 = new System.Windows.Forms.TextBox();
            this.pbxTile11 = new System.Windows.Forms.PictureBox();
            this.pbxTile12 = new System.Windows.Forms.PictureBox();
            this.pbxTile13 = new System.Windows.Forms.PictureBox();
            this.pbxTile14 = new System.Windows.Forms.PictureBox();
            this.pbxTile15 = new System.Windows.Forms.PictureBox();
            this.pbxTile16 = new System.Windows.Forms.PictureBox();
            this.pbxTile17 = new System.Windows.Forms.PictureBox();
            this.pbxTile1End = new System.Windows.Forms.PictureBox();
            this.cbxSystem = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFirstTooLong = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.dtpTime = new System.Windows.Forms.DateTimePicker();
            this.lblMatch = new System.Windows.Forms.Label();
            this.pbxTile21 = new System.Windows.Forms.PictureBox();
            this.pbxTile23 = new System.Windows.Forms.PictureBox();
            this.pbxTile25 = new System.Windows.Forms.PictureBox();
            this.pbxTile22 = new System.Windows.Forms.PictureBox();
            this.pbxTile24 = new System.Windows.Forms.PictureBox();
            this.pbxTile26 = new System.Windows.Forms.PictureBox();
            this.pbxTile27 = new System.Windows.Forms.PictureBox();
            this.pbxTile28 = new System.Windows.Forms.PictureBox();
            this.pbxTile29 = new System.Windows.Forms.PictureBox();
            this.pbxTile210 = new System.Windows.Forms.PictureBox();
            this.pbxTile211 = new System.Windows.Forms.PictureBox();
            this.pbxTile2End = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSecondRow = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbxMAC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile1End)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile29)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile210)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile211)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile2End)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "MAC address:";
            // 
            // txtMAC
            // 
            this.txtMAC.Location = new System.Drawing.Point(98, 12);
            this.txtMAC.Name = "txtMAC";
            this.txtMAC.Size = new System.Drawing.Size(100, 23);
            this.txtMAC.TabIndex = 1;
            this.txtMAC.TextChanged += new System.EventHandler(this.txtMAC_TextChanged);
            // 
            // pbxMAC
            // 
            this.pbxMAC.Location = new System.Drawing.Point(204, 14);
            this.pbxMAC.Name = "pbxMAC";
            this.pbxMAC.Size = new System.Drawing.Size(20, 20);
            this.pbxMAC.TabIndex = 2;
            this.pbxMAC.TabStop = false;
            // 
            // chkMini
            // 
            this.chkMini.AutoSize = true;
            this.chkMini.Location = new System.Drawing.Point(299, 43);
            this.chkMini.Name = "chkMini";
            this.chkMini.Size = new System.Drawing.Size(81, 19);
            this.chkMini.TabIndex = 4;
            this.chkMini.Text = "mini route";
            this.chkMini.UseVisualStyleBackColor = true;
            this.chkMini.CheckedChanged += new System.EventHandler(this.chkMini_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "First 7 tiles:";
            // 
            // txtFirst7
            // 
            this.txtFirst7.Location = new System.Drawing.Point(91, 73);
            this.txtFirst7.Name = "txtFirst7";
            this.txtFirst7.Size = new System.Drawing.Size(94, 23);
            this.txtFirst7.TabIndex = 9;
            this.txtFirst7.TextChanged += new System.EventHandler(this.txtFirst7_TextChanged);
            this.txtFirst7.Enter += new System.EventHandler(this.txtFirst7_Enter);
            // 
            // pbxTile11
            // 
            this.pbxTile11.BackColor = System.Drawing.Color.Red;
            this.pbxTile11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile11.Location = new System.Drawing.Point(12, 102);
            this.pbxTile11.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile11.Name = "pbxTile11";
            this.pbxTile11.Size = new System.Drawing.Size(32, 32);
            this.pbxTile11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile11.TabIndex = 6;
            this.pbxTile11.TabStop = false;
            this.pbxTile11.Visible = false;
            // 
            // pbxTile12
            // 
            this.pbxTile12.BackColor = System.Drawing.Color.Red;
            this.pbxTile12.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile12.Location = new System.Drawing.Point(44, 102);
            this.pbxTile12.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile12.Name = "pbxTile12";
            this.pbxTile12.Size = new System.Drawing.Size(32, 32);
            this.pbxTile12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile12.TabIndex = 6;
            this.pbxTile12.TabStop = false;
            this.pbxTile12.Visible = false;
            // 
            // pbxTile13
            // 
            this.pbxTile13.BackColor = System.Drawing.Color.Red;
            this.pbxTile13.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile13.Location = new System.Drawing.Point(76, 102);
            this.pbxTile13.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile13.Name = "pbxTile13";
            this.pbxTile13.Size = new System.Drawing.Size(32, 32);
            this.pbxTile13.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile13.TabIndex = 6;
            this.pbxTile13.TabStop = false;
            this.pbxTile13.Visible = false;
            // 
            // pbxTile14
            // 
            this.pbxTile14.BackColor = System.Drawing.Color.Red;
            this.pbxTile14.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile14.Location = new System.Drawing.Point(108, 102);
            this.pbxTile14.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile14.Name = "pbxTile14";
            this.pbxTile14.Size = new System.Drawing.Size(32, 32);
            this.pbxTile14.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile14.TabIndex = 6;
            this.pbxTile14.TabStop = false;
            this.pbxTile14.Visible = false;
            // 
            // pbxTile15
            // 
            this.pbxTile15.BackColor = System.Drawing.Color.Red;
            this.pbxTile15.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile15.Location = new System.Drawing.Point(140, 102);
            this.pbxTile15.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile15.Name = "pbxTile15";
            this.pbxTile15.Size = new System.Drawing.Size(32, 32);
            this.pbxTile15.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile15.TabIndex = 6;
            this.pbxTile15.TabStop = false;
            this.pbxTile15.Visible = false;
            // 
            // pbxTile16
            // 
            this.pbxTile16.BackColor = System.Drawing.Color.Red;
            this.pbxTile16.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile16.Location = new System.Drawing.Point(172, 102);
            this.pbxTile16.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile16.Name = "pbxTile16";
            this.pbxTile16.Size = new System.Drawing.Size(32, 32);
            this.pbxTile16.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile16.TabIndex = 6;
            this.pbxTile16.TabStop = false;
            this.pbxTile16.Visible = false;
            // 
            // pbxTile17
            // 
            this.pbxTile17.BackColor = System.Drawing.Color.Red;
            this.pbxTile17.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile17.Location = new System.Drawing.Point(204, 102);
            this.pbxTile17.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile17.Name = "pbxTile17";
            this.pbxTile17.Size = new System.Drawing.Size(32, 32);
            this.pbxTile17.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile17.TabIndex = 6;
            this.pbxTile17.TabStop = false;
            this.pbxTile17.Visible = false;
            // 
            // pbxTile1End
            // 
            this.pbxTile1End.BackColor = System.Drawing.Color.Red;
            this.pbxTile1End.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile1End.Location = new System.Drawing.Point(236, 102);
            this.pbxTile1End.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile1End.Name = "pbxTile1End";
            this.pbxTile1End.Size = new System.Drawing.Size(32, 32);
            this.pbxTile1End.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile1End.TabIndex = 6;
            this.pbxTile1End.TabStop = false;
            this.pbxTile1End.Visible = false;
            // 
            // cbxSystem
            // 
            this.cbxSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSystem.FormattingEnabled = true;
            this.cbxSystem.Items.AddRange(new object[] {
            "other"});
            this.cbxSystem.Location = new System.Drawing.Point(296, 12);
            this.cbxSystem.Name = "cbxSystem";
            this.cbxSystem.Size = new System.Drawing.Size(85, 23);
            this.cbxSystem.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(242, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "System:";
            // 
            // lblFirstTooLong
            // 
            this.lblFirstTooLong.AutoSize = true;
            this.lblFirstTooLong.Location = new System.Drawing.Point(298, 110);
            this.lblFirstTooLong.Name = "lblFirstTooLong";
            this.lblFirstTooLong.Size = new System.Drawing.Size(82, 15);
            this.lblFirstTooLong.TabIndex = 12;
            this.lblFirstTooLong.Text = "too many tiles";
            this.lblFirstTooLong.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "Date/Time:";
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(84, 41);
            this.dtpDate.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            this.dtpDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(99, 23);
            this.dtpDate.TabIndex = 6;
            this.dtpDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpDate.Leave += new System.EventHandler(this.dtpDateTime_Leave);
            // 
            // dtpTime
            // 
            this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpTime.Location = new System.Drawing.Point(189, 41);
            this.dtpTime.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            this.dtpTime.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpTime.Name = "dtpTime";
            this.dtpTime.ShowUpDown = true;
            this.dtpTime.Size = new System.Drawing.Size(99, 23);
            this.dtpTime.TabIndex = 7;
            this.dtpTime.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpTime.Leave += new System.EventHandler(this.dtpDateTime_Leave);
            // 
            // lblMatch
            // 
            this.lblMatch.AutoSize = true;
            this.lblMatch.Location = new System.Drawing.Point(12, 172);
            this.lblMatch.Name = "lblMatch";
            this.lblMatch.Size = new System.Drawing.Size(41, 15);
            this.lblMatch.TabIndex = 13;
            this.lblMatch.Text = "match";
            this.lblMatch.Visible = false;
            // 
            // pbxTile21
            // 
            this.pbxTile21.BackColor = System.Drawing.Color.Red;
            this.pbxTile21.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile21.Location = new System.Drawing.Point(12, 134);
            this.pbxTile21.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile21.Name = "pbxTile21";
            this.pbxTile21.Size = new System.Drawing.Size(32, 32);
            this.pbxTile21.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile21.TabIndex = 6;
            this.pbxTile21.TabStop = false;
            this.pbxTile21.Visible = false;
            // 
            // pbxTile23
            // 
            this.pbxTile23.BackColor = System.Drawing.Color.Red;
            this.pbxTile23.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile23.Location = new System.Drawing.Point(76, 134);
            this.pbxTile23.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile23.Name = "pbxTile23";
            this.pbxTile23.Size = new System.Drawing.Size(32, 32);
            this.pbxTile23.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile23.TabIndex = 6;
            this.pbxTile23.TabStop = false;
            this.pbxTile23.Visible = false;
            // 
            // pbxTile25
            // 
            this.pbxTile25.BackColor = System.Drawing.Color.Red;
            this.pbxTile25.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile25.Location = new System.Drawing.Point(140, 134);
            this.pbxTile25.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile25.Name = "pbxTile25";
            this.pbxTile25.Size = new System.Drawing.Size(32, 32);
            this.pbxTile25.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile25.TabIndex = 6;
            this.pbxTile25.TabStop = false;
            this.pbxTile25.Visible = false;
            // 
            // pbxTile22
            // 
            this.pbxTile22.BackColor = System.Drawing.Color.Red;
            this.pbxTile22.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile22.Location = new System.Drawing.Point(44, 134);
            this.pbxTile22.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile22.Name = "pbxTile22";
            this.pbxTile22.Size = new System.Drawing.Size(32, 32);
            this.pbxTile22.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile22.TabIndex = 6;
            this.pbxTile22.TabStop = false;
            this.pbxTile22.Visible = false;
            // 
            // pbxTile24
            // 
            this.pbxTile24.BackColor = System.Drawing.Color.Red;
            this.pbxTile24.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile24.Location = new System.Drawing.Point(108, 134);
            this.pbxTile24.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile24.Name = "pbxTile24";
            this.pbxTile24.Size = new System.Drawing.Size(32, 32);
            this.pbxTile24.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile24.TabIndex = 6;
            this.pbxTile24.TabStop = false;
            this.pbxTile24.Visible = false;
            // 
            // pbxTile26
            // 
            this.pbxTile26.BackColor = System.Drawing.Color.Red;
            this.pbxTile26.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile26.Location = new System.Drawing.Point(172, 134);
            this.pbxTile26.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile26.Name = "pbxTile26";
            this.pbxTile26.Size = new System.Drawing.Size(32, 32);
            this.pbxTile26.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile26.TabIndex = 6;
            this.pbxTile26.TabStop = false;
            this.pbxTile26.Visible = false;
            // 
            // pbxTile27
            // 
            this.pbxTile27.BackColor = System.Drawing.Color.Red;
            this.pbxTile27.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile27.Location = new System.Drawing.Point(204, 134);
            this.pbxTile27.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile27.Name = "pbxTile27";
            this.pbxTile27.Size = new System.Drawing.Size(32, 32);
            this.pbxTile27.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile27.TabIndex = 6;
            this.pbxTile27.TabStop = false;
            this.pbxTile27.Visible = false;
            // 
            // pbxTile28
            // 
            this.pbxTile28.BackColor = System.Drawing.Color.Red;
            this.pbxTile28.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile28.Location = new System.Drawing.Point(236, 134);
            this.pbxTile28.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile28.Name = "pbxTile28";
            this.pbxTile28.Size = new System.Drawing.Size(32, 32);
            this.pbxTile28.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile28.TabIndex = 6;
            this.pbxTile28.TabStop = false;
            this.pbxTile28.Visible = false;
            // 
            // pbxTile29
            // 
            this.pbxTile29.BackColor = System.Drawing.Color.Red;
            this.pbxTile29.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile29.Location = new System.Drawing.Point(268, 134);
            this.pbxTile29.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile29.Name = "pbxTile29";
            this.pbxTile29.Size = new System.Drawing.Size(32, 32);
            this.pbxTile29.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile29.TabIndex = 6;
            this.pbxTile29.TabStop = false;
            this.pbxTile29.Visible = false;
            // 
            // pbxTile210
            // 
            this.pbxTile210.BackColor = System.Drawing.Color.Red;
            this.pbxTile210.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile210.Location = new System.Drawing.Point(300, 134);
            this.pbxTile210.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile210.Name = "pbxTile210";
            this.pbxTile210.Size = new System.Drawing.Size(32, 32);
            this.pbxTile210.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile210.TabIndex = 6;
            this.pbxTile210.TabStop = false;
            this.pbxTile210.Visible = false;
            // 
            // pbxTile211
            // 
            this.pbxTile211.BackColor = System.Drawing.Color.Red;
            this.pbxTile211.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile211.Location = new System.Drawing.Point(332, 134);
            this.pbxTile211.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile211.Name = "pbxTile211";
            this.pbxTile211.Size = new System.Drawing.Size(32, 32);
            this.pbxTile211.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile211.TabIndex = 6;
            this.pbxTile211.TabStop = false;
            this.pbxTile211.Visible = false;
            // 
            // pbxTile2End
            // 
            this.pbxTile2End.BackColor = System.Drawing.Color.Red;
            this.pbxTile2End.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile2End.Location = new System.Drawing.Point(364, 134);
            this.pbxTile2End.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pbxTile2End.Name = "pbxTile2End";
            this.pbxTile2End.Size = new System.Drawing.Size(32, 32);
            this.pbxTile2End.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTile2End.TabIndex = 6;
            this.pbxTile2End.TabStop = false;
            this.pbxTile2End.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(197, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "Second row:";
            // 
            // txtSecondRow
            // 
            this.txtSecondRow.Enabled = false;
            this.txtSecondRow.Location = new System.Drawing.Point(275, 73);
            this.txtSecondRow.Name = "txtSecondRow";
            this.txtSecondRow.Size = new System.Drawing.Size(121, 23);
            this.txtSecondRow.TabIndex = 11;
            this.txtSecondRow.TextChanged += new System.EventHandler(this.txtSecondRow_TextChanged);
            this.txtSecondRow.Enter += new System.EventHandler(this.txtFirst7_Enter);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 207);
            this.Controls.Add(this.lblMatch);
            this.Controls.Add(this.dtpTime);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblFirstTooLong);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbxSystem);
            this.Controls.Add(this.pbxTile2End);
            this.Controls.Add(this.pbxTile28);
            this.Controls.Add(this.pbxTile1End);
            this.Controls.Add(this.pbxTile211);
            this.Controls.Add(this.pbxTile27);
            this.Controls.Add(this.pbxTile17);
            this.Controls.Add(this.pbxTile210);
            this.Controls.Add(this.pbxTile26);
            this.Controls.Add(this.pbxTile16);
            this.Controls.Add(this.pbxTile24);
            this.Controls.Add(this.pbxTile14);
            this.Controls.Add(this.pbxTile22);
            this.Controls.Add(this.pbxTile12);
            this.Controls.Add(this.pbxTile29);
            this.Controls.Add(this.pbxTile25);
            this.Controls.Add(this.pbxTile15);
            this.Controls.Add(this.pbxTile23);
            this.Controls.Add(this.pbxTile13);
            this.Controls.Add(this.pbxTile21);
            this.Controls.Add(this.pbxTile11);
            this.Controls.Add(this.txtSecondRow);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtFirst7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkMini);
            this.Controls.Add(this.pbxMAC);
            this.Controls.Add(this.txtMAC);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "NSMB RNG";
            ((System.ComponentModel.ISupportInitialize)(this.pbxMAC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile1End)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile29)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile210)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile211)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile2End)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox txtMAC;
        private PictureBox pbxMAC;
        private CheckBox chkMini;
        private Label label2;
        private TextBox txtFirst7;
        private PictureBox pbxTile11;
        private PictureBox pbxTile12;
        private PictureBox pbxTile13;
        private PictureBox pbxTile14;
        private PictureBox pbxTile15;
        private PictureBox pbxTile16;
        private PictureBox pbxTile17;
        private PictureBox pbxTile1End;
        private ComboBox cbxSystem;
        private Label label3;
        private Label lblFirstTooLong;
        private Label label4;
        private DateTimePicker dtpDate;
        private DateTimePicker dtpTime;
        private Label lblMatch;
        private PictureBox pbxTile21;
        private PictureBox pbxTile23;
        private PictureBox pbxTile25;
        private PictureBox pbxTile22;
        private PictureBox pbxTile24;
        private PictureBox pbxTile26;
        private PictureBox pbxTile27;
        private PictureBox pbxTile28;
        private PictureBox pbxTile29;
        private PictureBox pbxTile210;
        private PictureBox pbxTile211;
        private PictureBox pbxTile2End;
        private Label label5;
        private TextBox txtSecondRow;
    }
}
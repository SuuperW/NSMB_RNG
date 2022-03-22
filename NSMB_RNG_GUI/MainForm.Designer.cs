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
            this.cbxSystem = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.dtpTime = new System.Windows.Forms.DateTimePicker();
            this.lblMatch = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSecondRow = new System.Windows.Forms.TextBox();
            this.lblWorkStatus = new System.Windows.Forms.Label();
            this.tileDisplay1 = new NSMB_RNG_GUI.tileDisplay();
            this.tileDisplay2 = new NSMB_RNG_GUI.tileDisplay();
            this.tileDisplay3 = new NSMB_RNG_GUI.tileDisplay();
            this.tileDisplay4 = new NSMB_RNG_GUI.tileDisplay();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnTimeFinder = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbxMAC)).BeginInit();
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
            this.chkMini.TabIndex = 7;
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
            this.cbxSystem.SelectedIndexChanged += new System.EventHandler(this.cbxSystem_SelectedIndexChanged);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 15);
            this.label4.TabIndex = 4;
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
            this.dtpDate.TabIndex = 5;
            this.dtpDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDateTime_ValueChanged);
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
            this.dtpTime.TabIndex = 6;
            this.dtpTime.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpTime.ValueChanged += new System.EventHandler(this.dtpDateTime_ValueChanged);
            this.dtpTime.Leave += new System.EventHandler(this.dtpDateTime_Leave);
            // 
            // lblMatch
            // 
            this.lblMatch.AutoSize = true;
            this.lblMatch.Location = new System.Drawing.Point(12, 169);
            this.lblMatch.Name = "lblMatch";
            this.lblMatch.Size = new System.Drawing.Size(41, 15);
            this.lblMatch.TabIndex = 13;
            this.lblMatch.Text = "match";
            this.lblMatch.Visible = false;
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
            // lblWorkStatus
            // 
            this.lblWorkStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWorkStatus.AutoSize = true;
            this.lblWorkStatus.Location = new System.Drawing.Point(346, 169);
            this.lblWorkStatus.Name = "lblWorkStatus";
            this.lblWorkStatus.Size = new System.Drawing.Size(50, 15);
            this.lblWorkStatus.TabIndex = 14;
            this.lblWorkStatus.Text = "working";
            this.lblWorkStatus.Visible = false;
            // 
            // tileDisplay1
            // 
            this.tileDisplay1.Location = new System.Drawing.Point(12, 102);
            this.tileDisplay1.Name = "tileDisplay1";
            this.tileDisplay1.Size = new System.Drawing.Size(384, 32);
            this.tileDisplay1.TabIndex = 15;
            this.tileDisplay1.TileCount = 7;
            // 
            // tileDisplay2
            // 
            this.tileDisplay2.Location = new System.Drawing.Point(12, 134);
            this.tileDisplay2.Name = "tileDisplay2";
            this.tileDisplay2.Size = new System.Drawing.Size(384, 32);
            this.tileDisplay2.TabIndex = 16;
            this.tileDisplay2.TileCount = 11;
            // 
            // tileDisplay3
            // 
            this.tileDisplay3.Location = new System.Drawing.Point(12, 187);
            this.tileDisplay3.Name = "tileDisplay3";
            this.tileDisplay3.Size = new System.Drawing.Size(384, 32);
            this.tileDisplay3.TabIndex = 16;
            this.tileDisplay3.TileCount = 11;
            // 
            // tileDisplay4
            // 
            this.tileDisplay4.Location = new System.Drawing.Point(12, 219);
            this.tileDisplay4.Name = "tileDisplay4";
            this.tileDisplay4.Size = new System.Drawing.Size(384, 32);
            this.tileDisplay4.TabIndex = 16;
            this.tileDisplay4.TileCount = 11;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(154, 169);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 15);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 17;
            this.progressBar.Visible = false;
            // 
            // btnTimeFinder
            // 
            this.btnTimeFinder.Location = new System.Drawing.Point(299, 104);
            this.btnTimeFinder.Name = "btnTimeFinder";
            this.btnTimeFinder.Size = new System.Drawing.Size(97, 23);
            this.btnTimeFinder.TabIndex = 18;
            this.btnTimeFinder.Text = "Time Finder";
            this.btnTimeFinder.UseVisualStyleBackColor = true;
            this.btnTimeFinder.Visible = false;
            this.btnTimeFinder.Click += new System.EventHandler(this.btnTimeFinder_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 263);
            this.Controls.Add(this.btnTimeFinder);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.tileDisplay4);
            this.Controls.Add(this.tileDisplay3);
            this.Controls.Add(this.tileDisplay2);
            this.Controls.Add(this.tileDisplay1);
            this.Controls.Add(this.lblWorkStatus);
            this.Controls.Add(this.lblMatch);
            this.Controls.Add(this.dtpTime);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbxSystem);
            this.Controls.Add(this.txtSecondRow);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtFirst7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkMini);
            this.Controls.Add(this.pbxMAC);
            this.Controls.Add(this.txtMAC);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "NSMB RNG Magic Finder";
            ((System.ComponentModel.ISupportInitialize)(this.pbxMAC)).EndInit();
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
        private ComboBox cbxSystem;
        private Label label3;
        private Label label4;
        private DateTimePicker dtpDate;
        private DateTimePicker dtpTime;
        private Label lblMatch;
        private Label label5;
        private TextBox txtSecondRow;
        private Label lblWorkStatus;
        private tileDisplay tileDisplay1;
        private tileDisplay tileDisplay2;
        private tileDisplay tileDisplay3;
        private tileDisplay tileDisplay4;
        private ProgressBar progressBar;
        private Button btnTimeFinder;
    }
}
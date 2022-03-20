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
            ((System.ComponentModel.ISupportInitialize)(this.pbxMAC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTile1End)).BeginInit();
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
            this.chkMini.Location = new System.Drawing.Point(169, 43);
            this.chkMini.Name = "chkMini";
            this.chkMini.Size = new System.Drawing.Size(81, 19);
            this.chkMini.TabIndex = 3;
            this.chkMini.Text = "mini route";
            this.chkMini.UseVisualStyleBackColor = true;
            this.chkMini.CheckedChanged += new System.EventHandler(this.chkMini_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "First 7 tiles:";
            // 
            // txtFirst7
            // 
            this.txtFirst7.Location = new System.Drawing.Point(83, 102);
            this.txtFirst7.Name = "txtFirst7";
            this.txtFirst7.Size = new System.Drawing.Size(100, 23);
            this.txtFirst7.TabIndex = 5;
            this.txtFirst7.TextChanged += new System.EventHandler(this.txtFirst7_TextChanged);
            this.txtFirst7.Enter += new System.EventHandler(this.txtFirst7_Enter);
            // 
            // pbxTile11
            // 
            this.pbxTile11.BackColor = System.Drawing.Color.Red;
            this.pbxTile11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxTile11.Location = new System.Drawing.Point(12, 131);
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
            this.pbxTile12.Location = new System.Drawing.Point(44, 131);
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
            this.pbxTile13.Location = new System.Drawing.Point(76, 131);
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
            this.pbxTile14.Location = new System.Drawing.Point(108, 131);
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
            this.pbxTile15.Location = new System.Drawing.Point(140, 131);
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
            this.pbxTile16.Location = new System.Drawing.Point(172, 131);
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
            this.pbxTile17.Location = new System.Drawing.Point(204, 131);
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
            this.pbxTile1End.Location = new System.Drawing.Point(236, 131);
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
            this.cbxSystem.Location = new System.Drawing.Point(66, 41);
            this.cbxSystem.Name = "cbxSystem";
            this.cbxSystem.Size = new System.Drawing.Size(84, 23);
            this.cbxSystem.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "System:";
            // 
            // lblFirstTooLong
            // 
            this.lblFirstTooLong.AutoSize = true;
            this.lblFirstTooLong.Location = new System.Drawing.Point(204, 110);
            this.lblFirstTooLong.Name = "lblFirstTooLong";
            this.lblFirstTooLong.Size = new System.Drawing.Size(82, 15);
            this.lblFirstTooLong.TabIndex = 9;
            this.lblFirstTooLong.Text = "too many tiles";
            this.lblFirstTooLong.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Date/Time:";
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(84, 70);
            this.dtpDate.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            this.dtpDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(99, 23);
            this.dtpDate.TabIndex = 11;
            this.dtpDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpDate.Leave += new System.EventHandler(this.dtpDateTime_Leave);
            // 
            // dtpTime
            // 
            this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpTime.Location = new System.Drawing.Point(189, 70);
            this.dtpTime.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            this.dtpTime.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpTime.Name = "dtpTime";
            this.dtpTime.ShowUpDown = true;
            this.dtpTime.Size = new System.Drawing.Size(99, 23);
            this.dtpTime.TabIndex = 11;
            this.dtpTime.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpTime.Leave += new System.EventHandler(this.dtpDateTime_Leave);
            // 
            // lblMatch
            // 
            this.lblMatch.AutoSize = true;
            this.lblMatch.Location = new System.Drawing.Point(12, 236);
            this.lblMatch.Name = "lblMatch";
            this.lblMatch.Size = new System.Drawing.Size(41, 15);
            this.lblMatch.TabIndex = 12;
            this.lblMatch.Text = "match";
            this.lblMatch.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 307);
            this.Controls.Add(this.lblMatch);
            this.Controls.Add(this.dtpTime);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblFirstTooLong);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbxSystem);
            this.Controls.Add(this.pbxTile1End);
            this.Controls.Add(this.pbxTile17);
            this.Controls.Add(this.pbxTile16);
            this.Controls.Add(this.pbxTile14);
            this.Controls.Add(this.pbxTile12);
            this.Controls.Add(this.pbxTile15);
            this.Controls.Add(this.pbxTile13);
            this.Controls.Add(this.pbxTile11);
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
    }
}
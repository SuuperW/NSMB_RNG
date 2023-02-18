namespace NSMB_RNG_GUI
{
	partial class DoubleJumpsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tileDisplay2 = new NSMB_RNG_GUI.tileDisplay();
			this.tileDisplay1 = new NSMB_RNG_GUI.tileDisplay();
			this.lblExpectedPattern = new System.Windows.Forms.Label();
			this.lblTime = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.chkMini = new System.Windows.Forms.CheckBox();
			this.numPTile = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.lblDJCount = new System.Windows.Forms.Label();
			this.btnBack = new System.Windows.Forms.Button();
			this.lbl78 = new System.Windows.Forms.Label();
			this.btnWrongPattern = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numPTile)).BeginInit();
			this.SuspendLayout();
			// 
			// tileDisplay2
			// 
			this.tileDisplay2.Location = new System.Drawing.Point(0, 32);
			this.tileDisplay2.Name = "tileDisplay2";
			this.tileDisplay2.Size = new System.Drawing.Size(352, 32);
			this.tileDisplay2.TabIndex = 1;
			this.tileDisplay2.TileCount = 11;
			// 
			// tileDisplay1
			// 
			this.tileDisplay1.Location = new System.Drawing.Point(0, 0);
			this.tileDisplay1.Name = "tileDisplay1";
			this.tileDisplay1.Size = new System.Drawing.Size(352, 32);
			this.tileDisplay1.TabIndex = 2;
			this.tileDisplay1.TileCount = 11;
			// 
			// lblExpectedPattern
			// 
			this.lblExpectedPattern.AutoSize = true;
			this.lblExpectedPattern.Location = new System.Drawing.Point(7, 67);
			this.lblExpectedPattern.Name = "lblExpectedPattern";
			this.lblExpectedPattern.Size = new System.Drawing.Size(336, 15);
			this.lblExpectedPattern.TabIndex = 3;
			this.lblExpectedPattern.Text = "This is the pattern you should expect with the given date/time.";
			// 
			// lblTime
			// 
			this.lblTime.AutoSize = true;
			this.lblTime.Location = new System.Drawing.Point(7, 82);
			this.lblTime.Name = "lblTime";
			this.lblTime.Size = new System.Drawing.Size(31, 15);
			this.lblTime.TabIndex = 4;
			this.lblTime.Text = "time";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 137);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 15);
			this.label1.TabIndex = 5;
			this.label1.Text = "P tile position:";
			// 
			// chkMini
			// 
			this.chkMini.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.chkMini.AutoSize = true;
			this.chkMini.Location = new System.Drawing.Point(259, 107);
			this.chkMini.Name = "chkMini";
			this.chkMini.Size = new System.Drawing.Size(81, 19);
			this.chkMini.TabIndex = 6;
			this.chkMini.Text = "mini route";
			this.chkMini.UseVisualStyleBackColor = true;
			this.chkMini.CheckedChanged += new System.EventHandler(this.chkMini_CheckedChanged);
			// 
			// numPTile
			// 
			this.numPTile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numPTile.Location = new System.Drawing.Point(99, 135);
			this.numPTile.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
			this.numPTile.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numPTile.Name = "numPTile";
			this.numPTile.Size = new System.Drawing.Size(35, 23);
			this.numPTile.TabIndex = 7;
			this.numPTile.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
			this.numPTile.ValueChanged += new System.EventHandler(this.numPTile_ValueChanged);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 161);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(137, 15);
			this.label2.TabIndex = 8;
			this.label2.Text = "Numer of double jumps:";
			// 
			// lblDJCount
			// 
			this.lblDJCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblDJCount.AutoSize = true;
			this.lblDJCount.Location = new System.Drawing.Point(149, 161);
			this.lblDJCount.Name = "lblDJCount";
			this.lblDJCount.Size = new System.Drawing.Size(46, 15);
			this.lblDJCount.TabIndex = 9;
			this.lblDJCount.Text = "not 0, 8";
			// 
			// btnBack
			// 
			this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBack.Location = new System.Drawing.Point(265, 150);
			this.btnBack.Name = "btnBack";
			this.btnBack.Size = new System.Drawing.Size(75, 23);
			this.btnBack.TabIndex = 10;
			this.btnBack.Text = "Back";
			this.btnBack.UseVisualStyleBackColor = true;
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			// 
			// lbl78
			// 
			this.lbl78.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lbl78.AutoSize = true;
			this.lbl78.Location = new System.Drawing.Point(180, 129);
			this.lbl78.Name = "lbl78";
			this.lbl78.Size = new System.Drawing.Size(163, 15);
			this.lbl78.TabIndex = 11;
			this.lbl78.Text = "0, 2, 3, and 8 will always work.";
			// 
			// btnWrongPattern
			// 
			this.btnWrongPattern.Location = new System.Drawing.Point(12, 100);
			this.btnWrongPattern.Name = "btnWrongPattern";
			this.btnWrongPattern.Size = new System.Drawing.Size(100, 23);
			this.btnWrongPattern.TabIndex = 12;
			this.btnWrongPattern.Text = "Wrong pattern?";
			this.btnWrongPattern.UseVisualStyleBackColor = true;
			this.btnWrongPattern.Click += new System.EventHandler(this.btnWrongPattern_Click);
			// 
			// DoubleJumpsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(352, 185);
			this.Controls.Add(this.btnWrongPattern);
			this.Controls.Add(this.lbl78);
			this.Controls.Add(this.btnBack);
			this.Controls.Add(this.lblDJCount);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.numPTile);
			this.Controls.Add(this.chkMini);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblTime);
			this.Controls.Add(this.lblExpectedPattern);
			this.Controls.Add(this.tileDisplay2);
			this.Controls.Add(this.tileDisplay1);
			this.Name = "DoubleJumpsForm";
			this.Text = "NSMB RNG Double Jumps";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DoubleJumpsForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.numPTile)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private tileDisplay tileDisplay2;
		private tileDisplay tileDisplay1;
		private System.Windows.Forms.Label lblExpectedPattern;
		private System.Windows.Forms.Label lblTime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox chkMini;
		private System.Windows.Forms.NumericUpDown numPTile;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblDJCount;
		private System.Windows.Forms.Button btnBack;
		private System.Windows.Forms.Label lbl78;
		private System.Windows.Forms.Button btnWrongPattern;
	}
}
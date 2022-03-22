namespace NSMB_RNG_GUI
{
    partial class TimeFinder
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
            this.label1 = new System.Windows.Forms.Label();
            this.numSeconds = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numThreads = new System.Windows.Forms.NumericUpDown();
            this.chkMini = new System.Windows.Forms.CheckBox();
            this.chkAutoSeconds = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkL = new System.Windows.Forms.CheckBox();
            this.chkR = new System.Windows.Forms.CheckBox();
            this.chkStart = new System.Windows.Forms.CheckBox();
            this.chkSelect = new System.Windows.Forms.CheckBox();
            this.chkLeft = new System.Windows.Forms.CheckBox();
            this.chkUp = new System.Windows.Forms.CheckBox();
            this.chkDown = new System.Windows.Forms.CheckBox();
            this.chkRight = new System.Windows.Forms.CheckBox();
            this.chkY = new System.Windows.Forms.CheckBox();
            this.chkB = new System.Windows.Forms.CheckBox();
            this.chkX = new System.Windows.Forms.CheckBox();
            this.chkA = new System.Windows.Forms.CheckBox();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblResults = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.numSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seconds:";
            // 
            // numSeconds
            // 
            this.numSeconds.Enabled = false;
            this.numSeconds.Location = new System.Drawing.Point(71, 41);
            this.numSeconds.Name = "numSeconds";
            this.numSeconds.Size = new System.Drawing.Size(54, 23);
            this.numSeconds.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Threads:";
            // 
            // numThreads
            // 
            this.numThreads.Location = new System.Drawing.Point(71, 12);
            this.numThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThreads.Name = "numThreads";
            this.numThreads.Size = new System.Drawing.Size(54, 23);
            this.numThreads.TabIndex = 1;
            this.numThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkMini
            // 
            this.chkMini.AutoSize = true;
            this.chkMini.Location = new System.Drawing.Point(131, 13);
            this.chkMini.Name = "chkMini";
            this.chkMini.Size = new System.Drawing.Size(81, 19);
            this.chkMini.TabIndex = 8;
            this.chkMini.Text = "mini route";
            this.chkMini.UseVisualStyleBackColor = true;
            // 
            // chkAutoSeconds
            // 
            this.chkAutoSeconds.AutoSize = true;
            this.chkAutoSeconds.Checked = true;
            this.chkAutoSeconds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSeconds.Location = new System.Drawing.Point(131, 45);
            this.chkAutoSeconds.Name = "chkAutoSeconds";
            this.chkAutoSeconds.Size = new System.Drawing.Size(50, 19);
            this.chkAutoSeconds.TabIndex = 8;
            this.chkAutoSeconds.Text = "auto";
            this.chkAutoSeconds.UseVisualStyleBackColor = true;
            this.chkAutoSeconds.CheckedChanged += new System.EventHandler(this.chkAutoSeconds_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Buttons:";
            // 
            // chkL
            // 
            this.chkL.AutoSize = true;
            this.chkL.Location = new System.Drawing.Point(69, 4);
            this.chkL.Margin = new System.Windows.Forms.Padding(1);
            this.chkL.Name = "chkL";
            this.chkL.Size = new System.Drawing.Size(32, 19);
            this.chkL.TabIndex = 10;
            this.chkL.Tag = "1";
            this.chkL.Text = "L";
            this.chkL.UseVisualStyleBackColor = true;
            // 
            // chkR
            // 
            this.chkR.AutoSize = true;
            this.chkR.Location = new System.Drawing.Point(147, 4);
            this.chkR.Margin = new System.Windows.Forms.Padding(1);
            this.chkR.Name = "chkR";
            this.chkR.Size = new System.Drawing.Size(33, 19);
            this.chkR.TabIndex = 10;
            this.chkR.Text = "R";
            this.chkR.UseVisualStyleBackColor = true;
            // 
            // chkStart
            // 
            this.chkStart.AutoSize = true;
            this.chkStart.Location = new System.Drawing.Point(71, 85);
            this.chkStart.Margin = new System.Windows.Forms.Padding(1);
            this.chkStart.Name = "chkStart";
            this.chkStart.Size = new System.Drawing.Size(50, 19);
            this.chkStart.TabIndex = 10;
            this.chkStart.Text = "Start";
            this.chkStart.UseVisualStyleBackColor = true;
            // 
            // chkSelect
            // 
            this.chkSelect.AutoSize = true;
            this.chkSelect.Location = new System.Drawing.Point(123, 85);
            this.chkSelect.Margin = new System.Windows.Forms.Padding(1);
            this.chkSelect.Name = "chkSelect";
            this.chkSelect.Size = new System.Drawing.Size(57, 19);
            this.chkSelect.TabIndex = 10;
            this.chkSelect.Text = "Select";
            this.chkSelect.UseVisualStyleBackColor = true;
            // 
            // chkLeft
            // 
            this.chkLeft.AutoSize = true;
            this.chkLeft.Location = new System.Drawing.Point(43, 43);
            this.chkLeft.Margin = new System.Windows.Forms.Padding(1);
            this.chkLeft.Name = "chkLeft";
            this.chkLeft.Size = new System.Drawing.Size(32, 19);
            this.chkLeft.TabIndex = 10;
            this.chkLeft.Text = "L";
            this.chkLeft.UseVisualStyleBackColor = true;
            // 
            // chkUp
            // 
            this.chkUp.AutoSize = true;
            this.chkUp.Location = new System.Drawing.Point(61, 22);
            this.chkUp.Margin = new System.Windows.Forms.Padding(1);
            this.chkUp.Name = "chkUp";
            this.chkUp.Size = new System.Drawing.Size(34, 19);
            this.chkUp.TabIndex = 10;
            this.chkUp.Text = "U";
            this.chkUp.UseVisualStyleBackColor = true;
            // 
            // chkDown
            // 
            this.chkDown.AutoSize = true;
            this.chkDown.Location = new System.Drawing.Point(62, 64);
            this.chkDown.Margin = new System.Windows.Forms.Padding(1);
            this.chkDown.Name = "chkDown";
            this.chkDown.Size = new System.Drawing.Size(34, 19);
            this.chkDown.TabIndex = 10;
            this.chkDown.Text = "D";
            this.chkDown.UseVisualStyleBackColor = true;
            // 
            // chkRight
            // 
            this.chkRight.AutoSize = true;
            this.chkRight.Location = new System.Drawing.Point(77, 43);
            this.chkRight.Margin = new System.Windows.Forms.Padding(1);
            this.chkRight.Name = "chkRight";
            this.chkRight.Size = new System.Drawing.Size(33, 19);
            this.chkRight.TabIndex = 10;
            this.chkRight.Text = "R";
            this.chkRight.UseVisualStyleBackColor = true;
            // 
            // chkY
            // 
            this.chkY.AutoSize = true;
            this.chkY.Location = new System.Drawing.Point(112, 43);
            this.chkY.Margin = new System.Windows.Forms.Padding(1);
            this.chkY.Name = "chkY";
            this.chkY.Size = new System.Drawing.Size(33, 19);
            this.chkY.TabIndex = 10;
            this.chkY.Text = "Y";
            this.chkY.UseVisualStyleBackColor = true;
            // 
            // chkB
            // 
            this.chkB.AutoSize = true;
            this.chkB.Location = new System.Drawing.Point(131, 64);
            this.chkB.Margin = new System.Windows.Forms.Padding(1);
            this.chkB.Name = "chkB";
            this.chkB.Size = new System.Drawing.Size(33, 19);
            this.chkB.TabIndex = 10;
            this.chkB.Text = "B";
            this.chkB.UseVisualStyleBackColor = true;
            // 
            // chkX
            // 
            this.chkX.AutoSize = true;
            this.chkX.Location = new System.Drawing.Point(130, 22);
            this.chkX.Margin = new System.Windows.Forms.Padding(1);
            this.chkX.Name = "chkX";
            this.chkX.Size = new System.Drawing.Size(33, 19);
            this.chkX.TabIndex = 10;
            this.chkX.Text = "X";
            this.chkX.UseVisualStyleBackColor = true;
            // 
            // chkA
            // 
            this.chkA.AutoSize = true;
            this.chkA.Location = new System.Drawing.Point(146, 43);
            this.chkA.Margin = new System.Windows.Forms.Padding(1);
            this.chkA.Name = "chkA";
            this.chkA.Size = new System.Drawing.Size(34, 19);
            this.chkA.TabIndex = 10;
            this.chkA.Text = "A";
            this.chkA.UseVisualStyleBackColor = true;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlButtons.Controls.Add(this.chkSelect);
            this.pnlButtons.Controls.Add(this.chkL);
            this.pnlButtons.Controls.Add(this.chkA);
            this.pnlButtons.Controls.Add(this.label3);
            this.pnlButtons.Controls.Add(this.chkRight);
            this.pnlButtons.Controls.Add(this.chkLeft);
            this.pnlButtons.Controls.Add(this.chkX);
            this.pnlButtons.Controls.Add(this.chkY);
            this.pnlButtons.Controls.Add(this.chkUp);
            this.pnlButtons.Controls.Add(this.chkStart);
            this.pnlButtons.Controls.Add(this.chkR);
            this.pnlButtons.Controls.Add(this.chkDown);
            this.pnlButtons.Controls.Add(this.chkB);
            this.pnlButtons.Location = new System.Drawing.Point(0, 70);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(259, 110);
            this.pnlButtons.TabIndex = 11;
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(174, 186);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 12;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(12, 186);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 13;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new System.Drawing.Point(12, 217);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(52, 15);
            this.lblResults.TabIndex = 0;
            this.lblResults.Text = "Progress";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(70, 213);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(179, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // TimeFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 245);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.chkAutoSeconds);
            this.Controls.Add(this.chkMini);
            this.Controls.Add(this.numThreads);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numSeconds);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlButtons);
            this.Name = "TimeFinder";
            this.Text = "TimeFinder";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TimeFinder_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.numSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numSeconds;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numThreads;
        private System.Windows.Forms.CheckBox chkMini;
        private System.Windows.Forms.CheckBox chkAutoSeconds;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkL;
        private System.Windows.Forms.CheckBox chkR;
        private System.Windows.Forms.CheckBox chkStart;
        private System.Windows.Forms.CheckBox chkSelect;
        private System.Windows.Forms.CheckBox chkLeft;
        private System.Windows.Forms.CheckBox chkUp;
        private System.Windows.Forms.CheckBox chkDown;
        private System.Windows.Forms.CheckBox chkRight;
        private System.Windows.Forms.CheckBox chkY;
        private System.Windows.Forms.CheckBox chkB;
        private System.Windows.Forms.CheckBox chkX;
        private System.Windows.Forms.CheckBox chkA;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}
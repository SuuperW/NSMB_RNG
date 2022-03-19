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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pbxMAC);
            this.Controls.Add(this.txtMAC);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "NSMB RNG";
            ((System.ComponentModel.ISupportInitialize)(this.pbxMAC)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox txtMAC;
        private PictureBox pbxMAC;
    }
}
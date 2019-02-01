namespace ClientsLibrary
{
    partial class FormLogView
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
            this.userButton6 = new HslCommunication.Controls.UserButton();
            this.userButton7 = new HslCommunication.Controls.UserButton();
            this.logNetAnalysisControl1 = new HslCommunication.LogNet.LogNetAnalysisControl();
            this.SuspendLayout();
            // 
            // userButton6
            // 
            this.userButton6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.userButton6.BackColor = System.Drawing.Color.Transparent;
            this.userButton6.CustomerInformation = "";
            this.userButton6.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton6.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton6.Location = new System.Drawing.Point(907, 674);
            this.userButton6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton6.Name = "userButton6";
            this.userButton6.Size = new System.Drawing.Size(103, 26);
            this.userButton6.TabIndex = 36;
            this.userButton6.UIText = "运行日志清空";
            this.userButton6.Click += new System.EventHandler(this.userButton6_Click);
            // 
            // userButton7
            // 
            this.userButton7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.userButton7.BackColor = System.Drawing.Color.Transparent;
            this.userButton7.CustomerInformation = "";
            this.userButton7.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton7.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton7.Location = new System.Drawing.Point(797, 674);
            this.userButton7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton7.Name = "userButton7";
            this.userButton7.Size = new System.Drawing.Size(103, 26);
            this.userButton7.TabIndex = 35;
            this.userButton7.UIText = "运行日志查看";
            this.userButton7.Click += new System.EventHandler(this.userButton7_Click);
            // 
            // logNetAnalysisControl1
            // 
            this.logNetAnalysisControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logNetAnalysisControl1.Location = new System.Drawing.Point(4, 6);
            this.logNetAnalysisControl1.Name = "logNetAnalysisControl1";
            this.logNetAnalysisControl1.Size = new System.Drawing.Size(789, 697);
            this.logNetAnalysisControl1.TabIndex = 47;
         
            // 
            // FormLogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(1023, 713);
            this.Controls.Add(this.logNetAnalysisControl1);
            this.Controls.Add(this.userButton6);
            this.Controls.Add(this.userButton7);
            this.Name = "FormLogView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "日志查看器";
            this.Load += new System.EventHandler(this.FormLogView_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private HslCommunication.Controls.UserButton userButton6;
        private HslCommunication.Controls.UserButton userButton7;
        private HslCommunication.LogNet.LogNetAnalysisControl logNetAnalysisControl1;
    }
}
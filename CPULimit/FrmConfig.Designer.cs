namespace CPULimit
{
    partial class FrmConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConfig));
            this.LboList = new System.Windows.Forms.ListBox();
            this.BtnInsert = new System.Windows.Forms.Button();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.NUDTimerInterval = new System.Windows.Forms.NumericUpDown();
            this.NUDPauseInterval = new System.Windows.Forms.NumericUpDown();
            this.Lbl6 = new System.Windows.Forms.Label();
            this.Lbl4 = new System.Windows.Forms.Label();
            this.Lbl2 = new System.Windows.Forms.Label();
            this.TboRemark = new System.Windows.Forms.TextBox();
            this.Lbl3 = new System.Windows.Forms.Label();
            this.CboProcess = new System.Windows.Forms.ComboBox();
            this.Lbl5 = new System.Windows.Forms.Label();
            this.Lbl7 = new System.Windows.Forms.Label();
            this.BtnFlush = new System.Windows.Forms.Button();
            this.TboUsername = new System.Windows.Forms.TextBox();
            this.Lbl1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NUDTimerInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDPauseInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // LboList
            // 
            this.LboList.Dock = System.Windows.Forms.DockStyle.Left;
            this.LboList.FormattingEnabled = true;
            this.LboList.ItemHeight = 12;
            this.LboList.Location = new System.Drawing.Point(0, 0);
            this.LboList.Name = "LboList";
            this.LboList.ScrollAlwaysVisible = true;
            this.LboList.Size = new System.Drawing.Size(208, 262);
            this.LboList.TabIndex = 1;
            this.LboList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LboList_MouseDown);
            // 
            // BtnInsert
            // 
            this.BtnInsert.Location = new System.Drawing.Point(214, 224);
            this.BtnInsert.Name = "BtnInsert";
            this.BtnInsert.Size = new System.Drawing.Size(66, 26);
            this.BtnInsert.TabIndex = 6;
            this.BtnInsert.Text = "创建任务";
            this.BtnInsert.UseVisualStyleBackColor = true;
            this.BtnInsert.Click += new System.EventHandler(this.BtnInsert_Click);
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(306, 224);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(66, 26);
            this.BtnDelete.TabIndex = 7;
            this.BtnDelete.Text = "删除任务";
            this.BtnDelete.UseVisualStyleBackColor = true;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // NUDTimerInterval
            // 
            this.NUDTimerInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NUDTimerInterval.Location = new System.Drawing.Point(273, 92);
            this.NUDTimerInterval.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NUDTimerInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUDTimerInterval.Name = "NUDTimerInterval";
            this.NUDTimerInterval.Size = new System.Drawing.Size(77, 21);
            this.NUDTimerInterval.TabIndex = 4;
            this.NUDTimerInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUDTimerInterval.ThousandsSeparator = true;
            this.NUDTimerInterval.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // NUDPauseInterval
            // 
            this.NUDPauseInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NUDPauseInterval.Location = new System.Drawing.Point(273, 119);
            this.NUDPauseInterval.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NUDPauseInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUDPauseInterval.Name = "NUDPauseInterval";
            this.NUDPauseInterval.Size = new System.Drawing.Size(77, 21);
            this.NUDPauseInterval.TabIndex = 5;
            this.NUDPauseInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUDPauseInterval.ThousandsSeparator = true;
            this.NUDPauseInterval.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // Lbl6
            // 
            this.Lbl6.AutoSize = true;
            this.Lbl6.Location = new System.Drawing.Point(214, 123);
            this.Lbl6.Name = "Lbl6";
            this.Lbl6.Size = new System.Drawing.Size(53, 12);
            this.Lbl6.TabIndex = 7;
            this.Lbl6.Text = "暂停时间";
            // 
            // Lbl4
            // 
            this.Lbl4.AutoSize = true;
            this.Lbl4.Location = new System.Drawing.Point(214, 96);
            this.Lbl4.Name = "Lbl4";
            this.Lbl4.Size = new System.Drawing.Size(53, 12);
            this.Lbl4.TabIndex = 6;
            this.Lbl4.Text = "暂停间隔";
            // 
            // Lbl2
            // 
            this.Lbl2.AutoSize = true;
            this.Lbl2.Location = new System.Drawing.Point(224, 42);
            this.Lbl2.Name = "Lbl2";
            this.Lbl2.Size = new System.Drawing.Size(29, 12);
            this.Lbl2.TabIndex = 5;
            this.Lbl2.Text = "备注";
            // 
            // TboRemark
            // 
            this.TboRemark.Location = new System.Drawing.Point(259, 39);
            this.TboRemark.Name = "TboRemark";
            this.TboRemark.Size = new System.Drawing.Size(113, 21);
            this.TboRemark.TabIndex = 2;
            // 
            // Lbl3
            // 
            this.Lbl3.AutoSize = true;
            this.Lbl3.Location = new System.Drawing.Point(224, 69);
            this.Lbl3.Name = "Lbl3";
            this.Lbl3.Size = new System.Drawing.Size(29, 12);
            this.Lbl3.TabIndex = 4;
            this.Lbl3.Text = "进程";
            // 
            // CboProcess
            // 
            this.CboProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CboProcess.FormattingEnabled = true;
            this.CboProcess.Location = new System.Drawing.Point(259, 66);
            this.CboProcess.Name = "CboProcess";
            this.CboProcess.Size = new System.Drawing.Size(113, 20);
            this.CboProcess.TabIndex = 3;
            this.CboProcess.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CboProcess_MouseDown);
            // 
            // Lbl5
            // 
            this.Lbl5.AutoSize = true;
            this.Lbl5.Location = new System.Drawing.Point(354, 96);
            this.Lbl5.Name = "Lbl5";
            this.Lbl5.Size = new System.Drawing.Size(29, 12);
            this.Lbl5.TabIndex = 1;
            this.Lbl5.Text = "毫秒";
            // 
            // Lbl7
            // 
            this.Lbl7.AutoSize = true;
            this.Lbl7.Location = new System.Drawing.Point(354, 123);
            this.Lbl7.Name = "Lbl7";
            this.Lbl7.Size = new System.Drawing.Size(29, 12);
            this.Lbl7.TabIndex = 0;
            this.Lbl7.Text = "毫秒";
            // 
            // BtnFlush
            // 
            this.BtnFlush.Location = new System.Drawing.Point(214, 195);
            this.BtnFlush.Name = "BtnFlush";
            this.BtnFlush.Size = new System.Drawing.Size(158, 23);
            this.BtnFlush.TabIndex = 8;
            this.BtnFlush.Text = "刷新进程列表与信息";
            this.BtnFlush.UseVisualStyleBackColor = true;
            this.BtnFlush.Click += new System.EventHandler(this.BtnFlush_Click);
            // 
            // TboUsername
            // 
            this.TboUsername.Location = new System.Drawing.Point(259, 12);
            this.TboUsername.Name = "TboUsername";
            this.TboUsername.Size = new System.Drawing.Size(113, 21);
            this.TboUsername.TabIndex = 9;
            // 
            // Lbl1
            // 
            this.Lbl1.AutoSize = true;
            this.Lbl1.Location = new System.Drawing.Point(224, 15);
            this.Lbl1.Name = "Lbl1";
            this.Lbl1.Size = new System.Drawing.Size(29, 12);
            this.Lbl1.TabIndex = 0;
            this.Lbl1.Text = "用户";
            // 
            // FrmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 262);
            this.Controls.Add(this.TboUsername);
            this.Controls.Add(this.Lbl1);
            this.Controls.Add(this.BtnFlush);
            this.Controls.Add(this.Lbl7);
            this.Controls.Add(this.Lbl5);
            this.Controls.Add(this.CboProcess);
            this.Controls.Add(this.Lbl3);
            this.Controls.Add(this.TboRemark);
            this.Controls.Add(this.Lbl2);
            this.Controls.Add(this.Lbl4);
            this.Controls.Add(this.Lbl6);
            this.Controls.Add(this.NUDPauseInterval);
            this.Controls.Add(this.NUDTimerInterval);
            this.Controls.Add(this.BtnDelete);
            this.Controls.Add(this.BtnInsert);
            this.Controls.Add(this.LboList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfig";
            this.Text = "管理进程列表";
            ((System.ComponentModel.ISupportInitialize)(this.NUDTimerInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDPauseInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox LboList;
        private System.Windows.Forms.Button BtnInsert;
        private System.Windows.Forms.Button BtnDelete;
        private System.Windows.Forms.NumericUpDown NUDTimerInterval;
        private System.Windows.Forms.NumericUpDown NUDPauseInterval;
        private System.Windows.Forms.Label Lbl6;
        private System.Windows.Forms.Label Lbl4;
        private System.Windows.Forms.Label Lbl2;
        private System.Windows.Forms.TextBox TboRemark;
        private System.Windows.Forms.Label Lbl3;
        private System.Windows.Forms.ComboBox CboProcess;
        private System.Windows.Forms.Label Lbl5;
        private System.Windows.Forms.Label Lbl7;
        private System.Windows.Forms.Button BtnFlush;
        private System.Windows.Forms.TextBox TboUsername;
        private System.Windows.Forms.Label Lbl1;
    }
}
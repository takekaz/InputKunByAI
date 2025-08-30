namespace InputKunByAI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.PictureBox pbDrag;
        private System.Windows.Forms.TextBox txtEndTime;
        private System.Windows.Forms.Label lblEndTime;
        private System.Windows.Forms.FlowLayoutPanel flpPanels;
        private System.Windows.Forms.Label lblHeaderWorkNo;
        private System.Windows.Forms.Label lblHeaderWorkKind;
        private System.Windows.Forms.Label lblHeaderBikou;
        private System.Windows.Forms.Label lblHeaderStart;
        private System.Windows.Forms.ContextMenuStrip ctxTime;

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
            this.components = new System.ComponentModel.Container();
            this.pbDrag = new System.Windows.Forms.PictureBox();
            this.txtEndTime = new System.Windows.Forms.TextBox();
            this.lblEndTime = new System.Windows.Forms.Label();
            this.flpPanels = new System.Windows.Forms.FlowLayoutPanel();
            this.lblHeaderWorkNo = new System.Windows.Forms.Label();
            this.lblHeaderWorkKind = new System.Windows.Forms.Label();
            this.lblHeaderBikou = new System.Windows.Forms.Label();
            this.lblHeaderStart = new System.Windows.Forms.Label();
            this.ctxTime = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbDrag)).BeginInit();
            this.SuspendLayout();
            // 
            // pbDrag
            // 
            this.pbDrag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbDrag.Location = new System.Drawing.Point(12, 12);
            this.pbDrag.Name = "pbDrag";
            this.pbDrag.Size = new System.Drawing.Size(32, 32);
            this.pbDrag.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbDrag.TabIndex = 0;
            this.pbDrag.TabStop = false;
            this.pbDrag.Paint += new System.Windows.Forms.PaintEventHandler(this.pbDrag_Paint);
            this.pbDrag.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbDrag_MouseDown);
            // 
            // txtEndTime
            // 
            this.txtEndTime.Location = new System.Drawing.Point(130, 18);
            this.txtEndTime.Name = "txtEndTime";
            this.txtEndTime.Size = new System.Drawing.Size(80, 23);
            this.txtEndTime.TabIndex = 1;
            this.txtEndTime.ContextMenuStrip = this.ctxTime;
            // 
            // lblEndTime
            // 
            this.lblEndTime.AutoSize = true;
            this.lblEndTime.Location = new System.Drawing.Point(62, 22);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new System.Drawing.Size(56, 15);
            this.lblEndTime.TabIndex = 2;
            this.lblEndTime.Text = "終了時刻";
            // 
            // lblHeaderWorkNo
            // 
            this.lblHeaderWorkNo.AutoSize = true;
            this.lblHeaderWorkNo.Location = new System.Drawing.Point(12, 64);
            this.lblHeaderWorkNo.Name = "lblHeaderWorkNo";
            this.lblHeaderWorkNo.Size = new System.Drawing.Size(55, 15);
            this.lblHeaderWorkNo.TabIndex = 3;
            this.lblHeaderWorkNo.Text = "作業番号";
            // 
            // lblHeaderWorkKind
            // 
            this.lblHeaderWorkKind.AutoSize = true;
            this.lblHeaderWorkKind.Location = new System.Drawing.Point(220, 64);
            this.lblHeaderWorkKind.Name = "lblHeaderWorkKind";
            this.lblHeaderWorkKind.Size = new System.Drawing.Size(55, 15);
            this.lblHeaderWorkKind.TabIndex = 4;
            this.lblHeaderWorkKind.Text = "作業区分";
            // 
            // lblHeaderBikou
            // 
            this.lblHeaderBikou.AutoSize = true;
            this.lblHeaderBikou.Location = new System.Drawing.Point(400, 64);
            this.lblHeaderBikou.Name = "lblHeaderBikou";
            this.lblHeaderBikou.Size = new System.Drawing.Size(31, 15);
            this.lblHeaderBikou.TabIndex = 5;
            this.lblHeaderBikou.Text = "備考";
            // 
            // lblHeaderStart
            // 
            this.lblHeaderStart.AutoSize = true;
            this.lblHeaderStart.Location = new System.Drawing.Point(600, 64);
            this.lblHeaderStart.Name = "lblHeaderStart";
            this.lblHeaderStart.Size = new System.Drawing.Size(88, 15);
            this.lblHeaderStart.TabIndex = 6;
            this.lblHeaderStart.Text = "作業時刻(開始)";
            // 
            // flpPanels
            // 
            this.flpPanels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)))
            ;
            this.flpPanels.AutoScroll = true;
            this.flpPanels.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpPanels.Location = new System.Drawing.Point(12, 84);
            this.flpPanels.Name = "flpPanels";
            this.flpPanels.Size = new System.Drawing.Size(760, 660);
            this.flpPanels.TabIndex = 7;
            this.flpPanels.WrapContents = false;
            // 
            // ctxTime
            // 
            this.ctxTime.Name = "ctxTime";
            this.ctxTime.Size = new System.Drawing.Size(61, 4);
            this.ctxTime.Opening += new System.ComponentModel.CancelEventHandler(this.ctxTime_Opening);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 720);
            this.Controls.Add(this.flpPanels);
            this.Controls.Add(this.lblHeaderStart);
            this.Controls.Add(this.lblHeaderBikou);
            this.Controls.Add(this.lblHeaderWorkKind);
            this.Controls.Add(this.lblHeaderWorkNo);
            this.Controls.Add(this.lblEndTime);
            this.Controls.Add(this.txtEndTime);
            this.Controls.Add(this.pbDrag);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "Form1";
            this.Text = "InputKunByAI";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbDrag)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}

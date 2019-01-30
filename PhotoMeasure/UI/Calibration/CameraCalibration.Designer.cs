namespace PhotoMeasure.UI
{
    partial class CameraCalibration
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
            this.lbImages = new System.Windows.Forms.ListBox();
            this.iePreview = new PhotoMeasure.UI.ImageExplorer();
            this.ofdImages = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cmdCalibrate = new System.Windows.Forms.Button();
            this.lvResults = new System.Windows.Forms.ListView();
            this.lblSummary = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdAccept = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCalibrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCalibrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbImages
            // 
            this.lbImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbImages.FormattingEnabled = true;
            this.lbImages.Location = new System.Drawing.Point(3, 3);
            this.lbImages.Name = "lbImages";
            this.lbImages.Size = new System.Drawing.Size(223, 381);
            this.lbImages.TabIndex = 0;
            this.lbImages.SelectedIndexChanged += new System.EventHandler(this.lbImages_SelectedIndexChanged);
            // 
            // iePreview
            // 
            this.iePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.iePreview.Location = new System.Drawing.Point(3, 3);
            this.iePreview.Name = "iePreview";
            this.iePreview.Size = new System.Drawing.Size(449, 337);
            this.iePreview.TabIndex = 1;
            this.iePreview.PaintImage += new System.EventHandler<System.Windows.Forms.PaintEventArgs>(this.iePreview_PaintImage);
            // 
            // ofdImages
            // 
            this.ofdImages.Filter = "JPEG|*.jpg";
            this.ofdImages.Multiselect = true;
            this.ofdImages.Title = "Select camera images for calibration";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 37);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbImages);
            this.splitContainer1.Panel1.Controls.Add(this.cmdCalibrate);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvResults);
            this.splitContainer1.Panel2.Controls.Add(this.lblSummary);
            this.splitContainer1.Panel2.Controls.Add(this.cmdCancel);
            this.splitContainer1.Panel2.Controls.Add(this.cmdAccept);
            this.splitContainer1.Panel2.Controls.Add(this.iePreview);
            this.splitContainer1.Size = new System.Drawing.Size(688, 416);
            this.splitContainer1.SplitterDistance = 229;
            this.splitContainer1.TabIndex = 2;
            // 
            // cmdCalibrate
            // 
            this.cmdCalibrate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCalibrate.Location = new System.Drawing.Point(151, 388);
            this.cmdCalibrate.Name = "cmdCalibrate";
            this.cmdCalibrate.Size = new System.Drawing.Size(75, 23);
            this.cmdCalibrate.TabIndex = 2;
            this.cmdCalibrate.Text = "Calibrate";
            this.cmdCalibrate.UseVisualStyleBackColor = true;
            this.cmdCalibrate.Click += new System.EventHandler(this.cmdCalibrate_Click);
            // 
            // lvResults
            // 
            this.lvResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvResults.Location = new System.Drawing.Point(3, 346);
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(287, 67);
            this.lvResults.TabIndex = 4;
            this.lvResults.UseCompatibleStateImageBehavior = false;
            this.lvResults.View = System.Windows.Forms.View.List;
            // 
            // lblSummary
            // 
            this.lblSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSummary.AutoSize = true;
            this.lblSummary.Location = new System.Drawing.Point(84, 393);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(0, 13);
            this.lblSummary.TabIndex = 3;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.Location = new System.Drawing.Point(296, 388);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdAccept
            // 
            this.cmdAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAccept.Enabled = false;
            this.cmdAccept.Location = new System.Drawing.Point(377, 388);
            this.cmdAccept.Name = "cmdAccept";
            this.cmdAccept.Size = new System.Drawing.Size(75, 23);
            this.cmdAccept.TabIndex = 0;
            this.cmdAccept.Text = "Accept";
            this.cmdAccept.UseVisualStyleBackColor = true;
            this.cmdAccept.Click += new System.EventHandler(this.cmdAccept_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(712, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCalibrationToolStripMenuItem,
            this.saveCalibrationToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openCalibrationToolStripMenuItem
            // 
            this.openCalibrationToolStripMenuItem.Name = "openCalibrationToolStripMenuItem";
            this.openCalibrationToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.openCalibrationToolStripMenuItem.Text = "&Open calibration";
            // 
            // saveCalibrationToolStripMenuItem
            // 
            this.saveCalibrationToolStripMenuItem.Enabled = false;
            this.saveCalibrationToolStripMenuItem.Name = "saveCalibrationToolStripMenuItem";
            this.saveCalibrationToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.saveCalibrationToolStripMenuItem.Text = "&Save calibration";
            // 
            // CameraCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 465);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CameraCalibration";
            this.Text = "CameraCalibration";
            this.Load += new System.EventHandler(this.CameraCalibration_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbImages;
        private ImageExplorer iePreview;
        private System.Windows.Forms.OpenFileDialog ofdImages;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdAccept;
        private System.Windows.Forms.Button cmdCalibrate;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCalibrationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCalibrationToolStripMenuItem;
        private System.Windows.Forms.ListView lvResults;
    }
}
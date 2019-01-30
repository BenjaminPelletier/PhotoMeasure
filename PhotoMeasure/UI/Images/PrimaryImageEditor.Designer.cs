namespace PhotoMeasure.UI
{
    partial class PrimaryImageEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrimaryImageEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbSelect = new System.Windows.Forms.ToolStripButton();
            this.tsbAddPointFeatures = new System.Windows.Forms.ToolStripButton();
            this.tslStatus = new System.Windows.Forms.ToolStripLabel();
            this.tstbFeatureName = new System.Windows.Forms.ToolStripTextBox();
            this.tsbMatchPointFeatures = new System.Windows.Forms.ToolStripButton();
            this.ieImage = new PhotoMeasure.UI.ImageExplorer();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSelect,
            this.tsbAddPointFeatures,
            this.tslStatus,
            this.tstbFeatureName,
            this.tsbMatchPointFeatures});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(379, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbSelect
            // 
            this.tsbSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSelect.Image = ((System.Drawing.Image)(resources.GetObject("tsbSelect.Image")));
            this.tsbSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSelect.Name = "tsbSelect";
            this.tsbSelect.Size = new System.Drawing.Size(23, 22);
            this.tsbSelect.Text = "Select";
            this.tsbSelect.Click += new System.EventHandler(this.tsbCursor_Click);
            // 
            // tsbAddPointFeatures
            // 
            this.tsbAddPointFeatures.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddPointFeatures.Image = ((System.Drawing.Image)(resources.GetObject("tsbAddPointFeatures.Image")));
            this.tsbAddPointFeatures.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddPointFeatures.Name = "tsbAddPointFeatures";
            this.tsbAddPointFeatures.Size = new System.Drawing.Size(23, 22);
            this.tsbAddPointFeatures.Text = "Add point features";
            this.tsbAddPointFeatures.Click += new System.EventHandler(this.tsbAddPointFeatures_Click);
            // 
            // tslStatus
            // 
            this.tslStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tslStatus.Name = "tslStatus";
            this.tslStatus.Size = new System.Drawing.Size(42, 22);
            this.tslStatus.Text = "Cursor";
            // 
            // tstbFeatureName
            // 
            this.tstbFeatureName.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tstbFeatureName.Name = "tstbFeatureName";
            this.tstbFeatureName.Size = new System.Drawing.Size(100, 25);
            this.tstbFeatureName.Text = "Point";
            this.tstbFeatureName.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tstbFeatureName.Visible = false;
            // 
            // tsbMatchPointFeatures
            // 
            this.tsbMatchPointFeatures.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMatchPointFeatures.Image = ((System.Drawing.Image)(resources.GetObject("tsbMatchPointFeatures.Image")));
            this.tsbMatchPointFeatures.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMatchPointFeatures.Name = "tsbMatchPointFeatures";
            this.tsbMatchPointFeatures.Size = new System.Drawing.Size(23, 22);
            this.tsbMatchPointFeatures.Text = "Match point features";
            this.tsbMatchPointFeatures.Click += new System.EventHandler(this.tsbMatchPointFeatures_Click);
            // 
            // ieImage
            // 
            this.ieImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ieImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ieImage.Location = new System.Drawing.Point(3, 28);
            this.ieImage.Name = "ieImage";
            this.ieImage.Size = new System.Drawing.Size(375, 308);
            this.ieImage.TabIndex = 0;
            this.ieImage.PaintImage += new System.EventHandler<System.Windows.Forms.PaintEventArgs>(this.ieImage_PaintImage);
            this.ieImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ieImage_MouseDown);
            // 
            // PrimaryImageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.ieImage);
            this.Name = "PrimaryImageEditor";
            this.Size = new System.Drawing.Size(379, 337);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageExplorer ieImage;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbSelect;
        private System.Windows.Forms.ToolStripButton tsbAddPointFeatures;
        private System.Windows.Forms.ToolStripLabel tslStatus;
        private System.Windows.Forms.ToolStripTextBox tstbFeatureName;
        private System.Windows.Forms.ToolStripButton tsbMatchPointFeatures;
    }
}

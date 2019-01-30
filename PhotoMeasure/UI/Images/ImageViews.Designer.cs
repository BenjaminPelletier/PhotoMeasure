namespace PhotoMeasure.UI
{
    partial class ImageViews
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
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.piePrimary = new PhotoMeasure.UI.PrimaryImageEditor();
            this.rieReference = new PhotoMeasure.UI.ReferenceImageViewer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.piePrimary);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.rieReference);
            this.splitContainer4.Size = new System.Drawing.Size(434, 549);
            this.splitContainer4.SplitterDistance = 264;
            this.splitContainer4.TabIndex = 1;
            // 
            // piePrimary
            // 
            this.piePrimary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.piePrimary.Location = new System.Drawing.Point(3, 3);
            this.piePrimary.Name = "piePrimary";
            this.piePrimary.Size = new System.Drawing.Size(428, 258);
            this.piePrimary.TabIndex = 0;
            // 
            // rieReference
            // 
            this.rieReference.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rieReference.Location = new System.Drawing.Point(3, 3);
            this.rieReference.Name = "rieReference";
            this.rieReference.Size = new System.Drawing.Size(428, 275);
            this.rieReference.TabIndex = 2;
            // 
            // ImageViews
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer4);
            this.Name = "ImageViews";
            this.Size = new System.Drawing.Size(434, 549);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer4;
        private PrimaryImageEditor piePrimary;
        private UI.ReferenceImageViewer rieReference;
    }
}

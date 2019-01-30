namespace PhotoMeasure.UI
{
    partial class ReferenceImageViewer
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
            this.ieImage = new PhotoMeasure.UI.ImageExplorer();
            this.SuspendLayout();
            // 
            // ieImage
            // 
            this.ieImage.AllowDrop = true;
            this.ieImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ieImage.Location = new System.Drawing.Point(1, 1);
            this.ieImage.Name = "ieImage";
            this.ieImage.Size = new System.Drawing.Size(427, 352);
            this.ieImage.TabIndex = 0;
            this.ieImage.PaintImage += new System.EventHandler<System.Windows.Forms.PaintEventArgs>(this.ieViewer_PaintImage);
            this.ieImage.DragDrop += new System.Windows.Forms.DragEventHandler(this.ieViewer_DragDrop);
            this.ieImage.DragEnter += new System.Windows.Forms.DragEventHandler(this.ieViewer_DragEnter);
            // 
            // ReferenceImageViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.ieImage);
            this.Name = "ReferenceImageViewer";
            this.Size = new System.Drawing.Size(429, 354);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageExplorer ieImage;
    }
}

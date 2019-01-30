namespace PhotoMeasure.UI
{
    partial class ImageList
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
            this.components = new System.ComponentModel.Container();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.lvImages = new System.Windows.Forms.ListView();
            this.ofdImage = new System.Windows.Forms.OpenFileDialog();
            this.ilThumbs = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // cmdAdd
            // 
            this.cmdAdd.Location = new System.Drawing.Point(3, 3);
            this.cmdAdd.Name = "cmdAdd";
            this.cmdAdd.Size = new System.Drawing.Size(24, 24);
            this.cmdAdd.TabIndex = 0;
            this.cmdAdd.Text = "+";
            this.cmdAdd.UseVisualStyleBackColor = true;
            this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // lvImages
            // 
            this.lvImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvImages.HideSelection = false;
            this.lvImages.Location = new System.Drawing.Point(3, 33);
            this.lvImages.Name = "lvImages";
            this.lvImages.OwnerDraw = true;
            this.lvImages.Size = new System.Drawing.Size(216, 251);
            this.lvImages.TabIndex = 1;
            this.lvImages.UseCompatibleStateImageBehavior = false;
            this.lvImages.View = System.Windows.Forms.View.List;
            this.lvImages.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lvImages_DrawItem);
            this.lvImages.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvImages_ItemDrag);
            this.lvImages.SelectedIndexChanged += new System.EventHandler(this.lvImages_SelectedIndexChanged);
            // 
            // ofdImage
            // 
            this.ofdImage.DefaultExt = "jpg";
            this.ofdImage.FileName = "*.jpg";
            this.ofdImage.Filter = "JPEG|*.jpg";
            this.ofdImage.Multiselect = true;
            // 
            // ilThumbs
            // 
            this.ilThumbs.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilThumbs.ImageSize = new System.Drawing.Size(32, 32);
            this.ilThumbs.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ImageList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvImages);
            this.Controls.Add(this.cmdAdd);
            this.Name = "ImageList";
            this.Size = new System.Drawing.Size(222, 287);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.ListView lvImages;
        private System.Windows.Forms.OpenFileDialog ofdImage;
        private System.Windows.Forms.ImageList ilThumbs;
    }
}

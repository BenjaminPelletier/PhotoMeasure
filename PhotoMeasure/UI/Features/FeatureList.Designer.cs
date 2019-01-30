namespace PhotoMeasure.UI
{
    partial class FeatureList
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
            this.cmdAdd = new System.Windows.Forms.Button();
            this.lvFeatures = new System.Windows.Forms.ListView();
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
            // lvFeatures
            // 
            this.lvFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFeatures.HideSelection = false;
            this.lvFeatures.Location = new System.Drawing.Point(3, 33);
            this.lvFeatures.Name = "lvFeatures";
            this.lvFeatures.OwnerDraw = true;
            this.lvFeatures.Size = new System.Drawing.Size(157, 156);
            this.lvFeatures.TabIndex = 1;
            this.lvFeatures.UseCompatibleStateImageBehavior = false;
            this.lvFeatures.View = System.Windows.Forms.View.List;
            this.lvFeatures.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lvFeatures_DrawItem);
            this.lvFeatures.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvFeatures_ItemSelectionChanged);
            // 
            // FeatureList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvFeatures);
            this.Controls.Add(this.cmdAdd);
            this.Name = "FeatureList";
            this.Size = new System.Drawing.Size(163, 192);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.ListView lvFeatures;
    }
}

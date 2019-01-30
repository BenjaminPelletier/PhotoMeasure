namespace PhotoMeasure.UI.Constraints
{
    partial class DistanceBetweenTwoPointsControl
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
            this.cbPoint1 = new System.Windows.Forms.ComboBox();
            this.cbPoint2 = new System.Windows.Forms.ComboBox();
            this.tbDistance = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbPoint1
            // 
            this.cbPoint1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbPoint1.FormattingEnabled = true;
            this.cbPoint1.Location = new System.Drawing.Point(3, 3);
            this.cbPoint1.Name = "cbPoint1";
            this.cbPoint1.Size = new System.Drawing.Size(193, 21);
            this.cbPoint1.TabIndex = 0;
            this.cbPoint1.TextChanged += new System.EventHandler(this.cbPoint1_TextChanged);
            // 
            // cbPoint2
            // 
            this.cbPoint2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbPoint2.FormattingEnabled = true;
            this.cbPoint2.Location = new System.Drawing.Point(3, 56);
            this.cbPoint2.Name = "cbPoint2";
            this.cbPoint2.Size = new System.Drawing.Size(193, 21);
            this.cbPoint2.TabIndex = 1;
            this.cbPoint2.TextChanged += new System.EventHandler(this.cbPoint2_TextChanged);
            // 
            // tbDistance
            // 
            this.tbDistance.Location = new System.Drawing.Point(23, 30);
            this.tbDistance.Name = "tbDistance";
            this.tbDistance.Size = new System.Drawing.Size(74, 20);
            this.tbDistance.TabIndex = 2;
            this.tbDistance.Text = "1.0";
            this.tbDistance.TextChanged += new System.EventHandler(this.tbDistance_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "is";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(103, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "away from";
            // 
            // DistanceBetweenTwoPointsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDistance);
            this.Controls.Add(this.cbPoint2);
            this.Controls.Add(this.cbPoint1);
            this.Name = "DistanceBetweenTwoPointsControl";
            this.Size = new System.Drawing.Size(200, 82);
            this.Load += new System.EventHandler(this.DistanceBetweenTwoPointsControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbPoint1;
        private System.Windows.Forms.ComboBox cbPoint2;
        private System.Windows.Forms.TextBox tbDistance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

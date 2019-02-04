namespace PhotoMeasure.UI
{
    partial class Main
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.gbImages = new System.Windows.Forms.GroupBox();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.gbFeatures = new System.Windows.Forms.GroupBox();
            this.gbConstraints = new System.Windows.Forms.GroupBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showcalibrationScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calibrateFromCheckerboardImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAzpiEl0ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdProject = new System.Windows.Forms.OpenFileDialog();
            this.sfdProject = new System.Windows.Forms.SaveFileDialog();
            this.ilImages = new PhotoMeasure.UI.ImageList();
            this.flFeatures = new PhotoMeasure.UI.FeatureList();
            this.clConstraints = new PhotoMeasure.UI.Constraints.ConstraintList();
            this.ivImageView = new PhotoMeasure.UI.ImageViews();
            this.sstSceneControl = new PhotoMeasure.UI.Scenes.SceneSolveTracker();
            this.svPreview = new PhotoMeasure.UI.Scenes.SceneView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.gbImages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.gbFeatures.SuspendLayout();
            this.gbConstraints.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(1117, 582);
            this.splitContainer1.SplitterDistance = 219;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.gbImages);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer2.Size = new System.Drawing.Size(219, 582);
            this.splitContainer2.SplitterDistance = 235;
            this.splitContainer2.TabIndex = 0;
            // 
            // gbImages
            // 
            this.gbImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbImages.Controls.Add(this.ilImages);
            this.gbImages.Location = new System.Drawing.Point(0, 0);
            this.gbImages.Name = "gbImages";
            this.gbImages.Size = new System.Drawing.Size(219, 236);
            this.gbImages.TabIndex = 0;
            this.gbImages.TabStop = false;
            this.gbImages.Text = "Images";
            // 
            // splitContainer4
            // 
            this.splitContainer4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer4.Location = new System.Drawing.Point(3, 3);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.gbFeatures);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.gbConstraints);
            this.splitContainer4.Size = new System.Drawing.Size(213, 337);
            this.splitContainer4.SplitterDistance = 168;
            this.splitContainer4.TabIndex = 1;
            // 
            // gbFeatures
            // 
            this.gbFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFeatures.Controls.Add(this.flFeatures);
            this.gbFeatures.Location = new System.Drawing.Point(0, 0);
            this.gbFeatures.Name = "gbFeatures";
            this.gbFeatures.Size = new System.Drawing.Size(216, 169);
            this.gbFeatures.TabIndex = 0;
            this.gbFeatures.TabStop = false;
            this.gbFeatures.Text = "Features";
            // 
            // gbConstraints
            // 
            this.gbConstraints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbConstraints.Controls.Add(this.clConstraints);
            this.gbConstraints.Location = new System.Drawing.Point(3, 3);
            this.gbConstraints.Name = "gbConstraints";
            this.gbConstraints.Size = new System.Drawing.Size(207, 159);
            this.gbConstraints.TabIndex = 1;
            this.gbConstraints.TabStop = false;
            this.gbConstraints.Text = "Constraints";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.ivImageView);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.sstSceneControl);
            this.splitContainer3.Panel2.Controls.Add(this.svPreview);
            this.splitContainer3.Size = new System.Drawing.Size(894, 582);
            this.splitContainer3.SplitterDistance = 575;
            this.splitContainer3.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.cameraToolStripMenuItem,
            this.sceneToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1117, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.saveProjectasToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.openProjectToolStripMenuItem.Text = "&Open project";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveProjectToolStripMenuItem.Text = "&Save project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // saveProjectasToolStripMenuItem
            // 
            this.saveProjectasToolStripMenuItem.Name = "saveProjectasToolStripMenuItem";
            this.saveProjectasToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveProjectasToolStripMenuItem.Text = "Save project &as...";
            this.saveProjectasToolStripMenuItem.Click += new System.EventHandler(this.saveProjectasToolStripMenuItem_Click);
            // 
            // cameraToolStripMenuItem
            // 
            this.cameraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showcalibrationScreenToolStripMenuItem,
            this.calibrateFromCheckerboardImagesToolStripMenuItem});
            this.cameraToolStripMenuItem.Name = "cameraToolStripMenuItem";
            this.cameraToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.cameraToolStripMenuItem.Text = "&Camera";
            // 
            // showcalibrationScreenToolStripMenuItem
            // 
            this.showcalibrationScreenToolStripMenuItem.Name = "showcalibrationScreenToolStripMenuItem";
            this.showcalibrationScreenToolStripMenuItem.Size = new System.Drawing.Size(266, 22);
            this.showcalibrationScreenToolStripMenuItem.Text = "Show calibration &screen";
            this.showcalibrationScreenToolStripMenuItem.Click += new System.EventHandler(this.showcalibrationScreenToolStripMenuItem_Click);
            // 
            // calibrateFromCheckerboardImagesToolStripMenuItem
            // 
            this.calibrateFromCheckerboardImagesToolStripMenuItem.Name = "calibrateFromCheckerboardImagesToolStripMenuItem";
            this.calibrateFromCheckerboardImagesToolStripMenuItem.Size = new System.Drawing.Size(266, 22);
            this.calibrateFromCheckerboardImagesToolStripMenuItem.Text = "&Calibrate from checkerboard images";
            this.calibrateFromCheckerboardImagesToolStripMenuItem.Click += new System.EventHandler(this.calibrateFromCheckerboardImagesToolStripMenuItem_Click);
            // 
            // sceneToolStripMenuItem
            // 
            this.sceneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.solveToolStripMenuItem});
            this.sceneToolStripMenuItem.Name = "sceneToolStripMenuItem";
            this.sceneToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.sceneToolStripMenuItem.Text = "&Scene";
            // 
            // solveToolStripMenuItem
            // 
            this.solveToolStripMenuItem.Name = "solveToolStripMenuItem";
            this.solveToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.solveToolStripMenuItem.Text = "&Solve";
            this.solveToolStripMenuItem.Click += new System.EventHandler(this.solveToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setAzpiEl0ToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // setAzpiEl0ToolStripMenuItem
            // 
            this.setAzpiEl0ToolStripMenuItem.Name = "setAzpiEl0ToolStripMenuItem";
            this.setAzpiEl0ToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.setAzpiEl0ToolStripMenuItem.Text = "Set az=pi, el=0";
            this.setAzpiEl0ToolStripMenuItem.Click += new System.EventHandler(this.setAzpiEl0ToolStripMenuItem_Click);
            // 
            // ofdProject
            // 
            this.ofdProject.DefaultExt = "pmp";
            this.ofdProject.FileName = "PhotoMeasure.pmproject";
            this.ofdProject.Filter = "PhotoMeasure project|*.pmproject";
            // 
            // sfdProject
            // 
            this.sfdProject.DefaultExt = "pmp";
            this.sfdProject.FileName = "PhotoMeasure.pmproject";
            this.sfdProject.Filter = "PhotoMeasure project|*.pmproject";
            // 
            // ilImages
            // 
            this.ilImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ilImages.Location = new System.Drawing.Point(6, 19);
            this.ilImages.Name = "ilImages";
            this.ilImages.Size = new System.Drawing.Size(207, 211);
            this.ilImages.TabIndex = 0;
            this.ilImages.IntrinsicsRequired += new System.EventHandler<PhotoMeasure.UI.CameraIntrinsicsRequiredEventArgs>(this.ilImages_IntrinsicsRequired);
            this.ilImages.ImageChanged += new System.EventHandler<PhotoMeasure.UI.ImageList.ImageChangedEventArgs>(this.ilImages_ImageChanged);
            // 
            // flFeatures
            // 
            this.flFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flFeatures.Location = new System.Drawing.Point(6, 13);
            this.flFeatures.Name = "flFeatures";
            this.flFeatures.Size = new System.Drawing.Size(204, 144);
            this.flFeatures.TabIndex = 1;
            this.flFeatures.FeaturesSelected += new System.EventHandler<PhotoMeasure.UI.FeatureList.FeaturesSelectedEventArgs>(this.flFeatures_FeaturesSelected);
            this.flFeatures.FeaturesChanged += new System.EventHandler(this.flFeatures_FeaturesChanged);
            // 
            // clConstraints
            // 
            this.clConstraints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clConstraints.Location = new System.Drawing.Point(6, 19);
            this.clConstraints.Name = "clConstraints";
            this.clConstraints.Size = new System.Drawing.Size(195, 134);
            this.clConstraints.TabIndex = 0;
            // 
            // ivImageView
            // 
            this.ivImageView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ivImageView.Location = new System.Drawing.Point(3, 3);
            this.ivImageView.Name = "ivImageView";
            this.ivImageView.Size = new System.Drawing.Size(569, 576);
            this.ivImageView.TabIndex = 0;
            this.ivImageView.MatchPointsMode += new System.EventHandler(this.ivImageView_MatchPointsMode);
            this.ivImageView.NextFeatureRequested += new System.EventHandler(this.ivImageView_NextFeatureRequested);
            this.ivImageView.NewFeature += new System.EventHandler<PhotoMeasure.UI.PrimaryImageEditor.NewFeatureEventArgs>(this.ivImageView_NewFeature);
            // 
            // sstSceneControl
            // 
            this.sstSceneControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sstSceneControl.Location = new System.Drawing.Point(3, 556);
            this.sstSceneControl.Name = "sstSceneControl";
            this.sstSceneControl.Size = new System.Drawing.Size(309, 23);
            this.sstSceneControl.TabIndex = 1;
            this.sstSceneControl.SceneChanged += new System.EventHandler<PhotoMeasure.UI.Scenes.LocatedSceneEventArgs>(this.sstSceneControl_SceneChanged);
            // 
            // svPreview
            // 
            this.svPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.svPreview.BackColor = System.Drawing.Color.Black;
            this.svPreview.FieldOfViewVertical = 1.0471975511965976D;
            this.svPreview.Location = new System.Drawing.Point(3, 3);
            this.svPreview.Name = "svPreview";
            this.svPreview.Size = new System.Drawing.Size(309, 547);
            this.svPreview.TabIndex = 0;
            this.svPreview.VSync = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1117, 606);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Main";
            this.Text = "PhotoMeasure";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.gbImages.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.gbFeatures.ResumeLayout(false);
            this.gbConstraints.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox gbImages;
        private ImageList ilImages;
        private System.Windows.Forms.GroupBox gbFeatures;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private UI.FeatureList flFeatures;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectasToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog ofdProject;
        private System.Windows.Forms.SaveFileDialog sfdProject;
        private UI.ImageViews ivImageView;
        private System.Windows.Forms.ToolStripMenuItem cameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showcalibrationScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calibrateFromCheckerboardImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solveToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.GroupBox gbConstraints;
        private Constraints.ConstraintList clConstraints;
        private Scenes.SceneView svPreview;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setAzpiEl0ToolStripMenuItem;
        private Scenes.SceneSolveTracker sstSceneControl;
    }
}


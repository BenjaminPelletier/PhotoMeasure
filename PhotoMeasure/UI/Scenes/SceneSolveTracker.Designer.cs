namespace PhotoMeasure.UI.Scenes
{
    partial class SceneSolveTracker
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
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.cmdSolver = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.cmsSolver = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tstbMaxFunctionEvaluations = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsSolver.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(3, 0);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(113, 23);
            this.pbProgress.TabIndex = 0;
            // 
            // cmdSolver
            // 
            this.cmdSolver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSolver.Location = new System.Drawing.Point(352, 0);
            this.cmdSolver.Name = "cmdSolver";
            this.cmdSolver.Size = new System.Drawing.Size(55, 23);
            this.cmdSolver.TabIndex = 1;
            this.cmdSolver.Text = "Solver";
            this.cmdSolver.UseVisualStyleBackColor = true;
            this.cmdSolver.Click += new System.EventHandler(this.cmdSolver_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(122, 5);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(35, 13);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "label1";
            // 
            // cmsSolver
            // 
            this.cmsSolver.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cancelToolStripMenuItem,
            this.toolStripSeparator1,
            this.tstbMaxFunctionEvaluations});
            this.cmsSolver.Name = "cmsSolver";
            this.cmsSolver.Size = new System.Drawing.Size(161, 79);
            this.cmsSolver.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.cmsSolver_Closing);
            // 
            // cancelToolStripMenuItem
            // 
            this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            this.cancelToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.cancelToolStripMenuItem.Text = "&Cancel";
            this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
            // 
            // tstbMaxFunctionEvaluations
            // 
            this.tstbMaxFunctionEvaluations.Name = "tstbMaxFunctionEvaluations";
            this.tstbMaxFunctionEvaluations.Size = new System.Drawing.Size(100, 23);
            this.tstbMaxFunctionEvaluations.Text = "10000";
            this.tstbMaxFunctionEvaluations.Leave += new System.EventHandler(this.tstbMaxFunctionEvaluations_Leave);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // SceneSolveTracker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdSolver);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.lblMessage);
            this.Name = "SceneSolveTracker";
            this.Size = new System.Drawing.Size(410, 127);
            this.cmsSolver.ResumeLayout(false);
            this.cmsSolver.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button cmdSolver;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ContextMenuStrip cmsSolver;
        private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox tstbMaxFunctionEvaluations;
    }
}

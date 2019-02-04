using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Easy3D.Scenes;
using Easy3D.Numerics;

namespace PhotoMeasure.UI.Scenes
{
    public partial class SceneSolveTracker : UserControl
    {
        private SceneSolver _Solver = new SceneSolver();
        private NelderMead.Configuration _Config = new NelderMead.Configuration();

        public event EventHandler<LocatedSceneEventArgs> SceneChanged;

        public SceneSolveTracker()
        {
            InitializeComponent();
            _Solver.SceneSolveProgress += Solver_SceneSolveProgress;
            _Solver.SceneSolved += Solver_SceneSolved;
        }

        public void StartSolving(Scene scene)
        {
            _Solver.StartSolving(scene, _Config);
        }

        private void Solver_SceneSolveProgress(object sender, SceneSolver.SceneSolveProgressEventArgs e)
        {
            cancelToolStripMenuItem.Enabled = true;
            pbProgress.Visible = true;
            pbProgress.Value = (int)(100 * Math.Min((float)e.SimplexIteration.FunctionCount / e.Solver.Config.MaximumFunctionEvaluations, 1));
            lblMessage.Text = $"Error: {e.SimplexIteration.Error:f2} pixels decreasing at {e.SimplexIteration.dF:f5}, {e.SimplexIteration.FunctionCount} evaluations";
            SceneChanged?.Invoke(this, new LocatedSceneEventArgs(e.LocatedScene));
        }

        private void Solver_SceneSolved(object sender, SceneSolver.SceneSolveProgressEventArgs e)
        {
            cancelToolStripMenuItem.Enabled = false;
            pbProgress.Visible = false;
            lblMessage.Text = "Solver completed.";
            SceneChanged?.Invoke(this, new LocatedSceneEventArgs(e.LocatedScene));
        }

        private void cmdSolver_Click(object sender, EventArgs e)
        {
            tstbMaxFunctionEvaluations.Text = _Config.MaximumFunctionEvaluations.ToString();
            cmsSolver.Show(cmdSolver, 0, 0);
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _Solver.Cancel();
        }

        private void tstbMaxFunctionEvaluations_Leave(object sender, EventArgs e)
        {
            int maxFunEvals = _Config.MaximumFunctionEvaluations;
            if (int.TryParse(tstbMaxFunctionEvaluations.Text, out maxFunEvals))
            {
                _Config.MaximumFunctionEvaluations = maxFunEvals;
            }
        }

        private void cmsSolver_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            tstbMaxFunctionEvaluations_Leave(sender, e);
        }
    }
}

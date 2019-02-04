using Easy3D.Numerics;
using Easy3D.Scenes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes
{
    public class SceneSolver
    {
        public class SceneSolveProgressEventArgs : EventArgs
        {
            public LocatedScene LocatedScene;
            public NelderMead Solver;
            public NelderMead.SimplexIterationEventArgs SimplexIteration;

            public SceneSolveProgressEventArgs(LocatedScene scene, NelderMead solver, NelderMead.SimplexIterationEventArgs iteration)
            {
                this.LocatedScene = scene;
                this.Solver = solver;
                this.SimplexIteration = iteration;
            }
        }

        private class SolveRequest
        {
            public Scene Scene;
            public NelderMead.Configuration Config;
        }

        private BackgroundWorker _Worker;

        public event EventHandler<SceneSolveProgressEventArgs> SceneSolveProgress;
        public event EventHandler<SceneSolveProgressEventArgs> SceneSolved;

        public SceneSolver()
        {
            _Worker = new BackgroundWorker();
            _Worker.WorkerReportsProgress = true;
            _Worker.WorkerSupportsCancellation = true;
            _Worker.ProgressChanged += Worker_ProgressChanged;
            _Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _Worker.DoWork += this.DoWork;
        }

        public void StartSolving(Scene scene, NelderMead.Configuration config)
        {
            if (_Worker.IsBusy)
            {
                throw new InvalidOperationException("Can't start solve while another solve is running");
            }
            
            _Worker.RunWorkerAsync(new SolveRequest() { Scene = scene, Config = config });
        }

        public void Cancel()
        {
            if (_Worker.IsBusy)
            {
                _Worker.CancelAsync();
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            SolveRequest request = e.Argument as SolveRequest;
            Scene scene = request.Scene;

            var solver = new NelderMead(request.Config);
            var sceneMaker = new LocatedSceneMaker(scene);

            Func<double[], double> errorFunction = p =>
            {
                double errSum = 0;
                LocatedScene locatedScene = sceneMaker.MakeScene(p);
                double[] errorVector = locatedScene.ErrorVector.ToArray();
                foreach (double err in errorVector)
                {
                    errSum += err * err;
                }
                return Math.Sqrt(errSum / errorVector.Length);
            };

            solver.SimplexIteration += (solverSender, iterationArgs) =>
            {
                worker.ReportProgress(0, new SceneSolveProgressEventArgs(sceneMaker.MakeScene(iterationArgs.x.Peek().x), solver, iterationArgs));
                iterationArgs.Cancel = worker.CancellationPending;
            };

            double[] bestParameters = solver.FindMinimum(errorFunction, sceneMaker.GetInitialGuess().ToArray(), 0.1);

            e.Result = sceneMaker.MakeScene(bestParameters);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progressArgs = e.UserState as SceneSolveProgressEventArgs;
            SceneSolveProgress?.Invoke(this, progressArgs);
            if (progressArgs.SimplexIteration.FinalUpdate)
            {
                SceneSolved?.Invoke(this, progressArgs);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}

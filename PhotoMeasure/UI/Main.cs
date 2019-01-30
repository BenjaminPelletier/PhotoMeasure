using Easy3D.Geometry;
using Easy3D.Numerics;
using Easy3D.Projection;
using Easy3D.Scenes;
using Easy3D.Scenes.Features;
using EasySerialization.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMeasure.UI
{
    public partial class Main : Form
    {
        private JsonTranslator _Translator = new JsonTranslator();
        private CameraIntrinsics _Camera = null;

        private CameraCalibration _CameraCalibrationDialog = new CameraCalibration();

        public Main()
        {
            InitializeComponent();
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdProject.ShowDialog(this) == DialogResult.OK)
            {
                var project = _Translator.MakeObject<Project>(JsonObject.Parse(File.ReadAllText(ofdProject.FileName)));
                ilImages.Images = project.Scene.Views.Select(v => new PhotoMeasure.ViewImage(v));
                flFeatures.Features = project.Scene.Features;
                clConstraints.Constraints = project.Scene.Constraints;
                _Camera = project.Camera;

                sfdProject.FileName = ofdProject.FileName;
                sfdProject.InitialDirectory = ofdProject.InitialDirectory;
            }
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var project = new Project(GetScene(), _Camera);
            File.WriteAllText(sfdProject.FileName, _Translator.MakeJson(project).ToMultilineString());
        }

        private void saveProjectasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sfdProject.ShowDialog(this) == DialogResult.OK)
            {
                saveProjectToolStripMenuItem_Click(sender, e);
                ofdProject.FileName = sfdProject.FileName;
                ofdProject.InitialDirectory = sfdProject.InitialDirectory;
            }
        }

        private void ilImages_ImageChanged(object sender, ImageList.ImageChangedEventArgs e)
        {
            ivImageView.PrimaryImage = e.Image;
            flFeatures.MeasuredImage = e.Image;
        }

        private void ivImageView_NewFeature(object sender, PrimaryImageEditor.NewFeatureEventArgs e)
        {
            e.Feature = flFeatures.AddFeature(e.Type, e.DesiredName);
        }

        private void showcalibrationScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new Checkerboard(); // TODO: change to singleton
            dlg.Show(this);
        }

        private void calibrateFromCheckerboardImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_CameraCalibrationDialog.ShowDialog(this) == DialogResult.OK)
            {
                _Camera = _CameraCalibrationDialog.Intrinsics;
            }
        }

        private void flFeatures_FeaturesSelected(object sender, FeatureList.FeaturesSelectedEventArgs e)
        {
            ilImages.SelectFeatures(e.SelectedFeatures);
            ivImageView.SelectFeatures(e.SelectedFeatures);
        }

        private void ivImageView_MatchPointsMode(object sender, EventArgs e)
        {
            if (flFeatures.SelectedFeatures.Count() == 0 && flFeatures.Features.Count() > 0)
            {
                flFeatures.SelectedFeatures = new Feature[] { flFeatures.Features.First() };
            }
        }

        private void ivImageView_NextFeatureRequested(object sender, EventArgs e)
        {
            flFeatures.SelectNextFeature();
        }

        private void solveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var loader = new BackgroundWorker();
            Scene scene = GetScene();
            LocatedScene locatedScene = null;
            loader.DoWork += (s, w) =>
            {
                var solver = new NelderMead();

                solver.SimplexIteration += (solverSender, iterationArgs) =>
                {
                    loader.ReportProgress(
                        (100 * iterationArgs.FunctionCount) / solver.MaximumFunctionEvaluations,
                        $"Error: {iterationArgs.Error:f2} pixels decreasing at {iterationArgs.dF:f5}, {iterationArgs.FunctionCount} evaluations");
                    iterationArgs.Cancel = w.Cancel;
                };

                locatedScene = Solver.Solve(scene, solver);
            };

            var progress = new ProgressDialog(loader, "Solving scene");
            if (progress.ShowDialog(this) == DialogResult.Abort)
            {
                MessageBox.Show(this, "Aborted");
                return;
            }

            svPreview.Scene = locatedScene;
        }

        private void flFeatures_FeaturesChanged(object sender, EventArgs e)
        {
            clConstraints.Features = flFeatures.Features;
        }

        private Scene GetScene()
        {
            return new Scene(ilImages.Images, flFeatures.Features, clConstraints.Constraints);
        }

        private void ilImages_IntrinsicsRequired(object sender, CameraIntrinsicsRequiredEventArgs e)
        {
            if (_Camera != null)
            {
                e.Intrinsics = _Camera;
            }
        }

        private void setAzpiEl0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            svPreview.SetAzEl(Math.PI, 0);
        }
    }
}

using Easy3D.Projection;
using OpenCvSharp;
using OpenTK;
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
    public partial class CameraCalibration : Form
    {
        private const int POINT_RADIUS = 5;
        private static readonly OpenCvSharp.Size SIZE = new OpenCvSharp.Size(9, 6);
        
        class CalibrationImage
        {
            public enum Status
            {
                Ok,
                Incomplete,
                NotDetected
            }

            public string Path;
            public string Name;
            public Bitmap Image;
            public Point2f[] Corners;
            public Point2f[] ReprojectedCorners;

            public CalibrationImage(string path, Point2f[] corners)
            {
                this.Path = path;
                var fi = new FileInfo(path);
                this.Name = fi.Name;
                this.Image = new Bitmap(path);
                this.Corners = corners;
            }

            public Status DetectionStatus
            {
                get
                {
                    if (Corners == null)
                    {
                        return Status.NotDetected;
                    }
                    else if (Corners.Length < SIZE.Width * SIZE.Height)
                    {
                        return Status.Incomplete;
                    }
                    else
                    {
                        return Status.Ok;
                    }
                }
            }

            public double SumSquareError
            {
                get
                {
                    if (ReprojectedCorners == null) { return double.NaN; }
                    double sse = 0;
                    for (int i = 0; i < Corners.Length; i++)
                    {
                        double dx = Corners[i].X - ReprojectedCorners[i].X;
                        double dy = Corners[i].Y - ReprojectedCorners[i].Y;
                        sse += dx * dx + dy * dy;
                    }
                    return sse;
                }
            }

            public override string ToString()
            {
                return "[" + this.DetectionStatus + "] " + this.Name;
            }
        }

        public CameraIntrinsics Intrinsics;

        public CameraCalibration()
        {
            InitializeComponent();
        }

        private void CameraCalibration_Load(object sender, EventArgs e)
        {
            if (ofdImages.ShowDialog(this) != DialogResult.OK)
            {
                this.Close();
                return;
            }

            var loader = new BackgroundWorker();
            var imgs = new List<CalibrationImage>();
            loader.DoWork += (s, w) =>
            {
                float pComplete = 0;
                float dComplete = 100f / (ofdImages.FileNames.Length * 2);
                foreach (string imgPath in ofdImages.FileNames)
                {
                    var fi = new FileInfo(imgPath);

                    loader.ReportProgress((int)pComplete, "Loading " + fi.Name);
                    pComplete += dComplete;
                    using (Mat img = new Mat(imgPath))
                    {
                        if (w.Cancel) { break; }

                        loader.ReportProgress((int)pComplete, "Finding corners in " + fi.Name);
                        pComplete += dComplete;
                        Point2f[] pts = CameraIntrinsics.FindChessboardCorners(img, SIZE);
                        imgs.Add(new CalibrationImage(imgPath, pts));

                        if (w.Cancel) { break; }
                    }
                }
            };

            var progress = new ProgressDialog(loader, "Finding chessboard corners");
            if (progress.ShowDialog(this) == DialogResult.Abort || imgs.Count == 0)
            {
                this.Close();
            }

            lbImages.Items.AddRange(imgs.ToArray());
        }

        private void lbImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbImages.SelectedIndex == -1)
            {
                iePreview.Image = null;
                return;
            }

            CalibrationImage calImg = lbImages.Items[lbImages.SelectedIndex] as CalibrationImage;
            iePreview.Image = calImg.Image;
        }

        private void iePreview_PaintImage(object sender, PaintEventArgs e)
        {
            if (lbImages.SelectedIndex == -1) { return; }

            CalibrationImage calImg = lbImages.Items[lbImages.SelectedIndex] as CalibrationImage;

            if (calImg.Corners == null) { return; }

            var c = iePreview.Converter;

            foreach (Point2f p2f in calImg.Corners)
            {
                var p = new PointF(c.VpX(p2f.X), c.VpY(p2f.Y));
                e.Graphics.DrawEllipse(Pens.Red, p.X - POINT_RADIUS, p.Y - POINT_RADIUS, 2 * POINT_RADIUS, 2 * POINT_RADIUS);
            }
            
            if (calImg.ReprojectedCorners != null)
            {
                for (int i=0; i<calImg.Corners.Length; i++)
                {
                    var pImg = new PointF(c.VpX(calImg.Corners[i].X), c.VpY(calImg.Corners[i].Y));
                    var pModel = new PointF(c.VpX(calImg.ReprojectedCorners[i].X), c.VpY(calImg.ReprojectedCorners[i].Y));
                    e.Graphics.DrawLine(Pens.Yellow, pImg, pModel);
                    e.Graphics.DrawEllipse(Pens.Green, pModel.X - POINT_RADIUS, pModel.Y - POINT_RADIUS, 2 * POINT_RADIUS, 2 * POINT_RADIUS);
                }
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmdCalibrate_Click(object sender, EventArgs e)
        {
            var calImgs = new List<CalibrationImage>();
            foreach (object item in lbImages.Items)
            {
                calImgs.Add(item as CalibrationImage);
            }
            double rmsError = double.NaN;

            var loader = new BackgroundWorker();
            loader.DoWork += (s, w) =>
            {
                var imgSize = new OpenCvSharp.Size(calImgs[0].Image.Width, calImgs[0].Image.Height);
                List<Point2f[]> cornerSets = calImgs.Where(img => img.DetectionStatus == CalibrationImage.Status.Ok).Select(img => img.Corners).ToList();

                if (cornerSets.Count < 3)
                {
                    throw new InvalidOperationException("Cannot calibrate with less than 3 camera images");
                }

                loader.ReportProgress(0, "Calibrating camera from " + cornerSets.Count + " images");
                Vec3d[] rvecs;
                Vec3d[] tvecs;
                Intrinsics = CameraIntrinsics.FromChessboardCorners(cornerSets, imgSize, SIZE, out rvecs, out tvecs);

                loader.ReportProgress(50, "Finding camera positions and orientations for reprojection");
                double sumSquareError = 0;
                int n = 0;
                int i = 0;
                foreach (CalibrationImage calImg in calImgs)
                {
                    if (calImg.Corners == null)
                    {
                        continue;
                    }
                    Vector3d[] pts3d = CameraIntrinsics.ChessboardPoints(SIZE);
                    LocatedCamera lcam0 = new LocatedCamera(Intrinsics, rvecs[i].ToArray(), tvecs[i].ToArray());
                    LocatedCamera lcam = LocatedCamera.Create(Intrinsics, pts3d, calImg.Corners);
                    calImg.ReprojectedCorners = lcam.Project(pts3d);
                    sumSquareError += calImg.SumSquareError;
                    n += calImg.Corners.Length;
                    i++;
                }
                rmsError = Math.Sqrt(sumSquareError / n);
            };

            var progress = new ProgressDialog(loader, "Calibrating camera");
            if (progress.ShowDialog(this) == DialogResult.OK)
            {
                lblSummary.Text = string.Format("{0:f2} pixels RMS error", rmsError);
                cmdCalibrate.Enabled = false;
                cmdAccept.Enabled = true;

                lvResults.Items.Add("Width=" + Intrinsics.Width);
                lvResults.Items.Add("Height=" + Intrinsics.Height);
                lvResults.Items.Add("k1=" + Intrinsics.Dist[0]);
                lvResults.Items.Add("k2=" + Intrinsics.Dist[1]);
                lvResults.Items.Add("p1=" + Intrinsics.Dist[2]);
                lvResults.Items.Add("p2=" + Intrinsics.Dist[3]);
                lvResults.Items.Add("k3=" + Intrinsics.Dist[4]);
                lvResults.Items.Add("fx=" + Intrinsics.Mat[0, 0]);
                lvResults.Items.Add("fy=" + Intrinsics.Mat[1, 1]);
                lvResults.Items.Add("cx=" + Intrinsics.Mat[0, 2]);
                lvResults.Items.Add("cy=" + Intrinsics.Mat[1, 2]);
            }

            iePreview.Invalidate();
        }

        private void cmdAccept_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

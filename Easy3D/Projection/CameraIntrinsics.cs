using OpenCvSharp;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Projection
{
    /// <summary>
    /// http://docs.opencv.org/2.4/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#calibratecamera
    /// </summary>
    public class CameraIntrinsics
    {
        public int Width;
        public int Height;

        /// <summary>
        /// 3x3 camera matrix
        /// </summary>
        public double[,] Mat;

        /// <summary>
        /// 5-element distortion coefficients (k1,k2,p1,p2,k3)
        /// </summary>
        public double[] Dist;

        public CameraIntrinsics(int width, int height, double[,] mat, double[] dist)
        {
            this.Width = width;
            this.Height = height;
            this.Mat = mat;
            this.Dist = dist;
        }

        public Vector3d Unproject(Point2f pt)
        {
            double fx = Mat[0, 0];
            double fy = Mat[1, 1];
            double cx = Mat[0, 2];
            double cy = Mat[1, 2];

            double xp = (pt.X - cx) / fx;
            double yp = (pt.Y - cy) / fy;
            return new Vector3d((float)xp, (float)yp, 1);
        }

        public static CameraIntrinsics FromChessboardImages(IEnumerable<Mat> chessboardImages, Size chessboardSize)
        {
            List<List<Vector3d>> objpts;
            List<Point2f[]> imgpts;
            List<Mat> goodIms;
            Vec3d[] rvecs, tvecs;
            return FromChessboardImages(chessboardImages,
                chessboardSize, out objpts, out imgpts, out goodIms, out rvecs, out tvecs);
        }

        public static CameraIntrinsics FromChessboardImages(
            IEnumerable<Mat> chessboardImages, Size chessboardSize,
            out List<List<Vector3d>> objpts, out List<Point2f[]> imgpts, out List<Mat> goodIms,
            out Vec3d[] rvecs, out Vec3d[] tvecs,
            int pixelBuffer = 100)
        {
            objpts = new List<List<Vector3d>>();
            imgpts = new List<Point2f[]>();
            goodIms = new List<Mat>();

            Size imgsize = new Size(0, 0);
            foreach (Mat img in chessboardImages)
            {
                imgsize = img.Size();
                Point2f[] pts2d = FindChessboardCorners(img, chessboardSize);

                if (pts2d == null || pts2d.Length < chessboardSize.Width * chessboardSize.Height)
                    continue;

                goodIms.Add(img);

                Vector3d[] pts3d = ChessboardPoints(chessboardSize);

                List<Point2f> pgood = new List<Point2f>();
                List<Vector3d> ogood = new List<Vector3d>();
                for (int i = 0; i < pts2d.Length; i++)
                {
                    double x = pts2d[i].X;
                    double y = pts2d[i].Y;
                    if (x < pixelBuffer || y < pixelBuffer ||
                        x > imgsize.Width - pixelBuffer || y > imgsize.Height - pixelBuffer)
                        continue;

                    pgood.Add(pts2d[i]);
                    ogood.Add(pts3d[i]);
                }

                imgpts.Add(pgood.ToArray());
                objpts.Add(ogood);
                //imgpts.Add(pts2d);
                //objpts.Add(pts3d);

            }

            double[,] mat = new double[3, 3];
            double[] dist = new double[5];
            Cv2.CalibrateCamera(objpts.ToPoint3f(), imgpts, imgsize, mat, dist, out rvecs, out tvecs);

            return new CameraIntrinsics(imgsize.Width, imgsize.Height, mat, dist);
        }

        public static Point2f[] FindChessboardCorners(Mat img, Size chessboardSize)
        {
            var cornerMat = new Mat();
            var corners = OutputArray.Create(cornerMat);
            bool success = Cv2.FindChessboardCorners(img, chessboardSize, corners);
            if (!success) { return null; }

            Point2f[] pts2d = new Point2f[cornerMat.Size(0)];

            cornerMat.GetArray(0, 0, pts2d);

            return pts2d;
        }

        public static CameraIntrinsics FromChessboardCorners(IEnumerable<Point2f[]> cornerSets, Size imgSize, Size chessboardSize, out Vec3d[] rvecs, out Vec3d[] tvecs)
        {
            var objpts = new List<Vector3d[]>();
            var imgpts = new List<Point2f[]>();
            Vector3d[] pts3d = ChessboardPoints(chessboardSize);

            foreach (Point2f[] pts2d in cornerSets)
            {
                if (pts2d == null || pts2d.Length < chessboardSize.Width * chessboardSize.Height)
                    continue;

                imgpts.Add(pts2d);
                objpts.Add(pts3d);
            }

            double[,] mat = new double[3, 3];
            double[] dist = new double[5];
            Cv2.CalibrateCamera(objpts.ToPoint3f(), imgpts, imgSize, mat, dist, out rvecs, out tvecs);

            return new CameraIntrinsics(imgSize.Width, imgSize.Height, mat, dist);
        }

        public static Vector3d[] ChessboardPoints(Size chessboardSize)
        {
            var pts3d = new Vector3d[chessboardSize.Height * chessboardSize.Width];
            int k = 0;
            for (int i = 0; i < chessboardSize.Height; i++)
            {
                for (int j = 0; j < chessboardSize.Width; j++)
                {
                    pts3d[k++] = new Vector3d(j, i, 0);
                }
            }

            return pts3d;
        }
    }
}

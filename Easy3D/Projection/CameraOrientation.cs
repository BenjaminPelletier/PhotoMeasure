using OpenCvSharp;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Projection
{
    public class CameraOrientation
    {
        public readonly Vector3d uAxis;
        public readonly Vector3d vAxis;
        public readonly Vector3d opticAxis;

        /// <summary>
        /// Create a CameraOrientation by defining the u and v axes explicitly.
        /// </summary>
        /// <param name="uAxis">Vector pointing from left to right on the image.</param>
        /// <param name="vAxis">Vector pointing from top to bottom on the image.</param>
        public CameraOrientation(Vector3d uAxis, Vector3d vAxis)
        {
            this.uAxis = uAxis.Normalized();
            this.opticAxis = Vector3d.Cross(uAxis, vAxis).Normalized();

            // Adjust v axis of camera to be orthogonal to the u axis and optic axis
            this.vAxis = Vector3d.Cross(opticAxis, uAxis);
        }

        public CameraOrientation(double[] rvec)
        {
            double[,] m;
            Cv2.Rodrigues(rvec, out m);
            uAxis = new Vector3d(m[0, 0], m[0, 1], m[0, 2]);
            vAxis = new Vector3d(m[1, 0], m[1, 1], m[1, 2]);
            opticAxis = new Vector3d(m[2, 0], m[2, 1], m[2, 2]);
        }

        public double[] rvec
        {
            get
            {
                // Compute the implied Rodrigues rotation to produce the estimated u, v, and optic axes of the camera
                double[] rvec;
                Cv2.Rodrigues(this.m, out rvec);
                return rvec;
            }
        }

        public double[,] m
        {
            get
            {
                return new double[3, 3]
                {
                    {uAxis.X, uAxis.Y, uAxis.Z },
                    {vAxis.X, vAxis.Y, vAxis.Z },
                    {opticAxis.X, opticAxis.Y, opticAxis.Z }
                };
            }
        }

        public static CameraOrientation Guess(CameraIntrinsics intrinsics, Vector3d[] objPts, Point2f[] imgPts)
        {
            // Fit a formula that predicts change in (u,v) screen coordinates based on change in (x,y,z) world coordinates
            double fx = intrinsics.Mat[0, 0];
            double fy = intrinsics.Mat[1, 1];
            double cx = intrinsics.Mat[0, 2];
            double cy = intrinsics.Mat[1, 2];
            Point2f[] screenPts = imgPts.Select(p => new Point2f((float)((p.X - cx) / fx), (float)((p.Y - cy) / fy))).ToArray();
            var x = new double[objPts.Length * (objPts.Length - 1) / 2][];
            var y_u = new double[x.Length];
            var y_v = new double[x.Length];
            int k = 0;
            for (int i = 0; i < objPts.Length - 1; i++)
            {
                for (int j = i + 1; j < objPts.Length; j++)
                {
                    x[k] = new double[] { objPts[i].X - objPts[j].X, objPts[i].Y - objPts[j].Y, objPts[i].Z - objPts[j].Z };
                    y_u[k] = screenPts[i].X - screenPts[j].X;
                    y_v[k] = screenPts[i].Y - screenPts[j].Y;
                    k++;
                }
            }
            var rand = new Random();
            if (x.Skip(1).All(r => r[2] == x[0][2]))
            {
                double radius = 1;
                for (int i = 0; i < x.Length; i++)
                {
                    x[i][2] = rand.Next(-1, 2) * radius * (float)1e6;
                }
            }

            // du = pu[0]*dx + pu[1]*dy + pu[2]*dz
            double[] pu = MathNet.Numerics.LinearRegression.MultipleRegression.DirectMethod(x, y_u);
            // dv = pv[0]*dx + pv[1]*dy + pv[2]*dz
            double[] pv = MathNet.Numerics.LinearRegression.MultipleRegression.DirectMethod(x, y_v);

            // Compute the inverse: Find two 3D vectors.  One of these vectors causes movement only along the u axis in the image,
            // the other vector causes movement only along the v axis in the image; see first 4 equations in the Solve note below.
            // The cross product of these two vectors should cause no movement in either the u axis of the image, nor the v axis
            // of the image (it should be aligned with the optic axis of the camera); see last 2 equations in the Solve note below.
            double u0 = pu[0], u1 = pu[1], u2 = pu[2], v0 = pv[0], v1 = pv[1], v2 = pv[2];

            // ui are the best-fit linear coefficients relating movements in 3D to horizontal image movements
            // vi are the best-fit linear coefficients relating movements in 3D to vertical image movements
            // (ux,uy,uz) is the first vector mentioned above
            // (vx,vy,vz) is the second vector mentioned above
            //FullSimplify[Solve[{
            //   1 == ux u0 + uy u1 + uz u2,
            //   0 == vx u0 + vy u1 + vz u2,
            //   0 == ux v0 + uy v1 + uz v2,
            //   1 == vx v0 + vy v1 + vz v2,
            //   0 == (uy vz - uz vy) u0 + (uz vx - ux vz) u1 + (ux vy - uy vx) u2,
            //   0 == (uy vz - uz vy) v0 + (uz vx - ux vz) v1 + (ux vy - uy vx) v2
            //   }, {ux, uy, uz, vx, vy, vz}]]
            double denom = u2 * u2 * (v0 * v0 + v1 * v1) - 2 * u0 * u2 * v0 * v2 - 2 * u1 * v1 * (u0 * v0 + u2 * v2) + u1 * u1 * (v0 * v0 + v2 * v2) + u0 * u0 * (v1 * v1 + v2 * v2);
            double ux = -(u1 * v0 * v1 + u2 * v0 * v2 - u0 * (v1 * v1 + v2 * v2)) / denom;
            double uy = (-v1 * (u0 * v0 + u2 * v2) + u1 * (v0 * v0 + v2 * v2)) / denom;
            double uz = (u2 * (v0 * v0 + v1 * v1) - (u0 * v0 + u1 * v1) * v2) / denom;
            double vx = (u1 * u1 * v0 - u0 * u1 * v1 + u2 * (u2 * v0 - u0 * v2)) / denom;
            double vy = (-u0 * u1 * v0 + u0 * u0 * v1 + u2 * (u2 * v1 - u1 * v2)) / denom;
            double vz = (-u2 * (u0 * v0 + u1 * v1) + (u0 * u0 + u1 * u1) * v2) / denom;

            return new CameraOrientation(new Vector3d(ux, uy, uz), new Vector3d(vx, vy, vz));
        }
    }
}

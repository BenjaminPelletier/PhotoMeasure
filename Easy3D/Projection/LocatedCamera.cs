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
    /// Camera with a fully-defined position and orientation in addition to intrinsics
    /// </summary>
    public class LocatedCamera
    {
        public CameraIntrinsics Intrinsics;
        private double[] rvec;
        private double[] tvec;
        public double[,] rmat;
        public Vector3d Location;

        /// <summary>
        /// Create a LocatedCamera with the specified intrinsic parameters, rotation, and translation
        /// </summary>
        /// <param name="rvec">Orientation of camera as Rodrigues rotation; see CameraTools.RodriguesToAxisAngle</param>
        /// <param name="tvec">Information about location of camera; see CameraTools.CameraPosition</param>
        public LocatedCamera(CameraIntrinsics intrinsics, double[] rvec = null, double[] tvec = null)
        {
            this.Intrinsics = intrinsics;
            this.rvec = rvec == null ? new double[] { 0, 0, 0 } : rvec;
            this.tvec = tvec == null ? new double[] { 0, 0, 0 } : tvec;
            Cv2.Rodrigues(this.rvec, out this.rmat);
            Location = CameraTools.CameraPosition(this.rmat, this.tvec);
        }

        /// <summary>
        /// Projects a set of points in the 3D world space into the pixels space of this camera
        /// </summary>
        public Point2f[] Project(IEnumerable<Vector3d> objPoints)
        {
            Point2f[] imgPoints;
            double[,] jacobian;
            Cv2.ProjectPoints(objPoints.ToPoint3f(), this.rvec, this.tvec, this.Intrinsics.Mat, this.Intrinsics.Dist, out imgPoints, out jacobian);
            return imgPoints;
        }

        public Point2f Project(Vector3d objPt)
        {
            return Project(new Vector3d[] { objPt })[0];
        }

        public IEnumerable<Vector3d> Unproject(IEnumerable<Point2f> imgPoints)
        {
            foreach (Point2f imgPt in imgPoints)
                yield return Unproject(imgPt);
        }

        /// <summary>
        /// Find the vector, in 3D world space, from the camera focal point pointing along the ray that the specified pixel observes
        /// </summary>
        public Vector3d Unproject(Point2f imgPt)
        {
            Vector3d pt3d = Intrinsics.Unproject(imgPt);

            double r00 = rmat[0, 0], r01 = rmat[0, 1], r02 = rmat[0, 2];
            double r10 = rmat[1, 0], r11 = rmat[1, 1], r12 = rmat[1, 2];
            double r20 = rmat[2, 0], r21 = rmat[2, 1], r22 = rmat[2, 2];
            double x = pt3d.X, y = pt3d.Y, z = pt3d.Z;

            double denom = r02 * r11 * r20 - r01 * r12 * r20 - r02 * r10 * r21 + r00 * r12 * r21 + r01 * r10 * r22 - r00 * r11 * r22;
            Vector3d pt3dr = new Vector3d(
                (float)((r12 * r21 * x - r11 * r22 * x - r02 * r21 * y + r01 * r22 * y + r02 * r11 * z - r01 * r12 * z) / denom),
                -(float)((r12 * r20 * x - r10 * r22 * x - r02 * r20 * y + r00 * r22 * y + r02 * r10 * z - r00 * r12 * z) / denom),
                (float)((r11 * r20 * x - r10 * r21 * x - r01 * r20 * y + r00 * r21 * y + r01 * r10 * z - r00 * r11 * z) / denom));
            return pt3dr;
        }

        public double RmsResidual(IEnumerable<Vector3d> objPoints, IEnumerable<Point2f> imgPoints)
        {
            return Math.Sqrt(imgPoints.Zip(Project(objPoints), (p1, p2) => p1.DistanceTo(p2)).Select(v => v * v).Average());
        }

        public static LocatedCamera Create(CameraIntrinsics intrinsics, IEnumerable<Vector3d> objPoints, IEnumerable<Point2f> imgPoints)
        {
            // Set local variables for calibration resources
            Vector3d[] objPts = objPoints.ToArray();
            Point2f[] imgPts = imgPoints.ToArray();
            double[] dist = intrinsics.Dist;

            // Evaluate bulk characteristics of known 3D calibration points
            Vector3d center = new Vector3d(objPts.Select(p => p.X).Average(), objPts.Select(p => p.Y).Average(), objPts.Select(p => p.Z).Average());
            float radius = (float)Math.Sqrt(objPts.Select(p => (p - center).LengthSquared).Max());

            CameraOrientation guess = CameraOrientation.Guess(intrinsics, objPts, imgPts);

            // Place the camera 2x the diameter of the sphere enclosing all 3D calibration points away from the
            // center of those calibration points along the optic axis
            Vector3d p0 = center - 4 * radius * guess.opticAxis;
            double[] tvec = CameraTools.tvecFromCameraPosition(guess.m, p0);

            // Actually solve for the camera pose given our good initial guess
            double[] rvec = guess.rvec;
            CameraTools.SolvePnP(objPts, imgPts, intrinsics.Mat, intrinsics.Dist, ref rvec, ref tvec);

            // Wrap the result in a LocatedCamera
            var lcam = new LocatedCamera(intrinsics, rvec, tvec);
            var imgPts2 = lcam.Project(objPts);
            var err = Math.Sqrt(imgPts.Zip(imgPts2, (p1, p2) => (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)).Average());

            var unp = lcam.Unproject(imgPts).Select(v => lcam.Location + v).ToArray();
            var imgPtsUP = lcam.Project(unp);

            return lcam;
        }

        public static LocatedCamera Create(LocatedCamera guess, IEnumerable<Vector3d> objPoints, IEnumerable<Point2f> imgPoints)
        {
            double[] rvec = new double[3];
            Array.Copy(guess.rvec, rvec, 3);
            double[] tvec = new double[3];
            Array.Copy(guess.tvec, tvec, 3);

            CameraTools.SolvePnP(objPoints.ToArray(), imgPoints.ToArray(), guess.Intrinsics.Mat, guess.Intrinsics.Dist, ref rvec, ref tvec);

            // Wrap the result in a new LocatedCamera
            var lcam = new LocatedCamera(guess.Intrinsics, rvec, tvec);
            //var imgPts2 = lcam.Project(objPts);
            //var err = Math.Sqrt(imgPts.Zip(imgPts2, (p1, p2) => (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)).Average());

            return lcam;
        }
    }
}

using OpenCvSharp;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Projection
{
    public static class CameraTools
    {
        /// <summary>
        /// Convert a Rodrigues rotation to rotation axis and angle about that axis
        /// </summary>
        /// <param name="rvec">Rodrigues rotation</param>
        /// <returns>Unitized rotation axis, and angle about that axis</returns>
        /// <remarks>
        /// A Rodrigues rotation is simply r_hat * tan(alpha/2)
        /// http://pajarito.materials.cmu.edu/rollett/27750/Rodrigues-Vectors-Seth-2Jan15.pdf
        /// </remarks>
        /// <seealso cref="AxisAngleToRodrigues(Vector3d, double)"/>
        public static Tuple<Vector3d, double> RodriguesToAxisAngle(double[] rvec)
        {
            double r = Math.Sqrt(rvec[0] * rvec[0] + rvec[1] * rvec[1] + rvec[2] * rvec[2]);
            double theta = Math.Atan(r) * 2;
            return new Tuple<Vector3d, double>(new Vector3d((float)(rvec[0] / r), (float)(rvec[1] / r), (float)(rvec[2] / r)), theta);
        }

        /// <summary>
        /// Create a Rodrigues rotation from a rotational axis and an angle about that axis
        /// </summary>
        /// <param name="axis">Rotation axis (need not be unitized)</param>
        /// <param name="angle">Angle about the axis</param>
        /// <returns>Rodrigues rotation</returns>
        /// <seealso cref="RodriguesToAxisAngle(double[])"/>
        public static double[] AxisAngleToRodrigues(Vector3d axis, double angle)
        {
            double r = Math.Sqrt(axis.X * axis.X + axis.Y * axis.Y + axis.Z * axis.Z);
            double tanalpha2 = Math.Tan(angle / 2);
            return new double[] { axis.X / r * tanalpha2, axis.Y / r * tanalpha2, axis.Z / r * tanalpha2 };
        }

        /// <summary>
        /// Finds a camera pose from 3D-2D point correspondences.
        /// </summary>
        /// <param name="objectPoints">A set of points with known 3D locations.</param>
        /// <param name="imagePoints">The corresponding projected locations of the known 3D points.</param>
        /// <param name="cameraMatrix">Pinhole projection matrix for the camera.</param>
        /// <param name="distCoeffs">Distortion coefficients; see OpenCV documentation.</param>
        /// <param name="rvec">An output: the orientation of the camera pose.  Also, if rvec and tvec are both non-null, an input: the initial guess for the orientation of the camera pose.</param>
        /// <param name="tvec">An output: the position of the camera pose.  Also, if rvec and tvec are both non-null, an input: the initial guess for the position of the camera pose.</param>
        /// <param name="flags">Method for solving a PnP problme; see OpenCV documentation.</param>
        /// <remarks>OpenCV documentation: http://docs.opencv.org/2.4/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html</remarks>
        public static void SolvePnP(Vector3d[] objectPoints, Point2f[] imagePoints, double[,] cameraMatrix, double[] distCoeffs, ref double[] rvec, ref double[] tvec, SolvePnPFlags flags = SolvePnPFlags.Iterative)
        {
            if (objectPoints.Length != imagePoints.Length)
                throw new ArgumentException("Number of object points (" + objectPoints.Length + ") must match the number of image points (" + imagePoints.Length + ")");
            if (cameraMatrix.GetLength(0) != 3 || cameraMatrix.GetLength(1) != 3)
                throw new ArgumentException("Camera matrix must be 3x3");
            if (distCoeffs.Length != 4 && distCoeffs.Length != 5 && distCoeffs.Length != 8)
                throw new ArgumentException("Distortion coefficients must contain either 4, 5, or 8 elements");

            using (Mat objPtsM = new Mat(objectPoints.Length, 1, MatType.CV_64FC3, objectPoints))
            using (Mat imgPtsM = new Mat(imagePoints.Length, 1, MatType.CV_32FC2, imagePoints))
            using (Mat mM = new Mat(3, 3, MatType.CV_64F, cameraMatrix))
            using (Mat distM = new Mat(1, distCoeffs.Length, MatType.CV_64F, distCoeffs))
            using (Mat rvecM = rvec != null ? new Mat(1, 3, MatType.CV_64F, rvec) : new Mat(1, 3, MatType.CV_64F))
            using (Mat tvecM = tvec != null ? new Mat(1, 3, MatType.CV_64F, tvec) : new Mat(1, 3, MatType.CV_64F))
            {
                Cv2.SolvePnP(objPtsM, imgPtsM, mM, distM, rvecM, tvecM, rvec != null && tvec != null, flags);

                if (rvec == null)
                    rvec = new double[3];
                if (tvec == null)
                    tvec = new double[3];

                rvecM.GetArray(0, 0, rvec); //TODO: when is this necessary?
                tvecM.GetArray(0, 0, tvec); //TODO: when is this necessary?
            }
        }

        public static Vector3d CameraPosition(double[] rvec, double[] tvec)
        {
            double[,] r;
            Cv2.Rodrigues(rvec, out r);
            return CameraPosition(r, tvec);
        }

        public static Vector3d CameraPosition(double[,] r, double[] tvec)
        {
            return new Vector3d(
                (float)(-r[0, 0] * tvec[0] - r[1, 0] * tvec[1] - r[2, 0] * tvec[2]),
                (float)(-r[0, 1] * tvec[0] - r[1, 1] * tvec[1] - r[2, 1] * tvec[2]),
                (float)(-r[0, 2] * tvec[0] - r[1, 2] * tvec[1] - r[2, 2] * tvec[2])
            );
        }

        public static double[] tvecFromCameraPosition(double[] rvec, Vector3d p)
        {
            double[,] r;
            Cv2.Rodrigues(rvec, out r);
            return tvecFromCameraPosition(r, p);
        }

        public static double[] tvecFromCameraPosition(double[,] r, Vector3d p)
        {
            double r00 = r[0, 0], r01 = r[0, 1], r02 = r[0, 2];
            double r10 = r[1, 0], r11 = r[1, 1], r12 = r[1, 2];
            double r20 = r[2, 0], r21 = r[2, 1], r22 = r[2, 2];
            double px = p.X;
            double py = p.Y;
            double pz = p.Z;

            double denom = -r02 * r11 * r20 + r01 * r12 * r20 + r02 * r10 * r21 - r00 * r12 * r21 - r01 * r10 * r22 + r00 * r11 * r22;

            double[] tvec = new double[3];
            tvec[0] = (pz * r11 * r20 - py * r12 * r20 - pz * r10 * r21 + px * r12 * r21 + py * r10 * r22 - px * r11 * r22) / denom;
            tvec[1] = -(pz * r01 * r20 - py * r02 * r20 - pz * r00 * r21 + px * r02 * r21 + py * r00 * r22 - px * r01 * r22) / denom;
            tvec[2] = (pz * r01 * r10 - py * r02 * r10 - pz * r00 * r11 + px * r02 * r11 + py * r00 * r12 - px * r01 * r12) / denom;

            return tvec;
        }
    }
}

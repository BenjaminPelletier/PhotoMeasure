using OpenCvSharp;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Projection
{
    public static class ConversionExtensions
    {
        public static IEnumerable<Point3f> ToPoint3f(this IEnumerable<Vector3d> vectors)
        {
            return vectors.Select(v => new Point3f((float)v.X, (float)v.Y, (float)v.Z));
        }

        public static IEnumerable<IEnumerable<Point3f>> ToPoint3f(this IEnumerable<IEnumerable<Vector3d>> vectorLists)
        {
            return vectorLists.Select(vl => vl.ToPoint3f());
        }

        public static Point2f ToPoint2f(this PointF pixel)
        {
            return new Point2f(pixel.X, pixel.Y);
        }

        public static T[] Take<T>(this IEnumerator<T> enumerator, int n)
        {
            var result = new T[n];
            for (int i = 0; i < n; i++)
            {
                enumerator.MoveNext();
                result[i] = enumerator.Current;
            }
            return result;
        }

        public static double[] ToArray(this Vec3d v)
        {
            return new double[] { v.Item0, v.Item1, v.Item2 };
        }
    }
}

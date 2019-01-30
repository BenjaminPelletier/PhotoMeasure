using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Geometry
{
    public static class GeometryExtensions
    {
        public static Quaterniond GetQuaternionTo(this Vector3d v0, Vector3d v1)
        {
            Vector3d axis = Vector3d.Cross(v0, v1);
            double angle = Vector3d.CalculateAngle(v0, v1);
            return Quaterniond.FromAxisAngle(axis, angle);
        }

        public static Vector3d Toward(this Vector3d v0, Vector3d v1, double f)
        {
            return (1 - f) * v0 + f * v1;
        }

        public static Vector3 Toward(this Vector3 v0, Vector3 v1, float f)
        {
            return (1 - f) * v0 + f * v1;
        }

        public static Vector3 AsFloat(this Vector3d v)
        {
            return (Vector3)v;
        }

        public static Vector3d Center(this IEnumerable<Vector3d> points)
        {
            int n = 0;
            Vector3d result = new Vector3d(0, 0, 0);
            foreach (Vector3d p in points)
            {
                result += p;
                n++;
            }
            return result / n;
        }
    }
}

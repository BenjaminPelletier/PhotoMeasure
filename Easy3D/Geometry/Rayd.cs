using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Geometry
{
    public struct Rayd
    {
        public Vector3d Origin;
        public Vector3d Direction;

        public Rayd(Vector3d origin, Vector3d direction)
        {
            this.Origin = origin;
            this.Direction = direction;
        }

        /// <summary>
        /// Find the points on each ray nearest the other ray
        /// </summary>
        /// <returns>Ray where origin is the point on r1 closest to r2, and Origin + Direction is the point on r2 closest to r1</returns>
        public static Rayd NearestPoints(Rayd r1, Rayd r2)
        {
            Vector3d p0 = r1.Origin;
            Vector3d p1 = r2.Origin;
            Vector3d v0 = r1.Direction;
            Vector3d v1 = r2.Direction;

            double p0v0 = Vector3d.Dot(p0, v0);
            double p0v1 = Vector3d.Dot(p0, v1);
            double p1v0 = Vector3d.Dot(p1, v0);
            double p1v1 = Vector3d.Dot(p1, v1);
            double v0v0 = Vector3d.Dot(v0, v0);
            double v1v1 = Vector3d.Dot(v1, v1);
            double v0v1 = Vector3d.Dot(v0, v1);

            double denom = v0v1 * v0v1 - v0v0 * v1v1;

            double t0 = (v0v1 * (p1v1 - p0v1) + v1v1 * (p0v0 - p1v0)) / denom;
            double t1 = (v0v1 * (p0v0 - p1v0) + v0v0 * (p1v1 - p0v1)) / denom;

            Vector3d i0 = p0 + t0 * v0;
            Vector3d i1 = p1 + t1 * v1;

            return new Rayd(i0, i1 - i0);
        }

        public Vector3d DistanceAlong(double d)
        {
            return this.Origin + d * this.Direction;
        }

        public double DistanceTo(Planed plane)
        {
            return (plane.Normal.X * plane.Origin.X + plane.Normal.Y * plane.Origin.Y + plane.Normal.Z * plane.Origin.Z - plane.Normal.X * Origin.X - plane.Normal.Y * Origin.Y - plane.Normal.Z * Origin.Z) / (plane.Normal.X * Direction.X + plane.Normal.Y * Direction.Y + plane.Normal.Z * Direction.Z);
        }

        public Vector3d IntersectionWith(Planed plane)
        {
            return this.DistanceAlong(DistanceTo(plane));
        }

        public static Vector3d NearestPoint(IEnumerable<Rayd> rays)
        {
            double xx = 0, xy = 0, xz = 0, xc = 0, yy = 0, yz = 0, yc = 0, zz = 0, zc = 0;
            foreach (Rayd r in rays)
            {
                double x0 = r.Origin.X, y0 = r.Origin.Y, z0 = r.Origin.Z;
                double vx = r.Direction.X, vy = r.Direction.Y, vz = r.Direction.Z;
                double vx2 = vx * vx, vy2 = vy * vy, vz2 = vz * vz;
                double vmag = vx2 + vy2 + vz2;
                xx += (vy2 + vz2) / vmag;
                xy -= (vx * vy) / vmag;
                xz -= (vx * vz) / vmag;
                xc += ((vy2 + vz2) * x0 - vx * vy * y0 - vx * vz * z0) / vmag;
                yy += (vx2 + vz2) / vmag;
                yz -= (vy * vz) / vmag;
                yc += (-vx * vy * x0 + (vx2 + vz2) * y0 - vy * vz * z0) / vmag;
                zz += (vx2 + vy2) / vmag;
                zc += (-vx * vz * x0 - vy * vz * y0 + (vx2 + vy2) * z0) / vmag;
            }

            double denom = xz * (yy * xz - 2 * xy * yz) + xx * yz * yz + xy * xy * zz - xx * yy * zz;
            double px = ((xz * yy - xy * yz) * zc + (xy * zz - xz * yz) * yc + (yz * yz - yy * zz) * xc) / denom;
            double py = ((xx * yz - xy * xz) * zc + (xz * xz - xx * zz) * yc + (xy * zz - xz * yz) * xc) / denom;
            double pz = ((xy * xy - xx * yy) * zc + (xx * yz - xy * xz) * yc + (xz * yy - xy * yz) * xc) / denom;

            return new Vector3d(px, py, pz);
        }
    }
}

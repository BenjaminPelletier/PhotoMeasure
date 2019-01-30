using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Geometry
{
    public struct Planed
    {
        public Vector3d Origin;
        public Vector3d Normal;

        public Planed(Vector3d origin, Vector3d normal)
        {
            this.Origin = origin;
            this.Normal = normal;
            this.Normal.Normalize();
        }

        public Planed(Vector3 origin, Vector3 normal)
        {
            this.Origin = (Vector3d)origin;
            this.Normal = (Vector3d)normal;
            this.Normal.Normalize();
        }
    }
}

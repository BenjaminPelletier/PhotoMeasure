using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy3D.Projection;
using Easy3D.Scenes.Observations;

namespace Easy3D.Scenes.Features
{
    public class LocatedCircle : IObservableFeature
    {
        public readonly Vector3d Center;
        public readonly Vector3d Normal;

        public LocatedCircle(Vector3d center, Vector3d normal)
        {
            this.Center = center;
            this.Normal = normal;
        }

        public double ObservationError(LocatedCamera camera, Observation observation)
        {
            throw new NotImplementedException();
        }
    }
}

using Easy3D.Projection;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy3D.Scenes.Observations;
using OpenCvSharp;

namespace Easy3D.Scenes.Features
{
    public class LocatedPoint : IObservableFeature
    {
        public readonly Vector3d Location;

        public LocatedPoint(Vector3d location)
        {
            this.Location = location;
        }

        public double ObservationError(LocatedCamera camera, Observation observation)
        {
            Point2f expected = camera.Project(this.Location);
            PointF actual = observation.Point.Location;
            float dx = expected.X - actual.X;
            float dy = expected.Y - actual.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes.Observations
{
    public class CircleObservation
    {
        public readonly List<PointF> Points;

        public CircleObservation(IEnumerable<PointF> points)
        {
            this.Points = points.ToList();
        }
    }
}

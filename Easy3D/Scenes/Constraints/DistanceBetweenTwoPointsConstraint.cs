using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy3D.Scenes.Features;

namespace Easy3D.Scenes.Constraints
{
    public class DistanceBetweenTwoPointsConstraint : ISpatialRelationship
    {
        public string Point1;
        public string Point2;
        public double Distance;

        public DistanceBetweenTwoPointsConstraint(string point1, string point2, double distance)
        {
            this.Point1 = point1;
            this.Point2 = point2;
            this.Distance = distance;
        }

        public double LayoutError(Dictionary<string, LocatedFeature> features)
        {
            return (features[Point1].Point.Location - features[Point2].Point.Location).Length;
        }
    }
}

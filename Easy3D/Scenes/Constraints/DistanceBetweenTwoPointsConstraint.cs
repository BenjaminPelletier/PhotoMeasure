using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes.Constraints
{
    public class DistanceBetweenTwoPointsConstraint
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
    }
}

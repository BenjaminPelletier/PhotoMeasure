using Easy3D.Scenes.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes.Observations
{
    /// <summary>
    /// The two-dimensional observed location of a three-dimensional feature in a particular camera.
    /// </summary>
    public class Observation
    {
        public string FeatureName;
        public FeatureType FeatureType;
        public PointObservation Point;
        public CircleObservation Circle;

        public Observation(string name, PointObservation observation)
        {
            this.FeatureName = name;
            this.FeatureType = FeatureType.Point;
            this.Point = observation;
        }

        public Observation(string name, CircleObservation observation)
        {
            this.FeatureName = name;
            this.FeatureType = FeatureType.Circle;
            this.Circle = observation;
        }
    }
}

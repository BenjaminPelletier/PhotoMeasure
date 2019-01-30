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
    public class LocatedFeature : IObservableFeature
    {
        public readonly string Name;
        public readonly FeatureType Type;
        public readonly LocatedPoint Point;
        public readonly LocatedCircle Circle;

        public LocatedFeature(string name, LocatedPoint point)
        {
            this.Name = name;
            this.Type = FeatureType.Point;
            this.Point = point;
        }

        public LocatedFeature(string name, LocatedCircle circle)
        {
            this.Name = name;
            this.Type = FeatureType.Circle;
            this.Circle = circle;
        }

        public IObservableFeature ObservableFeature
        {
            get
            {
                switch (this.Type)
                {
                    case FeatureType.Point:
                        return this.Point;
                    case FeatureType.Circle:
                        return this.Circle;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public double ObservationError(LocatedCamera camera, Observation observation)
        {
            return this.ObservableFeature.ObservationError(camera, observation);
        }
    }
}

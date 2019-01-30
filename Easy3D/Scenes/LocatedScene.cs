using Easy3D.Geometry;
using Easy3D.Projection;
using Easy3D.Scenes.Features;
using Easy3D.Scenes.Observations;
using OpenCvSharp;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes
{
    /// <summary>
    /// Completely defined scene with known camera positions and orientations as well as positions for all Features.
    /// </summary>
    public class LocatedScene
    {
        public readonly List<LocatedView> Views;
        public readonly List<LocatedFeature> Features;

        public LocatedScene(IEnumerable<LocatedView> locatedViews, IEnumerable<LocatedFeature> locatedFeatures)
        {
            this.Views = locatedViews.ToList();
            this.Features = locatedFeatures.ToList();
        }

        public IEnumerable<double> ErrorVector
        {
            get
            {
                //TODO: ensure proper (repeatable) ordering of observation errors
                Dictionary<string, LocatedFeature> features = this.Features.ToDictionary(f => f.Name, f => f);
                foreach (LocatedView locatedView in Views)
                {
                    foreach (Observation observation in locatedView.Observations)
                    {
                        if (observation.FeatureType == FeatureType.Point)
                        {
                            yield return features[observation.FeatureName].ObservationError(locatedView.Camera, observation);
                        }
                        else
                        {
                            throw new NotImplementedException("Feature type " + observation.FeatureType + " is not yet supported in LocatedView.ErrorVector");
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Easy3D.Projection;
using Easy3D.Scenes.Observations;
using Easy3D.Scenes.Features;

namespace Easy3D.Scenes
{
    /// <summary>
    /// Observations made by a particular camera with known position and orientation.
    /// </summary>
    public class LocatedView
    {
        public string ImagePath;
        public LocatedCamera Camera;
        public List<Observation> Observations;

        public LocatedView(View view, double[] rvec, double[] tvec)
        {
            this.ImagePath = view.ImagePath;
            this.Camera = new LocatedCamera(view.Intrinsics, rvec, tvec);
            this.Observations = view.Observations;
        }

        /// <summary>
        /// The error between observations of features and where they should be in a LocatedView.
        /// </summary>
        public class Error
        {
            /// <summary>
            /// Key: Feature name.
            /// Value: Pixel error between projected feature location and observed feature location.
            /// </summary>
            public Dictionary<string, double> FeatureErrors = new Dictionary<string, double>();
        }

        public Error GetError(Dictionary<string, LocatedFeature> features)
        {
            var result = new Error();

            foreach (Observation observation in this.Observations)
            {
                if (observation.FeatureType == FeatureType.Point)
                {
                    result.FeatureErrors[observation.FeatureName] = features[observation.FeatureName].ObservationError(this.Camera, observation);
                }
                else
                {
                    throw new NotImplementedException("Feature type " + observation.FeatureType + " is not yet supported in LocatedView.GetError");
                }
            }

            return result;
        }
    }
}

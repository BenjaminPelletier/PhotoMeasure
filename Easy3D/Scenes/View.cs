using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Easy3D.Projection;
using Easy3D.Scenes.Observations;

namespace Easy3D.Scenes
{
    /// <summary>
    /// Observations made by a particular camera of unknown position and orientation.
    /// </summary>
    public class View
    {
        public string ImagePath;
        public CameraIntrinsics Intrinsics;
        public readonly List<Observation> Observations;

        public View(CameraIntrinsics intrinsics, IEnumerable<Observation> observations, string imagePath = null)
        {
            this.Intrinsics = intrinsics;
            this.Observations = observations.ToList();
            this.ImagePath = imagePath;
        }

        public Observation GetObservation(string name)
        {
            foreach (Observation observation in this.Observations)
            {
                if (observation.FeatureName == name)
                {
                    return observation;
                }
            }
            return null;
        }

        /// <summary>
        /// Indicate whether there are enough features observed to make it possible for the camera which observed this view to be located.
        /// </summary>
        public bool IsLocatable()
        {
            return Observations.Where(observation => observation.FeatureType == Features.FeatureType.Point).Count() >= 3;
        }
    }
}

using Easy3D.Geometry;
using Easy3D.Numerics;
using Easy3D.Projection;
using Easy3D.Scenes.Constraints;
using Easy3D.Scenes.Features;
using Easy3D.Scenes.Observations;
using OpenCvSharp;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes
{
    public static class Solver
    {
        /// <summary>
        /// Determine the best locations for features that that can be esimated by observations from known-6DOF cameras.
        /// </summary>
        static IEnumerable<LocatedFeature> EstimateFeatureLocations(IEnumerable<LocatedView> locatedViews, IEnumerable<Feature> features)
        {
            List<LocatedView> views = locatedViews.ToList();
            Dictionary<LocatedView, Dictionary<string, Observation>> observationByName = views.ToDictionary(view => view, view => view.Observations.ToDictionary(obs => obs.FeatureName, obs => obs));
            foreach (Feature feature in features)
            {
                if (feature.Type == FeatureType.Point)
                {
                    var rays = new List<Rayd>();
                    foreach (LocatedView locatedView in locatedViews)
                    {
                        var observations = observationByName[locatedView];
                        if (observations.ContainsKey(feature.Name))
                        {
                            Point2f pixel = observations[feature.Name].Point.Location.ToPoint2f();
                            rays.Add(new Rayd(locatedView.Camera.Location, locatedView.Camera.Unproject(pixel)));
                        }
                    }
                    if (rays.Count >= 2)
                    {
                        yield return new LocatedFeature(feature.Name, new LocatedPoint(Rayd.NearestPoint(rays)));
                    }
                }
            }
        }
    }
}

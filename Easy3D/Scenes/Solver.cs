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

        /// <summary>
        /// Given a nominal set of feature locations, computed an adjusted set of locations that conform to the specified constraints.
        /// </summary>
        static List<LocatedFeature> ConstrainFeatureLocations(IEnumerable<LocatedFeature> locatedFeatures, IEnumerable<Constraint> constraints)
        {
            return locatedFeatures.ToList();
        }

        public static LocatedScene Solve(Scene scene, NelderMead solver = null)
        {
            LocatedView[] knownViews = new LocatedView[] { new LocatedView(scene.Views.First(), new double[] { 0, 0, 0 }, new double[] { 0, 0, 0 }) };
            View[] unknownViews = scene.Views.Skip(1).Where(view => view.IsLocatable()).ToArray();

            Func<IEnumerable<double>, LocatedScene> makeScene = p =>
            {
                var paramSet = p.GetEnumerator();
                var views = new List<LocatedView>();
                views.AddRange(knownViews);
                foreach (View view in unknownViews)
                {
                    views.Add(new LocatedView(view, paramSet.Take(3), paramSet.Take(3)));
                }
                LocatedFeature[] locatedFeatures = EstimateFeatureLocations(views, scene.Features).ToArray();
                locatedFeatures = ConstrainFeatureLocations(locatedFeatures, scene.Constraints).ToArray();
                return new LocatedScene(views, locatedFeatures);
            };

            Func<double[], double> errorFunction = p =>
            {
                double errSum = 0;
                LocatedScene locatedScene = makeScene(p);
                double[] errorVector = locatedScene.ErrorVector.ToArray();
                foreach (double err in errorVector)
                {
                    errSum += err * err;
                }
                return Math.Sqrt(errSum / errorVector.Length);
            };

            if (solver == null)
            {
                solver = new NelderMead();
            }

            // Make initial guesses for camera positions and orientations
            Vector3d center = knownViews[0].Camera.Location + knownViews[0].Camera.Unproject(new Point2f(knownViews[0].Camera.Intrinsics.Width / 2, knownViews[0].Camera.Intrinsics.Height / 2));
            //TODO: Estimate feature locations directly from known views if there is more than one known view
            Dictionary<string, Vector3d> objPts = knownViews[0].Observations
                .Where(obs => obs.FeatureType == FeatureType.Point)
                .ToDictionary(
                    obs => obs.FeatureName,
                    obs => knownViews[0].Camera.Location + knownViews[0].Camera.Unproject(obs.Point.Location.ToPoint2f()));
            var initialGuess = new List<double>();
            foreach (View view in unknownViews)
            {
                Dictionary<string, Point2f> imgPts = view.Observations
                    .Where(obs => objPts.ContainsKey(obs.FeatureName))
                    .ToDictionary(obs => obs.FeatureName, obs => obs.Point.Location.ToPoint2f());
                if (imgPts.Count >= 3)
                {
                    CameraOrientation orientation = CameraOrientation.Guess(view.Intrinsics, imgPts.Keys.Select(k => objPts[k]).ToArray(), imgPts.Values.ToArray());
                    initialGuess.AddRange(orientation.rvec);
                    Vector3d position = center - 2 * orientation.opticAxis;
                    initialGuess.AddRange(CameraTools.tvecFromCameraPosition(orientation.m, position));
                }
                else
                {
                    throw new NotImplementedException("Can't create initial guess for camera that doesn't share at least 3 points with known camera");
                }
            }

            // Optimize camera positions and orientations
            double[] bestParameters = solver.FindMinimum(errorFunction, initialGuess.ToArray(), 0.1);

            return makeScene(bestParameters);
        }
    }
}

using Easy3D.Projection;
using Easy3D.Scenes.Features;
using Easy3D.Scenes.Observations;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes
{
    /// <summary>
    /// Creates a LocatedScene from an sequence of double values.
    /// </summary>
    class LocatedSceneMaker
    {
        private readonly Scene _Scene;
        private readonly View[] _Views;
        private readonly HashSet<string> _ViewedFeatures = new HashSet<string>();

        private static readonly double[] _DefaultCameraVector;
        private static readonly Vector3d _DefaultPointLocation = Vector3d.Zero;
        private static readonly Vector3d _DefaultCircleOrigin = Vector3d.Zero;
        private static readonly Vector3d _DefaultCircleNormal = Vector3d.UnitY;

        static LocatedSceneMaker()
        {
            _DefaultCameraVector = new double[6];
            var defaultCameraOrientation = new CameraOrientation(Vector3d.UnitX, -Vector3d.UnitZ);
            Array.Copy(defaultCameraOrientation.rvec, _DefaultCameraVector, 3);
            double[] defaulttvec = CameraTools.tvecFromCameraPosition(defaultCameraOrientation.m, new Vector3d(0, -1, 0));
            Array.Copy(defaulttvec, 0, _DefaultCameraVector, 3, 3);
        }

        public LocatedSceneMaker(Scene scene)
        {
            _Scene = scene;
            _Views = scene.Views.Where(view => view.IsLocatable()).ToArray();

            foreach (View view in _Views)
            {
                foreach (Observation observation in view.Observations)
                {
                    _ViewedFeatures.Add(observation.FeatureName);
                }
            }
        }

        public LocatedScene MakeScene(IEnumerable<double> parameters)
        {
            var paramSet = parameters.GetEnumerator();
            var views = new List<LocatedView>();
            foreach (View view in _Views)
            {
                double[] rvec = paramSet.Take(3);
                double[] tvec = paramSet.Take(3);
                views.Add(new LocatedView(view, rvec, tvec));
            }
            var features = new List<LocatedFeature>();
            foreach (Feature feature in _Scene.Features)
            {
                if (_ViewedFeatures.Contains(feature.Name))
                {
                    switch (feature.Type)
                    {
                        case FeatureType.Point:
                            Vector3d pt = paramSet.TakeVector3d();
                            features.Add(new LocatedFeature(feature.Name, new LocatedPoint(pt)));
                            break;
                        case FeatureType.Circle:
                            Vector3d origin = paramSet.TakeVector3d();
                            Vector3d normal = paramSet.TakeVector3d();
                            features.Add(new LocatedFeature(feature.Name, new LocatedCircle(origin, normal)));
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            return new LocatedScene(views, features);
        }

        public IEnumerable<double> GetInitialGuess()
        {
            foreach (View view in _Views)
            {
                foreach (double v in _DefaultCameraVector)
                {
                    yield return v;
                }
            }
            var features = new List<LocatedFeature>();
            foreach (Feature feature in _Scene.Features)
            {
                if (_ViewedFeatures.Contains(feature.Name))
                {
                    switch (feature.Type)
                    {
                        case FeatureType.Point:
                            yield return _DefaultPointLocation.X; yield return _DefaultPointLocation.Y; yield return _DefaultPointLocation.Z;
                            break;
                        case FeatureType.Circle:
                            yield return _DefaultCircleOrigin.X; yield return _DefaultCircleOrigin.Y; yield return _DefaultCircleOrigin.Z;
                            yield return _DefaultCircleNormal.X; yield return _DefaultCircleNormal.Y; yield return _DefaultCircleNormal.Z;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }
    }
}

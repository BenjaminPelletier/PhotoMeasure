using Easy3D.Geometry;
using Easy3D.Projection;
using Easy3D.Scenes.Constraints;
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

        /// <summary>
        /// The error between a LocatedScene and a set of actual observations and known constraints.
        /// </summary>
        /// <remarks>
        /// The TotalError will become invalid if any of the *Errors are modified after the TotalError is first retrieved.
        /// </remarks>
        public class Error : IComparable<Error>
        {
            private double _TotalError = double.NaN;
            public Dictionary<string, LocatedView.Error> ProjectionErrors = new Dictionary<string, LocatedView.Error>();
            public Dictionary<string, double> ConstraintErrors = new Dictionary<string, double>();

            public double TotalError
            {
                get
                {
                    if (double.IsNaN(_TotalError))
                    {
                        _TotalError = ComputeTotalError();
                    }
                    return _TotalError;
                }
            }

            private double ComputeTotalError()
            {
                double projectionError = 0;
                int nProjectionErrors = 0;
                foreach (LocatedView.Error viewError in ProjectionErrors.Values)
                {
                    foreach (double featureError in viewError.FeatureErrors.Values)
                    {
                        projectionError += featureError * featureError;
                        nProjectionErrors++;
                    }
                }
                projectionError = Math.Sqrt(projectionError / nProjectionErrors);

                double constraintError = ConstraintErrors.Count > 0 ? Math.Sqrt(ConstraintErrors.Values.Select(e => e * e).Average()) : 0;

                //TODO: Computer smarter tradeoff between constraint and projection errors
                return projectionError + constraintError * 100;
            }

            public int CompareTo(Error other)
            {
                return this.TotalError.CompareTo(other.TotalError);
            }

            public override string ToString()
            {
                return $"LocatedScene.Error {_TotalError:f2}";
            }
        }

        public Error GetError(IEnumerable<Constraint> constraints)
        {
            var result = new Error();

            Dictionary<string, LocatedFeature> features = this.Features.ToDictionary(f => f.Name, f => f);
            foreach (LocatedView locatedView in Views)
            {
                result.ProjectionErrors[locatedView.ImagePath] = locatedView.GetError(features);
            }

            foreach (Constraint constraint in constraints)
            {
                result.ConstraintErrors[constraint.Name] = constraint.LayoutError(features);
            }

            return result;
        }
    }
}

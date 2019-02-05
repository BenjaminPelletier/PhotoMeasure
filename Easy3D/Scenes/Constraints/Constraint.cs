using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy3D.Scenes.Features;

namespace Easy3D.Scenes.Constraints
{
    /// <summary>
    /// A relationship between Features that is known to be true, regardless of view of those Features.
    /// </summary>
    public class Constraint : ISpatialRelationship
    {
        public string Name;
        public readonly DistanceBetweenTwoPointsConstraint DistanceBetweenTwoPoints;
        public readonly CoplanarFeaturesConstraint CoplanarFeatures;

        public Constraint(string name, DistanceBetweenTwoPointsConstraint constraint)
        {
            this.Name = name;
            this.DistanceBetweenTwoPoints = constraint;
        }

        public Constraint(string name, CoplanarFeaturesConstraint constraint)
        {
            this.Name = name;
            this.CoplanarFeatures = constraint;
        }

        public ConstraintType Type
        {
            get
            {
                if (DistanceBetweenTwoPoints != null)
                {
                    return ConstraintType.DistanceBetweenTwoPoints;
                }
                else if (CoplanarFeatures != null)
                {
                    return ConstraintType.CoplanarFeatures;
                }
                else
                {
                    throw new InvalidOperationException("Unknown constraint type");
                }
            }
        }

        private ISpatialRelationship SpatialRelationship
        {
            get
            {
                switch (this.Type)
                {
                    case ConstraintType.CoplanarFeatures:
                        return this.CoplanarFeatures;
                    case ConstraintType.DistanceBetweenTwoPoints:
                        return this.DistanceBetweenTwoPoints;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public double LayoutError(Dictionary<string, LocatedFeature> features)
        {
            return this.SpatialRelationship.LayoutError(features);
        }
    }

    public enum ConstraintType
    {
        DistanceBetweenTwoPoints,
        CoplanarFeatures,
    }
}

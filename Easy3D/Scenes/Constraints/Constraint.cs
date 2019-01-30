using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes.Constraints
{
    public class Constraint
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
    }

    public enum ConstraintType
    {
        DistanceBetweenTwoPoints,
        CoplanarFeatures,
    }
}

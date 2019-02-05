using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy3D.Scenes.Features;

namespace Easy3D.Scenes.Constraints
{
    public class CoplanarFeaturesConstraint : ISpatialRelationship
    {
        public List<string> Features = new List<string>();

        public CoplanarFeaturesConstraint(IEnumerable<string> features = null)
        {
            if (features != null)
            {
                Features.AddRange(features);
            }
        }

        public double LayoutError(Dictionary<string, LocatedFeature> features)
        {
            throw new NotImplementedException();
        }
    }
}

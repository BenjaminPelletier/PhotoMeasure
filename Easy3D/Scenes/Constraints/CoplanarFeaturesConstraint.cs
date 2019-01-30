using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes.Constraints
{
    public class CoplanarFeaturesConstraint
    {
        public List<string> Features = new List<string>();

        public CoplanarFeaturesConstraint(IEnumerable<string> features = null)
        {
            if (features != null)
            {
                Features.AddRange(features);
            }
        }
    }
}

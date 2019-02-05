using Easy3D.Scenes.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes.Constraints
{
    interface ISpatialRelationship
    {
        /// <summary>
        /// Compute the error in the spatial relationship with the provided Feature locations/layout.
        /// </summary>
        /// <param name="features">Locations of Features in the scene.</param>
        /// <returns>Spatial distance between expected and actual Features based on the relationship.</returns>
        double LayoutError(Dictionary<string, LocatedFeature> features);
    }
}

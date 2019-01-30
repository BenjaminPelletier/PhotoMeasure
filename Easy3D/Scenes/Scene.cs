using Easy3D.Projection;
using Easy3D.Scenes.Constraints;
using Easy3D.Scenes.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes
{
    public class Scene
    {
        public readonly List<View> Views;
        public readonly List<Feature> Features;
        public readonly List<Constraint> Constraints;

        public Scene(IEnumerable<View> views, IEnumerable<Feature> features, IEnumerable<Constraint> constraints)
        {
            this.Views = views.ToList();
            this.Features = features.ToList();
            this.Constraints = constraints.ToList();
        }
    }
}

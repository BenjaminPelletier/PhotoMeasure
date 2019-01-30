using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes.Features
{
    public class Feature
    {
        public readonly FeatureType Type;
        public string Name;

        public Feature(FeatureType type, string name)
        {
            this.Type = type;
            this.Name = name;
        }
    }

    public enum FeatureType
    {
        Point,
        Circle,
        PlanarCurve,
    }
}

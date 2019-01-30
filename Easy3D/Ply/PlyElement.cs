using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    class PlyElement
    {
        public string Name;
        public List<PlyProperty> Properties;

        public PlyElement(string name)
        {
            this.Name = name;
            Properties = new List<PlyProperty>();
        }

        public void AddProperty(PlyProperty property)
        {
            this.Properties.Add(property);
        }
    }
}

using Easy3D.Ply;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ui.GLUtils
{
    static class ObjectLibrary
    {
        public static Mesh GetSegmentedCube()
        {
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Easy3D.Ui.Resources.SegmentedCube.ply"))
            {
                if (s == null)
                {
                    //TODO: Something dramatic should go here since this may happen silently otherwise (in UserControl.OnLoad)
                }
                var p = new PlyFile(s);
                return p.ToMesh();
            }
        }
    }
}

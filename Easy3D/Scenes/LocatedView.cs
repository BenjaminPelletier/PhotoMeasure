using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Easy3D.Projection;
using Easy3D.Scenes.Observations;
using Easy3D.Scenes.Features;

namespace Easy3D.Scenes
{
    /// <summary>
    /// Observations made by a particular camera with known position and orientation.
    /// </summary>
    public class LocatedView
    {
        public LocatedCamera Camera;
        public List<Observation> Observations;

        public LocatedView(View view, double[] rvec, double[] tvec)
        {
            this.Camera = new LocatedCamera(view.Intrinsics, rvec, tvec);
            this.Observations = view.Observations;
        }
    }
}

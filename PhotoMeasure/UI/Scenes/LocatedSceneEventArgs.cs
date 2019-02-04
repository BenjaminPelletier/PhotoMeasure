using Easy3D.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMeasure.UI.Scenes
{
    public class LocatedSceneEventArgs
    {
        public LocatedScene LocatedScene;

        public LocatedSceneEventArgs(LocatedScene scene)
        {
            this.LocatedScene = scene;
        }
    }
}

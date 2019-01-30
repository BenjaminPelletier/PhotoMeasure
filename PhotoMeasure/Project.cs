using Easy3D.Projection;
using Easy3D.Scenes;
using Easy3D.Scenes.Constraints;
using Easy3D.Scenes.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMeasure
{
    class Project
    {
        public Scene Scene;
        public CameraIntrinsics Camera;

        public Project(Scene scene, CameraIntrinsics camera)
        {
            this.Scene = scene;
            this.Camera = camera;
        }
    }
}

using Easy3D.Ui;
using Easy3D.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Easy3D.Scenes.Features;
using Easy3D.Projection;
using OpenTK;
using OpenCvSharp;

namespace PhotoMeasure.UI.Scenes
{
    class SceneView : InteractiveGLControl
    {
        private LocatedScene _Scene;

        public SceneView() : base()
        {
            base.Paint += Base_PaintScene;
        }

        public LocatedScene Scene
        {
            set
            {
                _Scene = value;
                this.Invalidate();
            }
        }

        private void Base_PaintScene(object sender, PaintEventArgs e)
        {
            if (_Scene == null)
            {
                return;
            }

            GL.PointSize(5);
            GL.Begin(PrimitiveType.Points);

            //GL.Vertex3(0, 0, 0);
            //GL.Vertex3(1, 0, 0);
            //GL.Vertex3(0, 1, 0);
            //GL.Vertex3(0, 0, 1);

            GL.Color3(Color.Green);
            foreach (var view in _Scene.Views)
            {
                GL.Vertex3(view.Camera.Location);
            }

            GL.Color3(Color.Gray);
            foreach (var feature in _Scene.Features)
            {
                if (feature.Type == FeatureType.Point)
                {
                    GL.Vertex3(feature.Point.Location);
                }
            }

            GL.End();

            foreach (var view in _Scene.Views)
            {
                DrawCamera(view.Camera);
            }
        }

        private void DrawCamera(LocatedCamera camera)
        {
            Vector3d v0 = camera.Location;
            Vector3d ul = v0 + camera.Unproject(new Point2f(0, 0)).Normalized();
            Vector3d ur = v0 + camera.Unproject(new Point2f(camera.Intrinsics.Width, 0)).Normalized();
            Vector3d ll = v0 + camera.Unproject(new Point2f(0, camera.Intrinsics.Height)).Normalized();
            Vector3d lr = v0 + camera.Unproject(new Point2f(camera.Intrinsics.Width, camera.Intrinsics.Height)).Normalized();
            GL.LineWidth(2);
            GL.Color3(Color.Green);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(v0); GL.Vertex3(ul);
            GL.Vertex3(v0); GL.Vertex3(ur);
            GL.Vertex3(v0); GL.Vertex3(ll);
            GL.Vertex3(v0); GL.Vertex3(lr);
            GL.Vertex3(ul); GL.Vertex3(ur);
            GL.Vertex3(ur); GL.Vertex3(lr);
            GL.Vertex3(lr); GL.Vertex3(ll);
            GL.Vertex3(ll); GL.Vertex3(ul);
            GL.End();
        }
    }
}

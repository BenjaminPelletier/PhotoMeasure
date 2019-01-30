using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using OpenTK;
using System.Windows.Forms;

namespace Easy3D.Ui
{
    public class MouseDragEventArgs : MouseEventArgs
    {
        public object SelectedObject;
        public Vector3d LastLocation;
        public Vector3d CurrentLocation;

        public MouseDragEventArgs(MouseEventArgs e, object selectedObject, Vector3d vStart, Vector3d vEnd)
            : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
            this.SelectedObject = selectedObject;
            this.LastLocation = vStart;
            this.CurrentLocation = vEnd;
        }
    }
}

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
using Easy3D.Geometry;

namespace Easy3D.Ui
{
    public class GLMouseUpEventArgs : MouseEventArgs
    {
        public Rayd InitialLocation;
        public bool Dragging;

        public GLMouseUpEventArgs(MouseEventArgs e, Rayd initialLocation, bool dragging)
            : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
            this.InitialLocation = initialLocation;
            this.Dragging = dragging;
        }
    }
}

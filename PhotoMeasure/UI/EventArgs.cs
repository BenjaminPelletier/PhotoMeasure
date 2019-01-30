using Easy3D.Projection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMeasure.UI
{
    public class CameraIntrinsicsRequiredEventArgs : EventArgs
    {
        public CameraIntrinsics Intrinsics;

        public CameraIntrinsicsRequiredEventArgs(CameraIntrinsics intrinsics = null)
        {
            Intrinsics = intrinsics;
        }
    }
}

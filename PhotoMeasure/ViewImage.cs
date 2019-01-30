using Easy3D.Projection;
using Easy3D.Scenes;
using Easy3D.Scenes.Observations;
using EasySerialization.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMeasure
{
    public class ViewImage : View
    {
        [JsonIgnore]
        private Bitmap _Bitmap;

        public ViewImage(CameraIntrinsics intrinsics, string imagePath)
            : base(intrinsics, new Observation[] { }, imagePath)
        { }

        public ViewImage(View v)
            : base(v.Intrinsics, v.Observations, v.ImagePath)
        { }

        public ViewImage(CameraIntrinsics intrinsics, IEnumerable<Observation> observations, string imagePath = null)
            : base(intrinsics, observations, imagePath)
        { }

        public string Name
        {
            get
            {
                var fi = new FileInfo(this.ImagePath);
                return fi.Name;
            }
        }

        public Bitmap Bitmap
        {
            get
            {
                if (this._Bitmap == null)
                {
                    this._Bitmap = new Bitmap(this.ImagePath);
                }
                return this._Bitmap;
            }
        }
    }
}

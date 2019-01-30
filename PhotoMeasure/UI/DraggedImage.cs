using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMeasure.UI
{
    class DraggedImage
    {
        private ViewImage _Image;

        public event EventHandler ClaimedAsReferenceImage;

        public DraggedImage(ViewImage img)
        {
            _Image = img;
        }

        public ViewImage ClaimAsReferenceImage()
        {
            ClaimedAsReferenceImage?.Invoke(this, EventArgs.Empty);
            return _Image;
        }
    }
}

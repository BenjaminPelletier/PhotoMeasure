using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Easy3D.Projection;
using Easy3D.Scenes.Features;

namespace PhotoMeasure.UI
{
    public partial class ReferenceImageViewer : UserControl
    {
        ViewImage _MeasuredImage;
        Dictionary<string, Feature> _SelectedFeatures = new Dictionary<string, Feature>();
        private const int CENTER_THRESHOLD = 20;

        private const float POINT_RADIUS = 5; //pixels

        public ReferenceImageViewer()
        {
            InitializeComponent();
        }

        private void ieViewer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DraggedImage)))
            {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void ieViewer_DragDrop(object sender, DragEventArgs e)
        {
            DraggedImage di = e.Data.GetData(typeof(DraggedImage)) as DraggedImage;
            _MeasuredImage = di.ClaimAsReferenceImage();
            ieImage.Image = _MeasuredImage.Bitmap;
        }

        public void SelectFeatures(IEnumerable<Feature> features)
        {
            _SelectedFeatures = features.ToDictionary(f => f.Name, f => f);
            if (_SelectedFeatures.Count > 0 && _MeasuredImage != null)
            {
                PointF imgPt = _MeasuredImage.GetObservation(_SelectedFeatures.First().Key).Point.Location;
                var c = ieImage.Converter;
                PointF viewportPt = c.Vp(imgPt);
                if (viewportPt.X < CENTER_THRESHOLD || viewportPt.Y < CENTER_THRESHOLD ||
                    viewportPt.X > ieImage.ClientSize.Width - CENTER_THRESHOLD ||
                    viewportPt.Y > ieImage.ClientSize.Height - CENTER_THRESHOLD)
                {
                    ieImage.CenterOn(imgPt);
                }
            }
            ieImage.Invalidate();
        }

        private void ieViewer_PaintImage(object sender, PaintEventArgs e)
        {
            if (_MeasuredImage == null) { return; }

            var c = ieImage.Converter;

            foreach (var observation in _MeasuredImage.Observations)
            {
                Pen pen;
                if (_SelectedFeatures.ContainsKey(observation.FeatureName)) { pen = Pens.Blue; }
                else { pen = Pens.Gray; }
                PointF p = c.Vp(observation.Point.Location);
                e.Graphics.DrawEllipse(pen, p.X - POINT_RADIUS, p.Y - POINT_RADIUS, 2 * POINT_RADIUS, 2 * POINT_RADIUS);
                e.Graphics.DrawLine(pen, p.X - 1, p.Y, p.X + 1, p.Y);
                e.Graphics.DrawLine(pen, p.X, p.Y - 1, p.X, p.Y + 1);
            }
        }
    }
}

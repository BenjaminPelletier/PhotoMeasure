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
using Easy3D.Scenes.Observations;

namespace PhotoMeasure.UI
{
    public partial class PrimaryImageEditor : UserControl
    {
        public class NewFeatureEventArgs : EventArgs
        {
            public FeatureType Type;
            public string DesiredName;
            public Feature Feature;
        }

        private ViewImage _MeasuredImage;
        private string _SelectedLabel;
        private Dictionary<string, Feature> _SelectedFeatures = new Dictionary<string, Feature>();

        private const float POINT_RADIUS = 5; //pixels

        private enum Mode
        {
            Cursor,
            AddPointFeatures,
            MatchPointFeatures,
        }
        private Mode _Mode = Mode.Cursor;

        public event EventHandler<NewFeatureEventArgs> NewFeature;

        public event EventHandler MatchPointsMode;

        public event EventHandler NextFeatureRequested;

        public PrimaryImageEditor()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewImage Image
        {
            set
            {
                _MeasuredImage = value;
                ieImage.Image = _MeasuredImage?.Bitmap;
            }
        }

        private void ChangeMode(Mode newMode)
        {
            _Mode = newMode;
            tslStatus.Text = newMode.ToString();
            if (newMode == Mode.Cursor)
            {
                tstbFeatureName.Visible = false;
            }
            else if (newMode == Mode.AddPointFeatures)
            {
                tstbFeatureName.Visible = true;
            }
        }

        private void tsbCursor_Click(object sender, EventArgs e)
        {
            ChangeMode(Mode.Cursor);
            ieImage.Cursor = Cursors.Default;
        }

        private void tsbAddPointFeatures_Click(object sender, EventArgs e)
        {
            ChangeMode(Mode.AddPointFeatures);
            ieImage.Cursor = Cursors.Cross;
        }

        private void ieImage_PaintImage(object sender, PaintEventArgs e)
        {
            if (_MeasuredImage == null) { return; }

            var c = ieImage.Converter;

            foreach (var obs in _MeasuredImage.Observations)
            {
                Pen pen;
                if (obs.FeatureName == _SelectedLabel) { pen = Pens.Red; }
                else if (_SelectedFeatures.ContainsKey(obs.FeatureName)) { pen = Pens.Blue; }
                else { pen = Pens.Gray; }
                PointF p = c.Vp(obs.Point.Location);
                e.Graphics.DrawEllipse(pen, p.X - POINT_RADIUS, p.Y - POINT_RADIUS, 2 * POINT_RADIUS, 2 * POINT_RADIUS);
                e.Graphics.DrawLine(pen, p.X - 1, p.Y, p.X + 1, p.Y);
                e.Graphics.DrawLine(pen, p.X, p.Y - 1, p.X, p.Y + 1);
            }
        }

        private void ieImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (_Mode == Mode.AddPointFeatures && _MeasuredImage != null && e.Button == MouseButtons.Left)
            {
                var f = new NewFeatureEventArgs { Type = FeatureType.Point, DesiredName = tstbFeatureName.Text };
                NewFeature?.Invoke(this, f);
                if (f.Feature == null) { return; }
                if (f.Feature.Type != FeatureType.Point) { throw new InvalidOperationException("Handler for PrimaryImageEditor.NewFeature created " + f.Feature.Type + " feature when POINT was requested"); }

                var c = ieImage.Converter;
                _SelectedLabel = f.Feature.Name;
                _MeasuredImage.Observations.Add(new Observation(_SelectedLabel, new PointObservation(c.Img((PointF)e.Location))));
                ieImage.Invalidate();
            }

            if (_Mode == Mode.MatchPointFeatures && _MeasuredImage != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    NextFeatureRequested?.Invoke(this, EventArgs.Empty);
                    return;
                }
                else if (e.Button == MouseButtons.Left)
                {
                    if (_SelectedFeatures.Count == 0)
                    {
                        MessageBox.Show(this, "No feature selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (_SelectedFeatures.Count > 1)
                    {
                        MessageBox.Show(this, "Too many features selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    Feature feature = _SelectedFeatures.First().Value;
                    if (feature.Type == FeatureType.Point)
                    {
                        var c = ieImage.Converter;
                        _MeasuredImage.Observations.Add(new Observation(feature.Name, new PointObservation(c.Img((PointF)e.Location))));
                        NextFeatureRequested?.Invoke(this, EventArgs.Empty);
                    } else
                    {
                        MessageBox.Show(this, "Labeling not yet supported for " + feature.Type + " features");
                        return;
                    }
                }
            }
        }

        public void SelectFeatures(IEnumerable<Feature> features)
        {
            _SelectedFeatures = features.ToDictionary(f => f.Name, f => f);
            ieImage.Invalidate();
        }

        private void tsbMatchPointFeatures_Click(object sender, EventArgs e)
        {
            ChangeMode(Mode.MatchPointFeatures);
            ieImage.Cursor = Cursors.Cross;
            MatchPointsMode?.Invoke(this, EventArgs.Empty);
        }
    }
}

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
    public partial class ImageViews : UserControl
    {
        public event EventHandler MatchPointsMode;
        public event EventHandler NextFeatureRequested;

        public ImageViews()
        {
            InitializeComponent();
            piePrimary.MatchPointsMode += (s, e) => MatchPointsMode?.Invoke(this, e);
            piePrimary.NextFeatureRequested += (s, e) => NextFeatureRequested?.Invoke(this, e);
        }

        public event EventHandler<PrimaryImageEditor.NewFeatureEventArgs> NewFeature
        {
            add { piePrimary.NewFeature += value; }
            remove { piePrimary.NewFeature -= value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewImage PrimaryImage
        {
            set
            {
                piePrimary.Image = value;
            }
        }

        public void SelectFeatures(IEnumerable<Feature> features)
        {
            piePrimary.SelectFeatures(features);
            rieReference.SelectFeatures(features);
        }
    }
}

using Easy3D.Projection;
using Easy3D.Scenes.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PhotoMeasure.UI
{
    public partial class FeatureList : UserControl
    {
        private ViewImage _Image;

        public class FeaturesSelectedEventArgs : EventArgs
        {
            public readonly Feature[] SelectedFeatures;

            public FeaturesSelectedEventArgs(IEnumerable<Feature> features)
            {
                this.SelectedFeatures = features.ToArray();
            }
        }

        private NewFeatureDialog nfdNewFeature = new NewFeatureDialog();

        public event EventHandler<FeaturesSelectedEventArgs> FeaturesSelected;

        private bool _FeaturesChangedEnabled = true;
        public event EventHandler FeaturesChanged;

        public FeatureList()
        {
            InitializeComponent();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            if (nfdNewFeature.ShowDialog(this) == DialogResult.OK)
            {
                if (this.FeaturesByName.ContainsKey(nfdNewFeature.FeatureName))
                {
                    MessageBox.Show(this, "There is already a feature named '" + nfdNewFeature.FeatureName + "'", "Error creating feature", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                AddFeature(new Feature(nfdNewFeature.FeatureType, nfdNewFeature.FeatureName));
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<Feature> Features
        {
            get
            {
                foreach (ListViewItem item in lvFeatures.Items)
                {
                    yield return item.Tag as Feature;
                }
            }
            set
            {
                lvFeatures.Items.Clear();
                _FeaturesChangedEnabled = false;
                foreach (Feature feature in value)
                {
                    AddFeature(feature);
                }
                _FeaturesChangedEnabled = true;
                FeaturesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<Feature> SelectedFeatures
        {
            get
            {
                var alreadyReturned = new HashSet<ListViewItem>();
                foreach (ListViewItem item in lvFeatures.Items)
                {
                    if (item.Selected && item.Focused)
                    {
                        yield return item.Tag as Feature;
                        alreadyReturned.Add(item);
                    }
                }
                foreach (ListViewItem item in lvFeatures.Items)
                {
                    if (item.Selected && !alreadyReturned.Contains(item))
                    {
                        yield return item.Tag as Feature;
                    }
                }
            }
            set
            {
                var selected = value.ToDictionary(f => f, f => true);
                foreach (ListViewItem item in lvFeatures.Items)
                {
                    item.Selected = selected.ContainsKey(item.Tag as Feature);
                }
            }
        }

        public void SelectNextFeature()
        {
            Feature feature = SelectedFeatures.FirstOrDefault();
            for (int i = 0; i < lvFeatures.Items.Count; i++)
            {
                if (lvFeatures.Items[i].Tag == feature)
                {
                    SelectedFeatures = new Feature[] { lvFeatures.Items[(i + 1) % lvFeatures.Items.Count].Tag as Feature };
                    return;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewImage MeasuredImage
        {
            set
            {
                _Image = value;
                lvFeatures.Invalidate();
            }
        }

        private void AddFeature(Feature feature)
        {
            var item = new ListViewItem(feature.Name);
            item.Tag = feature;
            lvFeatures.Items.Add(item);
            if (_FeaturesChangedEnabled)
            {
                FeaturesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Dictionary<string, Feature> FeaturesByName
        {
            get
            {
                var result = new Dictionary<string, Feature>();
                foreach (ListViewItem item in lvFeatures.Items)
                {
                    var feature = item.Tag as Feature;
                    result[feature.Name] = feature;
                }
                return result;
            }
        }

        public Feature AddFeature(FeatureType type, string desiredName)
        {
            var featuresByName = this.FeaturesByName;
            var baseName = desiredName == "" ? type.ToString() : desiredName;
            int i = 1;
            while (true)
            {
                string name = baseName + " " + i;
                if (featuresByName.ContainsKey(name))
                {
                    i += 1;
                }
                else
                {
                    var feature = new Feature(type, name);
                    AddFeature(feature);
                    return feature;
                }
            }
        }

        private void lvFeatures_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            e.Item.BackColor = e.IsSelected ? System.Drawing.Color.LightGray : lvFeatures.BackColor;
            FeaturesSelected?.Invoke(this, new FeaturesSelectedEventArgs(this.SelectedFeatures));
        }

        private void lvFeatures_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Brush bg;
            if (e.Item.Selected)
            {
                bg = Brushes.LightBlue;
            }
            else
            {
                bg = Brushes.White;
            }

            e.Graphics.FillRectangle(bg, e.Bounds);

            if ((e.State & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                e.DrawFocusRectangle();
            }

            if (_Image == null)
            {
                e.Item.ForeColor = Color.Goldenrod;
            }
            else if (_Image.Observations.Any(obs => obs.FeatureName == (e.Item.Tag as Feature).Name))
            {
                e.Item.ForeColor = Color.Black;
            }
            else
            {
                e.Item.ForeColor = Color.Gray;
            }

            e.DrawText();
        }
    }
}

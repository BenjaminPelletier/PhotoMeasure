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
    public partial class ImageList : UserControl
    {
        private ListViewItem _DisplayedItem;
        private ListViewItem _ReferenceItem;
        public CameraIntrinsics Intrinsics;
        private Dictionary<string, Feature> _SelectedFeatures = new Dictionary<string, Feature>();

        public event EventHandler<CameraIntrinsicsRequiredEventArgs> IntrinsicsRequired;

        public ImageList()
        {
            InitializeComponent();
        }

        public class ImageChangedEventArgs : EventArgs
        {
            public ViewImage Image;

            public ImageChangedEventArgs(ViewImage img)
            {
                this.Image = img;
            }
        }

        public event EventHandler<ImageChangedEventArgs> ImageChanged;

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            var args = new CameraIntrinsicsRequiredEventArgs(Intrinsics);
            IntrinsicsRequired?.Invoke(this, args);
            Intrinsics = args.Intrinsics;
            if (Intrinsics == null)
            {
                MessageBox.Show(this, "The intrinsics for the camera that took an image or images must first be defined before adding that image or images", "Cannot add image", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (ofdImage.ShowDialog(this) == DialogResult.OK)
            {
                foreach (var filename in ofdImage.FileNames)
                {
                    var img = new ViewImage(Intrinsics, filename);
                    var item = new ListViewItem(img.Name);
                    item.Tag = img;
                    lvImages.Items.Add(item);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ViewImage> Images
        {
            get
            {
                foreach (ListViewItem item in lvImages.Items)
                {
                    yield return item.Tag as ViewImage;
                }
            }
            set
            {
                lvImages.Items.Clear();
                foreach (ViewImage img in value)
                {
                    var item = new ListViewItem(img.Name);
                    item.Tag = img;
                    lvImages.Items.Add(item);
                }
            }
        }

        private void lvImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewImage img = null;
            if (lvImages.SelectedItems.Count > 0)
            {
                img = lvImages.SelectedItems[0].Tag as ViewImage;
                _DisplayedItem = lvImages.SelectedItems[0];
            }
            lvImages.Invalidate();
            ImageChanged?.Invoke(this, new ImageChangedEventArgs(img));
        }

        private void lvImages_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Brush bg;
            if (e.Item.Selected)
            {
                if (e.Item == _DisplayedItem)
                {
                    bg = Brushes.LightBlue;
                }
                else
                {
                    bg = Brushes.LightGreen;
                }
            }
            else if (e.Item == _ReferenceItem)
            {
                bg = Brushes.LightGoldenrodYellow;
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

            if (_SelectedFeatures.Count == 0)
            {
                e.Item.ForeColor = Color.Black;
            }
            else
            {
                ViewImage img = e.Item.Tag as ViewImage;
                if (img.Observations.Any(obs => _SelectedFeatures.ContainsKey(obs.FeatureName)))
                {
                    e.Item.ForeColor = Color.Black;
                }
                else
                {
                    e.Item.ForeColor = Color.Gray;
                }
            }

            e.DrawText();
        }

        private void lvImages_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var lvi = e.Item as ListViewItem;
            var di = new DraggedImage(lvi.Tag as ViewImage);
            di.ClaimedAsReferenceImage += (s, _) =>
            {
                _ReferenceItem = lvi;
                lvImages.Invalidate();
            };
            DoDragDrop(di, DragDropEffects.Link);
        }

        public void SelectFeatures(IEnumerable<Feature> features)
        {
            _SelectedFeatures = features.ToDictionary(f => f.Name, f => f);
            lvImages.Invalidate();
        }
    }
}

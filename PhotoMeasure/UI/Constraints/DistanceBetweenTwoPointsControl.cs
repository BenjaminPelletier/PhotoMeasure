using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Easy3D.Scenes;
using Easy3D.Projection;
using Easy3D.Scenes.Features;
using Easy3D.Scenes.Constraints;

namespace PhotoMeasure.UI.Constraints
{
    public partial class DistanceBetweenTwoPointsControl : UserControl, IConstraintProvider
    {
        private List<Feature> _Features = new List<Feature>();
        private bool _CurrentlyValid = false;

        public event EventHandler<ConstraintValidityEventArgs> ConstraintValidityChanged;

        public DistanceBetweenTwoPointsControl()
        {
            InitializeComponent();
        }

        public IEnumerable<Feature> Features
        {
            set
            {
                _Features = value.ToList();
            }
        }

        public Constraint GetConstraint(string name)
        {
            return _CurrentlyValid ? new Constraint(name, new DistanceBetweenTwoPointsConstraint(cbPoint1.Text, cbPoint2.Text, double.Parse(tbDistance.Text))) : null;
        }

        private void DistanceBetweenTwoPointsControl_Load(object sender, EventArgs e)
        {
            foreach (Feature f in _Features)
            {
                if (f.Type == FeatureType.Point)
                {
                    cbPoint1.Items.Add(f.Name);
                    cbPoint2.Items.Add(f.Name);
                }
            }
            if (cbPoint1.Items.Count >= 1)
            {
                cbPoint1.SelectedIndex = 0;
            }
            if (cbPoint2.Items.Count >= 2)
            {
                cbPoint2.SelectedIndex = 1;
                ConstraintValidityChanged?.Invoke(this, new ConstraintValidityEventArgs(true));
            }
        }

        private void CheckValidity()
        {
            double dummy;
            bool nowValid = cbPoint1.SelectedIndex > -1 || cbPoint2.SelectedIndex > -1 || cbPoint1.SelectedIndex != cbPoint2.SelectedIndex || double.TryParse(tbDistance.Text, out dummy);
            if (_CurrentlyValid != nowValid)
            {
                _CurrentlyValid = nowValid;
                ConstraintValidityChanged?.Invoke(this, new ConstraintValidityEventArgs(nowValid));
            }
        }

        private void cbPoint1_TextChanged(object sender, EventArgs e)
        {
            CheckValidity();
        }

        private void cbPoint2_TextChanged(object sender, EventArgs e)
        {
            CheckValidity();
        }

        private void tbDistance_TextChanged(object sender, EventArgs e)
        {
            CheckValidity();
        }
    }
}

using Easy3D.Projection;
using Easy3D.Scenes;
using Easy3D.Scenes.Constraints;
using Easy3D.Scenes.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMeasure.UI.Constraints
{
    public partial class ConstraintDialog : Form
    {
        private int _ButtonSpacing;
        private int _BaseHeight;
        private Control _Details;
        private bool _CurrentlyValid = false;
        private List<Feature> _Features = new List<Feature>();

        public ConstraintDialog()
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

        private void cmdOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public ConstraintType ConstraintType
        {
            get
            {
                return (ConstraintType)Enum.Parse(typeof(ConstraintType), cbType.Text);
            }
        }

        public Constraint Constraint
        {
            get
            {
                return (_Details as IConstraintProvider)?.GetConstraint(txtName.Text);
            }
        }

        private void ConstraintDialog_Load(object sender, EventArgs e)
        {
            _ButtonSpacing = cmdCancel.Top - txtName.Bottom;
            _BaseHeight = this.Height;
            foreach (string t in Enum.GetNames(typeof(ConstraintType)))
            {
                cbType.Items.Add(t);
            }
            cbType.SelectedIndex = 0;
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeValidity(false);
            switch (this.ConstraintType)
            {
                case ConstraintType.DistanceBetweenTwoPoints:
                    if (_Details != null && _Details.GetType().IsInstanceOfType(typeof(DistanceBetweenTwoPointsControl)))
                    {
                        return;
                    }
                    if (_Details != null)
                    {
                        this.Controls.Remove(_Details);
                    }
                    var dbtpc = new DistanceBetweenTwoPointsControl();
                    dbtpc.Features = _Features;
                    dbtpc.ConstraintValidityChanged += Details_ConstraintValidityChanged;
                    _Details = dbtpc;
                    _Details.Top = txtName.Bottom + _ButtonSpacing;
                    break;

                default:
                    if (_Details != null)
                    {
                        this.Controls.Remove(_Details);
                    }
                    var lbl = new Label();
                    lbl.AutoSize = true;
                    lbl.ForeColor = Color.Red;
                    _Details = lbl;
                    _Details.Text = "Constraint type " + this.ConstraintType + " not yet supported";
                    _Details.Top = txtName.Bottom + _ButtonSpacing;
                    break;
            }
            _Details.Left = cmdCancel.Left;
            this.Controls.Add(_Details);
            this.Height = _BaseHeight + _Details.Height + _ButtonSpacing;
        }

        private void Details_ConstraintValidityChanged(object sender, ConstraintValidityEventArgs e)
        {
            ChangeValidity(e.ConstraintValid);
        }

        private void ChangeValidity(bool valid)
        {
            _CurrentlyValid = valid;
            cmdOk.Enabled = valid;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            ChangeValidity(txtName.Text.Length > 0); //TODO: Make sure there are no Constraint name collisions
        }
    }
}

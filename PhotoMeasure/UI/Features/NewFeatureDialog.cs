using Easy3D.Scenes.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMeasure.UI
{
    public partial class NewFeatureDialog : Form
    {
        public NewFeatureDialog()
        {
            InitializeComponent();
            foreach (string t in Enum.GetNames(typeof(FeatureType)))
            {
                cbType.Items.Add(t);
            }
            cbType.SelectedIndex = 0;
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

        public FeatureType FeatureType
        {
            get
            {
                return (FeatureType)Enum.Parse(typeof(FeatureType), cbType.Text);
            }
        }

        public string FeatureName
        {
            get
            {
                return txtName.Text;
            }
        }
    }
}

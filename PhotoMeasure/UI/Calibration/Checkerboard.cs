using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMeasure.UI
{
    public partial class Checkerboard : Form
    {
        private Bitmap _Checkerboard;

        public Checkerboard()
        {
            InitializeComponent();

            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("PhotoMeasure.Resources.Checkerboard.png"))
            {
                _Checkerboard = new Bitmap(s);
            }
        }

        private void Checkerboard_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Checkerboard_Paint(object sender, PaintEventArgs e)
        {
            float fx = (float)this.ClientSize.Width / _Checkerboard.Width;
            float fy = (float)this.ClientSize.Height / _Checkerboard.Height;
            float x0 = 0;
            float y0 = 0;
            if (fx < fy)
            {
                // Image is wider than form
                fy = fx;
                y0 = (this.ClientSize.Height - _Checkerboard.Height * fy) / 2;
            }
            else
            {
                // Image is taller than form
                fx = fy;
                x0 = (this.ClientSize.Width - _Checkerboard.Width * fx) / 2;
            }
            e.Graphics.Clear(Color.Black);
            e.Graphics.DrawImage(_Checkerboard, x0, y0, _Checkerboard.Width * fx, _Checkerboard.Height * fy);
        }

        private void Checkerboard_Load(object sender, EventArgs e)
        {

        }
    }
}

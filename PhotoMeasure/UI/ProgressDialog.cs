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
    public partial class ProgressDialog : Form
    {
        private BackgroundWorker _Worker;

        public ProgressDialog(BackgroundWorker worker, string dialogTitle)
        {
            InitializeComponent();
            this.Text = dialogTitle;
            _Worker = worker;
            _Worker.WorkerReportsProgress = true;
            _Worker.WorkerSupportsCancellation = true;
            _Worker.ProgressChanged += Worker_ProgressChanged;
            _Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            _Worker.CancelAsync();
            cmdCancel.Text = "Cancelling...";
            cmdCancel.Enabled = false;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage;
            lblStatus.Text = e.UserState.ToString();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else if (e.Error != null)
            {
                this.DialogResult = DialogResult.Abort;
                lblStatus.Text = e.Error.ToString();
                cmdCancel.Visible = false;
                cmdOk.Visible = true;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ProgressDialog_Load(object sender, EventArgs e)
        {
            _Worker.RunWorkerAsync();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

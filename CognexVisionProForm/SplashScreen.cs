using System;
using System.Windows.Forms;

namespace CognexVisionProForm
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            pbLoadApp.Value = 0;
        }

        public void UpdateProgress(string text, int percent)
        {
            int loadAppValue;
            lbLoadProgress.Text = text;
            loadAppValue = pbLoadApp.Value + percent;

            if (loadAppValue <= 100) { pbLoadApp.Value = loadAppValue; }
            else { pbLoadApp.Value = 100; }
            this.Refresh();

        }

        private void lbLoadProgress_Click(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CognexVisionProForm
{
    public partial class ShowResultData : Form
    {
        CameraControl cameraData;
        public ShowResultData(CameraControl Sender)
        {
            cameraData = Sender;
            InitializeComponent();
        }

        private void ShowResultData_Load(object sender, EventArgs e)
        {
            UpdateResultData();
        }

        public void UpdateResultData()
        {
            this.Text = cameraData.Camera.Name + " Tool Data";

            Control control = (Control)cameraData;
            Point cameraDataLocation = control.PointToScreen(cameraData.Location);


            this.Left = cameraDataLocation.X + 5;
            this.Top = cameraDataLocation.Y + 5;

            this.Width = cameraData.Width - 10;
            this.Height = cameraData.Height - 10;

            //Update information for Preprocess
            lbPreprocessResultdata.Items.Clear();
            lbPreprocessResultdata.Items.Add($"Results - {cameraData.PreProcess.cogToolBlock.RunStatus.Result}");
            lbPreprocessResultdata.Items.Add($"TotalTime - {cameraData.PreProcess.cogToolBlock.RunStatus.TotalTime.ToString()}");
            lbPreprocessResultdata.Height = lbPreprocessResultdata.PreferredHeight;

            label2.Top = lbPreprocessResultdata.Bottom + 10;
            label2.Visible = true;

            tbPreprocessMessage.Top = label2.Bottom + 5;
            tbPreprocessMessage.Visible = true;
            tbPreprocessMessage.Enabled = false;
            tbPreprocessMessage.Clear();
            tbPreprocessMessage.Text = cameraData.PreProcess.cogToolBlock.RunStatus.Message;
            tbPreprocessMessage.Height = lbPreprocessResultdata.PreferredHeight;

            //Update information for selected tool
            label9.Top = tbPreprocessMessage.Bottom + 10;
            lbToolResultData.Top = label9.Bottom + 5;
            lbToolResultData.Items.Clear();
            lbToolResultData.Items.Add($"Results - {cameraData.Tool.cogToolBlock.RunStatus.Result}");
            lbToolResultData.Items.Add($"TotalTime - {cameraData.Tool.cogToolBlock.RunStatus.TotalTime.ToString()}");
            lbToolResultData.Height = lbToolResultData.PreferredHeight;
            tbToolMessage.Clear();


            label1.Top = lbToolResultData.Bottom + 10;
            label1.Visible = true;
            tbToolMessage.Visible = true;
            tbToolMessage.Enabled = false;

            tbToolMessage.Top = label1.Bottom + 5;
            tbToolMessage.Text = cameraData.Tool.cogToolBlock.RunStatus.Message;         
            tbToolMessage.Height = tbToolMessage.PreferredHeight;
        }
    }
}

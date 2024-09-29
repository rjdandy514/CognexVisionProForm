using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            lbToolResultData.Items.Clear();
            lbToolResultData.Items.Add($"Results - {cameraData.Tool.cogToolBlock.RunStatus.Result}");
            lbToolResultData.Items.Add($"TotalTime - {cameraData.Tool.cogToolBlock.RunStatus.TotalTime.ToString()}");
            tbToolMessage.Clear();
            tbToolMessage.Text = cameraData.Tool.cogToolBlock.RunStatus.Message;
        }
    }
}

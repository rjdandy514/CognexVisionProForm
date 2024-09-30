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

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(String));

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

            lProcessMessage.Top = lbPreprocessResultdata.Bottom + 10;
            lProcessMessage.Visible = true;

            tbPreprocessMessage.Top = lProcessMessage.Bottom + 5;
            tbPreprocessMessage.Visible = true;
            tbPreprocessMessage.Enabled = false;
            tbPreprocessMessage.Clear();
            if(cameraData.PreProcess.cogToolBlock.RunStatus.Result == Cognex.VisionPro.CogToolResultConstants.Accept)
            {
                tbPreprocessMessage.Text = "No Messages";
            }
            else
            {
                tbPreprocessMessage.Text = cameraData.PreProcess.cogToolBlock.RunStatus.Message;
            }
            
            tbPreprocessMessage.Height = lbPreprocessResultdata.PreferredHeight;

            //Update information for selected tool
            lToolBlockResult.Top = tbPreprocessMessage.Bottom + 10;
            lbToolResultData.Top = lToolBlockResult.Bottom + 5;
            lbToolResultData.Items.Clear();
            lbToolResultData.Items.Add($"Results - {cameraData.Tool.cogToolBlock.RunStatus.Result}");
            lbToolResultData.Items.Add($"TotalTime - {cameraData.Tool.cogToolBlock.RunStatus.TotalTime.ToString()}");
            lbToolResultData.Height = lbToolResultData.PreferredHeight;
            tbToolMessage.Clear();


            lToolBlockMessage.Top = lbToolResultData.Bottom + 10;
            lToolBlockMessage.Visible = true;
            
            tbToolMessage.Visible = true;
            tbToolMessage.Enabled = false;
            tbToolMessage.Top = lToolBlockMessage.Bottom + 5;
            if(cameraData.Tool.cogToolBlock.RunStatus.Result == Cognex.VisionPro.CogToolResultConstants.Accept)
            {
                tbToolMessage.Text = "No Messages";
            }
            else
            {
                tbToolMessage.Text = cameraData.Tool.cogToolBlock.RunStatus.Message;
            }
            tbToolMessage.Height = tbToolMessage.PreferredHeight;

            //
            // Preprocess Input
            //
            lPreprocessInput.Top = tbToolMessage.Bottom + 10;
            lbPreprocessInput.Top = lPreprocessInput.Bottom + 5;
            
            lbPreprocessInput.Items.Clear();
            lbPreprocessInput.Height = lbPreprocessInput.PreferredHeight;

            //
            // Preprocess Output
            //
            lPreprocessOutput.Top = lbPreprocessInput.Bottom + 10;
            lbPreprocessOutput.Top = lPreprocessOutput.Bottom + 5;
            
            lbPreprocessOutput.Items.Clear();
            lbPreprocessOutput.Height = lbPreprocessOutput.PreferredHeight;
            //
            // Tool Block Input
            //
            lToolBlockInput.Top = lbPreprocessOutput.Bottom + 10;
            lbToolBlockInput.Top = lToolBlockInput.Bottom + 5;
            lbToolBlockInput.Items.Clear();

            for (int i = 0; i < cameraData.Tool.cogToolBlock.Inputs.Count; i++)
            {

                var toolBlockInput = cameraData.Tool.cogToolBlock.Inputs[i];
                string toolBlockInputString = toolBlockInput.Name;
                var toolBlockInputValue = toolBlockInput.Value;


                if (converter.IsValid(toolBlockInputValue.ToString()))
                {
                    toolBlockInputString = toolBlockInputString + " - " + toolBlockInputValue.ToString();
                }
                else
                {
                    toolBlockInputString = toolBlockInputString + " - " + toolBlockInput.GetType().Name;
                }

                lbToolBlockInput.Items.Add(toolBlockInputString);
            }

            lbToolBlockInput.Height = lbToolBlockInput.PreferredHeight;
            //
            // Tool Block Output
            //
            lToolBlockOutput.Top = lbToolBlockInput.Bottom + 10;
            lbToolBlockOutput.Top = lToolBlockOutput.Bottom + 5;
            
            lbToolBlockOutput.Items.Clear();

            for (int i = 0; i < cameraData.Tool.cogToolBlock.Outputs.Count; i++)
            {

                var toolBlockOutput = cameraData.Tool.cogToolBlock.Outputs[i];
                string toolBlockOutputString = toolBlockOutput.Name;
                var toolBlockOutputValue = toolBlockOutput.Value;

                if (converter.IsValid(toolBlockOutputValue.ToString()))
                {
                    toolBlockOutputString = toolBlockOutputString + " - " + toolBlockOutputValue.ToString();
                }
                else
                {
                    toolBlockOutputString = toolBlockOutputString + " - " + toolBlockOutput.GetType().Name;
                }

                lbToolBlockOutput.Items.Add(toolBlockOutputString);
            }

            lbToolBlockOutput.Height = lbToolBlockOutput.PreferredHeight;


        }
    }
}

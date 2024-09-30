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
            

            
            lbPreprocessResultdata.Top = lPrepocessResult.Bottom + 5;
            lbPreprocessResultdata.Items.Clear();
            if (cameraData.PreProcess.ToolReady)
            {
                lbPreprocessResultdata.Items.Add($"Results - {cameraData.PreProcess.cogToolBlock.RunStatus.Result}");
                lbPreprocessResultdata.Items.Add($"TotalTime - {cameraData.PreProcess.cogToolBlock.RunStatus.TotalTime.ToString()}");
            }
            lbPreprocessResultdata.Height = lbPreprocessResultdata.PreferredHeight;

            lProcessMessage.Top = lbPreprocessResultdata.Bottom + 10;
            lProcessMessage.Visible = true;

            tbPreprocessMessage.Top = lProcessMessage.Bottom + 5;
            tbPreprocessMessage.Visible = true;
            tbPreprocessMessage.Enabled = true;
            tbPreprocessMessage.ReadOnly = true;
            tbPreprocessMessage.Clear();

            if(!cameraData.PreProcess.ToolReady || cameraData.PreProcess.cogToolBlock.RunStatus.Result == Cognex.VisionPro.CogToolResultConstants.Accept)
            {
                tbPreprocessMessage.Text = "No Messages";
            }
            else
            {
                tbPreprocessMessage.Text = cameraData.PreProcess.cogToolBlock.RunStatus.Message;
            }

            tbPreprocessMessage.Height = tbPreprocessMessage.PreferredHeight;


            //Update information for selected tool
            lToolBlockResult.Top = tbPreprocessMessage.Bottom + 10;
            lbToolResultData.Top = lToolBlockResult.Bottom + 5;
            lbToolResultData.Items.Clear();
            lbToolResultData.Items.Add($"Results - {cameraData.Tool.cogToolBlock.RunStatus.Result}");
            lbToolResultData.Items.Add($"TotalTime - {cameraData.Tool.cogToolBlock.RunStatus.TotalTime.ToString()}");
            lbToolResultData.Height = lbToolResultData.PreferredHeight;
            lToolBlockMessage.Top = lbToolResultData.Bottom + 10;
            lToolBlockMessage.Visible = true;
            
            tbToolMessage.Clear();
            tbToolMessage.Visible = true;
            tbToolMessage.Enabled = true;
            tbToolMessage.ReadOnly = true;
            tbToolMessage.Top = lToolBlockMessage.Bottom + 5;
            tbToolMessage.Width = this.Width - 50;

            if (cameraData.Tool.cogToolBlock.RunStatus.Result == Cognex.VisionPro.CogToolResultConstants.Accept)
            {
                tbToolMessage.Text = "No Messages";
            }
            else
            {
                int stringIndex = 0;
                int stringLength = tbToolMessage.Width / tbToolMessage.Font.Height;
                string initialMessage = cameraData.Tool.cogToolBlock.RunStatus.Message;
                string[] stringSplit = new string[1+(initialMessage.Length/ stringLength)];
                


                while (initialMessage != "")
                {
                    int remainingString = initialMessage.Length;

                    if (remainingString < stringLength)
                    {
                        stringLength = remainingString;
                    }

                    stringSplit[stringIndex] = initialMessage.Substring(0, stringLength);
                    initialMessage = initialMessage.Remove(0, stringLength);
                    stringIndex++;
                }
                
                tbToolMessage.Lines = stringSplit;
                Size sizetemp = tbToolMessage.PreferredSize;
            }
            
            tbToolMessage.Height = tbToolMessage.PreferredSize.Height;
            tbToolMessage.Width = tbToolMessage.PreferredSize.Width;


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


                if (toolBlockInput.Value != null && converter.IsValid(toolBlockInputValue.ToString()))
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

                if (toolBlockOutput.Value != null && converter.IsValid(toolBlockOutputValue.ToString()))
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



            this.Width = this.PreferredSize.Width;


        }
    }
}

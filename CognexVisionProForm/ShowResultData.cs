using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
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
using static System.Resources.ResXFileRef;

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

            int boxWidth = 50;

            this.Text = cameraData.Camera.Name + " Tool Data";

            Control control = (Control)cameraData;
            Point cameraDataLocation = control.PointToScreen(cameraData.Location);

            lPrepocessResult.Left = 5;
            lProcessMessage.Left = 5;
            lPreprocessInput.Left = 5;
            lPreprocessOutput.Left = 5;

            lToolBlockResult.Left = 5;
            lToolBlockMessage.Left = 5;
            lToolBlockInput.Left = 5;
            lToolBlockOutput.Left = 5;

            //Update information for Preprocess
            lbPreprocessResultdata.Top = lPrepocessResult.Bottom + 5;
            setupResults(lbPreprocessResultdata, cameraData.PreProcess);
            lbPreprocessResultdata.Width = this.Width - boxWidth;

            lProcessMessage.Top = lbPreprocessResultdata.Bottom + 10;

            tbPreprocessMessage.Top = lProcessMessage.Bottom + 5;
            setupMessages(tbPreprocessMessage, cameraData.PreProcess);
            

            //Update information for selected tool
            lToolBlockResult.Top = tbPreprocessMessage.Bottom + 10;
            
            lbToolResultData.Top = lToolBlockResult.Bottom + 5;
            lbToolResultData.Width = this.Width - boxWidth;
            setupResults(lbToolResultData, cameraData.Tool);
            lbToolResultData.Height = lbToolResultData.PreferredHeight;



            lToolBlockMessage.Top = lbToolResultData.Bottom + 10;
            tbToolMessage.Top = lToolBlockMessage.Bottom + 5;
            setupMessages(tbToolMessage, cameraData.Tool);

            //
            // Preprocess Input
            //
            lPreprocessInput.Top = tbToolMessage.Bottom + 10;
            lbPreprocessInput.Top = lPreprocessInput.Bottom + 5;

            setupIO(lbPreprocessInput, cameraData.PreProcess.Inputs);
            lbPreprocessInput.Width = this.Width - boxWidth;
            lbPreprocessInput.Height = lbPreprocessInput.PreferredHeight;

            //
            // Preprocess Output
            //
            lPreprocessOutput.Top = lbPreprocessInput.Bottom + 10;
            lbPreprocessOutput.Top = lPreprocessOutput.Bottom + 5;

            setupIO(lbPreprocessOutput, cameraData.PreProcess.Outputs);
            lbPreprocessOutput.Width = this.Width - boxWidth;
            lbPreprocessOutput.Height = lbPreprocessOutput.PreferredHeight;
            //
            // Tool Block Input
            //
            lToolBlockInput.Top = lbPreprocessOutput.Bottom + 10;
            lbToolBlockInput.Top = lToolBlockInput.Bottom + 5;

            setupIO(lbToolBlockInput, cameraData.Tool.Inputs);
            lbToolBlockInput.Width = this.Width - boxWidth;

            //
            // Tool Block Output
            //
            lToolBlockOutput.Top = lbToolBlockInput.Bottom + 10;
            lbToolBlockOutput.Top = lToolBlockOutput.Bottom + 5;

            setupIO(lbToolBlockOutput, cameraData.Tool.Outputs);
            lbToolBlockOutput.Width = this.Width - boxWidth;
        }

        public void setupResults(ListBox box, ToolBlock tool)
        {
            box.Left = 5;
            box.Items.Clear();
            if (tool.ToolReady)
            {
                box.Items.Add($"Results - {tool.toolBlock.RunStatus.Result}");
                box.Items.Add($"TotalTime - {tool.toolBlock.RunStatus.TotalTime.ToString()}");
            }
            box.Height = box.PreferredHeight;
            box.Update();
        }
        public void setupMessages(RichTextBox box, ToolBlock tool)
        {
            int stringIndex;
            int stringLength;
            string initialMessage;
            string[] stringSplit;
            int remainingString;

            box.Left = 5;
            box.Visible = true;
            box.Enabled = true;
            box.ReadOnly = true;
            box.Width = this.Width - 50;
            box.Clear();

            if (tool.Result) { box.Text = "No Messages"; }
            else if(tool.ToolReady)
            {
                stringIndex = 0;
                stringLength = box.Width / (box.Font.Height/2);
                initialMessage = tool.toolBlock.RunStatus.Message;
                stringSplit = new string[1 + (initialMessage.Length / stringLength)];


                while (initialMessage != "")
                {
                    remainingString = initialMessage.Length;

                    if (remainingString < stringLength) { stringLength = remainingString; }

                    stringSplit[stringIndex] = initialMessage.Substring(0, stringLength);
                    initialMessage = initialMessage.Remove(0, stringLength);
                    stringIndex++;
                }

                box.Lines = stringSplit;
                Size sizetemp = box.PreferredSize;
            }

            box.Height = box.PreferredSize.Height;
            box.Update();
        }
        public void setupIO(ListBox box, CogToolBlockTerminalCollection data)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(String));
            string toolBlockOutputString;

            box.Left = 5;
            box.Items.Clear();
                for (int i = 0; i < data.Count; i++)
            {

                toolBlockOutputString = data[i].Name;

                if (data[i].Value != null && converter.IsValid(data[i].Value.ToString()))
                {
                    toolBlockOutputString = toolBlockOutputString + " - " + data[i].Value.ToString();
                }
                else { toolBlockOutputString = toolBlockOutputString + " - " + data[i].GetType().Name; }

                box.Items.Add(toolBlockOutputString);
            }

            box.Height = box.PreferredHeight;
            box.Update();
        }
    }
}

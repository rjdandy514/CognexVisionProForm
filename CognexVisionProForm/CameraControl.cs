using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;

namespace CognexVisionProForm
{
    public partial class CameraControl : Form
    {
        CognexVisionProForm _form;
        DalsaImage camera;
        ToolBlock tool;
        ToolBlock preProcess;
        public ICogImage image;
        public ICogRecord record;
        private int toolSelect = 0;
        private bool updateDisplayRequest;
        private System.Windows.Forms.Timer pollingTimer;
        ShowResultData toolFailedDisplay;
        public DalsaImage Camera
        {
            get { return camera; }
        }
        public ToolBlock Tool
        {
            get
            {
                return tool; 
            }

            set
            {
                tool = value;
            }
        }
        public ToolBlock PreProcess
        {
            get { return preProcess; }
            set { preProcess = value; }
        }
        public int ToolSelect
        {
            get
            {
                return toolSelect;
            }
            set
            {
                toolSelect = value;
            }
        }
        public bool UpdateDisplayRequest
        {
            set 
            {
                updateDisplayRequest = value;
                BeginInvoke(new Set_UpdateDisplay(UpdateToolDisplay));

            }
        }
        public CameraControl(CognexVisionProForm Sender, DalsaImage Camera)
        {
            _form = new CognexVisionProForm();
            _form = Sender;
            camera = Camera;
            toolFailedDisplay = new ShowResultData(this);



            InitializeComponent();
        }
        private void pollingTimer_Tick(object sender, EventArgs e)
        {
            pollingTimer.Stop();

            cbCameraConnected.Checked = camera.Connected;
            cbTrigger.Checked = camera.Trigger;
            cbArchiveImageActive.Checked = camera.ArchiveImageActive;
            cbImageReady.Checked = camera.ImageReady;
            numToolSelect.Value = toolSelect;
            UpdateButton();
            pollingTimer.Start();
        }
        private void UpdateButton()
        {
            numToolSelect.Enabled = !_form.PlcCommsActive;

            bool imageControl = !_form.PlcCommsActive && !(camera.Snapping || camera.Acquiring || camera.Grabbing);
            bttnCameraSnap.Enabled = imageControl;
            bttnGrab.Enabled = imageControl;

            bttnCameraAbort.Enabled = camera.Snapping || camera.Acquiring || camera.Grabbing;


            if (camera.Acquiring) { bttnCameraSnap.Text = " Aquiring"; }
            else if (camera.Snapping) { bttnCameraSnap.Text = " Snapping"; }
            else { bttnCameraSnap.Text = " Press To Snap"; }

            if (camera.Grabbing) { bttnGrab.Text = "Grabbing"; }
            else { bttnGrab.Text = "Press To Grab"; }

            if (camera.SaveImageSelected)
            {
                if (camera.LimitReached) { bttnCameraLog.Text = "Log Images - Active/Full"; }
                else { bttnCameraLog.Text = "Log Images - Active"; }

            }
            else if(!camera.SaveImageSelected)
            {
                if (camera.LimitReached) { bttnCameraLog.Text = "Log Images - Full"; }
                else { bttnCameraLog.Text = "Log Images"; }
            }
        }
        private void CameraControl_Load(object sender, EventArgs e)
        {
            pollingTimer = new System.Windows.Forms.Timer();
            pollingTimer.Tick += new EventHandler(pollingTimer_Tick);
            pollingTimer.Interval = 200; // in miliseconds
            pollingTimer.Start();

            lbCameraName.Text = camera.Name;
            lbCameraDescription.Text = camera.Description;

            bttnCameraAbort.Enabled = !camera.ArchiveImageActive;
            bttnCameraLog.Enabled = !camera.ArchiveImageActive;

            numToolSelect.Maximum = _form.toolCount;

        }
        private void bttnCameraSnap_Click(object sender, EventArgs e)
        {
            lbAcqTime.Text = $"Aquisition: --- ms";
            camera.Trigger = true;
            UpdateButton();
        }
        private void bttnCameraAbort_Click(object sender, EventArgs e)
        {
            camera.TriggerGrab = false;
            camera.Trigger = false;
            _form.CameraAbort(camera.Id);
            
        }
        private void bttnCameraLog_Click(object sender, EventArgs e)
        {
            if (camera.SaveImageSelected)
            {
                camera.SaveImageSelected = false;
                if (camera.LimitReached) { bttnCameraLog.Text = "Log Images - Full"; }
                else { bttnCameraLog.Text = "Log Images"; }
                
            }
            else if (!camera.SaveImageSelected)
            {
                camera.SaveImageSelected = true;
                if (camera.LimitReached) { bttnCameraLog.Text = "Log Images - Active/Full"; }
                else { bttnCameraLog.Text = "Log Images - Active"; }
            }
        }
        private delegate void Set_UpdateDisplay();
        public void UpdateToolDisplay()
        {            
            lbToolName.Text = tool.Name;
            lbAcqTime.Text = $"Aquisition: {camera.AcqTime} ms";
            lbToolRunTime.Text = $"Tool Time: {tool.RunStatus.TotalTime} ms";
            cbToolPassed.Checked = tool.Result;
            cbResultsUpdated.Checked = tool.ResultUpdated;

            //
            // Update all Input data that is able to be displayed
            //
            lbToolInput.Items.Clear();
            lbToolInput.BeginUpdate();
            
            for (int i = 1; i < tool.cogToolBlock.Inputs.Count; i++)
            {
                if (tool.cogToolBlock.Inputs[i] != null && tool.cogToolBlock.Inputs[i].ValueType.Name == "Double")
                {
                    string toolInput = tool.cogToolBlock.Inputs[i].Name + ": " + Math.Round(Convert.ToDouble(tool.cogToolBlock.Inputs[i].Value), 2).ToString();
                    lbToolInput.Items.Add(toolInput);
                }
            }
            
            lbToolInput.EndUpdate();
            lbToolInput.Height = lbToolInput.PreferredHeight;

            //
            // Update all output data that is able to be displayed
            //
            lbToolData.Items.Clear();
            lbToolData.BeginUpdate();
            lbToolData.Location = new Point(lbToolData.Location.X,lbToolInput.Location.Y + lbToolInput.Height + 5);
            for (int i = 0; i < tool.ToolOutput.Count; i++)
            {
                if (tool.ToolOutput[i] != null && (tool.ToolOutput[i].ValueType.Name == "Double" || tool.ToolOutput[i].ValueType.Name == "Int32")) 
                {
                    string tooldata =   tool.ToolOutput[i].Name + ": " + 
                                        Math.Round(Convert.ToDouble(tool.ToolOutput[i].Value), 2).ToString(); 
                    lbToolData.Items.Add(tooldata);
                }
            }
            lbToolData.EndUpdate();
            lbToolData.Height = lbToolData.PreferredHeight;

            //
            // Display message if tool failed
            //

            if(tool.RunStatus.Result != CogToolResultConstants.Accept || preProcess.RunStatus.Result != CogToolResultConstants.Accept)
            {                
                if(!toolFailedDisplay.Visible)
                {
                    toolFailedDisplay.ShowDialog();
                }
                else
                {
                    toolFailedDisplay.UpdateResultData();
                }
                

            }

            UpdateImageRecord();
            updateDisplayRequest = false;

        }
        public void UpdateImageRecord()
        {
            if(tool.cogToolBlock !=null)
            {
                record = tool.cogToolBlock.CreateLastRunRecord();
            }
            
            //Determine last record to display
            if (record != null)
            {
                numRecordSelect.Minimum = 0;
                numRecordSelect.Maximum = record.SubRecords.Count - 1;

                int selectedRecord = Convert.ToInt32(numRecordSelect.Value);
                recordDisplay.Record = record.SubRecords[selectedRecord];
                lbRecordName.Text = record.SubRecords[selectedRecord].Annotation;


            }
        }
        private void bttnCameraSnap_MouseUp(object sender, MouseEventArgs e)
        {
                camera.Trigger = false;
        }
        private void CameraControl_Resize(object sender, EventArgs e)
        {
            UpdateImage();
        }
        
        private delegate void Set_UpdateImage();
        public void UpdateImage()
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Set_UpdateImage(UpdateImage));
                return;
            }

            double zoomRequired = 0;
            double zoomWidth;
            double zoomHeight;
            int reqHeight;
            int reqWidth;
           
            image = camera.Image;

            if (image == null) 
            { 
                return; 
            }

            recordDisplay.Image = image;
            //*********************************************
            //determine the zoom factor to use full window
            //update cogdisplay
            //*********************************************
            int cogWidth = plControl.Location.X - recordDisplay.Location.X - 5;
            int cogHeight = this.Size.Height - 10;

            //Do not try to resize if App is Minimized
            if (_form.WindowState == FormWindowState.Minimized) { return; }
            
            //determine the correct zoom factor to fill Window
            zoomWidth = Convert.ToDouble(cogWidth) / Convert.ToDouble(image.Width);
            zoomHeight = Convert.ToDouble(cogHeight) / Convert.ToDouble(image.Height);
            if (zoomWidth < zoomHeight) { zoomRequired = zoomWidth; }
            else{ zoomRequired = zoomHeight; }

            //Update Display Width and Height - if needed
            reqWidth = Convert.ToInt16(Convert.ToDouble(image.Width) * zoomRequired);
            reqHeight = Convert.ToInt16(Convert.ToDouble(image.Height) * zoomRequired);
            if(recordDisplay.Width != reqWidth && recordDisplay.Height != reqHeight)
            {
                recordDisplay.Width = Convert.ToInt16(Convert.ToDouble(image.Width) * zoomRequired);
                recordDisplay.Height = Convert.ToInt16(Convert.ToDouble(image.Height) * zoomRequired);
            }
            // Fit Image to Display
            recordDisplay.Fit();
        }

        public void EnableCameraControl()
        {
            bttnCameraSnap.Enabled = true;
            bttnCameraAbort.Enabled = true;
        }
        public void DisableCameraControl()
        {
            bttnCameraSnap.Enabled = false;
            bttnCameraAbort.Enabled = false;
        }

        private void numToolSelect_ValueChanged(object sender, EventArgs e)
        {
            toolSelect = Convert.ToInt32(numToolSelect.Value);
        }

        private void bttnGrab_Click(object sender, EventArgs e)
        {
            camera.TriggerGrab = true;
            UpdateButton();
        }

        
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,string key, string def, StringBuilder retVal,int size, string filePath);
        private void numRecordSelect_ValueChanged(object sender, EventArgs e)
        {
            
            UpdateImageRecord();

        }

        private void bttnGetToolData_Click(object sender, EventArgs e)
        {
            toolFailedDisplay.ShowDialog();
        }
    }
}

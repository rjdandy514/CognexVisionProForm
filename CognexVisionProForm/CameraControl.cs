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
using Cognex.VisionPro.Implementation;
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
                BeginInvoke(new Set_UpdateImage(UpdateImage));
                BeginInvoke(new Set_UpdateDisplay(UpdateToolDisplay));

            }
        }
        public bool AutoDisplay
        {
            get;set;
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

            if (plToolData.Visible) { bttnGetToolData.Text = "Hide Tool Data"; }
            else { bttnGetToolData.Text = "Show Tool Data"; }

            if (tool == null)
            {
                numRecordSelect.Visible = false;
                bttnGetToolData.Visible = false;
            }
            else
            {
                numRecordSelect.Visible = true;
                bttnGetToolData.Visible = true;
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
            _form.CameraAbort();
            
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
        private delegate void Set_UpdateImage();
        public void UpdateToolDisplay()
        {
            lbToolName.Text = tool.Name;
            lbAcqTime.Text = $"Aquisition: {camera.AcqTime} ms";
            lbToolRunTime.Text = $"Tool Time: {tool.RunStatus.TotalTime} ms";
            cbToolPassed.Checked = tool.Result;
            cbResultsUpdated.Checked = tool.ResultUpdated;
            UpdateImageRecord();

            // Display message if tool failed
            if ((!tool.Result || (preProcess.ToolReady && !preProcess.Result)) && WindowState != FormWindowState.Minimized && toolFailedDisplay.AutoDisplayShow)
            {
                resizeToolData();

                if (toolFailedDisplay.Loaded)
                {
                    Utilities.LoadForm(plToolData, toolFailedDisplay);
                    plToolData.Visible = true;
                }
                else { toolFailedDisplay.UpdateResultData(); }
            }

            updateDisplayRequest = false;
        }
        public void UpdateImageRecord()
        {
            if(tool.toolBlock !=null)
            {
                record = tool.toolBlock.CreateLastRunRecord();
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
        public void UpdateImage()
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Set_UpdateImage(UpdateImage));
                return;
            }

            image = camera.Image;
            if (image == null) { return; }
            //Do not try to resize if App is Minimized
            if (_form.WindowState == FormWindowState.Minimized) { return; }

            recordDisplay.Image = image;
            //*********************************************
            //update cogdisplay
            //*********************************************
            int cogWidth = plControl.Location.X - recordDisplay.Location.X - 5;
            int cogHeight = this.Size.Height - 10;

            recordDisplay.Width = cogWidth;
            recordDisplay.Height = cogHeight;

            // Fit Image to Display
            recordDisplay.Fit();
        }
        private void bttnCameraSnap_MouseUp(object sender, MouseEventArgs e)
        {
                camera.Trigger = false;
        }
        private void CameraControl_Resize(object sender, EventArgs e)
        {
            plToolData.Visible = false;
            UpdateImage();
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
            try
            {
                if (!plToolData.Visible && tool.ToolReady)
                {
                    Utilities.LoadForm(plToolData, toolFailedDisplay);
                    resizeToolData();
                    toolFailedDisplay.UpdateResultData();
                }
                plToolData.Visible = !plToolData.Visible;
            }
            catch 
            { 
            }
            


        }

        private void bttnTest_Click(object sender, EventArgs e)
        {

            _form.RetryToolBlock();
            //camera.Abort();
        }

        private void resizeToolData()
        {
            plToolData.Left = 5;
            plToolData.Top = 5;
            plToolData.Width = this.plControl.Left - 5;
            plToolData.Height = this.Height - 5;
            
        }
    }
}

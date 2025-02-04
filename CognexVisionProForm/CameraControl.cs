using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        public ICogRecord record;
        private int toolSelect = 0;
        private bool updateDisplayRequest;
        private bool Result_Update_Mem;
        private System.Windows.Forms.Timer pollingTimer;
        ShowResultData toolFailedDisplay;

        public DalsaImage Camera
        {
            get { return camera; }
        }
        public ToolBlock Tool
        {
            get { return tool; }

            set { tool = value; }
        }
        public ToolBlock PreProcess
        {
            get { return preProcess; }
            set { preProcess = value; }
        }
        public int ToolSelect
        {
            get { return toolSelect; }
            set { toolSelect = value; }
        }
        public bool AutoDisplay
        {
            get; set;
        }
        public bool PauseTimer
        {
            set
            {
                if (pollingTimer == null) { return; }
                if (value)
                {

                    pollingTimer.Stop();
                }
                else { pollingTimer.Start(); }
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
        private void CameraControl_Resize(object sender, EventArgs e)
        {
            plToolData.Visible = false;
            UpdateImage();
        }
        private void pollingTimer_Tick(object sender, EventArgs e)
        {
            pollingTimer.Stop();

            this.BeginInvoke((Action)delegate { UpdateAll(); });

            pollingTimer.Start();
        }
        
        private void bttnCameraSnap_MouseUp(object sender, MouseEventArgs e)
        {
            camera.Trigger = false;
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
                camera.SaveImageBMP();
                if (camera.LimitReached) { bttnCameraLog.Text = "Log Images - Active/Full"; }
                else { bttnCameraLog.Text = "Log Images - Active"; }
            }
        }
        private void numRecordSelect_ValueChanged(object sender, EventArgs e)
        {
            if (!_form.SystemIdle) { return; }
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
            //_form.ToolBlockReload();
            _form.RetryToolBlock();

        }
        private void numToolSelect_ValueChanged(object sender, EventArgs e)
        {
            toolSelect = Convert.ToInt32(numToolSelect.Value);
            _form.ToolNumberUpdate(camera.Id);
        }
        private void bttnGrab_Click(object sender, EventArgs e)
        {
            camera.TriggerGrab = true;
            UpdateButton();
        }
        public void UpdateAll()
        {
            cbCameraConnected.Checked = camera.Connected;
            cbTrigger.Checked = camera.Trigger;
            cbArchiveImageActive.Checked = camera.ArchiveImageActive;
            cbImageReady.Checked = camera.ImageReady;
            numToolSelect.Value = toolSelect;
            UpdateButton();

            if (tool.ResultUpdated != Result_Update_Mem && !_form.ThreadsAlive)
            {
                this.ActiveControl = null;
                UpdateToolDisplay();
                UpdateImageRecord();
                Result_Update_Mem = tool.ResultUpdated;
            }
        }
        public void UpdateToolDisplay()
        {
            lbToolName.Text = tool.Name;
            lbAcqTime.Text = $"Aquisition: {Math.Round(camera.AcqTime, 2)} ms";
            lbToolRunTime.Text = $"Tool Time: {Math.Round(tool.RunStatus.TotalTime, 2)} ms";
            cbToolPassed.Checked = tool.Result;
            cbResultsUpdated.Checked = tool.ResultUpdated;

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
            recordDisplay.Enabled = false;

            if (tool.toolBlock != null)
            {
                try { record = tool.toolBlock.CreateLastRunRecord(); }
                catch (Exception e) { Debug.WriteLine(e); }
            }
            
            //Determine last record to display
            if (record != null)
            {
                numRecordSelect.Minimum = 0;
                numRecordSelect.Maximum = record.SubRecords.Count - 1;
                if (numRecordSelect.Value > numRecordSelect.Maximum) { numRecordSelect.Value = 0; }

                try
                {
                    int selectedRecord = Convert.ToInt32(numRecordSelect.Value);
                    recordDisplay.Record = CogSerializer.DeepCopyObject(record.SubRecords[selectedRecord]) as ICogRecord;
                    recordDisplay.Fit();
                    lbRecordName.Text = record.SubRecords[selectedRecord].Annotation;
                }
                catch (Exception e) { Debug.WriteLine(e); }
                

            }
            record = null;
            recordDisplay.Enabled = true;
        }
        public void UpdateImage()
        {
            //Do not try to resize if App is Minimized
            if (_form.WindowState == FormWindowState.Minimized) { return; }
            //*********************************************
            //update cogdisplay
            //*********************************************
            int cogWidth = plControl.Location.X - recordDisplay.Location.X - 5;
            int cogHeight = this.Size.Height - 10;

            recordDisplay.Width = cogWidth;
            recordDisplay.Height = cogHeight;

            // Fit Image to Display
            if(recordDisplay.Created && recordDisplay.Image!=null)
            {
                recordDisplay.Fit();
            }
            
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

            bttnCameraLog.Enabled = !camera.ArchiveImageActive;
            if (camera.SaveImageSelected)
            {
                if (camera.LimitReached) { bttnCameraLog.Text = "Log Images - Active/Full"; }
                else { bttnCameraLog.Text = "Log Images - Active"; }

            }
            else if (!camera.SaveImageSelected)
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
        private void resizeToolData()
        {
            plToolData.Left = 5;
            plToolData.Top = 5;
            plToolData.Width = this.plControl.Left - 5;
            plToolData.Height = this.Height - 5;
            
        }

    }
}

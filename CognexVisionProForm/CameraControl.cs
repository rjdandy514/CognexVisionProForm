﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        bool continousSnap = false;
        double acqTime = 0;
        ToolBlock tool;
        public ICogImage image;
        public ICogRecord record;
        private int toolSelect = 0;
        private System.Windows.Forms.Timer pollingTimer;

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
        public CameraControl(CognexVisionProForm Sender, DalsaImage Camera)
        {
            _form = new CognexVisionProForm();
            _form = Sender;
            camera = Camera;

            InitializeComponent();
        }
        private void pollingTimer_Tick(object sender, EventArgs e)
        {
            pollingTimer.Stop();

            cbCameraConnected.Checked = camera.Connected;
            cbTrigger.Checked = camera.Trigger;
            cbArchiveImageActive.Checked = camera.ArchiveImageActive;
            cbImageReady.Checked = camera.ImageReady;

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

            if(camera.ServerType == DalsaImage.ServerCategory.ServerAcq) 
            {
                if (camera.IsMaster != 0) { bttnEncoderPhase.Visible = true; }
                else { bttnEncoderPhase.Visible = false; }
            }
            else { bttnEncoderPhase.Visible = false; }


        }
        private void bttnCameraSnap_Click(object sender, EventArgs e)
        {
            toolSelect = Convert.ToInt32(numToolSelect.Value);
            camera.Trigger = true;
            lbAcqTime.Text = $"Aquisition: --- ms";
            UpdateButton();
        }
        private void bttnCameraAbort_Click(object sender, EventArgs e)
        {
            camera.TriggerGrab = false;
            _form.CameraAbort(camera.Id);
            camera.Abort();
            
        }
        private void bttnCameraLog_Click(object sender, EventArgs e)
        {
            if (camera.SaveImageSelected)
            {
                camera.SaveImageSelected = false;
                bttnCameraLog.Text = "Log Images";
            }
            else if (!camera.SaveImageSelected)
            {
                camera.SaveImageSelected = true;
                bttnCameraLog.Text = "Log Images - Active";
            }
        }
        private delegate void Set_UpdateDisplay();
        public void UpdateToolDisplay()
        {
            
            if (this.InvokeRequired)
            {
                BeginInvoke(new Set_UpdateDisplay(UpdateToolDisplay));
                return;
            }

            numToolSelect.Value = toolSelect;
            lbToolName.Text = tool.Name;
            lbAcqTime.Text = $"Aquisition: {camera.AcqTime} ms";
            lbToolRunTime.Text = $"Tool Time: {tool.RunStatus.TotalTime} ms";
            cbToolPassed.Checked = tool.Result;
            lbToolData.Items.Clear();
            lbToolData.BeginUpdate();
            for (int i = 0; i < tool.ToolOutput.Length; i++)
            {
                if (tool.ToolOutput[i] != null) 
                {
                    string tooldata =   tool.ToolOutput[i].Name + ": " + 
                                        Math.Round(Convert.ToDouble(tool.ToolOutput[i].Value), 2).ToString(); 
                    lbToolData.Items.Add(tooldata);
                }
            }
            lbToolData.EndUpdate();
            lbToolData.Height = lbToolData.PreferredHeight;
            
            UpdateImageRecord();

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

        private void bttnEncoderPhase_Click(object sender, EventArgs e)
        {
            int encoderPhase = camera.EncoderPhase();

            bttnEncoderPhase.Text = $"Current Phase - {encoderPhase}";
        }

        private void numToolSelect_ValueChanged(object sender, EventArgs e)
        {

        }

        private void bttnGrab_Click(object sender, EventArgs e)
        {
            camera.TriggerGrab = true;
            UpdateButton();
        }
    }
}

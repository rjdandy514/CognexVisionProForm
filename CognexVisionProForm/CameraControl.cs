using System;
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
        public ICogImage image;
        public ICogRecord record;
        private int toolSelect = 0;
        private System.Windows.Forms.Timer pollingTimer;
        public ICogImage Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }
        public double AcqTime
        {
            get { return acqTime; }
            set
            {
                acqTime = value;
                //AcqTimeUpdate();
            }
        }
        public ToolBlock Tool
        {
            get;set;
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
            cbTriggerAck.Checked = camera.TriggerAck;
            cbAbortTriggeAck.Checked = camera.AbortTriggerAck;
            cbArchiveImageActive.Checked = camera.ArchiveImageActive;
            cbImageReady.Checked = camera.ImageReady;

            if (!camera.Grabbing && !_form.PlcCommsActive)
            {
                bttnCameraSnap.Text = " Press To Snap";
                bttnCameraSnap.Enabled = true;
            }

            pollingTimer.Start();
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
        }
        private void bttnCameraSnap_Click(object sender, EventArgs e)
        {
            toolSelect = Convert.ToInt32(numToolSelect.Value);
            camera.Trigger = true;
            bttnCameraSnap.Enabled = false;
            lbAcqTime.Text = $"Aquisition: --- ms";
            bttnCameraSnap.Text = "Grabbing";
        }
        private void bttnCameraAbort_Click(object sender, EventArgs e)
        {
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
        public void UpdateDisplay()
        {
            
            if (this.InvokeRequired)
            {
                BeginInvoke(new Set_UpdateDisplay(UpdateDisplay));
                return;
            }

            numToolSelect.Value = toolSelect;
            lbAcqTime.Text = $"Aquisition: {AcqTime} ms";
            lbToolName.Text = Tool.Name;
            lbToolRunTime.Text = $"Tool Time: {Tool.TotalTime} ms";
            cbToolPassed.Checked = Tool.Result;
            lbToolData.Items.Clear();
            lbToolData.BeginUpdate();
            for (int i = 0; i < Tool.ToolOutput.Length; i++)
            {
                if (Tool.ToolOutput[i] != null) 
                {
                    string tooldata =   Tool.ToolOutput[i].Name + ": " + 
                                        Math.Round(Convert.ToDouble(Tool.ToolOutput[i].Value), 2).ToString(); 
                    lbToolData.Items.Add(tooldata);
                }
            }
            lbToolData.EndUpdate();
            lbToolData.Height = lbToolData.PreferredHeight;
            
            UpdateImage();

        }
        private delegate void Set_ResizeWindow();
        public void UpdateImage()
        {
            if(Tool.cogToolBlock !=null)
            {
                record = Tool.cogToolBlock.CreateLastRunRecord();
            }
            
            //Determine last record to display
            if (record != null && Tool.Result)
            {
                int lastRecordIndex = Math.Max(record.SubRecords.Count - 1, 0);
                cogRecordDisplay.Record = record.SubRecords[lastRecordIndex];
            }
            cogRecordDisplay.Image = image;

            ResizeWindow();
        }
        private void bttnCameraSnap_MouseUp(object sender, MouseEventArgs e)
        {
                camera.Trigger = false;
        }
        private void CameraControl_Resize(object sender, EventArgs e)
        {
            ResizeWindow();
        }

        public void ResizeWindow()
        {
            if (image == null) { return; }
            //*********************************************
            //determine the zoom factor to use full window
            //update cogdisplay
            //*********************************************
            int cogWidth = plControl.Location.X - cogRecordDisplay.Location.X - 5;
            int cogHeight = this.Size.Height - 10;

            //Do not try to resize if App is Minimized
            if (_form.WindowState == FormWindowState.Minimized) { return; }
            
            //Do not resize if Display is already correct size
            if (cogRecordDisplay.Width == cogWidth || cogRecordDisplay.Height == cogHeight) { return; }

            //determine the correct zoom factor to fill Window
            double zoomWidth = Convert.ToDouble(cogWidth) / Convert.ToDouble(image.Width);
            double zoomHeight = Convert.ToDouble(cogHeight) / Convert.ToDouble(image.Height);
            if (zoomWidth < zoomHeight) { cogRecordDisplay.Zoom = zoomWidth; }
            else{ cogRecordDisplay.Zoom = zoomHeight; }

            //Update Display Width and Height
            cogRecordDisplay.Width = Convert.ToInt16(Convert.ToDouble(image.Width) * cogRecordDisplay.Zoom);
            cogRecordDisplay.Height = Convert.ToInt16(Convert.ToDouble(image.Height) * cogRecordDisplay.Zoom);
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

        private void CameraControl_Shown(object sender, EventArgs e)
        {

        }
    }
}

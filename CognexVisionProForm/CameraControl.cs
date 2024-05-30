using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        double acqTime;
        public ICogImage image;
        public ICogRecord record;
        private Timer pollingTimer;
        public ICogImage Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                ResizeWindow();
            }
        }
        public double AcqTime
        {
            get { return acqTime; }
            set
            {
                acqTime = value;
                AcqTimeUpdate();
            }
        }
        public CogToolBlockTerminal[] ToolData
        {
            set
            {
                lbToolData.Items.Clear();
                lbToolData.BeginUpdate();
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] != null) { lbToolData.Items.Add(value[i].Value.ToString()); }
                    
                }
                lbToolData.EndUpdate();
                lbToolData.Height = lbToolData.PreferredHeight;
            }
        }
        public string ToolName
        {
            get;set;
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

            lbToolName.Text = ToolName;
            cbCameraConnected.Checked = camera.Connected;
            cbTriggerAck.Checked = camera.TriggerAck;
            cbAbortTriggeAck.Checked = camera.AbortTriggerAck;
            cbArchiveImageActive.Checked = camera.ArchiveImageActive;
            cbImageReady.Checked = camera.ImageReady;

            if(camera.TriggerAck)
            {
                bttnCameraSnap.Text = "Single Snap";
            }
            
            pollingTimer.Start();
        }
        private void CameraControl_Load(object sender, EventArgs e)
        {
            pollingTimer = new Timer();
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

            if(camera.Trigger == true)
            {
                camera.Trigger = false;
                bttnCameraSnap.Text = "Single Snap";
            }
            else
            {
                camera.Trigger = true;
                bttnCameraSnap.Text = "Triggering";
            }
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
        private delegate void Set_ResizeWindow();
        public void ResizeWindow()
        {
            //Determine last record to display
            if (record != null)
            {
                int lastRecordIndex = Math.Max(record.SubRecords.Count - 1, 0);
                cogRecordDisplay1.Record = record.SubRecords[lastRecordIndex];
            }

            //*********************************************
            //determine the zoom factor to use full window
            //update cogdisplay
            //*********************************************
            int cogWidth = plControl.Location.X - cogRecordDisplay1.Location.X - 5;
            int cogHeight = this.Size.Height - 5;

            double zoomWidth = Convert.ToDouble(cogWidth) / Convert.ToDouble(image.Width);
            double zoomHeight = Convert.ToDouble(cogHeight) / Convert.ToDouble(image.Height);
            
            

            if (zoomWidth < zoomHeight)
            {
                cogRecordDisplay1.Zoom = zoomWidth;
            }
            else if (zoomHeight < zoomWidth)
            {
                cogRecordDisplay1.Zoom = zoomHeight;
            }

            if (this.InvokeRequired)
            {
                BeginInvoke(new Set_ResizeWindow(ResizeWindow));
                return;
            }

            cogRecordDisplay1.Width = Convert.ToInt16(Convert.ToDouble(image.Width) * cogRecordDisplay1.Zoom);
            cogRecordDisplay1.Height = Convert.ToInt16(Convert.ToDouble(image.Height) * cogRecordDisplay1.Zoom);
            cogRecordDisplay1.Image = image;

            
            
            



        }
        private delegate void Set_AcqTimeUpdate();
        public void AcqTimeUpdate()
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Set_AcqTimeUpdate(AcqTimeUpdate));
                return;
            }
            lbAcqTime.Text = $"Aquisition: {AcqTime}ms";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

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

namespace CognexVisionProForm
{
    public partial class CameraControl : Form
    {
        Form1 _form;
        DalsaImage camera;
        ICogImage image;
        public ToolBlock[] cogTool;
        double acqTime;
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
        public CameraControl(Form1 Sender, DalsaImage Camera)
        {
            _form = new Form1();
            _form = Sender;
            camera = Camera;

            InitializeComponent();
        }
        private void pollingTimer_Tick(object sender, EventArgs e)
        {
            pollingTimer.Stop();

            cbCameraConnected.Checked = camera.Connected;
            cbArchiveImageActive.Checked = camera.ArchiveImageActive;
            cbImageReady.Checked = camera.ImageReady;

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

            bttnCameraAbort.Visible = !camera.ArchiveImageActive;
            bttnCameraLog.Visible = !camera.ArchiveImageActive;

        }
        private void bttnCameraSnap_Click(object sender, EventArgs e)
        {
            //_form.CameraTrigger(camera.Id);

            if ( camera.Connected)
            {
                camera.SnapPicture();
            }
            else if (camera.ArchiveImageActive)
            {
                camera.CreateBufferFromFile();
                camera.ArchiveImageIndex++;
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
            //*********************************************
            //determine the zoom factor to use full window
            //update cogdisplay
            //*********************************************
            int cogWidth = plControl.Location.X - 5;
            int cogHeight = this.Size.Height - 5;

            double zoomWidth = Convert.ToDouble(cogWidth) / Convert.ToDouble(image.Width);
            double zoomHeight = Convert.ToDouble(cogHeight) / Convert.ToDouble(image.Height);

            if (zoomWidth < zoomHeight)
            {
                cogImageDisplay.Zoom = zoomWidth;
            }
            else if (zoomHeight < zoomWidth)
            {
                cogImageDisplay.Zoom = zoomHeight;
            }

            if (this.InvokeRequired)
            {
                BeginInvoke(new Set_ResizeWindow(ResizeWindow));
                return;
            }

            cogImageDisplay.Width = Convert.ToInt16(Convert.ToDouble(image.Width) * cogImageDisplay.Zoom);
            cogImageDisplay.Height = Convert.ToInt16(Convert.ToDouble(image.Height) * cogImageDisplay.Zoom);
            cogImageDisplay.Image = image;

            
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

    }
}

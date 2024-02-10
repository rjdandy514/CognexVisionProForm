using Cognex.VisionPro;
using Cognex.VisionPro.Exceptions;
using Cognex.VisionPro.ImageFile;
using DALSA.SaperaProcessing.CPro;
using DALSA.SaperaLT.SapClassBasic;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using EventArgs = System.EventArgs;
using static System.Net.Mime.MediaTypeNames;
using Cognex.VisionPro.ToolBlock;
using LibplctagWrapper;
using System.Net.NetworkInformation;
using System.Collections;
using static DalsaImage;
using System.IO;

namespace CognexVisionProForm
{
    public partial class Form1 : Form
    {
        private bool heartBeat = false;
        private bool CommsUp = false;

        private CogStringCollection LicenseCheck;
        private bool cogLicenseOk;

        private DalsaImage Camera01Acq;

        private string ToolBlockLocation;
        private int ExpireCount = 0;

        private bool ExpireError = false;
        private ToolBlock Camera01Tb01;
        private ToolBlock Camera01Tb02;

        private Calculations Camera01Calc;

        private PlcComms MainPLC;

        public TextWriterTraceListener appListener;

        private double FF_Radians;
        private double FF_L;

        private double FC_Radians;
        private double FC_L;

        double Part_X;
        double Part_Y;
        double Part_XNom;
        double Part_YNom;
        double Part_Radians;

        double Robot_X;
        double Robot_Y;
        double Robot_Degree;

        string exeFileExtension;
        string exeFileName;
        string exeFilePath;

        string C1Tb1Name;
        string ServerNotFound = "No Server Found";
        string m_ServerName;
        int m_ResourceIndex;

        private Timer pollingTimer;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void pollingTimer_Tick(object sender, EventArgs e)
        {
            if (heartBeat) { heartBeat = false; }
            else { heartBeat = true; }

            cbHeartbeat.Checked = heartBeat;

            BitArray CameraStatus = new BitArray(32, false);
            BitArray ToolStatus01 = new BitArray(32, false);

            //Camera Status: info related to camera and general system
            CameraStatus[0] = heartBeat;
            CameraStatus[1] = Camera01Acq.Connected;
            CameraStatus[2] = cogLicenseOk;
            CameraStatus[3] = false;
            CameraStatus[4] = false;
            CameraStatus[5] = false;
            CameraStatus[6] = false;
            CameraStatus.CopyTo(MainPLC.PCtoPLC_Data,0);

            //Tool Status: info related to Camera Tool 1
            ToolStatus01[0] = Camera01Tb01.ToolReady;
            ToolStatus01[1] = Camera01Tb01.ResultUpdated;
            ToolStatus01[2] = Convert.ToBoolean(Camera01Tb01.RunStatus.Result);
            ToolStatus01[3] = false;
            ToolStatus01[4] = false;
            ToolStatus01[5] = false;
            ToolStatus01[6] = false;
            ToolStatus01.CopyTo(MainPLC.PCtoPLC_Data, 1);


            //All data variables
            MainPLC.PCtoPLC_Data[8] = Convert.ToInt32(Camera01Acq.AcqTime*100);
            MainPLC.PCtoPLC_Data[9] = Convert.ToInt32(Camera01Tb01.RunStatus.TotalTime*100);
            MainPLC.PCtoPLC_Data[10] = Convert.ToInt32(Camera01Tb02.RunStatus.TotalTime * 100);


            if (CommsUp)
            {
                if (MainPLC.IPAddress == "10.2.4.10")
                {
                    MainPLC.ReadPlcTag();
                    MainPLC.WritePlcTag();
                }
            }
            
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Eclipse Vision Application";

            exeFileExtension = Path.GetExtension(System.Windows.Forms.Application.ExecutablePath);
            exeFileName = Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath);
            exeFilePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            
            InitializeLogging();
            InitializeAcquisition();
            LoadSettings();
            InitializeServerList();
            InitializeResourceList();

            CheckLicense();
            if(cogLicenseOk)
            {
                Camera01Tb01.InitializeJobManager();
                Camera01Tb02.InitializeJobManager();
            }
            else
            {
                MessageBox.Show("Cognex VisionPro License did not load properly");
                tabControl1.SelectedIndex = 2;
            }

            cbConfigFileFound.Checked = Camera01Acq.ConfigFilePresent;
            cbC1Tb1FileFound.Checked = Camera01Tb01.ToolFilePresent;

        }
        private void btnLicenseCheck_Click(object sender, EventArgs e)
        {
            CheckLicense();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Camera01Tb01.Cleaning();
            Camera01Acq.Cleaning();
            MainPLC.Cleaning();
            cogToolBlockEditV21.Dispose();
            pollingTimer.Dispose();
            appListener.Close();
            appListener.Dispose();

            SaveSettings();
        }
        private void bttnArchive_Click(object sender, EventArgs e)
        {
            Process.Start(txtArchive.Text);
        }
        private void bttnLog_Click(object sender, EventArgs e)
        {
            Process.Start(txtLogFile.Text);
        }
        private void bttnC1TB1FileSelect_Click(object sender, EventArgs e)
        {

            Camera01Tb01.LoadvisionProject();
            cbC1Tb1FileFound.Checked = Camera01Tb01.ToolFilePresent;
        }
        private void btnPartCalc_Click(object sender, EventArgs e)
        {
            double temp_FC_XDistance = 214.5;
            double temp_FC_YDistance = 20;
            double temp_FF_XDistance = 415.5;
            double temp_FF_YDistance = 38.7;

            double temp_F1_XPos = 0.0;
            double temp_F1_YPos = 0.0;
            double temp_F2_XPos = 360.216;
            double temp_F2_YPos = 210.672;

            F1_XPosition.Text = temp_F1_XPos.ToString();
            F1_YPosition.Text = temp_F1_YPos.ToString();
            F2_XPosition.Text = temp_F2_XPos.ToString();
            F2_YPosition.Text = temp_F2_YPos.ToString();

            FC_XDistance.Text = temp_FC_XDistance.ToString();
            FC_YDistance.Text = temp_FC_YDistance.ToString();
            FF_XDistance.Text = temp_FF_XDistance.ToString();
            FF_YDistance.Text = temp_FF_YDistance.ToString();

            double FC_X = Convert.ToDouble(FC_XDistance.Text);
            double FC_Y = Convert.ToDouble(FC_YDistance.Text);

            double FF_X = Convert.ToDouble(FF_XDistance.Text);
            double FF_Y = Convert.ToDouble(FF_YDistance.Text);

            FC_Radians = Math.Atan2(FC_Y,FC_X);
            FF_Radians = Math.Atan2(FF_Y, FF_X);

            FC_L = Camera01Calc.DistanceBetweenPoints(0, 0, FC_X, FC_Y);
            FF_L = Camera01Calc.DistanceBetweenPoints(0,0,FF_X,FF_Y);

            FC_Angle.Text = FC_Radians.ToString();
            FF_Angle.Text = FF_Radians.ToString();

            FC_LDistance.Text = FC_L.ToString();
            FF_LDistance.Text = FF_L.ToString();

        }
        private void btnPartLocCalc_Click(object sender, EventArgs e)
        {
            
            double temp_PartData_XNom = 214.5;
            double temp_PartData_YNom = 20;

            PartData_XNom.Text = temp_PartData_XNom.ToString();
            PartData_YNom.Text = temp_PartData_YNom.ToString();

            double F1_X = Convert.ToDouble(F1_XPosition.Text);
            double F1_Y = Convert.ToDouble(F1_YPosition.Text);
            double F2_X = Convert.ToDouble(F2_XPosition.Text);
            double F2_Y = Convert.ToDouble(F2_YPosition.Text);

            Part_Radians = Math.Atan2(F2_Y - F1_Y, F2_X - F1_X)-FF_Radians;

            Part_X = FC_L * Math.Cos(FC_Radians+ Part_Radians) + F1_X;
            Part_Y = FC_L * Math.Sin(FC_Radians + Part_Radians) + F1_Y;


            PartData_XPosition.Text = Part_X.ToString();
            PartData_YPosition.Text = Part_Y.ToString();
            PartData_Angle.Text = Part_Radians.ToString();
        }
        private void RobotCalc_Click(object sender, EventArgs e)
        {
            double Robot_XNeg;
            double Robot_YNeg;

            Part_XNom = Convert.ToDouble(PartData_XNom.Text);
            Part_YNom = Convert.ToDouble(PartData_YNom.Text);

            Robot_XNeg = -1*(Part_X - Part_XNom);
            Robot_YNeg = -1*(Part_Y - Part_YNom);

            Robot_X = Robot_XNeg * Math.Cos(-Part_Radians) - Robot_YNeg * Math.Sin(-Part_Radians);
            Robot_Y = Robot_XNeg * Math.Sin(-Part_Radians) + Robot_YNeg * Math.Cos(-Part_Radians);
            Robot_Degree = Camera01Calc.RadiansToDegree(Part_Radians);

            VGR_XOffset.Text = Robot_X.ToString();
            VGR_YOffset.Text = Robot_Y.ToString();
            VGR_DegOffset.Text = Robot_Degree.ToString();

        }
        private void cbServerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MyListBoxItem item = (MyListBoxItem)cbServerList.SelectedItem;
            bool configFileRequired = item.ItemData;
            cbConfigFileReq.Checked = configFileRequired;
            m_ServerName = item.ToString();

            InitializeResourceList();

        }
        private void bttnConnectCamera_Click(object sender, EventArgs e)
        {
            if (cbServerList.SelectedItem != null && cbDeviceList.SelectedItem != null)
            {
                Camera01Acq.CreateCamera(cbServerList.SelectedItem.ToString(), cbDeviceList.SelectedIndex);
            }
            if (Camera01Acq.Connected) { cbCameraConnected.Checked = true; }
            else
            {
                MessageBox.Show("FAILED TO CONNECT SELECTED CAMERA");
                cbCameraConnected.Checked = false;
            }
        }
        private void bttnC1Snap_Click(object sender, EventArgs e)
        {
            Camera1Trigger();
        }
        private void bttnC1Grab_Click(object sender, EventArgs e)
        {
            if (cbCameraConnected.Checked)
            {
                Camera01Acq.GrabPicture();
            }
            else
            {
                MessageBox.Show("NO CAMERA IS CONNECTED");
                tabControl1.SelectedIndex = 1;
            }
        }
        private void bttnC1Freeze_Click(object sender, EventArgs e)
        {
            if (cbCameraConnected.Checked)
            {
                Camera01Acq.Freeze();
            }
            else
            {
                MessageBox.Show("NO CAMERA IS CONNECTED");
                tabControl1.SelectedIndex = 1;
            }
        }
        private void bttnC1Abort_Click(object sender, EventArgs e)
        {
            if (cbCameraConnected.Checked)
            {
                Camera01Acq.Abort();
            }
            else
            {
                MessageBox.Show("NO CAMERA IS CONNECTED");
                tabControl1.SelectedIndex = 1;
            }
        }
        private void cbCameraSelected_DropDown(object sender, EventArgs e)
        {
            cbCameraSelected.Items.Clear();
            cbCameraSelected.Items.Add(Camera01Tb01.ToolName);
            cbCameraSelected.Items.Add(Camera01Tb02.ToolName);
        }
        private void cbCameraSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCameraSelected.SelectedItem.ToString() == Camera01Tb01.ToolName)
            {
                cogToolBlockEditV21.Subject = Camera01Tb01.cogToolBlock;
            }
            else if(cbCameraSelected.SelectedItem.ToString() == Camera01Tb02.ToolName)
            {
                cogToolBlockEditV21.Subject = Camera01Tb02.cogToolBlock;
            }
            if(cogToolBlockEditV21.Subject != null)
            {
                cogToolBlockEditV21.Subject.Run();
            }
        }
        private void bttnPLC_Click(object sender, EventArgs e)
        {
            
            MainPLC.InitializePlcComms(numIP1.Value.ToString(), numIP2.Value.ToString(), numIP3.Value.ToString(), numIP4.Value.ToString());
            
            if(MainPLC.InitialCheck.Status == IPStatus.Success)
            {
                CommsUp = true;
                pollingTimer.Start();
            }

        }
        private void bttnC1Config_Click(object sender, EventArgs e)
        {
            Camera01Acq.LoadConfigFile();
            cbConfigFileFound.Checked = Camera01Acq.ConfigFilePresent;

        }
        class MyListBoxItem
        {
            public MyListBoxItem(string Text)
            {
                Camdesc = Text;
                itemData = false;
            }

            public MyListBoxItem(string Text, bool ItemData)
            {
                Camdesc = Text;
                itemData = ItemData;
            }

            public bool ItemData
            {
                get
                {
                    return itemData;
                }
                set
                {
                    itemData = value;
                }
            }

            public override string ToString()
            {
                return Camdesc;
            }

            private string Camdesc;
            private bool itemData;
        }
        private void bttnC1LogImages_Click(object sender, EventArgs e)
        {
            if (Camera01Acq.SaveImageSelected) 
            {
                Camera01Acq.SaveImageSelected = false;
                bttnC1LogImages.Text = "Log Images";
            }
            else if(!Camera01Acq.SaveImageSelected)
            {
                Camera01Acq.SaveImageSelected = true;
                bttnC1LogImages.Text = "Log Images - Active";
            }
        }

        private void tbC1TB1Name_Leave(object sender, EventArgs e)
        {
            Camera01Tb01.ToolName = tbC1TB1Name.Text.ToString();
        }
    }

}


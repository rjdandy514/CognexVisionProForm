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
using System.Linq;
using System.Net.Http.Headers;
using System.Drawing;

namespace CognexVisionProForm
{
    public partial class Form1 : Form
    {
        private int cogHeight;
        private int cogWidth;
        private bool heartBeat = false;
        private bool CommsUp = false;

        private BitArray CameraControl;
        private BitArray CameraStatus;

        private BitArray toolControl;
        private BitArray toolStatus;
        private bool toolRunComplete;
        private int toolRunCount;
        private int toolCompleteCount;

        private CogStringCollection LicenseCheck;
        private bool cogLicenseOk;

        int selectedCameraId;
        public DalsaImage[] CameraAcqArray;
        ICogImage CameraImage;
        bool cameraSnapComplete;
        public CameraControl[] cameraControl;


        int toolBlockRunComplete;
        private int ExpireCount = 0;

        private bool ExpireError = false;

        private ToolBlock[,] toolblockArray;

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

        int cameraCount;
        int toolCount;

        string ServerNotFound = "No Server Found";

        private Timer pollingTimer;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void pollingTimer_Tick(object sender, EventArgs e)
        {
            pollingTimer.Stop();
            if (heartBeat) { heartBeat = false; }
            else { heartBeat = true; }

            cbHeartbeat.Checked = heartBeat;

            PlcRead();

            CameraAcqArray[selectedCameraId].Trigger = CameraControl[0];

            PlcWrite();


            if (CommsUp)
            {
                if (MainPLC.IPAddress == "10.2.4.10")
                {
                    MainPLC.ReadPlcTag();
                    MainPLC.WritePlcTag();
                }
            }
            pollingTimer.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Eclipse Vision Application";
           
            string LogDir = Utilities.ExeFilePath + "\\LogFile\\";
            string ArchiveLogDir = LogDir + "Archive\\";
            Utilities.InitializeLog(LogDir, ArchiveLogDir);

            cameraCount = 3;
            toolCount = 4;
            CameraAcqArray = new DalsaImage[cameraCount];
            toolblockArray = new ToolBlock[cameraCount, toolCount];
            cameraControl = new CameraControl[cameraCount];

            InitializeClasses();
            LoadSettings();
            InitializeServerList(0);
            InitializeResourceList(0);
            InitializeJobManager();

            CheckLicense();
            if(!cogLicenseOk)
            {
                MessageBox.Show("Cognex VisionPro License did not load properly");
                tabControl1.SelectedIndex = 2;
            }
        }
        private void btnLicenseCheck_Click(object sender, EventArgs e)
        {
            CheckLicense();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int j = 0; j < cameraCount; j++)
            {
                CameraAcqArray[j].Cleaning();
                
                for (int i = 0; i < toolCount; i++)
                {
                    toolblockArray[j, i].Cleaning();
                }
            }
            
            
            MainPLC.Cleaning();
            cogToolBlockEditV21.Dispose();
            pollingTimer.Dispose();
            Utilities.Closing();

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
        private void bttnToolBockFileSelect_Click(object sender, EventArgs e)
        {
            
            string toolNameUpdated = tbC1TB1Name.Text.ToString();
            int toolSelected = cbToolBlock.SelectedIndex;


            if (toolSelected >= 0 && toolSelected < toolCount)
            {
                toolblockArray[0, toolSelected].Name = toolNameUpdated;
                toolblockArray[0, toolSelected].LoadvisionProject();
                toolblockArray[0, toolSelected].InitializeJobManager();

                cbToolBlock.Items[toolSelected] = toolNameUpdated;
            }

            
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
            CameraAcqArray[selectedCameraId].LoadServerSelect = item.ToString();

            InitializeResourceList(selectedCameraId);

        }
        private void bttnConnectCamera_Click(object sender, EventArgs e)
        {
            if(!CameraAcqArray[selectedCameraId].Connected)
            {
                CameraAcqArray[selectedCameraId].LoadServerSelect = cbServerList.SelectedItem.ToString();
                CameraAcqArray[selectedCameraId].LoadResourceIndex = cbDeviceList.SelectedIndex;

                if (CameraAcqArray[selectedCameraId].LoadServerSelect != null && CameraAcqArray[selectedCameraId].LoadResourceIndex != -1)
                {
                    CameraAcqArray[selectedCameraId].CreateCamera();
                }
                if(!CameraAcqArray[selectedCameraId].Connected)
                {
                    MessageBox.Show("Failed to conenct");
                }
                
            }
            else { CameraAcqArray[selectedCameraId].Disconnect(); }

            cbCameraConnected.Checked = CameraAcqArray[selectedCameraId].Connected;
        }
        private void cbCameraSelected_DropDown(object sender, EventArgs e)
        {
            cbToolBlockSelected.Items.Clear();
            for (int i = 0; i < toolblockArray.GetLength(1); i++)
            {
                cbToolBlockSelected.Items.Add(toolblockArray[0,i].Name);
            }
        }
        private void cbCameraSelected_SelectedIndexChanged(object sender, EventArgs e)
        {

            if(toolblockArray[0,cbToolBlockSelected.SelectedIndex].ToolReady)
            {
                cogToolBlockEditV21.Subject = toolblockArray[0, cbToolBlockSelected.SelectedIndex].cogToolBlock;
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
            CameraAcqArray[selectedCameraId].LoadConfigFile();
            cbConfigFileFound.Checked = CameraAcqArray[selectedCameraId].ConfigFilePresent;

        }
        private void bttnArchiveImage_Click(object sender, EventArgs e)
        {
            if(CameraAcqArray[selectedCameraId].Connected)
            {
                CameraAcqArray[selectedCameraId].Disconnect();
                cbCameraConnected.Checked = CameraAcqArray[selectedCameraId].Connected;
            }

            if (!CameraAcqArray[selectedCameraId].ArchiveImageActive)
            {
                CameraAcqArray[selectedCameraId].FindArchivedImages();
                if (CameraAcqArray[selectedCameraId].ArchiveImageCount > 0) 
                { 
                    CameraAcqArray[selectedCameraId].ArchiveImageActive = true;
                    
                }
            }
            else { CameraAcqArray[selectedCameraId].ArchiveImageActive = false; }
            cbArchiveActive.Checked = CameraAcqArray[selectedCameraId].ArchiveImageActive;
        }
        private void cbToolBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbC1Tb1FileFound.Checked = toolblockArray[0,cbToolBlock.SelectedIndex].FilePresent;
            tbToolBlockName.Text = toolblockArray[0, cbToolBlock.SelectedIndex].Name;
        }
        private void cogToolBlockEditV21_Load(object sender, EventArgs e)
        {

        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == 0)
            {
                if (CameraAcqArray[0].Connected || CameraAcqArray[0].ArchiveImageActive)
                {
                    LoadForm(this.Camera1Panel, cameraControl[0]);

                }
                if (CameraAcqArray[1].Connected || CameraAcqArray[1].ArchiveImageActive)
                {
                    LoadForm(this.Camera2Panel, cameraControl[1]);

                }
                if (CameraAcqArray[2].Connected || CameraAcqArray[2].ArchiveImageActive)
                {
                    LoadForm(this.Camera3Panel, cameraControl[2]);
                }
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                UpdateFrameGrabberTab();
            }
            else if(tabControl1.SelectedIndex == 2)
            {

            }
            else if(tabControl1.SelectedIndex == 7)
            {
            }
        }

        private void cbCameraIdSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCameraId = cbCameraIdSelected.SelectedIndex;
            
            cbConfigFileFound.Checked = CameraAcqArray[selectedCameraId].ConfigFilePresent;
            cbCameraConnected.Checked = CameraAcqArray[selectedCameraId].Connected;
            tbCameraName.Text = CameraAcqArray[selectedCameraId].Name;
            
            tbArchiveCount.Text = CameraAcqArray[selectedCameraId].ArchiveImageCount.ToString();
            tbArchiveIndex.Text = CameraAcqArray[selectedCameraId].ArchiveImageIndex.ToString();
            cbArchiveActive.Checked = CameraAcqArray[selectedCameraId].ArchiveImageActive;

            InitializeServerList(selectedCameraId);
            InitializeResourceList(selectedCameraId);


            for (int i = 0; i < toolCount; i++)
            {
                if (toolblockArray[0, i].FilePresent) { cbToolBlock.Items.Add(toolblockArray[0, i].Name); }
                else { cbToolBlock.Items.Add($"Empty{i}"); }
            }
            cbToolBlock.SelectedIndex = 0;
        }

        private void bttnCameraNameUpdate_Click(object sender, EventArgs e)
        {
            CameraAcqArray[cbCameraIdSelected.SelectedIndex].Name = tbCameraName.Text;
        }

        private void bttnGlobalSnap_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < cameraCount;i++)
            {
                if(CameraAcqArray[i].Connected || CameraAcqArray[i].ArchiveImageActive)
                {
                    CameraTrigger(i);
                }
            }
        }

        private void tbCamersDesc_Leave(object sender, EventArgs e)
        {
            CameraAcqArray[selectedCameraId].Description = tbCamersDesc.Text;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {

            
            Camera1Panel.Height = this.tabControl1.Height / 2 - 2;
            Camera1Panel.Width = this.tabControl1.Width / 2 - 2;
            Camera1Panel.Location = new Point(0, 0);

            Camera2Panel.Height = this.tabControl1.Height / 2 - 2;
            Camera2Panel.Width = this.tabControl1.Width / 2 - 2;
            Camera2Panel.Location = new Point(tabControl1.Width - Camera2Panel.Width, 0);

            Camera3Panel.Height = this.tabControl1.Height / 2 - 2;
            Camera3Panel.Width = this.tabControl1.Width / 2 - 2;
            Camera3Panel.Location = new Point(0, tabControl1.Height - Camera2Panel.Height);

            Camera4Panel.Height = this.tabControl1.Height / 2 - 2;
            Camera4Panel.Width = this.tabControl1.Width / 2 - 2;
            Camera4Panel.Location = new Point(tabControl1.Width - Camera2Panel.Width, tabControl1.Height - Camera2Panel.Height);



        }
    }

}


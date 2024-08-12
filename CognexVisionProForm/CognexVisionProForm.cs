﻿using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using EventArgs = System.EventArgs;


namespace CognexVisionProForm
{
    public partial class CognexVisionProForm : Form
    {
        private bool heartBeat = false;
        private bool PlcAutoMode;

        private BitArray generalControl;

        private BitArray CameraControl;
        private BitArray generalStatus;

        private BitArray toolControl;
        private BitArray cameraStatus;
        private bool toolRunComplete;
        private int toolRunCount;
        private int toolCompleteCount;

        private CogStringCollection LicenseCheck;
        private bool cogLicenseOk;

        int selectedCameraId;
        int cameraConnectCount;
        int dataLength = 8;
        public DalsaImage[] CameraAcqArray;
        ICogImage CameraImage;
        bool[] cameraSnap;
        bool[] cameraSnapComplete;
        bool[] toolTrigger;
        bool[] toolTriggerComplete;
        public CameraControl[] cameraControl;

        SplashScreen splashScreen;


        int toolBlockRunComplete;
        private int ExpireCount = 0;

        private bool ExpireError = false;

        private int[] desiredTool;
        private int[] plcTool;
        private int[] hmiTool;
        private ToolBlock[,] toolblockArray;

        private PlcComms MainPLC;

        public TextWriterTraceListener appListener;

        int cameraCount;
        int toolCount;
        bool toolResultUpdate_Mem;
        string computerName;

        string ServerNotFound = "No Server Found";

        private Timer pollingTimer;


        public CognexVisionProForm()
        {
            InitializeComponent();

        }

        private void pollingTimer_Tick(object sender, EventArgs e)
        {
            pollingTimer.Stop();

            heartBeat = !heartBeat;

            cbHeartbeat.Checked = heartBeat;

            if (MainPLC.InitialCheck.Status == IPStatus.Success)
            {
                //Get all data from PLC
                MainPLC.ReadPlcTag();
                MainPLC.ReadPlcDataTag();
                PlcRead();
                
                //Send Data to PLC
                PlcWrite();
                MainPLC.WritePlcTag();
                MainPLC.WritePlcDataTag();
            }
            pollingTimer.Start();
        }
        //*********************************************************************
        //GENERAL FORM CONTROL
        //*********************************************************************
        private void Form1_Load(object sender, EventArgs e)
        {

            ComputerSetup();

            this.Text = $"{computerName} - Eclipse Vision Application";

            splashScreen = new SplashScreen();
            splashScreen.Show();splashScreen.UpdateProgress("Form Load", 10);


            string LogDir = Utilities.ExeFilePath + "\\LogFile\\";
            string ArchiveLogDir = LogDir + "Archive\\";
            Utilities.InitializeLog(LogDir, ArchiveLogDir);
            txtArchive.Text = ArchiveLogDir;
            txtLogFile.Text = LogDir + "Log.log";

            CameraAcqArray = new DalsaImage[cameraCount];
            toolblockArray = new ToolBlock[cameraCount, toolCount];
            desiredTool = new int[cameraCount];
            plcTool = new int[cameraCount];
            cameraControl = new CameraControl[cameraCount];
            cameraSnap = new bool[cameraCount];
            cameraSnapComplete = new bool[cameraCount];
            toolTrigger = new bool[cameraCount];
            toolTriggerComplete = new bool[cameraCount];

            splashScreen.UpdateProgress("Initialize Classes", 10);
            InitializeClasses();

            splashScreen.UpdateProgress("Load Settings", 10);
            LoadSettings();

            splashScreen.UpdateProgress("Initialize Server List", 10);
            InitializeServerList(0);

            splashScreen.UpdateProgress("Initialize Resource List", 10);
            InitializeResourceList(0);

            splashScreen.UpdateProgress("Initialize Job Manager", 10);
            InitializeJobManager();

            splashScreen.UpdateProgress("Check License", 10);
            CheckLicense();
            if (!cogLicenseOk)
            {
                MessageBox.Show("Cognex VisionPro License did not load properly");
                tabControl1.SelectedIndex = 2;
            }

            splashScreen.UpdateProgress("Getting Ready", 10);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            resize_Tab00();
            resize_tabToolBlock();
            resize_tabFileControl();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();

            for (int j = 0; j < cameraCount; j++)
            {
                CameraAcqArray[j].Destroy();
                CameraAcqArray[j].Dispose();

                for (int i = 0; i < toolCount; i++)
                {
                    toolblockArray[j, i].Cleaning();
                }
            }


            MainPLC.Cleaning();
            cogToolBlockEditV21.Dispose();
            pollingTimer.Dispose();
            Utilities.Closing();
            

            
        }
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {

        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (tabControl1.SelectedIndex == 0)
            {
                Panel[] allPanels = new Panel[] { Camera1Panel, Camera2Panel, Camera3Panel, Camera4Panel, Camera5Panel, Camera6Panel };
                cameraConnectCount = 0;
                for (int i = 0; i < cameraCount; i++)
                {
                    if (CameraAcqArray[i].Connected || CameraAcqArray[i].ArchiveImageActive)
                    {
                        LoadForm(allPanels[cameraConnectCount], cameraControl[i]);
                        cameraConnectCount++;
                    }
                }
                resize_Tab00();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                UpdateFrameGrabberTab();
            }
            else if (tabControl1.SelectedIndex == 5)
            {
                cbTBCameraSelected.Items.Clear();

                foreach (DalsaImage camera in CameraAcqArray)
                {
                    cbTBCameraSelected.Items.Add(camera.Name);
                }
                cbTBCameraSelected.SelectedIndex = 0;
            }
            else if (tabControl1.SelectedIndex == 6)
            {
                tbBaseTag.Text = MainPLC.BaseTag;
            }
        }
        //*********************************************************************
        //SINGLE CAMERA CONTROL
        //*********************************************************************

        //*********************************************************************
        //FRAME GRABBER
        //*********************************************************************
        private void cbCameraIdSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCameraId = cbCameraIdSelected.SelectedIndex;

            cbConfigFileFound.Checked = CameraAcqArray[selectedCameraId].ConfigFilePresent;
            cbCameraConnected.Checked = CameraAcqArray[selectedCameraId].Connected;
            tbCameraName.Text = CameraAcqArray[selectedCameraId].Name;
            tbCameraDesc.Text = CameraAcqArray[selectedCameraId].Description;

            tbArchiveCount.Text = CameraAcqArray[selectedCameraId].ArchiveImageCount.ToString();
            tbArchiveIndex.Text = CameraAcqArray[selectedCameraId].ArchiveImageIndex.ToString();
            cbArchiveActive.Checked = CameraAcqArray[selectedCameraId].ArchiveImageActive;

            if (CameraAcqArray[selectedCameraId].Connected) { bttnConnectCamera.Text = "Disconnect"; }
            else { bttnConnectCamera.Text = "Connect"; }

            InitializeServerList(selectedCameraId);
            InitializeResourceList(selectedCameraId);

            cbToolBlock.Items.Clear();
            for (int i = 0; i < toolCount; i++)
            {

                if (toolblockArray[cbCameraIdSelected.SelectedIndex, i].FilePresent) { cbToolBlock.Items.Add(toolblockArray[cbCameraIdSelected.SelectedIndex, i].Name); }
                else { cbToolBlock.Items.Add($"Empty{i}"); }
            }
            cbToolBlock.SelectedIndex = 0;
        }
        private void tbCamersDesc_Leave(object sender, EventArgs e)
        {
            CameraAcqArray[selectedCameraId].Name = tbCameraName.Text;
            CameraAcqArray[selectedCameraId].Description = tbCameraDesc.Text;
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
            if (!CameraAcqArray[selectedCameraId].Connected)
            {
                CameraAcqArray[selectedCameraId].LoadServerSelect = cbServerList.SelectedItem.ToString();
                CameraAcqArray[selectedCameraId].LoadResourceIndex = cbDeviceList.SelectedIndex;

                if (CameraAcqArray[selectedCameraId].LoadServerSelect != null && CameraAcqArray[selectedCameraId].LoadResourceIndex != -1)
                {
                    CameraAcqArray[selectedCameraId].CreateCamera();
                }
                if (!CameraAcqArray[selectedCameraId].Connected)
                {
                    MessageBox.Show("Failed to conenct");
                }

            }
            else 
            {
                CameraAcqArray[selectedCameraId].Disconnect();
                CameraAcqArray[selectedCameraId].Cleaning();
            }

            cbCameraConnected.Checked = CameraAcqArray[selectedCameraId].Connected;

            if(CameraAcqArray[selectedCameraId].Connected)
            {
                bttnConnectCamera.Text = "Disconnect";
            }
            else
            {
                bttnConnectCamera.Text = "Connect";
            }
        }
        private void cbCameraSelected_DropDown(object sender, EventArgs e)
        {
            cbToolBlock.Items.Clear();
            for (int i = 0; i < toolblockArray.GetLength(1); i++)
            {
                cbToolBlock.Items.Add(toolblockArray[cbCameraIdSelected.SelectedIndex, i].Name);
            }
        }
        private void bttnC1Config_Click(object sender, EventArgs e)
        {
            CameraAcqArray[selectedCameraId].LoadConfigFile();
            cbConfigFileFound.Checked = CameraAcqArray[selectedCameraId].ConfigFilePresent;

        }
        private void bttnArchiveImage_Click(object sender, EventArgs e)
        {
            if (CameraAcqArray[selectedCameraId].Connected)
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

            cbC1Tb1FileFound.Checked = toolblockArray[cbCameraIdSelected.SelectedIndex, cbToolBlock.SelectedIndex].FilePresent;
            tbToolBlockName.Text = toolblockArray[cbCameraIdSelected.SelectedIndex, cbToolBlock.SelectedIndex].Name;

        }
        private void bttnToolBockFileSelect_Click(object sender, EventArgs e)
        {

            string toolNameUpdated = tbC1TB1Name.Text.ToString();
            int toolSelected = cbToolBlock.SelectedIndex;


            if (toolSelected >= 0 && toolSelected < toolCount)
            {
                toolblockArray[cbCameraIdSelected.SelectedIndex, toolSelected].Name = toolNameUpdated;
                toolblockArray[cbCameraIdSelected.SelectedIndex, toolSelected].LoadvisionProject();
                toolblockArray[cbCameraIdSelected.SelectedIndex, toolSelected].InitializeJobManager();

                cbToolBlock.Items[toolSelected] = toolNameUpdated;
            }


        }
        //*********************************************************************
        //lICENSE CHECK
        //*********************************************************************
        private void btnLicenseCheck_Click(object sender, EventArgs e)
        {
            CheckLicense();
        }
        //*********************************************************************
        //FILE CONTROL
        //*********************************************************************
        private void bttnArchive_Click(object sender, EventArgs e)
        {
            Process.Start(txtArchive.Text);
        }
        private void bttnLog_Click(object sender, EventArgs e)
        {
            Process.Start(txtLogFile.Text);
        }
        //*********************************************************************
        //TOOL BLOCK
        //*********************************************************************

        private void cogToolBlockEditV21_Load(object sender, EventArgs e)
        {

        }
        private void cbTBCameraSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbTBToolSelected.Items.Clear();
            for (int i = 0; i < toolCount; i++)
            {
                cbTBToolSelected.Items.Add(toolblockArray[cbTBCameraSelected.SelectedIndex, i].Name);
            }
        }
        private void bttnToolBlockLoad_Click(object sender, EventArgs e)
        {

            int cameraSelected = cbTBCameraSelected.SelectedIndex;
            int toolSelected = cbTBToolSelected.SelectedIndex;

            //make sure that there are no selection out of range of arrays
            if (cameraSelected < 0 || cameraSelected >= cameraCount) { return; }
            if (toolSelected < 0 || toolSelected >= toolCount) { return; }


            if (toolblockArray[cameraSelected, toolSelected].ToolReady)
            {
                cogToolBlockEditV21.Subject = toolblockArray[cameraSelected, toolSelected].cogToolBlock;
            }

            if (cogToolBlockEditV21.Subject != null)
            {
                cogToolBlockEditV21.Subject.Inputs[0].Value = toolblockArray[cameraSelected, toolSelected].cogToolBlock.Inputs[0].Value;
                cogToolBlockEditV21.Subject.Run();
            }
        }
        private void bttnUpdateImage_Click(object sender, EventArgs e)
        {
            int cameraSelected = cbTBCameraSelected.SelectedIndex;
            if (cogToolBlockEditV21.Subject != null)
            {
                cogToolBlockEditV21.Subject.Inputs[0].Value = CameraAcqArray[cameraSelected].Image;
                cogToolBlockEditV21.Subject.Run();
            }
        }

        //*********************************************************************
        //PLC CONNECTION
        //*********************************************************************
        private void bttnPLC_Click(object sender, EventArgs e)
        {
            MainPLC.BaseTag = tbBaseTag.Text;
            MainPLC.IPAddress = $"{numIP1.Value.ToString()}.{numIP2.Value.ToString()}.{numIP3.Value.ToString()}.{numIP4.Value.ToString()}";

            if(!PlcCommsActive)
            {
                MainPLC.InitializePlcComms();

                if (MainPLC.InitialCheck.Status == IPStatus.Success)
                {
                    bttnPLC.Text = "Connected - Press to Disconnect";
                    pollingTimer.Start();
                }
                for (int i = 0; i < cameraCount; i++) { cameraControl[i].DisableCameraControl(); }

                PlcCommsActive = true;
            }
            else
            {
                bttnPLC.Text = "Disconnected - Press to Connect ";
                pollingTimer.Stop();
                for (int i = 0; i < cameraCount; i++) { cameraControl[i].EnableCameraControl(); }
                PlcCommsActive = false;
            }

        }

        private void bttnPlcPing_Click(object sender, EventArgs e)
        {
            MainPLC.IPAddress = $"{numIP1.Value.ToString()}.{numIP2.Value.ToString()}.{numIP3.Value.ToString()}.{numIP4.Value.ToString()}";
            tbPlcPingResponse.Text = "Pinging";
            this.Refresh();
            PingReply temp = MainPLC.PingPLC();
            tbPlcPingResponse.Text = temp.Status.ToString();
        }

        private void cbHeartbeat_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void bttnOpenProject_Click(object sender, EventArgs e)
        {
            Process.Start(Utilities.ExeFilePath);
        }

        private void bttnAutoConnect_Click(object sender, EventArgs e)
        {
            
            for (int i = cameraCount - 1; i >= 0; i--)
            {
                if (!CameraAcqArray[i].Connected && CameraAcqArray[i].LoadServerSelect != null && CameraAcqArray[i].LoadResourceIndex != -1)
                {
                    CameraAcqArray[i].CreateCamera();
                    if (!CameraAcqArray[i].Connected) { return; }
                }
                
            }

            this.WindowState = FormWindowState.Maximized;
            tabControl1.SelectedIndex = 0;
        }

        private void cbCameraSelected_DropDown(object sender, DragEventArgs e)
        {

        }
    }

}


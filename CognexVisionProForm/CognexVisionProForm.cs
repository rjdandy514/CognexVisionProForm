using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using DALSA.SaperaLT.SapClassBasic;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using EventArgs = System.EventArgs;


namespace CognexVisionProForm
{
    public partial class CognexVisionProForm : Form
    {
        private bool heartBeat = false;
        private bool PlcAutoMode;
        private bool[] ResultReadyOk;

        private CogStringCollection LicenseCheck;
        private bool cogLicenseOk;

        int selectedCameraId;
        int cameraConnectCount;
        int dataLength = 8;
        public DalsaImage[] CameraAcqArray;
        bool[] cameraSnap;
        bool[] cameraSnapComplete;
        bool[] toolTrigger;
        bool[] toolTriggerComplete;
        public CameraControl[] cameraControl;

        SplashScreen splashScreen;

        private int ExpireCount = 0;

        private bool ExpireError = false;

        private int[] desiredTool;
        private int[] plcTool;
        private ToolBlock[,] toolblockArray;

        private PlcComms MainPLC;

        public TextWriterTraceListener appListener;

        int cameraCount;
        public int toolCount;
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
                MainPLC.ReadPlcDataTag();
                MainPLC.ReadPlcTag();
               
                PlcReadData();
                PlcRead();

                //Send Data to PLC
                PlcWriteData();
                PlcWrite();
                MainPLC.WritePlcDataTag();
                MainPLC.WritePlcTag();
                
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
            Utilities.InitLog(LogDir, ArchiveLogDir);
            txtArchive.Text = ArchiveLogDir;
            txtLogFile.Text = LogDir + "Log.log";

            CameraAcqArray = new DalsaImage[cameraCount];
            toolblockArray = new ToolBlock[cameraCount, toolCount];
            desiredTool = new int[cameraCount];
            plcTool = new int[cameraCount];
            cameraControl = new CameraControl[cameraCount];
            cameraSnap = new bool[cameraCount];
            cameraSnapComplete = new bool[cameraCount];
            ResultReadyOk = new bool[cameraCount];
            toolTrigger = new bool[cameraCount];
            toolTriggerComplete = new bool[cameraCount];

            

            splashScreen.UpdateProgress("Initialize Classes", 10);
            InitClasses();

            splashScreen.UpdateProgress("Load Settings", 10);
            LoadSettings();

            splashScreen.UpdateProgress("Initialize Server List", 10);
            InitServerList(0);

            splashScreen.UpdateProgress("Initialize Resource List", 10);
            InitResourceList(0);

            splashScreen.UpdateProgress("Initialize Job Manager", 10);
            InitJobManager();

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
            resize_CameraControl();
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
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (tabControl1.SelectedTab.Name == "tabImage")
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
                resize_CameraControl();
            }
            else if (tabControl1.SelectedTab.Name == "tabFrameGrabber")
            {
                UpdateFrameGrabberTab();
            }
            else if (tabControl1.SelectedTab.Name == "tabToolBlock")
            {
                cbTBCameraSelected.Items.Clear();

                foreach (DalsaImage camera in CameraAcqArray)
                {
                    cbTBCameraSelected.Items.Add(camera.Name);
                }
                cbTBCameraSelected.SelectedIndex = 0;
                
                cbTBToolSelected.Items.Clear();
                for (int i = 0; i < toolCount; i++)
                {
                    cbTBToolSelected.Items.Add(toolblockArray[cbTBCameraSelected.SelectedIndex, i].Name);
                }
                cbTBToolSelected.SelectedIndex = 0;

            }
            else if (tabControl1.SelectedTab.Name == "tabPlcConnection")
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

            UpdateConfigFileData();
            cbCameraConnected.Checked = CameraAcqArray[selectedCameraId].Connected;
            tbCameraName.Text = CameraAcqArray[selectedCameraId].Name;
            tbCameraDesc.Text = CameraAcqArray[selectedCameraId].Description;

            tbArchiveCount.Text = CameraAcqArray[selectedCameraId].ArchiveImageCount.ToString();
            tbArchiveIndex.Text = CameraAcqArray[selectedCameraId].ArchiveImageIndex.ToString();
            cbArchiveActive.Checked = CameraAcqArray[selectedCameraId].ArchiveImageActive;

            if (CameraAcqArray[selectedCameraId].Connected) { bttnConnectCamera.Text = "Disconnect"; }
            else { bttnConnectCamera.Text = "Connect"; }

            InitServerList(selectedCameraId);
            InitResourceList(selectedCameraId);

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
            int acqCount = SapManager.GetResourceCount(cbServerList.SelectedItem.ToString(), SapManager.ResourceType.Acq);
            int acqDeviceCount = SapManager.GetResourceCount(cbServerList.SelectedItem.ToString(), SapManager.ResourceType.AcqDevice);


            cbConfigFileReq.Checked = acqCount >0;
            CameraAcqArray[selectedCameraId].LoadServerSelect = cbServerList.SelectedItem.ToString();
            InitResourceList(selectedCameraId);

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
        private void bttnConfigSelect_Click(object sender, EventArgs e)
        {
            CameraAcqArray[selectedCameraId].LoadConfigFile();
            UpdateConfigFileData();
            


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

            cbToolBlockFileFound.Checked = toolblockArray[cbCameraIdSelected.SelectedIndex, cbToolBlock.SelectedIndex].FilePresent;
            tbToolBlockName.Text = toolblockArray[cbCameraIdSelected.SelectedIndex, cbToolBlock.SelectedIndex].Name;
            tbToolBlockNameEdit.Text = toolblockArray[cbCameraIdSelected.SelectedIndex, cbToolBlock.SelectedIndex].Name;
            cbToolBlockEnabled.Checked = toolblockArray[cbCameraIdSelected.SelectedIndex, cbToolBlock.SelectedIndex].ToolReady;
        }
        private void bttnToolBockFileSelect_Click(object sender, EventArgs e)
        {

            string toolNameUpdated = tbToolBlockNameEdit.Text.ToString();
            int toolSelected = cbToolBlock.SelectedIndex;


            if (toolSelected >= 0 && toolSelected < toolCount)
            {
                toolblockArray[cbCameraIdSelected.SelectedIndex, toolSelected].Name = toolNameUpdated;
                toolblockArray[cbCameraIdSelected.SelectedIndex, toolSelected].LoadvisionProject();
                toolblockArray[cbCameraIdSelected.SelectedIndex, toolSelected].InitJobManager();

                cbToolBlock.Items[toolSelected] = toolNameUpdated;
            }


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
        private void bttnOpenProject_Click(object sender, EventArgs e)
        {
            Process.Start(Utilities.ExeFilePath);
        }
        //*********************************************************************
        //TOOL BLOCK
        //*********************************************************************

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
        private void bttnSaveJob_Click(object sender, EventArgs e)
        {
            toolblockArray[cbTBCameraSelected.SelectedIndex, cbTBToolSelected.SelectedIndex].SaveVisionProject();
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


    }

}


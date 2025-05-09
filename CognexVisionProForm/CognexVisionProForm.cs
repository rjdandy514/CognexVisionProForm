﻿using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using DALSA.SaperaLT.SapClassBasic;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using EventArgs = System.EventArgs;
using System.Threading.Tasks;
using System.Data;
using System.IO;


namespace CognexVisionProForm
{
    public partial class CognexVisionProForm : Form
    {
        private bool heartBeat = false;
        private bool PlcAutoMode;
        private bool[] ResultReadyOk;
        private bool plcRetry;
        private bool plcRetry_Mem;


        private CogStringCollection LicenseCheck;
        private bool cogLicenseOk;
        private bool systemIdle = true;

        int selectedCameraId;
        int cameraConnectCount;
        int cameraImageFlip;
        int dataLength;
        public DalsaImage[] CameraAcqArray;
        bool[] cameraSnap;
        bool[] cameraSnapComplete;
        bool[] toolTrigger;
        bool[] toolTriggerComplete;
        public CameraControl[] cameraControl;
        int[] recipeSelected;
        Task[] taskToolRun;

        SplashScreen splashScreen;

        private int ExpireCount = 0;

        private bool ExpireError = false;

        TypeConverter convertToString = TypeDescriptor.GetConverter(typeof(String));
        TypeConverter convertToDouble = TypeDescriptor.GetConverter(typeof(double));

        private int[] desiredTool;
        private int[] plcTool;
        private ToolBlock[,] toolblockArray;
        private ToolBlock[] preProcess;

        private PlcComms MainPLC;

        public TextWriterTraceListener appListener;

        int cameraCount;
        public int toolCount;
        string computerName;
        bool preProcessRequired;

        string ServerNotFound = "No Server Found";

        private System.Windows.Forms.Timer pollingTimer;
        private System.Windows.Forms.Timer watchDogTimer;
        public CognexVisionProForm()
        {
            InitializeComponent();

        }

        private void pollingTimer_Tick(object sender, EventArgs e)
        {
            pollingTimer.Stop();

            heartBeat = !heartBeat;
            cbHeartbeat.Checked = heartBeat;
            cbSystemIdle.Checked = SystemIdle;



            if (!ThreadsAlive && MainPLC.InitialCheck.Status == IPStatus.Success)
            {
                //Get all data from PLC
                MainPLC.ReadPlcDataTag();
                MainPLC.ReadPlcTag();
                MainPLC.ReadPlcStringTag();
               
                PlcReadData();
                PlcRead();

                //Send Data to PLC
                PlcWriteData();
                PlcWrite();
                MainPLC.WritePlcString();
                MainPLC.WritePlcDataTag();
                MainPLC.WritePlcTag();
                
            }
            pollingTimer.Start();
        }
        private void watchDogTimer_Tick(object sender, EventArgs e)
        {
            watchDogTimer.Stop();

            MessageBox.Show("watchdogTimer was triggered");

        }
        //*********************************************************************
        //GENERAL FORM CONTROL
        //*********************************************************************
        private void Form1_Load(object sender, EventArgs e)
        {

           

            Thread.CurrentThread.Name = "Main Form";
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
            preProcess = new ToolBlock[cameraCount];
            toolblockArray = new ToolBlock[cameraCount, toolCount];
            desiredTool = new int[cameraCount];
            plcTool = new int[cameraCount];
            cameraControl = new CameraControl[cameraCount];
            cameraSnap = new bool[cameraCount];
            cameraSnapComplete = new bool[cameraCount];
            ResultReadyOk = new bool[cameraCount];
            toolTrigger = new bool[cameraCount];
            toolTriggerComplete = new bool[cameraCount];
            taskToolRun = new Task[cameraCount];
            recipeSelected = new int[cameraCount];
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
            else { tabControl1.SelectedIndex = 1; }

            cogToolBlockEditV21.SubjectChanged += new EventHandler(cogToolBlockEditV21_SubjectChanged);

            splashScreen.UpdateProgress("Getting Ready", 10);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            resize_CameraControl();
            resize_tabToolBlock();
            resize_tabFileControl();
            resize_tabCameraData();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseCancel() == false)
            {
                e.Cancel = true;
                return;
            }

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

            cogToolBlockEditV21.SubjectChanged -= new EventHandler(cogToolBlockEditV21_SubjectChanged);
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
                        Utilities.LoadForm(allPanels[cameraConnectCount], cameraControl[i]);
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
            else if (tabControl1.SelectedTab.Name == "tabCameraData")
            {
                resize_CameraControl();
            }

            if (tabControl1.SelectedTab.Name != "tabToolBlock")
            {
                    cogToolBlockEditV21.Subject = null;
            }
            if (tabControl1.SelectedTab.Name != "tabImage")
            {

            }
        }
        private void tabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            

        }
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!SystemIdle) { e.Cancel = true; }

            if (e.TabPage == tabToolBlock)
            {
                if (PlcCommsActive)
                {
                    e.Cancel = true;
                    MessageBox.Show("Disconnect from PLC before accessing Toolblock");
                }
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

            int archiveCount = (int) CameraAcqArray[selectedCameraId].ArchiveImageCount;
            tbArchiveCount.Text = archiveCount.ToString();
            numArchiveIndex.Maximum = archiveCount - 1;

            numArchiveIndex.Minimum = 0;
            numArchiveIndex.Value = 0;
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

            cbPreProcessFileFound.Checked = preProcess[cbCameraIdSelected.SelectedIndex].FilePresent;
            cbPreProcessEnabled.Checked = preProcess[cbCameraIdSelected.SelectedIndex].ToolReady;
            tbPreProcessName.Text = preProcess[cbCameraIdSelected.SelectedIndex].Name;

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

            SaveSettings();
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
                toolblockArray[cbCameraIdSelected.SelectedIndex, toolSelected].LoadVisionProject();
                toolblockArray[cbCameraIdSelected.SelectedIndex, toolSelected].InitJobManager();

                cbToolBlock.Items[toolSelected] = toolNameUpdated;
            }

            SaveSettings();
        }
        private void bttnAutoConnect_Click(object sender, EventArgs e)
        {
           var allConnected = CameraAcqArray.All(x => x.Connected);

            if(!allConnected)
            {
                for (int i = cameraCount - 1; i >= 0; i--)
                {

                    if (!CameraAcqArray[i].Connected && CameraAcqArray[i].LoadServerSelect != null && CameraAcqArray[i].LoadResourceIndex != -1)
                    {

                        //get serial number of pr
                        SapLocation serverLocation = new SapLocation(CameraAcqArray[i].LoadServerSelect, CameraAcqArray[i].LoadResourceIndex);

                        string serialNumberCheck = SapManager.GetSerialNumber(serverLocation);

                        if (CameraAcqArray[i].SerialNumber != serialNumberCheck)
                        {
                            MessageBox.Show($"{CameraAcqArray[i].Name} did not connect: Serial number stored{CameraAcqArray[i].SerialNumber} does not match current selection {serialNumberCheck}");
                            continue;
                        }
                        CameraAcqArray[i].CreateCamera();



                        if (!CameraAcqArray[i].Connected) { return; }
                        Thread.Sleep(2000);

                        CameraAcqArray[i].SaveImageSelected = true;
                    }

                }
                SaveSettings();
                this.WindowState = FormWindowState.Maximized;
                tabControl1.SelectedIndex = 0;

                bttnAutoConnect.Text = "Auto Disconnect All Cameras";
            }
            else
            {
                for (int i = 0;i < cameraCount; i++)
                {
                    CameraAcqArray[i].Disconnect();
                    Thread.Sleep(300);
                }
                bttnAutoConnect.Text = "Auto Connect All Cameras";
            }


            
        }
        private void bttnPreProcessFileSelect_Click(object sender, EventArgs e)
        {
            string preProcessNameUpdated = tbPreProcessNameEdit.Text.ToString();

            preProcess[cbCameraIdSelected.SelectedIndex].Name = preProcessNameUpdated;
            preProcess[cbCameraIdSelected.SelectedIndex].LoadVisionProject();
            preProcess[cbCameraIdSelected.SelectedIndex].InitJobManager();

            SaveSettings();

        }
        private void numArchiveIndex_ValueChanged(object sender, EventArgs e)
        {
            CameraAcqArray[selectedCameraId].ArchiveImageIndex = (int)numArchiveIndex.Value;
        }
        private void bttnLoadParameter_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Recipe Name", typeof(string));
            dt.Columns.Add("Data Name", typeof(string));

            int toolIndex = cbToolBlock.SelectedIndex;
            int cam = cbCameraIdSelected.SelectedIndex;

            if (toolblockArray[cam, toolIndex].ToolReady)
            { 
                toolblockArray[cam, toolIndex].GetRecipe();
            }

            object[] rowArray = new object[2];
            if(toolblockArray[cam, toolIndex].Recipe == null) { return; }
            
            for (int i = 0; i < toolblockArray[cam, toolIndex].Recipe.Count; i++)
            {
                rowArray[0] = toolblockArray[cam, toolIndex].Recipe[i].Name;
                rowArray[1] = toolblockArray[cam, toolIndex].Recipe[i].Value.ToString();
                dt.Rows.Add(rowArray);
            }
            dgRecipe.DataSource = dt;
            dt= null;
            
        }
        private void bttnSaveParameter_Click(object sender, EventArgs e)
        {
            List<ToolRecipe> recipes = new List<ToolRecipe>();

            for(int i = 0; i< dgRecipe.Rows.Count; i++)
            {
                if (dgRecipe.Rows[i].Cells[0].Value == null) { continue; }
                string name = dgRecipe.Rows[i].Cells[0].Value.ToString();
                double value = Convert.ToDouble(dgRecipe.Rows[i].Cells[1].Value);
                recipes.Add(new ToolRecipe(name, value));
            }

            int toolIndex = cbToolBlock.SelectedIndex;
            int cam = cbCameraIdSelected.SelectedIndex;
            toolblockArray[cam, toolIndex].Recipe = recipes;

            for (int i = 0; i < cameraCount; i++)
            {
                if (toolblockArray[i, toolIndex].ToolReady)
                {
                    toolblockArray[i, toolIndex].Recipe = recipes;
                    toolblockArray[i, toolIndex].SetRecipe();
                }
            }
                recipes = null;
        }

        //*********************************************************************
        //FRAME GRABBER - RECIPE CONTROL
        //*********************************************************************
        private void bttnRecipeCreate_Click(object sender, EventArgs e)
        {
            bool recipeExists;
            string recipeName;
            string recipePath;

            if (tbRecipeNew.Text == "")
            {
                MessageBox.Show("Enter valid Name");
                return;
            }

            recipeName = tbRecipeNew.Text;
            recipePath = Utilities.ExeFilePath + $"\\Camera{cbCameraIdSelected.SelectedIndex.ToString("00")}\\Recipe\\{recipeName}";
            recipeExists = Directory.Exists(recipePath);

            if (!recipeExists) { Directory.CreateDirectory(recipePath); }

            CreateRecipeList();
        }
        private void bttnRecipeDelete_Click(object sender, EventArgs e)
        {
            bool recipeExists;
            string recipeName;
            string recipePath;

            if (cbRecipe.SelectedItem == null) { return; }
            recipeName = cbRecipe.SelectedItem.ToString();
            recipePath = Utilities.ExeFilePath + $"\\Camera{cbCameraIdSelected.SelectedIndex.ToString("00")}\\Recipe\\{recipeName}";

            recipeExists = Directory.Exists(recipePath);
            if (recipeExists) { Directory.Delete(recipePath,true); }
            CreateRecipeList();
        }
        private void bttnRecipeSave_Click(object sender, EventArgs e)
        {
            if(cbRecipe.SelectedItem.ToString() != "" && cbCameraIdSelected.SelectedIndex >= 0)
            {
                CameraConfigSave(cbRecipe.SelectedItem.ToString(), cbCameraIdSelected.SelectedIndex);
                ToolblockRecipeSave(cbRecipe.SelectedItem.ToString(), cbCameraIdSelected.SelectedIndex);
            }

        }
        private void bttnRecipeLoad_Click(object sender, EventArgs e)
        {

            if (cbRecipe.SelectedItem.ToString() != "" && cbCameraIdSelected.SelectedIndex >= 0)
            {
                CameraConfigLoad(cbRecipe.SelectedItem.ToString(), cbCameraIdSelected.SelectedIndex);
                ToolblockRecipeLoad(cbRecipe.SelectedItem.ToString(), cbCameraIdSelected.SelectedIndex);
                recipeSelected[cbCameraIdSelected.SelectedIndex] = cbRecipe.SelectedIndex;
            }
            UpdateFrameGrabberTab();
        }
        private void bttnRecipeLoadAll_Click(object sender, EventArgs e)
        {
            for(int i = cameraCount - 1; i >= 0;i--)
            {
                if (cbRecipe.SelectedItem.ToString() == "") { return; }
                CameraConfigLoad(cbRecipe.SelectedItem.ToString(), i);
                ToolblockRecipeLoad(cbRecipe.SelectedItem.ToString(), i);
                recipeSelected[i] = cbRecipe.SelectedIndex;
            }

            UpdateFrameGrabberTab();
            MessageBox.Show("All Recipe Files have been Loaded \n Reconnected Cameras ");

        }
        private void bttnRecipeSaveAll_Click(object sender, EventArgs e)
        {
            for (int i = cameraCount - 1; i >= 0; i--)
            {
                if (cbRecipe.SelectedItem.ToString() == "") { return; }
                CameraConfigSave(cbRecipe.SelectedItem.ToString(), i);
                ToolblockRecipeSave(cbRecipe.SelectedItem.ToString(), i);
            }

            MessageBox.Show("All Recipe Files have been Saved");
        }
        private void cbRecipe_SelectedIndexChanged(object sender, EventArgs e)
        {

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
            int toolMemory = cbTBToolSelected.SelectedIndex;
            cbTBToolSelected.Items.Clear();
            for (int i = 0; i < toolCount; i++)
            {
                cbTBToolSelected.Items.Add(toolblockArray[cbTBCameraSelected.SelectedIndex, i].Name);
            }

            if(toolMemory < cbTBToolSelected.Items.Count)
            {
                cbTBToolSelected.SelectedIndex = toolMemory;
            }
        }
        private void bttnToolBlockLoad_Click(object sender, EventArgs e)
        {
            CogToolBlock toolBlock;
            int cameraSelected = cbTBCameraSelected.SelectedIndex;
            int toolSelected = cbTBToolSelected.SelectedIndex;

            //make sure that there are no selection out of range of arrays
            if (cameraSelected < 0 || cameraSelected >= cameraCount) 
            {
                MessageBox.Show("Invalid Camera Selected");
                return; 
            }
            if (toolSelected < 0 || toolSelected >= toolCount) 
            {
                MessageBox.Show("Invalid ToolBlock Selected");
                return; 
            }
            if (toolblockArray[cameraSelected, toolSelected].toolBlock == null) 
            {
                MessageBox.Show("ToolBlock is null");
                return; 
            }

            try
            {

                //Disbale the Page to prevent users from stealing focus while toolblock is loading
                cogToolBlockEditV21.Enabled = false;
                Thread.Sleep(200);
                toolBlock = CogSerializer.DeepCopyObject(toolblockArray[cameraSelected, toolSelected].toolBlock) as CogToolBlock;
                cogToolBlockEditV21.Subject = new CogToolBlock();
                cogToolBlockEditV21.Subject = toolBlock;
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void bttnUpdateImage_Click(object sender, EventArgs e)
        {
            int cameraSelected = cbTBCameraSelected.SelectedIndex;
            if (cogToolBlockEditV21.Subject != null)
            {
                if(preProcessRequired)
                {
                    preProcess[cameraSelected].Inputs[0].Value = CameraAcqArray[cameraSelected].Image;
                    preProcess[cameraSelected].Inputs[1].Value = cameraSelected;
                    preProcess[cameraSelected].ToolRun();
                    Thread.Sleep(200);
                    cogToolBlockEditV21.Subject.Inputs[0].Value = preProcess[cameraSelected].Outputs[0].Value as CogImage8Grey;
                }
                else
                {
                    cogToolBlockEditV21.Subject.Inputs[0].Value = CameraAcqArray[cameraSelected].Image;
                }
                cogToolBlockEditV21.Subject.Run();
            }
        }
        private void bttnSaveJob_Click(object sender, EventArgs e)
        {
            if (cogToolBlockEditV21.Subject != null)
            {
                toolblockArray[cbTBCameraSelected.SelectedIndex, cbTBToolSelected.SelectedIndex].toolBlock = (CogToolBlock)CogSerializer.DeepCopyObject(cogToolBlockEditV21.Subject);
                toolblockArray[cbTBCameraSelected.SelectedIndex, cbTBToolSelected.SelectedIndex].SaveVisionProject();
                MessageBox.Show("Program Saved");

            }
        }
        void cogToolBlockEditV21_SubjectChanged(object sender, EventArgs e)
        {
            //Triggered When the new tool block is loaded
            //This has not been tested
            cogToolBlockEditV21.Enabled = true;
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
        //*********************************************************************
        //camera Data
        //*********************************************************************
        private void bttnGetData_Click(object sender, EventArgs e)
        {
            BuildDataGrid();
        }


    }

}


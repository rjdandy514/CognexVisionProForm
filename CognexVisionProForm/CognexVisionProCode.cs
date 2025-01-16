using Cognex.VisionPro;
using DALSA.SaperaLT.SapClassBasic;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DalsaImage;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Cognex.VisionPro.QuickBuild.Implementation.Internal;
using System.Xml.Linq;
using System.Net.NetworkInformation;
using Cognex.Vision.Meta;
using Cognex.VisionPro.ToolBlock;
using Cognex.Vision;
using System.Threading;
using System.Diagnostics;
using Cognex.VisionPro.Exceptions;
using System.Data;
using System.ComponentModel;
using System.IO;

namespace CognexVisionProForm
{
    public partial class CognexVisionProForm
    {
        public int CameraSnap
        {
            set
            {
                cameraSnap[value] = true;
            }
        }
        public int CameraSnapComplete
        {
            set
            {
                cameraSnapComplete[value] = true;
                CameraUpdate();
            }
        }
        public bool PlcCommsActive
        {
            get; set;
        }
        public bool ThreadsAlive
        {
            get { return threadToolAlive; }
        }
        public int Recipe
        {
            get => recipe;
            set 
            {
                recipe = value;
                RecipeChange();
            }
        }
        public int StationNumber
        { get; set; }

        public void InitClasses()
        {
            pollingTimer = new System.Windows.Forms.Timer();
            pollingTimer.Tick += new EventHandler(pollingTimer_Tick);
            pollingTimer.Interval = 200; // in miliseconds

            for (int j = 0; j < cameraCount; j++)
            {
                splashScreen.UpdateProgress($"Initialize Camera {j}", 5);
                CameraAcqArray[j] = new DalsaImage(this);
                CameraAcqArray[j].Flip = cameraImageFlip;
                CameraAcqArray[j].Id = j;

                preProcess[j] = new ToolBlock(this);
                preProcess[j].CameraId = j;
                preProcess[j].Preprocess = true;

                for (int i = 0; i < toolCount; i++)
                {
                    toolblockArray[j, i] = new ToolBlock(this);
                    toolblockArray[j, i].CameraId = j;
                    toolblockArray[j, i].Preprocess = false;
                    
                }

                cameraControl[j] = new CameraControl(this, CameraAcqArray[j]);
                cameraControl[j].PreProcess = preProcess[j];
                cameraControl[j].Tool = toolblockArray[j, 0];
                
            }
            recipeEcho = 99;
            Recipe = 0;

            MainPLC = new PlcComms(this);
            dataLength = MainPLC.PlcToPcControlData.Length / cameraCount;

            Utilities.LoggingStatment($"InitializeAcquisition Complete");
        }
        public void InitJobManager()
        {
            splashScreen.UpdateProgress("Initialize JobManager", 10);

            Thread[] threadPreProcess;
            threadPreProcess = new Thread[cameraCount];
            
            Thread[] threadToolBlock;
            threadToolBlock = new Thread[cameraCount];

            for (int cam = 0; cam <cameraCount; cam++)
            {
                if(preProcess[cam].FilePresent)
                {
                    splashScreen.UpdateProgress($"Initialize JobManager: {CameraAcqArray[cam].Name} - {preProcess[cam].Name} Toolblock", 1);

                    //threadPreProcess[cam] = new Thread(preProcess[cam].InitJobManager); 
                    //threadPreProcess[cam].Start();

                    preProcess[cam].InitJobManager();
                }

                for (int i = 0; i < toolblockArray.GetLength(1); i++)
                {
                    if (toolblockArray[cam, i].FilePresent)
                    { 
                        splashScreen.UpdateProgress($"Initialize JobManager: {CameraAcqArray[cam].Name} - {toolblockArray[cam, i].Name} Toolblock", 1);

                        //threadToolBlock[cam] = new Thread(toolblockArray[cam, i].InitJobManager);
                        //threadToolBlock[cam].Start();

                        toolblockArray[cam, i].InitJobManager();
                    }
                }

                /*
                if (threadPreProcess[cam] != null) { threadPreProcess[cam].Join(); }
                foreach (Thread t in threadToolBlock)
                {
                    if (t != null) { t.Join(); }
                }
                */

            }
            
            splashScreen.Close();
        }
        public void InitServerList(int cameraIndex)
        {
            cbServerList.Items.Clear();

            int serverCount = SapManager.GetServerCount();
            for (int i = 0; i < serverCount; i++)
            {

                int acqCount = SapManager.GetResourceCount(i, SapManager.ResourceType.Acq);
                int acqDeviceCount = SapManager.GetResourceCount(i, SapManager.ResourceType.AcqDevice);

                // Does this server support "Acq" (frame-grabber) or "AcqDevice" (camera)?
                bool bAcq =(acqCount > 0);
                bool bAcqDevice =(acqDeviceCount > 0 && acqCount == 0);

                //Add all servers to the combobox list

                if (bAcq) { cbServerList.Items.Add(SapManager.GetServerName(i)); }
                else if (bAcqDevice) { cbServerList.Items.Add(SapManager.GetServerName(i)); }
            }

            //select the server that was used last time
            int ServerNameFoundIndex = cbServerList.FindString(CameraAcqArray[cameraIndex].LoadServerSelect, 0);
            if (!string.IsNullOrEmpty(CameraAcqArray[cameraIndex].LoadServerSelect) && ServerNameFoundIndex != -1)
            {
                cbServerList.SelectedIndex = ServerNameFoundIndex;
            }
            else if (cbServerList.Items.Count > 0)
            {
                cbServerList.SelectedIndex = 0;
            }
            else
            {
                cbServerList.Items.Add(ServerNotFound);
                cbServerList.SelectedIndex = 0;
            }

            CameraAcqArray[cameraIndex].LoadServerSelect = cbServerList.SelectedItem.ToString();
        }
        public void InitResourceList(int cameraIndex)
        {
            int AcqCount = 0;
            int AcqDeviceCount = 0;

            cbDeviceList.Items.Clear();
            
            //Get count of both Acq (frame-grabber) or AcqDevices (camera)
            if (!string.IsNullOrEmpty(CameraAcqArray[cameraIndex].LoadServerSelect) && CameraAcqArray[cameraIndex].LoadServerSelect != ServerNotFound)
            {
                
                AcqCount = SapManager.GetResourceCount(CameraAcqArray[cameraIndex].LoadServerSelect, SapManager.ResourceType.Acq);
                AcqDeviceCount = SapManager.GetResourceCount(CameraAcqArray[cameraIndex].LoadServerSelect, SapManager.ResourceType.AcqDevice);
            }

            if (AcqCount > 0)
            {
                for (int i = 0; i < AcqCount; i++)
                {
                    
                    string AcqName = SapManager.GetResourceName(CameraAcqArray[cameraIndex].LoadServerSelect, SapManager.ResourceType.Acq, i);
                    
                    if (SapManager.IsResourceAvailable(CameraAcqArray[cameraIndex].LoadServerSelect, SapManager.ResourceType.Acq, i))
                    {
                        cbDeviceList.Items.Add(AcqName);
                    }
                    else
                    {
                        cbDeviceList.Items.Add("Not Available - Resource in Use");
                    }
                }
            }

            if (AcqDeviceCount > 0)
            {
                for (int i = 0; i < AcqDeviceCount; i++)
                {
                    string AcqDevice = SapManager.GetResourceName(CameraAcqArray[cameraIndex].LoadServerSelect, SapManager.ResourceType.AcqDevice, i);
                    if (SapManager.IsResourceAvailable(CameraAcqArray[cameraIndex].LoadServerSelect, SapManager.ResourceType.AcqDevice, i))
                    {
                        cbDeviceList.Items.Add(AcqDevice);
                    }
                    else
                    {
                        cbDeviceList.Items.Add("Not Available - Resource in Use");
                    }
                }
            }

            if (AcqCount == 0 && AcqDeviceCount == 0 || cbDeviceList.Items.Count == 0)
            {
                cbDeviceList.Items.Add("No Camera Found");
                CameraAcqArray[cameraIndex].LoadResourceIndex = 0;
            }

            cbDeviceList.SelectedIndex = CameraAcqArray[cameraIndex].LoadResourceIndex;
            CameraAcqArray[cameraIndex].LoadResourceName = cbDeviceList.SelectedItem.ToString();
        }
        public void UpdateConfigFileData()
        {
            cbConfigFileFound.Checked = CameraAcqArray[selectedCameraId].ConfigFilePresent;

            if(CameraAcqArray[selectedCameraId].ConfigFilePresent)
            {
                CameraAcqArray[selectedCameraId].GetConfigFileInfo();
                txtCompany.Text = CameraAcqArray[selectedCameraId].CompanyName;
                txtModel.Text = CameraAcqArray[selectedCameraId].ModelName;
                txtVicName.Text = CameraAcqArray[selectedCameraId].Vicname;
            }
            
        }
        public void RecipeData()
        {
            
            //Update the Part Type
            PartType[0] = "LCS-M";
            PartType[1] = "LCS-H";
            PartType[2] = "HPS";
            PartType[3] = "LCS-X";
            //determine FOV of camera depending on part type and station number
            switch (StationNumber)
            {
                case 70:
                    cameraCropLeft[0] = 1400;
                    cameraCropWidth[0] = 5300;
                    cameraCropLeft[1] = 0;
                    cameraCropWidth[1] = 8192;
                    cameraCropLeft[2] = 1400;
                    cameraCropWidth[2] = 5300;
                    cameraCropLeft[3] = 1400;
                    cameraCropWidth[3] = 5300;
                    break;
                case 90:
                    cameraCropLeft[0] = 1500;
                    cameraCropWidth[0] = 5300;
                    cameraCropLeft[1] = 1500;
                    cameraCropWidth[1] = 5300;
                    cameraCropLeft[2] = 1500;
                    cameraCropWidth[2] = 5300;
                    cameraCropLeft[3] = 1500;
                    cameraCropWidth[3] = 5300;
                    break;
                default:
                    cameraCropLeft[0] = 0;
                    cameraCropWidth[0] = 8192;
                    cameraCropLeft[1] = 0;
                    cameraCropWidth[1] = 8192;
                    cameraCropLeft[2] = 0;
                    cameraCropWidth[2] = 8192;
                    cameraCropLeft[3] = 0;
                    cameraCropWidth[3] = 8192;
                    break;
            }
        }
        public void RecipeChange()
        {
            //update Recipe data
            RecipeData();

            //Only change recipe if system is idel
            if (!systemIdle) { return; }
            if (recipe >= cameraCropWidth.Length) { recipe = 0; }
            
            //only update recipe if recipe number has changed
            if (recipe != recipeEcho)
            {
                for(int cam = cameraCount-1; cam >= 0; cam--)
                {
                    CameraAcqArray[cam].CropWidth = cameraCropWidth[recipe];
                    CameraAcqArray[cam].CropLeft = cameraCropLeft[recipe];
                    if (CameraAcqArray[cam].Connected) { CameraAcqArray[cam].ChangeFOV(); }
                    CameraAcqArray[cam].PartType = PartType[recipe];
                }
                recipeEcho = recipe;
            }
        }
        public void CameraAbort()
        {
            Array.Clear(toolTrigger, 0, toolTrigger.Length);
            Array.Clear(toolTriggerComplete, 0, toolTriggerComplete.Length);
            Array.Clear(cameraSnap, 0, cameraSnap.Length);
            Array.Clear(cameraSnapComplete, 0, cameraSnapComplete.Length);

            for (int i = 0; i < cameraCount;i++)
            {
                CameraAcqArray[i].AbortTrigger = true;
            }
        }
        public void CameraUpdate()
        {
            // Run ToolBlocks that are enabled
            if (cameraSnap.SequenceEqual(cameraSnapComplete)) 
            {
                Array.Clear(toolTrigger, 0, toolTrigger.Length);
                //if the cognex license is present, trigger Toolblock
                if (cogLicenseOk) { ToolBlockTrigger(); }
            }

        }
        public void ToolNumberUpdate(int cam)
        {
            if (MainPLC.InitialCheck != null && MainPLC.InitialCheck.Status == IPStatus.Success && PlcCommsActive)
            {
                if (desiredTool[cam] == plcTool[cam]) { return; };
                if (Enumerable.Range(0, toolCount).Contains(plcTool[cam]) && toolblockArray[cam, plcTool[cam]].ToolReady)
                {
                    desiredTool[cam] = plcTool[cam];
                }
                else { desiredTool[cam] = 0; }
            }

            else
            {
                if (desiredTool[cam] == cameraControl[cam].ToolSelect) { return; };
                if (Enumerable.Range(0, toolCount).Contains(cameraControl[cam].ToolSelect) && toolblockArray[cam, cameraControl[cam].ToolSelect].ToolReady)
                {
                    desiredTool[cam] = cameraControl[cam].ToolSelect;
                }
                else
                {
                    desiredTool[cam] = 0;
                }
            }
            cameraControl[cam].ToolSelect = desiredTool[cam];
            toolblockArray[cam, desiredTool[cam]].ResultUpdated = false;
            cameraControl[cam].Tool = toolblockArray[cam, desiredTool[cam]];

        }
        public void ToolBlockTrigger()
        {
            CogImage8Grey processedImage;
            Debug.WriteLine("Beginning of ToolBlock Trigger");

            if (ThreadAlive(taskToolRun)) { return; }
            GC.Collect();

            cogToolBlockEditV21.ActiveControl = null;

            for (int j = 0; j < cameraCount; j++)
            {
                if (cameraSnapComplete[j])
                {
                    toolTrigger[j] = true;
                    cameraSnap[j] = false;
                    cameraSnapComplete[j] = false;
                    
                    if (preProcessRequired && preProcess[j].ToolReady)
                    {
                        preProcess[j].Inputs[0].Value = CameraAcqArray[j].Image;
                        preProcess[j].ToolRun();
                    }
                }
            }

            for(int i = 0; i < cameraCount;i++)
            {
                if (preProcessRequired) { processedImage = preProcess[i].Outputs[0].Value as CogImage8Grey; }
                else { processedImage = CameraAcqArray[i].Image as CogImage8Grey; }
                if (toolTrigger[i])
                {
                    toolblockArray[i, desiredTool[i]].Inputs[0].Value = processedImage;
                    //toolblockArray[i, desiredTool[i]].ToolRun();

                    int camera = i;
                    int tool = desiredTool[camera];

                    taskToolRun[camera] = new Task(() => toolblockArray[camera, tool].ToolRun());
                    taskToolRun[camera].Start();
                }
            }
            Debug.WriteLine("Trigger Complete");

            Array.Clear(toolTrigger, 0, toolTrigger.Length);
            Array.Clear(cameraSnap, 0, cameraSnap.Length);
            Array.Clear(cameraSnapComplete, 0, cameraSnapComplete.Length);

        }
        public void RetryToolBlock()
        {
           
            for (int i = 0; i< cameraCount;i++)
            {
                if (CameraAcqArray[i].Image != null) { cameraSnapComplete[i] = true; }
            }

            ToolBlockTrigger();
        }
        public void UpdateFrameGrabberTab()
        {
            cbConfigFileFound.Checked = CameraAcqArray[selectedCameraId].ConfigFilePresent;
            cbCameraConnected.Checked = CameraAcqArray[selectedCameraId].Connected;
            tbCameraName.Text = CameraAcqArray[selectedCameraId].Name;
            tbArchiveCount.Text = CameraAcqArray[selectedCameraId].ArchiveImageCount.ToString();
            numArchiveIndex.Value = CameraAcqArray[selectedCameraId].ArchiveImageIndex;
            cbArchiveActive.Checked = CameraAcqArray[selectedCameraId].ArchiveImageActive;

            

            cbCameraIdSelected.Items.Clear();
            for (int i = 0; i < cameraCount; i++)
            {
                if (!String.IsNullOrEmpty(CameraAcqArray[i].Name)) { cbCameraIdSelected.Items.Add(CameraAcqArray[i].Name); }
                else { cbCameraIdSelected.Items.Add($"Empty{i}"); }
            }

            cbCameraIdSelected.SelectedIndex = 0;

            cbToolBlock.Items.Clear();
            for (int i = 0; i < toolCount; i++)
            {
                if (toolblockArray[0, i].FilePresent) { cbToolBlock.Items.Add(toolblockArray[0, i].Name); }
                else { cbToolBlock.Items.Add($"Empty{i}"); }
            }
            cbToolBlock.SelectedIndex = 0;
        }
        public void CheckLicense()
        {
            ExpireCount = 0;
            ExpireError = false;
            LicenseCheckList.Items.Clear();
            LicenseCheck = new CogStringCollection();
            LicenseCheck = CogLicense.GetLicensedFeatures(false, false);
            CogLicense.GetDaysRemaining(out ExpireCount, ExpireError);
            cogLicenseOk = LicenseCheck.Count > 0;

            tbExpireDate.Text = "License Expires in: " + ExpireCount.ToString() + " days";

            LicenseCheckList.BeginUpdate();
            foreach (string licenseString in LicenseCheck)
            {
                LicenseCheckList.Items.Add(licenseString);
            }
            LicenseCheckList.EndUpdate();
            LicenseCheckList.Height = LicenseCheckList.PreferredHeight;
        }
        public void resize_CameraControl()
        {
            int height;
            int width;
            int xMiddle;
            int right;
            int yMiddle;

            if (cameraConnectCount == 1)
            {

                Camera1Panel.Location = new Point(5, 5);

                height = this.tabImage.Height - (2 * Camera1Panel.Location.X);
                width = this.tabImage.Width - (2 * Camera1Panel.Location.Y);

                Camera1Panel.Height = height;
                Camera1Panel.Width = width;


                Camera2Panel.SendToBack();
                Camera3Panel.SendToBack();
                Camera4Panel.SendToBack();
                Camera5Panel.SendToBack();
                Camera6Panel.SendToBack();


            }
            else if (cameraConnectCount == 2)
            {
                height = (this.tabImage.Height - (2 * Camera1Panel.Location.X));
                width = (this.tabImage.Width - (3 * Camera1Panel.Location.Y)) / 2;

                right = 2 * Camera1Panel.Location.X + width;
                yMiddle = 2 * Camera1Panel.Location.Y + height;

                Camera1Panel.Height = height;
                Camera1Panel.Width = width;
                Camera1Panel.Location = new Point(5, 5);

                Camera2Panel.Height = height;
                Camera2Panel.Width = width;
                Camera2Panel.Location = new Point(right, 5);

                Camera3Panel.SendToBack();
                Camera4Panel.SendToBack();
                Camera5Panel.SendToBack();
                Camera6Panel.SendToBack();
            }
            else if (cameraConnectCount <= 4)
            {
                height = (this.tabImage.Height - (3 * Camera1Panel.Location.X)) / 2;
                width = (this.tabImage.Width - (3 * Camera1Panel.Location.Y)) / 2;

                right = 2 * Camera1Panel.Location.X + width;
                yMiddle = 2 * Camera1Panel.Location.Y + height;

                Camera1Panel.Height = height;
                Camera1Panel.Width = width;
                Camera1Panel.Location = new Point(5, 5);

                Camera2Panel.Height = height;
                Camera2Panel.Width = width;
                Camera2Panel.Location = new Point(right, 5);

                Camera3Panel.Height = height;
                Camera3Panel.Width = width;
                Camera3Panel.Location = new Point(5, yMiddle);

                Camera4Panel.Height = height;
                Camera4Panel.Width = width;
                Camera4Panel.Location = new Point(right, yMiddle);

                Camera5Panel.SendToBack();
                Camera6Panel.SendToBack();

            }
            else
            {
                height = (this.tabImage.Height - (3 * Camera1Panel.Location.X)) / 2;
                width = (this.tabImage.Width - (4 * Camera1Panel.Location.Y)) / 3;

                xMiddle = 2 * Camera1Panel.Location.X + width;
                right = (3 * Camera1Panel.Location.X) + (2 * width);

                yMiddle = 2 * Camera1Panel.Location.Y + height;

                Camera1Panel.Height = height;
                Camera1Panel.Width = width;
                Camera1Panel.Location = new Point(0, 0);

                Camera2Panel.Height = height;
                Camera2Panel.Width = width;
                Camera2Panel.Location = new Point(xMiddle, 0);

                Camera3Panel.Height = height;
                Camera3Panel.Width = width;
                Camera3Panel.Location = new Point(right, 0);

                Camera4Panel.Height = height;
                Camera4Panel.Width = width;
                Camera4Panel.Location = new Point(0, yMiddle);

                Camera5Panel.Height = height;
                Camera5Panel.Width = width;
                Camera5Panel.Location = new Point(xMiddle, yMiddle);

                Camera6Panel.Height = height;
                Camera6Panel.Width = width;
                Camera6Panel.Location = new Point(right, yMiddle);
            }
        }
        public void resize_tabToolBlock()
        {

            cogToolBlockEditV21.Location = new Point(5, panel7.Location.Y + panel7.Height + 5);

            cogToolBlockEditV21.Width = this.tabToolBlock.Width - cogToolBlockEditV21.Location.X - 5;
            cogToolBlockEditV21.Height = this.tabToolBlock.Height - cogToolBlockEditV21.Location.Y - 5;

        }
        public void resize_tabFileControl()
        {
        }
        public void resize_tabCameraData()
        {
            dgCameraData.Width = dgCameraData.PreferredSize.Width;
            dgCameraData.Height = this.taCameraData.Height - dgCameraData.Top - 10;

        }
        public void PlcRead()
        {
            
            //GENERAL COMMANDS
            int index = 0;
            PlcAutoMode = (MainPLC.PlcToPcControl[index] & (1 << 0)) != 0;
            recipe = MainPLC.PlcToPcControl[index] >> 16;

            systemIdle = true;
            systemIdle &= toolTrigger.All(x => x == false);
            systemIdle &= cameraSnap.All(x => x == false);
            systemIdle &= cameraSnapComplete.All(x => x == false);
            systemIdle &= !threadToolAlive;


            //CAMERA COMMANDS
            index = 1;
            for (int cam = 0; cam < cameraCount; cam++)
            {
                CameraAcqArray[cam].Trigger = systemIdle && ((MainPLC.PlcToPcControl[index + cam] & (1 << 0)) != 0);

                if ((MainPLC.PlcToPcControl[index + cam] & (1 << 1)) != 0) { CameraAbort(); }
                else { CameraAcqArray[cam].AbortTrigger = false; }

                plcTool[cam] = MainPLC.PlcToPcControl[index + cam] >> 16;
                ToolNumberUpdate(cam);

            }
        }
        public void PlcReadData()
        {
            int controlDataLoop;
            double dataInput;
            ToolBlock tool;



            for (int cam = 0; cam < cameraCount; cam++)
            {
                CameraAcqArray[cam].PartSerialNumber = MainPLC.PlcToPcString[cam];

                if (preProcessRequired)
                {
                    controlDataLoop = 1;
                    if (preProcess[cam].ToolReady) { preProcess[cam].Inputs[1].Value = MainPLC.PlcToPcControlData[cam * dataLength] / 10000; }                   
                    
                }
                else { controlDataLoop = 0; }

                int toolIndex = 0;
                tool = toolblockArray[cam, desiredTool[cam]];

                tool.PartSerialNumber = MainPLC.PlcToPcString[cam];

                if (tool.ToolReady == false) { continue; }

                for (int j = controlDataLoop; j < Math.Min(tool.Inputs.Count, dataLength); j++)
                {
                    dataInput = Convert.ToDouble(MainPLC.PlcToPcControlData[j + cam * dataLength])/10000;


                    while (toolIndex < tool.Inputs.Count && !Utilities.IsNumeric( tool.Inputs[toolIndex].ValueType.Name))
                    {
                        toolIndex++;
                    }
                    if (toolIndex >= toolblockArray[cam, desiredTool[cam]].Inputs.Count) { break; }
                    
                    if (tool.Inputs[toolIndex].ValueType.Name == "Double")
                    {
                        tool.Inputs[toolIndex].Value = dataInput;
                        toolIndex++;
                    }
                    else if (tool.Inputs[toolIndex].ValueType.Name == "Int32")
                    {
                        tool.Inputs[toolIndex].Value = dataInput;
                        toolIndex++;
                    }
                }
                tool.Inputs = toolblockArray[cam, desiredTool[cam]].Inputs;
            }
        }
        public void PlcWrite()
        {
            int index = 0;
            int tempTag = 0;

            //Camera Status: info related to camera and general system         
            tempTag |= ((heartBeat ? 1 : 0 )<< 0);
            tempTag |= ((cogLicenseOk ? 1:0 ) << 1);
            tempTag |= ((!systemIdle ? 1 : 0) << 2);

            tempTag |= recipeEcho << 16;

            MainPLC.PcToPlcStatus[index] = tempTag;
            index++;


            //map camera status 
            for (int cam = 0; cam < cameraCount; cam++)
            {
                tempTag = 0;

                tempTag |= ((CameraAcqArray[cam].Connected ? 1 : 0) << 0);
                tempTag |= ((CameraAcqArray[cam].TriggerAck ? 1 : 0) << 1);
                tempTag |= ((CameraAcqArray[cam].AbortTriggerAck ? 1 : 0) << 2);
                tempTag |= ((CameraAcqArray[cam].ImageReady ? 1 : 0) << 3);
                tempTag |= ((toolblockArray[cam, desiredTool[cam]].ToolReady ? 1 : 0) << 4);

                tempTag |= ((toolblockArray[cam, desiredTool[cam]].ResultUpdated ? 1 : 0) << 5);

                tempTag |= ((toolblockArray[cam, desiredTool[cam]].Result ? 1 : 0) << 6);
                tempTag |= ((toolblockArray[cam, desiredTool[cam]].FilePresent ? 1 : 0) << 7);
                tempTag |= ((CameraAcqArray[cam].Snapping ? 1 : 0) << 8);
                tempTag |= ((CameraAcqArray[cam].Acquiring ? 1 : 0) << 9);

                tempTag |= desiredTool[cam] << 16;

                MainPLC.PcToPlcStatus[index] = tempTag;
                index++;
            }
            
        }
        public void PlcWriteData()
        {

            string dataTypeName;
            double dData;
            int iData;
            ToolBlock tool;

            //Add Tool Data to String Array to send to PLC
            //Limit Doubles to 4 decimal places
            for (int i = 0; i < cameraCount; i++)
            {
                MainPLC.PcToPlcString[i] = CameraAcqArray[i].PartSerialNumber;

                tool = toolblockArray[i, desiredTool[i]];
                if (tool.ResultUpdated != tool.ResultUpdated_Mem)
                {
                    tool.ResultUpdated_Mem = tool.ResultUpdated;

                    Array.Clear(MainPLC.PcToPlcStatusData, i * dataLength, dataLength);
                    int writeIndex = 0;
                    for (int j = 0; j < Math.Min(tool.Outputs.Count, dataLength); j++)
                    {
                        if (tool.Outputs[j] == null) { break; }

                        dataTypeName = tool.Outputs[j].ValueType.Name;
                        if (!Utilities.IsNumeric(dataTypeName)) { continue; }
                        if (dataTypeName == "Double")
                        {
                            dData = Convert.ToDouble(tool.Outputs[j].Value);
                            iData = Convert.ToInt32(dData * 10000);

                            MainPLC.PcToPlcStatusData[(i * dataLength) + writeIndex] = iData;
                        }
                        else if (dataTypeName == "Int32")
                        {
                            MainPLC.PcToPlcStatusData[(i * dataLength) + j] = Convert.ToInt32(tool.Outputs[j].Value);
                        }
                        writeIndex++;
                    }
                }
            }
        }
        public void SaveSettings()
        {
            String KeyPath = Utilities.ExeFilePath + "\\RegisterKey";

            RegistryKey RegKey = Registry.CurrentUser.CreateSubKey(KeyPath);

            // Write config file name and location (server and resource)
            for (int i = 0; i < cameraCount; i++)
            {
                RegKey.SetValue($"Camera{i}_Name", CameraAcqArray[i].Name);
                RegKey.SetValue($"Camera{i}_Description", CameraAcqArray[i].Description);
                RegKey.SetValue($"Camera{i}_Server", CameraAcqArray[i].LoadServerSelect);
                RegKey.SetValue($"Camera{i}_Resource", CameraAcqArray[i].LoadResourceIndex);
                RegKey.SetValue($"Camera{i}CongfigFile", CameraAcqArray[i].ConfigFile);
                RegKey.SetValue($"Camera{i}SerialNumber", CameraAcqArray[i].SerialNumber);

                // Pre-Processing Data
                RegKey.SetValue($"PreProcess{i}_ToolBlock", preProcess[i].Name);

                for (int j = 0; j < toolCount; j++)
                {
                    RegKey.SetValue($"Camera{i}_ToolBlock{j}", toolblockArray[i, j].Name);
                }
            }

            //save IP address used for PLC
            RegKey.SetValue("MainPLC_IP1", Convert.ToInt32(numIP1.Value));
            RegKey.SetValue("MainPLC_IP2", Convert.ToInt32(numIP2.Value));
            RegKey.SetValue("MainPLC_IP3", Convert.ToInt32(numIP3.Value));
            RegKey.SetValue("MainPLC_IP4", Convert.ToInt32(numIP4.Value));

            RegKey.SetValue("MainPLC_BASETAG", MainPLC.BaseTag);

        }
        public void LoadSettings()
        {

            String KeyPath = Utilities.ExeFilePath + "\\RegisterKey";
            RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(KeyPath);
            if (RegKey != null)
            {
                // Read location (server and resource) and file name
                for (int i = 0; i < cameraCount; i++)
                {
                    CameraAcqArray[i].Name = RegKey.GetValue($"Camera{i}_Name", "").ToString();
                    CameraAcqArray[i].Description = RegKey.GetValue($"Camera{i}_Description", "").ToString();
                    CameraAcqArray[i].LoadServerSelect = RegKey.GetValue($"Camera{i}_Server", "").ToString();
                    CameraAcqArray[i].LoadResourceIndex = (int)RegKey.GetValue($"Camera{i}_Resource", 0);
                    CameraAcqArray[i].ConfigFile = RegKey.GetValue($"Camera{i}CongfigFile", "").ToString();
                    CameraAcqArray[i].ConfigFilePresent = System.IO.File.Exists(CameraAcqArray[i].ConfigFile);
                    CameraAcqArray[i].SerialNumber = RegKey.GetValue($"Camera{i}SerialNumber", "").ToString();

                    // Pre-Processing Data
                    preProcess[i].Name = (RegKey.GetValue($"PreProcess{i}_ToolBlock", "").ToString());

                    for (int j = 0; j < toolCount; j++)
                    {
                        toolblockArray[i, j].Name = (RegKey.GetValue($"Camera{i}_ToolBlock{j}", "").ToString());
                    }
                }

               

                //retreaving information for IP address
                numIP1.Value = (int)RegKey.GetValue("MainPLC_IP1", 0);
                numIP2.Value = (int)RegKey.GetValue("MainPLC_IP2", 0);
                numIP3.Value = (int)RegKey.GetValue("MainPLC_IP3", 0);
                numIP4.Value = (int)RegKey.GetValue("MainPLC_IP4", 0);

                MainPLC.BaseTag = RegKey.GetValue("MainPLC_BASETAG", "").ToString();

                RegKey.Close();

            }
        }
        public static bool CloseCancel()
        {
            const string message = "Are you sure that you would like to Close the Vision Application?";
            const string caption = "Close Vision Application";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                return true;
            else
                return false;
        }
        public void BuildDataGrid()
        {
            DataTable dt = new DataTable();
            List<ToolData>[] data = new List<ToolData>[cameraCount];

            for (int i = 0; i < cameraCount;i++)
            {
                data[i] = new List<ToolData>();
                data[i] = toolblockArray[i, desiredTool[i]].AllData;
                
            }
            if (data[0] == null) { return; }
            dt.Columns.Add("Tool Name", typeof(string));
            dt.Columns.Add("Data Name", typeof(string));
            for(int i = 0; i < cameraCount; i++)
            {
                dt.Columns.Add($"Camera {i} Data", typeof(string));
            }

            object[] rowArray = new object[cameraCount + 2];
            rowArray[0] = "Camera";
            rowArray[1] = "Serial Number";
            for (int i = 0; i < cameraCount; i++)
            {
                rowArray[i + 2] = CameraAcqArray[i].PartSerialNumber;
            }
            dt.Rows.Add(rowArray);

            rowArray = new object[cameraCount + 2];
            rowArray[0] = "Tool";
            rowArray[1] = "Serial Number";
            for (int i = 0; i < cameraCount; i++)
            {
                rowArray[i + 2] = toolblockArray[i, desiredTool[i]].PartSerialNumber;
            }
            dt.Rows.Add(rowArray);

            for (int i = 0; i < data[0].Count;i++)
            {
                rowArray = new object[cameraCount + 2];
                rowArray[0] = data[0][i].ToolName;
                rowArray[1] = data[0][i].Name;

                for (int j= 0; j < cameraCount; j++)
                {
                    if (data[j] == null || data[j].Count != data[0].Count) { rowArray[j + 2] = "n/a"; }
                    else { rowArray[j + 2] = data[j][i].Value.ToString(); }
                }

                dt.Rows.Add(rowArray);
            }

            dgCameraData.DataSource = dt;

            dt = null;
            data = null;

            resize_tabCameraData();

        }
        public void ComputerSetup()
        {
            string OP15_IP = "10.10.30.87";
            string OP45_55_IP = "10.10.30.88";
            string OP70_IP = "10.10.30.207";
            string OP90_IP = "10.10.30.208";

            string iP = Utilities.GetLocalIPAddress("10.10.30.");

            

            if (iP == OP15_IP)
            {
                StationNumber = 15;
                computerName = "OP15 Computer";
                cameraCount = 2;
                toolCount = 4;
                preProcessRequired = false;
            }
            else if(iP == OP45_55_IP)
            {
                StationNumber = 45;
                computerName = "OP45/55 Computer";
                cameraCount = 2;
                toolCount = 9;

                preProcessRequired = true;
            }
            else if (iP == OP70_IP)
            {
                StationNumber = 70;
                computerName = "OP70 Computer";
                cameraCount = 6;
                toolCount = 4;
                
                cameraImageFlip = 1;
                preProcessRequired = true;
            }
            else if (iP == OP90_IP)
            {
                StationNumber = 90;
                computerName = "OP90 Computer";
                cameraCount = 6;
                toolCount = 4;
                cameraImageFlip = 0;
                preProcessRequired = true;
            }
            else
            {
                computerName = "Unknown Computer";
                cameraCount = 6;
                toolCount = 2;
                preProcessRequired = true;
            }
        }
        private bool ThreadAlive(Task[] task)
        {
            bool alive = false;

            for (int i = 0; i < task.Length; i++)
            {
                if (task[i] != null) { alive |= (task[i].Status == TaskStatus.Running); }
                
            }

            return alive;
        }


    }
}

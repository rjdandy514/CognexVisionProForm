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

namespace CognexVisionProForm
{
    public partial class CognexVisionProForm
    {
        public int CameraSnap
        {
            set
            {
                cameraSnap[value] = true;
                ToolNumberUpdate(value);
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
        public int ToolBlockRunComplete
        {
            set
            {
                toolTriggerComplete[value] = true;
                BeginInvoke(new Set_ToolBlockUpdate(ToolBlockUpdate));
            }
        }
        public bool PlcCommsActive
        {
            get;set;
        }
        public void InitClasses()
        {
            pollingTimer = new Timer();
            pollingTimer.Tick += new EventHandler(pollingTimer_Tick);
            pollingTimer.Interval = 200; // in miliseconds


            for (int j = 0; j < cameraCount; j++)
            {
                splashScreen.UpdateProgress($"Initialize Camera {j}", 5);
                CameraAcqArray[j] = new DalsaImage(this);
                CameraAcqArray[j].Id = j;
                cameraControl[j] = new CameraControl(this, CameraAcqArray[j]);

                preProcess[j] = new ToolBlock(this);
                preProcess[j].CameraId = j;


                for (int i = 0; i < toolCount; i++)
                {
                    toolblockArray[j, i] = new ToolBlock(this);
                    toolblockArray[j, i].CameraId = j;
                }
            }

            MainPLC = new PlcComms(this);
            dataLength = MainPLC.PlcToPcControlData.Length / cameraCount;

            Utilities.LoggingStatment($"InitializeAcquisition Complete");
        }
        public void InitJobManager()
        {
            splashScreen.UpdateProgress("Initialize JobManager", 10);

            for (int cam = 0; cam <cameraCount; cam++)
            {

                if(preProcess[cam].FilePresent)
                {
                    splashScreen.UpdateProgress($"Initialize JobManager: {CameraAcqArray[cam].Name} - {preProcess[cam].Name} Toolblock", 1);
                    preProcess[cam].InitJobManager();
                }

                for (int i = 0; i < toolblockArray.GetLength(1); i++)
                {
                    if (toolblockArray[cam, i].FilePresent)
                    { 
                        splashScreen.UpdateProgress($"Initialize JobManager: {CameraAcqArray[cam].Name} - {toolblockArray[cam, i].Name} Toolblock", 1);
                        toolblockArray[cam, i].InitJobManager();
                    }
                }
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
        public void CameraAbort(int i)
        {
            cameraSnap[i] = false;
            cameraSnapComplete[i] = false;
            toolTrigger[i] = false;
            toolTriggerComplete[i] = false;
            CameraAcqArray[i].AbortTrigger = true;
            CameraAcqArray[i].AbortTrigger = false;
            CameraUpdate();
        }
        public void CameraUpdate()
        {
            // Run ToolBlocks that are enabled
            if (cameraSnap.SequenceEqual(cameraSnapComplete)) 
            {
                Array.Clear(toolTrigger, 0, toolTrigger.Length);
                Array.Clear(toolTriggerComplete, 0, toolTriggerComplete.Length);
                //if the cognex license is present, trigger Toolblock
                if (cogLicenseOk) { ToolBlockTrigger(); }
                else
                {
                    for (int i = 0; i < cameraCount; i++)
                    {
                        if (cameraSnapComplete[i])
                        {
                            cameraControl[i].UpdateDisplayRequest = true;
                        }
                    }
                }
            }

        }
        public void ToolNumberUpdate(int cam)
        {
                if (MainPLC.InitialCheck != null && MainPLC.InitialCheck.Status == IPStatus.Success && PlcCommsActive)
                {
                    if (Enumerable.Range(0, toolCount).Contains(plcTool[cam]))
                    {
                        desiredTool[cam] = plcTool[cam];
                    }
                    else { desiredTool[cam] = 0; }

                    cameraControl[cam].ToolSelect = desiredTool[cam];
                }
                else
                {
                    if (Enumerable.Range(0, toolCount).Contains(cameraControl[cam].ToolSelect))
                    {
                        desiredTool[cam] = cameraControl[cam].ToolSelect;
                    }
                    else
                    {
                        cameraControl[cam].ToolSelect = 0;
                        desiredTool[cam] = 0;
                    }
                }

        }
        
        public void ToolBlockTrigger()
        {
            CogImage8Grey processedImage;

            for (int j = 0; j < cameraCount; j++)
            {

                if (!toolTrigger[j] && cameraSnapComplete[j] && toolblockArray[j, desiredTool[j]].ToolReady)
                {
                    toolTrigger[j] = true;

                    if (preProcess[j].ToolReady && preProcessRequired)
                    {
                        preProcess[j].Inputs[1].Value = 2;
                        preProcess[j].ToolRun(CameraAcqArray[j].Image as CogImage8Grey);
                        
                        processedImage = preProcess[j].Outputs[0].Value as CogImage8Grey;
                    }
                    else { processedImage = CameraAcqArray[j].Image as CogImage8Grey; }

                    CogImage8Grey tempImage = toolblockArray[j, desiredTool[j]].toolBlock.Inputs[0].Value as CogImage8Grey;

                    toolblockArray[j, desiredTool[j]].ToolRun(processedImage as CogImage8Grey);


                }
            }
            Array.Clear(cameraSnap, 0, cameraSnap.Length);
            Array.Clear(cameraSnapComplete,0, cameraSnapComplete.Length);

        }
        private delegate void Set_ToolBlockUpdate();
        public void ToolBlockUpdate()
        {
            
            if (this.InvokeRequired)
            {
                BeginInvoke(new Set_ToolBlockUpdate(ToolBlockUpdate));
                return;
            }

            if(toolTrigger.SequenceEqual(toolTriggerComplete))
            {
                for (int i = 0; i < cameraCount; i++)
                {
                    if (toolTriggerComplete[i])
                    {
                        cameraControl[i].PreProcess = preProcess[i];
                        cameraControl[i].Tool = toolblockArray[i, desiredTool[i]];
                        cameraControl[i].UpdateImage();
                        cameraControl[i].UpdateDisplayRequest = true;

                    }
                }

                Array.Clear(toolTrigger,0, toolTrigger.Length);
                Array.Clear(toolTriggerComplete,0, toolTriggerComplete.Length);
            }
        }
        private delegate void Set_UpdateImageTab();
        public void UpdateFrameGrabberTab()
        {
            cbConfigFileFound.Checked = CameraAcqArray[selectedCameraId].ConfigFilePresent;
            cbCameraConnected.Checked = CameraAcqArray[selectedCameraId].Connected;
            tbCameraName.Text = CameraAcqArray[selectedCameraId].Name;
            tbArchiveCount.Text = CameraAcqArray[selectedCameraId].ArchiveImageCount.ToString();
            tbArchiveIndex.Text = CameraAcqArray[selectedCameraId].ArchiveImageIndex.ToString();
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
        public void PlcRead()
        {

            //GENERAL COMMANDS
            int index = 0;
            PlcAutoMode = (MainPLC.PlcToPcControl[index] & (1 << 0)) != 0;

            //CAMERA COMMANDS
            index = 1;
            for (int cam = 0; cam < cameraCount; cam++)
            {
                CameraAcqArray[cam].Trigger = (MainPLC.PlcToPcControl[index + cam] & (1 << 0)) != 0;
                
                if((MainPLC.PlcToPcControl[index + cam] & (1 << 1)) != 0){ CameraAbort(cam); }

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

                if (preProcessRequired)
                {
                    controlDataLoop = 1;
                    if (preProcess[cam].ToolReady) { preProcess[cam].Inputs[1].Value = MainPLC.PlcToPcControlData[cam * dataLength] / 10000; }                   
                    
                }
                else { controlDataLoop = 0; }

                int toolIndex = 0;
                tool = toolblockArray[cam, desiredTool[cam]];
                
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
                tempTag |= ((toolTrigger.All(x => x == false) ? 1 : 0) << 10);
                tempTag |= ((toolTriggerComplete.All(x => x == false) ? 1 : 0) << 11);
                tempTag |= ((cameraSnap.All(x => x == false) ? 1 : 0) << 12);
                tempTag |= ((cameraSnapComplete.All(x => x == false) ? 1 : 0) << 13);

                tempTag |= desiredTool[cam] << 16;

                MainPLC.PcToPlcStatus[index] = tempTag;
                index++;
            }
            bool temp = Array.TrueForAll(toolTrigger, value => { return value; });
            
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
                            MainPLC.PcToPlcStatusData[(i * dataLength) + j] = Convert.ToInt32(tool.Outputs[j].Value) * 10000;
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
        public void LoadForm(object Panel, object Form)
        {
            Panel p = Panel as Panel;
            if (p.Controls.Count > 0)
            {
                p.Controls.RemoveAt(0);
            }
            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            p.Controls.Add(f);
            p.Tag = f;
            f.Show();
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
                computerName = "OP15 Computer";
                cameraCount = 2;
                toolCount = 4;
                preProcessRequired = false;
            }
            else if(iP == OP45_55_IP)
            {
                computerName = "OP45/55 Computer";
                cameraCount = 2;
                toolCount = 8;
                preProcessRequired = true;
            }
            else if (iP == OP70_IP)
            {
                computerName = "OP70 Computer";
                cameraCount = 6;
                toolCount = 4;
                preProcessRequired = false;
            }
            else if (iP == OP90_IP)
            {
                computerName = "OP90 Computer";
                cameraCount = 6;
                toolCount = 4;
                preProcessRequired = false;
            }
            else
            {
                computerName = "Unknown Computer";
                cameraCount = 1;
                toolCount = 2;
                preProcessRequired = true;
            }
        }

    }
}

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
        public int ToolBlockRunComplete
        {
            set
            {

                toolTriggerComplete[value] = true;
                
                if (toolCompleteCount == toolRunCount)
                {
                    toolRunComplete = true;
                    ToolBlockUpdate();
                }

            }
        }
        public bool PlcCommsActive
        {
            get;set;
        }
        public void InitializeClasses()
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

                for (int i = 0; i < toolCount; i++)
                {
                    toolblockArray[j, i] = new ToolBlock(this);
                    toolblockArray[j, i].Id = i;
                    toolblockArray[j, i].CameraId = j;
                }
            }

            MainPLC = new PlcComms(this);

            Utilities.LoggingStatment($"InitializeAcquisition Complete");
        }
        public void InitializeJobManager()
        {
            splashScreen.UpdateProgress("Initialize JobManager", 10);

            for(int cam = 0; cam <cameraCount; cam++)
            {
                for (int i = 0; i < toolblockArray.GetLength(1); i++)
                {
                    if (toolblockArray[cam, i].FilePresent)
                    {
                        toolblockArray[cam, i].InitializeJobManager();
                    }
                }
            }
            
            splashScreen.Close();
        }
        public void InitializeServerList(int cameraIndex)
        {
            cbServerList.Items.Clear();
            for (int i = 0; i < SapManager.GetServerCount(); i++)
            {
                // Does this server support "Acq" (frame-grabber) or "AcqDevice" (camera)?
                bool bAcq = (CameraAcqArray[cameraIndex].ServerType == ServerCategory.ServerAcq || CameraAcqArray[cameraIndex].ServerType == ServerCategory.ServerAll) &&
                                (SapManager.GetResourceCount(i, SapManager.ResourceType.Acq) > 0);

                bool bAcqDevice = (CameraAcqArray[cameraIndex].ServerType == ServerCategory.ServerAcqDevice || CameraAcqArray[cameraIndex].ServerType == ServerCategory.ServerAll) &&
                                    (SapManager.GetResourceCount(i, SapManager.ResourceType.AcqDevice) > 0 &&
                                    (SapManager.GetResourceCount(i, SapManager.ResourceType.Acq) == 0));

                //Add all servers to the combobox list
                if (bAcq)
                {
                    cbServerList.Items.Add(new MyListBoxItem(SapManager.GetServerName(i), true));
                    
                }
                else if (bAcqDevice)
                {

                    cbServerList.Items.Add(new MyListBoxItem(SapManager.GetServerName(i), false));
                }
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
                cbServerList.Items.Add(new MyListBoxItem(ServerNotFound, false));
                cbServerList.SelectedIndex = 0;
            }

            CameraAcqArray[cameraIndex].LoadServerSelect = cbServerList.SelectedItem.ToString();
        }
        public void InitializeResourceList(int cameraIndex)
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
        public void CameraTrigger(int i)
        {
            if (CameraAcqArray[i].Connected)
            {
                CameraAcqArray[i].SnapPicture();
            }
            else if (CameraAcqArray[i].ArchiveImageActive)
            {
                CameraAcqArray[i].CreateBufferFromFile();
                CameraAcqArray[i].ArchiveImageIndex++;
            }
            else
            {
                MessageBox.Show("NO CAMERA IS CONNECTED");
                tabControl1.SelectedIndex = 1;
            }

        }
        public void CameraAbort(int i)
        {
            cameraSnap[i] = false;
            cameraSnapComplete[i] = false;
            toolTrigger[i] = false;
            toolTriggerComplete[i] = false;
        }
        public void CameraUpdate()
        {
            bool allCamerasComplete = true;

            if(!cameraSnap.SequenceEqual(cameraSnapComplete))
            {
                allCamerasComplete = false;
            }

            // Run ToolBlocks that are enabled
            if (allCamerasComplete) 
            {
                //if the cognex license is present, trigger Toolblock
                if (cogLicenseOk) { ToolBlockTrigger(); }
                else
                {
                    for (int i = 0; i < cameraCount; i++)
                    {
                        if (cameraSnapComplete[i])
                        {
                            cameraControl[i].UpdateImage();
                        }
                    }
                }
            }

        }
        public void ToolBlockTrigger()
        {
            toolRunComplete = false;
            toolRunCount = 0;
            toolCompleteCount = 0;

            for (int j = 0; j < cameraCount; j++)
            {
                if (MainPLC.InitialCheck != null && MainPLC.InitialCheck.Status == IPStatus.Success)
                {
                    if (Enumerable.Range(0, toolCount - 1).Contains(plcTool[j]))
                    {
                        desiredTool[j] = plcTool[j];
                    }
                    else { desiredTool[j] = 0; }
                }
                else
                {
                    if (Enumerable.Range(0, toolCount - 1).Contains(cameraControl[j].ToolSelect))
                    {
                        desiredTool[j] = cameraControl[j].ToolSelect;
                    }
                    else 
                    {
                        cameraControl[j].ToolSelect = 0;
                        desiredTool[j] = 0; 
                    }
                }

                if (CameraAcqArray[j].ImageReady && cameraSnapComplete[j] && toolblockArray[j, desiredTool[j]].ToolReady)
                {
                        toolTrigger[j] = true;
                        toolblockArray[j, desiredTool[j]].ToolRun(CameraAcqArray[j].Image as CogImage8Grey);
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

            bool allToolComplete = true;

            if (!toolTrigger.SequenceEqual(toolTriggerComplete))
            {
                allToolComplete = false;
            }


            if(allToolComplete)
            {
                for (int i = 0; i < cameraCount; i++)
                {
                    if (toolTriggerComplete[i])
                    {
                        cameraControl[i].Tool = toolblockArray[i, desiredTool[i]];
                        cameraControl[i].UpdateImage();
                        cameraControl[i].UpdateToolDisplay();

                    }
                }

                Array.Clear(toolTrigger,0, toolTrigger.Length);
                Array.Clear(toolTriggerComplete,0, toolTriggerComplete.Length);
            }
        }
        private delegate void Set_UpdateImageTab();
        public void UpdateImageTab()
        {


        }
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
            cogLicenseOk = ExpireCount > 0;

            tbExpireDate.Text = "License Expires in: " + ExpireCount.ToString() + " days";

            LicenseCheckList.BeginUpdate();
            foreach (string licenseString in LicenseCheck)
            {
                LicenseCheckList.Items.Add(licenseString);
            }
            LicenseCheckList.EndUpdate();
            LicenseCheckList.Height = LicenseCheckList.PreferredHeight;
        }
        public void resize_Tab00()
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
                CameraAcqArray[cam].AbortTrigger = (MainPLC.PlcToPcControl[index + cam] & (1 << 1)) != 0;

                
                int toolCheck = MainPLC.PlcToPcControl[index + cam] >> 16;

                if (Enumerable.Range(0, toolCount - 1).Contains(toolCheck))
                {
                    plcTool[cam] = toolCheck;
                }
                else { plcTool[cam] = 0; }
                

            }


            double[] controlData = new double[dataLength];
            for (int cam = 0; cam < cameraCount; cam++)
            {
                for (int j = 0; j < controlData.Length; j++)
                {
                    if (CameraAcqArray[cam].Connected)
                    {
                        controlData[j] = MainPLC.PlcToPcControlData[j + cam * controlData.Length];
                    }
                    else
                    {
                        controlData[j] = 0;
                    }
                }
                toolblockArray[cam, desiredTool[cam]].ToolInput = controlData;
                
            }


        }
        public void PlcWrite()
        {
            
            string dataTypeName;
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
                tempTag |= ((CameraAcqArray[cam].Grabbing ? 1 : 0) << 9);

                tempTag |= desiredTool[cam] << 16;

                MainPLC.PcToPlcStatus[index] = tempTag;
                index++;
            }


            //Add Tool Data to String Array to send to PLC
            //Limit each tool to 4 data points
            //Limit Doubles to 2 decimal places
            for(int i = 0; i< cameraCount;i++)
            {

                ToolBlock tool = toolblockArray[i, desiredTool[i]];

                if (tool.ResultUpdated)
                {
                    for(int j = 0;j < Math.Min(tool.ToolOutput.Length, dataLength);j++)
                    {
                       
                        if(tool.ToolOutput[j] == null || tool.RunStatus.Result == CogToolResultConstants.Error)
                        {
                            break;
                        }

                        dataTypeName = tool.ToolOutput[j].Value.GetType().Name;

                        if (dataTypeName == "Double")
                        {
                            double dData = Convert.ToDouble(tool.ToolOutput[j].Value);
                            int iData = Convert.ToInt32(dData * 100);

                            MainPLC.PcToPlcStatusData[j] = iData;
                        }
                        else if (dataTypeName == "Int32")
                        {
                            MainPLC.PcToPlcStatusData[j] = Convert.ToInt32(tool.ToolOutput[j].Value);
                        }

                    }
                }
            }
        }
        public void SaveSettings()
        {
            MyListBoxItem serverListSelected = (MyListBoxItem)cbServerList.SelectedItem;
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
            }
            else if(iP == OP45_55_IP)
            {
                computerName = "OP45/55 Computer";
                cameraCount = 2;
                toolCount = 4;
            }
            else if (iP == OP70_IP)
            {
                computerName = "OP70 Computer";
                cameraCount = 6;
                toolCount = 4;
            }
            else if (iP == OP90_IP)
            {
                computerName = "OP90 Computer";
                cameraCount = 6;
                toolCount = 4;
            }
            else
            {
                computerName = "Unknown Computer";
                cameraCount = 6;
                toolCount = 4;
            }
        }

    }
}

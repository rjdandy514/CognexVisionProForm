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
using System.IO;
using Cognex.Vision.Implementation;
using Microsoft.Win32;
using static DalsaImage;
using System.Reflection;
using System.Drawing;

namespace CognexVisionProForm
{
    public partial class Form1
    {
        public bool CameraSnapComplete
        {
            set
            {
                cameraSnapComplete = value;
                CameraUpdate();
                cameraSnapComplete = false;

            }
        }
        public int ToolBlockRunComplete
        {
            set
            {
                toolBlockRunComplete = value;
                toolCompleteCount++;
                
                toolBlockRunComplete = 0;
            }
        }
        public void InitializeClasses()
        {
            

            pollingTimer = new Timer();
            pollingTimer.Tick += new EventHandler(pollingTimer_Tick);
            pollingTimer.Interval = 200; // in miliseconds

            Camera01Calc = new Calculations();

            for (int j = 0; j < cameraCount; j++) 
            {
                CameraAcqArray[j] = new DalsaImage(this);
                CameraAcqArray[j].Id = j;
                cameraControl[j] = new CameraControl(this, CameraAcqArray[j]);

                for (int i = 0;i < toolCount; i++)
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
            for (int i = 0; i < toolblockArray.GetLength(1); i++) 
            { 
                if(toolblockArray[0,i].FilePresent)
                {
                    toolblockArray[0,i].InitializeJobManager();
                }
                
            }
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
            else if(cbServerList.Items.Count > 0)
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
                    string AcqName = SapManager.GetResourceName(CameraAcqArray[cameraIndex].LoadServerSelect, SapManager.ResourceType.AcqDevice, i);
                    if(SapManager.IsResourceAvailable(CameraAcqArray[cameraIndex].LoadServerSelect, SapManager.ResourceType.Acq, i))
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
                    if(SapManager.IsResourceAvailable(CameraAcqArray[cameraIndex].LoadServerSelect, SapManager.ResourceType.AcqDevice, i))
                    {
                        cbDeviceList.Items.Add(AcqDevice);
                    }
                    else
                    {
                        cbDeviceList.Items.Add("Not Available - Resource in Use");
                    }
                }
            }

            if(AcqCount == 0 && AcqDeviceCount == 0|| cbDeviceList.Items.Count == 0)
            {
                cbDeviceList.Items.Add("No Camera Found");
                CameraAcqArray[cameraIndex].LoadResourceIndex = 0;
            }
            
            cbDeviceList.SelectedIndex = CameraAcqArray[cameraIndex].LoadResourceIndex;
            CameraAcqArray[cameraIndex].LoadResourceName = cbDeviceList.SelectedItem.ToString();
        }
        public void CameraTrigger(int i)
        {
            if (CameraAcqArray[i].Connected || CameraAcqArray[i].ArchiveImageActive)
            {
                //DalsaImage.cs looks for rising edge of trigger
                CameraAcqArray[i].Trigger = true;
            }
            else
            {
                MessageBox.Show("NO CAMERA IS CONNECTED");
                tabControl1.SelectedIndex = 1;
            }

        }
        public void CameraAbort(int i)
        {
            if (CameraAcqArray[i].Connected)
            {
                CameraAcqArray[i].AbortTrigger = true;
            }
            else
            {
                MessageBox.Show("NO CAMERA IS CONNECTED");
                tabControl1.SelectedIndex = 1;
            }
        }
        public void CameraUpdate()
        {
            bool allCamerasComplete = true;
            for (int i = 0; i < cameraCount; i++)
            {
                if (CameraAcqArray[i].ImageReady)
                {
                    cameraControl[i].Image = CameraAcqArray[i].Image;
                    cameraControl[i].AcqTime = CameraAcqArray[i].AcqTime;
                }
                //if any camera that is connected is not ready exit method without 
                if((CameraAcqArray[i].Connected || CameraAcqArray[i].ArchiveImageActive) && !CameraAcqArray[i].ImageReady)
                {
                    allCamerasComplete = false;
                }
                
            }

            // Run ToolBlocks that are enabled
            if (allCamerasComplete) { ToolBlockTrigger(); }
            
           
        }
        public void ToolBlockTrigger()
        {
            
            toolRunComplete = false;
            toolRunCount = 0;
            toolCompleteCount = 0;

            for (int j = 0; j < cameraCount; j++)
            {
                if (CameraAcqArray[j].ImageReady)
                {
                    for (int i = 0; i < toolCount; i++)
                    {
                        toolblockArray[j, i].Enabled = true;
                        if(toolblockArray[j, i].Enabled && toolblockArray[j, i].ToolReady)
                        {
                            toolRunCount++;
                            if (i == toolCount - 1) { toolRunComplete = true; }
                            toolblockArray[j, i].ToolRun(cameraControl[j].Image as CogImage8Grey);
                        }
                    }
                }
                
            } 
        }
        private delegate void Set_ToolBlockUpdate(int number);
        public void ToolBlockUpdate(int index)
        {
            if(this.InvokeRequired)
            {
                BeginInvoke(new Set_ToolBlockUpdate(ToolBlockUpdate),index);
                return;
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
                height = this.tabControl1.Height - 4;
                width = this.tabControl1.Width -4;

                Camera1Panel.Height = height;
                Camera1Panel.Width = width;
                Camera1Panel.Location = new Point(2, 2);

                Camera2Panel.SendToBack();
                Camera3Panel.SendToBack();
                Camera4Panel.SendToBack();
                Camera5Panel.SendToBack();
                Camera6Panel.SendToBack();


            }
            else if (cameraConnectCount < 4)
            {
                height = this.tabControl1.Height / 2 - 2;
                width = this.tabControl1.Width / 2 - 2;

                right = this.tabControl1.Width - width;

                yMiddle = this.tabControl1.Height - height;

                Camera1Panel.Height = height;
                Camera1Panel.Width = width;
                Camera1Panel.Location = new Point(0, 0);

                Camera2Panel.Height = height;
                Camera2Panel.Width = width;
                Camera2Panel.Location = new Point(right, 0);

                Camera3Panel.Height = height;
                Camera3Panel.Width = width;
                Camera3Panel.Location = new Point(0, yMiddle);

                Camera4Panel.Height = height;
                Camera4Panel.Width = width;
                Camera4Panel.Location = new Point(right, yMiddle);
                
                Camera5Panel.SendToBack();
                Camera6Panel.SendToBack();

            }
            else
            {
                height = this.tabControl1.Height / 2 - 2;
                width = this.tabControl1.Width / 3 - 2;

                xMiddle = this.tabControl1.Width / 2 - width / 2;
                right = this.tabControl1.Width - width;

                yMiddle = this.tabControl1.Height - height;

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
            cogToolBlockEditV21.Width = this.tabControl1.Width - 4;
            cogToolBlockEditV21.Height = this.tabControl1.Height - 42;
            cogToolBlockEditV21.Location = new Point(2, 40);

        }
        public void PlcRead()
        {
            int[] temp = new int[1];

            foreach(DalsaImage camera in CameraAcqArray)
            {
                temp[0] = MainPLC.PlcToPC_Data[camera.Id];
                CameraControl = new BitArray(temp);

                camera.Trigger = CameraControl[0];
                camera.AbortTrigger = CameraControl[1];

                foreach (ToolBlock tool in toolblockArray)
                {
                    if (camera.Id != tool.Id) { continue; }
                    tool.Enabled = CameraControl[tool.Id + 2];
                }
            }
        }
        public void PlcWrite()
        {
            int index = 0;
            int toolIndex = 0;
            CameraStatus = new BitArray(32, false);
            toolStatus = new BitArray(32, false);

            //Camera Status: info related to camera and general system
            CameraStatus[0] = heartBeat;
            CameraStatus[1] = CameraAcqArray[0].Connected;
            CameraStatus[2] = CameraAcqArray[1].Connected;
            CameraStatus[3] = CameraAcqArray[2].Connected;
            CameraStatus[4] = CameraAcqArray[3].Connected;
            CameraStatus[5] = CameraAcqArray[4].Connected;
            CameraStatus[6] = CameraAcqArray[5].Connected;
            CameraStatus[7] = cogLicenseOk;
            CameraStatus.CopyTo(MainPLC.PCtoPLC_Data, index);

            index = 1;
            toolIndex = 0;
            foreach (ToolBlock tool in toolblockArray)
            {
                toolStatus.SetAll(false);
                if (tool != null)
                {
                    //Tool Status: info related to Camera Tool 1
                    toolStatus[0] = tool.ToolReady;
                    toolStatus[1] = tool.ResultUpdated;
                    toolStatus[2] = tool.Result;
                    toolStatus[3] = tool.Enabled;
                    toolStatus[4] = tool.FilePresent;
                    toolStatus[5] = false;
                    toolStatus[6] = false;
                    toolStatus[7] = false;
                }
                toolStatus.CopyTo(MainPLC.PCtoPLC_Data, index + toolIndex);
                toolIndex++;
            }

            /*Loop through all tools for total time (SaMPLE CODE)
            foreach (ToolBlock tool in toolblockArray)
            {
                if (tool != null){MainPLC.PCtoPLC_Data[index + toolIndex] = Convert.ToInt32(tool.TotalTime * 100);}
                else { MainPLC.PCtoPLC_Data[index + toolIndex] = 0; }
                toolIndex++;
            }
            */
                
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
            }
            
            for(int i = 0;i<toolblockArray.GetLength(1); i++)
            {
                RegKey.SetValue($"Camera1_ToolBlock{i}", toolblockArray[0,i].Name);
            }

            //save IP address used for PLC
            RegKey.SetValue("MainPLC_IP1", Convert.ToInt32(numIP1.Value));
            RegKey.SetValue("MainPLC_IP2", Convert.ToInt32(numIP2.Value));
            RegKey.SetValue("MainPLC_IP3", Convert.ToInt32(numIP3.Value));
            RegKey.SetValue("MainPLC_IP4", Convert.ToInt32(numIP4.Value));

            RegKey.SetValue("MainPLC_READTAG", MainPLC.ReadTag);
            RegKey.SetValue("MainPLC_WRITETAG", MainPLC.WriteTag);

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
                }
                   
                for (int i = 0; i < toolblockArray.GetLength(1); i++)
                {
                    toolblockArray[0,i].Name = (RegKey.GetValue($"Camera1_ToolBlock{i}", "").ToString());
                }

                //retreaving information for IP address
                numIP1.Value = (int)RegKey.GetValue("MainPLC_IP1", 0);
                numIP2.Value = (int)RegKey.GetValue("MainPLC_IP2", 0);
                numIP3.Value = (int)RegKey.GetValue("MainPLC_IP3", 0);
                numIP4.Value = (int)RegKey.GetValue("MainPLC_IP4", 0);

                MainPLC.ReadTag = RegKey.GetValue("MainPLC_READTAG", "").ToString();
                MainPLC.WriteTag = RegKey.GetValue("MainPLC_WRITETAG", "").ToString();

                RegKey.Close();

            }
        }
        public void LoadForm(object Panel,object Form)
        {
            Panel p = Panel as Panel;
            if(p.Controls.Count > 0)
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

    }
}

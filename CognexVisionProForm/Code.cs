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

namespace CognexVisionProForm
{
    public partial class Form1
    {
        public string ExeFileExtension
        {
            get { return exeFileExtension; } 
        }
        public string ExeFileName
        {
            get { return exeFileName; }
        }
        public string ExeFilePath
        {
            get { return exeFilePath; }
        }
        public void InitializeAcquisition()
        {
            Trace.AutoFlush = true;
            Trace.Indent();

            pollingTimer = new Timer();
            pollingTimer.Tick += new EventHandler(pollingTimer_Tick);
            pollingTimer.Interval = 200; // in miliseconds

            Camera01Acq = new DalsaImage("Camera01", this);

            Camera01Calc = new Calculations();

            Camera01Tb01 = new ToolBlock("Camera 01 Toolblock 01", this);
            Camera01Tb02 = new ToolBlock("Camera 01 Toolblock 02", this);

            

            MainPLC = new PlcComms(this);

            LoggingStatment($"InitializeAcquisition Complete");
        }
        public void InitializeLogging()
        {
            string LogDir = exeFilePath + "\\LogFile\\";
            string ArchiveLogDir = LogDir + "Archive\\";

            System.IO.Directory.CreateDirectory(LogDir);
            System.IO.Directory.CreateDirectory(ArchiveLogDir);

            string LogFile = LogDir + "Log.log";

            //If file already exist, rename and move to archive folder
            if (System.IO.File.Exists(LogFile) == true)
            {
                DirectoryInfo LogeDirInfo = new DirectoryInfo(LogDir);
                double logSize = DirSize(LogeDirInfo);

                if (logSize > 10)
                {
                    MessageBox.Show(logSize.ToString());
                    Archive(LogFile);
                }
                else { MessageBox.Show($"Log File not large enough to archive: {logSize} MB"); }

            }
            //Create Trace Class for camera
            appListener = new TextWriterTraceListener(LogFile);
            Trace.Close();
            Trace.Listeners.Add(appListener);
            appListener.WriteLine("------------------------------------------------------------------------");
            appListener.WriteLine(" Log Initiated created: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
        }
        public void InitializeServerList()
        {
            cbServerList.Items.Clear();
            for (int i = 0; i < SapManager.GetServerCount(); i++)
            {
                // Does this server support "Acq" (frame-grabber) or "AcqDevice" (camera)?
                bool bAcq = (Camera01Acq.ServerType == ServerCategory.ServerAcq || Camera01Acq.ServerType == ServerCategory.ServerAll) &&
                                (SapManager.GetResourceCount(i, SapManager.ResourceType.Acq) > 0);

                bool bAcqDevice = (Camera01Acq.ServerType == ServerCategory.ServerAcqDevice || Camera01Acq.ServerType == ServerCategory.ServerAll) &&
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
            int ServerNameFoundIndex = cbServerList.FindString(m_ServerName, 0);
            if (!string.IsNullOrEmpty(m_ServerName) && ServerNameFoundIndex != -1)
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

            m_ServerName = cbServerList.SelectedItem.ToString();


        }
        public void InitializeResourceList()
        {
            int AcqCount = 0;
            int AcqDeviceCount = 0;

            cbDeviceList.Items.Clear();
            if(!string.IsNullOrEmpty(m_ServerName) && m_ServerName != ServerNotFound)
            {
                AcqCount = SapManager.GetResourceCount(m_ServerName, SapManager.ResourceType.Acq);
                AcqDeviceCount = SapManager.GetResourceCount(m_ServerName, SapManager.ResourceType.AcqDevice);
            }

            if (AcqCount > 0)
            {
                for (int i = 0; i < AcqCount; i++)
                {
                    string AcqName = SapManager.GetResourceName(m_ServerName, SapManager.ResourceType.AcqDevice, i);
                    cbDeviceList.Items.Add(AcqName);
                }
            }

            if (AcqDeviceCount > 0)
            {
                for (int i = 0; i < AcqDeviceCount; i++)
                {
                    string AcqDevice = SapManager.GetResourceName(m_ServerName, SapManager.ResourceType.AcqDevice, i);
                    cbDeviceList.Items.Add(AcqDevice);
                }
            }

            if(AcqCount == 0 && AcqDeviceCount == 0)
            {
                cbDeviceList.Items.Add("No Camera Found");
                m_ResourceIndex = 0;
            }


                cbDeviceList.SelectedIndex = m_ResourceIndex;

            
        }
        public void LoggingStatment(string Message)
        {
            appListener.Write(DateTime.Now.ToString("G") + ": ");
            appListener.WriteLine(Message);
        }
        delegate void Camera1TriggerToolBlockCallBack();
        public void Camera1Trigger()
        {
            if (cbCameraConnected.Checked)
            {
                Camera01Acq.SnapPicture();

                if(Camera01Acq.SaveImageSelected)
                {
                    Camera01Acq.SaveImageBMP();
                }
            }
            else
            {
                MessageBox.Show("NO CAMERA IS CONNECTED");
                tabControl1.SelectedIndex = 1;
            }
        }
        public void Camera1TriggerToolBlock()
        {
            ICogImage Camera01Image = Camera01Acq.MarshalToCogImage();

            Camera01Tb01.ToolRun(Camera01Image as CogImage8Grey);
            Camera01Tb02.ToolRun(Camera01Image as CogImage8Grey);

            if (this.cogDisplay1.InvokeRequired)
            {
                Camera1TriggerToolBlockCallBack d = new Camera1TriggerToolBlockCallBack(Camera1TriggerToolBlock);
                this.Invoke(d);
            }
            else
            {
                cogDisplay1.Image = Camera01Image;
                cogDisplay1.Fit();
                cogDisplay1.Width = Convert.ToInt16(Convert.ToDouble(Camera01Image.Width) * cogDisplay1.Zoom);
                cogDisplay1.Height = Convert.ToInt16(Convert.ToDouble(Camera01Image.Height) * cogDisplay1.Zoom);
            }
        }
        public void Camera1ToolBlockUpdate()
        {
            txtC1ImageTime.Text = Camera01Acq.AcqTime.ToString();
            if (Camera01Tb01.ResultUpdated)
            {
                lbToolData.Items.Clear();

                lbToolData.Items.Add(Camera01Tb01.ToolName);
                lbToolData.Items.Add(Camera01Tb01.RunStatus.Result.ToString());
                lbToolData.Items.Add(Camera01Tb01.RunStatus.TotalTime.ToString());

                for (int i = 0; i < Camera01Tb01.ToolOutput.Length; i++)
                {
                    lbToolData.Items.Add($"{Camera01Tb01.ToolOutput[i].Name} - {Camera01Tb01.ToolOutput[i].Value}");
                }
            }
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

            for (int i = 0; i < LicenseCheck.Count; i++)
            {
                LicenseCheckList.Items.Add(LicenseCheck[i]);
            }
        }
        public void Import(string DeviceID, String FileType, String Extension)
        {

            // get file from file explorer
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = ToolBlockLocation;
            openFileDialog1.Title = "Select Configuration File";
            openFileDialog1.ShowDialog();

            string newfigFile = openFileDialog1.FileName;
            string newfigFileExtension = Path.GetExtension(newfigFile);
            string newfigFileName = Path.GetFileNameWithoutExtension(newfigFile);
            string newfigFilePath = Path.GetDirectoryName(newfigFile);

            //CONFIRM SELECTED FILE IS CORRECT TYPE, IF NOT END WITH MESSAGE
            if (newfigFileExtension != Extension) 
            {
                MessageBox.Show("FILE DOES NOT HAVE CORRECT EXTENSION");
                return; 
            }

            string fullFilePath = exeFilePath + "\\" + DeviceID + "_" + FileType + Extension;

            if (File.Exists(fullFilePath)) 
            {
                Archive(fullFilePath);
            }

            File.Copy(newfigFile, fullFilePath);
        }
        public void Archive(string FullFilePath)
        {
            string fileExtension = Path.GetExtension(FullFilePath);
            string fileName = Path.GetFileNameWithoutExtension(FullFilePath);
            string filePath = Path.GetDirectoryName(FullFilePath);

            string archiveFolder = exeFilePath + "\\" + fileName;


            System.IO.Directory.CreateDirectory(archiveFolder);

            DirectoryInfo ArchiveDir = new DirectoryInfo(archiveFolder);
            double archiveSize = DirSize(ArchiveDir);

            MessageBox.Show($"{fileName} current size = {archiveSize} MB");

            string fullArchivePath = archiveFolder + "\\"+ "Archive_" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileExtension;

            File.Move(FullFilePath, fullArchivePath);

        }
        public double DirSize(DirectoryInfo Folder)
        {
            double size = 0;
            // Add file sizes.
            FileInfo[] fileArray = Folder.GetFiles();
            foreach (FileInfo file in fileArray)
            {
                size += file.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = Folder.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size/1000000.00;
        }
        public void SaveSettings()
        {
            MyListBoxItem serverListSelected = (MyListBoxItem)cbServerList.SelectedItem;
            String KeyPath = ExeFilePath + "\\RegisterKey";

            RegistryKey RegKey = Registry.CurrentUser.CreateSubKey(KeyPath);

            // Write config file name and location (server and resource)
            RegKey.SetValue("Camera1_Name", tbC1TB1Name.Text);
            RegKey.SetValue("Camera1_Server", serverListSelected.ToString());
            RegKey.SetValue("Camera1_Resource", cbDeviceList.SelectedIndex);

            //save IP address used for PLC
            RegKey.SetValue("MainPLC_IP1", Convert.ToInt32(numIP1.Value));
            RegKey.SetValue("MainPLC_IP2", Convert.ToInt32(numIP2.Value));
            RegKey.SetValue("MainPLC_IP3", Convert.ToInt32(numIP3.Value));
            RegKey.SetValue("MainPLC_IP4", Convert.ToInt32(numIP4.Value));
        }
        public void LoadSettings()
        {
            String KeyPath = ExeFilePath + "\\RegisterKey";
            RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(KeyPath);
            if (RegKey != null)
            {
                // Read location (server and resource) and file name
                tbC1TB1Name.Text = RegKey.GetValue("Camera1_Name", "").ToString();
                m_ServerName = RegKey.GetValue("Camera1_Server", "").ToString();
                m_ResourceIndex = (int)RegKey.GetValue("Camera1_Resource", 0);

                //retreaving information for IP address
                numIP1.Value = (int)RegKey.GetValue("MainPLC_IP1", 0);
                numIP2.Value = (int)RegKey.GetValue("MainPLC_IP2", 0);
                numIP3.Value = (int)RegKey.GetValue("MainPLC_IP3", 0);
                numIP4.Value = (int)RegKey.GetValue("MainPLC_IP4", 0);

                RegKey.Close();

            }
        }
    }
}

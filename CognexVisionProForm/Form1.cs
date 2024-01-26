﻿using Cognex.VisionPro;
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

namespace CognexVisionProForm
{
    public partial class Form1 : Form
    {

        private CogStringCollection LicenseCheck;

        private DalsaImage Camera01Acq;

        private string ToolBlockLocation;
        private int ExpireCount = 0;

        private bool ExpireError = false;
        private ToolBlock BlobCount;
        private ToolBlock BlobCount2;

        private Calculations Camera01Calc;

        private PlcComms MainPLC;

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

        public Form1()
        {
            InitializeComponent();
        }

        public void InitializeAcquisition()
        {
            Trace.AutoFlush = true;
            Trace.Indent();

            Camera01Calc = new Calculations();
            BlobCount = new ToolBlock("BlobCount", this);
            BlobCount2 = new ToolBlock("BlobCount2", this);
            Camera01Acq = new DalsaImage(this);
            MainPLC = new PlcComms(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Eclipse Vision Application";
            InitializeAcquisition();
            BlobCount.InitializeJobManager();
            BlobCount2.InitializeJobManager();

            txtArchive.Text = BlobCount.ArchiveDir;
            txtLogFile.Text = BlobCount.LogDir;
            txtLoadFile.Text = ToolBlockLocation;

            numIP1.Value = 10;
            numIP2.Value = 2;
            numIP3.Value = 4;
            numIP4.Value = 10;
        }
        private void btnLicenseCheck_Click(object sender, EventArgs e)
        {
            ExpireCount = 0;
            ExpireError = false;
            LicenseCheckList.Items.Clear();
            LicenseCheck = new CogStringCollection();
            LicenseCheck = CogLicense.GetLicensedFeatures(false, false);
            CogLicense.GetDaysRemaining(out ExpireCount, ExpireError);
            tbExpireDate.Text = "License Expires in: " + ExpireCount.ToString() + " days";
            for (int i = 0; i < LicenseCheck.Count; i++)
            {
                LicenseCheckList.Items.Add(LicenseCheck[i]);
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            BlobCount.Cleaning();
            Camera01Acq.Cleaning();
            MainPLC.Cleaning();
            cogToolBlockEditV21.Dispose();
        }
        private void bttnArchive_Click(object sender, EventArgs e)
        {
            Process.Start(txtArchive.Text);
        }
        private void bttnLog_Click(object sender, EventArgs e)
        {
            Process.Start(txtLogFile.Text);
        }
        private void bttnNewJob_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = ToolBlockLocation;
            openFileDialog1.Title = "Select To Job File to be Loaded";
            openFileDialog1.ShowDialog();

            txtLoadFile.Text = openFileDialog1.FileName;

            BlobCount.LoadvisionProject(openFileDialog1.FileName);
        }

        delegate void Camera1TriggerToolBlockCallBack();
        public void Camera1TriggerToolBlock()
        {
            
            SapFormat format = Camera01Acq.m_Buffers.XferParams.Format;
            int width = Camera01Acq.m_Buffers.Width;
            int height = Camera01Acq.m_Buffers.Height;
            
            ICogImage Camera01Image = BlobCount.MarshalToCogImage(Camera01Acq.BufferAddress, width, height, format);

            BlobCount.ToolRun(Camera01Image as CogImage8Grey);
            BlobCount2.ToolRun(Camera01Image as CogImage8Grey);

            
            if (this.cogDisplay1.InvokeRequired)
            {
                Camera1TriggerToolBlockCallBack d = new Camera1TriggerToolBlockCallBack(Camera1TriggerToolBlock);
                this.Invoke(d);
            }
            else
            {
                cogDisplay1.Image = Camera01Image;
                cogDisplay1.Fit();
                cogDisplay1.Width = Convert.ToInt16(Convert.ToDouble(width) * cogDisplay1.Zoom);
                cogDisplay1.Height = Convert.ToInt16(Convert.ToDouble(height) * cogDisplay1.Zoom);
            }
        }
        public void Camera1ToolBlockUpdate()
        {
            txtC1ImageTime.Text = Camera01Acq.AcqTime.ToString();
            if (BlobCount.ResultUpdated)
            {
                lbToolData.Items.Clear();

                lbToolData.Items.Add(BlobCount.ToolName);
                lbToolData.Items.Add(BlobCount.RunStatus.Result.ToString());
                lbToolData.Items.Add(BlobCount.RunStatus.TotalTime.ToString());

                for(int i =  0; i < BlobCount.ToolOutput.Length;i++)
                {
                    lbToolData.Items.Add($"{BlobCount.ToolOutput[i].Name} - {BlobCount.ToolOutput[i].Value}");
                }

                BlobCount.ResultUpdated = false;

            }
        }

        private void label11_Click(object sender, EventArgs e)
        {

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

        }

        private void cbServerList_DropDown(object sender, EventArgs e)
        {
            int i = 0;
            cbServerList.Items.Clear();
            for (i = 0; i < SapManager.GetServerCount(); i++)
            {
                string serverName = SapManager.GetServerName(i);
                cbServerList.Items.Add(serverName);
            }
        }

        private void cbDeviceList_DropDown(object sender, EventArgs e)
        {
            string SelectedServer = cbServerList.SelectedItem.ToString();
            cbDeviceList.Items.Clear();

            int AcqCount = SapManager.GetResourceCount(SelectedServer.ToString(), SapManager.ResourceType.Acq);
            int AcqDeviceCount = SapManager.GetResourceCount(SelectedServer.ToString(), SapManager.ResourceType.AcqDevice);

            if (AcqCount > 0)
            {
                for (int i = 0; i < AcqCount; i++)
                {
                    Console.WriteLine(SapManager.GetResourceName(SelectedServer.ToString(), SapManager.ResourceType.Acq, i));
                    string AcqName = SapManager.GetResourceName(SelectedServer.ToString(), SapManager.ResourceType.AcqDevice, i);
                    cbDeviceList.Items.Add(AcqName);
                }
            }

            if (AcqDeviceCount > 0)
            {
                for (int i = 0; i < AcqDeviceCount; i++)
                {
                    string AcqDevice = SapManager.GetResourceName(SelectedServer.ToString(), SapManager.ResourceType.AcqDevice, i);
                    cbDeviceList.Items.Add(AcqDevice);
                }
            }
        }

        private void bttnConnectCamera_Click(object sender, EventArgs e)
        {
            if (cbServerList.SelectedItem != null && cbDeviceList.SelectedItem != null)
            {
                Camera01Acq.CreateCamera(cbServerList.SelectedItem.ToString(), cbDeviceList.SelectedIndex);
            }
            if(Camera01Acq.m_Xfer != null)
            {
                if (Camera01Acq.m_Xfer.Connected && Camera01Acq.m_Xfer.Initialized)
                {
                    cbCameraConnected.Checked = true;
                }
                else
                {
                    MessageBox.Show("FAILED TO CONNECT SELECTED CAMERA");
                    cbCameraConnected.Checked = false;
                }
            }
            else
            {
                MessageBox.Show("SELECTED CAMERA IS NOT VALID");
            }
        }

        private void bttnC1Snap_Click(object sender, EventArgs e)
        {
            if(cbCameraConnected.Checked)
            {
                Camera01Acq.SnapPicture();
            }
            else
            {
                MessageBox.Show("NO CAMERA IS CONNECTED");
                tabControl1.SelectedIndex = 1;
            }
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

        private void cbCameraSelected_DragDrop(object sender, DragEventArgs e)
        {
            
        }

        private void cbCameraSelected_DropDown(object sender, EventArgs e)
        {
            cbCameraSelected.Items.Clear();
            cbCameraSelected.Items.Add(BlobCount.ToolName);
            cbCameraSelected.Items.Add(BlobCount2.ToolName);
        }

        private void cbCameraSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCameraSelected.SelectedItem.ToString() == BlobCount.ToolName)
            {
                cogToolBlockEditV21.Subject = BlobCount.cogToolBlock;
            }
            else if(cbCameraSelected.SelectedItem.ToString() == BlobCount2.ToolName)
            {
                cogToolBlockEditV21.Subject = BlobCount2.cogToolBlock;
            }
            if(cogToolBlockEditV21.Subject != null)
            {
                cogToolBlockEditV21.Subject.Run();
            }
        }

        private void bttnPLC_Click(object sender, EventArgs e)
        {
            
            MainPLC.InitializePlcComms(numIP1.Value.ToString(), numIP2.Value.ToString(), numIP3.Value.ToString(), numIP4.Value.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainPLC.WritePlcTag();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainPLC.ReadPlcTag();
        }
    }
}


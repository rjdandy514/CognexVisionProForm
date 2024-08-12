﻿using System;

namespace CognexVisionProForm
{
    partial class CognexVisionProForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        
       /// #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabPlcConnection = new System.Windows.Forms.TabPage();
            this.panel8 = new System.Windows.Forms.Panel();
            this.bttnPLC = new System.Windows.Forms.Button();
            this.cbHeartbeat = new System.Windows.Forms.CheckBox();
            this.numIP4 = new System.Windows.Forms.NumericUpDown();
            this.tbBaseTag = new System.Windows.Forms.TextBox();
            this.numIP3 = new System.Windows.Forms.NumericUpDown();
            this.label41 = new System.Windows.Forms.Label();
            this.numIP2 = new System.Windows.Forms.NumericUpDown();
            this.numIP1 = new System.Windows.Forms.NumericUpDown();
            this.label33 = new System.Windows.Forms.Label();
            this.bttnPlcPing = new System.Windows.Forms.Button();
            this.tbPlcPingResponse = new System.Windows.Forms.TextBox();
            this.tabToolBlock = new System.Windows.Forms.TabPage();
            this.cogToolBlockEditV21 = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.panel7 = new System.Windows.Forms.Panel();
            this.bttnToolBlockLoad = new System.Windows.Forms.Button();
            this.cbTBToolSelected = new System.Windows.Forms.ComboBox();
            this.cbTBCameraSelected = new System.Windows.Forms.ComboBox();
            this.label40 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.bttnUpdateImage = new System.Windows.Forms.Button();
            this.tabFileControl = new System.Windows.Forms.TabPage();
            this.panel10 = new System.Windows.Forms.Panel();
            this.txtArchive = new System.Windows.Forms.Label();
            this.txtLogFile = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bttnLog = new System.Windows.Forms.Button();
            this.bttnArchive = new System.Windows.Forms.Button();
            this.bttnOpenProject = new System.Windows.Forms.Button();
            this.tabLicenseCheck = new System.Windows.Forms.TabPage();
            this.panel11 = new System.Windows.Forms.Panel();
            this.tbExpireDate = new System.Windows.Forms.TextBox();
            this.btnLicenseCheck = new System.Windows.Forms.Button();
            this.LicenseCheckList = new System.Windows.Forms.ListBox();
            this.tabFrameGrabber = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbArchiveCount = new System.Windows.Forms.TextBox();
            this.tbArchiveIndex = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.bttnArchiveImage = new System.Windows.Forms.Button();
            this.cbArchiveActive = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cbDeviceList = new System.Windows.Forms.ComboBox();
            this.bttnC1Config = new System.Windows.Forms.Button();
            this.cbConfigFileReq = new System.Windows.Forms.CheckBox();
            this.cbServerList = new System.Windows.Forms.ComboBox();
            this.cbConfigFileFound = new System.Windows.Forms.CheckBox();
            this.bttnConnectCamera = new System.Windows.Forms.Button();
            this.cbCameraConnected = new System.Windows.Forms.CheckBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.tbCameraName = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.cbCameraIdSelected = new System.Windows.Forms.ComboBox();
            this.tbCameraDesc = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.bttnAutoConnect = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tbC1TB1Name = new System.Windows.Forms.TextBox();
            this.cbC1Tb1FileFound = new System.Windows.Forms.CheckBox();
            this.cbToolBlock = new System.Windows.Forms.ComboBox();
            this.bttnToolBlockFileSelect = new System.Windows.Forms.Button();
            this.tbToolBlockName = new System.Windows.Forms.TextBox();
            this.cbToolBlockEnabled = new System.Windows.Forms.CheckBox();
            this.label36 = new System.Windows.Forms.Label();
            this.tabImage = new System.Windows.Forms.TabPage();
            this.Camera1Panel = new System.Windows.Forms.Panel();
            this.Camera2Panel = new System.Windows.Forms.Panel();
            this.Camera3Panel = new System.Windows.Forms.Panel();
            this.Camera4Panel = new System.Windows.Forms.Panel();
            this.Camera5Panel = new System.Windows.Forms.Panel();
            this.Camera6Panel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPlcConnection.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIP4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP1)).BeginInit();
            this.tabToolBlock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).BeginInit();
            this.panel7.SuspendLayout();
            this.tabFileControl.SuspendLayout();
            this.panel10.SuspendLayout();
            this.tabLicenseCheck.SuspendLayout();
            this.panel11.SuspendLayout();
            this.tabFrameGrabber.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabImage.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPlcConnection
            // 
            this.tabPlcConnection.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tabPlcConnection.Controls.Add(this.panel8);
            this.tabPlcConnection.Location = new System.Drawing.Point(4, 29);
            this.tabPlcConnection.Name = "tabPlcConnection";
            this.tabPlcConnection.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlcConnection.Size = new System.Drawing.Size(1284, 716);
            this.tabPlcConnection.TabIndex = 6;
            this.tabPlcConnection.Text = "PLC Connection";
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel8.Controls.Add(this.tbPlcPingResponse);
            this.panel8.Controls.Add(this.bttnPlcPing);
            this.panel8.Controls.Add(this.label33);
            this.panel8.Controls.Add(this.numIP1);
            this.panel8.Controls.Add(this.numIP2);
            this.panel8.Controls.Add(this.label41);
            this.panel8.Controls.Add(this.numIP3);
            this.panel8.Controls.Add(this.tbBaseTag);
            this.panel8.Controls.Add(this.numIP4);
            this.panel8.Controls.Add(this.cbHeartbeat);
            this.panel8.Controls.Add(this.bttnPLC);
            this.panel8.Location = new System.Drawing.Point(5, 5);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(451, 210);
            this.panel8.TabIndex = 70;
            this.panel8.Tag = "pnCameraControl";
            // 
            // bttnPLC
            // 
            this.bttnPLC.Location = new System.Drawing.Point(8, 61);
            this.bttnPLC.Name = "bttnPLC";
            this.bttnPLC.Size = new System.Drawing.Size(182, 24);
            this.bttnPLC.TabIndex = 21;
            this.bttnPLC.Text = "CONNECT TO PLC";
            this.bttnPLC.UseVisualStyleBackColor = true;
            this.bttnPLC.Click += new System.EventHandler(this.bttnPLC_Click);
            // 
            // cbHeartbeat
            // 
            this.cbHeartbeat.AutoCheck = false;
            this.cbHeartbeat.Location = new System.Drawing.Point(296, 3);
            this.cbHeartbeat.Name = "cbHeartbeat";
            this.cbHeartbeat.Size = new System.Drawing.Size(118, 24);
            this.cbHeartbeat.TabIndex = 24;
            this.cbHeartbeat.Text = "Heartbeat";
            this.cbHeartbeat.CheckedChanged += new System.EventHandler(this.cbHeartbeat_CheckedChanged);
            // 
            // numIP4
            // 
            this.numIP4.Location = new System.Drawing.Point(243, 5);
            this.numIP4.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numIP4.Name = "numIP4";
            this.numIP4.Size = new System.Drawing.Size(47, 20);
            this.numIP4.TabIndex = 20;
            this.numIP4.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // tbBaseTag
            // 
            this.tbBaseTag.Location = new System.Drawing.Point(121, 91);
            this.tbBaseTag.Name = "tbBaseTag";
            this.tbBaseTag.Size = new System.Drawing.Size(209, 20);
            this.tbBaseTag.TabIndex = 34;
            // 
            // numIP3
            // 
            this.numIP3.Location = new System.Drawing.Point(190, 5);
            this.numIP3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numIP3.Name = "numIP3";
            this.numIP3.Size = new System.Drawing.Size(47, 20);
            this.numIP3.TabIndex = 19;
            this.numIP3.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(5, 94);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(56, 13);
            this.label41.TabIndex = 35;
            this.label41.Text = "Base Tag:";
            // 
            // numIP2
            // 
            this.numIP2.Location = new System.Drawing.Point(137, 5);
            this.numIP2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numIP2.Name = "numIP2";
            this.numIP2.Size = new System.Drawing.Size(47, 20);
            this.numIP2.TabIndex = 18;
            this.numIP2.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // numIP1
            // 
            this.numIP1.Location = new System.Drawing.Point(84, 5);
            this.numIP1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numIP1.Name = "numIP1";
            this.numIP1.Size = new System.Drawing.Size(47, 20);
            this.numIP1.TabIndex = 17;
            this.numIP1.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(5, 9);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(75, 13);
            this.label33.TabIndex = 16;
            this.label33.Text = "PLC IP Adress";
            // 
            // bttnPlcPing
            // 
            this.bttnPlcPing.Location = new System.Drawing.Point(8, 31);
            this.bttnPlcPing.Name = "bttnPlcPing";
            this.bttnPlcPing.Size = new System.Drawing.Size(182, 24);
            this.bttnPlcPing.TabIndex = 38;
            this.bttnPlcPing.Text = "PING TO PLC";
            this.bttnPlcPing.UseVisualStyleBackColor = true;
            this.bttnPlcPing.Click += new System.EventHandler(this.bttnPlcPing_Click);
            // 
            // tbPlcPingResponse
            // 
            this.tbPlcPingResponse.Location = new System.Drawing.Point(196, 33);
            this.tbPlcPingResponse.Name = "tbPlcPingResponse";
            this.tbPlcPingResponse.Size = new System.Drawing.Size(209, 20);
            this.tbPlcPingResponse.TabIndex = 39;
            // 
            // tabToolBlock
            // 
            this.tabToolBlock.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tabToolBlock.Controls.Add(this.panel7);
            this.tabToolBlock.Controls.Add(this.cogToolBlockEditV21);
            this.tabToolBlock.Location = new System.Drawing.Point(4, 29);
            this.tabToolBlock.Margin = new System.Windows.Forms.Padding(0);
            this.tabToolBlock.Name = "tabToolBlock";
            this.tabToolBlock.Padding = new System.Windows.Forms.Padding(5);
            this.tabToolBlock.Size = new System.Drawing.Size(1284, 716);
            this.tabToolBlock.TabIndex = 5;
            this.tabToolBlock.Text = "Tool Block";
            // 
            // cogToolBlockEditV21
            // 
            this.cogToolBlockEditV21.AllowDrop = true;
            this.cogToolBlockEditV21.ContextMenuCustomizer = null;
            this.cogToolBlockEditV21.Cursor = System.Windows.Forms.Cursors.Default;
            this.cogToolBlockEditV21.Location = new System.Drawing.Point(5, 39);
            this.cogToolBlockEditV21.Margin = new System.Windows.Forms.Padding(5);
            this.cogToolBlockEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogToolBlockEditV21.Name = "cogToolBlockEditV21";
            this.cogToolBlockEditV21.ShowNodeToolTips = true;
            this.cogToolBlockEditV21.Size = new System.Drawing.Size(1126, 676);
            this.cogToolBlockEditV21.SuspendElectricRuns = false;
            this.cogToolBlockEditV21.TabIndex = 19;
            this.cogToolBlockEditV21.Load += new System.EventHandler(this.cogToolBlockEditV21_Load);
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel7.Controls.Add(this.bttnUpdateImage);
            this.panel7.Controls.Add(this.label4);
            this.panel7.Controls.Add(this.label40);
            this.panel7.Controls.Add(this.cbTBCameraSelected);
            this.panel7.Controls.Add(this.cbTBToolSelected);
            this.panel7.Controls.Add(this.bttnToolBlockLoad);
            this.panel7.Location = new System.Drawing.Point(5, 5);
            this.panel7.Margin = new System.Windows.Forms.Padding(5);
            this.panel7.Name = "panel7";
            this.panel7.Padding = new System.Windows.Forms.Padding(5);
            this.panel7.Size = new System.Drawing.Size(887, 30);
            this.panel7.TabIndex = 70;
            this.panel7.Tag = "pnCameraControl";
            // 
            // bttnToolBlockLoad
            // 
            this.bttnToolBlockLoad.Location = new System.Drawing.Point(601, 3);
            this.bttnToolBlockLoad.Name = "bttnToolBlockLoad";
            this.bttnToolBlockLoad.Size = new System.Drawing.Size(102, 24);
            this.bttnToolBlockLoad.TabIndex = 20;
            this.bttnToolBlockLoad.Text = "Load Toolblock";
            this.bttnToolBlockLoad.UseVisualStyleBackColor = true;
            this.bttnToolBlockLoad.Click += new System.EventHandler(this.bttnToolBlockLoad_Click);
            // 
            // cbTBToolSelected
            // 
            this.cbTBToolSelected.FormattingEnabled = true;
            this.cbTBToolSelected.Location = new System.Drawing.Point(436, 5);
            this.cbTBToolSelected.Name = "cbTBToolSelected";
            this.cbTBToolSelected.Size = new System.Drawing.Size(120, 21);
            this.cbTBToolSelected.TabIndex = 21;
            // 
            // cbTBCameraSelected
            // 
            this.cbTBCameraSelected.FormattingEnabled = true;
            this.cbTBCameraSelected.Location = new System.Drawing.Point(113, 5);
            this.cbTBCameraSelected.Name = "cbTBCameraSelected";
            this.cbTBCameraSelected.Size = new System.Drawing.Size(120, 21);
            this.cbTBCameraSelected.TabIndex = 1;
            this.cbTBCameraSelected.SelectedIndexChanged += new System.EventHandler(this.cbTBCameraSelected_SelectedIndexChanged);
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(312, 9);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(103, 13);
            this.label40.TabIndex = 22;
            this.label40.Text = "Selected Tool Block";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Selected Camera";
            // 
            // bttnUpdateImage
            // 
            this.bttnUpdateImage.Location = new System.Drawing.Point(709, 3);
            this.bttnUpdateImage.Name = "bttnUpdateImage";
            this.bttnUpdateImage.Size = new System.Drawing.Size(128, 24);
            this.bttnUpdateImage.TabIndex = 23;
            this.bttnUpdateImage.Text = "Grab Updated Image";
            this.bttnUpdateImage.UseVisualStyleBackColor = true;
            this.bttnUpdateImage.Click += new System.EventHandler(this.bttnUpdateImage_Click);
            // 
            // tabFileControl
            // 
            this.tabFileControl.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tabFileControl.Controls.Add(this.panel10);
            this.tabFileControl.Location = new System.Drawing.Point(4, 29);
            this.tabFileControl.Name = "tabFileControl";
            this.tabFileControl.Padding = new System.Windows.Forms.Padding(3);
            this.tabFileControl.Size = new System.Drawing.Size(1284, 716);
            this.tabFileControl.TabIndex = 3;
            this.tabFileControl.Text = "File Control";
            // 
            // panel10
            // 
            this.panel10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel10.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel10.Controls.Add(this.bttnOpenProject);
            this.panel10.Controls.Add(this.bttnArchive);
            this.panel10.Controls.Add(this.bttnLog);
            this.panel10.Controls.Add(this.label1);
            this.panel10.Controls.Add(this.label2);
            this.panel10.Controls.Add(this.txtLogFile);
            this.panel10.Controls.Add(this.txtArchive);
            this.panel10.Location = new System.Drawing.Point(5, 5);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(1273, 170);
            this.panel10.TabIndex = 70;
            this.panel10.Tag = "pnCameraControl";
            // 
            // txtArchive
            // 
            this.txtArchive.AutoSize = true;
            this.txtArchive.Location = new System.Drawing.Point(216, 53);
            this.txtArchive.Name = "txtArchive";
            this.txtArchive.Size = new System.Drawing.Size(54, 13);
            this.txtArchive.TabIndex = 12;
            this.txtArchive.Text = "txtArchive";
            // 
            // txtLogFile
            // 
            this.txtLogFile.AutoSize = true;
            this.txtLogFile.Location = new System.Drawing.Point(216, 83);
            this.txtLogFile.Name = "txtLogFile";
            this.txtLogFile.Size = new System.Drawing.Size(52, 13);
            this.txtLogFile.TabIndex = 13;
            this.txtLogFile.Text = "txtLogFile";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(132, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Log File Folder:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(132, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Archive Folder:";
            // 
            // bttnLog
            // 
            this.bttnLog.Location = new System.Drawing.Point(3, 78);
            this.bttnLog.Name = "bttnLog";
            this.bttnLog.Size = new System.Drawing.Size(115, 23);
            this.bttnLog.TabIndex = 16;
            this.bttnLog.Text = "Open Log Folder";
            this.bttnLog.UseVisualStyleBackColor = true;
            this.bttnLog.Click += new System.EventHandler(this.bttnLog_Click);
            // 
            // bttnArchive
            // 
            this.bttnArchive.Location = new System.Drawing.Point(3, 48);
            this.bttnArchive.Name = "bttnArchive";
            this.bttnArchive.Size = new System.Drawing.Size(115, 23);
            this.bttnArchive.TabIndex = 15;
            this.bttnArchive.Text = "Open Archive Folder";
            this.bttnArchive.UseVisualStyleBackColor = true;
            this.bttnArchive.Click += new System.EventHandler(this.bttnArchive_Click);
            // 
            // bttnOpenProject
            // 
            this.bttnOpenProject.Location = new System.Drawing.Point(3, 19);
            this.bttnOpenProject.Name = "bttnOpenProject";
            this.bttnOpenProject.Size = new System.Drawing.Size(115, 23);
            this.bttnOpenProject.TabIndex = 17;
            this.bttnOpenProject.Text = "Open Project Folder";
            this.bttnOpenProject.UseVisualStyleBackColor = true;
            this.bttnOpenProject.Click += new System.EventHandler(this.bttnOpenProject_Click);
            // 
            // tabLicenseCheck
            // 
            this.tabLicenseCheck.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tabLicenseCheck.Controls.Add(this.panel11);
            this.tabLicenseCheck.Location = new System.Drawing.Point(4, 29);
            this.tabLicenseCheck.Name = "tabLicenseCheck";
            this.tabLicenseCheck.Padding = new System.Windows.Forms.Padding(3);
            this.tabLicenseCheck.Size = new System.Drawing.Size(1284, 716);
            this.tabLicenseCheck.TabIndex = 2;
            this.tabLicenseCheck.Text = "LicenseCheck";
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel11.Controls.Add(this.LicenseCheckList);
            this.panel11.Controls.Add(this.btnLicenseCheck);
            this.panel11.Controls.Add(this.tbExpireDate);
            this.panel11.Location = new System.Drawing.Point(5, 5);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(375, 714);
            this.panel11.TabIndex = 70;
            this.panel11.Tag = "pnCameraControl";
            // 
            // tbExpireDate
            // 
            this.tbExpireDate.Location = new System.Drawing.Point(106, 7);
            this.tbExpireDate.Name = "tbExpireDate";
            this.tbExpireDate.ReadOnly = true;
            this.tbExpireDate.Size = new System.Drawing.Size(259, 20);
            this.tbExpireDate.TabIndex = 2;
            // 
            // btnLicenseCheck
            // 
            this.btnLicenseCheck.Location = new System.Drawing.Point(5, 5);
            this.btnLicenseCheck.Name = "btnLicenseCheck";
            this.btnLicenseCheck.Size = new System.Drawing.Size(97, 24);
            this.btnLicenseCheck.TabIndex = 5;
            this.btnLicenseCheck.Text = "License Check";
            this.btnLicenseCheck.UseVisualStyleBackColor = true;
            this.btnLicenseCheck.Click += new System.EventHandler(this.btnLicenseCheck_Click);
            // 
            // LicenseCheckList
            // 
            this.LicenseCheckList.Location = new System.Drawing.Point(5, 35);
            this.LicenseCheckList.Name = "LicenseCheckList";
            this.LicenseCheckList.Size = new System.Drawing.Size(360, 628);
            this.LicenseCheckList.TabIndex = 1;
            // 
            // tabFrameGrabber
            // 
            this.tabFrameGrabber.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tabFrameGrabber.Controls.Add(this.panel3);
            this.tabFrameGrabber.Controls.Add(this.panel4);
            this.tabFrameGrabber.Controls.Add(this.panel2);
            this.tabFrameGrabber.Location = new System.Drawing.Point(4, 29);
            this.tabFrameGrabber.Margin = new System.Windows.Forms.Padding(5);
            this.tabFrameGrabber.Name = "tabFrameGrabber";
            this.tabFrameGrabber.Padding = new System.Windows.Forms.Padding(5);
            this.tabFrameGrabber.Size = new System.Drawing.Size(1284, 716);
            this.tabFrameGrabber.TabIndex = 0;
            this.tabFrameGrabber.Text = "FrameGrabber";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2.Controls.Add(this.cbArchiveActive);
            this.panel2.Controls.Add(this.bttnArchiveImage);
            this.panel2.Controls.Add(this.label34);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tbArchiveIndex);
            this.panel2.Controls.Add(this.tbArchiveCount);
            this.panel2.Location = new System.Drawing.Point(936, 5);
            this.panel2.Margin = new System.Windows.Forms.Padding(5);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(225, 250);
            this.panel2.TabIndex = 31;
            this.panel2.Tag = "pnCameraControl";
            // 
            // tbArchiveCount
            // 
            this.tbArchiveCount.Location = new System.Drawing.Point(140, 96);
            this.tbArchiveCount.Name = "tbArchiveCount";
            this.tbArchiveCount.Size = new System.Drawing.Size(76, 20);
            this.tbArchiveCount.TabIndex = 28;
            // 
            // tbArchiveIndex
            // 
            this.tbArchiveIndex.Location = new System.Drawing.Point(140, 67);
            this.tbArchiveIndex.Name = "tbArchiveIndex";
            this.tbArchiveIndex.Size = new System.Drawing.Size(76, 20);
            this.tbArchiveIndex.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Archive Index:";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(8, 100);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(109, 13);
            this.label34.TabIndex = 30;
            this.label34.Text = "Archive Image Count:";
            // 
            // bttnArchiveImage
            // 
            this.bttnArchiveImage.Location = new System.Drawing.Point(48, 9);
            this.bttnArchiveImage.Name = "bttnArchiveImage";
            this.bttnArchiveImage.Size = new System.Drawing.Size(106, 24);
            this.bttnArchiveImage.TabIndex = 26;
            this.bttnArchiveImage.Text = "Use Archived Images";
            this.bttnArchiveImage.UseVisualStyleBackColor = true;
            this.bttnArchiveImage.Click += new System.EventHandler(this.bttnArchiveImage_Click);
            // 
            // cbArchiveActive
            // 
            this.cbArchiveActive.AutoCheck = false;
            this.cbArchiveActive.Location = new System.Drawing.Point(6, 38);
            this.cbArchiveActive.Name = "cbArchiveActive";
            this.cbArchiveActive.Size = new System.Drawing.Size(178, 24);
            this.cbArchiveActive.TabIndex = 31;
            this.cbArchiveActive.Text = "Using Archive Image Selected";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel4.Controls.Add(this.bttnAutoConnect);
            this.panel4.Controls.Add(this.label39);
            this.panel4.Controls.Add(this.tbCameraDesc);
            this.panel4.Controls.Add(this.cbCameraIdSelected);
            this.panel4.Controls.Add(this.label38);
            this.panel4.Controls.Add(this.label35);
            this.panel4.Controls.Add(this.tbCameraName);
            this.panel4.Controls.Add(this.label37);
            this.panel4.Controls.Add(this.label31);
            this.panel4.Controls.Add(this.cbCameraConnected);
            this.panel4.Controls.Add(this.bttnConnectCamera);
            this.panel4.Controls.Add(this.cbConfigFileFound);
            this.panel4.Controls.Add(this.cbServerList);
            this.panel4.Controls.Add(this.cbConfigFileReq);
            this.panel4.Controls.Add(this.bttnC1Config);
            this.panel4.Controls.Add(this.cbDeviceList);
            this.panel4.Location = new System.Drawing.Point(5, 5);
            this.panel4.Margin = new System.Windows.Forms.Padding(5);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(5);
            this.panel4.Size = new System.Drawing.Size(592, 250);
            this.panel4.TabIndex = 33;
            this.panel4.Tag = "pnCameraControl";
            // 
            // cbDeviceList
            // 
            this.cbDeviceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDeviceList.FormattingEnabled = true;
            this.cbDeviceList.Location = new System.Drawing.Point(120, 152);
            this.cbDeviceList.Name = "cbDeviceList";
            this.cbDeviceList.Size = new System.Drawing.Size(451, 21);
            this.cbDeviceList.TabIndex = 17;
            this.cbDeviceList.DragDrop += new System.Windows.Forms.DragEventHandler(this.cbCameraSelected_DropDown);
            // 
            // bttnC1Config
            // 
            this.bttnC1Config.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bttnC1Config.Location = new System.Drawing.Point(9, 218);
            this.bttnC1Config.Name = "bttnC1Config";
            this.bttnC1Config.Size = new System.Drawing.Size(106, 23);
            this.bttnC1Config.TabIndex = 20;
            this.bttnC1Config.Text = "Select ConfigFile";
            this.bttnC1Config.UseVisualStyleBackColor = true;
            this.bttnC1Config.Click += new System.EventHandler(this.bttnC1Config_Click);
            // 
            // cbConfigFileReq
            // 
            this.cbConfigFileReq.AutoCheck = false;
            this.cbConfigFileReq.Location = new System.Drawing.Point(9, 187);
            this.cbConfigFileReq.Name = "cbConfigFileReq";
            this.cbConfigFileReq.Size = new System.Drawing.Size(190, 24);
            this.cbConfigFileReq.TabIndex = 22;
            this.cbConfigFileReq.Text = "Configuration File Required";
            // 
            // cbServerList
            // 
            this.cbServerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbServerList.FormattingEnabled = true;
            this.cbServerList.Location = new System.Drawing.Point(120, 125);
            this.cbServerList.Name = "cbServerList";
            this.cbServerList.Size = new System.Drawing.Size(451, 21);
            this.cbServerList.TabIndex = 16;
            this.cbServerList.SelectedIndexChanged += new System.EventHandler(this.cbServerList_SelectedIndexChanged);
            // 
            // cbConfigFileFound
            // 
            this.cbConfigFileFound.AutoCheck = false;
            this.cbConfigFileFound.Location = new System.Drawing.Point(120, 217);
            this.cbConfigFileFound.Name = "cbConfigFileFound";
            this.cbConfigFileFound.Size = new System.Drawing.Size(147, 24);
            this.cbConfigFileFound.TabIndex = 23;
            this.cbConfigFileFound.Text = "Configuration File Found";
            // 
            // bttnConnectCamera
            // 
            this.bttnConnectCamera.Location = new System.Drawing.Point(9, 94);
            this.bttnConnectCamera.Name = "bttnConnectCamera";
            this.bttnConnectCamera.Size = new System.Drawing.Size(106, 24);
            this.bttnConnectCamera.TabIndex = 18;
            this.bttnConnectCamera.Text = "Connect to Camera";
            this.bttnConnectCamera.UseVisualStyleBackColor = true;
            this.bttnConnectCamera.Click += new System.EventHandler(this.bttnConnectCamera_Click);
            // 
            // cbCameraConnected
            // 
            this.cbCameraConnected.AutoCheck = false;
            this.cbCameraConnected.Location = new System.Drawing.Point(120, 94);
            this.cbCameraConnected.Name = "cbCameraConnected";
            this.cbCameraConnected.Size = new System.Drawing.Size(118, 24);
            this.cbCameraConnected.TabIndex = 19;
            this.cbCameraConnected.Text = "Camera Connected";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(9, 129);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(80, 13);
            this.label31.TabIndex = 30;
            this.label31.Text = "Camera Server:";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(9, 156);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(95, 13);
            this.label37.TabIndex = 31;
            this.label37.Text = "Camera Resource:";
            // 
            // tbCameraName
            // 
            this.tbCameraName.Location = new System.Drawing.Point(120, 40);
            this.tbCameraName.Name = "tbCameraName";
            this.tbCameraName.Size = new System.Drawing.Size(209, 20);
            this.tbCameraName.TabIndex = 32;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(9, 44);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(77, 13);
            this.label35.TabIndex = 33;
            this.label35.Text = "Camera Name:";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(9, 15);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(60, 13);
            this.label38.TabIndex = 34;
            this.label38.Text = "Camera ID:";
            // 
            // cbCameraIdSelected
            // 
            this.cbCameraIdSelected.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCameraIdSelected.FormattingEnabled = true;
            this.cbCameraIdSelected.Location = new System.Drawing.Point(120, 11);
            this.cbCameraIdSelected.Name = "cbCameraIdSelected";
            this.cbCameraIdSelected.Size = new System.Drawing.Size(210, 21);
            this.cbCameraIdSelected.TabIndex = 35;
            this.cbCameraIdSelected.SelectedIndexChanged += new System.EventHandler(this.cbCameraIdSelected_SelectedIndexChanged);
            // 
            // tbCameraDesc
            // 
            this.tbCameraDesc.Location = new System.Drawing.Point(120, 67);
            this.tbCameraDesc.Name = "tbCameraDesc";
            this.tbCameraDesc.Size = new System.Drawing.Size(209, 20);
            this.tbCameraDesc.TabIndex = 37;
            this.tbCameraDesc.Leave += new System.EventHandler(this.tbCamersDesc_Leave);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(9, 71);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(102, 13);
            this.label39.TabIndex = 38;
            this.label39.Text = "Camera Description:";
            // 
            // bttnAutoConnect
            // 
            this.bttnAutoConnect.Location = new System.Drawing.Point(346, 11);
            this.bttnAutoConnect.Name = "bttnAutoConnect";
            this.bttnAutoConnect.Size = new System.Drawing.Size(225, 24);
            this.bttnAutoConnect.TabIndex = 39;
            this.bttnAutoConnect.Text = "Auto Connect All Cameras";
            this.bttnAutoConnect.UseVisualStyleBackColor = true;
            this.bttnAutoConnect.Click += new System.EventHandler(this.bttnAutoConnect_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel3.Controls.Add(this.label36);
            this.panel3.Controls.Add(this.cbToolBlockEnabled);
            this.panel3.Controls.Add(this.tbToolBlockName);
            this.panel3.Controls.Add(this.bttnToolBlockFileSelect);
            this.panel3.Controls.Add(this.cbToolBlock);
            this.panel3.Controls.Add(this.cbC1Tb1FileFound);
            this.panel3.Controls.Add(this.tbC1TB1Name);
            this.panel3.Location = new System.Drawing.Point(599, 5);
            this.panel3.Margin = new System.Windows.Forms.Padding(5);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5);
            this.panel3.Size = new System.Drawing.Size(335, 250);
            this.panel3.TabIndex = 36;
            // 
            // tbC1TB1Name
            // 
            this.tbC1TB1Name.Location = new System.Drawing.Point(126, 11);
            this.tbC1TB1Name.Name = "tbC1TB1Name";
            this.tbC1TB1Name.Size = new System.Drawing.Size(203, 20);
            this.tbC1TB1Name.TabIndex = 34;
            // 
            // cbC1Tb1FileFound
            // 
            this.cbC1Tb1FileFound.AutoCheck = false;
            this.cbC1Tb1FileFound.Location = new System.Drawing.Point(5, 38);
            this.cbC1Tb1FileFound.Name = "cbC1Tb1FileFound";
            this.cbC1Tb1FileFound.Size = new System.Drawing.Size(147, 24);
            this.cbC1Tb1FileFound.TabIndex = 24;
            this.cbC1Tb1FileFound.Text = "Tool Block Found";
            // 
            // cbToolBlock
            // 
            this.cbToolBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbToolBlock.FormattingEnabled = true;
            this.cbToolBlock.Location = new System.Drawing.Point(5, 67);
            this.cbToolBlock.Name = "cbToolBlock";
            this.cbToolBlock.Size = new System.Drawing.Size(324, 21);
            this.cbToolBlock.TabIndex = 35;
            this.cbToolBlock.SelectedIndexChanged += new System.EventHandler(this.cbToolBlock_SelectedIndexChanged);
            // 
            // bttnToolBlockFileSelect
            // 
            this.bttnToolBlockFileSelect.Location = new System.Drawing.Point(5, 10);
            this.bttnToolBlockFileSelect.Name = "bttnToolBlockFileSelect";
            this.bttnToolBlockFileSelect.Size = new System.Drawing.Size(106, 23);
            this.bttnToolBlockFileSelect.TabIndex = 21;
            this.bttnToolBlockFileSelect.Text = "Select New Job";
            this.bttnToolBlockFileSelect.UseVisualStyleBackColor = true;
            this.bttnToolBlockFileSelect.Click += new System.EventHandler(this.bttnToolBockFileSelect_Click);
            // 
            // tbToolBlockName
            // 
            this.tbToolBlockName.Location = new System.Drawing.Point(109, 96);
            this.tbToolBlockName.Name = "tbToolBlockName";
            this.tbToolBlockName.Size = new System.Drawing.Size(210, 20);
            this.tbToolBlockName.TabIndex = 34;
            // 
            // cbToolBlockEnabled
            // 
            this.cbToolBlockEnabled.AutoCheck = false;
            this.cbToolBlockEnabled.Location = new System.Drawing.Point(126, 38);
            this.cbToolBlockEnabled.Name = "cbToolBlockEnabled";
            this.cbToolBlockEnabled.Size = new System.Drawing.Size(147, 24);
            this.cbToolBlockEnabled.TabIndex = 36;
            this.cbToolBlockEnabled.Text = "Tool  Block Enabled";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(5, 100);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(92, 13);
            this.label36.TabIndex = 35;
            this.label36.Text = "Tool Block Name:";
            // 
            // tabImage
            // 
            this.tabImage.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tabImage.Controls.Add(this.Camera6Panel);
            this.tabImage.Controls.Add(this.Camera5Panel);
            this.tabImage.Controls.Add(this.Camera4Panel);
            this.tabImage.Controls.Add(this.Camera3Panel);
            this.tabImage.Controls.Add(this.Camera2Panel);
            this.tabImage.Controls.Add(this.Camera1Panel);
            this.tabImage.Location = new System.Drawing.Point(4, 29);
            this.tabImage.Name = "tabImage";
            this.tabImage.Padding = new System.Windows.Forms.Padding(5);
            this.tabImage.Size = new System.Drawing.Size(1284, 716);
            this.tabImage.TabIndex = 1;
            this.tabImage.Text = "Single Camera Control";
            // 
            // Camera1Panel
            // 
            this.Camera1Panel.Location = new System.Drawing.Point(8, 6);
            this.Camera1Panel.Name = "Camera1Panel";
            this.Camera1Panel.Size = new System.Drawing.Size(420, 340);
            this.Camera1Panel.TabIndex = 23;
            // 
            // Camera2Panel
            // 
            this.Camera2Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Camera2Panel.Location = new System.Drawing.Point(430, 8);
            this.Camera2Panel.Name = "Camera2Panel";
            this.Camera2Panel.Size = new System.Drawing.Size(420, 340);
            this.Camera2Panel.TabIndex = 24;
            // 
            // Camera3Panel
            // 
            this.Camera3Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Camera3Panel.Location = new System.Drawing.Point(860, 0);
            this.Camera3Panel.Name = "Camera3Panel";
            this.Camera3Panel.Size = new System.Drawing.Size(420, 340);
            this.Camera3Panel.TabIndex = 25;
            // 
            // Camera4Panel
            // 
            this.Camera4Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Camera4Panel.Location = new System.Drawing.Point(2, 371);
            this.Camera4Panel.Name = "Camera4Panel";
            this.Camera4Panel.Size = new System.Drawing.Size(420, 340);
            this.Camera4Panel.TabIndex = 25;
            // 
            // Camera5Panel
            // 
            this.Camera5Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Camera5Panel.Location = new System.Drawing.Point(428, 374);
            this.Camera5Panel.Name = "Camera5Panel";
            this.Camera5Panel.Size = new System.Drawing.Size(420, 340);
            this.Camera5Panel.TabIndex = 26;
            // 
            // Camera6Panel
            // 
            this.Camera6Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Camera6Panel.Location = new System.Drawing.Point(856, 374);
            this.Camera6Panel.Name = "Camera6Panel";
            this.Camera6Panel.Size = new System.Drawing.Size(420, 340);
            this.Camera6Panel.TabIndex = 26;
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabImage);
            this.tabControl1.Controls.Add(this.tabFrameGrabber);
            this.tabControl1.Controls.Add(this.tabLicenseCheck);
            this.tabControl1.Controls.Add(this.tabFileControl);
            this.tabControl1.Controls.Add(this.tabToolBlock);
            this.tabControl1.Controls.Add(this.tabPlcConnection);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(5, 5);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1292, 749);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // CognexVisionProForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1292, 749);
            this.Controls.Add(this.tabControl1);
            this.IsMdiContainer = true;
            this.Name = "CognexVisionProForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.tabPlcConnection.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIP4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP1)).EndInit();
            this.tabToolBlock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.tabFileControl.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.tabLicenseCheck.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.tabFrameGrabber.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabImage.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabPage tabPlcConnection;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.TextBox tbPlcPingResponse;
        private System.Windows.Forms.Button bttnPlcPing;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.NumericUpDown numIP1;
        private System.Windows.Forms.NumericUpDown numIP2;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.NumericUpDown numIP3;
        private System.Windows.Forms.TextBox tbBaseTag;
        private System.Windows.Forms.NumericUpDown numIP4;
        private System.Windows.Forms.CheckBox cbHeartbeat;
        private System.Windows.Forms.Button bttnPLC;
        private System.Windows.Forms.TabPage tabToolBlock;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button bttnUpdateImage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.ComboBox cbTBCameraSelected;
        private System.Windows.Forms.ComboBox cbTBToolSelected;
        private System.Windows.Forms.Button bttnToolBlockLoad;
        private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cogToolBlockEditV21;
        private System.Windows.Forms.TabPage tabFileControl;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button bttnOpenProject;
        private System.Windows.Forms.Button bttnArchive;
        private System.Windows.Forms.Button bttnLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label txtLogFile;
        private System.Windows.Forms.Label txtArchive;
        private System.Windows.Forms.TabPage tabLicenseCheck;
        private System.Windows.Forms.Panel panel11;
        internal System.Windows.Forms.ListBox LicenseCheckList;
        private System.Windows.Forms.Button btnLicenseCheck;
        private System.Windows.Forms.TextBox tbExpireDate;
        private System.Windows.Forms.TabPage tabFrameGrabber;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.CheckBox cbToolBlockEnabled;
        private System.Windows.Forms.TextBox tbToolBlockName;
        private System.Windows.Forms.Button bttnToolBlockFileSelect;
        private System.Windows.Forms.ComboBox cbToolBlock;
        private System.Windows.Forms.CheckBox cbC1Tb1FileFound;
        private System.Windows.Forms.TextBox tbC1TB1Name;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button bttnAutoConnect;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.TextBox tbCameraDesc;
        private System.Windows.Forms.ComboBox cbCameraIdSelected;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox tbCameraName;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.CheckBox cbCameraConnected;
        private System.Windows.Forms.Button bttnConnectCamera;
        private System.Windows.Forms.CheckBox cbConfigFileFound;
        private System.Windows.Forms.ComboBox cbServerList;
        private System.Windows.Forms.CheckBox cbConfigFileReq;
        private System.Windows.Forms.Button bttnC1Config;
        private System.Windows.Forms.ComboBox cbDeviceList;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox cbArchiveActive;
        private System.Windows.Forms.Button bttnArchiveImage;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbArchiveIndex;
        private System.Windows.Forms.TextBox tbArchiveCount;
        private System.Windows.Forms.TabPage tabImage;
        private System.Windows.Forms.Panel Camera6Panel;
        private System.Windows.Forms.Panel Camera5Panel;
        private System.Windows.Forms.Panel Camera4Panel;
        private System.Windows.Forms.Panel Camera3Panel;
        private System.Windows.Forms.Panel Camera2Panel;
        private System.Windows.Forms.Panel Camera1Panel;
        private System.Windows.Forms.TabControl tabControl1;
    }
}


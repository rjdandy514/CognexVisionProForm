using System;

namespace CognexVisionProForm
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cbHeartbeat = new System.Windows.Forms.CheckBox();
            this.bttnPLC = new System.Windows.Forms.Button();
            this.numIP4 = new System.Windows.Forms.NumericUpDown();
            this.numIP3 = new System.Windows.Forms.NumericUpDown();
            this.numIP2 = new System.Windows.Forms.NumericUpDown();
            this.numIP1 = new System.Windows.Forms.NumericUpDown();
            this.label33 = new System.Windows.Forms.Label();
            this.tabToolBlock = new System.Windows.Forms.TabPage();
            this.cogToolBlockEditV21 = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.Camera1_DisplayStatusBar = new Cognex.VisionPro.CogDisplayStatusBarV2();
            this.label4 = new System.Windows.Forms.Label();
            this.cbCameraSelected = new System.Windows.Forms.ComboBox();
            this.tabPartData = new System.Windows.Forms.TabPage();
            this.VGR_DegOffset = new System.Windows.Forms.TextBox();
            this.VGR_YOffset = new System.Windows.Forms.TextBox();
            this.VGR_XOffset = new System.Windows.Forms.TextBox();
            this.PartData_YNom = new System.Windows.Forms.TextBox();
            this.PartData_XNom = new System.Windows.Forms.TextBox();
            this.PartData_YPosition = new System.Windows.Forms.TextBox();
            this.PartData_XPosition = new System.Windows.Forms.TextBox();
            this.PartData_Angle = new System.Windows.Forms.TextBox();
            this.F2_YPosition = new System.Windows.Forms.TextBox();
            this.F2_XPosition = new System.Windows.Forms.TextBox();
            this.F1_YPosition = new System.Windows.Forms.TextBox();
            this.F1_XPosition = new System.Windows.Forms.TextBox();
            this.FF_LDistance = new System.Windows.Forms.TextBox();
            this.FC_LDistance = new System.Windows.Forms.TextBox();
            this.FF_Angle = new System.Windows.Forms.TextBox();
            this.FF_YDistance = new System.Windows.Forms.TextBox();
            this.FF_XDistance = new System.Windows.Forms.TextBox();
            this.FC_Angle = new System.Windows.Forms.TextBox();
            this.FC_YDistance = new System.Windows.Forms.TextBox();
            this.FC_XDistance = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.RobotCalc = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.btnPartLocCalc = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.btnPartCalc = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabFileControl = new System.Windows.Forms.TabPage();
            this.bttnLog = new System.Windows.Forms.Button();
            this.bttnArchive = new System.Windows.Forms.Button();
            this.txtLogFile = new System.Windows.Forms.Label();
            this.txtArchive = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabLicenseCheck = new System.Windows.Forms.TabPage();
            this.btnLicenseCheck = new System.Windows.Forms.Button();
            this.tbExpireDate = new System.Windows.Forms.TextBox();
            this.LicenseCheckList = new System.Windows.Forms.ListBox();
            this.tabFrameGrabber = new System.Windows.Forms.TabPage();
            this.cbToolBlock = new System.Windows.Forms.ComboBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label35 = new System.Windows.Forms.Label();
            this.tbCameraName = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.cbCameraConnected = new System.Windows.Forms.CheckBox();
            this.bttnConnectCamera = new System.Windows.Forms.Button();
            this.cbConfigFileFound = new System.Windows.Forms.CheckBox();
            this.cbServerList = new System.Windows.Forms.ComboBox();
            this.cbConfigFileReq = new System.Windows.Forms.CheckBox();
            this.bttnC1Config = new System.Windows.Forms.Button();
            this.cbDeviceList = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bttnArchiveImage = new System.Windows.Forms.Button();
            this.label34 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbArchiveIndex = new System.Windows.Forms.TextBox();
            this.tbArchiveCount = new System.Windows.Forms.TextBox();
            this.tbC1TB1Name = new System.Windows.Forms.TextBox();
            this.cbC1Tb1FileFound = new System.Windows.Forms.CheckBox();
            this.bttnC1TB1FileSelect = new System.Windows.Forms.Button();
            this.tabImage = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bttnC1LogImages = new System.Windows.Forms.Button();
            this.lbToolData = new System.Windows.Forms.ListBox();
            this.label32 = new System.Windows.Forms.Label();
            this.bttnC1Abort = new System.Windows.Forms.Button();
            this.bttnC1Freeze = new System.Windows.Forms.Button();
            this.bttnC1Grab = new System.Windows.Forms.Button();
            this.bttnC1Snap = new System.Windows.Forms.Button();
            this.txtC1ImageTime = new System.Windows.Forms.Label();
            this.cogDisplay1 = new Cognex.VisionPro.Display.CogDisplay();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.cbArchiveActive = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIP4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP1)).BeginInit();
            this.tabToolBlock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).BeginInit();
            this.tabPartData.SuspendLayout();
            this.tabFileControl.SuspendLayout();
            this.tabLicenseCheck.SuspendLayout();
            this.tabFrameGrabber.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabImage.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogDisplay1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cbHeartbeat);
            this.tabPage1.Controls.Add(this.bttnPLC);
            this.tabPage1.Controls.Add(this.numIP4);
            this.tabPage1.Controls.Add(this.numIP3);
            this.tabPage1.Controls.Add(this.numIP2);
            this.tabPage1.Controls.Add(this.numIP1);
            this.tabPage1.Controls.Add(this.label33);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1082, 719);
            this.tabPage1.TabIndex = 6;
            this.tabPage1.Text = "PLC Connection";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cbHeartbeat
            // 
            this.cbHeartbeat.AutoCheck = false;
            this.cbHeartbeat.Location = new System.Drawing.Point(315, 12);
            this.cbHeartbeat.Name = "cbHeartbeat";
            this.cbHeartbeat.Size = new System.Drawing.Size(118, 24);
            this.cbHeartbeat.TabIndex = 24;
            this.cbHeartbeat.Text = "Heartbeat";
            // 
            // bttnPLC
            // 
            this.bttnPLC.Location = new System.Drawing.Point(25, 58);
            this.bttnPLC.Name = "bttnPLC";
            this.bttnPLC.Size = new System.Drawing.Size(182, 24);
            this.bttnPLC.TabIndex = 21;
            this.bttnPLC.Text = "CONNECT TO PLC";
            this.bttnPLC.UseVisualStyleBackColor = true;
            this.bttnPLC.Click += new System.EventHandler(this.bttnPLC_Click);
            // 
            // numIP4
            // 
            this.numIP4.Location = new System.Drawing.Point(262, 15);
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
            // numIP3
            // 
            this.numIP3.Location = new System.Drawing.Point(209, 15);
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
            // numIP2
            // 
            this.numIP2.Location = new System.Drawing.Point(156, 15);
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
            this.numIP1.Location = new System.Drawing.Point(103, 15);
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
            this.label33.Location = new System.Drawing.Point(22, 17);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(75, 13);
            this.label33.TabIndex = 16;
            this.label33.Text = "PLC IP Adress";
            // 
            // tabToolBlock
            // 
            this.tabToolBlock.Controls.Add(this.cogToolBlockEditV21);
            this.tabToolBlock.Controls.Add(this.Camera1_DisplayStatusBar);
            this.tabToolBlock.Controls.Add(this.label4);
            this.tabToolBlock.Controls.Add(this.cbCameraSelected);
            this.tabToolBlock.Location = new System.Drawing.Point(4, 22);
            this.tabToolBlock.Name = "tabToolBlock";
            this.tabToolBlock.Padding = new System.Windows.Forms.Padding(3);
            this.tabToolBlock.Size = new System.Drawing.Size(1082, 719);
            this.tabToolBlock.TabIndex = 5;
            this.tabToolBlock.Text = "Tool Block";
            this.tabToolBlock.UseVisualStyleBackColor = true;
            // 
            // cogToolBlockEditV21
            // 
            this.cogToolBlockEditV21.AllowDrop = true;
            this.cogToolBlockEditV21.ContextMenuCustomizer = null;
            this.cogToolBlockEditV21.Cursor = System.Windows.Forms.Cursors.Default;
            this.cogToolBlockEditV21.Location = new System.Drawing.Point(9, 33);
            this.cogToolBlockEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogToolBlockEditV21.Name = "cogToolBlockEditV21";
            this.cogToolBlockEditV21.ShowNodeToolTips = true;
            this.cogToolBlockEditV21.Size = new System.Drawing.Size(1046, 578);
            this.cogToolBlockEditV21.SuspendElectricRuns = false;
            this.cogToolBlockEditV21.TabIndex = 19;
            this.cogToolBlockEditV21.Load += new System.EventHandler(this.cogToolBlockEditV21_Load);
            // 
            // Camera1_DisplayStatusBar
            // 
            this.Camera1_DisplayStatusBar.CoordinateSpaceName = "*\\#";
            this.Camera1_DisplayStatusBar.CoordinateSpaceName3D = "*\\#";
            this.Camera1_DisplayStatusBar.Location = new System.Drawing.Point(6, 617);
            this.Camera1_DisplayStatusBar.Name = "Camera1_DisplayStatusBar";
            this.Camera1_DisplayStatusBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Camera1_DisplayStatusBar.Size = new System.Drawing.Size(1049, 22);
            this.Camera1_DisplayStatusBar.TabIndex = 17;
            this.Camera1_DisplayStatusBar.Use3DCoordinateSpaceTree = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Selected Camera";
            // 
            // cbCameraSelected
            // 
            this.cbCameraSelected.FormattingEnabled = true;
            this.cbCameraSelected.Location = new System.Drawing.Point(117, 6);
            this.cbCameraSelected.Name = "cbCameraSelected";
            this.cbCameraSelected.Size = new System.Drawing.Size(121, 21);
            this.cbCameraSelected.TabIndex = 1;
            this.cbCameraSelected.DropDown += new System.EventHandler(this.cbCameraSelected_DropDown);
            this.cbCameraSelected.SelectedIndexChanged += new System.EventHandler(this.cbCameraSelected_SelectedIndexChanged);
            // 
            // tabPartData
            // 
            this.tabPartData.Controls.Add(this.VGR_DegOffset);
            this.tabPartData.Controls.Add(this.VGR_YOffset);
            this.tabPartData.Controls.Add(this.VGR_XOffset);
            this.tabPartData.Controls.Add(this.PartData_YNom);
            this.tabPartData.Controls.Add(this.PartData_XNom);
            this.tabPartData.Controls.Add(this.PartData_YPosition);
            this.tabPartData.Controls.Add(this.PartData_XPosition);
            this.tabPartData.Controls.Add(this.PartData_Angle);
            this.tabPartData.Controls.Add(this.F2_YPosition);
            this.tabPartData.Controls.Add(this.F2_XPosition);
            this.tabPartData.Controls.Add(this.F1_YPosition);
            this.tabPartData.Controls.Add(this.F1_XPosition);
            this.tabPartData.Controls.Add(this.FF_LDistance);
            this.tabPartData.Controls.Add(this.FC_LDistance);
            this.tabPartData.Controls.Add(this.FF_Angle);
            this.tabPartData.Controls.Add(this.FF_YDistance);
            this.tabPartData.Controls.Add(this.FF_XDistance);
            this.tabPartData.Controls.Add(this.FC_Angle);
            this.tabPartData.Controls.Add(this.FC_YDistance);
            this.tabPartData.Controls.Add(this.FC_XDistance);
            this.tabPartData.Controls.Add(this.label30);
            this.tabPartData.Controls.Add(this.label29);
            this.tabPartData.Controls.Add(this.label27);
            this.tabPartData.Controls.Add(this.label28);
            this.tabPartData.Controls.Add(this.RobotCalc);
            this.tabPartData.Controls.Add(this.label25);
            this.tabPartData.Controls.Add(this.label26);
            this.tabPartData.Controls.Add(this.label24);
            this.tabPartData.Controls.Add(this.label21);
            this.tabPartData.Controls.Add(this.label22);
            this.tabPartData.Controls.Add(this.label23);
            this.tabPartData.Controls.Add(this.btnPartLocCalc);
            this.tabPartData.Controls.Add(this.label18);
            this.tabPartData.Controls.Add(this.label19);
            this.tabPartData.Controls.Add(this.label20);
            this.tabPartData.Controls.Add(this.label15);
            this.tabPartData.Controls.Add(this.label16);
            this.tabPartData.Controls.Add(this.label17);
            this.tabPartData.Controls.Add(this.label14);
            this.tabPartData.Controls.Add(this.label13);
            this.tabPartData.Controls.Add(this.btnPartCalc);
            this.tabPartData.Controls.Add(this.label8);
            this.tabPartData.Controls.Add(this.label9);
            this.tabPartData.Controls.Add(this.label12);
            this.tabPartData.Controls.Add(this.label11);
            this.tabPartData.Controls.Add(this.label10);
            this.tabPartData.Controls.Add(this.label7);
            this.tabPartData.Controls.Add(this.label6);
            this.tabPartData.Controls.Add(this.label5);
            this.tabPartData.Location = new System.Drawing.Point(4, 22);
            this.tabPartData.Name = "tabPartData";
            this.tabPartData.Padding = new System.Windows.Forms.Padding(3);
            this.tabPartData.Size = new System.Drawing.Size(1082, 719);
            this.tabPartData.TabIndex = 4;
            this.tabPartData.Text = "Part Data";
            this.tabPartData.UseVisualStyleBackColor = true;
            // 
            // VGR_DegOffset
            // 
            this.VGR_DegOffset.Location = new System.Drawing.Point(794, 77);
            this.VGR_DegOffset.Name = "VGR_DegOffset";
            this.VGR_DegOffset.ReadOnly = true;
            this.VGR_DegOffset.Size = new System.Drawing.Size(136, 20);
            this.VGR_DegOffset.TabIndex = 68;
            // 
            // VGR_YOffset
            // 
            this.VGR_YOffset.Location = new System.Drawing.Point(794, 53);
            this.VGR_YOffset.Name = "VGR_YOffset";
            this.VGR_YOffset.ReadOnly = true;
            this.VGR_YOffset.Size = new System.Drawing.Size(136, 20);
            this.VGR_YOffset.TabIndex = 65;
            // 
            // VGR_XOffset
            // 
            this.VGR_XOffset.Location = new System.Drawing.Point(794, 30);
            this.VGR_XOffset.Name = "VGR_XOffset";
            this.VGR_XOffset.ReadOnly = true;
            this.VGR_XOffset.Size = new System.Drawing.Size(136, 20);
            this.VGR_XOffset.TabIndex = 64;
            // 
            // PartData_YNom
            // 
            this.PartData_YNom.Location = new System.Drawing.Point(136, 537);
            this.PartData_YNom.Name = "PartData_YNom";
            this.PartData_YNom.Size = new System.Drawing.Size(136, 20);
            this.PartData_YNom.TabIndex = 60;
            // 
            // PartData_XNom
            // 
            this.PartData_XNom.Location = new System.Drawing.Point(136, 511);
            this.PartData_XNom.Name = "PartData_XNom";
            this.PartData_XNom.Size = new System.Drawing.Size(136, 20);
            this.PartData_XNom.TabIndex = 59;
            // 
            // PartData_YPosition
            // 
            this.PartData_YPosition.Location = new System.Drawing.Point(136, 584);
            this.PartData_YPosition.Name = "PartData_YPosition";
            this.PartData_YPosition.ReadOnly = true;
            this.PartData_YPosition.Size = new System.Drawing.Size(136, 20);
            this.PartData_YPosition.TabIndex = 56;
            // 
            // PartData_XPosition
            // 
            this.PartData_XPosition.Location = new System.Drawing.Point(136, 561);
            this.PartData_XPosition.Name = "PartData_XPosition";
            this.PartData_XPosition.ReadOnly = true;
            this.PartData_XPosition.Size = new System.Drawing.Size(136, 20);
            this.PartData_XPosition.TabIndex = 55;
            // 
            // PartData_Angle
            // 
            this.PartData_Angle.Location = new System.Drawing.Point(136, 606);
            this.PartData_Angle.Name = "PartData_Angle";
            this.PartData_Angle.ReadOnly = true;
            this.PartData_Angle.Size = new System.Drawing.Size(136, 20);
            this.PartData_Angle.TabIndex = 54;
            // 
            // F2_YPosition
            // 
            this.F2_YPosition.Location = new System.Drawing.Point(148, 420);
            this.F2_YPosition.Name = "F2_YPosition";
            this.F2_YPosition.Size = new System.Drawing.Size(136, 20);
            this.F2_YPosition.TabIndex = 46;
            // 
            // F2_XPosition
            // 
            this.F2_XPosition.Location = new System.Drawing.Point(148, 394);
            this.F2_XPosition.Name = "F2_XPosition";
            this.F2_XPosition.Size = new System.Drawing.Size(136, 20);
            this.F2_XPosition.TabIndex = 45;
            // 
            // F1_YPosition
            // 
            this.F1_YPosition.Location = new System.Drawing.Point(148, 350);
            this.F1_YPosition.Name = "F1_YPosition";
            this.F1_YPosition.Size = new System.Drawing.Size(136, 20);
            this.F1_YPosition.TabIndex = 41;
            // 
            // F1_XPosition
            // 
            this.F1_XPosition.Location = new System.Drawing.Point(148, 324);
            this.F1_XPosition.Name = "F1_XPosition";
            this.F1_XPosition.Size = new System.Drawing.Size(136, 20);
            this.F1_XPosition.TabIndex = 40;
            // 
            // FF_LDistance
            // 
            this.FF_LDistance.Location = new System.Drawing.Point(136, 238);
            this.FF_LDistance.Name = "FF_LDistance";
            this.FF_LDistance.ReadOnly = true;
            this.FF_LDistance.Size = new System.Drawing.Size(136, 20);
            this.FF_LDistance.TabIndex = 36;
            // 
            // FC_LDistance
            // 
            this.FC_LDistance.Location = new System.Drawing.Point(136, 111);
            this.FC_LDistance.Name = "FC_LDistance";
            this.FC_LDistance.ReadOnly = true;
            this.FC_LDistance.Size = new System.Drawing.Size(136, 20);
            this.FC_LDistance.TabIndex = 34;
            // 
            // FF_Angle
            // 
            this.FF_Angle.Location = new System.Drawing.Point(136, 212);
            this.FF_Angle.Name = "FF_Angle";
            this.FF_Angle.ReadOnly = true;
            this.FF_Angle.Size = new System.Drawing.Size(136, 20);
            this.FF_Angle.TabIndex = 31;
            // 
            // FF_YDistance
            // 
            this.FF_YDistance.Location = new System.Drawing.Point(136, 186);
            this.FF_YDistance.Name = "FF_YDistance";
            this.FF_YDistance.Size = new System.Drawing.Size(136, 20);
            this.FF_YDistance.TabIndex = 30;
            // 
            // FF_XDistance
            // 
            this.FF_XDistance.Location = new System.Drawing.Point(136, 160);
            this.FF_XDistance.Name = "FF_XDistance";
            this.FF_XDistance.Size = new System.Drawing.Size(136, 20);
            this.FF_XDistance.TabIndex = 29;
            // 
            // FC_Angle
            // 
            this.FC_Angle.Location = new System.Drawing.Point(136, 80);
            this.FC_Angle.Name = "FC_Angle";
            this.FC_Angle.ReadOnly = true;
            this.FC_Angle.Size = new System.Drawing.Size(136, 20);
            this.FC_Angle.TabIndex = 25;
            // 
            // FC_YDistance
            // 
            this.FC_YDistance.Location = new System.Drawing.Point(136, 54);
            this.FC_YDistance.Name = "FC_YDistance";
            this.FC_YDistance.Size = new System.Drawing.Size(136, 20);
            this.FC_YDistance.TabIndex = 24;
            // 
            // FC_XDistance
            // 
            this.FC_XDistance.Location = new System.Drawing.Point(136, 28);
            this.FC_XDistance.Name = "FC_XDistance";
            this.FC_XDistance.Size = new System.Drawing.Size(136, 20);
            this.FC_XDistance.TabIndex = 23;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(686, 80);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(66, 13);
            this.label30.TabIndex = 67;
            this.label30.Text = "Rotate (deg)";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(686, 13);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(67, 13);
            this.label29.TabIndex = 66;
            this.label29.Text = "Robot Offset";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(686, 56);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(70, 13);
            this.label27.TabIndex = 63;
            this.label27.Text = "Y Offset (mm)";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(686, 30);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(70, 13);
            this.label28.TabIndex = 62;
            this.label28.Text = "X Offset (mm)";
            // 
            // RobotCalc
            // 
            this.RobotCalc.Location = new System.Drawing.Point(946, 24);
            this.RobotCalc.Name = "RobotCalc";
            this.RobotCalc.Size = new System.Drawing.Size(97, 24);
            this.RobotCalc.TabIndex = 61;
            this.RobotCalc.Text = "Calculate";
            this.RobotCalc.UseVisualStyleBackColor = true;
            this.RobotCalc.Click += new System.EventHandler(this.RobotCalc_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(28, 537);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(80, 13);
            this.label25.TabIndex = 58;
            this.label25.Text = "Y Nominal (mm)";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(28, 511);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(80, 13);
            this.label26.TabIndex = 57;
            this.label26.Text = "X Nominal (mm)";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(22, 609);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(85, 13);
            this.label24.TabIndex = 53;
            this.label24.Text = "Part Angle (Rad)";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(28, 587);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(79, 13);
            this.label21.TabIndex = 50;
            this.label21.Text = "Y Position (mm)";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(28, 561);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(79, 13);
            this.label22.TabIndex = 49;
            this.label22.Text = "X Position (mm)";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(18, 468);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(52, 13);
            this.label23.TabIndex = 48;
            this.label23.Text = "Part Data";
            // 
            // btnPartLocCalc
            // 
            this.btnPartLocCalc.Location = new System.Drawing.Point(320, 324);
            this.btnPartLocCalc.Name = "btnPartLocCalc";
            this.btnPartLocCalc.Size = new System.Drawing.Size(97, 24);
            this.btnPartLocCalc.TabIndex = 47;
            this.btnPartLocCalc.Text = "Calculate";
            this.btnPartLocCalc.UseVisualStyleBackColor = true;
            this.btnPartLocCalc.Click += new System.EventHandler(this.btnPartLocCalc_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(28, 427);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(79, 13);
            this.label18.TabIndex = 44;
            this.label18.Text = "Y Position (mm)";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(28, 401);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(79, 13);
            this.label19.TabIndex = 43;
            this.label19.Text = "X Position (mm)";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(18, 379);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(89, 13);
            this.label20.TabIndex = 42;
            this.label20.Text = "Fiducial - Second";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(28, 357);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(79, 13);
            this.label15.TabIndex = 39;
            this.label15.Text = "Y Position (mm)";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(28, 331);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 13);
            this.label16.TabIndex = 38;
            this.label16.Text = "X Position (mm)";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(18, 309);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(71, 13);
            this.label17.TabIndex = 37;
            this.label17.Text = "Fiducial - First";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(33, 241);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(74, 13);
            this.label14.TabIndex = 35;
            this.label14.Text = "Distance (mm)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(33, 114);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 13);
            this.label13.TabIndex = 33;
            this.label13.Text = "Distance (mm)";
            // 
            // btnPartCalc
            // 
            this.btnPartCalc.Location = new System.Drawing.Point(320, 25);
            this.btnPartCalc.Name = "btnPartCalc";
            this.btnPartCalc.Size = new System.Drawing.Size(97, 24);
            this.btnPartCalc.TabIndex = 32;
            this.btnPartCalc.Text = "Calculate";
            this.btnPartCalc.UseVisualStyleBackColor = true;
            this.btnPartCalc.Click += new System.EventHandler(this.btnPartCalc_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 219);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Nominal Angle (Rad)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(23, 193);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(84, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Y Distance (mm)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(23, 167);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(84, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "X Distance (mm)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 87);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(104, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Nominal Angle (Rad)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 145);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "Fiducial to Fiducial";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Y Distance (mm)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "X Distance (mm)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Fiducial to Center";
            // 
            // tabFileControl
            // 
            this.tabFileControl.Controls.Add(this.bttnLog);
            this.tabFileControl.Controls.Add(this.bttnArchive);
            this.tabFileControl.Controls.Add(this.txtLogFile);
            this.tabFileControl.Controls.Add(this.txtArchive);
            this.tabFileControl.Controls.Add(this.label2);
            this.tabFileControl.Controls.Add(this.label1);
            this.tabFileControl.Location = new System.Drawing.Point(4, 22);
            this.tabFileControl.Name = "tabFileControl";
            this.tabFileControl.Padding = new System.Windows.Forms.Padding(3);
            this.tabFileControl.Size = new System.Drawing.Size(1082, 719);
            this.tabFileControl.TabIndex = 3;
            this.tabFileControl.Text = "File Control";
            this.tabFileControl.UseVisualStyleBackColor = true;
            // 
            // bttnLog
            // 
            this.bttnLog.Location = new System.Drawing.Point(6, 48);
            this.bttnLog.Name = "bttnLog";
            this.bttnLog.Size = new System.Drawing.Size(115, 23);
            this.bttnLog.TabIndex = 16;
            this.bttnLog.Text = "Open Log Folder";
            this.bttnLog.UseVisualStyleBackColor = true;
            this.bttnLog.Click += new System.EventHandler(this.bttnLog_Click);
            // 
            // bttnArchive
            // 
            this.bttnArchive.Location = new System.Drawing.Point(6, 19);
            this.bttnArchive.Name = "bttnArchive";
            this.bttnArchive.Size = new System.Drawing.Size(115, 23);
            this.bttnArchive.TabIndex = 15;
            this.bttnArchive.Text = "Open Archive Folder";
            this.bttnArchive.UseVisualStyleBackColor = true;
            this.bttnArchive.Click += new System.EventHandler(this.bttnArchive_Click);
            // 
            // txtLogFile
            // 
            this.txtLogFile.AutoSize = true;
            this.txtLogFile.Location = new System.Drawing.Point(211, 53);
            this.txtLogFile.Name = "txtLogFile";
            this.txtLogFile.Size = new System.Drawing.Size(52, 13);
            this.txtLogFile.TabIndex = 13;
            this.txtLogFile.Text = "txtLogFile";
            // 
            // txtArchive
            // 
            this.txtArchive.AutoSize = true;
            this.txtArchive.Location = new System.Drawing.Point(211, 24);
            this.txtArchive.Name = "txtArchive";
            this.txtArchive.Size = new System.Drawing.Size(54, 13);
            this.txtArchive.TabIndex = 12;
            this.txtArchive.Text = "txtArchive";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(127, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Log File Folder:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(127, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Archive Folder:";
            // 
            // tabLicenseCheck
            // 
            this.tabLicenseCheck.Controls.Add(this.btnLicenseCheck);
            this.tabLicenseCheck.Controls.Add(this.tbExpireDate);
            this.tabLicenseCheck.Controls.Add(this.LicenseCheckList);
            this.tabLicenseCheck.Location = new System.Drawing.Point(4, 22);
            this.tabLicenseCheck.Name = "tabLicenseCheck";
            this.tabLicenseCheck.Padding = new System.Windows.Forms.Padding(3);
            this.tabLicenseCheck.Size = new System.Drawing.Size(1082, 719);
            this.tabLicenseCheck.TabIndex = 2;
            this.tabLicenseCheck.Text = "LicenseCheck";
            this.tabLicenseCheck.UseVisualStyleBackColor = true;
            // 
            // btnLicenseCheck
            // 
            this.btnLicenseCheck.Location = new System.Drawing.Point(6, 6);
            this.btnLicenseCheck.Name = "btnLicenseCheck";
            this.btnLicenseCheck.Size = new System.Drawing.Size(97, 24);
            this.btnLicenseCheck.TabIndex = 5;
            this.btnLicenseCheck.Text = "License Check";
            this.btnLicenseCheck.UseVisualStyleBackColor = true;
            this.btnLicenseCheck.Click += new System.EventHandler(this.btnLicenseCheck_Click);
            // 
            // tbExpireDate
            // 
            this.tbExpireDate.Location = new System.Drawing.Point(109, 3);
            this.tbExpireDate.Name = "tbExpireDate";
            this.tbExpireDate.ReadOnly = true;
            this.tbExpireDate.Size = new System.Drawing.Size(257, 20);
            this.tbExpireDate.TabIndex = 2;
            // 
            // LicenseCheckList
            // 
            this.LicenseCheckList.Location = new System.Drawing.Point(6, 45);
            this.LicenseCheckList.Name = "LicenseCheckList";
            this.LicenseCheckList.Size = new System.Drawing.Size(360, 537);
            this.LicenseCheckList.TabIndex = 1;
            // 
            // tabFrameGrabber
            // 
            this.tabFrameGrabber.Controls.Add(this.panel3);
            this.tabFrameGrabber.Controls.Add(this.panel4);
            this.tabFrameGrabber.Controls.Add(this.panel2);
            this.tabFrameGrabber.Location = new System.Drawing.Point(4, 22);
            this.tabFrameGrabber.Name = "tabFrameGrabber";
            this.tabFrameGrabber.Padding = new System.Windows.Forms.Padding(3);
            this.tabFrameGrabber.Size = new System.Drawing.Size(1082, 719);
            this.tabFrameGrabber.TabIndex = 0;
            this.tabFrameGrabber.Text = "FrameGrabber";
            this.tabFrameGrabber.UseVisualStyleBackColor = true;
            // 
            // cbToolBlock
            // 
            this.cbToolBlock.FormattingEnabled = true;
            this.cbToolBlock.Location = new System.Drawing.Point(5, 68);
            this.cbToolBlock.Name = "cbToolBlock";
            this.cbToolBlock.Size = new System.Drawing.Size(324, 21);
            this.cbToolBlock.TabIndex = 35;
            this.cbToolBlock.SelectedIndexChanged += new System.EventHandler(this.cbToolBlock_SelectedIndexChanged);
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.Color.LightGray;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
            this.panel4.Location = new System.Drawing.Point(6, 6);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(332, 254);
            this.panel4.TabIndex = 33;
            this.panel4.Tag = "pnCameraControl";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(9, 6);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(77, 13);
            this.label35.TabIndex = 33;
            this.label35.Text = "Camera Name:";
            // 
            // tbCameraName
            // 
            this.tbCameraName.Location = new System.Drawing.Point(115, 3);
            this.tbCameraName.Name = "tbCameraName";
            this.tbCameraName.Size = new System.Drawing.Size(210, 20);
            this.tbCameraName.TabIndex = 32;
            this.tbCameraName.TextChanged += new System.EventHandler(this.tbCameraName_TextChanged);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(9, 107);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(95, 13);
            this.label37.TabIndex = 31;
            this.label37.Text = "Camera Resource:";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(9, 80);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(80, 13);
            this.label31.TabIndex = 30;
            this.label31.Text = "Camera Server:";
            // 
            // cbCameraConnected
            // 
            this.cbCameraConnected.AutoCheck = false;
            this.cbCameraConnected.Location = new System.Drawing.Point(115, 42);
            this.cbCameraConnected.Name = "cbCameraConnected";
            this.cbCameraConnected.Size = new System.Drawing.Size(118, 24);
            this.cbCameraConnected.TabIndex = 19;
            this.cbCameraConnected.Text = "Camera Connected";
            // 
            // bttnConnectCamera
            // 
            this.bttnConnectCamera.Location = new System.Drawing.Point(3, 41);
            this.bttnConnectCamera.Name = "bttnConnectCamera";
            this.bttnConnectCamera.Size = new System.Drawing.Size(106, 24);
            this.bttnConnectCamera.TabIndex = 18;
            this.bttnConnectCamera.Text = "Connect to Camera";
            this.bttnConnectCamera.UseVisualStyleBackColor = true;
            this.bttnConnectCamera.Click += new System.EventHandler(this.bttnConnectCamera_Click);
            // 
            // cbConfigFileFound
            // 
            this.cbConfigFileFound.AutoCheck = false;
            this.cbConfigFileFound.Location = new System.Drawing.Point(124, 164);
            this.cbConfigFileFound.Name = "cbConfigFileFound";
            this.cbConfigFileFound.Size = new System.Drawing.Size(147, 24);
            this.cbConfigFileFound.TabIndex = 23;
            this.cbConfigFileFound.Text = "Configuration File Found";
            // 
            // cbServerList
            // 
            this.cbServerList.FormattingEnabled = true;
            this.cbServerList.Location = new System.Drawing.Point(115, 72);
            this.cbServerList.Name = "cbServerList";
            this.cbServerList.Size = new System.Drawing.Size(210, 21);
            this.cbServerList.TabIndex = 16;
            this.cbServerList.SelectedIndexChanged += new System.EventHandler(this.cbServerList_SelectedIndexChanged);
            // 
            // cbConfigFileReq
            // 
            this.cbConfigFileReq.AutoCheck = false;
            this.cbConfigFileReq.Location = new System.Drawing.Point(3, 135);
            this.cbConfigFileReq.Name = "cbConfigFileReq";
            this.cbConfigFileReq.Size = new System.Drawing.Size(190, 24);
            this.cbConfigFileReq.TabIndex = 22;
            this.cbConfigFileReq.Text = "Configuration File Required";
            // 
            // bttnC1Config
            // 
            this.bttnC1Config.Location = new System.Drawing.Point(3, 165);
            this.bttnC1Config.Name = "bttnC1Config";
            this.bttnC1Config.Size = new System.Drawing.Size(115, 23);
            this.bttnC1Config.TabIndex = 20;
            this.bttnC1Config.Text = "Select ConfigFile";
            this.bttnC1Config.UseVisualStyleBackColor = true;
            this.bttnC1Config.Click += new System.EventHandler(this.bttnC1Config_Click);
            // 
            // cbDeviceList
            // 
            this.cbDeviceList.FormattingEnabled = true;
            this.cbDeviceList.Location = new System.Drawing.Point(115, 99);
            this.cbDeviceList.Name = "cbDeviceList";
            this.cbDeviceList.Size = new System.Drawing.Size(210, 21);
            this.cbDeviceList.TabIndex = 17;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.LightGray;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.cbArchiveActive);
            this.panel2.Controls.Add(this.bttnArchiveImage);
            this.panel2.Controls.Add(this.label34);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tbArchiveIndex);
            this.panel2.Controls.Add(this.tbArchiveCount);
            this.panel2.Location = new System.Drawing.Point(851, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(223, 132);
            this.panel2.TabIndex = 31;
            this.panel2.Tag = "pnCameraControl";
            // 
            // bttnArchiveImage
            // 
            this.bttnArchiveImage.Location = new System.Drawing.Point(48, 12);
            this.bttnArchiveImage.Name = "bttnArchiveImage";
            this.bttnArchiveImage.Size = new System.Drawing.Size(125, 24);
            this.bttnArchiveImage.TabIndex = 26;
            this.bttnArchiveImage.Text = "Use Archived Images";
            this.bttnArchiveImage.UseVisualStyleBackColor = true;
            this.bttnArchiveImage.Click += new System.EventHandler(this.bttnArchiveImage_Click);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(3, 99);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(109, 13);
            this.label34.TabIndex = 30;
            this.label34.Text = "Archive Image Count:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Archive Index:";
            // 
            // tbArchiveIndex
            // 
            this.tbArchiveIndex.Location = new System.Drawing.Point(140, 66);
            this.tbArchiveIndex.Name = "tbArchiveIndex";
            this.tbArchiveIndex.Size = new System.Drawing.Size(76, 20);
            this.tbArchiveIndex.TabIndex = 27;
            // 
            // tbArchiveCount
            // 
            this.tbArchiveCount.Location = new System.Drawing.Point(140, 96);
            this.tbArchiveCount.Name = "tbArchiveCount";
            this.tbArchiveCount.Size = new System.Drawing.Size(76, 20);
            this.tbArchiveCount.TabIndex = 28;
            // 
            // tbC1TB1Name
            // 
            this.tbC1TB1Name.Location = new System.Drawing.Point(126, 13);
            this.tbC1TB1Name.Name = "tbC1TB1Name";
            this.tbC1TB1Name.Size = new System.Drawing.Size(203, 20);
            this.tbC1TB1Name.TabIndex = 34;
            // 
            // cbC1Tb1FileFound
            // 
            this.cbC1Tb1FileFound.AutoCheck = false;
            this.cbC1Tb1FileFound.Location = new System.Drawing.Point(3, 41);
            this.cbC1Tb1FileFound.Name = "cbC1Tb1FileFound";
            this.cbC1Tb1FileFound.Size = new System.Drawing.Size(147, 24);
            this.cbC1Tb1FileFound.TabIndex = 24;
            this.cbC1Tb1FileFound.Text = "Tool Block Found";
            // 
            // bttnC1TB1FileSelect
            // 
            this.bttnC1TB1FileSelect.Location = new System.Drawing.Point(5, 12);
            this.bttnC1TB1FileSelect.Name = "bttnC1TB1FileSelect";
            this.bttnC1TB1FileSelect.Size = new System.Drawing.Size(115, 23);
            this.bttnC1TB1FileSelect.TabIndex = 21;
            this.bttnC1TB1FileSelect.Text = "Select New Job";
            this.bttnC1TB1FileSelect.UseVisualStyleBackColor = true;
            this.bttnC1TB1FileSelect.Click += new System.EventHandler(this.bttnC1TB1FileSelect_Click);
            // 
            // tabImage
            // 
            this.tabImage.Controls.Add(this.panel1);
            this.tabImage.Controls.Add(this.cogDisplay1);
            this.tabImage.Location = new System.Drawing.Point(4, 22);
            this.tabImage.Name = "tabImage";
            this.tabImage.Padding = new System.Windows.Forms.Padding(3);
            this.tabImage.Size = new System.Drawing.Size(1082, 719);
            this.tabImage.TabIndex = 1;
            this.tabImage.Text = "Image";
            this.tabImage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.LightGray;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.bttnC1LogImages);
            this.panel1.Controls.Add(this.lbToolData);
            this.panel1.Controls.Add(this.label32);
            this.panel1.Controls.Add(this.bttnC1Abort);
            this.panel1.Controls.Add(this.bttnC1Freeze);
            this.panel1.Controls.Add(this.bttnC1Grab);
            this.panel1.Controls.Add(this.bttnC1Snap);
            this.panel1.Controls.Add(this.txtC1ImageTime);
            this.panel1.Location = new System.Drawing.Point(851, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(223, 668);
            this.panel1.TabIndex = 20;
            this.panel1.Tag = "pnCameraControl";
            // 
            // bttnC1LogImages
            // 
            this.bttnC1LogImages.Location = new System.Drawing.Point(19, 174);
            this.bttnC1LogImages.Name = "bttnC1LogImages";
            this.bttnC1LogImages.Size = new System.Drawing.Size(182, 24);
            this.bttnC1LogImages.TabIndex = 23;
            this.bttnC1LogImages.Text = "Log Images";
            this.bttnC1LogImages.UseVisualStyleBackColor = true;
            this.bttnC1LogImages.Click += new System.EventHandler(this.bttnC1LogImages_Click);
            // 
            // lbToolData
            // 
            this.lbToolData.FormattingEnabled = true;
            this.lbToolData.Location = new System.Drawing.Point(19, 264);
            this.lbToolData.Name = "lbToolData";
            this.lbToolData.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbToolData.Size = new System.Drawing.Size(182, 394);
            this.lbToolData.TabIndex = 22;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(46, 11);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(125, 24);
            this.label32.TabIndex = 21;
            this.label32.Text = "CAMERA 01";
            // 
            // bttnC1Abort
            // 
            this.bttnC1Abort.Location = new System.Drawing.Point(17, 144);
            this.bttnC1Abort.Name = "bttnC1Abort";
            this.bttnC1Abort.Size = new System.Drawing.Size(182, 24);
            this.bttnC1Abort.TabIndex = 19;
            this.bttnC1Abort.Text = "Abort";
            this.bttnC1Abort.UseVisualStyleBackColor = true;
            this.bttnC1Abort.Click += new System.EventHandler(this.bttnC1Abort_Click);
            // 
            // bttnC1Freeze
            // 
            this.bttnC1Freeze.Location = new System.Drawing.Point(17, 114);
            this.bttnC1Freeze.Name = "bttnC1Freeze";
            this.bttnC1Freeze.Size = new System.Drawing.Size(182, 24);
            this.bttnC1Freeze.TabIndex = 18;
            this.bttnC1Freeze.Text = "Freeze";
            this.bttnC1Freeze.UseVisualStyleBackColor = true;
            this.bttnC1Freeze.Click += new System.EventHandler(this.bttnC1Freeze_Click);
            // 
            // bttnC1Grab
            // 
            this.bttnC1Grab.Location = new System.Drawing.Point(17, 84);
            this.bttnC1Grab.Name = "bttnC1Grab";
            this.bttnC1Grab.Size = new System.Drawing.Size(182, 24);
            this.bttnC1Grab.TabIndex = 17;
            this.bttnC1Grab.Text = "Continuous";
            this.bttnC1Grab.UseVisualStyleBackColor = true;
            this.bttnC1Grab.Click += new System.EventHandler(this.bttnC1Grab_Click);
            // 
            // bttnC1Snap
            // 
            this.bttnC1Snap.Location = new System.Drawing.Point(17, 54);
            this.bttnC1Snap.Name = "bttnC1Snap";
            this.bttnC1Snap.Size = new System.Drawing.Size(182, 24);
            this.bttnC1Snap.TabIndex = 16;
            this.bttnC1Snap.Text = "Single Image";
            this.bttnC1Snap.UseVisualStyleBackColor = true;
            this.bttnC1Snap.Click += new System.EventHandler(this.bttnC1Snap_Click);
            // 
            // txtC1ImageTime
            // 
            this.txtC1ImageTime.AutoSize = true;
            this.txtC1ImageTime.Location = new System.Drawing.Point(68, 248);
            this.txtC1ImageTime.Name = "txtC1ImageTime";
            this.txtC1ImageTime.Size = new System.Drawing.Size(84, 13);
            this.txtC1ImageTime.TabIndex = 13;
            this.txtC1ImageTime.Text = "Acquisition Time";
            // 
            // cogDisplay1
            // 
            this.cogDisplay1.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogDisplay1.ColorMapLowerRoiLimit = 0D;
            this.cogDisplay1.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogDisplay1.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogDisplay1.ColorMapUpperRoiLimit = 1D;
            this.cogDisplay1.DoubleTapZoomCycleLength = 2;
            this.cogDisplay1.DoubleTapZoomSensitivity = 2.5D;
            this.cogDisplay1.Location = new System.Drawing.Point(5, 5);
            this.cogDisplay1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogDisplay1.MouseWheelSensitivity = 1D;
            this.cogDisplay1.Name = "cogDisplay1";
            this.cogDisplay1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogDisplay1.OcxState")));
            this.cogDisplay1.Size = new System.Drawing.Size(823, 669);
            this.cogDisplay1.TabIndex = 15;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabImage);
            this.tabControl1.Controls.Add(this.tabFrameGrabber);
            this.tabControl1.Controls.Add(this.tabLicenseCheck);
            this.tabControl1.Controls.Add(this.tabFileControl);
            this.tabControl1.Controls.Add(this.tabPartData);
            this.tabControl1.Controls.Add(this.tabToolBlock);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1090, 745);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.TabIndexChanged += new System.EventHandler(this.tabControl1_TabIndexChanged);
            // 
            // cbArchiveActive
            // 
            this.cbArchiveActive.AutoCheck = false;
            this.cbArchiveActive.Location = new System.Drawing.Point(6, 45);
            this.cbArchiveActive.Name = "cbArchiveActive";
            this.cbArchiveActive.Size = new System.Drawing.Size(178, 24);
            this.cbArchiveActive.TabIndex = 31;
            this.cbArchiveActive.Text = "Using Archive Image Selected";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.LightGray;
            this.panel3.Controls.Add(this.bttnC1TB1FileSelect);
            this.panel3.Controls.Add(this.cbToolBlock);
            this.panel3.Controls.Add(this.cbC1Tb1FileFound);
            this.panel3.Controls.Add(this.tbC1TB1Name);
            this.panel3.Location = new System.Drawing.Point(6, 266);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(332, 99);
            this.panel3.TabIndex = 36;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1090, 745);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIP4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIP1)).EndInit();
            this.tabToolBlock.ResumeLayout(false);
            this.tabToolBlock.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).EndInit();
            this.tabPartData.ResumeLayout(false);
            this.tabPartData.PerformLayout();
            this.tabFileControl.ResumeLayout(false);
            this.tabFileControl.PerformLayout();
            this.tabLicenseCheck.ResumeLayout(false);
            this.tabLicenseCheck.PerformLayout();
            this.tabFrameGrabber.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabImage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogDisplay1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox cbHeartbeat;
        private System.Windows.Forms.Button bttnPLC;
        private System.Windows.Forms.NumericUpDown numIP4;
        private System.Windows.Forms.NumericUpDown numIP3;
        private System.Windows.Forms.NumericUpDown numIP2;
        private System.Windows.Forms.NumericUpDown numIP1;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TabPage tabToolBlock;
        private Cognex.VisionPro.CogDisplayStatusBarV2 Camera1_DisplayStatusBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbCameraSelected;
        private System.Windows.Forms.TabPage tabPartData;
        private System.Windows.Forms.TextBox VGR_DegOffset;
        private System.Windows.Forms.TextBox VGR_YOffset;
        private System.Windows.Forms.TextBox VGR_XOffset;
        private System.Windows.Forms.TextBox PartData_YNom;
        private System.Windows.Forms.TextBox PartData_XNom;
        private System.Windows.Forms.TextBox PartData_YPosition;
        private System.Windows.Forms.TextBox PartData_XPosition;
        private System.Windows.Forms.TextBox PartData_Angle;
        private System.Windows.Forms.TextBox F2_YPosition;
        private System.Windows.Forms.TextBox F2_XPosition;
        private System.Windows.Forms.TextBox F1_YPosition;
        private System.Windows.Forms.TextBox F1_XPosition;
        private System.Windows.Forms.TextBox FF_LDistance;
        private System.Windows.Forms.TextBox FC_LDistance;
        private System.Windows.Forms.TextBox FF_Angle;
        private System.Windows.Forms.TextBox FF_YDistance;
        private System.Windows.Forms.TextBox FF_XDistance;
        private System.Windows.Forms.TextBox FC_Angle;
        private System.Windows.Forms.TextBox FC_YDistance;
        private System.Windows.Forms.TextBox FC_XDistance;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Button RobotCalc;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button btnPartLocCalc;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnPartCalc;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabFileControl;
        private System.Windows.Forms.Button bttnLog;
        private System.Windows.Forms.Button bttnArchive;
        private System.Windows.Forms.Label txtLogFile;
        private System.Windows.Forms.Label txtArchive;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabLicenseCheck;
        private System.Windows.Forms.Button btnLicenseCheck;
        private System.Windows.Forms.TextBox tbExpireDate;
        internal System.Windows.Forms.ListBox LicenseCheckList;
        private System.Windows.Forms.TabPage tabFrameGrabber;
        private System.Windows.Forms.Panel panel4;
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
        private System.Windows.Forms.Button bttnArchiveImage;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbArchiveIndex;
        private System.Windows.Forms.TextBox tbArchiveCount;
        private System.Windows.Forms.TextBox tbC1TB1Name;
        private System.Windows.Forms.CheckBox cbC1Tb1FileFound;
        private System.Windows.Forms.Button bttnC1TB1FileSelect;
        private System.Windows.Forms.TabPage tabImage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bttnC1LogImages;
        private System.Windows.Forms.ListBox lbToolData;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Button bttnC1Abort;
        private System.Windows.Forms.Button bttnC1Freeze;
        private System.Windows.Forms.Button bttnC1Grab;
        private System.Windows.Forms.Button bttnC1Snap;
        private System.Windows.Forms.Label txtC1ImageTime;
        private Cognex.VisionPro.Display.CogDisplay cogDisplay1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ComboBox cbToolBlock;
        private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cogToolBlockEditV21;
        private System.Windows.Forms.CheckBox cbArchiveActive;
        private System.Windows.Forms.Panel panel3;
    }
}


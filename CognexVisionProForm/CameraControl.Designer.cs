﻿namespace CognexVisionProForm
{
    partial class CameraControl
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraControl));
            this.bttnCameraLog = new System.Windows.Forms.Button();
            this.bttnCameraAbort = new System.Windows.Forms.Button();
            this.bttnCameraSnap = new System.Windows.Forms.Button();
            this.lbCameraName = new System.Windows.Forms.Label();
            this.lbAcqTime = new System.Windows.Forms.Label();
            this.lbCameraDescription = new System.Windows.Forms.Label();
            this.cbCameraConnected = new System.Windows.Forms.CheckBox();
            this.cbImageReady = new System.Windows.Forms.CheckBox();
            this.cbArchiveImageActive = new System.Windows.Forms.CheckBox();
            this.plControl = new System.Windows.Forms.Panel();
            this.bttnTest = new System.Windows.Forms.Button();
            this.cbResultsUpdated = new System.Windows.Forms.CheckBox();
            this.bttnGetToolData = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numRecordSelect = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cbTrigger = new System.Windows.Forms.CheckBox();
            this.numToolSelect = new System.Windows.Forms.NumericUpDown();
            this.lbToolRunTime = new System.Windows.Forms.Label();
            this.cbToolPassed = new System.Windows.Forms.CheckBox();
            this.lbToolName = new System.Windows.Forms.Label();
            this.recordDisplay = new Cognex.VisionPro.CogRecordDisplay();
            this.lbRecordName = new System.Windows.Forms.Label();
            this.plToolData = new System.Windows.Forms.Panel();
            this.toolBlockBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cameraControlBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.plControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRecordSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numToolSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.recordDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolBlockBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cameraControlBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // bttnCameraLog
            // 
            this.bttnCameraLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bttnCameraLog.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.bttnCameraLog.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.bttnCameraLog.FlatAppearance.BorderSize = 2;
            this.bttnCameraLog.Location = new System.Drawing.Point(3, 497);
            this.bttnCameraLog.Name = "bttnCameraLog";
            this.bttnCameraLog.Size = new System.Drawing.Size(152, 29);
            this.bttnCameraLog.TabIndex = 2;
            this.bttnCameraLog.Text = "Log Images";
            this.bttnCameraLog.UseVisualStyleBackColor = false;
            this.bttnCameraLog.Click += new System.EventHandler(this.bttnCameraLog_Click);
            // 
            // bttnCameraAbort
            // 
            this.bttnCameraAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bttnCameraAbort.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.bttnCameraAbort.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.bttnCameraAbort.FlatAppearance.BorderSize = 2;
            this.bttnCameraAbort.Location = new System.Drawing.Point(3, 462);
            this.bttnCameraAbort.Name = "bttnCameraAbort";
            this.bttnCameraAbort.Size = new System.Drawing.Size(152, 29);
            this.bttnCameraAbort.TabIndex = 1;
            this.bttnCameraAbort.Text = "Abort";
            this.bttnCameraAbort.UseVisualStyleBackColor = false;
            this.bttnCameraAbort.Click += new System.EventHandler(this.bttnCameraAbort_Click);
            // 
            // bttnCameraSnap
            // 
            this.bttnCameraSnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bttnCameraSnap.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.bttnCameraSnap.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.bttnCameraSnap.FlatAppearance.BorderSize = 2;
            this.bttnCameraSnap.Location = new System.Drawing.Point(3, 428);
            this.bttnCameraSnap.Name = "bttnCameraSnap";
            this.bttnCameraSnap.Size = new System.Drawing.Size(152, 29);
            this.bttnCameraSnap.TabIndex = 0;
            this.bttnCameraSnap.Text = "Single Snap";
            this.bttnCameraSnap.UseVisualStyleBackColor = false;
            this.bttnCameraSnap.Click += new System.EventHandler(this.bttnCameraSnap_Click);
            this.bttnCameraSnap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bttnCameraSnap_MouseUp);
            // 
            // lbCameraName
            // 
            this.lbCameraName.AutoSize = true;
            this.lbCameraName.Location = new System.Drawing.Point(3, 9);
            this.lbCameraName.Name = "lbCameraName";
            this.lbCameraName.Size = new System.Drawing.Size(74, 13);
            this.lbCameraName.TabIndex = 17;
            this.lbCameraName.Text = "Camera Name";
            // 
            // lbAcqTime
            // 
            this.lbAcqTime.AutoSize = true;
            this.lbAcqTime.Location = new System.Drawing.Point(3, 41);
            this.lbAcqTime.Name = "lbAcqTime";
            this.lbAcqTime.Size = new System.Drawing.Size(84, 13);
            this.lbAcqTime.TabIndex = 18;
            this.lbAcqTime.Text = "Acquisition Time";
            // 
            // lbCameraDescription
            // 
            this.lbCameraDescription.AutoSize = true;
            this.lbCameraDescription.Location = new System.Drawing.Point(3, 25);
            this.lbCameraDescription.Name = "lbCameraDescription";
            this.lbCameraDescription.Size = new System.Drawing.Size(99, 13);
            this.lbCameraDescription.TabIndex = 19;
            this.lbCameraDescription.Text = "Camera Description";
            // 
            // cbCameraConnected
            // 
            this.cbCameraConnected.AutoSize = true;
            this.cbCameraConnected.Location = new System.Drawing.Point(3, 89);
            this.cbCameraConnected.Name = "cbCameraConnected";
            this.cbCameraConnected.Size = new System.Drawing.Size(117, 17);
            this.cbCameraConnected.TabIndex = 21;
            this.cbCameraConnected.Text = "Camera Connected";
            this.cbCameraConnected.UseVisualStyleBackColor = true;
            // 
            // cbImageReady
            // 
            this.cbImageReady.AutoSize = true;
            this.cbImageReady.Location = new System.Drawing.Point(3, 142);
            this.cbImageReady.Name = "cbImageReady";
            this.cbImageReady.Size = new System.Drawing.Size(89, 17);
            this.cbImageReady.TabIndex = 22;
            this.cbImageReady.Text = "Image Ready";
            this.cbImageReady.UseVisualStyleBackColor = true;
            // 
            // cbArchiveImageActive
            // 
            this.cbArchiveImageActive.AutoSize = true;
            this.cbArchiveImageActive.Location = new System.Drawing.Point(3, 107);
            this.cbArchiveImageActive.Name = "cbArchiveImageActive";
            this.cbArchiveImageActive.Size = new System.Drawing.Size(135, 17);
            this.cbArchiveImageActive.TabIndex = 23;
            this.cbArchiveImageActive.Text = "Using Archived Images";
            this.cbArchiveImageActive.UseVisualStyleBackColor = true;
            // 
            // plControl
            // 
            this.plControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plControl.BackColor = System.Drawing.SystemColors.ControlLight;
            this.plControl.Controls.Add(this.bttnTest);
            this.plControl.Controls.Add(this.cbResultsUpdated);
            this.plControl.Controls.Add(this.bttnGetToolData);
            this.plControl.Controls.Add(this.label2);
            this.plControl.Controls.Add(this.numRecordSelect);
            this.plControl.Controls.Add(this.label1);
            this.plControl.Controls.Add(this.cbTrigger);
            this.plControl.Controls.Add(this.numToolSelect);
            this.plControl.Controls.Add(this.lbToolRunTime);
            this.plControl.Controls.Add(this.cbToolPassed);
            this.plControl.Controls.Add(this.lbToolName);
            this.plControl.Controls.Add(this.cbArchiveImageActive);
            this.plControl.Controls.Add(this.cbImageReady);
            this.plControl.Controls.Add(this.cbCameraConnected);
            this.plControl.Controls.Add(this.lbCameraDescription);
            this.plControl.Controls.Add(this.lbAcqTime);
            this.plControl.Controls.Add(this.lbCameraName);
            this.plControl.Controls.Add(this.bttnCameraSnap);
            this.plControl.Controls.Add(this.bttnCameraAbort);
            this.plControl.Controls.Add(this.bttnCameraLog);
            this.plControl.Location = new System.Drawing.Point(549, 5);
            this.plControl.Margin = new System.Windows.Forms.Padding(20);
            this.plControl.Name = "plControl";
            this.plControl.Size = new System.Drawing.Size(158, 535);
            this.plControl.TabIndex = 18;
            // 
            // bttnTest
            // 
            this.bttnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bttnTest.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.bttnTest.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.bttnTest.FlatAppearance.BorderSize = 2;
            this.bttnTest.Location = new System.Drawing.Point(3, 305);
            this.bttnTest.Name = "bttnTest";
            this.bttnTest.Size = new System.Drawing.Size(152, 29);
            this.bttnTest.TabIndex = 40;
            this.bttnTest.Text = "Test Button";
            this.bttnTest.UseVisualStyleBackColor = false;
            this.bttnTest.Click += new System.EventHandler(this.bttnTest_Click);
            // 
            // cbResultsUpdated
            // 
            this.cbResultsUpdated.AutoSize = true;
            this.cbResultsUpdated.Location = new System.Drawing.Point(3, 179);
            this.cbResultsUpdated.Name = "cbResultsUpdated";
            this.cbResultsUpdated.Size = new System.Drawing.Size(103, 17);
            this.cbResultsUpdated.TabIndex = 39;
            this.cbResultsUpdated.Text = "Results updated";
            this.cbResultsUpdated.UseVisualStyleBackColor = true;
            // 
            // bttnGetToolData
            // 
            this.bttnGetToolData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bttnGetToolData.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.bttnGetToolData.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.bttnGetToolData.FlatAppearance.BorderSize = 2;
            this.bttnGetToolData.Location = new System.Drawing.Point(3, 340);
            this.bttnGetToolData.Name = "bttnGetToolData";
            this.bttnGetToolData.Size = new System.Drawing.Size(152, 29);
            this.bttnGetToolData.TabIndex = 37;
            this.bttnGetToolData.Text = "Get Tool Data";
            this.bttnGetToolData.UseVisualStyleBackColor = false;
            this.bttnGetToolData.Click += new System.EventHandler(this.bttnGetToolData_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 381);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "Selected Record";
            // 
            // numRecordSelect
            // 
            this.numRecordSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numRecordSelect.Location = new System.Drawing.Point(99, 379);
            this.numRecordSelect.Name = "numRecordSelect";
            this.numRecordSelect.Size = new System.Drawing.Size(50, 20);
            this.numRecordSelect.TabIndex = 34;
            this.numRecordSelect.ValueChanged += new System.EventHandler(this.numRecordSelect_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 407);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Selected Tool";
            // 
            // cbTrigger
            // 
            this.cbTrigger.AutoSize = true;
            this.cbTrigger.Location = new System.Drawing.Point(3, 125);
            this.cbTrigger.Name = "cbTrigger";
            this.cbTrigger.Size = new System.Drawing.Size(59, 17);
            this.cbTrigger.TabIndex = 31;
            this.cbTrigger.Text = "Trigger";
            this.cbTrigger.UseVisualStyleBackColor = true;
            // 
            // numToolSelect
            // 
            this.numToolSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numToolSelect.Location = new System.Drawing.Point(99, 405);
            this.numToolSelect.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numToolSelect.Name = "numToolSelect";
            this.numToolSelect.Size = new System.Drawing.Size(50, 20);
            this.numToolSelect.TabIndex = 30;
            this.numToolSelect.ValueChanged += new System.EventHandler(this.numToolSelect_ValueChanged);
            // 
            // lbToolRunTime
            // 
            this.lbToolRunTime.AutoSize = true;
            this.lbToolRunTime.Location = new System.Drawing.Point(3, 73);
            this.lbToolRunTime.Name = "lbToolRunTime";
            this.lbToolRunTime.Size = new System.Drawing.Size(77, 13);
            this.lbToolRunTime.TabIndex = 29;
            this.lbToolRunTime.Text = "Tool Run Time";
            // 
            // cbToolPassed
            // 
            this.cbToolPassed.AutoSize = true;
            this.cbToolPassed.Location = new System.Drawing.Point(3, 160);
            this.cbToolPassed.Name = "cbToolPassed";
            this.cbToolPassed.Size = new System.Drawing.Size(85, 17);
            this.cbToolPassed.TabIndex = 28;
            this.cbToolPassed.Text = "Tool Passed";
            this.cbToolPassed.UseVisualStyleBackColor = true;
            // 
            // lbToolName
            // 
            this.lbToolName.AutoSize = true;
            this.lbToolName.Location = new System.Drawing.Point(3, 57);
            this.lbToolName.Name = "lbToolName";
            this.lbToolName.Size = new System.Drawing.Size(59, 13);
            this.lbToolName.TabIndex = 27;
            this.lbToolName.Text = "Tool Name";
            // 
            // recordDisplay
            // 
            this.recordDisplay.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.recordDisplay.ColorMapLowerRoiLimit = 0D;
            this.recordDisplay.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.recordDisplay.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.recordDisplay.ColorMapUpperRoiLimit = 1D;
            this.recordDisplay.DoubleTapZoomCycleLength = 2;
            this.recordDisplay.DoubleTapZoomSensitivity = 2.5D;
            this.recordDisplay.Location = new System.Drawing.Point(5, 5);
            this.recordDisplay.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.recordDisplay.MouseWheelSensitivity = 1D;
            this.recordDisplay.Name = "recordDisplay";
            this.recordDisplay.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("recordDisplay.OcxState")));
            this.recordDisplay.Size = new System.Drawing.Size(62, 70);
            this.recordDisplay.TabIndex = 19;
            this.recordDisplay.TabStop = false;
            // 
            // lbRecordName
            // 
            this.lbRecordName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbRecordName.AutoSize = true;
            this.lbRecordName.Location = new System.Drawing.Point(8, 527);
            this.lbRecordName.Name = "lbRecordName";
            this.lbRecordName.Size = new System.Drawing.Size(73, 13);
            this.lbRecordName.TabIndex = 40;
            this.lbRecordName.Text = "Record Name";
            // 
            // plToolData
            // 
            this.plToolData.Location = new System.Drawing.Point(5, 83);
            this.plToolData.Name = "plToolData";
            this.plToolData.Size = new System.Drawing.Size(43, 46);
            this.plToolData.TabIndex = 41;
            // 
            // CameraControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(713, 543);
            this.ControlBox = false;
            this.Controls.Add(this.plToolData);
            this.Controls.Add(this.lbRecordName);
            this.Controls.Add(this.recordDisplay);
            this.Controls.Add(this.plControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CameraControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CameraControl";
            this.Load += new System.EventHandler(this.CameraControl_Load);
            this.Resize += new System.EventHandler(this.CameraControl_Resize);
            this.plControl.ResumeLayout(false);
            this.plControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRecordSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numToolSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.recordDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolBlockBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cameraControlBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bttnCameraLog;
        private System.Windows.Forms.Button bttnCameraAbort;
        private System.Windows.Forms.Button bttnCameraSnap;
        private System.Windows.Forms.Label lbCameraName;
        private System.Windows.Forms.Label lbAcqTime;
        private System.Windows.Forms.Label lbCameraDescription;
        private System.Windows.Forms.CheckBox cbCameraConnected;
        private System.Windows.Forms.CheckBox cbImageReady;
        private System.Windows.Forms.CheckBox cbArchiveImageActive;
        private System.Windows.Forms.Panel plControl;
        private System.Windows.Forms.Label lbToolName;
        private Cognex.VisionPro.CogRecordDisplay recordDisplay;
        private System.Windows.Forms.CheckBox cbToolPassed;
        private System.Windows.Forms.Label lbToolRunTime;
        private System.Windows.Forms.NumericUpDown numToolSelect;
        private System.Windows.Forms.CheckBox cbTrigger;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numRecordSelect;
        private System.Windows.Forms.Button bttnGetToolData;
        private System.Windows.Forms.CheckBox cbResultsUpdated;
        private System.Windows.Forms.Label lbRecordName;
        private System.Windows.Forms.Button bttnTest;
        private System.Windows.Forms.Panel plToolData;
        private System.Windows.Forms.BindingSource toolBlockBindingSource;
        private System.Windows.Forms.BindingSource cameraControlBindingSource;
    }
}
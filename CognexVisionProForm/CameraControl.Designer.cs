namespace CognexVisionProForm
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
            this.cogImageDisplay = new Cognex.VisionPro.Display.CogDisplay();
            this.bttnCameraLog = new System.Windows.Forms.Button();
            this.bttnCameraAbort = new System.Windows.Forms.Button();
            this.bttnCameraSnap = new System.Windows.Forms.Button();
            this.lbCameraName = new System.Windows.Forms.Label();
            this.plControl = new System.Windows.Forms.Panel();
            this.lbAcqTime = new System.Windows.Forms.Label();
            this.lbCameraDescription = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cogImageDisplay)).BeginInit();
            this.plControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // cogImageDisplay
            // 
            this.cogImageDisplay.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogImageDisplay.ColorMapLowerRoiLimit = 0D;
            this.cogImageDisplay.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogImageDisplay.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogImageDisplay.ColorMapUpperRoiLimit = 1D;
            this.cogImageDisplay.DoubleTapZoomCycleLength = 2;
            this.cogImageDisplay.DoubleTapZoomSensitivity = 2.5D;
            this.cogImageDisplay.Location = new System.Drawing.Point(5, 5);
            this.cogImageDisplay.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogImageDisplay.MouseWheelSensitivity = 1D;
            this.cogImageDisplay.Name = "cogImageDisplay";
            this.cogImageDisplay.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogImageDisplay.OcxState")));
            this.cogImageDisplay.Size = new System.Drawing.Size(451, 323);
            this.cogImageDisplay.TabIndex = 16;
            // 
            // bttnCameraLog
            // 
            this.bttnCameraLog.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.bttnCameraLog.FlatAppearance.BorderSize = 2;
            this.bttnCameraLog.Location = new System.Drawing.Point(3, 149);
            this.bttnCameraLog.Name = "bttnCameraLog";
            this.bttnCameraLog.Size = new System.Drawing.Size(152, 43);
            this.bttnCameraLog.TabIndex = 2;
            this.bttnCameraLog.Text = "Log Images";
            this.bttnCameraLog.UseVisualStyleBackColor = true;
            this.bttnCameraLog.Click += new System.EventHandler(this.bttnCameraLog_Click);
            // 
            // bttnCameraAbort
            // 
            this.bttnCameraAbort.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.bttnCameraAbort.FlatAppearance.BorderSize = 2;
            this.bttnCameraAbort.Location = new System.Drawing.Point(3, 100);
            this.bttnCameraAbort.Name = "bttnCameraAbort";
            this.bttnCameraAbort.Size = new System.Drawing.Size(152, 43);
            this.bttnCameraAbort.TabIndex = 1;
            this.bttnCameraAbort.Text = "Abort";
            this.bttnCameraAbort.UseVisualStyleBackColor = true;
            this.bttnCameraAbort.Click += new System.EventHandler(this.bttnCameraAbort_Click);
            // 
            // bttnCameraSnap
            // 
            this.bttnCameraSnap.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.bttnCameraSnap.FlatAppearance.BorderSize = 2;
            this.bttnCameraSnap.Location = new System.Drawing.Point(3, 51);
            this.bttnCameraSnap.Name = "bttnCameraSnap";
            this.bttnCameraSnap.Size = new System.Drawing.Size(152, 43);
            this.bttnCameraSnap.TabIndex = 0;
            this.bttnCameraSnap.Text = "Single Snap";
            this.bttnCameraSnap.UseVisualStyleBackColor = true;
            this.bttnCameraSnap.Click += new System.EventHandler(this.bttnCameraSnap_Click);
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
            // plControl
            // 
            this.plControl.Controls.Add(this.lbCameraDescription);
            this.plControl.Controls.Add(this.lbAcqTime);
            this.plControl.Controls.Add(this.lbCameraName);
            this.plControl.Controls.Add(this.bttnCameraSnap);
            this.plControl.Controls.Add(this.bttnCameraAbort);
            this.plControl.Controls.Add(this.bttnCameraLog);
            this.plControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.plControl.Location = new System.Drawing.Point(462, 0);
            this.plControl.Name = "plControl";
            this.plControl.Size = new System.Drawing.Size(158, 340);
            this.plControl.TabIndex = 18;
            // 
            // lbAcqTime
            // 
            this.lbAcqTime.AutoSize = true;
            this.lbAcqTime.Location = new System.Drawing.Point(3, 35);
            this.lbAcqTime.Name = "lbAcqTime";
            this.lbAcqTime.Size = new System.Drawing.Size(58, 13);
            this.lbAcqTime.TabIndex = 18;
            this.lbAcqTime.Text = "Acquisition";
            // 
            // lbCameraDescription
            // 
            this.lbCameraDescription.AutoSize = true;
            this.lbCameraDescription.Location = new System.Drawing.Point(3, 22);
            this.lbCameraDescription.Name = "lbCameraDescription";
            this.lbCameraDescription.Size = new System.Drawing.Size(99, 13);
            this.lbCameraDescription.TabIndex = 19;
            this.lbCameraDescription.Text = "Camera Description";
            // 
            // CameraControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(620, 340);
            this.ControlBox = false;
            this.Controls.Add(this.plControl);
            this.Controls.Add(this.cogImageDisplay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CameraControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CameraControl";
            this.Load += new System.EventHandler(this.CameraControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cogImageDisplay)).EndInit();
            this.plControl.ResumeLayout(false);
            this.plControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Cognex.VisionPro.Display.CogDisplay cogImageDisplay;
        private System.Windows.Forms.Button bttnCameraLog;
        private System.Windows.Forms.Button bttnCameraAbort;
        private System.Windows.Forms.Button bttnCameraSnap;
        private System.Windows.Forms.Label lbCameraName;
        private System.Windows.Forms.Panel plControl;
        private System.Windows.Forms.Label lbAcqTime;
        private System.Windows.Forms.Label lbCameraDescription;
    }
}
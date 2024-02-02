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

namespace CognexVisionProForm
{
    public partial class Form1
    {
        public void InitializeAcquisition()
        {
            Trace.AutoFlush = true;
            Trace.Indent();

            pollingTimer = new Timer();
            pollingTimer.Tick += new EventHandler(pollingTimer_Tick);
            pollingTimer.Interval = 200; // in miliseconds


            Camera01Calc = new Calculations();
            BlobCount = new ToolBlock("BlobCount", this);
            BlobCount2 = new ToolBlock("BlobCount2", this);
            Camera01Acq = new DalsaImage(this);
            MainPLC = new PlcComms(this);
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

                for (int i = 0; i < BlobCount.ToolOutput.Length; i++)
                {
                    lbToolData.Items.Add($"{BlobCount.ToolOutput[i].Name} - {BlobCount.ToolOutput[i].Value}");
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
    }
}

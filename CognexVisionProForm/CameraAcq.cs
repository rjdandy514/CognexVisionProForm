using Cognex.VisionPro;
using Cognex.VisionPro.Exceptions;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace CognexVisionProForm
{
    public class CameraAcq
    {
        ICogFrameGrabber FrameGrabber;
        ICogAcqFifo mAcqFifo = null;
        public ICogImage OutputImage;
        int numAcqs;
        string CameraName = null;
        string ImageSaveLocation;
        Form1 _form = new Form1();

        public CameraAcq(String UniqueName, Form1 Sender)
        {
            CameraName = UniqueName;
            
            ImageSaveLocation = "C:\\Users\\Robert.j.Dandy\\source\\repos\\CognexVisionProForm\\CognexVisionProForm\\Images\\Camera1_IDB.idb";
       
            _form = Sender;
        }

        public void ConnectFrameGrabber(ICogFrameGrabber input)
        {
            if (FrameGrabber != null) { FrameGrabber.Disconnect(false); }

            FrameGrabber = input;
            mAcqFifo = FrameGrabber.CreateAcqFifo(FrameGrabber.AvailableVideoFormats[0], CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);
            mAcqFifo.OwnedGigEVisionTransportParams.LatencyLevel = 1;

            mAcqFifo.Complete += new CogCompleteEventHandler(Subject_Complete);

        }

        public ICogImage AquireAndWait()
        {
            try
            {
                
                return mAcqFifo.Acquire(out int trignum);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null; 
            }
            
        }
        public void Aquire()
        {
            mAcqFifo.StartAcquire();
        }

        private  void Subject_Complete(object sender, EventArgs e)
        {
            CogAcqInfo info = new CogAcqInfo();

            try
            {
                int numReadyVal;
                int numPendingVal;
                bool busyVal;
                mAcqFifo.GetFifoState(out numPendingVal, out numReadyVal, out busyVal);              

                if (numReadyVal > 0)
                {
                    OutputImage = mAcqFifo.CompleteAcquireEx(info);
                }
                numAcqs++;

                if (numAcqs > 4)
                {
                    GC.Collect();
                    numAcqs = 0;
                }
                _form.Camera1TriggerToolBlock();

            }
            catch(CogException ce)
            {
                MessageBox.Show("The following error has occured\n" + ce.Message);
            }

        }

        public void CameraImageSave()
        {
            
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            Bitmap temp = OutputImage.ToBitmap();
            temp.Save(ImageSaveLocation +"\\"+ timestamp + ".bmp");
        }

        public void CameraClosing()
        {

            if (FrameGrabber != null) { FrameGrabber.Disconnect(false); }

        }


    }
}

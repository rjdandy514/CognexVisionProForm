using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using DALSA.SaperaLT.SapClassBasic;
using CognexVisionProForm;

class DalsaImage
{
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    public SapBuffer Buffers;

    public IntPtr BufferAddress;
    public SapAcqDevice acqDevice = null;
    public string m_ServerName;
    public int m_ResourceIndex;
    public SapLocation m_ServerLocation;
    public SapAcqDevice m_AcqDevice;
    public SapBuffer m_Buffers;
    public SapAcqDeviceToBuf m_Xfer;
    public SapView m_View;
    public object SelectedServer;

    Form1 _form = new Form1();

    public DalsaImage(Form1 Sender)
    {
        _form = Sender;
    }

    public void CreateBufferFromFile(string FileName)
    {
        // Allocate buffer with parameters compatible to file (does not load it)
        Buffers = new SapBuffer(FileName, SapBuffer.MemoryType.Default);
        // Create buffer object
        if (!Buffers.Create())
        {
            Cleaning();
            return;
        }
        // Load file
        if (!Buffers.Load(FileName, -1))
        {
            Cleaning();
            return;
        }
    }

    public void CreateCamera(string selectedServer, int selectedResource)
    {
        Cleaning();

        m_ServerLocation = new SapLocation(selectedServer, selectedResource);
        m_AcqDevice = new SapAcqDevice(m_ServerLocation, "");
        m_Buffers = new SapBufferWithTrash(4, m_AcqDevice, SapBuffer.MemoryType.ScatterGather);
        m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);    

        // End of frame event
        m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
        m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
        m_Xfer.XferNotifyContext = this;

        m_AcqDevice.Create();
        m_Buffers.Create();
        m_Xfer.Pairs[0].Cycle = SapXferPair.CycleMode.NextWithTrash;
        m_Xfer.Create();


    }

    public void SnapPicture()
    {
        m_Xfer.Snap();
        m_Xfer.Wait(2000);
    }

    public void GrabPicture()
    {
        m_Xfer.Grab();
    }

    public void Freeze()
    {
        m_Xfer.Freeze();
        m_Xfer.Wait(1000);
    }

    public void Abort()
    {
        m_Xfer.Abort();
    }

    public void SaveImageBMP(string FileName)
    {
        m_Buffers.Save(FileName, "-format bmp");
    }

    public void Cleaning()
    {
        if (m_Xfer != null)
        {
            m_Xfer.Destroy();
            m_Xfer.Dispose();
        }

        if (m_AcqDevice != null)
        {
            m_AcqDevice.Destroy();
            m_AcqDevice.Dispose();
        }

        if (Buffers != null)
        {
            Buffers.Destroy();
            Buffers.Dispose();
        }       
        if(m_View!=null)
        {
            m_View.Destroy();
            m_View.Dispose();
        }
    }

    public void xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
    {
        m_Buffers.GetAddress(BufferIndex, out BufferAddress);
        _form.Camera1TriggerToolBlock();
    }

    static void SapXferPair_XferNotify(Object sender, SapXferNotifyEventArgs args)
    {
        MessageBox.Show("Transfer Pair Notfy event handler");        
    }

    public int BufferIndex
    {
        get { return m_Buffers.Index; }
    }

}

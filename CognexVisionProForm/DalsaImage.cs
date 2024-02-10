using CognexVisionProForm;
using DALSA.SaperaLT.SapClassBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Cognex.VisionPro;
using Microsoft.Win32;

class DalsaImage
{

    IntPtr imageAddress;
    SapLocation m_ServerLocation;
    SapAcqDevice m_AcqDevice;
    SapAcquisition m_Acquisition;
    public SapBuffer m_Buffers;
    SapAcqDeviceToBuf m_AcqDeviceXfer;
    SapAcqToBuf m_AcqXfer;
    ServerCategory m_ServerCategory = ServerCategory.ServerAll;

    bool saveImageSelected;

    int imageWidth;
    int imageHeight;
    SapFormat imageFormat;
    
    Stopwatch acqTimeWatch;
    double acqTime;
    double snapTime;
    
    string configFile;
    string configFileLocation;
    string configFileType = "ConfigFile";
    string configFileExtension = ".cff";
    bool configFilePresent;
    string cameraName;

    Form1 form = new Form1();
    
    
    public DalsaImage(string CameraName, Form1 Form)
    {
        cameraName = CameraName;
        acqTimeWatch = new Stopwatch();
        form = Form;

        configFileLocation = form.ExeFilePath + "\\" + cameraName + "_" + configFileType + configFileExtension;
        if (File.Exists(configFileLocation)) 
        {
            configFilePresent = true;
            configFile = configFileLocation; 
        }
        else 
        {
            configFilePresent = false;
            configFile = ""; 
        }

        form.LoggingStatment(cameraName + ": DalsaImage Class created");
    }
    public double AcqTime
    {
        get { return acqTime; }
    }
    public double SnapTime
    {
        get { return snapTime; }
    }
    public int BufferIndex
    {
        get { return m_Buffers.Index; }
    }
    public string CongfigFile
    {
        get { return configFile; }
        set { configFile = value; }
    }
    public string CameraName
    {
        get { return cameraName; }
    }
    public bool Connected
    {
        get 
        {
            if (m_AcqDeviceXfer != null) { return m_AcqDeviceXfer.Connected && m_AcqDeviceXfer.Initialized; }
            else if (m_AcqXfer != null) { return m_AcqXfer.Connected && m_AcqXfer.Initialized; }
            else { return false; }
        }
    }
    public bool ConfigFilePresent
    {
        get => configFilePresent;
    }
    public enum ServerCategory
    {
        ServerAll,
        ServerAcq,
        ServerAcqDevice

    };
    public ServerCategory ServerType
    {
        get => m_ServerCategory;
    }
    public bool SaveImageSelected
    {
        get => saveImageSelected;
        set => saveImageSelected = value;
    }
    public void CreateBufferFromFile(string FileName)
    {
        // Allocate buffer with parameters compatible to file (does not load it)
        m_Buffers = new SapBuffer(FileName, SapBuffer.MemoryType.Default);
        // Create buffer object
        if (!m_Buffers.Create())
        {
            Cleaning();
            return;
        }
        // Load file
        if (!m_Buffers.Load(FileName, -1))
        {
            Cleaning();
            return;
        }
    }
    public void LoadConfigFile()
    {
        form.Import(cameraName, "ConfigFile", ".cff");
        configFile = configFileLocation;
        configFilePresent = true;

        form.LoggingStatment(cameraName + ": new log file from: " + configFile);

    }
    public void CreateCamera(string selectedServer, int selectedResource)
    {
        form.LoggingStatment($"{cameraName}: CreateCamera from {selectedServer}  {selectedResource} ");
        Cleaning();

        m_ServerLocation = new SapLocation(selectedServer, selectedResource);


        if (SapManager.GetResourceCount(m_ServerLocation, SapManager.ResourceType.Acq) > 0)
        {
            m_ServerCategory = ServerCategory.ServerAcq;

            m_Acquisition = new SapAcquisition(m_ServerLocation, configFile);
            m_Buffers = new SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGather);
            m_AcqXfer = new SapAcqToBuf(m_Acquisition, m_Buffers);

            // End of frame event
            m_AcqXfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfTransfer;
            m_AcqXfer.XferNotify += new SapXferNotifyHandler(XferNotify);
            m_AcqXfer.Pairs[0].Cycle = SapXferPair.CycleMode.NextWithTrash;
            m_AcqXfer.XferNotifyContext = this;

            // event for signal status
            m_Acquisition.SignalNotify += new SapSignalNotifyHandler(GetSignalStatus);
            m_Acquisition.SignalNotifyContext = this;
        }      
        else if (SapManager.GetResourceCount(m_ServerLocation, SapManager.ResourceType.AcqDevice) > 0)
        {
            m_ServerCategory = ServerCategory.ServerAcqDevice;
            m_AcqDevice = new SapAcqDevice(m_ServerLocation, configFile);
            m_Buffers = new SapBufferWithTrash(4, m_AcqDevice, SapBuffer.MemoryType.ScatterGather);
            m_AcqDeviceXfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);

            // End of frame event
            m_AcqDeviceXfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfTransfer;
            m_AcqDeviceXfer.XferNotify += new SapXferNotifyHandler(XferNotify);
            m_AcqDeviceXfer.XferNotifyContext = this;
            m_AcqDeviceXfer.Pairs[0].Cycle = SapXferPair.CycleMode.NextWithTrash;
        }


        if(m_Acquisition != null && !m_Acquisition.Initialized)
        {
            m_Acquisition.Create();
        }
        if (m_AcqDevice != null && !m_AcqDevice.Initialized)
        {
            m_AcqDevice.Create();
        }
        if (m_Buffers != null && !m_Buffers.Initialized)
        {
            m_Buffers.Create();
        }
        if (m_AcqDeviceXfer != null && !m_AcqDeviceXfer.Initialized)
        {
            m_AcqDeviceXfer.Create();
        }
    }
    public void SnapPicture()
    {
        acqTimeWatch.Start();
        if(m_AcqDeviceXfer.Connected)
        {
            if (m_AcqDeviceXfer.Snap())
            {
                m_AcqDeviceXfer.Wait(5000);
                snapTime = acqTimeWatch.ElapsedMilliseconds;
            }
        }
        else if(m_AcqXfer.Connected)
        {
            if (m_AcqXfer.Snap())
            {
                m_AcqXfer.Wait(5000);
                snapTime = acqTimeWatch.ElapsedMilliseconds;
            }
        }
        
    }
    public void GrabPicture()
    {
        if (m_AcqDeviceXfer.Connected)
        {
            m_AcqDeviceXfer.Grab();
            m_AcqDeviceXfer.Wait(3000);
        }
        else if (m_AcqXfer.Connected)
        {
            m_AcqXfer.Grab();
            m_AcqXfer.Wait(3000);
        }
    }
    public void Freeze()
    {
        if (m_AcqDeviceXfer.Connected)
        {
            m_AcqDeviceXfer.Freeze();
            m_AcqDeviceXfer.Wait(3000);
        }
        else if (m_AcqXfer.Connected)
        {
            m_AcqXfer.Freeze();
            m_AcqXfer.Wait(3000);
        }
    }
    public void Abort()
    {
        if (m_AcqDeviceXfer.Connected)
        {
            m_AcqDeviceXfer.Abort();
        }
        else if (m_AcqXfer.Connected)
        {
            m_AcqXfer.Abort();
        }
    }
    public void SaveImageBMP()
    {
        string ImageFilePath = form.ExeFilePath + "\\" + CameraName + "_Images\\";
        System.IO.Directory.CreateDirectory(ImageFilePath);

        DirectoryInfo ImageDirInfo = new DirectoryInfo(ImageFilePath);
        double ImageDirSize = form.DirSize(ImageDirInfo);
        string ImageFileName = "Image_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") +".bmp";
        if (ImageDirSize < 2000)
        {
            m_Buffers.Save(ImageFilePath + ImageFileName, "-format bmp");
        }
        else { MessageBox.Show($"{CameraName} Image folder has reached {ImageDirSize}MB"); }


        
        form.LoggingStatment($"{cameraName}: Save Image to BMP ");
    }
    public void Cleaning()
    {
        if (m_AcqDeviceXfer != null)
        {
            m_AcqDeviceXfer.Destroy();
            m_AcqDeviceXfer.Dispose();
        }

        if (m_AcqDevice != null)
        {
            m_AcqDevice.Destroy();
            m_AcqDevice.Dispose();
        }

        if (m_Buffers != null)
        {
            m_Buffers.Destroy();
            m_Buffers.Dispose();
        }       
    }
    public ICogImage RawToCogImage()
    {
        CogImage8Root NewImageRoot = new CogImage8Root();
        CogImage8Grey NewImage = new CogImage8Grey();

        NewImageRoot.Initialize(imageWidth, imageHeight, imageAddress, imageWidth, null);

        NewImage.SetRoot(NewImageRoot);

        return NewImage;
    }
    public ICogImage MarshalToCogImage()
    {
        
        CogImage8Root NewImageRoot = new CogImage8Root();
        CogImage8Grey NewImage8Grey = new CogImage8Grey();
        CogImage24PlanarColor NewImage24Color = new CogImage24PlanarColor();

        int managedArraySize = imageWidth * imageHeight;
        byte[] managedArray = new byte[managedArraySize];
        int size = Marshal.SizeOf(managedArray[0]) * managedArray.Length;
        IntPtr tempImageAddress = Marshal.AllocHGlobal(size);

        Marshal.Copy(imageAddress, managedArray, 0, size);
        Marshal.Copy(managedArray, 0, tempImageAddress, size);

        NewImageRoot.Initialize(imageWidth, imageHeight, tempImageAddress, imageWidth, null);

        if (imageFormat == SapFormat.Mono8)
        {
            NewImage8Grey.SetRoot(NewImageRoot);
            return NewImage8Grey;
        }
        else if (imageFormat == SapFormat.RGB888)
        {

            return NewImage24Color;
        }
        else
        {
            return NewImage24Color;
        }
    }
    public void XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
    {
        acqTime = acqTimeWatch.ElapsedMilliseconds;
        
        m_Buffers.GetAddress(BufferIndex, out imageAddress);
        
        imageWidth = m_Buffers.Width;
        imageHeight = m_Buffers.Height;
        imageFormat = m_Buffers.XferParams.Format;
        
        form.Camera1TriggerToolBlock();
        
        acqTimeWatch.Stop();
        acqTimeWatch.Reset();
    }
    static void GetSignalStatus(object sender, SapSignalNotifyEventArgs argsSignal)
    {
        SapAcquisition.AcqSignalStatus signalStatus = argsSignal.SignalStatus;
    }

    static void SapXferPair_XferNotify(Object sender, SapXferNotifyEventArgs args)
    {
        MessageBox.Show("Transfer Pair Notfy event handler");        
    }


}

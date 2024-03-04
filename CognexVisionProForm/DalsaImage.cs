using CognexVisionProForm;
using DALSA.SaperaLT.SapClassBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Cognex.VisionPro;
using Microsoft.Win32;
using System.Drawing;

public class DalsaImage
{

    IntPtr imageAddress;
    SapLocation serverLocation;
    SapAcqDevice acqDevice;
    SapAcquisition acquisition;
    SapBuffer buffers;
    SapBuffer archiveBuffers;
    SapAcqDeviceToBuf acqDeviceXfer;
    SapAcqToBuf acqXfer;
    ServerCategory serverCategory = ServerCategory.ServerAll;

    bool saveImageSelected;

    int id;

    ICogImage image;
    SapFormat imageFormat;
    bool imageReady;
    int imageWidth;
    int imageHeight;
    string imageFilePath;

    Stopwatch acqTimeWatch;
    double acqTime;
    double snapTime;

    string configFile;
    string configFileLocation;
    string configFileType = "ConfigFile";
    string configFileExtension = ".cff";
    bool configFilePresent;

    bool archiveImageActive;
    FileInfo[] archiveImageArray;
    int archiveImageCount;
    int archiveImageIndex;
    
    bool trigger;
    bool triggerMem = false;
    
    string cameraName;
    
    string loadServerSelect;
    string loadResourceName;
    int loadResourceIndex;


    Form1 form = new Form1();
    public DalsaImage(Form1 Form)
    {
        acqTimeWatch = new Stopwatch();
        form = Form;
        Utilities.LoggingStatment(cameraName + ": DalsaImage Class created");
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
        get {return buffers.Index; }
    }
    public string CongfigFile
    {
        get { return configFile; }
        set { configFile = value; }
    }
    public string Name
    {
        get { return cameraName; }
        set 
        { 
            cameraName = value;
            imageFilePath = Utilities.ExeFilePath + "\\Camera" + id.ToString("00")+ "\\Images\\";
            configFileLocation = Utilities.ExeFilePath + "\\Camera" + id.ToString("00") + "\\" + configFileType + configFileExtension;
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
        }
    }
    public string Description
    { get; set; }
    public int Id
    {
        get => id;
        set => id = value;
    }
    public bool Connected
    {
        get 
        {
            if (acqDeviceXfer != null) { return acqDeviceXfer.Connected && acqDeviceXfer.Initialized; }
            else if (acqXfer != null) { return acqXfer.Connected && acqXfer.Initialized; }
            else { return false; }
        }
    }
    public bool Trigger
    {
        get
        {
            return trigger;
        }
        set
        {
            trigger = value;
            if (trigger && !triggerMem) { SnapPicture(); }
            triggerMem = trigger;
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
        get => serverCategory;
    }
    public string LoadServerSelect
    {
        get => loadServerSelect;
        set => loadServerSelect = value;
    }
    public string LoadResourceName
    {
        get => loadResourceName;
        set => loadResourceName = value;
    }
    public int LoadResourceIndex
    {
        get => loadResourceIndex;
        set => loadResourceIndex = value;
    }
    public bool SaveImageSelected
    {
        get => saveImageSelected;
        set => saveImageSelected = value;
    }
    public bool ArchiveImageActive
    { 
        get => archiveImageActive; 
        set => archiveImageActive = value;
    }
    public int ArchiveImageCount
    {
        get 
        {
            FindArchivedImages();
            return archiveImageCount; 
        }
    }
    public int ArchiveImageIndex
    { 
        get { return archiveImageIndex; }
        set {  archiveImageIndex = value; }
    }
    public bool ImageReady
    {
        get => imageReady;
    }
    public ICogImage Image
    {
        get => image;
    }
    public void FindArchivedImages()
    {
        //Confirm Directory Exists
        if(!String.IsNullOrEmpty(imageFilePath))
        {

        }
        else
        {
            archiveImageCount = -1;
            return;
        }
        System.IO.Directory.CreateDirectory(imageFilePath);
        //Create Directory Type
        DirectoryInfo imageDirInfo = new DirectoryInfo(imageFilePath);
        //Create Array of names if files
        archiveImageArray = imageDirInfo.GetFiles();
        
        if (archiveImageArray.Length > 0) { archiveImageCount = archiveImageArray.Length; }
        else { archiveImageCount = -1; }

    }
    public void CreateBufferFromFile()
    {
        acqTimeWatch.Start();

        if (archiveImageCount == -1) {return; }
        if (archiveImageIndex > archiveImageCount - 1) { archiveImageIndex = 0; }
        if (archiveImageIndex < 0 ) { archiveImageIndex = archiveImageCount - 1; }

        string filename = imageFilePath + archiveImageArray[archiveImageIndex].Name;

        // Allocate buffer with parameters compatible to file (does not load it)
        archiveBuffers = new SapBuffer(filename, SapBuffer.MemoryType.Default);
        // Create buffer object
        if (!archiveBuffers.Create())
        {
            Cleaning();
            return;
        }
        // Load file
        if (!archiveBuffers.Load(filename, -1))
        {
            Cleaning();
            return;
        }

        UpdateImageData();

    }
    public void LoadConfigFile()
    {
        string filePath = Utilities.ExeFilePath + "\\Camera" + id.ToString("00");
        Utilities.Import(filePath,cameraName, "ConfigFile", ".cff");
        configFile = configFileLocation;
        configFilePresent = true;

        Utilities.LoggingStatment(cameraName + ": new log file from: " + configFile);

    }
    public void CreateCamera()
    {
        Utilities.LoggingStatment($"{cameraName}: CreateCamera from {loadServerSelect}  {loadResourceIndex} ");
        Cleaning();

        serverLocation = new SapLocation(loadServerSelect, loadResourceIndex);


        if (SapManager.GetResourceCount(serverLocation, SapManager.ResourceType.Acq) > 0)
        {
            serverCategory = ServerCategory.ServerAcq;

            acquisition = new SapAcquisition(serverLocation, configFile);
            buffers = new SapBufferWithTrash(2, acquisition, SapBuffer.MemoryType.ScatterGather);
            acqXfer = new SapAcqToBuf(acquisition, buffers);
            
            // End of frame event
            acqXfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfTransfer;
            acqXfer.XferNotify += new SapXferNotifyHandler(XferNotify);
            acqXfer.Pairs[0].Cycle = SapXferPair.CycleMode.NextWithTrash;
            acqXfer.XferNotifyContext = this;

            // event for signal status
            acquisition.SignalNotify += new SapSignalNotifyHandler(GetSignalStatus);
            acquisition.SignalNotifyContext = this;
        }      
        else if (SapManager.GetResourceCount(serverLocation, SapManager.ResourceType.AcqDevice) > 0)
        {
            serverCategory = ServerCategory.ServerAcqDevice;
            acqDevice = new SapAcqDevice(serverLocation, configFile);
            buffers = new SapBufferWithTrash(4, acqDevice, SapBuffer.MemoryType.ScatterGather);
            acqDeviceXfer = new SapAcqDeviceToBuf(acqDevice, buffers);

            // End of frame event
            acqDeviceXfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfTransfer;
            acqDeviceXfer.XferNotify += new SapXferNotifyHandler(XferNotify);
            acqDeviceXfer.XferNotifyContext = this;
            acqDeviceXfer.Pairs[0].Cycle = SapXferPair.CycleMode.NextWithTrash;
        }


        if(acquisition != null && !acquisition.Initialized)
        {
            acquisition.Create();
        }
        if (acqDevice != null && !acqDevice.Initialized)
        {
            acqDevice.Create();
        }
        if (buffers != null && !buffers.Initialized)
        {
            archiveImageActive = false;
            buffers.Create();
        }
        if (acqDeviceXfer != null && !acqDeviceXfer.Initialized)
        {
            acqDeviceXfer.Create();
        }
    }
    public void SnapPicture()
    {
        imageReady = false;

        acqTimeWatch.Start();
        if(acqDeviceXfer.Connected)
        {
            if (acqDeviceXfer.Snap())
            {
                
                snapTime = acqTimeWatch.ElapsedMilliseconds;
                acqDeviceXfer.Wait(5000);
            }
        }
        else if(acqXfer.Connected)
        {
            if (acqXfer.Snap())
            {
                acqXfer.Wait(5000);
                snapTime = acqTimeWatch.ElapsedMilliseconds;
            }
        }
    }
    public void Abort()
    {
        if (acqDeviceXfer.Connected)
        {
            acqDeviceXfer.Abort();
        }
        else if (acqXfer.Connected)
        {
            acqXfer.Abort();
        }
        imageReady = true;
    }
    public void SaveImageBMP()
    {
        System.IO.Directory.CreateDirectory(imageFilePath);

        DirectoryInfo ImageDirInfo = new DirectoryInfo(imageFilePath);
        double ImageDirSize = Utilities.DirSize(ImageDirInfo);
        string ImageFileName = "Image_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") +".bmp";
        if (ImageDirSize < 2000)
        {
            buffers.Save(imageFilePath + ImageFileName, "-format bmp");
        }
        else { MessageBox.Show($"{Name} Image folder has reached {ImageDirSize}MB"); }


        
        Utilities.LoggingStatment($"{cameraName}: Save Image to BMP ");
    }
    public void Disconnect()
    {
        if (acqDeviceXfer != null)
        {
            acqDeviceXfer.Disconnect();
            acqDeviceXfer.Destroy();
        }

        if (acqDevice != null)
        {
            acqDevice.Destroy();
        }
    }
    public void Cleaning()
    {
        if (acqDeviceXfer != null)
        {
            acqDeviceXfer.Destroy();
            acqDeviceXfer.Dispose();
        }

        if (acqDevice != null)
        {
            acqDevice.Destroy();
            acqDevice.Dispose();
        }

        if (buffers != null)
        {
            buffers.Destroy();
            buffers.Dispose();
        }
        if (archiveBuffers != null)
        {
            archiveBuffers.Destroy();
            archiveBuffers.Dispose();
        }
    }
    public void UpdateImageData()
    {
        acqTime = acqTimeWatch.ElapsedMilliseconds;

        // logic for using Camera
        if (buffers != null) 
        { 
            buffers.GetAddress(buffers.Index, out imageAddress);
            imageWidth = buffers.Width;
            imageHeight = buffers.Height;
            imageFormat = buffers.XferParams.Format;
        }
        //logic for using saved images
        else if(archiveBuffers != null)
        {
            archiveBuffers.GetAddress(archiveBuffers.Index, out imageAddress);
            imageWidth = archiveBuffers.Width;
            imageHeight = archiveBuffers.Height;
            imageFormat = archiveBuffers.XferParams.Format;
        }

        //Save Dalsa Image to Cog Image
        image = MarshalToCogImage();

        // Save Image as BMP to pre-defined location
        if (buffers != null && saveImageSelected) { SaveImageBMP(); }

        imageReady = true;
        form.CameraSnapComplete = true;

        acqTimeWatch.Stop();
        acqTimeWatch.Reset();
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
        UpdateImageData();
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

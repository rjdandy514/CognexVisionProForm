﻿using CognexVisionProForm;
using DALSA.SaperaLT.SapClassBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Cognex.VisionPro;
using Microsoft.Win32;
using System.Drawing;
using System.Threading.Tasks;

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

    SapFormat imageFormat;
    int imageWidth;
    int imageHeight;
    string imageFilePath;

    Stopwatch acqTimeWatch;

    string configFileLocation;
    string configFileType = "ConfigFile";
    string configFileExtension = ".cff";


    FileInfo[] archiveImageArray;
    int archiveImageCount;
    
    bool trigger;
    bool triggerMem = false;

    bool abort;
    bool abortMem = false;
    
    string cameraName;
    

    Form1 form = new Form1();
    public DalsaImage(Form1 Form)
    {
        acqTimeWatch = new Stopwatch();
        form = Form;
        Utilities.LoggingStatment(cameraName + ": DalsaImage Class created");
    }
    public double AcqTime
    {
        get;set;
    }
    public double SnapTime
    {
        get;set;
    }
    public int BufferIndex
    {
        get {return buffers.Index; }
    }
    public string CongfigFile
    {
        get; set;
    }
    public string Name
    {
        get { return cameraName; }
        set 
        { 
            cameraName = value;
            imageFilePath = Utilities.ExeFilePath + "\\Camera" + Id.ToString("00")+ "\\Images\\";
            configFileLocation = Utilities.ExeFilePath + "\\Camera" + Id.ToString("00") + "\\" + configFileType + configFileExtension;
            if (File.Exists(configFileLocation))
            {
                ConfigFilePresent = true;
                CongfigFile = configFileLocation;
            }
            else
            {
                ConfigFilePresent = false;
                CongfigFile = "";
            }
        }
    }
    public string Description
    { get; set; }
    public int Id
    {
        get; set;
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
            if (trigger && !triggerMem) 
            { 
                if(Connected)
                {
                    SnapPicture();
                }
                else if(ArchiveImageActive)
                {
                    CreateBufferFromFile();
                    ArchiveImageIndex++;
                }
                 
            }
            triggerMem = trigger;
        }
    }
    public bool AbortTrigger
    {
        get { return abort; }
        set
        {
            abort = value;
            if (abort && !abortMem)
            {
                Abort();
            }
            abortMem = abort;
        }
    }
    public bool ConfigFilePresent
    {
        get; set;
    }
    public enum ServerCategory
    {
        ServerAll,
        ServerAcq,
        ServerAcqDevice

    };
    public ServerCategory ServerType
    {
        get; set;
    }
    public string LoadServerSelect
    {
        get; set;
    }
    public string LoadResourceName
    {
        get; set;
    }
    public int LoadResourceIndex
    {
        get; set;
    }
    public bool SaveImageSelected
    {
        get;set;
    }
    public bool ArchiveImageActive
    {
        get; set;
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
        get; set;
    }
    public bool ImageReady
    {
        get; set;
    }
    public ICogImage Image
    {
        get;set;
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
    public async void CreateBufferFromFile()
    {
        acqTimeWatch.Start();
        ImageReady = false;

        if (archiveImageCount == -1) {return; }
        if (ArchiveImageIndex > archiveImageCount - 1) { ArchiveImageIndex = 0; }
        if (ArchiveImageIndex < 0 ) { ArchiveImageIndex = archiveImageCount - 1; }

        string filename = imageFilePath + archiveImageArray[ArchiveImageIndex].Name;

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
        await Task.Delay(1000);
        //System.Threading.Thread.Sleep(1000);

        UpdateImageData();

    }
    public void LoadConfigFile()
    {
        string filePath = Utilities.ExeFilePath + "\\Camera" + Id.ToString("00");
        Utilities.Import(filePath,cameraName, "ConfigFile", ".cff");
        CongfigFile = configFileLocation;
        ConfigFilePresent = true;

        Utilities.LoggingStatment(cameraName + ": new log file from: " + CongfigFile);

    }
    public void CreateCamera()
    {
        Utilities.LoggingStatment($"{cameraName}: CreateCamera from {LoadServerSelect}  {LoadResourceIndex} ");
        Cleaning();

        serverLocation = new SapLocation(LoadServerSelect, LoadResourceIndex);


        if (SapManager.GetResourceCount(serverLocation, SapManager.ResourceType.Acq) > 0)
        {
            ServerType = ServerCategory.ServerAcq;

            acquisition = new SapAcquisition(serverLocation, CongfigFile);
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
            ServerType = ServerCategory.ServerAcqDevice;
            acqDevice = new SapAcqDevice(serverLocation, CongfigFile);
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
            ArchiveImageActive = false;
            buffers.Create();
        }
        if (acqDeviceXfer != null && !acqDeviceXfer.Initialized)
        {
            acqDeviceXfer.Create();
        }
    }
    public void SnapPicture()
    {
        ImageReady = false;

        acqTimeWatch.Start();
        if(acqDeviceXfer.Connected)
        {
            if (acqDeviceXfer.Snap())
            {
                
                SnapTime = acqTimeWatch.ElapsedMilliseconds;
                acqDeviceXfer.Wait(5000);
            }
        }
        else if(acqXfer.Connected)
        {
            if (acqXfer.Snap())
            {
                acqXfer.Wait(5000);
                SnapTime = acqTimeWatch.ElapsedMilliseconds;
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
        ImageReady = true;
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
        AcqTime = acqTimeWatch.ElapsedMilliseconds;

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
        Image = MarshalToCogImage();

        // Save Image as BMP to pre-defined location
        if (buffers != null && SaveImageSelected) { SaveImageBMP(); }

        ImageReady = true;
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

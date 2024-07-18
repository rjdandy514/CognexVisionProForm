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
using System.Threading.Tasks;
using Cognex.Vision;
using System.CodeDom.Compiler;
using System.Windows.Forms.Design.Behavior;
using System.Threading;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices.ComTypes;

public class DalsaImage
{

    IntPtr imageAddress;
    SapLocation serverLocation;
    SapAcqDevice acqDevice;
    SapAcquisition acquisition;
    SapAcqDevice acqDeviceData;
    SapBuffer buffers;
    SapBuffer archiveBuffers;
    SapAcqDeviceToBuf acqDeviceXfer;
    SapAcqToBuf acqXfer;
    SapFeature deviceFeature;

    SapFormat imageFormat;
    int imageWidth;
    int imageHeight;
    string imageFilePath;

    Stopwatch acqTimeWatch;
    double acqTime = 0;
    double snapTime = 0;
    double startOfFrameTime = 0;
    int startOfFrame = 0;
    string configFileType = "ConfigFile";
    string configFileExtension = ".ccf";
    string serialNumber = "";

    IntPtr cogImageAddress;

    FileInfo[] archiveImageArray;
    int archiveImageCount;

    bool trigger = false;
    bool triggerMem = false;
    bool triggerGrab = false;
    bool triggerGrabMem = false;

    bool abort;
    bool abortMem = false;

    bool snapping = false;
    bool grabbing = false;
    bool acquiring = false;


    string cameraName;


    CognexVisionProForm.CognexVisionProForm form = new CognexVisionProForm.CognexVisionProForm();
    public DalsaImage(CognexVisionProForm.CognexVisionProForm Form)
    {
        acqTimeWatch = new Stopwatch();
        form = Form;
        Name = "";
        ConfigFile = "";
        Description = "";
        Utilities.LoggingStatment(Name + ": Dalsa Image Class created");
    }
    public double AcqTime
    {
        get
        {
            return acqTime;
        }
    }
    public double SnapTime
    {
        get
        {
            return snapTime;
        }
    }
    public int BufferIndex
    {
        get { return buffers.Index; }
    }
    public string ConfigFile
    {
        get; set;
    }
    public string Name
    {
        get { return cameraName; }
        set
        {
            cameraName = value;
            imageFilePath = Utilities.ExeFilePath + "\\Camera" + Id.ToString("00") + "\\Images\\";
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
        get { return trigger; }
        set
        {

            trigger = value;


            if (Connected && trigger && !triggerMem)
            {
                SnapPicture();
                form.CameraSnap = Id;
            }
            else if (ArchiveImageActive)
            {
                CreateBufferFromFile();
                ArchiveImageIndex++;
                form.CameraSnap = Id;
            }

            triggerMem = trigger;

        }
    }
    public bool TriggerGrab
    {
        get { return triggerGrab; }
        set
        {
            triggerGrab = value;

            if (Connected && triggerGrab && !triggerGrabMem && !ArchiveImageActive)
            {
                GrabPicture();
            }
            triggerGrabMem = triggerGrab;
        }
    }
    public bool TriggerAck
    {
        get
        {
            return trigger;
        }
    }
    public bool AbortTrigger
    {
        get { return abort; }
        set
        {
            abort = value;
            AbortTriggerAck = abort;
            if (abort && !abortMem)
            {
                Abort();
            }
            abortMem = abort;
        }
    }
    public bool AbortTriggerAck
    {
        get; set;
    }
    public bool ConfigFilePresent
    {
        get; set;
    }
    public bool Snapping
    {
        get { return snapping; }

    }
    public bool Grabbing
    {
        get 
        { 
            return grabbing; 
        }
    }
    public bool Acquiring
    {
        get { return acquiring; }
    }
    public int IsMaster
    {
        get
        {
            int returnGetParm = 0;
            bool returngetResult = acquisition.GetParameter(SapAcquisition.Prm.BOARD_SYNC_OUTPUT1_SOURCE, out returnGetParm);
            return returnGetParm;
        }
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
    public string GetSerialNumber
    {
        get
        {
            return SapManager.GetSerialNumber(serverLocation);
        }
    }
    public int LoadResourceIndex
    {
        get; set;
    }
    public bool SaveImageSelected
    {
        get; set;
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
        get; set;
    }
    public void FindArchivedImages()
    {
        //Confirm Directory Exists
        if (!String.IsNullOrEmpty(imageFilePath))
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

        if (archiveImageCount == -1) { return; }
        if (ArchiveImageIndex > archiveImageCount - 1) { ArchiveImageIndex = 0; }
        if (ArchiveImageIndex < 0) { ArchiveImageIndex = archiveImageCount - 1; }

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
        if (Utilities.Import(filePath, configFileType, configFileExtension))
        {
            ConfigFile = filePath + "\\" + configFileType + configFileExtension;
        }
        else { ConfigFile = ""; }

        ConfigFilePresent = File.Exists(ConfigFile);

        Utilities.LoggingStatment(cameraName + ": new log file from: " + ConfigFile);
    }
    public void CreateCamera()
    {
        Utilities.LoggingStatment($"{cameraName}: Create Camera from {LoadServerSelect}  {LoadResourceIndex} ");

        serverLocation = new SapLocation(LoadServerSelect, LoadResourceIndex);

        bool acq0SupportSG = SapBuffer.IsBufferTypeSupported(serverLocation, SapBuffer.MemoryType.ScatterGather);
        bool acq0SupportSGP = SapBuffer.IsBufferTypeSupported(serverLocation, SapBuffer.MemoryType.ScatterGatherPhysical);

        string configFile = "";
        if (ConfigFilePresent) { configFile = ConfigFile; }

        if (SapManager.GetResourceCount(serverLocation, SapManager.ResourceType.Acq) > 0)
        {
            ServerType = ServerCategory.ServerAcq;

            acquisition = new SapAcquisition(serverLocation, configFile);
            acqDeviceData = new SapAcqDevice(serverLocation, "");

            if (acq0SupportSG)
            {
                buffers = new SapBufferWithTrash(4, acquisition, SapBuffer.MemoryType.ScatterGather);
            }
            else if (acq0SupportSGP)
            {
                buffers = new SapBufferWithTrash(4, acquisition, SapBuffer.MemoryType.ScatterGatherPhysical);
            }
            acqXfer = new SapAcqToBuf(acquisition, buffers);

            // End of frame event (End Of Frame)
            acqXfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            acqXfer.XferNotify += new SapXferNotifyHandler(XferNotify);
            acqXfer.Pairs[0].Cycle = SapXferPair.CycleMode.NextWithTrash;
            acqXfer.XferNotifyContext = this;

            // event fot Acqcallback (Start Of Frame)
            acquisition.EventType = SapAcquisition.AcqEventType.StartOfFrame;
            acquisition.AcqNotify += new SapAcqNotifyHandler(StartOfFrameEvent);
            acquisition.AcqNotifyContext = this;

            // event for signal status
            acquisition.SignalNotify += new SapSignalNotifyHandler(GetSignalStatus);
            acquisition.SignalNotifyContext = this;
        }
        else if (SapManager.GetResourceCount(serverLocation, SapManager.ResourceType.AcqDevice) > 0)
        {
            ServerType = ServerCategory.ServerAcqDevice;

            acqDevice = new SapAcqDevice(serverLocation, configFile);

            if (acq0SupportSG)
            {
                buffers = new SapBufferWithTrash(4, acqDevice, SapBuffer.MemoryType.ScatterGather);
            }
            else if (acq0SupportSGP)
            {
                buffers = new SapBufferWithTrash(4, acqDevice, SapBuffer.MemoryType.ScatterGatherPhysical);
            }

            acqDeviceXfer = new SapAcqDeviceToBuf(acqDevice, buffers);

            // End of frame event
            acqDeviceXfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            acqDeviceXfer.XferNotify += new SapXferNotifyHandler(XferNotify);
            acqDeviceXfer.Pairs[0].Cycle = SapXferPair.CycleMode.NextWithTrash;
            acqDeviceXfer.XferNotifyContext = this;
        }


        if (acquisition != null && !acquisition.Initialized)
        {
            if (acquisition.Create() == false)
            {
                Destroy();
                return;
            }
        }
        if (acqDevice != null && !acqDevice.Initialized)
        {
            if (acqDevice.Create() == false)
            {
                Destroy();
                return;
            }
        }
        if (acqDeviceData != null && !acqDeviceData.Initialized)
        {
            if (acqDeviceData.Create() == false)
            {
                Destroy();
                return;
            }
        }
        if (buffers != null && !buffers.Initialized)
        {
            ArchiveImageActive = false;
            if (buffers.Create() == false)
            {
                Destroy();
                return;
            }
        }
        if (acqDeviceXfer != null && !acqDeviceXfer.Initialized)
        {
            if (acqDeviceXfer.Create() == false)
            {
                Destroy();
                return;
            }
        }
        if (acqXfer != null && !acqXfer.Initialized)
        {
            if (acqXfer.Create() == false)
            {
                Destroy();
                return;
            }
        }
        if(acquisition.Initialized)
        {
            CheckCameraFeatures();
        }

    }
    public void SnapPicture()
    {
        ImageReady = false;
        acqTimeWatch.Restart();

        if (acqDeviceXfer != null && acqDeviceXfer.Connected)
        {
            if (acqDeviceXfer.Snap())
            {
                snapTime = acqTimeWatch.ElapsedMilliseconds;
                acqDeviceXfer.Wait(5000);
                snapping = true;
            }
        }
        else if (acqXfer != null && acqXfer.Connected)
        {
            if (acqXfer.Snap())
            {
                //acqXfer.Wait(5000);

                snapTime = acqTimeWatch.ElapsedMilliseconds;
                snapping = true;
            }
        }
    }
    public void GrabPicture()
    {
        ImageReady = false;

        if (acqDeviceXfer != null && acqDeviceXfer.Connected)
        {
            if (acqDeviceXfer.Grab())
            {
                grabbing = true;
            }
        }
        else if (acqXfer != null && acqXfer.Connected)
        {
            if (acqXfer.Grab())
            {
                grabbing = true;
            }
        }
    }
    public void Abort()
    {
        if (acqDeviceXfer != null && acqDeviceXfer.Connected)
        {
            if (acqDeviceXfer.Grabbing) { acqDeviceXfer.Freeze(); }
            acqDeviceXfer.Abort();
        }
        else if (acqXfer != null && acqXfer.Connected)
        {
            if (acqXfer.Grabbing) { acqXfer.Freeze(); }
            acqXfer.Abort();
        }
        snapping = false;
        grabbing = false;
        acquiring = false;
        ImageReady = true;
    }
    public void SaveImageBMP()
    {
        System.IO.Directory.CreateDirectory(imageFilePath);

        DirectoryInfo ImageDirInfo = new DirectoryInfo(imageFilePath);
        double ImageDirSize = Utilities.DirSize(ImageDirInfo);
        string ImageFileName = "Image_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".bmp";
        if (ImageDirSize < 10000)
        {
            buffers.Save(imageFilePath + ImageFileName, "-format bmp");
        }
        else { MessageBox.Show($"{Name} Image folder has reached {ImageDirSize}MB"); }



        Utilities.LoggingStatment($"{cameraName}: Save Image to BMP ");
    }
    public void Disconnect()
    {
        if (acqXfer != null) { acqXfer.Disconnect(); }
        if (acqDeviceXfer != null) { acqDeviceXfer.Disconnect(); }
        if (acqDevice != null) { acqDevice.Destroy(); }

    }
    public void Cleaning()
    {
        if (acqDeviceXfer != null)
        {
            acqDeviceXfer.Destroy();
            acqDeviceXfer.Dispose();
        }

        if (acquisition != null)
        {
            acquisition.Destroy();
            acquisition.Dispose();
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
        Marshal.FreeHGlobal(cogImageAddress);
    }
    public void Destroy()
    {
        if (acqXfer != null && acqXfer.Initialized) { acqXfer.Destroy(); }
        if (acqDeviceXfer != null && acqDeviceXfer.Initialized) { acqDeviceXfer.Destroy(); }
        if (acqDevice != null && acqDevice.Initialized) { acqDevice.Destroy(); }
        if (acqDeviceData != null && acqDeviceData.Initialized) { acqDeviceData.Destroy(); }
        if (buffers != null && buffers.Initialized) { buffers.Destroy(); }
        if (archiveBuffers != null && archiveBuffers.Initialized) { archiveBuffers.Destroy(); }
        if (acquisition != null && acquisition.Initialized) { acquisition.Destroy(); }
    }
    public void Dispose()
    {
        if (acqXfer != null) { acqXfer.Dispose(); }
        if (acqDeviceXfer != null) { acqDeviceXfer.Dispose(); }
        if (acqDevice != null) { acqDevice.Dispose(); }
        if (acqDeviceData != null) { acqDeviceData.Dispose(); }
        if (buffers != null) { buffers.Dispose(); }
        if (archiveBuffers != null) { archiveBuffers.Dispose(); }
        if (acquisition != null) { acquisition.Dispose(); }

        acqXfer = null;
        acqDeviceXfer = null;
        acqDevice = null;
        acqDeviceData = null;
        buffers = null;
        archiveBuffers = null;
        acquisition = null;
    }
    public void UpdateImageData()
    {

        acqTime = acqTimeWatch.ElapsedMilliseconds - startOfFrameTime;

        // logic for using Camera
        if (buffers != null && Connected)
        {
            buffers.GetAddress(buffers.Index, out imageAddress);
            imageWidth = buffers.Width;
            imageHeight = buffers.Height;
            imageFormat = buffers.XferParams.Format;
        }
        //logic for using saved images
        else if (archiveBuffers != null && ArchiveImageActive)
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

        acqTimeWatch.Stop();
        acqTimeWatch.Reset();
        snapping = false;
        acquiring = false;
        form.CameraSnapComplete = Id;
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

        if (cogImageAddress.ToString().Equals("0"))
        {
            cogImageAddress = Marshal.AllocHGlobal(size);
        }

        //cogImageAddress = Marshal.AllocHGlobal(size);


        Marshal.Copy(imageAddress, managedArray, 0, size);
        Marshal.Copy(managedArray, 0, cogImageAddress, size);
        NewImageRoot.Initialize(imageWidth, imageHeight, cogImageAddress, imageWidth, null);

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
    public int EncoderPhase()
    {
        bool getResult;
        bool setResult;
        bool checkResult;

        int returnGetParm = 0;
        int returnCheckParm = 0;
        int returnTempParm = 0;

        int phaseA = 1;
        int phaseB = 2;
        if (acquisition == null) { return 0; }


        getResult = acquisition.GetParameter(SapAcquisition.Prm.EXT_LINE_TRIGGER_SOURCE, out returnGetParm);

        if (getResult && (returnGetParm == phaseA || returnGetParm == phaseB))
        {
            if (returnGetParm == phaseA)
            {
                setResult = acquisition.SetParameter(SapAcquisition.Prm.EXT_LINE_TRIGGER_SOURCE, phaseB, true);
            }
            if (returnGetParm == phaseB)
            {
                setResult = acquisition.SetParameter(SapAcquisition.Prm.EXT_LINE_TRIGGER_SOURCE, phaseA, true);
            }
        }
        
        checkResult = acquisition.GetParameter(SapAcquisition.Prm.EXT_LINE_TRIGGER_SOURCE, out returnCheckParm);

        return returnCheckParm;
    }
    public void CheckCameraFeatures()
    {
        bool getFeatureInfo = false;
        bool getFeatureValueResult = false;
        bool getEnumTextFromValueResult = false;
        bool setFeatureValueResult = false ;
        int enumValue;
        string enumString = "";
        string userSetSelector = "UserSetSelector";
        string userSetDefaultSelector = "UserSetDefaultSelector";
        string userSetLoad = "UserSetLoad";
        string triggerMode = "TriggerMode";
        string userSetSave = "UserSetSave";


        if (deviceFeature == null) { deviceFeature = new SapFeature(serverLocation); }
        if (deviceFeature != null && !deviceFeature.Initialized) { deviceFeature.Create(); }

        int numberOfFeatures = acqDeviceData.FeatureCount;
        string[] features = new string[numberOfFeatures];

        //get full List of Features Names
        for (int i = 0; i < features.Length; i++)
        {
            features[i] = acqDeviceData.FeatureNames[i];
        }

        //Find and Set UserSet
        getFeatureInfo = acqDeviceData.GetFeatureInfo(userSetSelector, deviceFeature);
        getFeatureValueResult = acqDeviceData.GetFeatureValue(userSetSelector, out enumValue);
        getEnumTextFromValueResult = deviceFeature.GetEnumTextFromValue(enumValue, out enumString);

        if(enumString != "UserSet1")
        {
            //Set UserSetSelect,UserSetDefaultSelect to UserSet1
            //Load select User Set
            setFeatureValueResult = acqDeviceData.SetFeatureValue(userSetDefaultSelector, 1);
            setFeatureValueResult = acqDeviceData.SetFeatureValue(userSetSelector, 1);
            setFeatureValueResult = acqDeviceData.SetFeatureValue(userSetLoad, true);
        }

        //Finding value of trigger mode
        getFeatureInfo = acqDeviceData.GetFeatureInfo(triggerMode, deviceFeature);
        getFeatureValueResult = acqDeviceData.GetFeatureValue(triggerMode, out enumValue);
        getEnumTextFromValueResult = deviceFeature.GetEnumTextFromValue(enumValue, out enumString);

        if(enumString != "External")
        {
            //Set TriggerMode to External
            //Save Values to UserSet1
            setFeatureValueResult = acqDeviceData.SetFeatureValue(triggerMode, 1);
            setFeatureValueResult = acqDeviceData.SetFeatureValue(userSetSave, true);
        }

    }
    public void XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
    {
        if (argsNotify.Trash) { return; }
        else{ UpdateImageData(); }

    }
    static void GetSignalStatus(object sender, SapSignalNotifyEventArgs argsSignal)
    {
        SapAcquisition.AcqSignalStatus signalStatus = argsSignal.SignalStatus;
    }
    public void StartOfFrameEvent(object sender, SapAcqNotifyEventArgs argsSignal)
    {

            snapping = false;
            acquiring = true;
            startOfFrameTime = acqTimeWatch.ElapsedMilliseconds;

    }




}

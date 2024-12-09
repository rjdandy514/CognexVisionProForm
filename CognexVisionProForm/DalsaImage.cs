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
using System.Text;
using System.Runtime.CompilerServices;

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

    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

    static string ConfigKeyName = "Camera Name";
    static string CompanyKeyName = "Company Name";
    static string ModelKeyName = "Model Name";
    static string VicKeyName = "Vic Name";

    StringBuilder tempServerName = new StringBuilder(512);
    StringBuilder sbCameraName = new StringBuilder(512);
    StringBuilder sbCompanyName = new StringBuilder(512);
    StringBuilder sbModelName = new StringBuilder(512);
    StringBuilder sbVicName = new StringBuilder(512);

    string companyName;
    string modelName;
    string vicName;
    string deviceUserID;
    string serialNumber;

    SapFormat imageFormat;
    int imageWidth;
    int imageHeight;
    string imageFilePath;

    Stopwatch acqTimeWatch;
    double acqTime = 0;
    double snapTime = 0;
    double startOfFrameTime = 0;
    string configFileType = "ConfigFile";
    string configFileExtension = ".ccf";

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

    bool limitReached = false;

    int isMaster;

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
    public string PartSerialNumber 
    { get; set; }
    public string PartType
    {  get; set; }
    public string ConfigFile
    {
        get; set;
    }
    public string CompanyName 
    { get
        { return companyName; }
    }
    public string ModelName { get => modelName; }
    public string Vicname { get => vicName; }
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
    public string SerialNumber 
    {
        get { return serialNumber; }
        set { serialNumber = value; }
    }
    public int CropLeft
    { get; set; }
    public int CropWidth
    { get; set; }
    public int Flip
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

            if(trigger && !triggerMem)
            {
                form.CameraSnap = Id;
                ImageReady = false;
                if (Connected)
                {
                    SnapPicture();
                    
                }
                else if (ArchiveImageActive)
                {
                    CreateBufferFromFile();
                    ArchiveImageIndex++;
                }
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
            if (abort && !abortMem)
            {
                Abort();
            }
            abortMem = abort;
        }
    }
    public bool AbortTriggerAck
    {
        get
        {
            return abort;
        }

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
    public bool LimitReached
    {
        get { return limitReached; }
    }
    public int IsMaster
    {
        get
        {
            int returnGetParm = 0;

            if (acquisition != null)
            {
                bool returngetResult = acquisition.GetParameter(SapAcquisition.Prm.BOARD_SYNC_OUTPUT1_SOURCE, out returnGetParm);
                isMaster = returnGetParm;
            }
            else { isMaster = 0; }
            return isMaster;
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
            Destroy();
            Dispose();
            return;
        }
        // Load file
        if (!archiveBuffers.Load(filename, -1))
        {
            Destroy();
            Dispose();
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
    public void GetConfigFileInfo()
    {
        /*
         * Get information about the config file to confirm correct one is loaded.
         * information is create from CamExpert when saving the config file for the
         * first time
        */
        var configFileInfo = new FileInfo(ConfigFile);
        
        StringBuilder tempServerName = new StringBuilder(512);
        StringBuilder sbCameraName = new StringBuilder(512);
        StringBuilder sbCompanyName = new StringBuilder(512);
        StringBuilder sbModelName = new StringBuilder(512);
        StringBuilder sbVicName = new StringBuilder(512);


        GetPrivateProfileString("General", CompanyKeyName, "", sbCompanyName, 512, ConfigFile);
        GetPrivateProfileString("General", ModelKeyName, "", sbModelName, 512, ConfigFile);
        GetPrivateProfileString("General", VicKeyName, "", sbVicName, 512, ConfigFile);

        companyName = sbCompanyName.ToString(0, sbCompanyName.Length);
        modelName = sbModelName.ToString(0, sbModelName.Length);
        vicName = sbVicName.ToString(0, sbVicName.Length);
    }
    public void CreateCamera()
    {
        Utilities.LoggingStatment($"{cameraName}: Create Camera from {LoadServerSelect}  {LoadResourceIndex} ");

        serverLocation = new SapLocation(LoadServerSelect, LoadResourceIndex);
        serialNumber = SapManager.GetSerialNumber(serverLocation);
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

            
            bool prmResult = false;
            prmResult = acquisition.SetParameter(SapAcquisition.Prm.CROP_WIDTH, CropWidth, false);
            prmResult = acquisition.SetParameter(SapAcquisition.Prm.CROP_LEFT, CropLeft, false);
            prmResult = acquisition.SetParameter(SapAcquisition.Prm.FLIP, Flip, true);
        }




        if (acqDevice != null && !acqDevice.Initialized)
        {
            if (acqDevice.Create() == false)
            {
                Destroy();
                return;
            }
            CheckAreaCameraFeatures();
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
        if(acquisition != null && acquisition.Initialized)
        {
            CheckLineScanFeatures();
        }
    }
    public void ChangeFOV()
    {
        if (Connected)
        {
            Disconnect();
            Dispose();
            CreateCamera();
        }
        
    }
    public void SnapPicture()
    {
        acqTimeWatch.Restart();

        if (acqDeviceXfer != null && acqDeviceXfer.Connected)
        {
            if (acqDeviceXfer.Snap())
            {
                snapTime = acqTimeWatch.ElapsedMilliseconds;
                acqDeviceXfer.Wait(500);
                snapping = true;
            }
        }
        else if (acqXfer != null && acqXfer.Connected)
        {
            if (acqXfer.Snap())
            {
                acqXfer.Wait(500);

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
        Trigger = false;

        if (IsMaster == 1) { ToggleEncoderPhase(); }
    }
    public void SoftwareTrigger()
    {
        bool triggerResult = acquisition.SoftwareTrigger(SapAcquisition.SoftwareTriggerType.ExternalFrame);

        if (triggerResult) { }
        else { }
    }
    public void SaveImageBMP()
    {
        System.IO.Directory.CreateDirectory(imageFilePath);

        DirectoryInfo ImageDirInfo = new DirectoryInfo(imageFilePath);
        double ImageDirSize = Utilities.DirSize(ImageDirInfo);
        string RemoveFile = Utilities.DirOldest(ImageDirInfo);

        string identifier;

        if (SerialNumber != "") { identifier = PartSerialNumber; }
        else { identifier = "Image"; }

        string ImageFileName = $"{PartType}_{identifier}_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".bmp";
        
        if (ImageDirSize >= 20000) 
        {
            limitReached = true;
            File.Delete(RemoveFile); 
        }
        else
        {
            limitReached = false;
        }
        buffers.Save(imageFilePath + ImageFileName, "-format bmp");
        Utilities.LoggingStatment($"{cameraName}: Save Image to BMP, folder size is {ImageDirSize}");
    }
    public void Disconnect()
    {
        Destroy();
        Dispose();

    }
    public void Destroy()
    {
        if (acqXfer != null && acqXfer.Initialized) {acqXfer.Destroy(); }
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
        else 
        {
            Marshal.FreeHGlobal(cogImageAddress);
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
    public int ToggleEncoderPhase()
    {
        bool getResult;
        bool setResult;
        bool checkResult;

        int returnGetParm = 0;
        int returnCheckParm = 0;

        int phaseA = 1;
        int phaseB = 2;
        if (acquisition == null) { return 0; }

        getResult = acquisition.GetParameter(SapAcquisition.Prm.EXT_LINE_TRIGGER_SOURCE, out returnGetParm);

        if (getResult && (returnGetParm == phaseA || returnGetParm == phaseB))
        {
            switch(returnGetParm)
            {
                case 1:
                    setResult = acquisition.SetParameter(SapAcquisition.Prm.EXT_LINE_TRIGGER_SOURCE, phaseB, true);
                    break;

                case 2:
                    setResult = acquisition.SetParameter(SapAcquisition.Prm.EXT_LINE_TRIGGER_SOURCE, phaseA, true);
                    break;

                default:
                    setResult = acquisition.SetParameter(SapAcquisition.Prm.EXT_LINE_TRIGGER_SOURCE, phaseB, true);
                    break;

            }
        }
        
        checkResult = acquisition.GetParameter(SapAcquisition.Prm.EXT_LINE_TRIGGER_SOURCE, out returnCheckParm);

        return returnCheckParm;
    }
    public void CheckLineScanFeatures()
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
            
        }

        acquisition.SaveParameters(ConfigFile);
    }
    public void CheckAreaCameraFeatures()
    {
        bool getFeatureInfo = false;
        bool getFeatureValueResult = false;
        bool getEnumTextFromValueResult = false;
        bool setFeatureValueResult = false;
        int enumValue;
        double exposureTime;
        int width;
        int height;
        string enumString = "";
        string userSetSelector = "UserSetSelector";
        string userSetDefaultSelector = "UserSetDefaultSelector";
        string userSetLoad = "UserSetLoad";

        if (deviceFeature == null) { deviceFeature = new SapFeature(serverLocation); }
        if (deviceFeature != null && !deviceFeature.Initialized) { deviceFeature.Create(); }

        int numberOfFeatures = acqDevice.FeatureCount;
        string[] features = new string[numberOfFeatures];

        //get full List of Features Names
        for (int i = 0; i < features.Length; i++)
        {
            features[i] = acqDevice.FeatureNames[i];
        }

        //Find and Set UserSet
        getFeatureInfo = acqDevice.GetFeatureInfo(userSetSelector, deviceFeature);
        getFeatureValueResult = acqDevice.GetFeatureValue(userSetSelector, out enumValue);
        getEnumTextFromValueResult = deviceFeature.GetEnumTextFromValue(enumValue, out enumString);



        if (enumString != "UserSet1")
        {

            //Set UserSetSelect,UserSetDefaultSelect to UserSet1
            //Load select User Set
            setFeatureValueResult = acqDevice.SetFeatureValue(userSetDefaultSelector, 4);
            setFeatureValueResult = acqDevice.SetFeatureValue(userSetSelector, 4);
            setFeatureValueResult = acqDevice.SetFeatureValue(userSetLoad, true);
        }

        //Get Image Width
        getFeatureValueResult = acqDevice.GetFeatureValue("Width", out width);
        //Get image height
        getFeatureValueResult = acqDevice.GetFeatureValue("Height", out height);
        //Get Exposure Time
        getFeatureValueResult = acqDevice.GetFeatureValue("ExposureTime", out exposureTime);
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

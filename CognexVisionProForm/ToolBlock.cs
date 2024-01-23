﻿using System;
using System.Diagnostics;
using System.IO;
using Cognex.VisionPro;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;
using System.Runtime.InteropServices;
using DALSA.SaperaLT.SapClassBasic;

namespace CognexVisionProForm
{
    public class ToolBlock
    {
        //Internal Variable
        string AppName;
      
        private string[] newFile;
        string InternalAppName;
        string ArchiveDate;
        public string ArchiveDir;

        string[] fileInfo;
        public string ExecutablePath;
        string ExecutableName;
        string ExecutableExtension;
        string LogFile;

        //For tracing
        public string ArchiveLogDir;
        public string ArchiveLog;
        public string LogDir;

        Cognex.VisionPro.ICogRecord ImageRecord;

        //public variable
        public int CogOutputCount;
        public bool ResultUpdated;
        public string UserResultTag;

        public bool ToolReady = false;
        public double TotalTime;

        public Cognex.VisionPro.ICogRecord ImageDisplayRecord;
        public Cognex.VisionPro.ICogRecord ImageStatusRecord;
        public Cognex.VisionPro.ICogRecord[] JobData;
        public TextWriterTraceListener CameraListener;
        public CogToolBlock cogToolBlock;
        Form1 _form = new Form1();
        public ToolBlock(String UniqueName, Form1 form)
        {
            // Update internal variables
            _form = form;


            //return File Path, File Name and File Type
            fileInfo = GetProgramPath(Application.ExecutablePath.ToString());

            ExecutablePath = fileInfo[0];
            ExecutableName = fileInfo[1];
            ExecutableExtension = fileInfo[2];

            //Create Directory for Log File Archive location for previous log files
            LogDir = ExecutablePath + "\\" + UniqueName + "LogFile\\";
            ArchiveLogDir = LogDir + "Archive\\";
            System.IO.Directory.CreateDirectory(LogDir);
            System.IO.Directory.CreateDirectory(ArchiveLogDir);
            LogFile = LogDir + ToolName + "Log.log";

            //Create Directory for archives for camera program
            ArchiveDir = ExecutablePath + "\\" + UniqueName + "Archive\\";
            System.IO.Directory.CreateDirectory(ArchiveDir);

            //If file already exist, rename and move to archive folder
            if (System.IO.File.Exists(LogFile) == true)
            {
                var f1 = new FileInfo(LogFile);
                var f1_length = f1.Length;

                if (f1_length > 10000)
                {
                    Console.WriteLine(f1_length.ToString());
                    ArchiveDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                    ArchiveLog = ArchiveLogDir + "Log_" + ArchiveDate + ".log";
                    System.IO.File.Move(LogFile, ArchiveLog);
                }
                else { Console.WriteLine("Log File not large enough to archive: " + f1_length); }
            }

            //Create Trace Class for camera
            CameraListener = new TextWriterTraceListener(LogFile);
            Trace.Close();
            Trace.Listeners.Add(CameraListener);
            CameraListener.WriteLine("------------------------------------------------------------------------");
            CameraListener.WriteLine(UniqueName + " created: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

            InternalAppName = UniqueName + "_ToolBlock.vpp";
            
        }

        public string ToolName
        {
            get
            {
                return cogToolBlock.Name;
            }

        }
        public string ToolBlockName
        {
            get { return cogToolBlock.Name; }
        }
        public ICogRunStatus RunStatus
        {
            get { return cogToolBlock.RunStatus; }
        }
        
        public void LoadvisionProject(string InAppName)
        {

            CameraListener.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": ");
            CameraListener.WriteLine("Entered Load Vision Applicaiton");

            AppName = InAppName;
            ArchiveDate = DateTime.Now.ToString("yyyyMMddHHmmss");


            //get file information about selected file
            newFile = GetProgramPath(AppName);

            string newFileExtension = newFile[2];
            string newFileName = newFile[1];
            string newFilePath = newFile[0];


            //confirm selected file is the correct file type
            if (newFileExtension == "vpp")
            {
                if (System.IO.File.Exists(ExecutablePath + "\\" + InternalAppName))
                {
                    //if a program already exists shut it down and move it to an archive folder with date stamp
                    if (cogToolBlock != null) { CloseToolBlock(); }

                    //move existing loaded file archive folder with time stamp
                    System.IO.File.Move(ExecutablePath + "\\" + InternalAppName, ArchiveDir + "Archive_" + ArchiveDate + ".vpp");
                }

                //move and rename new file into correct location
                System.IO.File.Copy(newFilePath + "\\" + newFileName + "." + newFileExtension, ExecutablePath + "\\" + InternalAppName);

                CameraListener.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": ");
                CameraListener.WriteLine("Load Complete: " + InternalAppName);
                Console.WriteLine("Load Complete: " + InternalAppName);
            }
            else
            {
                CameraListener.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": ");
                CameraListener.WriteLine("wrong file type selected");
            }
        }
        public void InitializeJobManager()
        {
            try
            {
                if (System.IO.File.Exists(fileInfo[0] + "\\" + InternalAppName) == true)
                {
                    
                    cogToolBlock = CogSerializer.LoadObjectFromFile(fileInfo[0] + "\\" + InternalAppName) as CogToolBlock;
                    cogToolBlock.Ran += new EventHandler(Subject_Ran);
                    cogToolBlock.Name = AppName;

                    ToolReady = true;

                    CameraListener.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": ");
                    CameraListener.WriteLine("Initialized complete Camera Ready");
                }
                else
                {
                    ToolReady = false;
                    throw new Exception("No Camera Program has Been Loaded"); 
                }

            }
            catch (Exception ex)
            {
                CameraListener.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": ");
                CameraListener.WriteLine(ex.Message);
            }
        }
        private string[] GetProgramPath(string RawString)
        {
            string[] FilePathSplit;
            string[] FileExtensionSplit;
            string[] returnString;

            string filePath;
            string fileExtension;
            string fileName;

            filePath = "";
            fileExtension = "";
            fileName = "";

            // split the file path to individual folders
            FilePathSplit = RawString.Split(Convert.ToChar("\\"));

            // split file to name and extension
            FileExtensionSplit = FilePathSplit[FilePathSplit.Length - 1].Split(Convert.ToChar("."));
            fileName = FileExtensionSplit[0];
            fileExtension = FileExtensionSplit[FileExtensionSplit.Length - 1];

            // build correct directory and return  all information
            filePath = FilePathSplit[0];
            for (int i = 1; i < FilePathSplit.Length - 1; i++)
            {
                filePath = filePath + "\\" + FilePathSplit[i];
            }

            returnString = new string[3] { filePath, fileName, fileExtension };
            return returnString;


        }
        public void ToolRun(CogImage8Grey InputImage)
        {
            ResultUpdated = false;
            ToolReady = false;
            try
            {
                cogToolBlock.Inputs["Image"].Value = InputImage;
                cogToolBlock.Run();
                
                CameraListener.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": ");
                CameraListener.WriteLine(ToolName + ": JOB TRIGGERED");
            }
            catch (Exception ex)
            {
                CameraListener.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": ");
                CameraListener.WriteLine(ex.Message);
            }


        }
        void Subject_Ran(object sender, EventArgs e)
        {

            GetInfoFromTool();
            _form.Camera1ToolBlockUpdate();

            CameraListener.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": ");
            CameraListener.WriteLine(ToolName + ": Toolblock completed Run");

        }
        private void GetInfoFromTool()
        {

            CogOutputCount = cogToolBlock.Outputs.Count;            

            ToolReady = true;
            ResultUpdated = true;

            CameraListener.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": ");
            CameraListener.WriteLine("Numer of Ouptuss - " + (CogOutputCount));
        }
        public void CloseToolBlock()
        {
            //clean up for vision pro
            if(cogToolBlock != null)
            {
                cogToolBlock.Dispose();
                cogToolBlock.Ran -= new EventHandler(Subject_Ran);
            }
            
            CameraListener.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": ");
            CameraListener.WriteLine("Job Manager closed down");
        }
        public ICogImage RawToCogImage(IntPtr ImageAddress, int Width, int Height)
        {
            CogImage8Root NewImageRoot = new CogImage8Root();
            CogImage8Grey NewImage = new CogImage8Grey();

            NewImageRoot.Initialize(Width, Height, ImageAddress, Width, null);

            NewImage.SetRoot(NewImageRoot);

            return NewImage;
        }
        public ICogImage MarshalToCogImage(IntPtr ImageAddress, int Width, int Height, SapFormat format)
        {
            CogImage8Root NewImageRoot = new CogImage8Root();
            CogImage8Grey NewImage8Grey = new CogImage8Grey();
            
            CogImage24PlanarColor NewImage24Color = new CogImage24PlanarColor();


            int managedArraySize = Width * Height;
            byte[] managedArray = new byte[managedArraySize];
            int size = Marshal.SizeOf(managedArray[0]) * managedArray.Length;
            IntPtr tempImageAddress = Marshal.AllocHGlobal(size);

            Marshal.Copy(ImageAddress, managedArray, 0, size);
            Marshal.Copy(managedArray, 0, tempImageAddress, size);

            NewImageRoot.Initialize(Width, Height, tempImageAddress, Width, null);

            if (format == SapFormat.Mono8) 
            {
                NewImage8Grey.SetRoot(NewImageRoot);
                return NewImage8Grey;
            }
            else if(format == SapFormat.RGB888)
            {

                return NewImage24Color;
            }
            else
            {
                return NewImage24Color;
            }
                
            

            
        }
    }
}

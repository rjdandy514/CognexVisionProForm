﻿using System;
using System.Diagnostics;
using System.IO;
using Cognex.VisionPro;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.QuickBuild.Implementation.Internal;
using System.Runtime.Serialization.Formatters.Binary;


namespace CognexVisionProForm
{
    public class ToolBlock
    {
        //Internal Variable
        string toolName;
        string toolFile;
        string toolFileLocation;
        string toolFileType = "ToolBlock";
        string toolFileExtension = ".vpp";

        bool filePresent = false;
        bool resultUpdated = false;
        bool resultUpdated_Mem = false;
        bool toolReady = false;
        public CogToolBlockTerminal[] Input_temp = new CogToolBlockTerminal[1];
        CogToolBlockTerminalCollection toolOutput = new CogToolBlockTerminalCollection();
        CogToolBlockTerminalCollection toolInput = new CogToolBlockTerminalCollection();

        public CogToolBlock cogToolBlock;
        public CogToolBlock cogPreProcessBlock;
        CognexVisionProForm form = new CognexVisionProForm();
       
        public ToolBlock(CognexVisionProForm Form)
        {
            // Update internal variables
            form = Form;
        }

        public int Id
        {
            get; set;
        }
        public int CameraId
        {
            get;set;
        }
        public string Name
        {
            get
            {
                if (cogToolBlock != null) { return cogToolBlock.Name; }
                else if (!String.IsNullOrEmpty(toolName)) { return toolName; }
                else { return "Tool Block not looaded yet"; }
            }
            set
            {
                if (cogToolBlock != null) { cogToolBlock.Name = value; }
                toolName = value.ToString();
                
                toolFileLocation = Utilities.ExeFilePath + "\\Camera" + CameraId.ToString("00") + "\\" + toolName + "_" + toolFileType + toolFileExtension;
                
                if (toolName !="" && File.Exists(toolFileLocation))
                {
                    filePresent = true;
                    toolFile = toolFileLocation;
                }
                else
                {
                    filePresent = false;
                    toolFile = "";
                }
            }
        }
        public bool Result
        {
            get
            {
                if (cogToolBlock != null && cogToolBlock.RunStatus.Result == CogToolResultConstants.Accept) { return true; }
                else { return false; }
            }
        }
        public ICogRunStatus RunStatus
        {
            get
            {
                if (cogToolBlock != null)
                {
                    return cogToolBlock.RunStatus;
                }
                else { return null; }
            }
        }
        public bool ToolReady
        {
            get 
            {

                return toolReady; 
            }
        }
        public bool ResultUpdated
        {
            get { return resultUpdated; }
            set { resultUpdated = value; }
        }
        public bool ResultUpdated_Mem
        {
            get { return resultUpdated_Mem; }
            set { resultUpdated_Mem = value; }
        }
        public bool FilePresent
        {
            get { return filePresent; }
            
        }
        public CogToolBlockTerminalCollection ToolOutput
        {
            get 
            { 
                if (toolOutput != null)
                {
                    return toolOutput;
                }
                else
                {
                    CogToolBlockTerminalCollection Empty = new CogToolBlockTerminalCollection();
                    return Empty; 
                }
            }
        }
        public CogToolBlockTerminalCollection ToolInput
        {
            set
            {
                toolInput = value;
            }
            get { return toolInput; }
        }
        public void LoadvisionProject()
        {
            Utilities.LoggingStatment($"{toolName}: Load Vision Applicaiton");

            //if a program already exists shut it down and move it to an archive folder with date stamp
            if (cogToolBlock != null) { Cleaning(); }

            string filePath = Utilities.ExeFilePath + "\\Camera" + CameraId.ToString("00");

            Utilities.Import(filePath,toolName + "_" + toolFileType, toolFileExtension);

            toolFileLocation = filePath + "\\" +toolName + "_" + toolFileType + toolFileExtension;
            
            if (File.Exists(toolFileLocation))
            {
                filePresent = true;
                toolFile = toolFileLocation;
            }
            else
            {
                filePresent = false;
                toolFile = "";
            }
        }
        public void SaveVisionProject()
        {
            if (toolFile != "") { CogSerializer.SaveObjectToFile(cogToolBlock, toolFile, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), 0); }

            
        }
        public void InitJobManager()
        {
            try
            {
                if (filePresent)
                {

                    
                    cogToolBlock = CogSerializer.LoadObjectFromFile(toolFile) as CogToolBlock;
                    
                    cogToolBlock.Ran += new EventHandler(Subject_Ran);
                    cogToolBlock.Running += new EventHandler(Subject_Running);
                    cogToolBlock.Name = toolName;
                    if (cogToolBlock.Inputs.Count >= 1) { toolInput = cogToolBlock.Inputs; }
                    if (cogToolBlock.Outputs.Count >= 1) { toolOutput = cogToolBlock.Outputs; }

                    toolReady = true;

                    Utilities.LoggingStatment($"{toolName}: Initialized complete Camera Ready");
                }
                else
                {
                    toolReady = false;
                    throw new Exception($"{toolName}: No Camera Program has Been Loaded"); 
                }
            }
            catch (Exception ex) { Utilities.LoggingStatment(ex.Message); }
        }
        public void ToolRun(CogImage8Grey InputImage)
        {
            toolReady = false;

            try
            {
                cogToolBlock.Inputs[0].Value = InputImage;

                if (cogToolBlock.Inputs.Count > 1)
                {
                    for (int i = 1; i < cogToolBlock.Inputs.Count; i++)
                    {
                        if (i >= toolInput.Count) { break; }
                        cogToolBlock.Inputs[i].Value = toolInput[i];
                        Utilities.LoggingStatment($"{toolName}: input # {i} = {cogToolBlock.Inputs[i].Value}");
                    }
                    if(cogToolBlock.Inputs.Count == 5)
                    {
                        cogToolBlock.Inputs[4].Value = Input_temp[0].Value;
                    }
                    
                }
                                   
                cogToolBlock.Run();
                Utilities.LoggingStatment($"{toolName}: Job Triggered");
            }
            catch (Exception ex) 
            {
                toolReady = true;
                Utilities.LoggingStatment(ex.Message); 
            }

        }
        void Subject_Ran(object sender, EventArgs e)
        {
            GetInfoFromTool();
        }
        void Subject_Running(object sender, EventArgs e)
        {

        }
        private void GetInfoFromTool()
        {
            CogToolBlockTerminal failedTool = new CogToolBlockTerminal("ToolFailed",0);

            int toolOutputCount = cogToolBlock.Outputs.Count;

            toolOutput = cogToolBlock.Outputs;
            for(int i = 0; i < toolOutput.Count;i++)
            {
                if(cogToolBlock.RunStatus.Result == CogToolResultConstants.Accept)
                {
                    toolOutput[i] = cogToolBlock.Outputs[i];
                }
                else
                {
                    toolOutput[i] = failedTool;
                }
                    
            }

            toolReady = true;

            if (resultUpdated) { resultUpdated = false; }
            else if (!resultUpdated) { resultUpdated = true; }
            form.ToolBlockRunComplete = CameraId;

            Utilities.LoggingStatment($"{toolName}: Number of Outputs - {toolOutputCount}");
            Utilities.LoggingStatment($"{toolName}: Toolblock completed Run");
        }
        public void Cleaning()
        {

            //clean up for vision pro
            if (cogToolBlock != null)
            {
                SaveVisionProject();
                
                cogToolBlock.Dispose();
                cogToolBlock.Ran -= new EventHandler(Subject_Ran);
                cogToolBlock.Running -= new EventHandler(Subject_Running);
            }
            
            Utilities.LoggingStatment($"{toolName}: Job Manager closed down");
        }

    }
}

using System;
using System.Diagnostics;
using System.IO;
using Cognex.VisionPro;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.QuickBuild.Implementation.Internal;


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
        string Previous = "";

        CogToolBlockTerminal[] toolOutput;
        double[] toolInput;

        public CogToolBlock cogToolBlock;
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
                
                if (File.Exists(toolFileLocation))
                {
                    FilePresent = true;
                    toolFile = toolFileLocation;
                }
                else
                {
                    FilePresent = false;
                    toolFile = "";
                }
            }
        }
        public bool Result
        {
            get
            {
                if(cogToolBlock != null)
                {
                    return Convert.ToBoolean(cogToolBlock.RunStatus.Result);
                }
                else
                {
                    return false;
                }
                
            }

        }
        public double TotalTime
        {
            get
            {
                if(cogToolBlock != null)
                {
                    return cogToolBlock.RunStatus.TotalTime;
                }
                else
                {
                    return 0.0;
                }
                
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
            get;set;
        }
        public bool ResultUpdated
        {
            get;set;
        }
        public bool FilePresent
        {
            get; set;
        }
        public CogToolBlockTerminal[] ToolOutput
        {
            get { return toolOutput; }
        }
        public double[] ToolInput
        {
            set
            {
                toolInput = value;
            }
            get
            {
                return toolInput;
            }
        }
        public void LoadvisionProject()
        {
            Utilities.LoggingStatment($"{toolName}: Load Vision Applicaiton");

            //if a program already exists shut it down and move it to an archive folder with date stamp
            if (cogToolBlock != null) { Cleaning(); }

            string filePath = Utilities.ExeFilePath + "\\Camera" + CameraId.ToString("00");

            Utilities.Import(filePath,toolName, toolFileType, toolFileExtension);

            toolFileLocation = filePath + "\\" +toolName + "_" + toolFileType + toolFileExtension;
            
            if (File.Exists(toolFileLocation))
            {
                FilePresent = true;
                toolFile = toolFileLocation;
            }
            else
            {
                FilePresent = false;
                toolFile = "";
            }
        }
        public void InitializeJobManager()
        {
            try
            {
                if (FilePresent)
                {
                    cogToolBlock = CogSerializer.LoadObjectFromFile(toolFile) as CogToolBlock;
                    cogToolBlock.Ran += new EventHandler(Subject_Ran);
                    cogToolBlock.Name = toolName;

                    ToolReady = true;

                    Utilities.LoggingStatment($"{toolName}: Initialized complete Camera Ready");
                }
                else
                {
                    ToolReady = false;
                    throw new Exception($"{toolName}: No Camera Program has Been Loaded"); 
                }

            }
            catch (Exception ex) { Utilities.LoggingStatment(ex.Message); }
        }
        public void ToolRun(CogImage8Grey InputImage)
        {

            ResultUpdated = false;
            ToolReady = false;

            try
            {
                cogToolBlock.Inputs[0].Value = InputImage;

                for(int i = 1; i < cogToolBlock.Inputs.Count; i++)
                {
                    if (toolInput != null && toolInput.Length > i)
                    {
                        cogToolBlock.Inputs[i].Value = toolInput[i - 1];
                        Utilities.LoggingStatment($"{toolName}: input # {i} = {cogToolBlock.Inputs[i].Value}");
                    }
                }

                cogToolBlock.Run();
                Utilities.LoggingStatment($"{toolName}: Job Triggered");
            }
            catch (Exception ex) 
            {
                ToolReady = true;
                Utilities.LoggingStatment(ex.Message); 
            }

        }
        void Subject_Ran(object sender, EventArgs e)
        {
            GetInfoFromTool();
            Utilities.LoggingStatment($"{toolName}: Toolblock completed Run");
        }
        private void GetInfoFromTool()
        {
            CogToolBlockTerminal failedTool = new CogToolBlockTerminal("ToolFailed",0);

            int toolOutputCount = cogToolBlock.Outputs.Count;

            toolOutput = new CogToolBlockTerminal[toolOutputCount];
            for(int i = 0; i < toolOutput.Length;i++)
            {
                if (cogToolBlock.RunStatus.Result == CogToolResultConstants.Accept)
                {
                    toolOutput[i] = cogToolBlock.Outputs[i];
                }
                else
                {
                    toolOutput[i] = failedTool;
                }
            }

            ToolReady = true;
            ResultUpdated = true;
            form.ToolBlockRunComplete = Id;

            Utilities.LoggingStatment($"{toolName}: Number of Ouptuss - {toolOutputCount}");
        }
        public void Cleaning()
        {
            //clean up for vision pro
            if(cogToolBlock != null)
            {
                cogToolBlock.Dispose();
                cogToolBlock.Ran -= new EventHandler(Subject_Ran);
            }
            
            Utilities.LoggingStatment($"{toolName}: Job Manager closed down");
        }

    }
}

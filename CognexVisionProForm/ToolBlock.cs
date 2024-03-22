using System;
using System.Diagnostics;
using System.IO;
using Cognex.VisionPro;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;


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


        CogToolBlockTerminal[] toolOutput;

        public CogToolBlock cogToolBlock;
        Form1 form = new Form1();
        public ToolBlock(Form1 Form)
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
        public bool Enabled
        {
            get; set;
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
            if (!Enabled || cogToolBlock == null) { return; }

            ResultUpdated = false;
            ToolReady = false;

            try
            {
                cogToolBlock.Inputs["Image"].Value = InputImage;
                cogToolBlock.Run();
                
                Utilities.LoggingStatment($"{toolName}: JOB TRIGGERED");
            }
            catch (Exception ex) { Utilities.LoggingStatment(ex.Message); }

        }
        void Subject_Ran(object sender, EventArgs e)
        {
            GetInfoFromTool();
            Utilities.LoggingStatment($"{toolName}: Toolblock completed Run");
        }
        private void GetInfoFromTool()
        {

            int toolOutputCount = cogToolBlock.Outputs.Count;

            toolOutput = new CogToolBlockTerminal[toolOutputCount];
            for(int i = 0; i < toolOutput.Length;i++)
            {
                toolOutput[i] = cogToolBlock.Outputs[i];
            }

            ToolReady = true;
            ResultUpdated = true;
            form.ToolBlockRunComplete = Id;

            Utilities.LoggingStatment($"{toolName}: Numer of Ouptuss - {toolOutputCount}");
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

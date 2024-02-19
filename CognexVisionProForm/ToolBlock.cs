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

        bool toolFilePresent;
        bool resultUpdated;
        bool toolReady = false;

        public CogToolBlock cogToolBlock;
        Form1 form = new Form1();
        public ToolBlock(Form1 Form)
        {
            // Update internal variables
            form = Form;
        }

        public string ToolName
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
                
                toolFileLocation = form.ExeFilePath + "\\" + toolName + "_" + toolFileType + toolFileExtension;
                
                if (File.Exists(toolFileLocation))
                {
                    toolFilePresent = true;
                    toolFile = toolFileLocation;
                }
                else
                {
                    toolFilePresent = false;
                    toolFile = "";
                }
            }
        }
        public ICogRunStatus RunStatus
        {
            get { return cogToolBlock.RunStatus; }
        }
        public bool ToolReady
        {
            get { return toolReady; }
        }
        public bool ResultUpdated
        {
            get { return resultUpdated; }
        }
        public bool ToolFilePresent
        {
            get { return toolFilePresent; }
        }
        public CogToolBlockTerminal[] ToolOutput
        {
            get { return toolOutput; }
        }
        public void LoadvisionProject()
        {
            form.LoggingStatment($"{toolName}: Load Vision Applicaiton");

            //if a program already exists shut it down and move it to an archive folder with date stamp
            if (cogToolBlock != null) { Cleaning(); }

            form.Import(toolName, toolFileType, toolFileExtension);

            toolFileLocation = form.ExeFilePath + "\\" + toolName + "_" + toolFileType + toolFileExtension;
            
            if (File.Exists(toolFileLocation))
            {
                toolFilePresent = true;
                toolFile = toolFileLocation;
            }
            else
            {
                toolFilePresent = false;
                toolFile = "";
            }
        }
        public void InitializeJobManager()
        {
            try
            {
                if (toolFilePresent)
                {
                    cogToolBlock = CogSerializer.LoadObjectFromFile(toolFile) as CogToolBlock;
                    cogToolBlock.Ran += new EventHandler(Subject_Ran);
                    cogToolBlock.Name = toolName;

                    toolReady = true;

                    form.LoggingStatment($"{toolName}: Initialized complete Camera Ready");
                }
                else
                {
                    toolReady = false;
                    throw new Exception($"{toolName}: No Camera Program has Been Loaded"); 
                }

            }
            catch (Exception ex) { form.LoggingStatment(ex.Message); }
        }
        public void ToolRun(CogImage8Grey InputImage)
        {
            if (cogToolBlock == null) { return; }
            resultUpdated = false;
            toolReady = false;
            try
            {
                cogToolBlock.Inputs["Image"].Value = InputImage;
                cogToolBlock.Run();
                
                form.LoggingStatment($"{toolName}: JOB TRIGGERED");
            }
            catch (Exception ex) { form.LoggingStatment(ex.Message); }

        }
        void Subject_Ran(object sender, EventArgs e)
        {

            GetInfoFromTool();
            form.Camera1ToolBlockUpdate();

            form.LoggingStatment($"{toolName}: Toolblock completed Run");

        }
        private void GetInfoFromTool()
        {

            int toolOutputCount = cogToolBlock.Outputs.Count;

            toolOutput = new CogToolBlockTerminal[toolOutputCount];
            for(int i = 0; i < toolOutput.Length;i++)
            {
                toolOutput[i] = cogToolBlock.Outputs[i];
            }

            toolReady = true;
            resultUpdated = true;

            form.LoggingStatment($"{toolName}: Numer of Ouptuss - {toolOutputCount}");
        }
        public void Cleaning()
        {
            //clean up for vision pro
            if(cogToolBlock != null)
            {
                cogToolBlock.Dispose();
                cogToolBlock.Ran -= new EventHandler(Subject_Ran);
            }
            
            form.LoggingStatment($"{toolName}: Job Manager closed down");
        }

    }
}

using System;
using System.Diagnostics;
using System.IO;
using Cognex.VisionPro;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.QuickBuild.Implementation.Internal;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Cognex.Vision;
using System.Data;
using System.Data.Common;
using Cognex.Vision.Implementation;


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
        string csvFileName;

        List<ToolData> data;
        DataTable dataTable;

        bool filePresent = false;
        bool resultUpdated = false;
        bool resultUpdated_Mem = false;
        bool toolReady = false;
        bool toolRunning = false;
        CogToolBlockTerminalCollection outputs = new CogToolBlockTerminalCollection();
        CogToolBlockTerminalCollection inputs = new CogToolBlockTerminalCollection();

        public CogToolBlock toolBlock;
        CognexVisionProForm form = new CognexVisionProForm();
       
        public ToolBlock(CognexVisionProForm Form)
        {
            // Update internal variables
            form = Form;
        }

        public int CameraId
        {
            get;set;
        }
        public string Name
        {
            get
            {
                if (toolBlock != null) { return toolBlock.Name; }
                else if (!String.IsNullOrEmpty(toolName)) { return toolName; }
                else { return "Tool Block not looaded yet"; }
            }
            set
            {
                if (toolBlock != null) { toolBlock.Name = value; }
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
        public string PartSerialNumber
        {
            get;set;
        }
        public bool Result
        {
            get
            {
                if (toolBlock != null && toolBlock.RunStatus.Result == CogToolResultConstants.Accept) { return true; }
                else { return false; }
            }
        }
        public bool Preprocess
        {
            get;set;
        }
        public ICogRunStatus RunStatus
        {
            get
            {
                if (toolBlock != null)
                {
                    return toolBlock.RunStatus;
                }
                else { return null; }
            }
        }
        public bool ToolRunning
        {
            get { return toolRunning; }
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
        public List<ToolData> AllData
        {
            get { return data; }
        }

        public CogToolBlockTerminalCollection Outputs
        {
            get 
            { 
                if (outputs != null)
                {
                    return outputs;
                }
                else
                {
                    CogToolBlockTerminalCollection emptyOutputs = new CogToolBlockTerminalCollection();
                    return emptyOutputs; 
                }
            }
        }
        public CogToolBlockTerminalCollection Inputs
        {
            set
            {
                inputs = value;
            }
            get { return inputs; }
        }
        public void LoadvisionProject()
        {
            Utilities.LoggingStatment($"{toolName}: Load Vision Applicaiton");

            //if a program already exists shut it down and move it to an archive folder with date stamp
            if (toolBlock != null) { Cleaning(); }

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
            if (toolFile != "") 
            {

                //toolBlock.Inputs[0].Value = null;
                //toolBlock.Run();
                CogSerializer.SaveObjectToFile(toolBlock, toolFile, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), 0); 
            }

            
        }
        public void InitJobManager()
        {
            try
            {
                if (filePresent)
                {
                    toolBlock = CogSerializer.LoadObjectFromFile(toolFile, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), 0) as CogToolBlock;
                    
                    toolBlock.Ran += new EventHandler(Subject_Ran);
                    toolBlock.Running += new EventHandler(Subject_Running);
                    toolBlock.Name = toolName;
                    if (toolBlock.Inputs.Count >= 1) { inputs = toolBlock.Inputs; }
                    if (toolBlock.Outputs.Count >= 1) { outputs = toolBlock.Outputs; }

                    toolBlock.AbortRunOnToolFailure = false;
                    toolBlock.GarbageCollectionEnabled = true;
                    toolBlock.FailOnInvalidDataBinding = true;
                    toolBlock.GarbageCollectionFrequency = 1;

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
        public void ToolSetup()
        {
            
        }
        public void ToolRun()
        {
            toolReady = false;
            
            if (toolBlock.Inputs.Count > 0)
            {
                for (int i = 0; i < toolBlock.Inputs.Count; i++)
                {
                    if (i >= inputs.Count) { break; }
                    toolBlock.Inputs[i].Value = inputs[i].Value;

                    Utilities.LoggingStatment($"{toolName}: input # {i} = {toolBlock.Inputs[i].Value}");
                }
            }
            //Debug.WriteLine(Thread.CurrentThread.Name);
            toolBlock.Run();


            Utilities.LoggingStatment($"{toolName}: Job Triggered");
        }
        void Subject_Ran(object sender, System.EventArgs e)
        {

            GetInfoFromTool();
            
        }
        void Subject_Running(object sender, System.EventArgs e)
        {
            toolRunning = true;
        }
        private void GetInfoFromTool()
        {
            int toolOutputCount = toolBlock.Outputs.Count;
            toolRunning = false;
            
            outputs = toolBlock.Outputs;
            for(int i = 0; i < outputs.Count;i++)
            {
                    outputs[i] = toolBlock.Outputs[i];
            }

            if (resultUpdated) { resultUpdated = false; }
            else { resultUpdated = true; }
            
            if(!Preprocess)
            {
              GetAllToolData();
              CreateTable();
            }
            
            
            toolReady = true;
            Utilities.LoggingStatment($"{toolName}: Number of Outputs - {toolOutputCount}");
            Utilities.LoggingStatment($"{toolName}: Toolblock completed Run");
        }
        public void Cleaning()
        {

            //clean up for vision pro
            if (toolBlock != null)
            {
                SaveVisionProject();
                
                toolBlock.Dispose();
                toolBlock.Ran -= new EventHandler(Subject_Ran);
                toolBlock.Running -= new EventHandler(Subject_Running);
            }
            
            Utilities.LoggingStatment($"{toolName}: Job Manager closed down");
        }
        public void GetAllToolData()
        {
            data = new List<ToolData>();
            string dataTypeName;
            double dataRound;
            double convertData;


            for (int i = 0; i < toolBlock.Tools.Count;i++)
            {
                if (toolBlock.Tools[i].GetType().Name ==  "CogToolBlock")
                {
                    CogToolBlock itool = toolBlock.Tools[i] as CogToolBlock;
                    // Collect all Inputs
                    for (int j = 0; j < itool.Inputs.Count; j++)
                    {
                        dataTypeName = itool.Inputs[j].ValueType.Name;
                        if (!Utilities.IsNumeric(dataTypeName)) { continue; }
                        
                        if (dataTypeName == "Double")
                        {
                            dataRound = Math.Round((double)itool.Inputs[j].Value, 4);
                           data.Add(new ToolData(itool.Name, itool.Inputs[j].Name, dataRound));
                        }
                        else if (dataTypeName == "Int32")
                        {
                            convertData = Convert.ToDouble(itool.Inputs[j].Value);
                            data.Add(new ToolData(itool.Name, itool.Inputs[j].Name, convertData));
                        }
                    }
                    // Collect all Outputs
                    for (int j = 0;j < itool.Outputs.Count;j++)
                    {

                        dataTypeName = itool.Outputs[j].ValueType.Name;
                        if (!Utilities.IsNumeric(dataTypeName)) { continue; }
                        if(itool.RunStatus.Result != CogToolResultConstants.Accept)
                        {
                            dataRound = 0;
                            data.Add(new ToolData(itool.Name, itool.Outputs[j].Name, dataRound));
                        }
                        else if (dataTypeName == "Double")
                        {
                            dataRound = Math.Round((double)itool.Outputs[j].Value,4);
                            data.Add(new ToolData(itool.Name,itool.Outputs[j].Name, dataRound));
                        }
                        else if (dataTypeName == "Int32")
                        {
                            convertData = Convert.ToDouble(itool.Outputs[j].Value);
                            data.Add(new ToolData(itool.Name, itool.Outputs[j].Name, convertData));
                        }
                    }
                }
            }
        }
        public void CreateTable()
        {
            //dataTable = null;
            //if table is null create base table
            if (dataTable == null) 
            { 
                dataTable = new DataTable();
                dataTable.Columns.Add("Part Serial Number", typeof(String));

                for (int i = 0; i < data.Count;i++)
                {
                    dataTable.Columns.Add($"{data[i].ToolName} - {data[i].Name}", typeof(String));
                }
                csvFileName = $"{toolName}_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.csv";

                Utilities.GenerateCsvFromdataTable(Utilities.ExeFilePath + "\\Camera" + CameraId.ToString("00") + "\\PartData\\", csvFileName, dataTable);
            }
            
            PartSerialNumber = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            object[] insert = new object[dataTable.Columns.Count];
            insert[0] = PartSerialNumber;
            
            for (int i = 1; i < insert.Length; i++)
            {
                if (data != null && (i - 1) < data.Count) { insert[i] = data[i - 1].Value.ToString(); }
                else { insert[i] = "n/a"; }
                
            }
            dataTable.Rows.Add(insert);
            Utilities.AppendDatatableToCSV(Utilities.ExeFilePath + "\\Camera" + CameraId.ToString("00") + "\\PartData\\", csvFileName, dataTable.Rows[dataTable.Rows.Count - 1]);

            

        }

    }
}

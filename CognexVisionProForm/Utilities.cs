using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CognexVisionProForm
{

    public static class Utilities
    {
        public static string ExeFileExtension
        {
            get { return Path.GetExtension(System.Windows.Forms.Application.ExecutablePath); }
        }
        public static string ExeFileName
        {
            get { return Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath); }
        }
        public static string ExeFilePath
        {
            get { return Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath); }
        }
        private static TextWriterTraceListener appListener;
        public static void InitLog(string logDir, string archiveDir)
        {
          
            string logFile = logDir + "Log.log";
            double logSize = 0;

            System.IO.Directory.CreateDirectory(logDir);
            System.IO.Directory.CreateDirectory(archiveDir);

            //If file already exist, rename and move to archive folder
            if (System.IO.File.Exists(logFile) == true)
            {
                DirectoryInfo LogeDirInfo = new DirectoryInfo(logDir);
                logSize = DirSize(LogeDirInfo);

                if (logSize > 10)
                {
                    Archive(logFile);
                }
                else { }

            }
            //Create Trace Class for camera
            appListener = new TextWriterTraceListener(logFile);
            Trace.AutoFlush = true;
            Trace.Indent();

            Trace.Close();
            Trace.Listeners.Add(appListener);
            appListener.WriteLine("------------------------------------------------------------------------");
            appListener.WriteLine(" Log Initiated created: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
       
            LoggingStatment($"Log File not large enough to archive: {logSize} MB");

        }
        public static void LoggingStatment(string Message)
        {
            appListener.Write(DateTime.Now.ToString("G") + ": ");
            appListener.WriteLine(Message);
        }
        public static void Closing()
        {
            appListener.Close();
            appListener.Dispose();
        }
        public static void Archive(string FullFilePath)
        {
            string fileExtension = Path.GetExtension(FullFilePath);
            string fileName = Path.GetFileNameWithoutExtension(FullFilePath);

            string filePath = Path.GetDirectoryName(FullFilePath);

            string archiveFolder = filePath + "\\" + fileName;


            System.IO.Directory.CreateDirectory(archiveFolder);

            DirectoryInfo ArchiveDir = new DirectoryInfo(archiveFolder);
            double archiveSize = Utilities.DirSize(ArchiveDir);

            MessageBox.Show($"{fileName} current size = {archiveSize} MB");

            string fullArchivePath = archiveFolder + "\\" + "Archive_" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileExtension;

            File.Move(FullFilePath, fullArchivePath);

        }
        public static bool Import(string NewPath, string NewFileName, string Extension)
        {
            string newfigFile = "";
            string newfigFileExtension = "";
            string newfigFileName = "";
            string newfigFilePath = "";

            // get file from file explorer
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = NewPath;
            openFileDialog1.Title = "Select Configuration File";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                newfigFile = openFileDialog1.FileName;
                newfigFileExtension = Path.GetExtension(newfigFile);
                newfigFileName = Path.GetFileNameWithoutExtension(newfigFile);
                newfigFilePath = Path.GetDirectoryName(newfigFile);
            }
            else
            {
                return false;
            }
            //CONFIRM SELECTED FILE IS CORRECT TYPE, IF NOT END WITH MESSAGE
            if (newfigFileExtension != Extension)
            {
                MessageBox.Show("FILE DOES NOT HAVE CORRECT EXTENSION");
                return false;
            }

            string fullFilePath = NewPath + "\\" + NewFileName + Extension;
            System.IO.Directory.CreateDirectory(NewPath);

            if (File.Exists(fullFilePath) && newfigFile != fullFilePath)
            {
                Archive(fullFilePath);
            }
            if(newfigFile != fullFilePath)
            {
                File.Copy(newfigFile, fullFilePath);
            }
            return true;
        }
        public static double DirSize(DirectoryInfo Folder)
        {
            double size = 0;
            // Add file sizes.
            FileInfo[] fileArray = Folder.GetFiles();
            foreach (FileInfo file in fileArray)
            {
                size += file.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = Folder.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size / 1000000.00;
        }
        public static string DirOldest(DirectoryInfo Folder)
        {
            string OldFile = "";
            DateTime OldFiledate = DateTime.MaxValue;
            // Add file sizes.
            FileInfo[] fileArray = Folder.GetFiles();
            foreach (FileInfo file in fileArray)
            {
                if (DateTime.Compare(OldFiledate, file.CreationTime) > 0)
                {
                    OldFile = file.FullName;
                    OldFiledate = file.CreationTime;
                }

            }

            return OldFile;
        }
        public static string GetLocalIPAddress(string range)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.ToString().StartsWith(range))
                {
                    return ip.ToString();
                }
            }
            return "255.255.255.255";
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static bool IsNumeric(string inputVariable)
        {
            bool isNumeric = false;

            switch(inputVariable)
            {
                case "Int32":
                    isNumeric = true;
                    break;
                case "Double":
                    isNumeric = true;
                    break;

                default:
                    isNumeric = false;
                    break;
            }
            

            return isNumeric;
        }
        public static void LoadForm(object Panel, object Form)
        {
            Panel p = Panel as Panel;
            if (p.Controls.Count > 0)
            {
                p.Controls.RemoveAt(0);
            }
            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            p.Controls.Add(f);
            p.Tag = f;
            f.Show();
        }
        public static void GenerateCsvFromdataTable(string fileLocation,ref string fileName,ref DataTable dt)
        {

            StringBuilder sb = new StringBuilder();

            string[] columnNames = new string[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                columnNames[i] = dt.Columns[i].ColumnName;
            }

            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = Array.ConvertAll(row.ItemArray, x => x.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            string fileNameFull = fileLocation + fileName;

            Directory.CreateDirectory(fileLocation);
            if (Directory.Exists(fileLocation)) { File.WriteAllText(fileNameFull, sb.ToString()); }
        }
        public static void AppendDatatableToCSV(string fileLocation, ref string fileName, DataRow dr)
        {
            StringBuilder sb = new StringBuilder();
            string[] fields = Array.ConvertAll(dr.ItemArray, x => x.ToString());
            sb.AppendLine(string.Join(",", fields));
            

            string fileNameFull = fileLocation + fileName;

            Directory.CreateDirectory(fileLocation);
            if (Directory.Exists(fileLocation)) { File.AppendAllText(fileNameFull, sb.ToString()); }
            
            sb = null;
            fields = null;
        }
    }
    public class ToolData
    {
        string internalToolName;
        public ToolData(string toolName, string name, double value)
        {
            ToolName = toolName;
            Name = name;
            Value = value;
        }
        public string ToolName
        { 
            get { return internalToolName;  }
            set
            {
                switch(value)
                {
                    case "OP":
                        internalToolName = "Outside Perimeter";
                        break;
                    case "COHI":
                        internalToolName = "Coolant Outlet Header";
                        break;
                    case "OOHI":
                        internalToolName = "Oxidant Outlet Header";
                        break;
                    case "FOHI":
                        internalToolName = "Fuel Outlet Header";
                        break;
                    case "CIHI":
                        internalToolName = "Coolant Inlet Header";
                        break;
                    case "OIHI":
                        internalToolName = "Oxidant Inlet Header";
                        break;
                    case "FIHI":
                        internalToolName = "Fuel Inlet Header";
                        break;
                    case "OHLI":
                        internalToolName = "Outlet Hole Left";
                        break;
                    case "OHRI":
                        internalToolName = "Outlet Hole Right";
                        break;
                    case "OHI":
                        internalToolName = "Outlet Hole";
                        break;
                    case "IHLI":
                        internalToolName = "Inlet Hole Left";
                        break;
                    case "IHRI":
                        internalToolName = "Inlet Hole Right";
                        break;
                    case "IHI":
                        internalToolName = "Inlet Hole";
                        break;

                    default:
                        internalToolName = value; 
                        break;
                }
                
            }
    }
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class ToolRecipe
    {
        public ToolRecipe(string name, double value)
        { 
            Name = name;
            Value = value;
        }
        public string Name { get; set; }
        public double Value { get; set; }
    }
}

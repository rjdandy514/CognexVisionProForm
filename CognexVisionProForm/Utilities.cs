﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public static void InitializeLog(string logDir, string archiveDir)
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
        public static void Import(string NewPath, string DeviceID, String FileType, String Extension)
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
                return;
            }
            //CONFIRM SELECTED FILE IS CORRECT TYPE, IF NOT END WITH MESSAGE
            if (newfigFileExtension != Extension)
            {
                MessageBox.Show("FILE DOES NOT HAVE CORRECT EXTENSION");
                return;
            }

            string fullFilePath = NewPath + "\\" + DeviceID + "_" + FileType + Extension;
            System.IO.Directory.CreateDirectory(NewPath);

            if (File.Exists(fullFilePath))
            {
                Archive(fullFilePath);
            }

            File.Copy(newfigFile, fullFilePath);
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
    }
}

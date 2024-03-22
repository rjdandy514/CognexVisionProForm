using LibplctagWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace CognexVisionProForm
{

    class PlcComms
    {
        Form1 _form;
        public PingReply InitialCheck;
        Tag tagPlcToPC;
        Tag tagPcToPlc;
        Tag tagPcToPlcString;
        Libplctag client;
        bool pinged;
        int DataTimeout = 2000;
        public int[] PlcToPC_Data = new int[64];
        public int[]PCtoPLC_Data = new int[64];
        string IP_Address;
        Ping pinger;
        
        public PlcComms(Form1 Sender)
        {
            _form = new Form1();
            pinger = new Ping();

            _form = Sender;
        }
        public string IPAddress
        {
            get { return InitialCheck.Address.ToString(); }
        }
        public string ReadTag
        {
            get;set;
        }
        public string WriteTag
        {
            get;set;
        }
        public string DecodeError(int result)
        {
                return client.DecodeError(result);
        }
        public void InitializePlcComms(string IP1, string IP2, string IP3, string IP4)
        {
            //Get IP Address from HMI screen
            string IP_Address = $"{IP1}.{IP2}.{IP3}.{IP4}";

            // Send Ping command to PLC
            InitialCheck = PingPLC(IP_Address);

            if (InitialCheck == null) { return; }

            if (client != null)
            {
                client.Dispose();
            }

            if (InitialCheck.Status == IPStatus.Success)
            {
                pinged = true;

                //create instance of PLC Client
                client = new Libplctag();

                // create the tag for PLC to PC communication
                tagPlcToPC = new Tag(IP_Address, "1,0", CpuType.LGX, $"{ReadTag}[0]", DataType.DINT, 32);
                CreatePlcTag(tagPlcToPC, $"{ReadTag}[0]");

                // create the tag for PC to PLC communication
                tagPcToPlc = new Tag(IP_Address, "1,0", CpuType.LGX, $"{WriteTag}[0]", DataType.DINT, 32);
                CreatePlcTag(tagPcToPlc, $"{WriteTag}[0]");

                /*
                Sample code for creating String Tag
                // create the tag for PC to PLC communication
                tagPcToPlcString = new Tag(IP_Address, "1,0", CpuType.LGX, "CameraInputString[0]", DataType.String, 10);
                CreatePlcTag(tagPcToPlcString, "CameraInputString[0]");
                */

            }
            else
            {
                pinged = false;
            }

        }
        private void CreatePlcTag(Tag tag, String VariableName)
        {
            int ConnectCount = 0;

            client.AddTag(tag);

            // check that the tag has been added, if it returns pending we have to retry
            while (client.GetStatus(tag) == Libplctag.PLCTAG_STATUS_PENDING)
            {
                Thread.Sleep(100);
                ConnectCount++;

                if (ConnectCount == 20) { break; }
            }
            // if the status is not ok, we have to handle the error
            if (client.GetStatus(tag) != Libplctag.PLCTAG_STATUS_OK)
            {
                MessageBox.Show($"{VariableName} caused the folowing error: {client.DecodeError(client.GetStatus(tag))}");
            }
        }
        public int WritePlcTag()
        {
            int result = 0;
            // set values on the tag buffer
            for (int i = 0; i < tagPcToPlc.ElementCount; i++)
            {
                client.SetInt32Value(tagPcToPlc, i * tagPcToPlc.ElementSize, PCtoPLC_Data[i]);
            }
            // write the values
            result = client.WriteTag(tagPcToPlc, DataTimeout);
            return result;

        }
        public int ReadPlcTag()
        {

            var result = client.ReadTag(tagPlcToPC, DataTimeout);

            if (result == Libplctag.PLCTAG_STATUS_OK)
            {
                for (int i = 0; i < tagPlcToPC.ElementCount; i++)
                {
                    PlcToPC_Data[i] = client.GetInt32Value(tagPlcToPC, i * tagPlcToPC.ElementSize);
                }
            }
            return result;
            
        }
        public PingReply PingPLC(string IP)
        {         
            PingReply replay;
            try
            {
                replay = pinger.Send(IP);
                return replay;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

        }
        public void Cleaning()
        {
            if(client != null)
            {
                client.Dispose();
            }
        }
    }
}

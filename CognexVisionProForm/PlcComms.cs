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
        CognexVisionProForm _form;
        public PingReply InitialCheck;
        Tag tagPlcToPC;
        Tag tagPcToPlc;
        Tag tagPcToPlcString;
        Libplctag client;
        bool pinged;
        int DataTimeout = 2000;
        public short[] PlcToPcData = new short[8];
        public short[] PcToPlcData = new short[8];
        public string[] PcToPlcString = new string[24];
        Ping pinger;
        
        public PlcComms(CognexVisionProForm Sender)
        {
            _form = new CognexVisionProForm();
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
                tagPlcToPC = new Tag(IP_Address, "1,0", CpuType.LGX, $"{ReadTag}[0]", DataType.Int16, 8);
                CreatePlcTag(tagPlcToPC, $"{ReadTag}[0]");

                // create the tag for PC to PLC communication
                tagPcToPlc = new Tag(IP_Address, "1,0", CpuType.LGX, $"{WriteTag}[0]", DataType.Int16, 8);
                CreatePlcTag(tagPcToPlc, $"{WriteTag}[0]");


                // create the tag for PC to PLC communication
                string strPcToPlc = "CameraString[0]";
                tagPcToPlcString = new Tag(IP_Address, "1,0", CpuType.LGX, strPcToPlc, DataType.String, 24);
                CreatePlcTag(tagPcToPlcString, strPcToPlc);


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
                client.SetInt32Value(tagPcToPlc, i * tagPcToPlc.ElementSize, PcToPlcData[i]);
            }
            // write the values
            result = client.WriteTag(tagPcToPlc, DataTimeout);
            return result;

        }
        public int WritePlcDataTag()
        {
            int result = 0;
            // set values on the tag buffer
            for (int i = 0; i < tagPcToPlcString.ElementCount; i++)
            {
                client.SetStringValue(tagPcToPlcString, i * tagPcToPlcString.ElementSize, PcToPlcString[i]);
            }
            // write the values
            result = client.WriteTag(tagPcToPlcString, DataTimeout);
            return result;

        }
        public int ReadPlcTag()
        {

            var result = client.ReadTag(tagPlcToPC, DataTimeout);

            if (result == Libplctag.PLCTAG_STATUS_OK)
            {
                for (int i = 0; i < tagPlcToPC.ElementCount; i++)
                {
                    PlcToPcData[i] = client.GetInt16Value(tagPlcToPC, i * tagPlcToPC.ElementSize);
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

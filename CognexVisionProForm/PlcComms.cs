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
using Cognex.VisionPro.Exceptions;

namespace CognexVisionProForm
{

    class PlcComms
    {
        CognexVisionProForm _form;
        public PingReply InitialCheck;
        Tag tagPlcToPc;
        Tag tagPlcToPcData;
        Tag tagPcToPlc;
        Tag tagPcToPlcData;
        Libplctag client;
        bool pinged;
        int DataTimeout = 2000;
        public int[] PlcToPcControl = new int[8];
        public int[] PcToPlcStatus = new int[8];
        public int[] PcToPlcStatusData = new int[24];
        public int[] PlcToPcControlData = new int[24];
        Ping pinger;
        
        public PlcComms(CognexVisionProForm Sender)
        {
            _form = new CognexVisionProForm();
            pinger = new Ping();

            _form = Sender;
        }
        public string IPAddress
        {
            get;set;
        }
        public string BaseTag
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
        public void InitializePlcComms()
        {
            // Send Ping command to PLC
            InitialCheck = PingPLC();

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

                string plcControl =     $"{BaseTag}.OUT.Control[0]";
                string plcControlData = $"{BaseTag}.OUT.ControlData[0]";
                string plcStatus =      $"{BaseTag}.IN.Status[0]";
                string plcStatusData =  $"{BaseTag}.IN.StatusData[0]";

                // create the tag for PLC to PC communication
                tagPlcToPc =        new Tag(IPAddress, "1,0", CpuType.LGX, plcControl, DataType.DINT, 8);
                tagPlcToPcData =    new Tag(IPAddress, "1,0", CpuType.LGX, plcControlData, DataType.DINT, 24);
                tagPcToPlc =        new Tag(IPAddress, "1,0", CpuType.LGX, plcStatus, DataType.DINT, 8);
                tagPcToPlcData =    new Tag(IPAddress, "1,0", CpuType.LGX, plcStatusData, DataType.DINT, 24);

                //Add tags to Client
                CreatePlcTag(tagPlcToPc, plcControl);
                CreatePlcTag(tagPlcToPcData, plcControlData);
                CreatePlcTag(tagPcToPlc, plcStatus);
                CreatePlcTag(tagPcToPlcData, plcStatusData);

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
                client.SetInt32Value(tagPcToPlc, i * tagPcToPlc.ElementSize, PcToPlcStatus[i]);
            }
            // write the values
            result = client.WriteTag(tagPcToPlc, DataTimeout);
            return result;

        }
        public int WritePlcDataTag()
        {
            int result = 0;
            // set values on the tag buffer
            for (int i = 0; i < tagPcToPlcData.ElementCount; i++)
            {
                client.SetInt32Value(tagPcToPlcData, i * tagPcToPlcData.ElementSize, PcToPlcStatusData[i]);
            }
            // write the values
            result = client.WriteTag(tagPcToPlcData, DataTimeout);
            return result;

        }
        public int ReadPlcTag()
        {

            var result = client.ReadTag(tagPlcToPc, DataTimeout);

            if (result == Libplctag.PLCTAG_STATUS_OK)
            {
                for (int i = 0; i < tagPlcToPc.ElementCount; i++)
                {
                    PlcToPcControl[i] = client.GetInt32Value(tagPlcToPc, i * tagPlcToPc.ElementSize);
                }
            }
            return result;

        }
        public int ReadPlcDataTag()
        {

            var result = client.ReadTag(tagPlcToPcData, DataTimeout);

            if (result == Libplctag.PLCTAG_STATUS_OK)
            {
                for (int i = 0; i < tagPlcToPc.ElementCount; i++)
                {
                    PlcToPcControlData[i] = client.GetInt32Value(tagPlcToPcData, i * tagPlcToPcData.ElementSize);
                }
            }
            return result;

        }
        public PingReply PingPLC()
        {         
            PingReply replay;

            if (IPAddress == null) { return null; }
            try
            {
                replay = pinger.Send(IPAddress);
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

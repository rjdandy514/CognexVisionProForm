using LibplctagWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.AccessControl;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace CognexVisionProForm
{

    class PlcComms
    {
        Form1 _form;
        PingReply InitialCheck;
        Tag tagPlcToPC;
        Tag tagPcToPlc;
        Tag tagPcToPlcString;
        Libplctag client;
        bool pinged;
        int DataTimeout = 2000;
        int[] PlcToPC_Data = new int[32];
        string IP_Address;
        BitArray DintArray;

        public PlcComms(Form1 Sender)
        {
            _form = new Form1();
            _form = Sender;
        }
        public string IPAddress
        {
            get { return IP_Address; }
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
                tagPlcToPC = new Tag(IP_Address, "1,0", CpuType.LGX, "CameraOutput[0]", DataType.DINT, 32);
                CreatePlcTag(tagPlcToPC, "CameraOutput[0]");

                // create the tag for PC to PLC communication
                tagPcToPlc = new Tag(IP_Address, "1,0", CpuType.LGX, "CameraInput[0]", DataType.DINT, 32);
                CreatePlcTag(tagPcToPlc, "CameraInput[0]");

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
        public void WritePlcTag()
        {

            // Use Bit Array to create individual signals and compile them to a single
            // INT back to the PLC
            BitArray myBA = new BitArray(32, false);

            myBA[0] = true;
            myBA[1] = false;
            myBA[2] = true;
            myBA[3] = false;
            myBA[4] = false;
            myBA[5] = true;
            myBA[6] = true;

            int[] ReadTag = new int[1];
            myBA.CopyTo(ReadTag, 0);

            // set values on the tag buffer
            client.SetInt32Value(tagPcToPlc, 0 * tagPcToPlc.ElementSize, ReadTag[0]);
            client.SetInt32Value(tagPcToPlc, 1 * tagPcToPlc.ElementSize, 20); // write 20
            client.SetInt32Value(tagPcToPlc, 2 * tagPcToPlc.ElementSize, 30); // write 30
            client.SetInt32Value(tagPcToPlc, 3 * tagPcToPlc.ElementSize, 40); // write 40

            // write the values
            var result = client.WriteTag(tagPcToPlc, DataTimeout);

            // check the result
            if (result != Libplctag.PLCTAG_STATUS_OK)
            {
                MessageBox.Show($"ERROR: Unable to read the data! Got error code {result}: {client.DecodeError(result)}\n");
                return;
            }


            /*
            sample code for sending string variable
            string InputString = "test";
            client.SetStringValue(tagPcToPlcString, 0 * tagPcToPlcString.ElementSize, InputString);
            client.SetStringValue(tagPcToPlcString, 1 * tagPcToPlcString.ElementSize, InputString);
            client.SetStringValue(tagPcToPlcString, 2 * tagPcToPlcString.ElementSize, InputString);
            client.SetStringValue(tagPcToPlcString, 3 * tagPcToPlcString.ElementSize, InputString);
            var result1 = client.WriteTag(tagPcToPlcString, DataTimeout);
            */

        }
        public void ReadPlcTag()
        {

            var result = client.ReadTag(tagPlcToPC, DataTimeout);

            if (result != Libplctag.PLCTAG_STATUS_OK)
            {
                MessageBox.Show($"ERROR for tag: {tagPlcToPC.Name} - Unable to read the data! Got error code {result}: {client.DecodeError(result)}\n");
                return;
            }

            // Convert the data
            // multiply with tag.ElementSize to keep indexes consistant with the indexes on the plc
            PlcToPC_Data[0] = client.GetInt32Value(tagPlcToPC, 0 * tagPlcToPC.ElementSize);
            PlcToPC_Data[1] = client.GetInt32Value(tagPlcToPC, 1 * tagPlcToPC.ElementSize);
            PlcToPC_Data[2] = client.GetInt32Value(tagPlcToPC, 2 * tagPlcToPC.ElementSize);
            PlcToPC_Data[3] = client.GetInt32Value(tagPlcToPC, 3 * tagPlcToPC.ElementSize);

            //convert Dint to array of Boolean values
            int[] DintToBool = new int[1] { PlcToPC_Data[0] };
            DintArray = new BitArray(DintToBool);

        }
        private PingReply PingPLC(string IP)
        {
            Ping pinger = new Ping();
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
            client.Dispose();
        }
    }
}

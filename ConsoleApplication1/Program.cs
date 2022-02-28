using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            Encapsulation.CommonPacketFormat commonPacketFormat = new Encapsulation.CommonPacketFormat();

            //----------------O->T Network Connection Parameters
            bool redundantOwner = (bool)true;
            byte connectionType = (byte)2; //1=Multicast, 2=P2P
            byte priority = (byte)10;         //00=low; 01=High; 10=Scheduled; 11=Urgent
            bool variableLength = false;       //0=fixed; 1=variable
            UInt16 O_T_Length = 256;
            Console.WriteLine("O_T_Length: " + (O_T_Length).ToString());
            UInt16 connectionSize = (ushort)(O_T_Length);      //The maximum size in bytes og the data for each direction (were applicable) of the connection. For a variable -> maximum
            Console.WriteLine("connectionSize: " + (connectionSize).ToString());
            UInt32 NetworkConnectionParameters;
            NetworkConnectionParameters = (UInt16)(connectionSize & 0x1FF);
            Console.WriteLine("NetworkConnectionParameters: " + (NetworkConnectionParameters).ToString());

            NetworkConnectionParameters = (UInt16)((UInt16)(connectionSize & 0x1FF) | ((Convert.ToUInt16(variableLength)) << 9));
            Console.WriteLine("NetworkConnectionParameters <<9: " + (NetworkConnectionParameters).ToString());
            Console.WriteLine("NetworkConnectionParameters byte: " + ((byte)NetworkConnectionParameters).ToString());
            Console.WriteLine("NetworkConnectionParameters byte >>8: " + ((byte)(NetworkConnectionParameters >> 8)).ToString());
            commonPacketFormat.Data.Add((byte)NetworkConnectionParameters);
            commonPacketFormat.Data.Add((byte)(NetworkConnectionParameters >> 8));

            Console.WriteLine("commonPacketFormat.Data[0]: " + ((byte)commonPacketFormat.Data[0]).ToString());
            Console.WriteLine("commonPacketFormat.Data[1]: " + ((byte)commonPacketFormat.Data[1]).ToString());

            NetworkConnectionParameters = (UInt16)((UInt16)(connectionSize & 0x1FF) | ((Convert.ToUInt16(variableLength)) << 9) | ((priority & 0x03) << 10) | ((connectionType & 0x03) << 13) | ((Convert.ToUInt16(redundantOwner)) << 15));
            commonPacketFormat.Data.Add((byte)NetworkConnectionParameters);
            commonPacketFormat.Data.Add((byte)(NetworkConnectionParameters >> 8));

            byte[] dataToWrite = new byte[commonPacketFormat.toBytes().Length];
            System.Buffer.BlockCopy(commonPacketFormat.toBytes(), 0, dataToWrite, 0, commonPacketFormat.toBytes().Length);

            for (int i = 0; i < dataToWrite.Length; i++)
            {
                Console.WriteLine("dataToWrite["+i+"]: " + ((byte)dataToWrite[i]).ToString());
            }


            EEIPClient eeipClient = new EEIPClient();
            eeipClient.IPAddress = "192.168.0.123";
            eeipClient.RegisterSession();
            byte[] response = eeipClient.GetAttributeSingle(0x66, 1, 0x325);
            Console.WriteLine("Current Value Sensor 1: " + (response[1] * 255 + response[0]).ToString());
            response = eeipClient.GetAttributeSingle(0x66, 2, 0x325);
            Console.WriteLine("Current Value Sensor 2: " + (response[1] * 255 + response[0]).ToString());
            Console.WriteLine();
            Console.Write("Enter intensity for Sensor 1 [1..100]");
            int value = int.Parse(Console.ReadLine());
            Console.WriteLine("Set Light intensity Sensor 1 to "+value+"%");
            eeipClient.SetAttributeSingle(0x66, 1, 0x389,new byte [] {(byte)value,0 });
            Console.Write("Enter intensity for Sensor 2 [1..100]");
            value = int.Parse(Console.ReadLine());
            Console.WriteLine("Set Light intensity Sensor 2 to " + value + "%");
            eeipClient.SetAttributeSingle(0x66, 2, 0x389, new byte[] { (byte)value, 0 });
            Console.WriteLine();
            Console.WriteLine("Read Values from device to approve the value");
            response = eeipClient.GetAttributeSingle(0x66, 1, 0x389);
            Console.WriteLine("Current light Intensity Sensor 1 in %: " + (response[1] * 255 + response[0]).ToString());
            response = eeipClient.GetAttributeSingle(0x66, 2, 0x389);
            Console.WriteLine("Current light Intensity Sensor 2 in %: " + (response[1] * 255 + response[0]).ToString());
            eeipClient.UnRegisterSession();
            Console.ReadKey();
     
       
        }
    }
}

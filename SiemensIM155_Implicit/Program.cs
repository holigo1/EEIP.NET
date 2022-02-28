using System;
using Sres.Net.EEIP;

namespace SiemensIM155_Implicit
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();
            //Ip-Address of the Ethernet-IP Device (In this case Allen-Bradley 1734-AENT Point I/O)
            eeipClient.IPAddress = "192.168.0.1";// "192.168.1.1"; //"192.168.1.254";
            //A Session has to be registered before any communication can be established
            eeipClient.RegisterSession();

            eeipClient.ConfigurationAssemblyInstanceID = 0x307; //1;
            //Parameters from Originator -> Target
            eeipClient.O_T_InstanceID = 0x300;// 112;              //Instance ID of the Output Assembly
            eeipClient.O_T_Length = 496;                     //The Method "Detect_O_T_Length" detect the Length using an UCMM Message
            eeipClient.O_T_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;   //Header Format
            eeipClient.O_T_OwnerRedundant = false;
            eeipClient.O_T_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.O_T_VariableLength = false;
            eeipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eeipClient.RequestedPacketRate_O_T = 100000;        //500ms is the Standard value

            //Parameters from Target -> Originator
            eeipClient.T_O_InstanceID = 0x301;//100; //0x67;
            eeipClient.T_O_Length = 500;
            eeipClient.T_O_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Modeless;
            eeipClient.T_O_OwnerRedundant = false;
            eeipClient.T_O_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.T_O_VariableLength = false;
            eeipClient.T_O_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eeipClient.RequestedPacketRate_T_O = 100000;    //RPI in  500ms is the Standard value

            eeipClient.OriginatorUDPPort = 2223;
            //Forward open initiates the Implicit Messaging
            eeipClient.ForwardOpen();

            while (true)
            {

                //Read the Inputs Transfered form Target -> Originator
                Console.WriteLine("CPU IDS: " + eeipClient.T_O_IOData[0]);
                Console.WriteLine("I0 in[0..7]: " + eeipClient.T_O_IOData[1]);
                Console.WriteLine("I0 in[8..F]: " + eeipClient.T_O_IOData[2]);

                //write the Outputs Transfered form Originator -> Target
                eeipClient.O_T_IOData[0] ^= eeipClient.O_T_IOData[0];        //Flip bit In 0..7

                System.Threading.Thread.Sleep(500);

            }

            //Close the Session
            eeipClient.ForwardClose();
            eeipClient.UnRegisterSession();

        }
    }
}

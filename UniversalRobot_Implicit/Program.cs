using System;
using Sres.Net.EEIP;

namespace UniversalRobot_Implicit
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();
            //Ip-Address of the Ethernet-IP Device
            eeipClient.IPAddress = "10.110.9.226";// URSim
            //A Session has to be registered before any communication can be established
            eeipClient.RegisterSession();

            eeipClient.ConfigurationAssemblyInstanceID = 0x1;
            //Parameters from Originator -> Target
            eeipClient.O_T_InstanceID = 112;              //Instance ID of the Output Assembly
            eeipClient.O_T_Length = 224;                     //The Method "Detect_O_T_Length" detect the Length using an UCMM Message
            eeipClient.O_T_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;   //Header Format
            eeipClient.O_T_OwnerRedundant = false;
            eeipClient.O_T_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.O_T_VariableLength = false;
            eeipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eeipClient.RequestedPacketRate_O_T = 500000;        //500ms is the Standard value

            //Parameters from Target -> Originator
            eeipClient.T_O_InstanceID = 100; 
            eeipClient.T_O_Length = 480;
            eeipClient.T_O_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Modeless;
            eeipClient.T_O_OwnerRedundant = false;
            eeipClient.T_O_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.T_O_VariableLength = false;
            eeipClient.T_O_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eeipClient.RequestedPacketRate_T_O = 500000;    //RPI in  500ms is the Standard value

            eeipClient.OriginatorUDPPort = 2223;	// UR 5.11.8 will ignore this and use instead default port 2222
            //Forward open initiates the Implicit Messaging
            eeipClient.ForwardOpen();

            // wait to receiv first data
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("UR Verion: " + eeipClient.T_O_IOData[0] + "." + eeipClient.T_O_IOData[1]);
			Console.WriteLine("Originator UDP Port used: " + eeipClient.OriginatorUDPPort);

            while (true)
            {

                //Read the Inputs Transfered form Target -> Originator
                Console.WriteLine("Robot Time: " + eeipClient.T_O_IOData[9] + "H " + eeipClient.T_O_IOData[8] + "min " + eeipClient.T_O_IOData[5] + "s ");
                Console.WriteLine("Bit Out[0..7]: " + eeipClient.T_O_IOData[280]);

                //write the Outputs Transfered form Originator -> Target
                eeipClient.O_T_IOData[24] ^= eeipClient.O_T_IOData[24];        //Flip bit In 0..7

                System.Threading.Thread.Sleep(500);
                
            }

            //Close the Session
            eeipClient.ForwardClose();
            eeipClient.UnRegisterSession();

        }
    }
}

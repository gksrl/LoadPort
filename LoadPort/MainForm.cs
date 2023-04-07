using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace LoadPort
{

    public partial class Main : Form
    {
        StreamReader Reading;
        StreamWriter Writing;
        StreamWriter STAT_Writing;
        StreamReader STAT_Reading;
        bool tcpconnect = false;

        bool INIT = false;
        bool ORGN = false;
        bool RSTA = false;
        bool CLMP = false;
        bool UCLM = false;
        public Main()
        {
            InitializeComponent();
            //Thread thread1 = new Thread(TCPConnect);
            //thread1.IsBackground = true;
            //thread1.Start();
        }
        public void Send(string Command)
        {
            if (Command == "INIT")
            {
                Writing.WriteLine("oSTG1.INIT" + "\r");
            }
            else if (Command == "ORGN")
            {
                Writing.WriteLine("oSTG1.ORGN" + "\r");
            }
            else if (Command == "RSTA")
            {
                Writing.WriteLine("oSTG1.RSTA" + "\r");
            }
            else if (Command == "CLMP")
            {
                Writing.WriteLine("oSTG1.CLMP" + "\r");
            }
            else if (Command == "UCLM")
            {
                Writing.WriteLine("oSTG1.UCLM" + "\r");
            }
            else if (Command == "EVNT")
            {
                Writing.WriteLine("oSTG1.EVNT(0,1)" + "\r");
            }

        }
        private void btn_Command_Click(object sender, EventArgs e)
        {
            if (tcpconnect)
            {
                Button command_button = (Button)sender;
                string command = command_button.Text;
                switch (command)
                {
                    case "INIT":
                        Writing.WriteLine("oSTG1.INIT" + '\r');
                        INIT = true;
                        break;
                    case "ORIGIN":
                        Writing.WriteLine("oSTG1.ORGN" + "\r");
                        ORGN = true;
                        break;
                    case "RSTA":
                        Writing.WriteLine("oSTG1.RSTA" + "\r");
                        RSTA = true;
                        break;
                    case "CLAMP":
                        Writing.WriteLine("oSTG1.CLMP" + "\r");
                        break;
                    case "UNCLAMP":
                        Writing.WriteLine("oSTG1.UCLM" + "\r");
                        break;
                }
            }
        }

        public void TCPConnect()
        {
            //TcpClient Client = new TcpClient();
            //IPEndPoint Point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse("11000") /*+  (iPortNo * 1000)*/);  // Client

            TcpListener tcpListener1 = new TcpListener(IPAddress.Parse("127.0.0.1"), int.Parse("11000"));   // Server
            tcpListener1.Start();
            TcpClient tcpClient1 = tcpListener1.AcceptTcpClient();
            tcpconnect = true;
            Writing = new StreamWriter(tcpClient1.GetStream());
            Reading = new StreamReader(tcpClient1.GetStream());
            Writing.WriteLine("oSTG1.EVNT(0,1)" + "\r");
            Writing.AutoFlush = true;

            while (tcpconnect)
            {
                if (Reading.ReadLine() != null)
                {
                    string ReceiveData = Reading.ReadLine();
                    this.Invoke(new Action(delegate ()
                    {
                        listBox1.Items.Add(ReceiveData);
                    }));
                }

                //string[] lines = ReceiveData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                // foreach (string strmsg in lines)
                //{
                //    this.Invoke(new Action(delegate ()
                //    {
                //        ProcessData(strmsg);
                //    }));
                //}

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread1 = new Thread(TCPConnect);
            thread1.IsBackground = true;
            thread1.Start();
        }


        //private void ProcessData(string sData) // Receive Data Split 
        //{
        //    string HED;
        //    string ID;
        //    string DATA;
        //    string sSTAT = "";
        //    string sGMAP = "";
        //    char[] cGMAP = null;
        //    string sInput = "";
        //    string sOutput = "";

        //    HED = sData.Substring(0, 1);
        //    ID = sData.Substring(1, 4);
        //    DATA = sData.Substring(sData.IndexOf('.') + 1, 4);

        //    switch (HED)
        //    {
        //        case "n":
        //            break;
        //        case "c":
        //            break;
        //        default:
        //            if (DATA == "CNCT")
        //            {
        //                tcpconnect = true;
        //                Send("EVNT");
        //            }
        //            else if (DATA == "GMAP")
        //            {
        //                sGMAP = sData.Substring(sData.IndexOf(':') + 1);
        //                cGMAP = sGMAP.ToCharArray();

        //                for (int i = 0; i < 25; i++)
        //                {
        //                    SYS.LP_GMAP[PortNum, i + 1] = cGMAP[i].ToString();
        //                }
        //            }
        //            else if (DATA == "STAT")
        //            {
        //                sSTAT = sData.Substring(sData.IndexOf(':') + 1, 5);
        //                ErrorCode = sData.Substring(sData.IndexOf('/') + 1, 4);

        //                for (int i = 1; i < 6; i++)
        //                {
        //                    SYS.Lp_STAT[PortNum, i] = sSTAT.Substring(i - 1, 1);
        //                }

        //                if (SYS.Lp_STAT[PortNum, 4] == "0" && SYS.LP_Door[PortNum] == "02/02")
        //                {
        //                    SEND("GMAP");
        //                }
        //            }
        //            else if (DATA == "GPIO")
        //            {
        //                sInput = sData.Substring(sData.IndexOf(':') + 1, sData.IndexOf('/') - sData.IndexOf(':') - 1);
        //                sOutput = sData.Substring(sData.IndexOf('/') + 1);

        //                for (int j = 0; j <= 15; j++)
        //                {
        //                    byte hex = Convert.ToByte(sInput.Substring(15 - j, 1), 16);
        //                    byte hex1 = Convert.ToByte(sOutput.Substring(15 - j, 1), 16);

        //                    for (int i = 0; i <= 3; i++)
        //                    {
        //                        LP_GPIN[i + (j * 4)] = Convert.ToBoolean(((Convert.ToInt16(hex)) & Convert.ToInt16(Math.Pow(2, i))) / (Convert.ToInt16(Math.Pow(2, i))));
        //                        LP_GPOUT[i + (j * 4)] = Convert.ToBoolean(((Convert.ToInt16(hex1)) & Convert.ToInt16(Math.Pow(2, i))) / (Convert.ToInt16(Math.Pow(2, i))));
        //                    }
        //                }
        //            }
        //            break;
        //    }
        //}
    }
}

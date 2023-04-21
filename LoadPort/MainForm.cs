using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LoadPort
{

    public partial class Main : Form
    {
        StreamReader Reading;
        StreamWriter Writing;
        TcpListener tcpListener1;
        TcpClient tcpClient1;
        NetworkStream stream;
        bool tcpconnect = false;
        byte[] buffer = new byte[1024];
        int[] Gpi_Array = new int[64];
        int[] Gpo_Array = new int[64];
        string ErrorCode = "";
        string sSTAT = "00000";
        string Gpi_bin = "";
        string Gpo_bin = "";
        string Ypos; // Y축 
        string Zpos; // Z축 
        string Wdata = "0000000000000000000000000";
        public Label[] lb_Wafer;


        const int OPERATION_MODE = 0;
        const int ORIGIN_COMPLETE = 1;
        const int COMMAND_PROCESSING = 2;
        const int OPERATION_MOVING = 3;
        const int MOTION_SPEED = 4;
        #region LoadPort GPI
        public const int GIN_PREPARATION_COMPLETED = 0;
        public const int GIN_TEMPORARILY_STOP = 1;
        public const int GIN_SIGNIFICANT_ERROR = 2;
        public const int GIN_LIGHT_ERROR = 3;
        public const int EXHAUST_FAN1 = 4;
        public const int EXHAUST_FAN2 = 5;
        public const int PROTRUSION = 6;
        public const int GIN_SIGNAL_NOT_CONNECTED = 7;
        public const int FOUP_DOOR_LEFT_CLOSE = 8;
        public const int FOUP_DOOR_LEFT_OPEN = 9;
        public const int FOUP_DOOR_RIGHT_CLOSE = 10;
        public const int FOUP_DOOR_RIGHT_OPEN = 11;
        public const int GIN_MAPPING_SENSOR_CONTAINING = 12;
        public const int GIN_MAPPING_SENSOR_PREPARATION = 13;
        public const int UPPER_PRESSURE_LIMIT = 14;
        public const int LOWER_PRESSURE_LIMIT = 15;
        public const int GIN_CARRIER_CLAMP_OPEN = 16;
        public const int GIN_CARRIER_CLAMP_CLOSE = 17;
        public const int PRESENCE_LEFT = 18;
        public const int PRESENCE_RIGHT = 19;
        public const int PRESENCE_MIDDLE = 20;
        public const int INFO_PAD_A = 21;
        public const int INFO_PAD_B = 22;
        public const int INFO_PAD_C = 23;
        public const int INFO_PAD_D = 24;
        public const int PRESENCE = 25;
        public const int FOSB_IDENTIFICATION_SENSOR = 26;
        public const int GIN_OBSTACLE_DETECTING_SENSOR = 27;
        public const int DOOR_DETECTION = 28;
        public const int OPEN_CARRIER_DETECTING_SENSOR = 30;
        public const int GIN_STAGE_ROTATION_BACK = 32;
        public const int GIN_STAGE_ROTATION_FRONT = 33;
        public const int GIN_BCR_LIFTING = 34;
        public const int GIN_BCR_LOWERING = 35;
        public const int GIN_COVER_LOCK = 36;
        public const int GIN_COVER_UNLOCK = 37;
        public const int GIN_CARRIER_RETAINER_LOWERING = 38;
        public const int GIN_CARRIER_RETAINER_LIFTING = 39;
        public const int EXTERNAL_SW1 = 40;
        public const int EXTERNAL_SW2 = 41;
        public const int EXTERNAL_SW3 = 42;
        public const int PFA_L = 46;
        public const int PFA_R = 47;
        public const int THREE_DSC = 48;
        public const int TWO_DSC = 49;
        public const int ONEPFIVE_DSC = 50;
        public const int COMMON = 51;
        public const int TWOHUNDRED = 52;
        public const int ONEHUNDRED_FIFTY = 53;
        public const int ADAPTER = 54;
        public const int COVER = 55;
        public const int VALID = 56;
        public const int CS_0 = 57;
        public const int CS_1 = 58;
        public const int TR_REQ = 60;
        public const int BUSY = 61;
        public const int COMPT = 62;
        public const int CONT = 63;
        #endregion
        #region LoadPort GPO
        public const int PREPARATION_COMPLETED = 0;
        public const int TEMPORARILY_STOP = 1;
        public const int SIGNIFICANT_ERROR = 2;
        public const int LIGHT_ERROR = 3;
        public const int SIGNAL_NOT_CONNECTED = 4;
        public const int ADAPTER_CLAMP = 5;
        public const int ADAPTER_POWER = 6;
        public const int OBSTACLE_DETECTION_CANCEL = 7;
        public const int CARRIER_CLAMP_CLOSE = 10;
        public const int CARRIER_CLAMP_OPEN = 11;
        public const int FOUP_DOOR_LOCK_OPEN = 12;
        public const int FOUP_DOOR_LOCK_CLOSE = 13;
        public const int MAPPING_SENSOR_PREPARATION = 16;
        public const int MAPPING_SENSOR_CONTAINING = 17;
        public const int CHUCKING_ON = 18;
        public const int CHUCKING_OFF = 19;
        public const int COVER_LOCK = 20;
        public const int COVER_UNLOCK = 21;
        public const int DOOR_OPEN = 24;
        public const int CARRIER_CLAMP = 25;
        public const int CARRIER_DETECTING_SENSOR_ON = 26;
        public const int PREPARATION_COMPLETED_EXTERNAL_OUTPUT = 27;
        public const int CARRYING_PROPERLY_LOADED = 28;
        public const int STAGE_ROTATION_BACK = 32;
        public const int STAGE_ROTATION_FRONT = 33;
        public const int BCR_LIFTING = 34;
        public const int BCR_LOWERING = 35;
        public const int CARRIER_RETAINER_LOWERING = 38;
        public const int CARRIER_RETAINER_LIFTING = 39;
        public const int EXTERNAL_SW1LED_LOAD = 40;
        public const int EXTERNAL_SW3LED_UNLOAD = 41;
        public const int LOAD_LED = 42;
        public const int UNLOAD_LED = 43;
        public const int PRESENCE_LED = 44;
        public const int PLACEMENT_LED = 45;
        public const int MANUAL_LED = 46;
        public const int ERROR_LED = 47;
        public const int CLAMP_LED = 48;
        public const int DOCK_LED = 49;
        public const int BUSY_LED = 50;
        public const int AUTO_LED = 51;
        public const int RESERVED_LED = 52;
        public const int CLOSED_LED = 53;
        public const int LOCK_LED = 54;
        public const int LOAD_REQUSET = 56;
        public const int UNLOAD_REQUSET = 57;
        public const int READY = 59;
        public const int HO_AVBL = 62;
        public const int ES = 63;
        #endregion
        public Main()
        {
            InitializeComponent();
        }
        public void Send(string command)
        {
            Writing.Write("oSTG1." + command + '\r');
            Writing.Flush();
            this.Invoke(new Action(delegate ()
            {
                listBox1.Items.Add("oSTG1." + command + '\r');
            }));
        }
        private void btn_Command_Click(object sender, EventArgs e)
       {
            if (tcpconnect)
            {
                if (sSTAT[OPERATION_MOVING].ToString() == "1")
                {
                    MessageBox.Show("LoadPort가 동작 중 입니다.");
                    return;
                }
                Button command_button = (Button)sender;
                string command = command_button.Text;
                switch (command)
                {
                    case "INIT":
                        if (sSTAT[OPERATION_MODE].ToString() == "0")
                        {
                            MessageBox.Show(" INITIAL 중 입니다.");
                            return;
                        }
                        Send("INIT");
                        break;
                    case "ORIGIN":
                        Send("ORGN");
                        break;
                    case "RSTA":
                        if (sSTAT[OPERATION_MOVING].ToString() == "1")
                        {
                            MessageBox.Show("RESET 중 입니다.");
                            return;
                        }
                        Send("RSTA(0)");
                        break;
                    case "CLAMP":
                        if (Ypos == "02" || Ypos == "03")
                        {
                            MessageBox.Show("LOADPORT가 이미 Dokcking 중 입니다.");
                            return;
                        }
                        Send("CLMP");
                        break;
                    case "UNCLAMP":
                        if (Ypos == "01" || Ypos == "04")
                        {
                            MessageBox.Show("LOADPORT가 이미 UnDokcking 중 입니다.");
                            return;
                        }
                        Send("UCLM");
                        break;
                }
            }
        }

        public void TCPConnect()
        {
            //TcpClient Client = new TcpClient();
            //IPEndPoint Point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse("11000") /*+  (iPortNo * 1000)*/);  // Client

            tcpListener1 = new TcpListener(IPAddress.Parse("127.0.0.1"), int.Parse("11000"));   // Server
            tcpListener1.Start();
            tcpClient1 = tcpListener1.AcceptTcpClient();
            stream = tcpClient1.GetStream();
            Writing = new StreamWriter(tcpClient1.GetStream());
            Reading = new StreamReader(tcpClient1.GetStream());
            Writing.AutoFlush = true;
            int n;

            while (true)
            {
                try
                {
                    if (tcpClient1.Connected)
                    {
                        tcpconnect = true;
                        while ((n = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            //stream.Write(buffer, 0, n);
                            string data = null;
                            data = Encoding.UTF8.GetString(buffer, 0, n);
                            string ReceiveData = data;

                            if (ReceiveData.Contains("CNCT"))
                            {
                                this.Invoke(new Action(delegate ()
                                {

                                    listBox1.Items.Add(ReceiveData);
                                }));
                                Send("EVNT(0,1)");
                            }

                            else
                            {
                                string[] lines = ReceiveData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string strmsg in lines)
                                {
                                    this.Invoke(new Action(delegate ()
                                    {

                                        listBox1.Items.Add(strmsg);
                                        ProcessData(strmsg);
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                        listBox1.SelectedIndex = -1;
                                        if (listBox1.Items.Count == 100)
                                        {
                                            listBox1.Items.RemoveAt(listBox1.SelectedIndex + 1);
                                        }
                                    }));
                        
                                }
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    MessageBox.Show("LoadPort와 연결이 끊어졌습니다.");
                    this.Invoke(new Action(delegate ()
                    {
                        listBox1.Items.Add("연결 대기중...");
                    }));
                    tcpconnect = false;
                }
                finally
                {
                    tcpClient1.Close();
                    tcpListener1.Stop();
                    TCPConnect();

                }
            }
        }

        private void ProcessData(string sData) // Receive Data Split 
        {
            string HED;
            string DATA;
            string Gpi = "";
            string Gpo = "";

            HED = sData.Substring(0, 1);
            DATA = sData.Substring(sData.IndexOf('.') + 1, 4);

            switch (HED)
            {
                case "n":
                    break;
                case "c":
                    break;
                default:

                    if (DATA == "STAT")
                    {
                        sSTAT = sData.Substring(sData.IndexOf(':') + 1, 5);
                        ErrorCode = sData.Substring(sData.IndexOf('/') + 1, 4);
                    }

                    if (DATA == "GPIO")
                    {
                        Gpi = sData.Substring(sData.IndexOf(':') + 1, 16);  // GP INPUT
                        Gpi_bin = Convert.ToString(Convert.ToInt64(Gpi, 16), 2); // 16진수를 2진수로 변환
                        if (Gpi_bin.Length < 63)                                // 부족한 BIT 수를 추가 [64]
                        {
                            for (int i = 0; Gpi_bin.Length < 64; i++)
                            {
                                Gpi_bin = Gpi_bin.Insert(0, "0");
                            }
                        }
                        for (int i = 0; i < 64; i++)                        // Gpi 배열에 Gpi binary 값을 역순으로 대입
                        {
                            Gpi_Array[63 - i] = int.Parse(Gpi_bin[i].ToString());
                        }

                        Gpo = sData.Substring(sData.IndexOf('/') + 1, 16); // GP OUTPUT
                        Gpo_bin = Convert.ToString(Convert.ToInt64(Gpo, 16), 2);  // 16진수를 2진수 변환
                        if (Gpo_bin.Length < 63)
                        {
                            for (int i = 0; Gpo_bin.Length < 64; i++)
                            {
                                Gpo_bin = Gpo_bin.Insert(0, "0");
                            }
                        }
                        for (int i = 0; i < 64; i++)
                        {
                            Gpo_Array[63 - i] = int.Parse(Gpo_bin[i].ToString());
                        }
                    }

                    if (DATA == "GPOS")
                    {
                        Ypos = sData.Substring(sData.IndexOf(':') + 1, 2); // Y축 
                        Zpos = sData.Substring(sData.IndexOf('/') + 1, 2); // Z축 

                        if (Ypos == "02" && Zpos == "02")
                        {
                            Writing.WriteLine("oSTG1.GMAP" + "\r"); // DOOR OPEN 시, GMAP 수행 [MAPPING READ]
                            listBox1.Items.Add("oSTG1.GMAP" + "\r");
                        }

                    }

                    if (DATA == "GMAP")
                    {
                        Wdata = sData.Substring(sData.IndexOf(':') + 1, 25);
                    }
                    break;
            }
        }

        private void DisplayFunction()
        {
            if (tcpconnect)
            {
                label_connect.BackColor = Color.SpringGreen;
            }
            else
            {
                label_connect.BackColor = SystemColors.Control;
            }

            if (ErrorCode == "0000")
            {
                label_errorcode.BackColor = Color.White;
                label_errorcode.Text = "Error CODE" + '\r' + ErrorCode;
            }
            else      // Error Code
            {
                label_errorcode.BackColor = Color.LightGreen;
                label_errorcode.Text = "Error CODE" + '\r' + ErrorCode;
            }


            if (sSTAT[0].ToString() == "1")
            {
                label_init.Text = "Initial";
                label_init.BackColor = Color.LightGreen;

                //작동모드 원격
            }
            else
            {
                label_init.BackColor = Color.White;
            }
            if (sSTAT[1].ToString() == "1")
            {
                label_origin.BackColor = Color.LightGreen;
                //원점복귀 미완료
            }
            else
            {
                label_origin.BackColor = Color.White;
                //원점복귀 미완료
            }

            if (sSTAT[2].ToString() == "1")
            {
                //명령처리 처리
            }
            else
            {
                //명령처리 정지
            }


            if (sSTAT[3].ToString() == "0")
            {
                label_moving.BackColor = Color.White;
                //LD포트동작 상태 정지
            }
            else if (sSTAT[3].ToString() == "1")
            {
                label_moving.Text = "MOVING";
                label_moving.BackColor = Color.LightGreen;
                //LD포트동작 상태 이동
            }
            else if (sSTAT[3].ToString() == "2")
            {
                label_moving.BackColor = Color.White;
                //LD포트동작 상태 일시정지
            }

            if (sSTAT[4].ToString() == "0")
            {
                //LD포트동작속도 보통
            }
            else
            {
                // 유지보수
            }

            if (Gpi_Array[GIN_CARRIER_CLAMP_CLOSE] == 1)  // CLAMP 조건
            {
                label_clamp.BackColor = Color.LightGreen;
            }
            else
            {
                label_clamp.BackColor = Color.White;
            }
            if (Gpi_Array[CHUCKING_ON] + Gpi_Array[PRESENCE_RIGHT] + Gpi_Array[PRESENCE_MIDDLE] + Gpi_Array[PRESENCE] == 4) // FOUP 조건
            {
                label_foup.BackColor = Color.LightGreen;
            }
            else
            {
                label_foup.BackColor = Color.White;
            }

            if (Ypos == "02" || Ypos == "03")   // Docking 조건 (GPOS의 Y축 Pos값이 2 또는 3)
            {
                label_docking.BackColor = Color.LightGreen;
            }

            else
            {
                label_docking.BackColor = Color.White;
            }

            // Door(문열림) 조건 => Docking 완료시 ON, 원점검색 또는 Undocking시 OFF

            if (Ypos == "02" && Zpos == "02")
            {
                label_door.BackColor = Color.LightGreen;
                for (int i = 1; i <= 25; i++)
                {
                    if (Wdata[i - 1].ToString() == "1")
                    {

                        lb_Wafer[i - 1].BackColor = Color.SpringGreen;
                    }

                    else
                    {
                        lb_Wafer[i - 1].BackColor = SystemColors.Control;
                    }
                }
            }

            else
            {
                label_door.BackColor = Color.White;
                for (int i = 1; i <= 25; i++)
                {
                        lb_Wafer[i - 1].BackColor = SystemColors.Control;
                }
            }

        }

        private void Main_Load(object sender, EventArgs e)
        {
            Thread thread1 = new Thread(TCPConnect);
            thread1.IsBackground = true;
            thread1.Start();

            lb_Wafer = new Label[] {lb_wafer_1, lb_wafer_2, lb_wafer_3, lb_wafer_4, lb_wafer_5, lb_wafer_6,
                                    lb_wafer_7, lb_wafer_8, lb_wafer_9, lb_wafer_10, lb_wafer_11, lb_wafer_12, lb_wafer_13,
                                    lb_wafer_14, lb_wafer_15, lb_wafer_16, lb_wafer_17, lb_wafer_18, lb_wafer_19, lb_wafer_20,
                                    lb_wafer_21, lb_wafer_22, lb_wafer_23, lb_wafer_24, lb_wafer_25};
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DisplayFunction();
        }
    }
}

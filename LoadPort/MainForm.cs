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
        string WriteCommand = "";
        bool tcpconnect = false;
        byte[] buffer = new byte[1024];

        int[] Gpi_Array = new int[64];
        int[] Gpo_Array = new int[64];
        string Gpi_bin = "";
        string Gpo_bin = "";
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
        Boolean GIN_carrier_clamp_close;
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

        Boolean preparation_completed;
        Boolean temporarily_stop;
        Boolean significant_error;
        Boolean light_error;
        Boolean siganl_not_connected;
        Boolean adapter_clamp;
        Boolean adapter_power;
        Boolean obstacle_detection_cancel;
        Boolean carrier_clamp_close;
        Boolean carrier_clamp_open;
        Boolean foup_door_lock_open;
        Boolean foup_door_lock_close;
        Boolean mapping_sensor_prepation;
        Boolean mapping_sensor_containning;
        Boolean chucking_on;
        Boolean chucking_off;
        Boolean cover_lock;
        Boolean cover_unlock;
        Boolean door_open;
        Boolean carrier_clamp;
        Boolean carrier_detecting_sensor_on;
        Boolean preparation_completed_external_output;
        Boolean carrying_properly_load;
        Boolean stage_rotation_back;
        Boolean stage_rotation_front;
        Boolean bcr_lifting;
        Boolean bcr_lowering;
        Boolean carrier_retainer_lowering;
        Boolean carrier_retainer_lifting;
        Boolean external_sw1led_load;
        Boolean external_sw3led_unload;
        Boolean load_led;
        Boolean unload_led;
        Boolean presence_led;
        Boolean placement_led;
        Boolean manual_led;
        Boolean error_led;
        Boolean clamp_led;
        Boolean dock_led;
        Boolean busy_led;
        Boolean auto_led;
        Boolean reserved_led;
        Boolean closed_led;
        Boolean lock_led;
        Boolean load_request;
        Boolean unload_request;
        Boolean ready;
        Boolean ho_avbl;
        Boolean es;
        #endregion
        public Main()
        {
            InitializeComponent();
            Thread thread1 = new Thread(TCPConnect);
            thread1.IsBackground = true;
            thread1.Start();
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
        }

        public void CheckGPIO()
        {
            if (Gpo_bin[PREPARATION_COMPLETED] == 1)
            {
                preparation_completed = true;
            }
            else
            {
                preparation_completed = false;
            }
            if (Gpo_bin[TEMPORARILY_STOP] == 1)
            {
                temporarily_stop = true;
            }
            else
            {
                preparation_completed = false;
            }
            if (Gpo_bin[SIGNIFICANT_ERROR] == 1)
            {
                significant_error = true;
            }
            else
            {
                preparation_completed = false;
            }
            if (Gpo_bin[LIGHT_ERROR] == 1)
            {
                light_error = true;
            }
            else
            {
                light_error = false;
            }
            if (Gpo_bin[DOOR_OPEN] == 1)
            {
                door_open = true;
            }
            else
            {
                door_open = false;
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
                        WriteCommand = "oSTG1.INIT" + '\r';
                        Writing.WriteLine(WriteCommand); // 명령 Write
                        listBox1.Items.Add(WriteCommand); // listbox에 명령 출력
                        break;
                    case "ORIGIN":
                        WriteCommand = "oSTG1.ORGN" + "\r";
                        Writing.WriteLine(WriteCommand);
                        listBox1.Items.Add(WriteCommand);
                        break;
                    case "RSTA":
                        WriteCommand = "oSTG1.RSTA(1)" + "\r";
                        Writing.WriteLine(WriteCommand);
                        listBox1.Items.Add(WriteCommand);
                        break;
                    case "CLAMP":
                        WriteCommand = "oSTG1.CLMP" + "\r";
                        Writing.WriteLine(WriteCommand);
                        listBox1.Items.Add(WriteCommand);
                        break;
                    case "UNCLAMP":
                        WriteCommand = "oSTG1.UCLM" + "\r";
                        Writing.WriteLine(WriteCommand);
                        listBox1.Items.Add(WriteCommand);
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
            NetworkStream stream = tcpClient1.GetStream();
            tcpconnect = true;
            Writing = new StreamWriter(tcpClient1.GetStream());
            Reading = new StreamReader(tcpClient1.GetStream());

            int n;
            Writing.AutoFlush = true;

            if (tcpClient1.Connected)
            {

                while ((n = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    label_connect.BackColor = Color.LightGreen;
                    stream.Write(buffer, 0, n);
                    string data = null;
                    data = Encoding.Default.GetString(buffer, 0, n);
                    string ReceiveData = data;
                    if (ReceiveData != null)
                    {
                        if (ReceiveData.Substring(0, 10) == "eSTG1.CNCT")
                        {
                            Writing.WriteLine("oSTG1.EVNT(0,1)" + '\r');
                        }


                        //    this.Invoke(new Action(delegate ()
                        //{
                        //    listBox1.Items.Add(ReceiveData);
                        //}));

                        string[] lines = ReceiveData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string strmsg in lines)
                        {
                            this.Invoke(new Action(delegate ()
                            {

                                listBox1.Items.Add(strmsg);
                                ProcessData(strmsg);
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                listBox1.SelectedIndex = -1;    // 리스트박스 자동스크롤
                                if (listBox1.Items.Count == 100) // 리스트 박스 100개 초과시, FIFO
                                {
                                    listBox1.Items.RemoveAt(listBox1.SelectedIndex + 1);
                                }
                            }));
                        }
                    }
                }
            }
        }

        private void ProcessData(string sData) // Receive Data Split 
        {
            string HED;
            string ID;
            string DATA;
            string sSTAT = "";
            string ErrorCode = "";
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

                        if (ErrorCode == "0000")
                        {
                            label_errorcode.BackColor = Color.White;
                            label_errorcode.Text = "Error CODE";
                        }
                        else      // Error Code
                        {
                            label_errorcode.BackColor = Color.LightGreen;
                            label_errorcode.Text = "Error CODE" + '\r' + ErrorCode;
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    int ConvertsSTAT = Convert.ToInt32(sSTAT[i] + "");
                                    if (ConvertsSTAT == 1)
                                    {
                                        label_init.Text = "Initial";
                                        label_init.BackColor = Color.LightGreen;
                                        //작동모드 원격
                                    }
                                    else
                                    {
                                        label_init.BackColor = Color.White;
                                    }
                                    break;
                                case 1:
                                    ConvertsSTAT = Convert.ToInt32(sSTAT[i] + "");
                                    if (ConvertsSTAT == 0)
                                    {
                                        label_origin.BackColor = Color.White;
                                        //원점복귀 미완료
                                    }
                                    else if (ConvertsSTAT == 1)
                                    {
                                        label_origin.BackColor = Color.LightGreen;
                                        //원점복귀 완료
                                    }
                                    break;
                                case 2:
                                    ConvertsSTAT = Convert.ToInt32(sSTAT[i] + "");
                                    if (ConvertsSTAT == 0)
                                    {
                                        //명령처리 정지
                                    }
                                    else if (ConvertsSTAT == 1)
                                    {
                                        //명령처리 처리
                                    }
                                    break;
                                case 3:
                                    ConvertsSTAT = Convert.ToInt32(sSTAT[i] + "");
                                    if (ConvertsSTAT == 0)
                                    {
                                        label_moving.BackColor = Color.White;
                                        //LD포트동작 상태 정지
                                    }
                                    else if (ConvertsSTAT == 1)
                                    {
                                        label_moving.Text = "MOVING";
                                        label_moving.BackColor = Color.LightGreen;
                                        //LD포트동작 상태 이동
                                    }
                                    else if (ConvertsSTAT == 2)
                                    {
                                        label_moving.BackColor = Color.White;
                                        //LD포트동작 상태 일시정지
                                    }
                                    break;
                                case 4:
                                    ConvertsSTAT = Convert.ToInt32(sSTAT[i] + "");
                                    if (ConvertsSTAT == 0)
                                    {
                                        //LD포트동작속도 보통
                                    }
                                    else
                                    {
                                        // 유지보수
                                    }
                                    break;
                            }
                        }

                    }

                    if (DATA == "GPIO")
                    {
                        Gpi = sData.Substring(sData.IndexOf(':') + 1, 16);  // 16진수를 2진수로 변환
                        Gpi_bin = Convert.ToString(Convert.ToInt64(Gpi, 16), 2);
                        if (Gpi_bin.Length < 63)
                        {
                            for (int i = 0; Gpi_bin.Length < 64; i++)
                            {
                                Gpi_bin = Gpi_bin.Insert(0, "0");
                            }
                        }
                        for (int i = 0; i < 64; i++)
                        {
                            Gpi_Array[63 - i] = int.Parse(Gpi_bin[i].ToString());
                        }

                        if(Gpi_Array[17] == 1)
                        {
                            label_clamp.BackColor = Color.LightGreen;
                        }
                        if(Gpi_Array[18]+ Gpi_Array[19]+ Gpi_Array[20] + Gpi_Array[25] == 4)
                        {
                            label_foup.BackColor = Color.LightGreen;
                        }
                        //Gpo = sData.Substring(sData.IndexOf('/') + 1, 16);
                        //Gpo_bin = Convert.ToString(Convert.ToInt32(Gpo, 16), 2);
                        //CheckGPIO();
                        //if (door_open)
                        //{
                        //    label_door.BackColor = Color.LightGreen;
                        //}
                    }

                    break;
            }
        }
    }
}

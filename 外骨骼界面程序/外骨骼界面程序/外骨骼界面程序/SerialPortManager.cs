using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Threading;

namespace 外骨骼界面程序
{
    class SerialPortManager
    {

        private string[] IsOpenSerialPortCount =null;

        private StreamWriter wr;
        private FileStream fs;
        private string dataString = null;
        private bool IsTrain = false;

        #region 压力参数
        private SerialPort SerialPort1;
                private SerialPort SerialPort2 ;
                private  SerialPort SerialPort3;
                public Int16[] tempPress = new Int16[8];//存储压力AD转换后的值（0-4096）
                private Int16[] tempAngle = new Int16[8];//存储倾角AD转换后的值（0-4096）
                public double[] dirangle = new double[8];//存储角度值（-90°到90°）

        #endregion

        #region 电机参数及其控制

        public double[] speed = new double[4];//速度
        public double[] current = new double[4];//电流
        public byte[] enable = new byte[4];//使能
        public byte[] direction = new byte[4];//方向
        #endregion

        #region 角度参数
        private DispatcherTimer ShowTimer1;
        private double angleDouble;//存储当前读取的传感器的角度数据    
        public double[] angle = new double[4];//存储四个传感器的角度
        private Int32 sendStep;//用来判断下一个发送的指令
        private int count = 0;//串口接收一次加一
        private int preCount = 0;//存储上一次的count，用来判断timer2时间内串口有没有接收到数据
        private Int16[] tempAngle2 = new Int16[8];//存储倾角AD转换后的值（0-4096）
        #endregion

        public double[] _angle = new double[8];
        private int _number = 0;
        private double[] _angleInitialization = new double[6];
        public string[] checkSerialPortCount()
        {
            IsOpenSerialPortCount = SerialPort.GetPortNames();
            return IsOpenSerialPortCount;
        }
        public bool SerialPortClose()
        {
            //byte[] closedbyte = new byte[19] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            //SendcontrolCMD(closedbyte);
            if (SerialPort1!=null)
            {
                SerialPort1.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(controlPort_DataReceived);

                SerialPort1.Close();
            }
            if (SerialPort2!=null)
            {
                SerialPort2.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(ReceivePressureDipAngleBytes);
                SerialPort2.Close();
            }
            if (SerialPort3!=null)
            {
               // SerialPort3.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(ReceiveAngleBytes);
                SerialPort3.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(CalculateAngle);
                ShowTimer1.Stop();
                SerialPort3.Close();
            }
            return true;
        }

        #region 压力传感器
        public void serialPortInt2(string comstring)
        {
            if (SerialPort2!=null)
            {
                if (SerialPort2.IsOpen)
                {
                    SerialPort2.Close();
                }
            }
          
            SerialPort2 = new SerialPort();
            SerialPort2.PortName = comstring;
            SerialPort2.BaudRate = 115200;
            SerialPort2.Parity = Parity.None;
            SerialPort2.StopBits = StopBits.One;
            SerialPort2.Open();
            SerialPort2.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(ReceivePressureDipAngleBytes);

        }
        public void ReceivePressureDipAngleBytes(object sender, SerialDataReceivedEventArgs e)
        {
            
            int bufferlen = SerialPort2.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            if (bufferlen >= 34)
            {
                byte[] bytes = new byte[bufferlen];//声明一个临时数组存储当前来的串口数据
                SerialPort2.Read(bytes, 0, bufferlen);//读取串口内部缓冲区数据到buf数组
                SerialPort2.DiscardInBuffer();//清空串口内部缓存
                                              //处理和存储数据
                Int16 endFlag = BitConverter.ToInt16(bytes, 32);
                if (endFlag == 2573)
                {
                    for (int f = 0; f < 8; f++)
                    {
                        tempPress[f] = BitConverter.ToInt16(bytes, f * 2);
                        tempAngle[f] = BitConverter.ToInt16(bytes, f * 2 + 16);
                        dirangle[f] = (Convert.ToDouble(tempAngle[f]) * (3.3 / 4096) - 0.7444) / 1.5 * 180;
                    }
                    if (IsTrain)
                    {

                        dataString += System.DateTime.Now.ToString("HH:mm:ss:fff"); ;

                        for (int f = 0; f < 8; f++)
                        {
                            dataString += " " + tempPress[f].ToString("F2");
                        }

                        for (int f = 0; f < 6; f++)
                        {
                            dataString += " " + _angle[f].ToString("F2");
                        }
                        for (int f = 0; f < 8; f++)
                        {
                            dataString += " " + dirangle[f].ToString("F2");
                        }
                        dataString += "\r\n ";

                        wr.Write(dataString);
                        wr.Flush();
                        dataString = null;
                    }
                }
            }
           
           
        }
#endregion

        #region 角度传感器
        public void serialPortInt3(string comstring)
        {

            //for (int i = 0; i < 6; i++)
            //    _angleInitialization[i] = 0;

            //int numberOfGather = 5;
            //for (int i = 0; i < numberOfGather; i++)
            //{
            //    for (int j = 0; j < 6; j++)
            //        _angleInitialization[j] += _angle[j];
            //}
            //for (int i = 0; i < 6; i++)
            //    _angleInitialization[i] /= numberOfGather;
            if (SerialPort3 != null)
            {
                if (SerialPort3.IsOpen)
                {
                    SerialPort3.Close();
                }
            }

            SerialPort3 = new SerialPort();
            SerialPort3.PortName = comstring;
            SerialPort3.BaudRate = 115200;
            SerialPort3.Parity = Parity.None;
            SerialPort3.StopBits = StopBits.One;
            SerialPort3.Open();
           // SerialPort3.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(ReceiveAngleBytes);
            SerialPort3.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(CalculateAngle);

            
            //ShowTimer1 = new System.Windows.Threading.DispatcherTimer();
            //ShowTimer1.Tick += new EventHandler(timer2_Tick);//起个Timer一直获取当前时间
            //ShowTimer1.Interval = new TimeSpan(0, 0, 0, 0, 100);
            //ShowTimer1.Start();
            SendCommands();
        }

        public void serialPotr3Int()
        {
            for (int i = 0; i < 6; i++)
                _angleInitialization[i] = 0;

            int numberOfGather = 5;
            for (int i = 0; i < numberOfGather; i++)
            {
                for (int j = 0; j < 6; j++)
                    _angleInitialization[j] += _angle[j];
            }
            for (int i = 0; i < 6; i++)
                _angleInitialization[i] /= numberOfGather;
        }
        private void SendCommands()
        {
            byte[] command = new byte[9];
            command[0] = 0x77;
            command[1] = 0x00;
            command[2] = 0x01;
            command[3] = 0x01;
            command[4] = 0x00;
            command[5] = 0x0E;
            command[6] = 0x00;
            command[7] = Convert.ToByte(0xFF - (command[1] + command[2] + command[3] + command[4] + command[5] + command[6]));
            command[8] = 0xAA;

           SerialPort3 .Write(command, 0, 9);
        }

        private void CalculateAngle(object sender, SerialDataReceivedEventArgs e)
        {

            int bufferlen = SerialPort3.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            if (bufferlen >= 34)
            {
                byte[] bytes = new byte[bufferlen];//声明一个临时数组存储当前来的串口数据
                SerialPort3.Read(bytes, 0, bufferlen);//读取串口内部缓冲区数据到buf数组
                SerialPort3.DiscardInBuffer();//清空串口内部缓存
                                              //处理和存储数据
                Int16 endFlag = BitConverter.ToInt16(bytes, 32);
                if (endFlag == 2573)
                {
                    for (int f = 0; f < 4; f++)
                    {
                        tempAngle2[f] = BitConverter.ToInt16(bytes, f * 2 + 16);
                        _angle[f] = Convert.ToDouble(tempAngle2[f]) / 4096 * 3.3 / 3.05 * 360;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        _angle[i] = _angle[i] - _angleInitialization[i];
                    }

                }


            }

            //    _number = SerialPort3.BytesToRead;

            //    if (_number < 25)
            //        return;

            //    try
            //    {
            //        byte[] data = new byte[_number];
            //        SerialPort3.Read(data, 0, _number);

            //        if (0xFF -
            //            (data[1] + data[2] + data[3] + data[4] + data[5] + data[6] + data[7] + data[8] + data[9] +
            //             data[10] +
            //             data[11] + data[12] + data[13] + data[14] + data[15] + data[16] + data[17] + data[18] +
            //             data[19] +
            //             data[20] + data[21] + data[22]) % 256 == data[23])
            //        {


            //            左髋、左膝、右髋、右膝
            //           _angle[0] = 256 * data[7] + data[6];
            //            _angle[1] = 256 * data[9] + data[8];
            //            _angle[2] = 256 * data[11] + data[10];
            //            _angle[3] = 256 * data[13] + data[12];
            //            _angle[4] = 256 * data[15] + data[14];
            //            _angle[5] = 256 * data[17] + data[16];
            //            for (int i = 0; i < 6; i++)
            //            {
            //                if (_angle[i] >= 0x1000)
            //                {
            //                    _angle[i] = 0xFFFF - _angle[i] + 1;
            //                    _angle[i] *= -1;
            //                }
            //                _angle[i] = _angle[i] / 10.0 - _angleInitialization[i];
            //            }

            //        }
            //    }
            //    catch (Exception exception)
            //    {
            //    }

            //    SendCommands();
            //}

        }



        //private void ReceiveAngleBytes(object sender, SerialDataReceivedEventArgs e)//串口接收代码
        //{

            //    int bufferlen = SerialPort3.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            //    if (bufferlen >= 7)
            //    {
            //        byte[] bytes = new byte[bufferlen];//声明一个临时数组存储当前来的串口数据
            //        SerialPort3.Read(bytes, 0, bufferlen);//读取串口内部缓冲区数据到buf数组
            //        SerialPort3.DiscardInBuffer();//清空串口缓存区
            //        angleDouble = (Convert.ToDouble(bytes[3] * 256 + bytes[4])) / 16384 * 360;//计算当前读取的传感器的角度数据
            //        count++;
            //        判断当前读取的传感器编号并存储角度数据
            //            if (bytes[0] == 0xA0)
            //        {
            //            angle[0] = angleDouble;
            //            sendStep = 1;
            //        }
            //        else if (bytes[0] == 0xA1)
            //        {
            //            angle[1] = angleDouble;
            //            sendStep = 2;
            //        }
            //        else if (bytes[0] == 0xA2)
            //        {
            //            angle[2] = angleDouble;
            //            sendStep = 3;
            //        }
            //        else if (bytes[0] == 0xA3)
            //        {
            //            angle[3] = angleDouble;
            //            sendStep = 0;
            //        }

            //        判断下一个要发送的指令
            //            if (sendStep == 0)
            //        {
            //            SendCommands0();

            //        }
            //        else if (sendStep == 1)
            //        {
            //            SendCommands1();

            //        }
            //        else if (sendStep == 2)
            //        {
            //            SendCommands2();

            //        }
            //        else if (sendStep == 3)
            //        {

            //            SendCommands3();
            //        }


            //    }

            //}

            //private void SendCommands0()
            //{
            //    byte[] command = new byte[8] { 0xA0, 0x03, 0x00, 0x64, 0x00, 0x01, 0xDC, 0xA4 };
            //    command[0] = 0xA0;
            //    command[1] = 0x03;
            //    command[2] = 0x00;
            //    command[3] = 0x64;
            //    command[4] = 0x00;
            //    command[5] = 0x01;
            //    command[6] = 0xDC;
            //    command[7] = 0xA4;
            //    SerialPort3.Write(command, 0, 8);
            //}

            //private void SendCommands1()
            //{
            //    byte[] command = new byte[8];
            //    command[0] = 0xA1;
            //    command[1] = 0x03;
            //    command[2] = 0x00;
            //    command[3] = 0x64;
            //    command[4] = 0x00;
            //    command[5] = 0x01;
            //    command[6] = 0xDD;
            //    command[7] = 0x75;
            //    SerialPort3.Write(command, 0, 8);
            //}
            //private void SendCommands2()
            //{
            //    byte[] command = new byte[8];
            //    command[0] = 0xA2;
            //    command[1] = 0x03;
            //    command[2] = 0x00;
            //    command[3] = 0x64;
            //    command[4] = 0x00;
            //    command[5] = 0x01;
            //    command[6] = 0xDD;
            //    command[7] = 0x46;
            //    SerialPort3.Write(command, 0, 8);
            //}
            //private void SendCommands3()
            //{
            //    byte[] command = new byte[8];
            //    command[0] = 0xA3;
            //    command[1] = 0x03;
            //    command[2] = 0x00;
            //    command[3] = 0x64;
            //    command[4] = 0x00;
            //    command[5] = 0x01;
            //    command[6] = 0xDC;
            //    command[7] = 0x97;
            //    SerialPort3.Write(command, 0, 8);
            //}
            //private void timer2_Tick(object sender, EventArgs e)  //定时器2  用来判断100ms内是否接受过串口数据，若没有证明串口出错了，发指令0重新开始接受串口
            //{
            //    if (preCount == count) SendCommands0();
            //    preCount = count;
            //}
            #endregion

            #region 电机及其控制

        public void serialPortInt1(string comstring)
        {
            if (SerialPort1 != null)
            {
                if (SerialPort1.IsOpen)
                {
                    SerialPort1.Close();
                }
            }

            SerialPort1 = new SerialPort();
            SerialPort1.PortName = comstring;
            SerialPort1.BaudRate = 115200;
            SerialPort1.Parity = Parity.None;
            SerialPort1.StopBits = StopBits.One;
            SerialPort1.Open();
            SerialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(controlPort_DataReceived);

        }
        private void controlPort_DataReceived(object sender, SerialDataReceivedEventArgs e)//压力倾角串口接收代码
        {
            try
            {
                int bufferlen = SerialPort1.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                if (bufferlen >= 27)
                {
                    byte[] bytes = new byte[bufferlen];//声明一个临时数组存储当前来的串口数据
                    SerialPort1.Read(bytes, 0, bufferlen);//读取串口内部缓冲区数据到buf数组
                    SerialPort1.DiscardInBuffer();//清空串口内部缓存
                    //处理和存储数据
                    Int16 endFlag = BitConverter.ToInt16(bytes, 25);
                    if (endFlag == 2573)
                    {
                        if (bytes[0] == 0x23)
                            for (int f = 0; f < 4; f++)
                            {
                                enable[f] = bytes[f * 6 + 1];
                                direction[f] = bytes[f * 6 + 2];
                                speed[f] = bytes[f * 6 + 3] * 256 + bytes[f * 6 + 4];
                                if (speed[f] >= 2048) speed[f] = (speed[f] - 2048) / 4096 * 5180;
                                else speed[f] = (2048 - speed[f]) / 4096 * -5180;
                                current[f] = bytes[f * 6 + 5] * 256 + bytes[f * 6 + 6];
                                if (current[f] >= 2048) current[f] = (current[f] - 2048) / 4096 * 30;
                                else current[f] = (2048 - current[f]) / 4096 * -30;
                            }


                    }

                }
            }
            catch
            {


            }
        }
        public void SendcontrolCMD(byte[] command)
        {
           // byte[] command = new byte[19];
            command[0] = 0x23;//开始字符
            //command[1] = 0x01;//电机A 使能端
            //command[2] = 0x01;//电机A 方向
            //command[3] = 0x08;//电机A 转速高位
            //command[4] = 0x88;//电机A 转速低位（范围1800-16200）对应速度范围（0-2590r/min）
            //command[5] = 0x01;//电机B
            //command[6] = 0x01;//电机B
            //command[7] = 0x08;//电机B
            //command[8] = 0x88;//电机B
            //command[9] = 0x01;//电机C
            //command[10] = 0x01;//电机C
            //command[11] = 0x08;//电机C
            //command[12] = 0x88;//电机C
            //command[13] = 0x01;//电机D
            //command[14] = 0x01;//电机D
            //command[15] = 0x08;//电机D
            //command[16] = 0x88;//电机D
            command[17] = 0x0D;//结束字符
            command[18] = 0x0A;
            SerialPort1.Write(command, 0, 19);
        }

        #endregion

        public void BeginTrain(string peoplestring)
        {
            string nowTime = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string filestream = nowTime + ".txt";
            fs = new FileStream(filestream, FileMode.Append);
            wr = new StreamWriter(fs);
            dataString+=peoplestring + "\r\n";
            IsTrain = true;



        }



        public void OverTrain()
        {
            IsTrain = false;
            wr.Close();
            fs.Close();
            dataString = null;

        }

  
    }
}

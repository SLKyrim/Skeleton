using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;


namespace Skeleton_Monitor
{
    //存放各种方法
    class Methods
    {
        #region 参数定义

        //串口
        private SerialPort motor_SerialPort = new SerialPort(); //电机串口
        private SerialPort press_SerialPort = new SerialPort(); //压力与倾角传感器串口
        private SerialPort angle_SerialPort = new SerialPort(); //角度传感器串口

        //电机的4个参数 
        public byte[] enable = new byte[4];        //使能
        public byte[] direction = new byte[4];     //方向
        public double[] speed = new double[4];     //转速
        public double[] current = new double[4];   //电流

        //8个压力传感器所需参数（实际只用到1个压力传感器模拟重物，6个压力传感器安装在鞋垫）
        //4个倾角传感器（分别有x轴和y轴）所需参数（实际只用到1个倾角传感器）
        public Int16[] tempPress = new Int16[8];   //存储压力AD转换后的值（0-4096）
        private Int16[] tempAngle = new Int16[8];  //存储倾角AD转换后的值（0-4096）
        public double[] dirangle = new double[8];  //存储顷角度值（-90°到90°）

        //6个角度传感器所需参数（只用到4个角度传感器）
        public double[] _angle = new double[6];//存储角度值（0°到 ?°)；只用到前4个
        public double[] _angleInitialization = new double[6] { 0, 0, 0, 0, 0, 0 };//【角度初始化】按钮也用到
        private Int16[] tempAngle2 = new Int16[8];//存储倾角AD转换后的值（0-4096）
        private double[] auxiliary_angle = new double[6];//初始角度的补角：角度传感器初始值在290°以上时，为防止转到360°后角度突变为0°，需要用到

        //关闭窗口
        //private DispatcherTimer ShowTimer1;

        //获取可用串口名
        private string[] IsOpenSerialPortCount = null;

        #endregion

        #region 电机
        public void motor_SerialPort_Init(string comstring)//电机串口初始化
        {
            if (motor_SerialPort != null)
            {
                if (motor_SerialPort.IsOpen)
                {
                    motor_SerialPort.Close();
                }
            }

            motor_SerialPort = new SerialPort();
            motor_SerialPort.PortName = comstring;
            motor_SerialPort.BaudRate = 115200;
            motor_SerialPort.Parity = Parity.None;
            motor_SerialPort.StopBits = StopBits.One;
            motor_SerialPort.Open();
            motor_SerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(motor_DataReceived);
        }

        private void motor_DataReceived(object sender, SerialDataReceivedEventArgs e)//电机及控制串口接收数据（算法）
        {
            try
            {
                int bufferlen = motor_SerialPort.BytesToRead;    //先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                if (bufferlen >= 27)                             //一个电机有使能，方向，转速，电流4个参数，前两个各占1个位，后两个各占2个位，故一个电机数据占6各位，加上一个开始位，两个停止位，故总有1+6*4+2=27位
                {
                    byte[] bytes = new byte[bufferlen];          //声明一个临时数组存储当前来的串口数据
                    motor_SerialPort.Read(bytes, 0, bufferlen);  //读取串口内部缓冲区数据到buf数组
                    motor_SerialPort.DiscardInBuffer();          //清空串口内部缓存
                    //处理和存储数据
                    Int16 endFlag = BitConverter.ToInt16(bytes, 25);
                    if (endFlag == 2573)                         //停止位0A0D (0D0A?)
                    {
                        if (bytes[0] == 0x23)
                            for (int f = 0; f < 4; f++)
                            {
                                enable[f] = bytes[f * 6 + 1];
                                direction[f] = bytes[f * 6 + 2];
                                speed[f] = bytes[f * 6 + 3] * 256 + bytes[f * 6 + 4];
                                if (speed[f] >= 2048) speed[f] = (speed[f] - 2048) / 4096 * 5180;          //实际范围-2590~2590,而对应范围是0~4096，故中间值位2048
                                else speed[f] = (2048 - speed[f]) / 4096 * -5180;
                                current[f] = bytes[f * 6 + 5] * 256 + bytes[f * 6 + 6];
                                if (current[f] >= 2048) current[f] = (current[f] - 2048) / 4096 * 30;
                                else current[f] = (2048 - current[f]) / 4096 * -30;
                            }
                    }
                }
            }
            catch { }
        }

        public void SendControlCMD(byte[] command)//电机串口写入字节命令
        {
            //byte[] command = new byte[19];
            command[0] = 0x23;//开始字符
            //command[1] = 0x01;//电机1 使能端
            //command[2] = 0x01;//电机1 方向
            //command[3] = 0x08;//电机1 转速高位
            //command[4] = 0x88;//电机1 转速低位（范围1800-16200）对应速度范围（0-2590r/min）
            //command[5] = 0x01;//电机2
            //command[6] = 0x01;//电机2
            //command[7] = 0x08;//电机2
            //command[8] = 0x88;//电机2
            //command[9] = 0x01;//电机3
            //command[10] = 0x01;//电机3
            //command[11] = 0x08;//电机3
            //command[12] = 0x88;//电机3
            //command[13] = 0x01;//电机4
            //command[14] = 0x01;//电机4
            //command[15] = 0x08;//电机4
            //command[16] = 0x88;//电机4
            command[17] = 0x0D;//结束字符
            command[18] = 0x0A;
            motor_SerialPort.Write(command, 0, 19);
        }
        #endregion

        #region 压力与倾角传感器
        public void press_SerialPort_Init(string comstring)//压力与倾角传感器串口初始化
        {
            if (press_SerialPort != null)
            {
                if (press_SerialPort.IsOpen)
                {
                    press_SerialPort.Close();
                }
            }

            press_SerialPort = new SerialPort();
            press_SerialPort.PortName = comstring;
            press_SerialPort.BaudRate = 115200;
            press_SerialPort.Parity = Parity.None;
            press_SerialPort.StopBits = StopBits.One;
            press_SerialPort.Open();
            press_SerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(press_DataReceived);
        }

        public void press_DataReceived(object sender, SerialDataReceivedEventArgs e)//接受压力相关数据的委托事件（算法）
        {
            int bufferlen = press_SerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            if (bufferlen >= 34)                   //前16位储存8个压力值，后16位储存8个倾角值，最后2位是停止位
            {
                byte[] bytes = new byte[bufferlen];//声明一个临时数组存储当前来的串口数据
                press_SerialPort.Read(bytes, 0, bufferlen);//读取串口内部缓冲区数据到buf数组
                press_SerialPort.DiscardInBuffer();//清空串口内部缓存
                                                   //处理和存储数据
                Int16 endFlag = BitConverter.ToInt16(bytes, 32);
                if (endFlag == 2573)
                {
                    for (int f = 0; f < 8; f++)
                    {
                        tempPress[f] = BitConverter.ToInt16(bytes, f * 2);    //byte是8位，tempPress是Int16，即16位，所以一个Int16占byte数组两个位   
                        tempAngle[f] = BitConverter.ToInt16(bytes, f * 2 + 16);
                        dirangle[f] = (Convert.ToDouble(tempAngle[f]) * (3.3 / 4096) - 0.7444) / 1.5 * 180;
                    }
                }
            }
        }
        #endregion

        #region 角度传感器
        public void angle_SerialPort_Init(string comstring)//角度传感器串口初始化
        {
            if (angle_SerialPort != null)
            {
                if (angle_SerialPort.IsOpen)
                {
                    angle_SerialPort.Close();
                }
            }

            angle_SerialPort = new SerialPort();
            angle_SerialPort.PortName = comstring;
            angle_SerialPort.BaudRate = 115200;
            angle_SerialPort.Parity = Parity.None;
            angle_SerialPort.StopBits = StopBits.One;
            angle_SerialPort.Open();
           
            angle_SerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(angle_DataReceived);

            SendAngleCMD();
        }

        private void angle_DataReceived(object sender, SerialDataReceivedEventArgs e)//对角度传感器串口增加的委托事件（算法）
        {
            int bufferlen = angle_SerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            if (bufferlen >= 34)
            {
                byte[] bytes = new byte[bufferlen];//声明一个临时数组存储当前来的串口数据
                angle_SerialPort.Read(bytes, 0, bufferlen);//读取串口内部缓冲区数据到buf数组
                angle_SerialPort.DiscardInBuffer();//清空串口内部缓存
                                                   //处理和存储数据
                Int16 endFlag = BitConverter.ToInt16(bytes, 32);
                if (endFlag == 2573)
                {
                    for (int f = 0; f < 4; f++)
                    { 
                        tempAngle2[f] = BitConverter.ToInt16(bytes, f * 2 + 16);              //用到的4个角度传感器的值看来是存放在bytes[16]到bytes[23]这个8个位上
                        _angle[f] = Convert.ToDouble(tempAngle2[f]) / 4096 * 3.3 / 3.05 * 360;//从AD值转化为对应角度值
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        if(_angleInitialization[i] < 290)
                        {
                            _angle[i] = _angle[i] - _angleInitialization[i];//角度初始化后的_angleInitialization[i]基本和_angle[i]相等，这样获得的_angle[i]即为初始值0
                        }
                        else//当角度传感器可能超过360°而突变为0°时
                        {
                            if (_angle[i] - _angleInitialization[i] > -5)//理论是0，但误差范围可以给大些，转过360°后该值会突变成非常小的负数（-300°左右）
                                _angle[i] = _angle[i] - _angleInitialization[i];
                            else
                                _angle[i] = _angle[i] + auxiliary_angle[i];
                        }
                    }
                }
            }
        }

        public void SendAngleCMD()//角度串口写入字节命令
        {
            byte[] command = new byte[9]; //byte类型用于存放二进制数据
            command[0] = 0x77;            //0x开头的是十六进制数据
            command[1] = 0x00;
            command[2] = 0x01;
            command[3] = 0x01;
            command[4] = 0x00;
            command[5] = 0x0E;
            command[6] = 0x00;
            command[7] = Convert.ToByte(0xFF - (command[1] + command[2] + command[3] + command[4] + command[5] + command[6]));
            command[8] = 0xAA;

            angle_SerialPort.Write(command, 0, 9);
        }

        public void AngleInit()//角度初始化
        {
            for (int i = 0; i < 6; i++)
                _angleInitialization[i] = 0;

            int numberOfGather = 5;  //_angleInitialization[j]加五次实际实时角度值_angle[j]，再除以5得实际的去除部分误差的初始值 _angleInitialization[i]

            for (int i = 0; i < numberOfGather; i++)
            {
                for (int j = 0; j < 6; j++)
                    _angleInitialization[j] += _angle[j];
            }
            for (int i = 0; i < 6; i++)
            {
                _angleInitialization[i] /= numberOfGather;
                auxiliary_angle[i] = 360.00 - _angleInitialization[i];//初始角度的补角
            }
                
        }
        #endregion

        public string[] CheckSerialPortCount()//获取可用串口名
        {
            IsOpenSerialPortCount = SerialPort.GetPortNames();
            return IsOpenSerialPortCount;
        }

        public bool SerialPortClose()//关闭窗口时执行
        {
            if (motor_SerialPort != null)
            {
                motor_SerialPort.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(motor_DataReceived);

                motor_SerialPort.Close();
            }
            if (press_SerialPort != null)
            {
                press_SerialPort.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(press_DataReceived);
                press_SerialPort.Close();
            }
            if (angle_SerialPort != null)
            {
                angle_SerialPort.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(angle_DataReceived);
                //ShowTimer1.Stop();
                angle_SerialPort.Close();
            }
            return true;
        }

    }
}

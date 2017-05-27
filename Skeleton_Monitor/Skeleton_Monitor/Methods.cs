using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Threading;
using System.Windows.Controls;


namespace Skeleton_Monitor
{
    class Methods
    {

        public void InitPort(SerialPort myPort, string portName)//串口初始化
        {
            myPort = new SerialPort();
            myPort.PortName = portName;
            myPort.BaudRate = 115200;
            myPort.Parity = Parity.None;
            myPort.StopBits = StopBits.One;
            myPort.Open();
        }

        private SerialPort angle_SerialPort = new SerialPort(); //角度传感器串口
        public void SendCommands()//角度串口写入字节命令
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

    }
}

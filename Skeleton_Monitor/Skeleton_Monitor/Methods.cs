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

        ////扫描串口
        //public int comcount = 0;                  //用来存储计算机可用串口数目，初始化为0
        //public bool flag = false;

        #endregion

        //public void ScanPorts(string[] mySPCount, 
        //                      ComboBox myMotor_comboBox,
        //                      ComboBox myPress_comboBox,
        //                      ComboBox myAngle_comboBox,
        //                      string mymotor_com,
        //                      string mypress_com,
        //                      string myangle_com,
        //                      TextBox myOut_textBox)//扫描可用串口
        //{
        //    mySPCount = SerialPort.GetPortNames();      //获得计算机可用串口名称数组
        //    ComboBoxItem tempComboBoxItem = new ComboBoxItem();
        //    if (comcount != mySPCount.Length)            //SPCount.length其实就是可用串口的个数
        //    {
        //        //当可用串口计数器与实际可用串口个数不相符时
        //        //初始化三个下拉窗口并将flag初始化为false

        //        myMotor_comboBox.Items.Clear();
        //        myPress_comboBox.Items.Clear();
        //        myAngle_comboBox.Items.Clear();

        //        tempComboBoxItem = new ComboBoxItem();
        //        tempComboBoxItem.Content = "请选择串口";
        //        myMotor_comboBox.Items.Add(tempComboBoxItem);
        //        myMotor_comboBox.SelectedIndex = 0;

        //        tempComboBoxItem = new ComboBoxItem();
        //        tempComboBoxItem.Content = "请选择串口";
        //        myPress_comboBox.Items.Add(tempComboBoxItem);
        //        myPress_comboBox.SelectedIndex = 0;

        //        tempComboBoxItem = new ComboBoxItem();
        //        tempComboBoxItem.Content = "请选择串口";
        //        myAngle_comboBox.Items.Add(tempComboBoxItem);
        //        myAngle_comboBox.SelectedIndex = 0;

        //        mymotor_com = null;
        //        mypress_com = null;
        //        myangle_com = null;

        //        flag = false;

        //        if (comcount != 0)
        //        {
        //            //在操作过程中增加或减少串口时发生
        //            MessageBox.Show("串口数目已改变，请重新选择串口");
        //        }

        //        comcount = mySPCount.Length;     //将可用串口计数器与现在可用串口个数匹配
        //    }

        //    if (!flag)
        //    {
        //        if (mySPCount.Length > 0)
        //        {
        //            //有可用串口时执行
        //            comcount = mySPCount.Length;

        //            myOut_textBox.Text = "检测到" + mySPCount.Length + "个串口!";

        //            for (int i = 0; i < mySPCount.Length; i++)
        //            {
        //                //分别将可用串口添加到三个下拉窗口中
        //                string tempstr = "串口" + mySPCount[i];

        //                tempComboBoxItem = new ComboBoxItem();
        //                tempComboBoxItem.Content = tempstr;
        //                myMotor_comboBox.Items.Add(tempComboBoxItem);

        //                tempComboBoxItem = new ComboBoxItem();
        //                tempComboBoxItem.Content = tempstr;
        //                myPress_comboBox.Items.Add(tempComboBoxItem);

        //                tempComboBoxItem = new ComboBoxItem();
        //                tempComboBoxItem.Content = tempstr;
        //                myAngle_comboBox.Items.Add(tempComboBoxItem);
        //            }

        //            flag = true;

        //        }
        //        else
        //        {
        //            comcount = 0;
        //            myOut_textBox.Text = "未检测到串口!";
        //        }
        //    }
        //}

        public void InitPort(SerialPort myPort, string portName)//串口初始化
        {
            myPort = new SerialPort();
            myPort.PortName = portName;
            myPort.BaudRate = 115200;
            myPort.Parity = Parity.None;
            myPort.StopBits = StopBits.One;
            myPort.Open();
        }

        public void SendAngleCMD(SerialPort myPort)//角度串口写入字节命令
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

            myPort.Write(command, 0, 9);
        }

        public void SendControlCMD(SerialPort myPort, byte[] command)//电机串口写入字节命令
        {
            //byte[] command = new byte[19];
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
            myPort.Write(command, 0, 19);
        }

    }
}

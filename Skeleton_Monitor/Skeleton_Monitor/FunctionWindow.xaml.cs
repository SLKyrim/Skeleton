﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.IO;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Skeleton_Monitor
{
    /// <summary>
    /// FunctionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionWindow : Page
    {
        //存放各种委托事件
        public FunctionWindow()
        {
            InitializeComponent();
        }

        private Methods methods = new Methods();

        #region 参数定义
        //文本输出相关参数
        private DispatcherTimer ShowTimer;

        //扫描串口
        public string[] SPCount = null;           //用来存储计算机串口名称数组
        public int comcount = 0;                  //用来存储计算机可用串口数目，初始化为0
        public bool flag = false;
        public string motor_com = null;           //存储电机所用串口
        public string press_com = null;           //存储压力及倾角传感器所用串口
        public string angle_com = null;           //存储角度传感器所用串口

        //压力与倾角传感器所需参数
        private bool PressSerialPortIsDone = false;

        //【添加命令】及【添加命令】按钮所需参数
        private byte[] cmdSendBytes = new byte[19]; //储存从电机控制窗口输入的字节命令
        public int choosecount = 0;                 //记录已添加命令的电机的个数

        //【角度初始化】按钮及Action委托所需参数
        public bool IsTrueForefootZero = false;

        //【动作开始】按钮及Action委托所需参数
        public bool IsTrueClickDown = false;

        //Action委托所需参数
        public bool IsTrueStanduping = false;
        public bool IsTrueSquatting = false;
        public bool istrueX = false;
        public bool istrueY = false;

        //动作模式选择面板
        public bool isActionPick = false;
        public bool isActionWalk = false;

        //ActionWalk所需参数
        //public bool DSt_left = false;   //进入左腿摆动相的过渡相
        //public bool DSt_right = false;  //进入右腿摆动相的过渡相
        //public bool init_LSW = false;   //左腿摆动相前期
        //public bool mid_LSW = false;    //左腿摆动相中期
        //public bool term_LSW = false;   //左腿摆动相后期
        //public bool init_RSW = false;   //右腿摆动相前期
        //public bool mid_RSW = false;    //右腿摆动相中期
        //public bool term_RSW = false;   //右腿摆动相后期
        public bool mid_flag = false;
        //public bool do_walk = false;
        public int status = 0;
        //public bool stop_walk = false;  //穿戴者通过按压力传感器8进行自主停机

        //电机默认执行速度
        int rSpeed1 = 3000;
        int rSpeed2 = 2500;
        #endregion

        #region ComboBox控件

        private void Motor_comboBox_DropDownClosed(object sender, EventArgs e)//电机及控制串口下拉窗口关闭后执行
        {
            ComboBoxItem item = Motor_comboBox.SelectedItem as ComboBoxItem; //下拉窗口当前选中的项赋给item
            string tempstr = item.Content.ToString();                        //将选中的项目转为字串存储在tempstr中

            for (int i = 0; i < SPCount.Length; i++)
            {
                if (tempstr == "串口" + SPCount[i])
                {
                    //当选中串口为串口SPCount[i]时
                    if (press_com == SPCount[i] || angle_com == SPCount[i])
                    {
                        //压力与倾角传感器或角度传感器已占用串口SPCount[i]时
                        //MessageBox.Show("串口" + SPCount[i] + "已被占用!");
                        statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                        statusInfoTextBlock.Text = "串口" + SPCount[i] + "已被占用!";
                    }
                    else
                    {
                        motor_com = SPCount[i];

                        methods.motor_SerialPort_Init(SPCount[i]);

                        //if (motor_SerialPort.IsOpen)   //如果电机正在使用串口，则关闭串口以备初始化
                        //    motor_SerialPort.Close();

                        ////电机串口初始化
                        //InitPort(motor_SerialPort, SPCount[i]);
                        //motor_SerialPort.DataReceived += new SerialDataReceivedEventHandler(motor_DataReceived);  //接收事件处理方法motor_DataReceived即电机的算法
                    }
                }
            }
        }

        private void Press_comboBox_DropDownClosed(object sender, EventArgs e)//压力及倾角串口下拉窗口关闭后执行
        {
            ComboBoxItem item = Press_comboBox.SelectedItem as ComboBoxItem; //下拉窗口当前选中的项赋给item
            string tempstr = item.Content.ToString();                        //将选中的项目转为字串存储在tempstr中

            for (int i = 0; i < SPCount.Length; i++)
            {
                if (tempstr == "串口" + SPCount[i])
                {
                    //当选中串口为串口SPCount[i]时
                    if (motor_com == SPCount[i] || angle_com == SPCount[i])
                    {
                        //电机或角度传感器已占用串口SPCount[i]时
                        //MessageBox.Show("串口" + SPCount[i] + "已被占用!");
                        statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                        statusInfoTextBlock.Text = "串口" + SPCount[i] + "已被占用!";
                    }
                    else
                    {
                        press_com = SPCount[i];

                        methods.press_SerialPort_Init(SPCount[i]);

                        PressSerialPortIsDone = true;
                        //if (press_SerialPort.IsOpen)   //如果压力与倾角传感器正在使用串口，则关闭串口以备初始化
                        //    press_SerialPort.Close();

                        ////压力与倾角传感器串口初始化
                        //InitPort(press_SerialPort, SPCount[i]);
                        //press_SerialPort.DataReceived += new SerialDataReceivedEventHandler(press_DataReceived);  //接收事件处理方法press_DataReceived即电机的算法
                    }
                }
            }
        }

        private void Angle_comboBox_DropDownClosed(object sender, EventArgs e)//角度传感器串口下拉窗口关闭后执行
        {
            ComboBoxItem item = Angle_comboBox.SelectedItem as ComboBoxItem; //下拉窗口当前选中的项赋给item
            string tempstr = item.Content.ToString();                        //将选中的项目转为字串存储在tempstr中

            for (int i = 0; i < SPCount.Length; i++)
            {
                if (tempstr == "串口" + SPCount[i])
                {
                    //当选中串口为串口SPCount[i]时
                    if (motor_com == SPCount[i] || press_com == SPCount[i])
                    {
                        //电机或压力与倾角传感器已占用串口SPCount[i]时
                        //MessageBox.Show("串口" + SPCount[i] + "已被占用!");
                        statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                        statusInfoTextBlock.Text = "串口" + SPCount[i] + "已被占用!";
                    }
                    else
                    {
                        angle_com = SPCount[i];

                        methods.angle_SerialPort_Init(SPCount[i]);

                        //if (angle_SerialPort.IsOpen)   //如果角度传感器正在使用串口，则关闭串口以备初始化
                        //    angle_SerialPort.Close();

                        ////电机串口初始化
                        //InitPort(angle_SerialPort, SPCount[i]);
                        //angle_SerialPort.DataReceived += new SerialDataReceivedEventHandler(angle_DataReceived);  //接收事件处理方法angle_DataReceived即电机的算法

                        //SendAngleCMD(angle_SerialPort);
                    }
                }
            }
        }

        #endregion

        #region 文本输出
        private void FunctionWindow_Loaded(object sender, RoutedEventArgs e)//打开窗口后进行的初始化操作
        {
            ShowTimer = new System.Windows.Threading.DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer); //Tick是超过计时器间隔时发生事件，此处为Tick增加了一个叫ShowCurTimer的取当前时间并扫描串口的委托
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0); //设置刻度之间的时间值，设定为1秒（即文本框会1秒改变一次输出文本）
            ShowTimer.Start();

            DispatcherTimer showTimer = new DispatcherTimer();
            showTimer.Tick += new EventHandler(ShowSenderTimer); //增加了一个叫ShowSenderTimer的在电机和传感器的只读文本框中输出信息的委托


            showTimer.Tick += new EventHandler(ActionPick);

            showTimer.Tick += new EventHandler(ActionWalk);

            showTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);  //文本变化间隔是??毫秒(并不准确)
            showTimer.Start();
        }

        public void ShowCurTimer(object sender, EventArgs e)//取当前时间并扫描可用串口的委托
        {
            string timeDateString = "";
            DateTime now = DateTime.Now;
            timeDateString = string.Format("{0}年{1}月{2}日 {3}:{4}:{5}",
                now.Year,
                now.Month.ToString("00"),
                now.Day.ToString("00"),
                now.Hour.ToString("00"),
                now.Minute.ToString("00"),
                now.Second.ToString("00"));

            timeDateTextBlock.Text = timeDateString;

            ScanPorts();//扫描可用串口
        }

        public void ScanPorts()//扫描可用串口
        {
            SPCount = methods.CheckSerialPortCount();      //获得计算机可用串口名称数组

            ComboBoxItem tempComboBoxItem = new ComboBoxItem();

            if (comcount != SPCount.Length)            //SPCount.length其实就是可用串口的个数
            {
                //当可用串口计数器与实际可用串口个数不相符时
                //初始化三个下拉窗口并将flag初始化为false

                Motor_comboBox.Items.Clear();
                Press_comboBox.Items.Clear();
                Angle_comboBox.Items.Clear();

                tempComboBoxItem = new ComboBoxItem();
                tempComboBoxItem.Content = "请选择串口";
                Motor_comboBox.Items.Add(tempComboBoxItem);
                Motor_comboBox.SelectedIndex = 0;

                tempComboBoxItem = new ComboBoxItem();
                tempComboBoxItem.Content = "请选择串口";
                Press_comboBox.Items.Add(tempComboBoxItem);
                Press_comboBox.SelectedIndex = 0;

                tempComboBoxItem = new ComboBoxItem();
                tempComboBoxItem.Content = "请选择串口";
                Angle_comboBox.Items.Add(tempComboBoxItem);
                Angle_comboBox.SelectedIndex = 0;

                motor_com = null;
                press_com = null;
                angle_com = null;

                flag = false;

                if (comcount != 0)
                {
                    //在操作过程中增加或减少串口时发生
                    //MessageBox.Show("串口数目已改变，请重新选择串口");
                    statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                    statusInfoTextBlock.Text = "串口数目已改变，请重新选择串口！";
                }

                comcount = SPCount.Length;     //将可用串口计数器与现在可用串口个数匹配
            }

            if (!flag)
            {
                if (SPCount.Length > 0)
                {
                    //有可用串口时执行
                    comcount = SPCount.Length;

                    statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                    statusInfoTextBlock.Text = "检测到" + SPCount.Length + "个串口!";

                    for (int i = 0; i < SPCount.Length; i++)
                    {
                        //分别将可用串口添加到三个下拉窗口中
                        string tempstr = "串口" + SPCount[i];

                        tempComboBoxItem = new ComboBoxItem();
                        tempComboBoxItem.Content = tempstr;
                        Motor_comboBox.Items.Add(tempComboBoxItem);

                        tempComboBoxItem = new ComboBoxItem();
                        tempComboBoxItem.Content = tempstr;
                        Press_comboBox.Items.Add(tempComboBoxItem);

                        tempComboBoxItem = new ComboBoxItem();
                        tempComboBoxItem.Content = tempstr;
                        Angle_comboBox.Items.Add(tempComboBoxItem);
                    }

                    flag = true;

                }
                else
                {
                    comcount = 0;
                    statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                    statusInfoTextBlock.Text = "未检测到串口!";
                }
            }
        }

        public void ShowSenderTimer(object sender, EventArgs e)//电机状态，压力，倾角，角度传感器状态的文本输出
        {
            //电机1的文本框输出
            Motor1_enable_textBox.Text = methods.enable[0].ToString();                       //使能
            Motor1_direction_textBox.Text = methods.direction[0].ToString("F");              //方向；"F"格式，默认保留两位小数
            Motor1_speed_textBox.Text = methods.speed[0].ToString("F");                      //转速：-2590~2590r/min
            Motor1_current_textBox.Text = Math.Abs(methods.current[0]).ToString("F");        //电流
            //电机2的文本框输出
            Motor2_enable_textBox.Text = methods.enable[1].ToString();
            Motor2_direction_textBox.Text = methods.direction[1].ToString("F");
            Motor2_speed_textBox.Text = methods.speed[1].ToString("F");
            Motor2_current_textBox.Text = Math.Abs(methods.current[1]).ToString("F");
            //电机3的文本框输出
            Motor3_enable_textBox.Text = methods.enable[2].ToString();
            Motor3_direction_textBox.Text = methods.direction[2].ToString("F");
            Motor3_speed_textBox.Text = methods.speed[2].ToString("F");
            Motor3_current_textBox.Text = Math.Abs(methods.current[2]).ToString("F");
            //电机4的文本框输出
            Motor4_enable_textBox.Text = methods.enable[3].ToString();
            Motor4_direction_textBox.Text = methods.direction[3].ToString("F");
            Motor4_speed_textBox.Text = methods.speed[3].ToString("F");
            Motor4_current_textBox.Text = Math.Abs(methods.current[3]).ToString("F");

            //8个压力传感器的文本框输出
            Pressure1_Textbox.Text = methods.tempPress[0].ToString();
            Pressure2_Textbox.Text = methods.tempPress[1].ToString();
            Pressure3_Textbox.Text = methods.tempPress[2].ToString();
            Pressure4_Textbox.Text = methods.tempPress[3].ToString();
            Pressure5_Textbox.Text = methods.tempPress[4].ToString();
            Pressure6_Textbox.Text = methods.tempPress[5].ToString();
            Pressure7_Textbox.Text = methods.tempPress[6].ToString();
            Pressure8_Textbox.Text = methods.tempPress[7].ToString();

            //4个倾角传感器各自的x轴和y轴的文本框输出
            //Dip1_x_TextBox.Text = Math.Abs(methods.dirangle[0]).ToString("F");
            //Dip1_y_TextBox.Text = Math.Abs(methods.dirangle[1]).ToString("F");
            //Dip2_x_TextBox.Text = Math.Abs(methods.dirangle[2]).ToString("F");
            //Dip2_y_TextBox.Text = Math.Abs(methods.dirangle[3]).ToString("F");
            //Dip3_x_TextBox.Text = Math.Abs(methods.dirangle[4]).ToString("F");
            //Dip3_y_TextBox.Text = Math.Abs(methods.dirangle[5]).ToString("F");
            //Dip4_x_TextBox.Text = Math.Abs(methods.dirangle[6]).ToString("F");
            //Dip4_y_TextBox.Text = Math.Abs(methods.dirangle[7]).ToString("F");

            Dip1_x_TextBox.Text = methods.dirangle[0].ToString("F");
            Dip1_y_TextBox.Text = methods.dirangle[1].ToString("F");
            Dip2_x_TextBox.Text = methods.dirangle[2].ToString("F");
            Dip2_y_TextBox.Text = methods.dirangle[3].ToString("F");
            Dip3_x_TextBox.Text = methods.dirangle[4].ToString("F");
            Dip3_y_TextBox.Text = methods.dirangle[5].ToString("F");
            Dip4_x_TextBox.Text = methods.dirangle[6].ToString("F");
            Dip4_y_TextBox.Text = methods.dirangle[7].ToString("F");

            //6个角度传感器绝对值的文本框输出
            //Angle1_Textbox.Text = Math.Abs(methods._angle[0]).ToString("F");
            //Angle2_Textbox.Text = Math.Abs(methods._angle[1]).ToString("F");
            //Angle3_Textbox.Text = Math.Abs(methods._angle[2]).ToString("F");
            //Angle4_Textbox.Text = Math.Abs(methods._angle[3]).ToString("F");
            //Angle5_Textbox.Text = Math.Abs(methods._angle[4]).ToString("F");
            //Angle6_Textbox.Text = Math.Abs(methods._angle[5]).ToString("F");

            //6个角度传感器的文本框输出
            Angle1_Textbox.Text = methods._angle[0].ToString("F");
            Angle2_Textbox.Text = methods._angle[1].ToString("F");
            Angle3_Textbox.Text = methods._angle[2].ToString("F");
            Angle4_Textbox.Text = methods._angle[3].ToString("F");
            Angle5_Textbox.Text = methods._angle[4].ToString("F");
            Angle6_Textbox.Text = methods._angle[5].ToString("F");
        }

        #endregion

        #region Button控件
        private void Add_button_Click(object sender, RoutedEventArgs e)//点击【添加命令】按钮时执行
        {
            int add_enable = Convert.ToInt16(MotorControl_enable_textBox.Text);
            int add_direction = Convert.ToInt16(MotorControl_direction_textBox.Text);
            int add_speed = Convert.ToInt16(MotorControl_speed_textBox.Text);
            int add_motorNum = Convert.ToInt16(MotorControl_motorNum_textBox.Text);

            if (add_motorNum != 1 && add_motorNum != 2 && add_motorNum != 3 && add_motorNum != 4)
            {
                //MessageBox.Show("选择电机号请输入1或2或3或4");
                statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                statusInfoTextBlock.Text = "未正确选择电机号！选择电机号请输入1或2或3或4！";
                In_textBox.Text = "未正确选择电机号！选择电机号请输入1或2或3或4！";
            }

            else
            {
                if (add_enable != 0 && add_enable != 1)
                {
                    //MessageBox.Show("选择是否使能请输入0或1");
                    statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                    statusInfoTextBlock.Text = "选择是否使能请输入0或1！";
                    In_textBox.Text = "选择是否使能请输入0或1！";
                }

                else
                {
                    if (add_speed > 3000 || add_speed < 2000)
                    {
                        //MessageBox.Show("输入转速无效，请在范围1800~3000内选择");
                        statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                        statusInfoTextBlock.Text = "输入转速无效，请在范围2000~3000内选择！";
                        In_textBox.Text = "输入转速无效，请在范围1800~3000内选择！";
                    }

                    else
                    {
                        if (add_direction != 0 && add_direction != 1)
                        {
                            //MessageBox.Show("选择电机方向请输入0或1");
                            statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                            statusInfoTextBlock.Text = "选择电机方向请输入0或1！";
                            In_textBox.Text = "选择电机方向请输入0或1！";
                        }

                        else
                        {

                            choosecount++;

                            if (choosecount < 5) /*？？？重新输入重号的电机怎么办，此处注定一次输入只能最多输入4个？？？*/
                                MotorControl_chosenCount_textBox.Text = choosecount.ToString(); //显示已添加电机数

                            byte add_enablebyte = Convert.ToByte(add_enable);           //使能数值命令转字节命令
                            byte add_directionbyte = Convert.ToByte(add_direction);     //方向数值命令转字节命令
                            byte[] add_speedbytes = BitConverter.GetBytes(add_speed);   //转速数值命令转字节命令
                            byte add_motorNumbyte = Convert.ToByte(add_motorNum);       //电机号数值命令转字节命令

                            //cmdSendBytes[0] = 0x23;//开始字符
                            //cmdSendBytes[17] = 0x0D;//结束字符
                            //cmdSendBytes[18] = 0x0A;

                            switch (add_motorNum)
                            {
                                case 1: //电机1添加字节命令
                                    cmdSendBytes[1] = add_enablebyte;
                                    cmdSendBytes[2] = add_directionbyte;
                                    cmdSendBytes[3] = add_speedbytes[1];
                                    cmdSendBytes[4] = add_speedbytes[0];
                                    In_textBox.Text = "添加【" + "电机" + add_motorNum + " 使能" + add_enable + " 方向" + add_direction + " 转速" + add_speed + "】命令";
                                    statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                                    statusInfoTextBlock.Text = "添加【" + "电机" + add_motorNum + " 使能" + add_enable + " 方向" + add_direction + " 转速" + add_speed + "】命令";
                                    break;
                                case 2: //电机2添加字节命令
                                    cmdSendBytes[5] = add_enablebyte;
                                    cmdSendBytes[6] = add_directionbyte;
                                    cmdSendBytes[7] = add_speedbytes[1];
                                    cmdSendBytes[8] = add_speedbytes[0];
                                    In_textBox.Text = "添加【" + "电机" + add_motorNum + " 使能" + add_enable + " 方向" + add_direction + " 转速" + add_speed + "】命令";
                                    statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                                    statusInfoTextBlock.Text = "添加【" + "电机" + add_motorNum + " 使能" + add_enable + " 方向" + add_direction + " 转速" + add_speed + "】命令";
                                    break;
                                case 3: //电机3添加字节命令
                                    cmdSendBytes[9] = add_enablebyte;
                                    cmdSendBytes[10] = add_directionbyte;
                                    cmdSendBytes[11] = add_speedbytes[1];
                                    cmdSendBytes[12] = add_speedbytes[0];
                                    In_textBox.Text = "添加【" + "电机" + add_motorNum + " 使能" + add_enable + " 方向" + add_direction + " 转速" + add_speed + "】命令";
                                    statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                                    statusInfoTextBlock.Text = "添加【" + "电机" + add_motorNum + " 使能" + add_enable + " 方向" + add_direction + " 转速" + add_speed + "】命令";
                                    break;
                                case 4: //电机4添加字节命令
                                    cmdSendBytes[13] = add_enablebyte;
                                    cmdSendBytes[14] = add_directionbyte;
                                    cmdSendBytes[15] = add_speedbytes[1];
                                    cmdSendBytes[16] = add_speedbytes[0];
                                    In_textBox.Text = "添加【" + "电机" + add_motorNum + " 使能" + add_enable + " 方向" + add_direction + " 转速" + add_speed + "】命令";
                                    statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                                    statusInfoTextBlock.Text = "添加【" + "电机" + add_motorNum + " 使能" + add_enable + " 方向" + add_direction + " 转速" + add_speed + "】命令";
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void MotorStop_button_Click(object sender, RoutedEventArgs e)//点击【电机停止】按钮时执行
        {
            byte[] clearBytes = new byte[19] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            try
            {
                methods.SendControlCMD(clearBytes);
                statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                statusInfoTextBlock.Text = "电机已停止！请将电机复位！";
            }
            catch
            {
                //MessageBox.Show("未正确选择电机串口!");
                statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                statusInfoTextBlock.Text = "未正确选择电机串口！";
            }

        }

        private void Send_button_Click(object sender, RoutedEventArgs e)//点击【发送命令】按钮时执行
        {
            if (cmdSendBytes != null)
            {
                Send_button.Content = "正在发送...";                    //改变发送命令按钮的文本为“正在发送...”，表示正在给电机串口写入字节命令

                try
                {
                    methods.SendControlCMD(cmdSendBytes);                           //给电机串口写入字节命令
                    statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                    statusInfoTextBlock.Text = "已发送至电机";
                }
                catch
                {
                    //MessageBox.Show("未正确选择电机串口!");
                    statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                    statusInfoTextBlock.Text = "未正确选择电机串口！";
                }

                Send_button.Content = "发送命令";                       //命令发送完毕后按钮文本变回来
                cmdSendBytes = new byte[19] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//重置字节命令存储器cmdSendBytes
                In_textBox.Text = "";                                   //清空输入信息窗口
                choosecount = 0;                                        //重置已添加命令电机个数
            }
            else
            {
                //MessageBox.Show("未给电机添加参数");
                statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                statusInfoTextBlock.Text = "未给电机添加参数";
            }
        }

        private void AngleInitializeButton_Click(object sender, RoutedEventArgs e)//点击【角度初始化】按钮时执行
        {
            statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
            statusInfoTextBlock.Text = "角度初始化完成! 可以按下【动作开始】按钮";
            methods.AngleInit();

            IsTrueForefootZero = true;

            AngleInit_button.IsEnabled = false;
            ActionStart_button.IsEnabled = true;
            //ActionPick_button.IsEnabled = false;
            //ActionWalk_button.IsEnabled = false;
        }

        private void ActionStartButton_Click(object sender, RoutedEventArgs e)//点击【动作开始】按钮时执行
        {
            //需要获取角度、倾角、足底压力信息，并实现电机操控
            //时间触发 为真

            statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
            if (isActionPick)
                statusInfoTextBlock.Text = "【拾取重物】动作已开始!";
            if (isActionWalk)
                statusInfoTextBlock.Text = "【行走】动作已开始!";

            ActionStart_button.Content = "已开始";
            ActionStop_button.Content = "动作停止";
            IsTrueClickDown = true;
            ActionStart_button.Background = Brushes.DarkSalmon;
            ActionStop_button.Background = Brushes.White;

            AngleInit_button.IsEnabled = false;
            ActionStart_button.IsEnabled = false;
            ActionStop_button.IsEnabled = true;

            //if (isActionWalk)
            //{
            //    ActionWalkDo_button.IsEnabled = true;
            //    ActionWalkEnd_button.IsEnabled = true;
            //}
            //else
            //{
            //    ActionWalkDo_button.IsEnabled = false;
            //    ActionWalkEnd_button.IsEnabled = false;
            //}
        }

        private void ActionStopButton_Click(object sender, RoutedEventArgs e)//点击【动作停止】按钮时执行
        {
            statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
            statusInfoTextBlock.Text = "动作已停止!";

            ActionStart_button.Content = "动作开始";
            ActionStop_button.Content = "已停止";
            ActionStart_button.Background = Brushes.White;
            IsTrueClickDown = false;
            ActionStop_button.Background = Brushes.DarkSalmon;

            methods._angleInitialization = new double[6] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

            AngleInit_button.IsEnabled = true;
            ActionStart_button.IsEnabled = false;
            ActionStop_button.IsEnabled = false;
            //ActionPick_button.IsEnabled = true;
            //ActionWalk_button.IsEnabled = true;
            //ActionWalkDo_button.IsEnabled = false;
            //ActionWalkEnd_button.IsEnabled = false;

            //参数重置避免重复进行动作时出错
            IsTrueStanduping = false;
            IsTrueSquatting = false;
            istrueX = false;
            istrueY = false;
            isActionPick = false;
            isActionWalk = false;
            //DSt_left = false;         //进入左腿摆动相的过渡相
            //DSt_right = false;        //进入右腿摆动相的过渡相
            //init_LSW = false;         //左腿摆动相前期
            //mid_LSW = false;          //左腿摆动相中期
            //term_LSW = false;         //左腿摆动相后期
            //init_RSW = false;         //右腿摆动相前期
            //mid_RSW = false;          //右腿摆动相中期
            //term_RSW = false;         //右腿摆动相后期
            mid_flag = false;
            //do_walk = false;

            byte[] clearBytes = new byte[19] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            try
            {
                methods.SendControlCMD(clearBytes);
                statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                statusInfoTextBlock.Text = "电机已停止！请将电机复位！";
            }
            catch
            {
                //MessageBox.Show("未正确选择电机串口!");
                statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                statusInfoTextBlock.Text = "未正确选择电机串口！";
            }
        }

        //private void ActionWalkButtonDo_Click(object sender, RoutedEventArgs e)//点击【行走执行】按钮时执行
        //{
        //    do_walk = true;
        //    stop_walk = true;
        //    ActionStop_button.IsEnabled = false;
        //}

        //private void ActionWalkButtonEnd_Click(object sender, RoutedEventArgs e)//点击【行走结束】按钮时执行
        //{
        //    do_walk = false;
        //    stop_walk = false;
        //    ActionStop_button.IsEnabled = true;
        //}

        private void SetSpeedButton_Click(object sender, RoutedEventArgs e)//点击【确认】按钮时执行
        {
            int rSpeed1_test = Convert.ToInt16(MotorSpeed_textBox.Text);

            if (rSpeed1_test < 2000 || rSpeed1_test > 6000)
            {
                statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                statusInfoTextBlock.Text = "输入转速无效，请在范围2000~6000内选择！";
                In_textBox.Text = "输入转速无效，请在范围2000~6000内选择！";
            }
            else
            {
                rSpeed1 = Convert.ToInt16(MotorSpeed_textBox.Text);
                statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                statusInfoTextBlock.Text = "已设置电机转速为" + rSpeed1;
            }
        }
        #endregion

        public void ActionPick(object sender, EventArgs e)//【拾取重物】动作
        {
            //检测开始按键是否按下
            if (IsTrueClickDown && isActionWalk == false)
            {
                //初始化已经完成，开始检测状态
                if (IsTrueForefootZero)
                {

                    byte[] rSpeedBytes1 = BitConverter.GetBytes(rSpeed1);
                    byte[] rSpeedBytes2 = BitConverter.GetBytes(rSpeed2);

                    //下蹲..检测后背倾角变化 腿角度是否为站立  是否起立中
                    if (!IsTrueStanduping)
                    {
                        //正在下蹲中
                        IsTrueSquatting = true;
                        //如果腿部角度未达到阈值
                        if (methods.dirangle[7] < 65 && (Math.Abs(methods._angle[2]) < 7 || Math.Abs(methods._angle[3]) < 7))
                        {
                            istrueX = true;
                            isActionPick = true;
                        }
                        if (istrueX && (Math.Abs(methods._angle[2]) < 65 || Math.Abs(methods._angle[3]) < 55))
                        {
                            status = 1;

                            //左腿从0°向-60°弯曲
                            if (methods._angle[2] > -65)
                            {
                                cmdSendBytes[5] = 1;
                                cmdSendBytes[6] = 1;
                                cmdSendBytes[7] = rSpeedBytes1[1];
                                cmdSendBytes[8] = rSpeedBytes1[0];
                                if (methods._angle[2] < -55)
                                {
                                    cmdSendBytes[7] = rSpeedBytes2[1];
                                    cmdSendBytes[8] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[5] = 0;
                            }

                            //右腿从0°向50°弯曲
                            if (methods._angle[3] < 55)
                            {
                                cmdSendBytes[9] = 1;
                                cmdSendBytes[10] = 0;
                                cmdSendBytes[11] = rSpeedBytes1[1];
                                cmdSendBytes[12] = rSpeedBytes1[0];
                                if (methods._angle[3] > 45)
                                {
                                    cmdSendBytes[11] = rSpeedBytes2[1];
                                    cmdSendBytes[12] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[9] = 0;
                            }


                            //左手从60°向10°伸直
                            if (methods._angle[0] > 10)
                            {
                                cmdSendBytes[1] = 1;
                                cmdSendBytes[2] = 1;
                                cmdSendBytes[3] = rSpeedBytes1[1];
                                cmdSendBytes[4] = rSpeedBytes1[0];

                                if (methods._angle[0] < 40)
                                {
                                    cmdSendBytes[3] = rSpeedBytes2[1];
                                    cmdSendBytes[4] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[1] = 0;
                            }

                            //右手从-60°向-20°伸直
                            if (methods._angle[1] < -20)
                            {
                                cmdSendBytes[13] = 1;
                                cmdSendBytes[14] = 0;
                                cmdSendBytes[15] = rSpeedBytes1[1];
                                cmdSendBytes[16] = rSpeedBytes1[0];

                                if (methods._angle[1] > -40)
                                {
                                    cmdSendBytes[15] = rSpeedBytes2[1];
                                    cmdSendBytes[16] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[13] = 0;
                            }
                        }

                        else
                        {
                            cmdSendBytes[5] = 0;
                            cmdSendBytes[9] = 0;

                            if (methods._angle[0] < 10 && methods._angle[1] > -20)
                            {
                                status = 2;
                                cmdSendBytes[1] = 0;
                                cmdSendBytes[13] = 0;

                                IsTrueSquatting = false;
                                istrueX = false;
                            }
                        }

                        try
                        {
                            methods.SendControlCMD(cmdSendBytes);
                        }
                        catch
                        {
                            //MessageBox.Show("未正确选择电机串口!");
                            statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                            statusInfoTextBlock.Text = "未正确选择电机串口！请选择正确电机串口后重新按下【动作开始】按钮";

                            IsTrueClickDown = false;
                            ActionStart_button.Content = "动作开始";
                            ActionStart_button.IsEnabled = true;
                            ActionStop_button.IsEnabled = false;
                        }
                    }

                    //起立 腿部大于阈值 脚底压力和变化 是否在下蹲中
                    if (!IsTrueSquatting)
                    {
                        //起立中
                        IsTrueStanduping = true;

                        if (methods.tempPress[7] > 500)
                        {
                            istrueY = true;
                        }
                        if (istrueY)
                        {
                            status = 3;
                            //左腿从-60°向-5°伸直
                            if (methods._angle[2] < -5)
                            {
                                cmdSendBytes[5] = 1;
                                cmdSendBytes[6] = 0;
                                cmdSendBytes[7] = rSpeedBytes1[1];
                                cmdSendBytes[8] = rSpeedBytes1[0];
                                if (methods._angle[2] > -10)
                                {
                                    cmdSendBytes[7] = rSpeedBytes2[1];
                                    cmdSendBytes[8] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[5] = 0;
                            }
                            //右腿从60°向5°伸直
                            if (methods._angle[3] > 5)
                            {
                                cmdSendBytes[9] = 1;
                                cmdSendBytes[10] = 1;
                                cmdSendBytes[11] = rSpeedBytes1[1];
                                cmdSendBytes[12] = rSpeedBytes1[0];
                                if (methods._angle[3] < 10)
                                {
                                    cmdSendBytes[11] = rSpeedBytes2[1];
                                    cmdSendBytes[12] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[9] = 0;
                            }

                            if (methods.tempPress[7] > 500)
                            {
                                status = 4;
                                //左手由5°向60°弯曲
                                if (methods._angle[0] < 60)
                                {
                                    cmdSendBytes[1] = 1;
                                    cmdSendBytes[2] = 0;
                                    cmdSendBytes[3] = rSpeedBytes1[1];
                                    cmdSendBytes[4] = rSpeedBytes1[0];
                                    if (methods._angle[0] > 50)
                                    {
                                        cmdSendBytes[3] = rSpeedBytes2[1];
                                        cmdSendBytes[4] = rSpeedBytes2[0];
                                    }
                                }
                                else
                                {
                                    cmdSendBytes[1] = 0;
                                }

                                //右手由-5°向-60°弯曲
                                if (methods._angle[1] > -60)
                                {
                                    cmdSendBytes[13] = 1;
                                    cmdSendBytes[14] = 1;
                                    cmdSendBytes[15] = rSpeedBytes1[1];
                                    cmdSendBytes[16] = rSpeedBytes1[0];
                                    if (methods._angle[1] < -50)
                                    {
                                        cmdSendBytes[15] = rSpeedBytes2[1];
                                        cmdSendBytes[16] = rSpeedBytes2[0];
                                    }
                                }
                                else
                                {
                                    cmdSendBytes[13] = 0;
                                }
                            }
                            else
                            {
                                cmdSendBytes[1] = 0;
                                cmdSendBytes[13] = 0;
                            }
                        }

                        if (Math.Abs(methods._angle[2]) < 5 && Math.Abs(methods._angle[3]) < 5)
                        {
                            cmdSendBytes[5] = 0;
                            cmdSendBytes[9] = 0;

                            if (istrueY && methods.tempPress[7] > 500)
                            {
                                status = 5;
                                if (Math.Abs(methods._angle[0]) > 60 && Math.Abs(methods._angle[1]) > 60)
                                {
                                    cmdSendBytes[1] = 0;
                                    cmdSendBytes[13] = 0;

                                    IsTrueStanduping = false;
                                    istrueY = false;
                                }
                            }
                            else
                            {
                                status = 6;
                                IsTrueStanduping = false;
                                istrueY = false;
                            }
                        }

                        try
                        {
                            methods.SendControlCMD(cmdSendBytes);
                        }
                        catch
                        {
                            //MessageBox.Show("未正确选择电机串口!");
                            statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 230, 20, 20));
                            statusInfoTextBlock.Text = "未正确选择电机串口！请选择正确电机串口后重新按下【动作开始】按钮";

                            IsTrueClickDown = false;
                            ActionStart_button.Content = "动作开始";
                            ActionStart_button.IsEnabled = true;
                            ActionStop_button.IsEnabled = false;
                        }
                    }

                    switch (status)//动作测试监控，方便了解哪个阶段出现问题
                    {

                        case 0:
                            break;

                        case 1:
                            {
                                Out_textBox.Text = "正在下蹲";
                                break;
                            }

                        case 2:
                            {
                                Out_textBox.Text = "下蹲动作完成";
                                break;
                            }

                        case 3:
                            {
                                Out_textBox.Text = "不抬起重物起立中";
                                break;
                            }

                        case 4:
                            {
                                Out_textBox.Text = "抬起重物起立中";
                                break;
                            }

                        case 5:
                            {
                                Out_textBox.Text = "抬起重物起立动作完成";
                                break;
                            }

                        case 6:
                            {
                                Out_textBox.Text = "不抬起重物起立动作完成";
                                break;
                            }
                    }
                }
            }
        }

        public void ActionWalk(object sender, EventArgs e)//【行走】动作
        {
            //检测【动作开始】按扭是否按下
            if (IsTrueClickDown && isActionPick == false)
            {
                //检测【角度初始化】按扭是否按下
                if (IsTrueForefootZero)
                {

                    isActionWalk = true;

                    byte[] rSpeedBytes1 = BitConverter.GetBytes(rSpeed1);
                    byte[] rSpeedBytes2 = BitConverter.GetBytes(rSpeed2);

                    //tempPress[0]左腿足跟
                    //tempPress[1]左腿足侧
                    //tempPress[2]左腿足尖
                    //tempPress[3]右腿足跟
                    //tempPress[4]右腿足侧
                    //tempPress[5]右腿足尖

                    //双脚站立支撑时电机停机
                    if ((methods.tempPress[0] > 150 || methods.tempPress[1] > 150 || methods.tempPress[2] > 150) && (methods.tempPress[3] > 150 || methods.tempPress[4] > 150 || methods.tempPress[5] > 150))
                    {
                        status = 1;

                        mid_flag = false;

                        cmdSendBytes[5] = 0;
                        cmdSendBytes[6] = 0;
                        cmdSendBytes[7] = 0;
                        cmdSendBytes[8] = 0;
                        cmdSendBytes[9] = 0;
                        cmdSendBytes[10] = 0;
                        cmdSendBytes[11] = 0;
                        cmdSendBytes[12] = 0;
                    }

                    //左腿离地右腿支撑：迈左腿，左腿足跟先离地
                    if (methods.tempPress[0] < 100 && (methods.tempPress[3] > 300 || methods.tempPress[4] > 300 || methods.tempPress[5] > 300))
                    {
                        status = 2;

                        cmdSendBytes[9] = 0;

                        if (mid_flag == false && methods._angle[2] > -90)//左腿0前1后从0°向-25°弯曲
                        {
                            cmdSendBytes[5] = 1;
                            cmdSendBytes[6] = 1;
                            cmdSendBytes[7] = rSpeedBytes1[1];
                            cmdSendBytes[8] = rSpeedBytes1[0];

                            if (methods._angle[2] < -20)
                            {
                                cmdSendBytes[7] = rSpeedBytes2[1];
                                cmdSendBytes[8] = rSpeedBytes2[0];
                            }

                            if (methods._angle[2] < -25)
                                mid_flag = true;
                        }
                        else
                        {
                            if (mid_flag == true && methods._angle[2] < -5)//左腿0前1后从-25°到-5°伸直
                            {
                                cmdSendBytes[5] = 1;
                                cmdSendBytes[6] = 0;
                                cmdSendBytes[7] = rSpeedBytes1[1];
                                cmdSendBytes[8] = rSpeedBytes1[0];

                                if (methods._angle[2] > -10)
                                {
                                    cmdSendBytes[7] = rSpeedBytes2[1];
                                    cmdSendBytes[8] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[5] = 0;
                            }
                        }
                    }

                    //右腿离地左腿支撑
                    if (methods.tempPress[3] < 100 && (methods.tempPress[0] > 300 || methods.tempPress[1] > 300 || methods.tempPress[2] > 300))
                    {
                        status = 3;

                        cmdSendBytes[5] = 0;

                        if (mid_flag == false && methods._angle[3] < 90)//右腿0后1前从0°向30°弯曲
                        {
                            cmdSendBytes[9] = 1;
                            cmdSendBytes[10] = 0;
                            cmdSendBytes[11] = rSpeedBytes1[1];
                            cmdSendBytes[12] = rSpeedBytes1[0];

                            if (methods._angle[3] > 25)
                            {
                                cmdSendBytes[11] = rSpeedBytes2[1];
                                cmdSendBytes[12] = rSpeedBytes2[0];
                            }

                            if (methods._angle[3] > 30)
                                mid_flag = true;
                        }
                        else
                        {
                            if (mid_flag == true && methods._angle[3] > 10)//右腿0后1前从30°到10°伸直
                            {
                                cmdSendBytes[9] = 1;
                                cmdSendBytes[10] = 1;
                                cmdSendBytes[11] = rSpeedBytes1[1];
                                cmdSendBytes[12] = rSpeedBytes1[0];

                                if (Math.Abs(methods._angle[3]) < 15)
                                {
                                    cmdSendBytes[11] = rSpeedBytes2[1];
                                    cmdSendBytes[12] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[9] = 0;
                            }
                        }
                    }

                    ////测试用状态
                    //if (methods.tempPress[0] < 100 && methods.tempPress[3] < 100)
                    //{
                    //    status = 4;

                    //    mid_flag = false;

                    //    cmdSendBytes[5] = 0;
                    //    cmdSendBytes[6] = 0;
                    //    cmdSendBytes[7] = 0;
                    //    cmdSendBytes[8] = 0;
                    //    cmdSendBytes[9] = 0;
                    //    cmdSendBytes[10] = 0;
                    //    cmdSendBytes[11] = 0;
                    //    cmdSendBytes[12] = 0;
                    //}

                    //保护状态
                    if ((methods.tempPress[0] < 100 && methods.tempPress[1] < 100 && methods.tempPress[2] < 100) && (methods.tempPress[3] < 100 && methods.tempPress[4] < 100 && methods.tempPress[5] < 100))
                    {
                        status = 4;

                        mid_flag = false;

                        cmdSendBytes[5] = 0;
                        cmdSendBytes[6] = 0;
                        cmdSendBytes[7] = 0;
                        cmdSendBytes[8] = 0;
                        cmdSendBytes[9] = 0;
                        cmdSendBytes[10] = 0;
                        cmdSendBytes[11] = 0;
                        cmdSendBytes[12] = 0;
                    }

                    //切换为拾取重物动作
                    if (methods.tempPress[7] > 1000)
                    {
                        isActionWalk = false;
                        isActionPick = true;

                        status = 5;

                        cmdSendBytes[5] = 0;
                        cmdSendBytes[6] = 0;
                        cmdSendBytes[7] = 0;
                        cmdSendBytes[8] = 0;
                        cmdSendBytes[9] = 0;
                        cmdSendBytes[10] = 0;
                        cmdSendBytes[11] = 0;
                        cmdSendBytes[12] = 0;
                    }

                    try
                    {
                        methods.SendControlCMD(cmdSendBytes);
                    }
                    catch
                    {
                        //MessageBox.Show("未正确选择电机串口!");
                        statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 150, 50, 50));
                        statusInfoTextBlock.Text = "未正确选择电机串口！请选择正确电机串口后重新按下【动作开始】按钮";

                        IsTrueClickDown = false;
                        ActionStart_button.Content = "动作开始";
                        ActionStart_button.IsEnabled = true;
                        ActionStop_button.IsEnabled = false;
                    }

                    switch (status)//动作测试监控，方便了解哪个阶段出现问题
                    {

                        case 0:
                            break;

                        case 1:
                            {
                                Out_textBox.Text = "双腿站立";
                                break;
                            }

                        case 2:
                            {
                                Out_textBox.Text = "迈左腿";
                                break;
                            }

                        case 3:
                            {
                                Out_textBox.Text = "迈右腿";
                                break;
                            }

                        case 4:
                            {
                                Out_textBox.Text = "保护状态";
                                break;
                            }

                        case 5:
                            {
                                Out_textBox.Text = "切换为拾取重物动作";
                                break;
                            }
                    }
                }

                else
                {
                    cmdSendBytes[5] = 0;
                    cmdSendBytes[6] = 0;
                    cmdSendBytes[7] = 0;
                    cmdSendBytes[8] = 0;
                    cmdSendBytes[9] = 0;
                    cmdSendBytes[10] = 0;
                    cmdSendBytes[11] = 0;
                    cmdSendBytes[12] = 0;

                    try
                    {
                        methods.SendControlCMD(cmdSendBytes);
                    }
                    catch
                    {
                        //MessageBox.Show("未正确选择电机串口!");
                        statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 150, 50, 50));
                        statusInfoTextBlock.Text = "未正确选择电机串口！请选择正确电机串口后重新按下【动作开始】按钮";

                        IsTrueClickDown = false;
                        ActionStart_button.Content = "动作开始";
                        ActionStart_button.IsEnabled = true;
                        ActionStop_button.IsEnabled = false;
                    }
                }
            }
        }
    }
}

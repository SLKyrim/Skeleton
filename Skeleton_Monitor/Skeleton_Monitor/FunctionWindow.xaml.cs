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
                        MessageBox.Show("串口" + SPCount[i] + "已被占用!");
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
                        MessageBox.Show("串口" + SPCount[i] + "已被占用!");
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
                        MessageBox.Show("串口" + SPCount[i] + "已被占用!");
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
            showTimer.Tick += new EventHandler(Action);          
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
                    MessageBox.Show("串口数目已改变，请重新选择串口");
                }

                comcount = SPCount.Length;     //将可用串口计数器与现在可用串口个数匹配
            }

            if (!flag)
            {
                if (SPCount.Length > 0)
                {
                    //有可用串口时执行
                    comcount = SPCount.Length;

                    Out_textBox.Text = "检测到" + SPCount.Length + "个串口!";

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
                    Out_textBox.Text = "未检测到串口!";
                }
            }
        }

        public void ShowSenderTimer(object sender, EventArgs e)//电机状态，压力，倾角，角度传感器状态的文本输出
        {
            //电机1的文本框输出
            Motor1_enable_textBox.Text = methods.enable[0].ToString();             //使能
            Motor1_direction_textBox.Text = methods.direction[0].ToString("F");    //方向；"F"格式，默认保留两位小数
            Motor1_speed_textBox.Text = methods.speed[0].ToString("F");            //转速
            Motor1_current_textBox.Text = methods.current[0].ToString("F");        //电流
            //电机2的文本框输出
            Motor2_enable_textBox.Text = methods.enable[1].ToString();
            Motor2_direction_textBox.Text = methods.direction[1].ToString("F");
            Motor2_speed_textBox.Text = methods.speed[1].ToString("F");
            Motor2_current_textBox.Text = methods.current[1].ToString("F");
            //电机3的文本框输出
            Motor3_enable_textBox.Text = methods.enable[2].ToString();
            Motor3_direction_textBox.Text = methods.direction[2].ToString("F");
            Motor3_speed_textBox.Text = methods.speed[2].ToString("F");
            Motor3_current_textBox.Text = methods.current[2].ToString("F");
            //电机4的文本框输出
            Motor4_enable_textBox.Text = methods.enable[3].ToString();
            Motor4_direction_textBox.Text = methods.direction[3].ToString("F");
            Motor4_speed_textBox.Text = methods.speed[3].ToString("F");
            Motor4_current_textBox.Text = methods.current[3].ToString("F");

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
            Dip1_x_TextBox.Text = methods.dirangle[0].ToString("F");
            Dip1_y_TextBox.Text = methods.dirangle[1].ToString("F");
            Dip2_x_TextBox.Text = methods.dirangle[2].ToString("F");
            Dip2_y_TextBox.Text = methods.dirangle[3].ToString("F");
            Dip3_x_TextBox.Text = methods.dirangle[4].ToString("F");
            Dip3_y_TextBox.Text = methods.dirangle[5].ToString("F");
            Dip4_x_TextBox.Text = methods.dirangle[6].ToString("F");
            Dip4_y_TextBox.Text = methods.dirangle[7].ToString("F");

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
                MessageBox.Show("选择电机号请输入1或2或3或4");
            else
            {
                if (add_enable != 0 && add_enable != 1)
                    MessageBox.Show("选择是否使能请输入0或1");
                else
                {
                    if (add_speed > 3000 || add_speed < 1800)
                        MessageBox.Show("输入转速无效，请在范围1800~3000内选择");
                    else
                    {
                        if (add_direction != 0 && add_direction != 1)
                            MessageBox.Show("选择电机方向请输入0或1");
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
                                    In_textBox.Text = "电机" + add_motorNum + "使能" + add_enable + "转速" + add_speed;
                                    break;
                                case 2: //电机2添加字节命令
                                    cmdSendBytes[5] = add_enablebyte;
                                    cmdSendBytes[6] = add_directionbyte;
                                    cmdSendBytes[7] = add_speedbytes[1];
                                    cmdSendBytes[8] = add_speedbytes[0];
                                    In_textBox.Text = "电机" + add_motorNum + "使能" + add_enable + "转速" + add_speed;
                                    break;
                                case 3: //电机3添加字节命令
                                    cmdSendBytes[9] = add_enablebyte;
                                    cmdSendBytes[10] = add_directionbyte;
                                    cmdSendBytes[11] = add_speedbytes[1];
                                    cmdSendBytes[12] = add_speedbytes[0];
                                    In_textBox.Text = "电机" + add_motorNum + "使能" + add_enable + "转速" + add_speed;
                                    break;
                                case 4: //电机4添加字节命令
                                    cmdSendBytes[13] = add_enablebyte;
                                    cmdSendBytes[14] = add_directionbyte;
                                    cmdSendBytes[15] = add_speedbytes[1];
                                    cmdSendBytes[16] = add_speedbytes[0];
                                    In_textBox.Text = "电机" + add_motorNum + "使能" + add_enable + "转速" + add_speed;
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
                Out_textBox.Text = "电机已停止";
            }
            catch
            {
                MessageBox.Show("未正确选择电机串口!");
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
                }
                catch
                {
                    MessageBox.Show("未正确选择电机串口!");
                }

                Send_button.Content = "发送命令";                       //命令发送完毕后按钮文本变回来
                Out_textBox.Text = "已发送至电机";                      //【输出信息窗口】显示“已发送至电机”
                cmdSendBytes = new byte[19] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//重置字节命令存储器cmdSendBytes
                In_textBox.Text = "";                                   //清空输入信息窗口
                choosecount = 0;                                        //重置已添加命令电机个数
            }
            else
            {
                MessageBox.Show("未给电机添加参数");
            }
        }

        private void AngleInitializeButton_Click(object sender, RoutedEventArgs e)//点击【角度初始化】按钮时执行
        {
            statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 233, 150, 122));
            statusInfoTextBlock.Text = "角度初始化完成!";
            Out_textBox.Text = "角度初始化完成!";
            methods.AngleInit();
            IsTrueForefootZero = true;
            AngleInit_button.IsEnabled = false;
            ActionStart_button.IsEnabled = true;
        }

        private void ActionStartButton_Click(object sender, RoutedEventArgs e)//点击【动作开始】按钮时执行
        {
            //需要获取角度、倾角、足底压力信息，并实现电机操控
            //时间触发 为真

            statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 233, 150, 122));
            statusInfoTextBlock.Text = "动作已开始!";
            Out_textBox.Text = "动作已开始!";
            ActionStart_button.Content = "已开始";
            IsTrueClickDown = true;
            ActionStart_button.Background = Brushes.DarkSalmon;
            ActionStop_button.Background = Brushes.White;

            AngleInit_button.IsEnabled = false;
            ActionStart_button.IsEnabled = false;
            ActionStop_button.IsEnabled = true;
        }

        private void ActionStopButton_Click(object sender, RoutedEventArgs e)//点击【动作停止】按钮时执行
        {
            statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 233, 150, 122));
            statusInfoTextBlock.Text = "动作已停止!";
            Out_textBox.Text = "动作已停止!";
            ActionStart_button.Background = Brushes.White;
            IsTrueClickDown = false;
            ActionStop_button.Background = Brushes.DarkSalmon;

            AngleInit_button.IsEnabled = true;
            ActionStart_button.IsEnabled = true;
            ActionStop_button.IsEnabled = false;
        }
        #endregion

        public void Action(object sender, EventArgs e)//雪松版动作
        {
            //动作流程说明：背往前倾准备下蹲 --> 双腿弯曲下蹲 --> 长按压力传感器8(模拟拿重物），双腿伸直起立同时双手弯曲 -->
            //              背往前倾准备下蹲 --> 双腿弯曲下蹲同时双手伸直（放下重物） --> 按一下压力传感器8，双腿伸直起立

            

            //检测【动作开始】按扭是否按下
            if (IsTrueClickDown)
            {
                //检测【角度初始化】按扭是否按下
                if (IsTrueForefootZero)
                {
                    //下蹲动作执行..条件：检测后背倾角变化；非站立；非起立中
                    if (!IsTrueStanduping)
                    {

                        IsTrueSquatting = true; //正在下蹲中

                        //如果腿部角度未达到阈值
                        if (methods.dirangle[7] < 75 && (Math.Abs(methods._angle[2]) < 5 || Math.Abs(methods._angle[3]) < 5))//后背陀螺仪y轴小于75°(初始是90°摆放)；左腿和右腿角度传感器小于5°（说明角度初始化完成）时执行
                        {
                            istrueX = true;
                        }
                        if (istrueX && (Math.Abs(methods._angle[2]) < 60 || Math.Abs(methods._angle[3]) < 60))//初始化完成且左腿和右腿没转到60°时执行
                        {
                            int rSpeed1 = 3000;//动作开始电机转速
                            int rSpeed2 = 2500;//动作结束前降速缓冲


                            byte[] rSpeedBytes1 = BitConverter.GetBytes(rSpeed1);
                            byte[] rSpeedBytes2 = BitConverter.GetBytes(rSpeed2);
                            //腿部电机控制

                            if (Math.Abs(methods._angle[2]) < 60)//左腿电机2控制
                            {
                                cmdSendBytes[5] = 1;//电机2使能
                                cmdSendBytes[6] = 1;//电机2方向
                                cmdSendBytes[7] = rSpeedBytes1[1];//电机2转速高位
                                cmdSendBytes[8] = rSpeedBytes1[0];//电机2转速地位 (范围1800-16200）对应速度范围（0-2590r/min）
                                if (Math.Abs(methods._angle[2]) > 50)//左腿电机快转到位时电机降速缓冲
                                {
                                    cmdSendBytes[7] = rSpeedBytes2[1];
                                    cmdSendBytes[8] = rSpeedBytes2[0];
                                }
                            }
                            else//左腿已转到位，电机2停止
                            {
                                cmdSendBytes[5] = 0;
                                cmdSendBytes[6] = 0;
                                cmdSendBytes[7] = 0;
                                cmdSendBytes[8] = 0;
                            }
                            if (Math.Abs(methods._angle[3]) < 60)//右腿电机3控制
                            {
                                cmdSendBytes[9] = 1; //电机3使能
                                cmdSendBytes[10] = 0;//电机3方向
                                cmdSendBytes[11] = rSpeedBytes1[1];//电机3转速高位
                                cmdSendBytes[12] = rSpeedBytes1[0];//电机3转速地位 (范围1800-16200）对应速度范围（0-2590r/min）
                                if (methods._angle[3] > 50)//右腿电机快转到位时电机降速至3000
                                {
                                    cmdSendBytes[11] = rSpeedBytes2[1];
                                    cmdSendBytes[12] = rSpeedBytes2[0];
                                }
                            }
                            else//右腿已转到位，电机3停止
                            {
                                cmdSendBytes[9] = 0;
                                cmdSendBytes[10] = 0;
                                cmdSendBytes[11] = 0;
                                cmdSendBytes[12] = 0;
                            }


                            //肘部角度值检测以 确定手部运动
                            //角度大于阈值（40度） 模拟放下重物动作（即伸直） 小于阈值 无动作
                            if (methods._angle[0] > 40)//左手弯曲时执行
                            {
                                cmdSendBytes[1] = 1;
                                cmdSendBytes[2] = 1;
                                cmdSendBytes[3] = rSpeedBytes1[1];
                                cmdSendBytes[4] = rSpeedBytes1[0];

                            }
                            else
                            {
                                if (methods._angle[0] > 5)
                                {
                                    cmdSendBytes[1] = 1;
                                    cmdSendBytes[2] = 1;
                                    cmdSendBytes[3] = rSpeedBytes2[1];
                                    cmdSendBytes[4] = rSpeedBytes2[0];
                                }
                                else//肘部关节小于5°时即停止电机
                                {
                                    //肘部电机停止 
                                    cmdSendBytes[1] = 0;

                                }

                            }
                            if (methods._angle[1] < -40)
                            {
                                cmdSendBytes[13] = 1;
                                cmdSendBytes[14] = 0;
                                cmdSendBytes[15] = rSpeedBytes1[1];
                                cmdSendBytes[16] = rSpeedBytes1[0];
                            }
                            else
                            {
                                if (methods._angle[1] < -5)
                                {

                                    cmdSendBytes[13] = 1;
                                    cmdSendBytes[14] = 0;
                                    cmdSendBytes[15] = rSpeedBytes2[1];
                                    cmdSendBytes[16] = rSpeedBytes2[0];
                                }
                                else
                                {
                                    //肘部电机停止 
                                    cmdSendBytes[13] = 0;
                                }

                            }

                        }
                        else//腿部角度传感器已弯曲大于60°或角度传感器未初始化时执行
                        {
                            //双腿电机不工作
                            cmdSendBytes[5] = 0;
                            cmdSendBytes[6] = 0;
                            cmdSendBytes[7] = 0;
                            cmdSendBytes[8] = 0;
                            cmdSendBytes[9] = 0;
                            cmdSendBytes[10] = 0;
                            cmdSendBytes[11] = 0;
                            cmdSendBytes[12] = 0;
                            //肘部小于阈值
                            if (methods._angle[0] < 5 && methods._angle[1] > -5)//若肘部关节比腿部关节到位慢，这里需要一个安全保证肘部到位时会停下来并指示蹲下动作完成
                            {
                                cmdSendBytes[1] = 0;
                                cmdSendBytes[2] = 0;
                                cmdSendBytes[3] = 0;
                                cmdSendBytes[4] = 0;
                                cmdSendBytes[13] = 0;
                                cmdSendBytes[14] = 0;
                                cmdSendBytes[15] = 0;
                                cmdSendBytes[16] = 0;
                                IsTrueSquatting = false; //蹲下动作完成
                                istrueX = false;
                            }
                        }

                        try
                        {
                            methods.SendControlCMD(cmdSendBytes);
                        }
                       catch
                        {
                            MessageBox.Show("未正确选择电机串口!");
                            IsTrueClickDown = false;
                            ActionStart_button.Content = "动作开始";
                            ActionStart_button.IsEnabled = true;
                            ActionStop_button.IsEnabled = false;
                        }
                    }

                    //起立动作执行..条件：腿部大于阈值；脚底压力和变化；是否在下蹲中
                    if (!IsTrueSquatting)
                    {
                        //起立中
                        IsTrueStanduping = true;
                        //判断腿部阈值是否近似于0
                        //int press = 0;
                        //for (int i = 0; i < 8; i++)
                        //{
                        //    press += spmManager.tempPress[i];
                        //}
                        if (methods.tempPress[7] > 200)//达到压力传感器8的阈值：原阈值设置为80，稍微扯一下压力传感器8的线双腿就会伸直，故这里设置得高一些
                        {
                            istrueY = true;
                        }
                        if (istrueY)
                        {
                            int rSpeed1 = 3600;
                            int rSpeed2 = 3000;


                            byte[] rSpeedBytes1 = BitConverter.GetBytes(rSpeed1);
                            byte[] rSpeedBytes2 = BitConverter.GetBytes(rSpeed2);
                            //  腿部电机控制

                            if (methods._angle[2] < -5)//左腿伸直
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
                            if (Math.Abs(methods._angle[3]) > 5)//右腿伸直:此角度传感器在增加到40°左右时会突变为-320°，此处改为绝对值依然可以完成原任务
                            {
                                cmdSendBytes[9] = 1;
                                cmdSendBytes[10] = 1;
                                cmdSendBytes[11] = rSpeedBytes1[1];
                                cmdSendBytes[12] = rSpeedBytes1[0];
                                if (Math.Abs(methods._angle[3]) < 10)
                                {
                                    cmdSendBytes[11] = rSpeedBytes2[1];
                                    cmdSendBytes[12] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[9] = 0;
                            }

                            if (methods.tempPress[7] > 200)
                            {  //肘部角度值检测以 确定手部运动
                               // 角度大于阈值（10度） 模拟拿起重物动作 小于阈值 无动作
                                if (methods._angle[0] < 50)//肘部弯曲
                                {
                                    cmdSendBytes[1] = 1;
                                    cmdSendBytes[2] = 0;
                                    cmdSendBytes[3] = rSpeedBytes1[1];
                                    cmdSendBytes[4] = rSpeedBytes1[0];

                                }
                                else
                                {
                                    if (methods._angle[0] < 60)
                                    {
                                        cmdSendBytes[1] = 1;
                                        cmdSendBytes[2] = 0;
                                        cmdSendBytes[3] = rSpeedBytes2[1];
                                        cmdSendBytes[4] = rSpeedBytes2[0];
                                    }
                                    else
                                    {
                                        //肘部电机停止
                                        cmdSendBytes[1] = 0;
                                        cmdSendBytes[2] = 0;
                                        cmdSendBytes[3] = 0;
                                        cmdSendBytes[4] = 0;
                                    }

                                }
                                if (methods._angle[1] > -50)//肘部弯曲
                                {
                                    cmdSendBytes[13] = 1;
                                    cmdSendBytes[14] = 1;
                                    cmdSendBytes[15] = rSpeedBytes1[1];
                                    cmdSendBytes[16] = rSpeedBytes1[0];
                                }
                                else
                                {
                                    if (methods._angle[1] > -60)
                                    {
                                        cmdSendBytes[13] = 1;
                                        cmdSendBytes[14] = 1;
                                        cmdSendBytes[15] = rSpeedBytes2[1];
                                        cmdSendBytes[16] = rSpeedBytes2[0];
                                    }
                                    else
                                    {
                                        // 肘部电机停止
                                        cmdSendBytes[13] = 0;

                                    }

                                }
                            }
                            else
                            {
                                cmdSendBytes[1] = 0;
                                cmdSendBytes[2] = 0;
                                cmdSendBytes[3] = 0;
                                cmdSendBytes[4] = 0;
                                cmdSendBytes[13] = 0;
                                cmdSendBytes[14] = 0;
                                cmdSendBytes[15] = 0;
                                cmdSendBytes[16] = 0;
                            }

                        }
                        if (Math.Abs(methods._angle[2]) < 5 && Math.Abs(methods._angle[3]) < 5)//腿已伸直时
                        {
                            cmdSendBytes[5] = 0;
                            cmdSendBytes[6] = 0;
                            cmdSendBytes[7] = 0;
                            cmdSendBytes[8] = 0;
                            cmdSendBytes[9] = 0;
                            cmdSendBytes[10] = 0;
                            cmdSendBytes[11] = 0;
                            cmdSendBytes[12] = 0;
                            if (istrueY && methods.tempPress[7] > 200)
                            {
                                if (Math.Abs(methods._angle[0]) > 60 && Math.Abs(methods._angle[1]) > 60)
                                {
                                    cmdSendBytes[1] = 0;
                                    cmdSendBytes[2] = 0;
                                    cmdSendBytes[3] = 0;
                                    cmdSendBytes[4] = 0;
                                    cmdSendBytes[13] = 0;
                                    cmdSendBytes[14] = 0;
                                    cmdSendBytes[15] = 0;
                                    cmdSendBytes[16] = 0;
                                    IsTrueStanduping = false;//拿重物起立动作完成
                                    istrueY = false;
                                }

                            }
                            else
                            {
                                IsTrueStanduping = false;//直接起来动作完成
                                istrueY = false;
                            }
                        }

                        try
                        {
                            methods.SendControlCMD(cmdSendBytes);
                        }
                        catch
                        {
                            MessageBox.Show("未正确选择电机串口!");
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
}

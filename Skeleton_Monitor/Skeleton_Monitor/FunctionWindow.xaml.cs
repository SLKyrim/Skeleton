using System;
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
        public string motor_com = null;           //存储电机所用串口
        public string press_com = null;           //存储压力及倾角传感器所用串口
        public string angle_com = null;           //存储角度传感器所用串口
        public bool flag = false;

        //串口
        private SerialPort motor_SerialPort = new SerialPort(); //电机串口
        private SerialPort press_SerialPort = new SerialPort(); //压力与倾角传感器串口
        private SerialPort angle_SerialPort = new SerialPort(); //角度传感器串口

        //电机的4个参数
        public byte[] enable = new byte[4];       //使能
        public byte[] direction = new byte[4];    //方向
        public double[] speed = new double[4];    //转速
        public double[] current = new double[4];   //电流

        //8个压力传感器所需参数（实际只用到1个压力传感器模拟重物，6个压力传感器安装在鞋垫）
        //4个倾角传感器（分别有x轴和y轴）所需参数（实际只用到1个倾角传感器）
        public Int16[] tempPress = new Int16[8];   //存储压力AD转换后的值（0-4096）
        private Int16[] tempAngle = new Int16[8];  //存储倾角AD转换后的值（0-4096）
        public double[] dirangle = new double[8];  //存储顷角度值（-90°到90°）

        //6个角度传感器所需参数（似乎只用到4个角度传感器）
        public double[] _angle = new double[6];
        private double[] _angleInitialization = new double[6];
        private Int16[] tempAngle2 = new Int16[8];//存储倾角AD转换后的值（0-4096）
        #endregion

        #region 文本输出
        private void FunctionWindow_Loaded(object sender, RoutedEventArgs e)//打开窗口后进行的初始化操作
        {
            ShowTimer = new System.Windows.Threading.DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer); //Tick是超过计时器间隔时发生事件，此处为Tick增加了一个叫ShowCurTimer的取当前时间并扫描串口的委托
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0); //设置刻度之间的时间值，设定为1秒（即文本框会1秒改变一次输出文本）
            ShowTimer.Start();

            //DispatcherTimer showTimer = new DispatcherTimer();
            //showTimer.Tick += new EventHandler(ShowSenderTimer); //增加了一个叫ShowSenderTimer的在电机和传感器的只读文本框中输出信息的委托
            //showTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);  //文本变化间隔是??毫秒(并不准确)
            //showTimer.Start();
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
            SPCount = SerialPort.GetPortNames();      //获得计算机可用串口名称数组
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
    

        #endregion

        #region comboBox控件

        #region 电机算法
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

                        if (motor_SerialPort.IsOpen)   //如果电机正在使用串口，则关闭串口以备初始化
                            motor_SerialPort.Close();

                        //电机串口初始化
                        methods.InitPort(motor_SerialPort, SPCount[i]);
                        motor_SerialPort.DataReceived += new SerialDataReceivedEventHandler(motor_DataReceived);  //接收事件处理方法motor_DataReceived即电机的算法
                    }
                }
            }
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
        #endregion

        #region 压力和倾角传感器算法
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

                        if (press_SerialPort.IsOpen)   //如果压力与倾角传感器正在使用串口，则关闭串口以备初始化
                            press_SerialPort.Close();

                        //压力与倾角传感器串口初始化
                        methods.InitPort(press_SerialPort, SPCount[i]);
                        press_SerialPort.DataReceived += new SerialDataReceivedEventHandler(press_DataReceived);  //接收事件处理方法press_DataReceived即电机的算法
                    }
                }
            }
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

        #region 角度传感器算法
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

                        if (angle_SerialPort.IsOpen)   //如果角度传感器正在使用串口，则关闭串口以备初始化
                            angle_SerialPort.Close();

                        //电机串口初始化
                        methods.InitPort(angle_SerialPort, SPCount[i]);
                        angle_SerialPort.DataReceived += new SerialDataReceivedEventHandler(angle_DataReceived);  //接收事件处理方法angle_DataReceived即电机的算法

                        methods.SendCommands();
                    }
                }
            }
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
                        tempAngle2[f] = BitConverter.ToInt16(bytes, f * 2 + 16);
                        _angle[f] = Convert.ToDouble(tempAngle2[f]) / 4096 * 3.3 / 3.05 * 360;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        _angle[i] = _angle[i] - _angleInitialization[i];
                    }
                }
            }      
        }
        #endregion

        #endregion

    }
}

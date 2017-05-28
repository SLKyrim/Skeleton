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
using System.IO;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace 外骨骼界面程序
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();

        }



        public string[] SPCount = null;
        public bool istrue = false;
        public string combobox1Com = null;
        public string combobox2Com = null;
        public string combobox3Com = null;
        public int comcount = 0;
        public int choosecount = 0;
        private DispatcherTimer ShowTimer;
        private SerialPortManager spmManager = new SerialPortManager();
        private bool IsTrain = false;
        private bool PressSerialPortIsDone = false;
        // private string dataString = null;
        private string peopleString = null;
        //private StreamWriter wr;
        //private FileStream fs;

        //private XmlSerializer xml;
        //private FileStream fs;
        //private SensorDataMode sdm;


        #region 以重构
        private void ComboBox1_OnDropDownClosed(object sender, EventArgs e)
        {
            ComboBoxItem item = ComboBox1.SelectedItem as ComboBoxItem;
            string str = item.Content.ToString();

            for (int i = 0; i < SPCount.Length; i++)
            {
                if (str == "串口" + SPCount[i])
                {
                    if (combobox2Com == SPCount[i] || combobox3Com == SPCount[i])
                    {
                        MessageBox.Show("串口" + SPCount[i] + "被占用");

                    }
                    else
                    {
                        combobox1Com = SPCount[i];

                        spmManager.serialPortInt1(SPCount[i]);


                    }
                }
            }
        }

        private void ComboBox2_OnDropDownClosed(object sender, EventArgs e)
        {
            ComboBoxItem item = ComboBox2.SelectedItem as ComboBoxItem;
            string str = item.Content.ToString();
            for (int i = 0; i < SPCount.Length; i++)
            {
                if (str == "串口" + SPCount[i])
                {
                    if (combobox1Com == SPCount[i] || combobox3Com == SPCount[i])
                    {
                        MessageBox.Show("串口" + SPCount[i] + "被占用");

                    }
                    else
                    {
                        combobox2Com = SPCount[i];

                        spmManager.serialPortInt2(SPCount[i]);
                        PressSerialPortIsDone = true;

                    }

                }
            }
        }

        private void ComboBox3_OnDropDownClosed(object sender, EventArgs e)
        {
            ComboBoxItem item = ComboBox3.SelectedItem as ComboBoxItem;
            string str = item.Content.ToString();
            for (int i = 0; i < SPCount.Length; i++)
            {
                if (str == "串口" + SPCount[i])
                {
                    if (combobox1Com == SPCount[i] || combobox2Com == SPCount[i])
                    {
                        MessageBox.Show("串口" + SPCount[i] + "被占用");

                    }
                    else
                    {
                        combobox3Com = SPCount[i];

                        spmManager.serialPortInt3(SPCount[i]);

                    }
                }
            }
        }

        public void ShowCurTimer(object sender, EventArgs e)
        {

            string nowTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            TextBoxday.Text = nowTime;
            spInt();
        }

        private void spInt()
        {


            SPCount = spmManager.checkSerialPortCount();



            ComboBoxItem t;

            if (comcount != SPCount.Length)
            {

                //需要删除所有下拉菜单
                ComboBox1.Items.Clear();
                ComboBox2.Items.Clear();
                ComboBox3.Items.Clear();



                t = new ComboBoxItem();
                t.Content = "请选择串口";
                ComboBox1.Items.Add(t);
                ComboBox1.SelectedIndex = 0;
                t = new ComboBoxItem();
                t.Content = "请选择串口";
                ComboBox2.Items.Add(t);
                ComboBox2.SelectedIndex = 0;
                t = new ComboBoxItem();
                t.Content = "请选择串口";
                ComboBox3.Items.Add(t);
                ComboBox3.SelectedIndex = 0;

                //ComboBoxItem item1 = ComboBox1.SelectedItem as ComboBoxItem;
                //string str1 = item1.Content.ToString();
                //ComboBoxItem item2 = ComboBox2.SelectedItem as ComboBoxItem;
                //string str2 = item2.Content.ToString();
                //ComboBoxItem item3 = ComboBox3.SelectedItem as ComboBoxItem;
                //string str3 = item3.Content.ToString();

                //for (int i = 0; i < SPCount.Length; i++)
                //{
                //    if (str1 == "串口" + SPCount[i])
                //    {
                //        if (str2 == "串口" + SPCount[i])
                //        {
                //            ComboBox3.Items.Remove(item3);
                //        }
                //        else
                //        {
                //            ComboBox2.Items.Remove(item2);
                //        }

                //    }
                //    else
                //    {
                //      ComboBox1.Items.Remove(item1);
                //    }
                //}




                combobox1Com = null;
                combobox2Com = null;
                combobox3Com = null;

                istrue = false;
                if (comcount != 0)
                {
                    MessageBox.Show("串口数量已变化 请重新选择串口号");
                }
                comcount = SPCount.Length;




            }

            if (!istrue)
            {


                if (SPCount.Length > 0)
                {
                    comcount = SPCount.Length;
                    TextBoxOut.Text = "已检测到串口" + SPCount.Length + "个!";
                    for (int i = 0; i < SPCount.Length; i++)
                    {
                        string st1 = "串口" + SPCount[i];
                        t = new ComboBoxItem();
                        t.Content = st1;
                        ComboBox1.Items.Add(t);
                        t = new ComboBoxItem();
                        t.Content = st1;
                        ComboBox2.Items.Add(t);
                        t = new ComboBoxItem();
                        t.Content = st1;
                        ComboBox3.Items.Add(t);

                    }
                    // spmManager.SerialPortInt(SPCount);
                    istrue = true;

                }
                else
                {
                    comcount = 0;
                    TextBoxOut.Text = "未检测到串口";
                }


            }


        }
        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {

            int num = Convert.ToInt16(TextBoxNum.Text);
            int enable = Convert.ToInt16(TextBoxcontrlEnable.Text);
            int dir = Convert.ToInt16(TextBoxDirection.Text);
            int rSpeed = Convert.ToInt16(TextBoxRotatingSpeed.Text);
            if (num == 0)
            {
                MessageBox.Show("请输入电机序号");
            }
            else
            {
                if (num > 5)
                {
                    MessageBox.Show("请输入1-4");
                }
                else
                {
                    if (enable > 1)
                    {
                        MessageBox.Show("请输入0-1");
                    }
                    else
                    {
                        if (rSpeed > 16200)
                        {
                            MessageBox.Show("输入转速过大，请小于16200");
                        }
                        else
                        {
                            choosecount++;

                            if (choosecount < 5)
                            {
                                TextBoxchoosecount.Text = choosecount.ToString();
                            }
                            byte numbyte = Convert.ToByte(num);
                            byte enablebyte = Convert.ToByte(enable);
                            byte[] rSpeedBytes = BitConverter.GetBytes(rSpeed);
                            byte dirnum = Convert.ToByte(dir);
                            switch (num)
                            {
                                case 1:
                                    cmdSendBytes[1] = enablebyte;
                                    cmdSendBytes[2] = dirnum;
                                    cmdSendBytes[3] = rSpeedBytes[1];
                                    cmdSendBytes[4] = rSpeedBytes[0];
                                    TextBoxIn.Text = "电机" + num + "使能" + enable + "转速" + rSpeed;
                                    break;
                                case 2:
                                    cmdSendBytes[5] = enablebyte;
                                    cmdSendBytes[6] = dirnum;
                                    cmdSendBytes[7] = rSpeedBytes[1];
                                    cmdSendBytes[8] = rSpeedBytes[0];
                                    TextBoxIn.Text = "电机" + num + "使能" + enable + "转速" + rSpeed;
                                    break;
                                case 3:
                                    cmdSendBytes[9] = enablebyte;
                                    cmdSendBytes[10] = dirnum;
                                    cmdSendBytes[11] = rSpeedBytes[1];
                                    cmdSendBytes[12] = rSpeedBytes[0];
                                    TextBoxIn.Text = "电机" + num + "使能" + enable + "转速" + rSpeed;
                                    break;
                                case 4:
                                    cmdSendBytes[13] = enablebyte;
                                    cmdSendBytes[14] = dirnum;
                                    cmdSendBytes[15] = rSpeedBytes[1];
                                    cmdSendBytes[16] = rSpeedBytes[0];
                                    TextBoxIn.Text = "电机" + num + "使能" + enable + "转速" + rSpeed;
                                    break;
                            }

                        }
                    }

                }
            }

        }

        private void ButtonClear_OnClick(object sender, RoutedEventArgs e)
        {
            byte[] clearBytes = new byte[19] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            spmManager.SendcontrolCMD(clearBytes);
        }

        private void ButtonAngleInitialize_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonAngleInitialize.Background = new SolidColorBrush(Color.FromArgb(255, 173, 255, 47));
            spmManager.serialPotr3Int();

            IsTrueForefootZero = true;
            ButtonAngleInitialize.Content = "初始化已完成";
        }

        private void beginbutton_Click(object sender, RoutedEventArgs e)
        {
            //需要获取角度、倾角、足底压力信息，并实现电机操控
            //时间触发 为真


            beginbutton.Content = "已开始";
            IsTrueClickDown = true;
            beginbutton.Background = Brushes.DarkSalmon;
            stopbutton.Background = Brushes.White;


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            beginbutton.Content = "动作开始";
            beginbutton.Background = Brushes.White;
            IsTrueClickDown = false;
            stopbutton.Background = Brushes.DarkSalmon;

        }

        public void ShowSenderTimer(object sender, EventArgs e)
        {
            Pressure1.Text = spmManager.tempPress[0].ToString();
            Pressure2.Text = spmManager.tempPress[1].ToString();
            Pressure3.Text = spmManager.tempPress[2].ToString();
            Pressure4.Text = spmManager.tempPress[3].ToString();
            Pressure5.Text = spmManager.tempPress[4].ToString();
            Pressure6.Text = spmManager.tempPress[5].ToString();
            Pressure7.Text = spmManager.tempPress[6].ToString();
            Pressure8.Text = spmManager.tempPress[7].ToString();

            TextBoxTiltQB1.Text = spmManager.dirangle[0].ToString("F");
            TextBoxTiltLR1.Text = spmManager.dirangle[1].ToString("F");
            TextBoxTiltQB2.Text = spmManager.dirangle[2].ToString("F");
            TextBoxTiltLR2.Text = spmManager.dirangle[3].ToString("F");
            TextBoxTiltQB3.Text = spmManager.dirangle[4].ToString("F");
            TextBoxTiltLR3.Text = spmManager.dirangle[5].ToString("F");
            TextBoxTiltQB4.Text = spmManager.dirangle[6].ToString("F");
            TextBoxTiltLR4.Text = spmManager.dirangle[7].ToString("F");

            TextBoxAngle1.Text = spmManager._angle[0].ToString("F");
            TextBoxAngle2.Text = spmManager._angle[1].ToString("F");
            TextBoxAngle3.Text = spmManager._angle[2].ToString("F");
            TextBoxAngle4.Text = spmManager._angle[3].ToString("F");
            TextBoxAngle5.Text = spmManager._angle[4].ToString("F");
            TextBoxAngle6.Text = spmManager._angle[5].ToString("F");

            TextBoxa1.Text = spmManager.enable[0].ToString();
            TextBoxa2.Text = spmManager.direction[0].ToString("F");
            TextBoxa3.Text = spmManager.speed[0].ToString("F");
            TextBoxa4.Text = spmManager.current[0].ToString("F");

            TextBoxb1.Text = spmManager.enable[1].ToString();
            TextBoxb2.Text = spmManager.direction[1].ToString("F");
            TextBoxb3.Text = spmManager.speed[1].ToString("F");
            TextBoxb4.Text = spmManager.current[1].ToString("F");

            TextBoxc1.Text = spmManager.enable[2].ToString();
            TextBoxc2.Text = spmManager.direction[2].ToString("F");
            TextBoxc3.Text = spmManager.speed[2].ToString("F");
            TextBoxc4.Text = spmManager.current[2].ToString("F");

            TextBoxd1.Text = spmManager.enable[3].ToString();
            TextBoxd2.Text = spmManager.direction[3].ToString("F");
            TextBoxd3.Text = spmManager.speed[3].ToString("F");
            TextBoxd4.Text = spmManager.current[3].ToString("F");

        }

        public void dongzuo(object sender, EventArgs e)
        {
            //检测开始按键是否按下
            if (IsTrueClickDown)
            {
                //初始化已经完成，开始检测状态
                if (IsTrueForefootZero)
                {
                    //下蹲..检测后背倾角变化 腿角度是否为站立  是否起立中
                    if (!IsTrueStanduping)
                    {

                        //正在下蹲中
                        IsTrueSquatting = true;
                        //如果腿部角度未达到阈值
                        if (spmManager.dirangle[7] < 75 && (Math.Abs(spmManager._angle[2]) < 7 || Math.Abs(spmManager._angle[3]) < 7))
                        {
                            istrueX = true;
                        }
                        if (istrueX && (Math.Abs(spmManager._angle[2]) < 60 || Math.Abs(spmManager._angle[3]) < 60))
                        {
                            int rSpeed1 = 3600;
                            int rSpeed2 = 3000;


                            byte[] rSpeedBytes1 = BitConverter.GetBytes(rSpeed1);
                            byte[] rSpeedBytes2 = BitConverter.GetBytes(rSpeed2);
                            //腿部电机控制

                            if (spmManager._angle[2] > -60)
                            {
                                cmdSendBytes[5] = 1;
                                cmdSendBytes[6] = 1;
                                cmdSendBytes[7] = rSpeedBytes1[1];
                                cmdSendBytes[8] = rSpeedBytes1[0];
                                if (spmManager._angle[2] < -50)
                                {
                                    cmdSendBytes[7] = rSpeedBytes2[1];
                                    cmdSendBytes[8] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[5] = 0;
                                cmdSendBytes[6] = 0;
                                cmdSendBytes[7] = 0;
                                cmdSendBytes[8] = 0;
                            }
                            if (spmManager._angle[3] < 60)
                            {
                                cmdSendBytes[9] = 1;
                                cmdSendBytes[10] = 0;
                                cmdSendBytes[11] = rSpeedBytes1[1];
                                cmdSendBytes[12] = rSpeedBytes1[0];
                                if (spmManager._angle[3] > 50)
                                {
                                    cmdSendBytes[11] = rSpeedBytes2[1];
                                    cmdSendBytes[12] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[9] = 0;
                                cmdSendBytes[10] = 0;
                                cmdSendBytes[11] = 0;
                                cmdSendBytes[12] = 0;
                            }


                            //肘部角度值检测以 确定手部运动
                            //角度大于阈值（40度） 放下动作  小于阈值 无动作
                            if (spmManager._angle[0] > 40)
                            {
                                cmdSendBytes[1] = 1;
                                cmdSendBytes[2] = 1;
                                cmdSendBytes[3] = rSpeedBytes1[1];
                                cmdSendBytes[4] = rSpeedBytes1[0];

                            }
                            else
                            {
                                if (spmManager._angle[0] > 5)
                                {
                                    cmdSendBytes[1] = 1;
                                    cmdSendBytes[2] = 1;
                                    cmdSendBytes[3] = rSpeedBytes2[1];
                                    cmdSendBytes[4] = rSpeedBytes2[0];
                                }
                                else
                                {
                                    //肘部电机停止 
                                    cmdSendBytes[1] = 0;

                                }

                            }
                            if (spmManager._angle[1] < -40)
                            {
                                cmdSendBytes[13] = 1;
                                cmdSendBytes[14] = 0;
                                cmdSendBytes[15] = rSpeedBytes1[1];
                                cmdSendBytes[16] = rSpeedBytes1[0];
                            }
                            else
                            {
                                if (spmManager._angle[1] < -5)
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
                            //肘部小于阈值
                            if (spmManager._angle[0] < 5 && spmManager._angle[1] > -5)
                            {
                                cmdSendBytes[1] = 0;
                                cmdSendBytes[2] = 0;
                                cmdSendBytes[3] = 0;
                                cmdSendBytes[4] = 0;
                                cmdSendBytes[13] = 0;
                                cmdSendBytes[14] = 0;
                                cmdSendBytes[15] = 0;
                                cmdSendBytes[16] = 0;
                                IsTrueSquatting = false;
                                istrueX = false;
                            }
                        }

                        spmManager.SendcontrolCMD(cmdSendBytes);



                    }

                    //起立 腿部大于阈值 脚底压力和变化 是否在下蹲中
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
                        if (spmManager.tempPress[7] > 40)
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

                            if (spmManager._angle[2] < -5)
                            {
                                cmdSendBytes[5] = 1;
                                cmdSendBytes[6] = 0;
                                cmdSendBytes[7] = rSpeedBytes1[1];
                                cmdSendBytes[8] = rSpeedBytes1[0];
                                if (spmManager._angle[2] > -10)
                                {
                                    cmdSendBytes[7] = rSpeedBytes2[1];
                                    cmdSendBytes[8] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[5] = 0;
                            }
                            if (spmManager._angle[3] > 5)
                            {
                                cmdSendBytes[9] = 1;
                                cmdSendBytes[10] = 1;
                                cmdSendBytes[11] = rSpeedBytes1[1];
                                cmdSendBytes[12] = rSpeedBytes1[0];
                                if (spmManager._angle[3] < 10)
                                {
                                    cmdSendBytes[11] = rSpeedBytes2[1];
                                    cmdSendBytes[12] = rSpeedBytes2[0];
                                }
                            }
                            else
                            {
                                cmdSendBytes[9] = 0;
                            }

                            if (spmManager.tempPress[7] > 200)
                            {  //肘部角度值检测以 确定手部运动
                               // 角度大于阈值（10度） 放下动作 小于阈值 无动作
                                if (spmManager._angle[0] < 50)
                                {
                                    cmdSendBytes[1] = 1;
                                    cmdSendBytes[2] = 0;
                                    cmdSendBytes[3] = rSpeedBytes1[1];
                                    cmdSendBytes[4] = rSpeedBytes1[0];

                                }
                                else
                                {
                                    if (spmManager._angle[0] < 60)
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
                                if (spmManager._angle[1] > -50)
                                {
                                    cmdSendBytes[13] = 1;
                                    cmdSendBytes[14] = 1;
                                    cmdSendBytes[15] = rSpeedBytes1[1];
                                    cmdSendBytes[16] = rSpeedBytes1[0];
                                }
                                else
                                {
                                    if (spmManager._angle[1] > -60)
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
                        if (Math.Abs(spmManager._angle[2]) < 5 && Math.Abs(spmManager._angle[3]) < 5)
                        {
                            cmdSendBytes[5] = 0;
                            cmdSendBytes[6] = 0;
                            cmdSendBytes[7] = 0;
                            cmdSendBytes[8] = 0;
                            cmdSendBytes[9] = 0;
                            cmdSendBytes[10] = 0;
                            cmdSendBytes[11] = 0;
                            cmdSendBytes[12] = 0;
                            if (istrueY && spmManager.tempPress[7] > 200)
                            {
                                if (Math.Abs(spmManager._angle[0]) > 60 && Math.Abs(spmManager._angle[1]) > 60)
                                {
                                    cmdSendBytes[1] = 0;
                                    cmdSendBytes[2] = 0;
                                    cmdSendBytes[3] = 0;
                                    cmdSendBytes[4] = 0;
                                    cmdSendBytes[13] = 0;
                                    cmdSendBytes[14] = 0;
                                    cmdSendBytes[15] = 0;
                                    cmdSendBytes[16] = 0;
                                    IsTrueStanduping = false;
                                    istrueY = false;
                                }

                            }
                            else
                            {
                                IsTrueStanduping = false;
                                istrueY = false;
                            }


                        }


                        spmManager.SendcontrolCMD(cmdSendBytes);
                    }
                    //  }
                }
            }
        }
        #endregion

        /// <summary>
        /// show time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>







        private byte[] cmdSendBytes = new byte[19];


        private void ButtonTrain_OnClick(object sender, RoutedEventArgs e)
        {


            int a = Convert.ToInt16(TextBoxtimeset.Text);
            if (a != 0)
            {
                if (PressSerialPortIsDone)
                {
                    ButtonTrain.Background = new SolidColorBrush(Color.FromArgb(255, 173, 255, 47));
                    //string nowTime = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    //sdm = new SensorDataMode();
                    //xml = new XmlSerializer(typeof(SensorDataMode));
                    //string name = nowTime + ".xml";
                    //fs = File.OpenWrite(name);
                    //DispatcherTimer showTimer = new DispatcherTimer();
                    //showTimer.Tick += new EventHandler(TrainCode);
                    //showTimer.Interval = new TimeSpan(0, 0, 0, 0, a);
                    //showTimer.Start();
                    // dataString = peopleString + "\r\n";
                    // dataString += " " + "采集时间" +
                    //" " + "电机1使能" + " " + "电机1方向" + " " + "电机1速度" + " " + "电机1角度" +
                    //" " + "电机2使能" + " " + "电机2方向" + " " + "电机2速度" + " " + "电机2角度" +
                    //" " + "电机3使能" + " " + "电机3方向" + " " + "电机3速度" + " " + "电机3角度" +
                    //" " + "电机4使能" + " " + "电机4方向" + " " + "电机4速度" + " " + "电机4角度" +
                    //   " " + "压力1" + " " + "压力2" + " " + "压力3" + " " + "压力4" +
                    //   " " + "压力5" + " " + "压力6" + " " + "压力7" + " " + "压力8" +
                    //  " " + "角度1" + " " + "角度2" + " " + "角度3" + " " + "角度4" + " " + "角度5" + " " + "角度6" +
                    //   " " + "倾角1左右" + " " + "倾角1前后" + " " + "倾角2左右" + " " + "倾角2前后" +
                    //   " " + "倾角3左右" + " " + "倾角3前后" + " " + "倾角4左右" + " " + "倾角4前后" +
                    //   "\r\n ";

                    //IsTrain = true;

                    spmManager.BeginTrain(peopleString);
                }
                else
                {
                    MessageBox.Show("压力传感器串口未连接，数据无法存储！");
                }


            }
            else
            {
                MessageBox.Show("采样时间设置应大于0！");
            }


            //Console.WriteLine("OK");

            //FileStream file = new FileStream("List.xml", FileMode.Open, FileAccess.Read);
            //XmlSerializer xmlSearializer = new XmlSerializer(typeof(SensorDataMode));
            //SensorDataMode info = (SensorDataMode)xmlSearializer.Deserialize(file);
            //file.Close();
            //foreach (SensorData per in info.SensorDatas)
            //{
            //    Console.WriteLine("人员：");
            //    Console.WriteLine(" 姓名：" + per.TimeString);


            //}
        }

        //private void TrainCode(object sender, EventArgs e)
        //{
        //    if (IsTrain)
        //    {
        //        //SensorData sd1 = new SensorData();
        //        //string nowTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
        //        //Motor m1 = new Motor(spmManager.enable[0], spmManager.direction[0], spmManager.speed[0], spmManager.current[0]);
        //        //sd1.Motors.Add(m1);
        //        //Motor m2 = new Motor(spmManager.enable[1], spmManager.direction[1], spmManager.speed[1], spmManager.current[1]);
        //        //sd1.Motors.Add(m2);
        //        //Motor m3 = new Motor(spmManager.enable[2], spmManager.direction[2], spmManager.speed[2], spmManager.current[2]);
        //        //sd1.Motors.Add(m3);
        //        //Motor m4 = new Motor(spmManager.enable[3], spmManager.direction[3], spmManager.speed[3], spmManager.current[3]);
        //        //sd1.Motors.Add(m4);
        //        //PressureSender p1 = new PressureSender(spmManager.tempPress[0], spmManager.tempPress[1], spmManager.tempPress[2],
        //        //    spmManager.tempPress[3], spmManager.tempPress[4], spmManager.tempPress[5], spmManager.tempPress[6], spmManager.tempPress[7]);

        //        //AngleSender a1 = new AngleSender(spmManager.angle[0],
        //        //    spmManager.angle[1], spmManager.angle[2], spmManager.angle[3]);

        //        //Dip d1 = new Dip(spmManager.dirangle[0], spmManager.dirangle[1]);
        //        //Dip d2 = new Dip(spmManager.dirangle[2], spmManager.dirangle[3]);
        //        //Dip d3 = new Dip(spmManager.dirangle[4], spmManager.dirangle[5]);
        //        //Dip d4 = new Dip(spmManager.dirangle[6], spmManager.dirangle[7]);
        //        //sd1.Pressures = p1;
        //        //sd1.Angles = a1;
        //        //sd1.Dips.Add(d1);
        //        //sd1.Dips.Add(d2);
        //        //sd1.Dips.Add(d3);
        //        //sd1.Dips.Add(d4);
        //        //sd1.TimeString = nowTime;

        //        //sdm.SensorDatas.Add(sd1);


        //        //dataString += DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + ":" + DateTime.Now.Millisecond.ToString();
        //        //for (int f = 0; f < 4; f++)
        //        //{
        //        //    dataString += " " + spmManager.enable[f].ToString("F2") + " "+ spmManager.direction[f].ToString("F2") + " " + spmManager.speed[f].ToString("F2") + " " + spmManager.current[f].ToString("F2");
        //        //}

        //        //for (int f = 0; f < 8; f++)
        //        //{
        //        //    dataString += " " + spmManager.tempPress[f].ToString("F2");
        //        //}

        //        //for (int f = 0; f < 6; f++)
        //        //{
        //        //    dataString += " " + spmManager._angle[f].ToString("F2");
        //        //}
        //        //for (int f = 0; f < 8; f++)
        //        //{
        //        //    dataString += " " + spmManager.dirangle[f].ToString("F2");
        //        //}
        //        //dataString += "\r\n ";

        //        //wr.Write(dataString);
        //        //wr.Flush();
        //        //dataString = null;
        //    }

        //}

        private void ButtonOverTrain_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonTrain.Background = new SolidColorBrush(Color.FromArgb(255, 240, 245, 255));
            //xml.Serialize(fs, sdm);

            spmManager.OverTrain();


            //wr.Close();
            //fs.Close();
            //dataString = null;
            //IsTrain = false;
            //fs.Close();
        }



        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            peopleString = null;
            PageFunction1 pf = new PageFunction1();
            pf.Return += new ReturnEventHandler<string>(pfReturn);
            this.NavigationService.Navigate(pf);




        }

        void pfReturn(object sender, ReturnEventArgs<string> e)
        {
            this.peopleString = e.Result;
        }

        private void Page1_OnLoaded(object sender, RoutedEventArgs e)
        {
            ShowTimer = new System.Windows.Threading.DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer);//起个Timer一直获取当前时间
                                                             // ShowTimer.Tick += new EventHandler(dongzuo);
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ShowTimer.Start();
            DispatcherTimer showTimer = new DispatcherTimer();
            showTimer.Tick += new EventHandler(ShowSenderTimer);
            showTimer.Tick += new EventHandler(dongzuo);
            showTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            showTimer.Start();
        }



        public bool IsTrueClickDown = false;
        public bool IsTrueForefootZero = false;
        public bool IsTrueInitialization = false;
        public bool IsTrueSquatting = false;
        public bool IsTrueStanduping = false;
        
        public bool istrueX = false;
        public bool istrueY = false;
        //   public byte[] OleBytes = new byte[19] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


        
    }
    }
    



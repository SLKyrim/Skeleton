using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;
using System.Xml.Serialization;

namespace 外骨骼界面程序
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window 
    {
        SerialPortManager spmManager = new SerialPortManager();




        public MainWindow()
        {
            InitializeComponent();
        }
   
      
       

     
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
           
            spmManager.SerialPortClose();
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}

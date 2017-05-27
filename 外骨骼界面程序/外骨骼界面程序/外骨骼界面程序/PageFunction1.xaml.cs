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

namespace 外骨骼界面程序
{
    /// <summary>
    /// PageFunction1.xaml 的交互逻辑
    /// </summary>
    public partial class PageFunction1 : PageFunction<String>
    {
        public PageFunction1()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
           
            string mystring = "姓名："+textBox1.Text+"身高："+textBox2+"体重："+textBox3
                + "大腿长：" + textBox4 + "小腿长：" + textBox5 + "年龄：" + textBox21
                + "双髋间距" + textBox22 + "脚板间距" + textBox23 + "脚踝高度" + textBox24 ;
            OnReturn(new ReturnEventArgs<string>(mystring));


        }
    }
}

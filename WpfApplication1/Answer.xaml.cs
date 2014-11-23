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
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для Answer.xaml
    /// </summary>
    public partial class AnswerMessage : Window
    {
        public AnswerMessage(string x, string ItNum) //Косяк по доступности, поэтому передаю строки вместо Solution
        {
            InitializeComponent();

            xLabel.Content = x;
            ItNumLabel.Content = ItNum;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

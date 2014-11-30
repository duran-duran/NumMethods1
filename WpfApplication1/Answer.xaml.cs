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
using System.Windows.Forms.DataVisualization.Charting;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для Answer.xaml
    /// </summary>
    public partial class AnswerMessage : Window
    {
        public AnswerMessage(string x, string ItNum, List<double> residual) //Косяк по доступности, поэтому передаю строки вместо Solution
        {
            InitializeComponent();

            xLabel.Content = x;
            ItNumLabel.Content = ItNum;

            ResidualChart.ChartAreas.Add("Residual");
            ResidualChart.ChartAreas["Residual"].AxisX.Title = "Iterations";
            ResidualChart.ChartAreas["Residual"].AxisY.Title = "Residual";

            ResidualChart.Series.Add("Series");
            ResidualChart.Series["Series"].ChartType = SeriesChartType.Line;
            ResidualChart.Series["Series"].Points.DataBindY(residual);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

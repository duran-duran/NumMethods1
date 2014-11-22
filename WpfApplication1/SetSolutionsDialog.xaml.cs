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
    /// Логика взаимодействия для SetSolutionsDialog.xaml
    /// </summary>
    public partial class SetSolutionsDialog : Window
    {
        public SetSolutionsDialog(int dim)
        {
            InitializeComponent();

            Matrix x = new Matrix(1, dim);
            List<double[]> xView = MainWindow.MatrixView(x);

            xGrid.Columns.Clear();
            xGrid.AutoGenerateColumns = false;

            for (int i = 0; i < x.cols; i++)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                Binding binding = new Binding("[" + i + "]");
                col.Binding = binding;
                col.Header = "x" + (i + 1);
                xGrid.Columns.Add(col);
            }
            xGrid.ItemsSource = xView;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

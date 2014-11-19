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

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public List<double[]> MatrixView(Matrix A)
        { 
            List<double[]> result = new List<double[]>();

            for (int i = 0; i < A.rows; i++)
            {
                double[] row = new double[A.cols];

                for (int j = 0; j < A.cols; j++)
                {
                    row[j] = A.values[i, j];
                }

                result.Add(row);
            }

            return result;
        }


        public MainWindow()
        {
            InitializeComponent();
            Matrix A = new Matrix(3, 3);    //5x1 + 3x2 + x3 = 14   x1 = 1
            A.values = new double[3,3]{     //-2x1 + x2 - 3x3 = -9  x2 = 2
                {5, 3, 1},                  // x1 + 7x2 - 2x3 = 9   x3 = 3
                {-2, 1, -3},
                {1, 7, -2}};
            Matrix b = new Matrix(3, 1);
            b.values = new double[3, 1] { { 14 }, { -9 }, { 9 } };
            Matrix L = A.ToLowTr();
            Matrix M = Methods.LowTrInverse(L);
      

            //MessageBox.Show(UpperRelax(A, b, 0.00000001, 1000000).ToString());
            //MessageBox.Show(JacobiMethod(A, b, 0.00000001, 1000000).ToString());   
        }

        private void SolveBtn_Click(object sender, RoutedEventArgs e)
        {
            Matrix A = new Matrix(AGrid.ItemsSource as List<double[]>);//Перегнать из гридов в матрицы
            Matrix b = new Matrix(bGrid.ItemsSource as List<double[]>);//Как насчет оформить валидации?

            Matrix x = Methods.JacobiMethod(A, b, 0.0000001, 1000000);//Решить систему
            MessageBox.Show(x.ToString());//Вывести ответ
            MessageBox.Show((A*x).ToString());
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            int dim = Convert.ToInt32(DimBox.Text);
            Matrix A = new Matrix(dim, dim);
            Matrix b = new Matrix(dim, 1);

            A.Randomize(Convert.ToDouble(MinBox.Text), Convert.ToDouble(MaxBox.Text));  //Как насчет оформить валидации?
            b.Randomize(Convert.ToDouble(MinBox.Text), Convert.ToDouble(MaxBox.Text));

            List<double[]> AView = MatrixView(A);
            List<double[]> bView = MatrixView(b);

            AGrid.Columns.Clear();
            AGrid.AutoGenerateColumns = false;
            bGrid.AutoGenerateColumns = false;
            Binding binding;

            for (int i = 0; i < A.cols; i++)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                binding = new Binding("[" + i + "]");
                col.Binding = binding;
                col.Header = "x" + (i + 1);
                AGrid.Columns.Add(col);
            }
            AGrid.ItemsSource = AView;

            binding = new Binding("[0]");
            DataGridTextColumn c = bGrid.Columns[0] as DataGridTextColumn;
            c.Binding = binding;
            bGrid.ItemsSource = bView;
        }
    }
}

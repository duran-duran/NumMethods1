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
        //Преобразование матрицы в коллекцию для отображения в DataGrid. Стоит ли выделить в отдельный класс?
        public static List<double[]> MatrixView(Matrix A)
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
      

            //MessageBox.Show(UpperRelax(A, b, 0.00000001, 1000000).ToString());
            //MessageBox.Show(JacobiMethod(A, b, 0.00000001, 1000000).ToString());   
        }


        private void SolveBtn_Click(object sender, RoutedEventArgs e)
        {
            Matrix A = new Matrix(AGrid.ItemsSource as List<double[]>);
            Matrix b = new Matrix(bGrid.ItemsSource as List<double[]>);//Как насчет оформить валидации?

            double eps = Convert.ToDouble(EpsBox.Text); //Впилить валидации. Мб заменить на tryParse?
            int maxN = Convert.ToInt32(MaxItNumBox.Text); //Впилить валидации

            string MethodCode = (MethodBox.SelectedItem as ComboBoxItem).Name;

            Solution result;

            switch (MethodCode)
            {
                case "J":
                    result = Methods.JacobiMethod(A, b, eps, maxN);
                    break;
                case "GS":
                    result = Methods.SOR(A, b, 1, eps, maxN);
                    break;
                case "SOR":
                    double t = tParamSlider.Value;
                    result = Methods.SOR(A, b, t, eps, maxN);
                    break;
                default://Чтобы не пищала ошибка про неинициализированную переменную
                    result = new Solution((new Matrix(A.rows,1)),1);
                    break;
            }

            AnswerMessage AnsMsg = new AnswerMessage(result.ToString(4), result.ItNum.ToString()); //Косяяяяяк
            AnsMsg.Owner = this;

            AnsMsg.ShowDialog();
            //Matrix x = result.res;
            //MessageBox.Show(result.ToString(3));//Вывести ответ
            //MessageBox.Show(result.ItNum.ToString());
            //MessageBox.Show((A*x).ToString());//Проверка
        }


        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EqSysContainer.Visibility == System.Windows.Visibility.Collapsed)  //При первой генерации показываем контейнер со СЛАУ
            {
                EqSysContainer.Visibility = System.Windows.Visibility.Visible;
            }

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


        private void SolGenBtn_Click(object sender, RoutedEventArgs e)
        {
            int dim = AGrid.Items.Count;
            SetSolutionsDialog dlg = new SetSolutionsDialog(dim);
            dlg.Owner = this;

            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                Matrix x = new Matrix(dlg.xGrid.ItemsSource as List<double[]>);
                Matrix A = new Matrix(AGrid.ItemsSource as List<double[]>);
                x = x.Transpose();
                Matrix b = A * x;

                List<double[]> bView = MatrixView(b);
                bGrid.ItemsSource = bView;
            }
        }

        private void MCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SolveBtn.IsEnabled = true;//Систему можно решить, только если выбран метод

            if ((sender as ComboBox).SelectedIndex == 0)
            {
                MethodParamsContainer.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                MethodParamsContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void MethodBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((sender as ComboBox).SelectedItem as ComboBoxItem).Name == "SOR")
            {
                tParamPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else if (tParamPanel != null)//Костыль, при инициализации окна при заданном SelectedItem'e происходит вызов данного метода
            {
                tParamPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

       
    }
}

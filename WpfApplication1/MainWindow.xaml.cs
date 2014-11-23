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
        }


        private void SolveBtn_Click(object sender, RoutedEventArgs e)
        {
            Matrix A = new Matrix(AGrid.ItemsSource as List<double[]>);
            Matrix b = new Matrix(bGrid.ItemsSource as List<double[]>);//Как насчет оформить валидации?

            //Преобразуем систему таким образом, чтобы матрица A была симметричной и положительно определенной
            Matrix tmp = A.Copy();
            A = A.Transpose() * A;
            b = tmp.Transpose() * b;

            double eps = Convert.ToDouble(EpsBox.Text); //Впилить валидации. Мб заменить на tryParse?
            int maxN = Convert.ToInt32(MaxItNumBox.Text); //Впилить валидации

            string PreconditionerCode = (PreconditionerBox.SelectedItem as ComboBoxItem).Name;
            string MethodCode = (MethodCombobox.SelectedItem as ComboBoxItem).Name;

            Matrix M = new Matrix(A.rows, A.cols);
            double t = 0;
            Solution result = new Solution();

            switch (PreconditionerCode)
            {
                case "Id":
                    M = new Matrix(A.rows, A.cols); //result = Methods.JacobiMethod(A, b, eps, maxN);
                    M.ToIdentityMatrix();
                    break;
                case "Diag":
                    M = Methods.GetD(A); //result = Methods.SOR(A, b, 1, eps, maxN);
                    break;
                case "LowTr":
                    t = tParamSlider.Value;
                    Matrix D = Methods.GetD(A); Matrix L = Methods.GetL(A); //result = Methods.SOR(A, b, t, eps, maxN);
                    M = D + t * L;
                    break;
            }

            switch (MethodCode)
            {
                case "Jacobi":
                    if (t == 0) //Если параметр не определен
                    {
                        double S = 1.15 * A.Norm(); //Некоторое число, большее нормы A. Сделать ли ввод?
                        t = 2 / S;
                    }
                    result = Methods.PreconditionedJacobiMethod(A, b, M, t, eps, maxN);
                    break;  
                case "SteepestDescent":
                    result = Methods.PreconditionedSteepestDescent(A, b, M, eps, maxN);
                    break;
            }

            int digitsNum = 4; //Сделать ли ввод?
            AnswerMessage AnsMsg = new AnswerMessage(result.ToString(digitsNum), result.ItNum.ToString()); //Косяяяяяк.
            AnsMsg.Owner = this;

            AnsMsg.ShowDialog();
            Matrix x = result.vector;
            MessageBox.Show((tmp*x).ToString());//Проверка
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

        private void MethodCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SolveBtn.IsEnabled = true;//Систему можно решить, только если выбран метод
            PreconditionerContainer.Visibility = System.Windows.Visibility.Visible;
        }

        private void PreconditionerBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((sender as ComboBox).SelectedItem as ComboBoxItem).Name == "LowTr")
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

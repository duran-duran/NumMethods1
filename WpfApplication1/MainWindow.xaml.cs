﻿using System;
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
        public MainWindow()
        {
            InitializeComponent();
            //Всякая херня для тестирования
            /*Matrix A = new Matrix(3,3);
            A.Randomize(1, 10);
            Matrix E = A.Copy();
            E.ToIdentityMatrix();

            MessageBox.Show(A.ToString());
            MessageBox.Show(E.ToString());
            MessageBox.Show(Methods.LowTrInverse(E).ToString());
            Matrix D = Methods.GetD(A);
            Matrix L = Methods.GetL(A);
            Matrix M = D + L;
            Matrix MInv = Methods.LowTrInverse(M);

            MessageBox.Show(M.ToString());
            MessageBox.Show(MInv.ToString());
            MessageBox.Show((M * MInv).ToString());
            MessageBox.Show((MInv * M).ToString());*/

        }


        private void SolveBtn_Click(object sender, RoutedEventArgs e)
        {
            Matrix A = (Matrix)(AGrid.ItemsSource as MatrixView);
            Matrix b = (Matrix)(bGrid.ItemsSource as MatrixView);

            double eps = Convert.ToDouble(EpsBox.Text); //Впилить валидации. Мб заменить на tryParse?
            int maxN = Convert.ToInt32(MaxItNumBox.Text); //Впилить валидации

            string PreconditionerCode = (PreconditionerBox.SelectedItem as ComboBoxItem).Name;
            string MethodCode = (MethodCombobox.SelectedItem as ComboBoxItem).Name;

            bool IsTransformNeeded = (bool)SysTransformFlag.IsChecked;

            Matrix tmp = A.Copy();
            //Преобразуем систему таким образом, чтобы матрица A была симметричной и положительно определенной
            if (IsTransformNeeded)
            {
                A = A.Transpose() * A;        //Для матриц с диагональным преобладанием данное преобразование только мешает. (В частности, метод Якоби с диагональным прекондишнером расходиться
                b = tmp.Transpose() * b;
            }
            
            Matrix M = new Matrix(A.rows, A.cols);
            double t = 0;
            Solution result = new Solution();

            //Как-то странно у меня получается с выбором прекондиционера и подгоном t в методе Якоби. По идее, должно быть что-то одно
            switch (PreconditionerCode)
            {
                case "Id":
                    M = new Matrix(A.rows, A.cols);
                    M.ToIdentityMatrix();
                    break;
                case "Diag":
                    M = Methods.GetD(A); //диагональный прекондиционер для Якоби как-то не канает, только для матриц с диагональным преобладанием
                    t = 1; // Экспериментирую
                    break;
                case "LowTr":
                    t = tParamSlider.Value;
                    Matrix D = Methods.GetD(A); Matrix L = Methods.GetL(A);
                    M = D + t * L;
                    break;
            }

            switch (MethodCode)
            {
                case "Jacobi":
                    if (t == 0) //Если параметр не определен
                    {
                        double S = 1.15 * A.Norm(); //Некоторое число, большее нормы A. Сделать ли ввод?
                        t = 2 / S; //Для диагонального прекондишнера данное тау - полная хрень бтв.
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
            bool isDiagDom = (bool)DiagDomFlag.IsChecked;
            Matrix A = new Matrix(dim, dim);
            Matrix b = new Matrix(dim, 1);

            A.Randomize(Convert.ToDouble(MinBox.Text), Convert.ToDouble(MaxBox.Text), isDiagDom);  //Как насчет оформить валидации?
            b.Randomize(Convert.ToDouble(MinBox.Text), Convert.ToDouble(MaxBox.Text), false);

            MatrixView AView = (MatrixView)A;
            MatrixView bView = (MatrixView)b;

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
                Matrix x = (Matrix)(dlg.xGrid.ItemsSource as MatrixView);
                Matrix A = (Matrix)(AGrid.ItemsSource as MatrixView);
                x = x.Transpose();
                Matrix b = A * x;

                MatrixView bView = (MatrixView)b;
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

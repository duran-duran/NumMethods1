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
        public Matrix JacobiMethod(Matrix A, Matrix b, double e, int maxN)
        {
            Matrix x = new Matrix(A.rows, 1);
            Matrix E = new Matrix(A.rows, A.cols); E.ToIdentityMatrix();

            Matrix tmp = A.Copy();
            A = A.Transpose() * A;
            b = tmp.Transpose() * b;
            tmp = b.Copy();
            double t = 2 / (0.79 * A.Norm());

            int i;
            for (i = 0; i < maxN; i++)
            {               
                x = (E - t*A) * tmp + t*b;
                if ((x - tmp).Norm() < e)
                    break;
                tmp = x.Copy();
            }

            return x;
        }


        public Matrix UpperRelax(Matrix A, Matrix b, double e, int maxN)
        {
            Matrix x = new Matrix(A.rows, 1);
            Matrix E = new Matrix(A.rows, A.cols); E.ToIdentityMatrix();

            Matrix tmp = A.Copy();
            A = A.Transpose() * A;
            b = tmp.Transpose() * b;
            Matrix M = A.ToLowTr();
            Matrix MInv = LowTrInverse(M);
            tmp = x.Copy();
            double t = 1;

            int i;
            for (i = 0; i < maxN; i++)
            {
                x = (E - t * MInv * A) * tmp + t * MInv * b;
                if ((x - tmp).Norm() < e)
                    break;
                tmp = x.Copy();
            }

            return x;
        }


        //Обращение нижнетреугольной матрицы. Сделать проверку?
        public Matrix LowTrInverse(Matrix A)
        {
            Matrix M = new Matrix(A.rows, A.cols);

            for (int i = 0; i < A.rows; i++)
            {
                M.values[i,i] = 1 / A.values[i, i];
                for (int j = 0; j < i; j++)
                {
                    for (int k = 0; k < i; k++)
                    {
                        M.values[i, j] += M.values[k, j] * A.values[i, k];
                    }
                    M.values[i, j] *= (-1 / A.values[i, i]);
                }
            }

            return M;
        }

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
            Matrix M = LowTrInverse(L);

            List<double[]> view = MatrixView(A);

            AGrid.AutoGenerateColumns = false;

            for (int i = 0; i < A.cols; i++)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                Binding binding = new Binding("[" + i + "]");
                col.Binding = binding;
                col.Header = "x" + (i + 1);
                AGrid.Columns.Add(col);
            }

            AGrid.ItemsSource = view;

            //MessageBox.Show(UpperRelax(A, b, 0.00000001, 1000000).ToString());
            //MessageBox.Show(JacobiMethod(A, b, 0.00000001, 1000000).ToString());   
        }
    }
}

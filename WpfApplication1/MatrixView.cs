using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class MatrixView : List<double[]>
    {
        public MatrixView() : base()
        {
        }


        public static explicit operator MatrixView(Matrix A)
        {
            MatrixView result = new MatrixView();

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
    }
}

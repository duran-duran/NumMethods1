using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Methods
    {
        public static Matrix JacobiMethod(Matrix A, Matrix b, double e, int maxN) //Корявый чек на сходимость, нужна оценка погрешности результата (или смотреть невязку?)
        {
            Matrix x = new Matrix(A.rows, 1);
            Matrix E = new Matrix(A.rows, A.cols); E.ToIdentityMatrix();

            Matrix tmp = A.Copy();
            A = A.Transpose() * A;
            b = tmp.Transpose() * b;
            tmp = b.Copy();
            double t = 2 / (1.2 * A.Norm());

            int i;
            for (i = 0; i < maxN; i++)
            {
                x = (E - t * A) * tmp + t * b;
                if ((x - tmp).Norm() < e)
                    break;
                tmp = x.Copy();
            }

            return x;
        }


        public static Matrix UpperRelax(Matrix A, Matrix b, double e, int maxN) //Пофиксить согласно теории (прикрутить t)
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

        public static Matrix LowTrInverse(Matrix A) //Пофиксить
        {
            Matrix M = new Matrix(A.rows, A.cols);

            for (int i = 0; i < A.rows; i++)
            {
                M.values[i, i] = 1 / A.values[i, i];
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
    }
}

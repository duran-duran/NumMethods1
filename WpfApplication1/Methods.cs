using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Methods
    {
        //Условность, принятая в обоих методах: матрица-preconditioner - нижнедиагональная
        public static Solution PreconditionedJacobiMethod(Matrix A, Matrix b, Matrix M, double t, double e, int maxN) 
        {
            Matrix MInv = LowTrInverse(M);

            //Matrix tmp;
            Matrix x = b.Copy();
            Matrix r;
            int i;
            List<double> residual = new List<double>();

            for (i = 0; i < maxN; i++)
            {
                //B = E - t * MInv * A
                //d = (x - tmp).Norm() / (1 - B.Norm()) - оценка погрешности. Очень странная ф-ла, т.к. знаменатель может быть меньше нуля. Мб нужна операторная норма?
                r = A * x - b;
                residual.Add(r.Norm());
                if (residual[i] < e)
                    break;
                //tmp = x.Copy();
                x = x - t * MInv * r;
                //if ((x - tmp).Norm() < e) - сходимость по соседним решениям заменена на сходимость по невязке
                //break;
            }

            return new Solution(x, i, residual);
        }

        public static Solution PreconditionedSteepestDescent(Matrix A, Matrix b, Matrix M, double e, int maxN)
        {
            Matrix MInv = LowTrInverse(M);

            //Matrix tmp;
            Matrix x = b.Copy();
            Matrix r;
            double t;
            int i;
            List<double> residual = new List<double>();

            for (i = 0; i < maxN; i++)
            {
                r = A * x - b;
                residual.Add(r.Norm());
                if (residual[i] < e)//Сходимость по невязке
                    break;
                t = r.DotProduct(MInv * r) / ((A * MInv * r).DotProduct(MInv * r));
                //tmp = x.Copy();
                x = x - t * MInv * r;
                //if ((x - tmp).Norm() < e) - сходимость по соседним решениям заменена на сходимость по невязке
                //break;
            }

            return new Solution(x, i, residual);
        }

        //Имеется в виду разложение матрицы A = L + D + U, гдн L содержит элементы под главной диагональю.
        public static Matrix GetL(Matrix A)
        {
            if (A.rows != A.cols)
                throw new Exception("Matrix is not square");

            Matrix L = new Matrix(A.rows, A.cols);

            for (int i = 0; i < A.rows; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    L.values[i, j] = A.values[i, j];
                }
            }

            return L;
        }


        //Имеется в виду разложение матрицы A = L + D + U, где D содержит диагональные элементы
        public static Matrix GetD(Matrix A)
        {
            if (A.rows != A.cols)
                throw new Exception("Matrix is not square");

            Matrix D = new Matrix(A.rows, A.cols);

            for (int i = 0; i < A.rows; i++)
            {
                D.values[i, i] = A.values[i, i];
            }

            return D;
        }


        //Метод для обращения нижнетреугольной матрицы
        //Проблема: необходимо, чтобы не было нулей на главной диагонали.
        public static Matrix LowTrInverse(Matrix A) 
        {
            //Нужна ли проверка на то, что матрица является нижнетреугольной?
            Matrix AInv = new Matrix(A.rows, A.cols);

            for (int i = 0; i < A.rows; i++)
            {
                AInv.values[i, i] = 1 / A.values[i, i];
                for (int j = 0; j < i; j++)
                {
                    for (int k = 0; k < i; k++)
                    {
                        AInv.values[i, j] += AInv.values[k, j] * A.values[i, k];
                    }
                    AInv.values[i, j] *= (-1 / A.values[i, i]);
                }
            }

            return AInv;
        }
    }
}

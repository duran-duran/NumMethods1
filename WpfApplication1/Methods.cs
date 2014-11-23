using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Methods//Необходимо пофиксить проверки на сходимость, нужна оценка погрешности результата (или смотреть невязку?)
    {
        public static Solution JacobiMethod(Matrix A, Matrix b, double e, int maxN) 
        {
            Matrix x = new Matrix(A.rows, 1);
            Matrix E = new Matrix(A.rows, A.cols); E.ToIdentityMatrix();

            Matrix tmp = A.Copy();
            A = A.Transpose() * A;
            b = tmp.Transpose() * b;
            
            double S = 1.10 * A.Norm();  //Некоторое число, большее нормы A
            double t = 2 / S;

            tmp = b.Copy();
            int i;

            for (i = 0; i < maxN; i++)
            {
                x = (E - t * A) * tmp + t * b;
                if ((x - tmp).Norm() < e)
                    break;
                tmp = x.Copy();
            }

            return new Solution(x, i);
        }

        //По-хорошему надо бы все запихнуть в один метод и просто передавать параметром матрицу-preconditioner
        public static Solution SOR(Matrix A, Matrix b, double t, double e, int maxN)
        {
            Matrix x = new Matrix(A.rows, 1);
            Matrix E = new Matrix(A.rows, A.cols); E.ToIdentityMatrix();

            Matrix tmp = A.Copy();
            A = A.Transpose() * A;
            b = tmp.Transpose() * b;

            Matrix L = GetL(A); Matrix D = GetD(A);
            Matrix M = D + t * L;
            Matrix MInv = LowTrInverse(M);           
            
            tmp = b.Copy();
            int i;

            for (i = 0; i < maxN; i++)
            {
                x = (E - t * MInv * A) * tmp + t * MInv * b;
                if ((x - tmp).Norm() < e)
                    break;
                tmp = x.Copy();
            }

            return new Solution(x, i);
        }

        public static void SteepestDescent(Matrix A, Matrix b)
        {
        }

        //Имеется в виду разложение матрицы A = L + D + U, гдн L содержит элементы под главной диагональю.
        public static Matrix GetL(Matrix A)
        {
            if (A.rows != A.cols)
                throw new Exception("Matrix is not square");

            Matrix L = new Matrix(A.rows, A.cols);

            for (int i = 0; i < A.rows; i++)
            {
                for (int j = 0; j < i + 1; j++)
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

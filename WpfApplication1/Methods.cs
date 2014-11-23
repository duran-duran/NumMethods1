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
            Matrix x = new Matrix(A.rows, 1);

            Matrix tmp = b.Copy();
            Matrix MInv = LowTrInverse(M);
            Matrix r;
            int i;

            for (i = 0; i < maxN; i++)
            {
                //B = E - t * MInv * A
                //d = (x - tmp).Norm() / (1 - B.Norm()) - оценка погрешности. Очень странная ф-ла, т.к. знаменатель может быть меньше нуля. Мб нужна операторная норма?
                r = A * tmp - b;
                if (r.Norm() < e)//if ((x - tmp).Norm() < e) - сходимость по соседним решениям заменена на сходимость по невязке
                    break;
                x = tmp - t * MInv * r;
                tmp = x.Copy();
            }

            return new Solution(x, i);
        }

        //По-хорошему надо бы все запихнуть в один метод и просто передавать параметром матрицу-preconditioner
        /*public static Solution SOR(Matrix A, Matrix b, double t, double e, int maxN)
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
        }*/

        public static Solution PreconditionedSteepestDescent(Matrix A, Matrix b, Matrix M, double e, int maxN)
        {
            Matrix x = new Matrix(A.rows, 1);

            Matrix tmp = b.Copy();
            Matrix MInv = LowTrInverse(M);
            Matrix r;
            double t;
            int i;

            for (i = 0; i < maxN; i++)
            {
                r = A * tmp - b;
                if (r.Norm() < e)//Сходимость по невязке
                    break;
                t = r.DotProduct(MInv * r) / ((A * MInv * r).DotProduct(MInv * r));//t = r.DotProduct(r) / ((A * r).DotProduct(r));
                x = tmp - t * MInv * r;//x = tmp - t * r;
                tmp = x.Copy();
            }

            return new Solution(x, i);
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

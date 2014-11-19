﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class Matrix
    {
        public int rows;
        public int cols;
        public double[,] values;


        public Matrix(int rowCount, int colCount)
        {
            rows = rowCount;
            cols = colCount;
            values = new double[rows, cols];
        }


        public Matrix(List<double[]> view)
        {
            rows = view.Count;
            cols = view[0].Length;
            values = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    values[i, j] = view[i][j];
                }
            }
        }


        public Matrix Copy()
        {
            Matrix res = new Matrix(this.rows, this.cols);

            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    res.values[i, j] = this.values[i, j];
                }
            }

            return res;
        }

        public void Randomize(double min, double max)
        {
            if (max < min)
            {
                double tmp = max;
                max = min;          //Своп в случае, если максимум и минимум введены в обратном порядке
                min = tmp;
            }

            Random random = new Random();

            if ((min == (int)min) && (max == (int)max))
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        values[i, j] = random.Next((int)min, (int)max + 1);
                    }
                }
            }
            else
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        values[i, j] = random.NextDouble() * (max - min) + min;
                    }
                }
            }
        }


        public void ToIdentityMatrix()
        {
            if (this.rows != this.cols)
                throw new Exception("Matrix is not square");

            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    if (i == j)
                        this.values[i, j] = 1;
                    else
                        this.values[i, j] = 0;
                }
            }
        }


        public static Matrix operator +(Matrix A, Matrix B)
        {
            if ((A.rows != B.rows) || (A.cols != B.cols))
                throw new Exception("Matrices' sizes are not equal");
            Matrix C = new Matrix(A.rows, B.cols);

            for (int i = 0; i < A.rows; i++)
            {
                for (int j = 0; j < A.cols; j++)
                {
                    C.values[i, j] = A.values[i, j] + B.values[i, j];
                }
            }

            return C;
        }


        public static Matrix operator -(Matrix A, Matrix B)
        {
            if ((A.rows != B.rows) || (A.cols != B.cols))
                throw new Exception("Matrices' sizes are not equal");
            Matrix C = new Matrix(A.rows, B.cols);

            for (int i = 0; i < A.rows; i++)
            {
                for (int j = 0; j < A.cols; j++)
                {
                    C.values[i, j] = A.values[i, j] - B.values[i, j];
                }
            }

            return C;
        }


        public static Matrix operator *(double t, Matrix A)
        {
            Matrix res = new Matrix(A.rows, A.cols);

            for (int i = 0; i < A.rows; i++)
            {
                for (int j = 0; j < A.cols; j++)
                {
                    res.values[i, j] = t * A.values[i, j];
                }
            }

            return res;
        }


        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.cols != B.rows)
                throw new Exception("Matrices' sizes are inappropriate");
            Matrix C = new Matrix(A.rows, B.cols);

            for (int i = 0; i < A.rows; i++)
            {
                for (int j = 0; j < B.cols; j++)
                {
                    for (int k = 0; k < A.cols; k++)
                    {
                        C.values[i, j] += A.values[i, k] * B.values[k, j];
                    }
                }
            }

            return C;
        }


        public Matrix Transpose()
        {
            Matrix res = new Matrix(this.cols, this.rows);

            for (int i = 0; i < this.cols; i++)
            {
                for (int j = 0; j < this.rows; j++)
                {
                    res.values[i, j] = this.values[j, i];
                }
            }

            return res;
        }


        public double Dot(Matrix b)
        {
            if ((this.cols != 1) || (b.cols != 1))
                throw new Exception("Operands are not vectors");
            if (this.rows != b.rows)
                throw new Exception("Vectors' lengths are not equal");
            double sum = 0;

            for (int i = 0; i < this.rows; i++)
                sum += this.values[i, 0] * this.values[i, 0];

            return sum;
        }


        public double Norm()
        {
            double maxSum = 0;
            double rowSum;

            for (int i = 0; i < this.rows; i++)
            {
                rowSum = 0;
                for (int j = 0; j < this.cols; j++)
                {
                    rowSum += Math.Abs(this.values[i, j]);
                }
                if (rowSum > maxSum)
                    maxSum = rowSum;
            }

            return maxSum;
        }


        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    result += this.values[i, j] + " ";
                }
                result += Environment.NewLine;
            }

            return result;
        }


        public Matrix ToLowTr() //В методе верхней релаксации не просто нижняя треугольная, а M = L + t * D !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            if (this.rows != this.cols)
                throw new Exception("Matrix is not square");

            Matrix L = new Matrix(this.rows, this.cols);

            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    if (i >= j)
                        L.values[i, j] = this.values[i, j];
                }
            }

            return L;
        }
    }
}

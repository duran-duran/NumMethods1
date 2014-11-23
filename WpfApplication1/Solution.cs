using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    //Хреново назвал свойства класса. Надо бы переименовать
    class Solution
    {
        public Matrix vector;
        public int ItNum;

        public Solution()
        {
        }

        public Solution(Matrix x, int n)
        {
            vector = x.Copy();
            ItNum = n;
        }

        //Внутренний цикл по идее не нужен, т.к. решение - это всегда вектор-столбец.
        //Нужно поюзать какой-нибудь нормальный StringFormat, чтобы выровнять числа
        public string ToString(int n)
        {
            string result = "";

            for (int i = 0; i < vector.rows; i++)
            {
                result += "x" + (i + 1) + " = ";
                for (int j = 0; j < vector.cols; j++)
                {
                    result += vector.values[i, j].ToString("F" + n) + " ";
                }
                result += Environment.NewLine;
            }

            result = result.Remove(result.LastIndexOf(Environment.NewLine));  //Удаляем последний символ переноса, чтобы Label нормально отцентрировался

            return result;
        } 
    }
}

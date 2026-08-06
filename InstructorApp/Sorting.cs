using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathQuestionChallenge
{
    internal class Sorting
    {
        /// <summary>
        /// Method:     BubbleSortAscending()
        /// Desc:       These sorting methods are a combination of the sorting methods 
        ///             provided in previous classes and the following book:
        ///             "C# Data Structures and Algorithms - Marcin Jamro ~ Pg.69-77"
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static MathQuestion[] BubbleSortAscending(List<MathQuestion> list)
        {
            MathQuestion[] arr = list.ToArray();
            int n = arr.Length;

            for (int i = 0; i < n - 1; i++)
            {
                bool swapped = false;
                for (int j = 0; j < n - i - 1; j++)
                {
                    // Compare by the Answer property
                    if (arr[j].Answer > arr[j + 1].Answer)
                    {
                        MathQuestion temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                        swapped = true;
                    }
                }
                if (!swapped) break;
            }
            return arr;
        }

        // 2. BUBBLE SORT - DESCENDING
        public static MathQuestion[] BubbleSortDescending(List<MathQuestion> list)
        {
            MathQuestion[] arr = list.ToArray();
            int n = arr.Length;

            for (int i = 0; i < n - 1; i++)
            {
                bool swapped = false;
                for (int j = 0; j < n - i - 1; j++)
                {
                    // Descending: swap if current is LESS than next
                    if (arr[j].Answer < arr[j + 1].Answer)
                    {
                        MathQuestion temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                        swapped = true;
                    }
                }
                if (!swapped) break;
            }
            return arr;
        }

        // Insertion Sort (Ascending by Answer)
        public static MathQuestion[] InsertionSortAscending(List<MathQuestion> list)
        {
            MathQuestion[] arr = list.ToArray();
            int n = arr.Length;

            for (int i = 1; i < n; i++)
            {
                MathQuestion key = arr[i];
                int j = i - 1;

                // Notice '< key.Answer' for Descending order
                while (j >= 0 && arr[j].Answer > key.Answer)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                arr[j + 1] = key;
            }
            return arr;
        }
    }
}

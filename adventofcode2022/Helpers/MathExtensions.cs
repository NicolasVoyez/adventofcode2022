using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace adventofcode2022.Helpers
{
    internal static class MathExtensions
    {
        public static int PPCM (this int i, int j)
        {
            int c = Math.Max(i, j);
            while (c % i != 0 || c % j != 0)
                c++;

            return c;
        }
        public static int PPCM(this IList<int> numbers)
        {
            var ppcm = numbers[0];
            for (int i = 1; i < numbers.Count; i++)
            {
                ppcm = ppcm.PPCM(numbers[i]);
            }
            return ppcm;
        }
    }
}

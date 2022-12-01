using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Helpers
{
    public static class StringExtensions
    {
        public static IEnumerable<int> SplitAsInt(this string current, string separator = " ")
        {
            return current.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
        }
        public static string[] SplitREE(this string current, string separator = " ")
        {
            return current.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}

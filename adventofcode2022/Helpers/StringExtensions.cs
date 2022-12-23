using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Helpers
{
    public static class StringExtensions
    {
        public static IList<int> SplitAsInt(this string current, string separator = " ")
        {
            return current.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        }
        public static string[] SplitREE(this string current, string separator = " ")
        {
            return current.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}

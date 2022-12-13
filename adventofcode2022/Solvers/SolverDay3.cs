using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.Solvers
{
    class SolverDay3 : ISolver
    {
        List<(string, string)> _rucksacks = new List<(string, string)>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var currentLine in splitContent)
            {
                _rucksacks.Add((currentLine.Substring(0, currentLine.Length / 2), currentLine.Substring(currentLine.Length / 2)));
            }
        }

        public int GetValue(char c)
        {
            if (char.IsUpper(c))
            {
                return (int)c - 65 + 27;
            }
            else
                return (int)c - 97 + 1;
        }

        public string SolveFirstProblem()
        {
            return _rucksacks.Sum(r => GetValue(r.Item1.First(c => r.Item2.Contains(c)))).ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            return _rucksacks.Zip(Enumerable.Range(0, _rucksacks.Count()),
                                (s, r) => new { Group = r / 3, Item = s })
                           .GroupBy(i => i.Group, g => g.Item.Item1 + g.Item.Item2)
                           .Select(g => GetValue(g.ElementAt(0).First(c => g.ElementAt(1).Contains(c) && g.ElementAt(2).Contains(c))))
                           .Sum().ToString();

        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

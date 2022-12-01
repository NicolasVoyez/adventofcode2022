using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2022.Solvers
{
    class SolverDay1 : ISolver
    {
        private List<List<int>> _caloriesByElf = new List<List<int>>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            var currentElf = new List<int>();
            foreach (var currentLine in splitContent)
            {
                if (string.IsNullOrEmpty(currentLine))
                {
                    if (currentElf.Count > 0)
                        _caloriesByElf.Add(currentElf);
                    currentElf = new List<int>();
                }
                if (int.TryParse(currentLine.Trim(), out var current))
                {
                    currentElf.Add(current);
                }
            }

            if (currentElf.Count > 0)
                _caloriesByElf.Add(currentElf);
        }

        public string SolveFirstProblem()
        {
            return _caloriesByElf.Max(foods => foods.Sum()).ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            return _caloriesByElf.Select(foods => foods.Sum()).OrderByDescending(c => c).Take(3).Sum().ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

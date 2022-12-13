using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Solvers
{
    class SolverDay6 : ISolver
    {
        private string _input;
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var currentLine in splitContent)
            {
                _input = currentLine;
            }
        }

        public string SolveFirstProblem()
        {
            for (int i = 0; i < _input.Length-5; i++)
            {
                var a = _input[i]; 
                var b = _input[i+1];
                var c = _input[i+2];    
                var d = _input[i+3];
                if (a != b && a != c && a != d && b != c && b != d && c != d)
                    return (i+4).ToString();
            }
            return "not found";
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            for (int i = 0; i < _input.Length - 15; i++)
            {
                if (_input.Skip(i).Take(14).Distinct().Count() == 14)
                    return (i + 14).ToString();

            }
            return "not found";
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

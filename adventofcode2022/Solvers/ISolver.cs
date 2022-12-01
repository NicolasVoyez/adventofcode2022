using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2022.Solvers
{
    internal interface ISolver
    {
        public void InitInput(string content);

        public string SolveFirstProblem();


        public string SolveSecondProblem(string firstProblemSolution);
        bool Question2CodeIsDone { get; }
        bool TestOnly { get; }
    }
}

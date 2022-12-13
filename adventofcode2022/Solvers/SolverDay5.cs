using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Solvers
{
    class SolverDay5 : ISolver
    {
        struct Instruction {
            public Instruction(int crates, int from, int to)
            {
                Crates = crates;
                From = from;
                To = to;
            }

            public int Crates { get; }
            public int From { get; }
            public int To { get; }
        }

        private List<List<char>> _stacks = new List<List<char>>();
        private List<Instruction> _instructions = new List<Instruction>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            bool instructions = false;
            foreach (var currentLine in splitContent)
            {
                if (string.IsNullOrEmpty(currentLine))
                {
                    instructions = true;
                    continue;
                }
                if (instructions)
                {
                    var split = currentLine.Split(new string[] { "move ", " from ", " to" }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    _instructions.Add(new Instruction(split[0], split[1], split[2]));
                }
                else
                {
                    if (_stacks.Count == 0)
                    {
                        for (int i = 0; i < (currentLine.Length+1) /4; i++)
                            _stacks.Add(new List<char>());
                    }
                    for (int i = 0; i < _stacks.Count; i++)
                    {
                        var curr = currentLine[i * 4 + 1];
                        if (curr != ' ' && char.IsLetter(curr)) 
                        _stacks[i].Add(curr);
                    }
                }
            }
            for (int i = 0; i < _stacks.Count; i++)
                _stacks[i].Reverse();
        }

        public string SolveFirstProblem()
        {
            var s = _stacks.Select(sub => sub.ToList()).ToList();
            foreach(var instruction in _instructions)
            {
                for (int i = 0; i < instruction.Crates; i++)
                {
                    var from = s[instruction.From - 1];
                    s[instruction.To - 1].Add(from[from.Count - 1]);
                    from.RemoveAt(from.Count - 1);
                }
            }
            return String.Join(null, s.Select(st => st.Last()));
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            var s = _stacks.Select(sub => sub.ToList()).ToList();   
            foreach (var instruction in _instructions)
            {
                for (int i = instruction.Crates-1; i >= 0; i--)
                {
                    var from = s[instruction.From - 1];
                    s[instruction.To - 1].Add(from[from.Count - i -1]);
                }
                for (int i = 0; i < instruction.Crates; i ++)
                {
                    var from = s[instruction.From - 1];
                    from.RemoveAt(from.Count - 1);
                }
            }
            return String.Join(null, s.Select(st => st.Last()));
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Solvers
{
    class SolverDay4 : ISolver
    {
        class Assignement
        {
            public int Elf1Min { get; set; }
            public int Elf2Min { get; set; }
            public int Elf1Max { get; set; }
            public int Elf2Max { get; set; }

            public Assignement(string assignement)
            {
                var vals = assignement.Split(new char[] { '-', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                Elf1Min = vals[0];
                Elf1Max = vals[1];
                Elf2Min = vals[2];
                Elf2Max = vals[3];
            }

            public bool Elf1Includes(int min, int max) => Elf1Min >= min && Elf1Max <= max;
            public bool Elf2Includes(int min, int max) => Elf2Min >= min && Elf2Max <= max;
            public bool IsElf1JobIncludedInElf2 => Elf1Includes(Elf2Min, Elf2Max);
            public bool IsElf2JobIncludedInElf1 => Elf1Includes(Elf1Min, Elf1Max);

            public bool Includes (Assignement other)
            {
                return (Elf1Includes(other.Elf1Min, other.Elf1Max) || Elf2Includes(other.Elf1Min, other.Elf1Max)) &&
                       (Elf1Includes(other.Elf2Min, other.Elf2Max) || Elf2Includes(other.Elf2Min, other.Elf2Max));
            }

            public bool ElvesWorkOVerlapse => (Elf1Min >= Elf2Min && Elf1Min <= Elf2Max) || (Elf2Min >= Elf1Min && Elf2Min <= Elf1Max);
        }

        private List<Assignement> _assignements = new List<Assignement>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var currentLine in splitContent)
            {
                _assignements.Add(new Assignement(currentLine));
            }
        }

        public string SolveFirstProblem()
        {
            return _assignements.Count(a => a.IsElf1JobIncludedInElf2 || a.IsElf2JobIncludedInElf1).ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            return _assignements.Count(a => a.ElvesWorkOVerlapse).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

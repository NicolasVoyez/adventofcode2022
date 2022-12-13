using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Solvers
{
    class SolverDay10 : ISolver
    {
        class SimpleCPU
        {
            public List<int> SignalStrength { get; } = new List<int>();
            public List<bool> _pixels { get; } = new List<bool>();
            public int RegisterX { get; set; } = 1;
            public int Cycle { get; set; }

            public void Noop() => IncreaseCycleAndCheckStrength();
            private void IncreaseCycleAndCheckStrength()
            {
                _pixels.Add(Math.Abs(RegisterX - Cycle%40) <= 1);

                Cycle++;
                if (Cycle == 20 || (Cycle > 20 && (Cycle - 20) % 40 == 0))
                    SignalStrength.Add(RegisterX * Cycle);
            }

            public void AddX(int value)
            {
                for (int i = 0; i < 2; i++)
                {
                    IncreaseCycleAndCheckStrength();
                }
                RegisterX += value;
            }
        }

        List<string> _instructions;
        private SimpleCPU _cpu = new SimpleCPU();
        public void InitInput(string content)
        {
            _instructions = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

        }

        public string SolveFirstProblem()
        {
            List<int> _registerValues = new List<int>();
            foreach(var instruction in _instructions)
            {
                if (instruction == "noop")
                    _cpu.Noop();
                else
                {
                    _cpu.AddX(int.Parse(instruction.SplitREE()[1]));
                }

            }

            return _cpu.SignalStrength.Sum().ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            string result = "\r\n";
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 40; x++) {
                    result += _cpu._pixels[y * 40 + x] ? "#" : " ";
                }
                result += "\r\n";
                
            }
            return result;
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

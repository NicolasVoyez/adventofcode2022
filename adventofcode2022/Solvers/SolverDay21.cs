using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2022.Solvers
{
    class SolverDay21 : ISolver
    {
        class Monkey
        {
            public Monkey(string description)
            {
                // either "zngr: 11"
                // or "fvlj: zlfw * gcvj"
                var split = description.Split(new[] {":", " " }, StringSplitOptions.RemoveEmptyEntries);
                Name = split[0];
                if (split.Length == 2)
                    Value = int.Parse(split[1]);
                else
                {
                    Op1 = split[1];
                    ParseOperation(split[2]);
                    Op2 = split[3];
                }
            }

            public string Name { get; set; }
            public BigInteger? Value { get; set; }
            public string Op1 { get; private set; }
            public string Op2 { get; private set; }
            public Func<BigInteger, BigInteger, BigInteger> Operation { get; set; }

            internal void ParseOperation (string op)
            {
                if (op == ("+"))
                    Operation = (v1, v2) => v1 + v2;
                else if (op==("*"))
                    Operation = (v1, v2) => v1 * v2;
                else if (op==("-"))
                    Operation = (v1, v2) => v1 - v2;
                else if (op==("/"))
                    Operation = (v1, v2) => v1 / v2;
            }
        }

        private Monkey root;
        private Dictionary<string, Monkey> _monkeys = new Dictionary<string, Monkey>();
        private HashSet<string> _monkeysWithValue = new HashSet<string>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var currentLine in splitContent)
            {
                var monkey = new Monkey(currentLine);
                if (monkey.Name == "root")
                    root = monkey;
                _monkeys[monkey.Name] = monkey;
                if (monkey.Value.HasValue)
                    _monkeysWithValue.Add(monkey.Name);
            }
        }



        public string SolveFirstProblem()
        {
            return Solve().ToString();
        }

        private BigInteger Solve()
        {
            Cleanup();
            while (!root.Value.HasValue)
            {
                ResolveStep();
            }

            return root.Value.Value;
        }

        private void ResolveStep()
        {
            foreach (var (name, monkey) in _monkeys)
            {
                if (monkey.Value.HasValue)
                    continue;
                if (_monkeysWithValue.Contains(monkey.Op1) && _monkeysWithValue.Contains(monkey.Op2))
                {
                    monkey.Value = monkey.Operation(_monkeys[monkey.Op1].Value.Value, _monkeys[monkey.Op2].Value.Value);
                    _monkeysWithValue.Add(monkey.Name);
                }
            }
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            root.ParseOperation("-");
            BigInteger human = 5;

            while (root.Value.HasValue && root.Value.Value != 0)
            {
                _monkeys["humn"].Value = human;
                var result = Solve();
                if (result == 0)
                    return human.ToString();
                _monkeys["humn"].Value = human +1;
                var r2 = Solve();
                if (r2 == 0)
                    return (human +1).ToString();
                var delta = r2 - result;
                human = human - result / (delta == 0 ? 1 : delta);
            }

            throw new Exception("Not there !");            
        }

        private void Cleanup()
        {
            foreach(var (name,monkey) in _monkeys)
            {
                if (!string.IsNullOrEmpty(monkey.Op1))
                {
                    monkey.Value = null;
                    _monkeysWithValue.Remove(monkey.Name);
                }
            }
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

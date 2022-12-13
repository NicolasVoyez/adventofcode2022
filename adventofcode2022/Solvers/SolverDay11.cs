using adventofcode2022.Helpers;
using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2022.Solvers
{
    class SolverDay11 : ISolver
    {
        class Monkey
        {
            public Monkey(int num)
            {
                Number = num;
            }

            public int Number { get; set; }
            public List<BigInteger> Objects { get; private set; } = new List<BigInteger>();
            public Func<BigInteger, BigInteger> Operation { get; set; }
            public int DivisibleTest { get; set; }
            public int TestPositiveMonkey { get; set; }
            public int TestNegativeMonkey { get; set; }
            public BigInteger Checks { get; private set; }

            public void CheckItems(IDictionary<int,Monkey> otherMonkeys,BigInteger ultimateModulo = default, int divideBy = 3)
            {
                foreach(var item in Objects.ToList())
                {
                    Objects.Remove(item);
                    Checks++;
                    var tested = Operation(item) / divideBy;
                    if (ultimateModulo != default)
                        tested = tested % ultimateModulo;
                    if (tested % DivisibleTest == 0)
                        otherMonkeys[TestPositiveMonkey].Objects.Add(tested);
                    else
                        otherMonkeys[TestNegativeMonkey].Objects.Add(tested);
                }
            }
            public Monkey CopyInitial()
            {
                return new Monkey(Number)
                {
                    Objects = new List<BigInteger>(Objects),
                    Checks = 0,
                    DivisibleTest = DivisibleTest,
                    Operation = Operation,
                    TestNegativeMonkey = TestNegativeMonkey,
                    TestPositiveMonkey = TestPositiveMonkey
                };
            }
        }

        private Dictionary<int, Monkey> _monkeys = new Dictionary<int, Monkey>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            Monkey currentMonkey = null;
            foreach (var currentLine in splitContent)
            {
                if (currentLine.StartsWith("Monkey "))
                {
                    currentMonkey = new Monkey(int.Parse(currentLine[currentLine.Length - 2].ToString()));
                    _monkeys[currentMonkey.Number] = currentMonkey;
                }
                else if (currentLine.StartsWith("  Starting items: "))
                {
                    var items = currentLine.Replace("  Starting items: ", "").SplitAsInt(", ").Select(i => new BigInteger(i));
                    currentMonkey.Objects.AddRange(items);
                }
                else if (currentLine.StartsWith("  Operation: new = "))
                {
                    var op = currentLine.Replace("  Operation: new = ", "");
                    if (op == "old * old")
                        currentMonkey.Operation = (value) => value * value;
                    else
                    {
                        int v = int.Parse(op.Substring(6));
                        if (op.StartsWith("old + "))
                            currentMonkey.Operation = (value) => value + v;
                        else if (op.StartsWith("old * "))
                            currentMonkey.Operation = (value) => value * v;
                        else if (op.StartsWith("old - "))
                            currentMonkey.Operation = (value) => value - v;
                        else if (op.StartsWith("old / "))
                            currentMonkey.Operation = (value) => value / v;
                    }
                }
                else if (currentLine.StartsWith("  Test: divisible by "))
                    currentMonkey.DivisibleTest = int.Parse(currentLine.Replace("  Test: divisible by ", ""));
                else if (currentLine.TrimStart().StartsWith("If true: throw to monkey "))
                    currentMonkey.TestPositiveMonkey = int.Parse(currentLine.TrimStart().Replace("If true: throw to monkey ", ""));
                else if (currentLine.TrimStart().StartsWith("If false: throw to monkey "))
                    currentMonkey.TestNegativeMonkey = int.Parse(currentLine.TrimStart().Replace("If false: throw to monkey ", ""));
            }
        }

        public string SolveFirstProblem()
        {
            var monkeys = new Dictionary<int, Monkey>();
            foreach (var (key, monkey) in _monkeys)
                monkeys[key] = monkey.CopyInitial();
            for (int i = 0; i < 20; i++)
            {
                 foreach(var (key,monkey) in monkeys.OrderBy(kvp => kvp.Key))
                 {
                     monkey.CheckItems(monkeys);
                 }
            }
            var orderedMonkeys = monkeys.OrderByDescending(kvp => kvp.Value.Checks).ToList();
            return (orderedMonkeys[0].Value.Checks * orderedMonkeys[1].Value.Checks).ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            var monkeys = new Dictionary<int, Monkey>();
            foreach (var (key, monkey) in _monkeys)
                monkeys[key] = monkey.CopyInitial();
            BigInteger ultimateModulo = monkeys.Values.Select(m => m.DivisibleTest).ToList().PPCM();
            for (int i = 0; i < 10000; i++)
            {
                foreach (var (key, monkey) in monkeys.OrderBy(kvp => kvp.Key))
                {
                    monkey.CheckItems(monkeys, ultimateModulo, 1);
                }
            }
            var orderedMonkeys = monkeys.OrderByDescending(kvp => kvp.Value.Checks).ToList();
            return (orderedMonkeys[0].Value.Checks * orderedMonkeys[1].Value.Checks).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

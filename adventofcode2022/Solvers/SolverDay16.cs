using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2022.Solvers
{
    class SolverDay16 : ISolver
    {
        class Valve
        {
            public Valve(string description)
            {
                //Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
                var split = description.Replace("valves", "valve").Replace("tunnels", "tunnel").Replace("leads", "lead")
                    .Split(new[] { "Valve ", " has flow rate=", "; tunnel lead to valve " }, StringSplitOptions.RemoveEmptyEntries);
                Name = split[0];
                FlowRate = int.Parse(split[1]);
                DirectNeighbors = split[2].SplitREE(", ").ToList();
            }

            public string Name { get; set; }
            public List<string> DirectNeighbors { get; set; }
            public int FlowRate { get; set; }
            // how many empty valves you skipped to get there
            public Dictionary<Valve, int> FlowingNeighbors { get; } = new Dictionary<Valve, int>();

            public override bool Equals(object? obj) => obj is Valve v && Equals(v);
            public bool Equals(Valve oth) => oth.Name == Name;
            public override int GetHashCode() => Name.GetHashCode();
            public override string ToString() => "(" + Name + " " + FlowRate + ")";
        }

        private Valve _start;
        private Dictionary<string, Valve> _valves = new Dictionary<string, Valve>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var currentLine in splitContent)
            {
                var valve = new Valve(currentLine);
                if (valve.Name == "AA")
                    _start = valve;
                _valves[valve.Name] = valve;
            }
            foreach (var (_, valve) in _valves)
            {
                AddFlowingNeighbors(valve);
            }
        }

        private void AddFlowingNeighbors(Valve addTo)
        {
            Dictionary<Valve, int> neighbors = new Dictionary<Valve, int>();
            Stack<(Valve, int)> todo = new Stack<(Valve, int)>();
            todo.Push((addTo, 0));
            while (todo.Count > 0)
            {
                var (current, skipped) = todo.Pop();
                foreach (var name in current.DirectNeighbors)
                {
                    var neighbor = _valves[name];
                    if (neighbors.TryGetValue(neighbor, out var oldSkip) && oldSkip <= skipped)
                        continue;

                    neighbors[neighbor] = skipped;

                    if (neighbor != addTo)
                        todo.Push((neighbor, skipped + 1));
                }
            }

            foreach (var (n, i) in neighbors)
                if (n != addTo && n.FlowRate != 0)
                    addTo.FlowingNeighbors[n] = i;
        }

        public string SolveFirstProblem()
        {
            int remainingTime = 30;
            int currentFlow = 0;
            var alreadyPassed = new HashSet<string>();

            int bestTime = GetBestTime(_start, currentFlow, remainingTime, alreadyPassed);
            return bestTime.ToString();
        }

        private int GetBestTime(Valve current, int currentFlow, int remainingTime, HashSet<string> alreadyPassed)
        {
            int bestFlow = currentFlow;
            var passed = new HashSet<string>(alreadyPassed);
            passed.Add(current.Name);
            foreach (var (next, skip) in current.FlowingNeighbors)
            {
                if (alreadyPassed.Contains(next.Name))
                    continue;
                var nextTime = remainingTime - 2 - skip; // skips + move to it + open
                if (nextTime < 0)
                    continue;
                var nextFlow = currentFlow + (nextTime * next.FlowRate);
                var res = GetBestTime(next, nextFlow, nextTime, passed);
                if (res > bestFlow)
                    bestFlow = res;
            }
            return bestFlow;
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            int remainingTime = 26;
            int currentFlow = 0;
            var alreadyPassed = new HashSet<string>();

            int bestTime = GetBestTime(_start,_start, currentFlow, remainingTime, remainingTime, alreadyPassed);
            return bestTime.ToString();
        }

        private int GetBestTime(Valve c1, Valve c2, int currentFlow, int remainingTime1, int remainingTime2, HashSet<string> alreadyPassed)
        {
            int bestFlow = currentFlow;
            var passed = new HashSet<string>(alreadyPassed);
            passed.Add(c1.Name);
            passed.Add(c2.Name);
            foreach (var (next1, skip1) in c1.FlowingNeighbors)
            {
                if (passed.Contains(next1.Name))
                    continue;
                var nextTime1 = remainingTime1 - 2 - skip1; // skips + move to it + open
                if (nextTime1 < 0)
                    continue;
                var nextFlow1 = currentFlow + (nextTime1 * next1.FlowRate);
                var res1 = GetBestTime(next1, c2, nextFlow1, nextTime1, remainingTime2, passed);
                if (res1 > bestFlow)
                    bestFlow = res1;

                foreach (var (next2, skip2) in c2.FlowingNeighbors)
                {
                    if (next2 == next1)
                        continue;
                    if (passed.Contains(next2.Name))
                        continue;
                    var nextTime2 = remainingTime2 - 2 - skip2; // skips + move to it + open
                    if (nextTime2 < 0)
                        continue;
                    var nextFlow2 = nextFlow1 + (nextTime2 * next2.FlowRate);
                    var res = GetBestTime(next1, next2, nextFlow2, nextTime1, nextTime2, passed);
                    if (res > bestFlow)
                        bestFlow = res;
                }
            }
            return bestFlow;
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

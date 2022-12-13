using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace AdventOfCode2022.Solvers
{
    class SolverDay12 : ISolver
    {
        class PathNode
        {
            public char Height { get; set; }

            public PathNode(char height)
            {
                if (height == 'S')
                {
                    Height = 'a';
                    StartPoint = true;
                    DistanceToStart = 0;
                }
                else
                {
                    if (height == 'E')
                    {
                        Height = 'z';
                        EndPoint = true;
                        QuickPath = true;
                    }
                    else
                        Height = height;
                    DistanceToStart = int.MaxValue;
                }
            }
            public bool StartPoint { get; set; } = false;
            public bool EndPoint { get; } = false;
            public bool QuickPath { get; set; } = false;
            public int DistanceToStart { get; set; }

            public override string ToString()
            {
                return Height + " : " + DistanceToStart;
            }
        }

        private Grid<PathNode> _grid;
        private Grid<PathNode>.Cell<PathNode> _end;

        private string _content;
        public void InitInput(string content)
        {
            _content = content;
        }
        public string SolveFirstProblem()
        {
            _grid = new Grid<PathNode>(_content, c => new PathNode(c));
            _end = _grid.All().First(c => c.Value.EndPoint);
            var _start = _grid.All().First(c => c.Value.StartPoint);
            List<Grid<PathNode>.Cell<PathNode>> todo = new List<Grid<PathNode>.Cell<PathNode>>();
            todo.Add(_start);

            while (todo.Count != 0 && (_end.Value.DistanceToStart == int.MaxValue || todo.Any(t => t.Value.DistanceToStart < _end.Value.DistanceToStart)))
            {
                var current = todo.OrderBy(x => x.Value.DistanceToStart).First();
                todo.Remove(current);
                var dist = current.Value.DistanceToStart + 1;
                foreach (var element in _grid.Around(current.Y, current.X, false))
                {
                    if (current.Value.Height < element.Value.Height + -1)
                        continue;
                    if (element.Value.DistanceToStart > dist)
                    {
                        element.Value.DistanceToStart = dist;
                        todo.Add(element);
                    }
                }
            }

            FlagChildrenOnPerfectPath(_end, n=> n == _start);
            Print();

            return _end.Value.DistanceToStart.ToString();
        }
        private void Print()
        {
            _grid.Print(c =>
            {
                if (c.QuickPath)
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                else if (c.Height == 'b')
                    Console.BackgroundColor = ConsoleColor.DarkBlue;

                if (c.DistanceToStart < 1000)
                    Console.Write(c.Height);
                else
                    Console.Write("#");

                Console.BackgroundColor = ConsoleColor.Black;
            });
        }

        private bool FlagChildrenOnPerfectPath(Grid<PathNode>.Cell<PathNode> current, Predicate<Grid<PathNode>.Cell<PathNode>> endCondition)
        {
            foreach (var element in _grid.Around(current.Y, current.X, false))
            {
                if (current.Value.DistanceToStart != element.Value.DistanceToStart + 1 ||
                    current.Value.Height - 1 > element.Value.Height)
                    continue;
                if (endCondition(element))
                {
                    element.Value.QuickPath = true;
                    return true;
                }

                if (FlagChildrenOnPerfectPath(element, endCondition))
                {
                    element.Value.QuickPath = true;
                    return true;
                }
            }
            return false;
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            _grid = new Grid<PathNode>(_content, c => new PathNode(c));
            _end = _grid.All().First(c => c.Value.EndPoint);
            var starts = _grid.All().Where(c => c.Value.Height == 'a');
            foreach (var start in starts)
                start.Value.StartPoint = true;
            List<Grid<PathNode>.Cell<PathNode>> todo = new List<Grid<PathNode>.Cell<PathNode>>();
            todo.AddRange(starts);

            while (todo.Count != 0 && (_end.Value.DistanceToStart == int.MaxValue || todo.Any(t => t.Value.DistanceToStart < _end.Value.DistanceToStart)))
            {
                var current = todo.OrderBy(x => x.Value.DistanceToStart).First();
                todo.Remove(current);
                if (current.Value.DistanceToStart >= _end.Value.DistanceToStart)
                    continue;
                var dist = current.Value.DistanceToStart + 1;
                foreach (var element in _grid.Around(current.Y, current.X, false))
                {
                    if (current.Value.Height < element.Value.Height - 1)
                        continue;
                    if (element.Value.DistanceToStart > dist)
                    {
                        element.Value.DistanceToStart = dist;
                        todo.Add(element);
                    }
                }
            }

            FlagChildrenOnPerfectPath(_end, n => n.Value.StartPoint);
            Print();

            return _end.Value.DistanceToStart.ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

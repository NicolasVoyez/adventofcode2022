using adventofcode2022.Helpers;
using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace AdventOfCode2022.Solvers
{
    class SolverDay18 : ISolver
    {
        internal HashSet<Point3D> _lava = new HashSet<Point3D>();
        internal int _minX = 0;
        internal int _maxX = 0;
        internal int _minY = 0;
        internal int _maxY = 0;
        internal int _minZ = 0;
        internal int _maxZ = 0;
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var currentLine in splitContent)
            {
                var s = currentLine.SplitAsInt(",");
                _lava.Add(new Point3D(s[0], s[1], s[2]));
            }
            _minX = _lava.Min(p => p.X);
            _maxX = _lava.Max(p => p.X);
            _minY = _lava.Min(p => p.Y);
            _maxY = _lava.Max(p => p.Y);
            _minZ = _lava.Min(p => p.Z);
            _maxZ = _lava.Max(p => p.Z);
        }

        public string SolveFirstProblem()
        {
            int sum = 0;
            foreach (var pt in _lava)
            {
                sum += 6 - _lava.Count(p => p.ManhattanDistance(pt) == 1);
            }

            return (sum.ToString());
        }

        class AirBubble
        {
            public AirBubble(Point3D start)
            {
                Air.Add(start);
            }
            public HashSet<Point3D> Air { get; } = new HashSet<Point3D>();
            public bool IsSealed { get; private set; } = true;

            public void Grow(SolverDay18 game)
            {
                Stack<Point3D> todo = new Stack<Point3D>();
                todo.Push(Air.First());
                while (todo.Count > 0)
                {
                    var current = todo.Pop();

                    foreach (var p in current.Neighbors)
                    {
                        if (Air.Contains(p))
                            continue;
                        if (p.X > game._maxX || p.Y > game._maxY || p.Z > game._maxZ || p.X < game._minX || p.Y < game._minY || p.Z < game._minZ)
                            IsSealed = false;
                        else if (!game._lava.Contains(p))
                        {
                            Air.Add(p);
                            todo.Push(p);
                        }
                    }
                }
            }
        }

        private List<AirBubble> _bubbles = new List<AirBubble>();
        public string SolveSecondProblem(string firstProblemSolution)
        {
            // generate bubbles
            for (int x = _minX; x <= _maxX + 1; x++)
                for (int y = _minY; y <= _maxY + 1; y++)
                    for (int z = _minZ; z <= _maxZ + 1; z++)
                    {
                        var p = new Point3D(x, y, z);
                        if (!_lava.Contains(p) && !_bubbles.Any(b => b.Air.Contains(p)))
                        {
                            var b = new AirBubble(p);
                            b.Grow(this);
                            _bubbles.Add(b);
                        }
                    }

            var invalidNeighbors = new HashSet<Point3D>();
            foreach(var pt in _bubbles.Where(b => b.IsSealed).SelectMany(b => b.Air))
                invalidNeighbors.Add(pt);

            int sum = 0;
            foreach (var pt in _lava)
            {
                sum += pt.Neighbors.Count(n => !invalidNeighbors.Contains(n) && ! _lava.Contains(n));
            }
            return (sum.ToString());
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

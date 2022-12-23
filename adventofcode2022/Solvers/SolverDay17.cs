using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AdventOfCode2022.Solvers
{
    class SolverDay17 : ISolver
    {
        class Rock
        {
            public List<Point> Points { get; private set; }
            public int Height { get; private set; }

            public static Rock GetLine(int yMin, int xMin = 2)
            {
                return new Rock()
                {
                    Points = new List<Point>
                    {
                        new Point(yMin, xMin),
                        new Point(yMin, xMin + 1),
                        new Point(yMin, xMin + 2),
                        new Point(yMin, xMin + 3)
                    },
                    Height = 1
                };
            }
            public static Rock GetCross(int yMin, int xMin = 2)
            {
                return new Rock()
                {
                    Points = new List<Point>
                    {
                        new Point(yMin, xMin +1),
                        new Point(yMin +1, xMin),
                        new Point(yMin +1, xMin + 1),
                        new Point(yMin +1, xMin + 2),
                        new Point(yMin +2, xMin + 1)
                    },
                    Height = 3
                };
            }
            public static Rock GetCorner(int yMin, int xMin = 2)
            {
                return new Rock()
                {
                    Points = new List<Point>
                    {
                        new Point(yMin+2, xMin +2),
                        new Point(yMin+1, xMin +2),
                        new Point(yMin, xMin +2),
                        new Point(yMin, xMin +1),
                        new Point(yMin, xMin)
                    },
                    Height = 3
                };
            }
            public static Rock GetVerticalLine(int yMin, int xMin = 2)
            {
                return new Rock()
                {
                    Points = new List<Point>
                    {
                        new Point(yMin, xMin),
                        new Point(yMin +1, xMin),
                        new Point(yMin +2, xMin),
                        new Point(yMin +3, xMin)
                    },
                    Height = 4
                };
            }
            public static Rock GetSquare(int yMin, int xMin = 2)
            {
                return new Rock()
                {
                    Points = new List<Point>
                    {
                        new Point(yMin   , xMin),
                        new Point(yMin +1, xMin),
                        new Point(yMin +1, xMin +1),
                        new Point(yMin   , xMin +1)
                    },
                    Height = 2
                };
            }

            public bool TryGoDirection(Direction dir, HashSet<Point> occupiedPositions, out Rock newPosition, int maxX = 7)
            {
                newPosition = this;
                var newPoints = new List<Point>();
                switch (dir)
                {
                    case Direction.Right:
                        foreach (var point in Points)
                        {
                            var n = point.ToDirection(Direction.Right);
                            if (n.X >= maxX)
                                return false;
                            if (occupiedPositions.Contains(n))
                                return false;
                            newPoints.Add(n);
                        }
                        break;
                    case Direction.Left:
                        foreach (var point in Points)
                        {
                            var n = point.ToDirection(Direction.Left);
                            if (n.X < 0)
                                return false;
                            if (occupiedPositions.Contains(n))
                                return false;
                            newPoints.Add(n);
                        }
                        break;
                    case Direction.Up:
                        throw new Exception("Never gonna give it up !");
                    case Direction.Down:
                        foreach (var point in Points)
                        {
                            var n = point.ToDirection(Direction.Down);
                            if (n.Y < 0 || occupiedPositions.Contains(n))
                                return false;
                            newPoints.Add(n);
                        }
                        break;
                }
                newPosition = new Rock
                {
                    Points = newPoints,
                    Height = Height
                };
                return true;
            }
        }
        private int _maxY = -1;

        private List<Direction> _wind = new List<Direction>();
        private HashSet<Point> _allPoints = new HashSet<Point>();
        private List<Point> _allListedPoints = new List<Point>();
        public void InitInput(string content)
        {
            foreach (var c in content)
            {
                if (c == '>')
                    _wind.Add(Direction.Right);
                else if (c == '<')
                    _wind.Add(Direction.Left);
            }
        }

        public string SolveFirstProblem()
        {
            for (int i = 0; i < 2022; i++)
            {
                Fall(i);
            }
            Print(12, false);
            Console.WriteLine();

            return (_maxY +1).ToString();
        }
        public string SolveSecondProblem(string firstProblemSolution)
        {
            BigInteger maxI = 1000000000000L;
            BigInteger skippedSize = 0;
            for (BigInteger i = 2022; i < maxI; i++)
            {
                var saveFoot = _magicFootprintRange.Item2 ==  -1;
                Fall(i, saveFoot);
                if (saveFoot != (_magicFootprintRange.Item2 == -1))
                {
                    var runsToSkip = (maxI - i) / _magicFootprintRange.Item1;
                    skippedSize = runsToSkip * _magicFootprintRange.Item2;
                    maxI -= runsToSkip * _magicFootprintRange.Item1;
                }
            }

            return (skippedSize + _maxY + 1).ToString();
        }

        // old i / old y
        private (BigInteger, int) _magicFootprintRange = (-1,-1);
        Dictionary<string, (BigInteger, int)> _footprints = new Dictionary<string, (BigInteger, int)>();
        private void Fall(BigInteger i, bool saveFootprint = false, bool shouldPrint = false)
        {
            var currentForm = GetNextForm((int)(i % 5), _maxY + 4);
            bool stillFall = true;
            while (stillFall)
            {
                currentForm.TryGoDirection(GetNextWind(), _allPoints, out currentForm);
                if (!currentForm.TryGoDirection(Direction.Down, _allPoints, out currentForm))

                {
                    foreach (var pt in currentForm.Points)
                        _allPoints.Add(pt);

                    _maxY = Math.Max(_maxY, currentForm.Points.Max(p => p.Y));
                    stillFall = false;

                    if (saveFootprint)
                        SaveFootprint(i);
                    if (shouldPrint)
                        Print();
                }
            }
        }

        private void SaveFootprint(BigInteger i)
        {
            if (i % 5 != 0)
                return;
            StringBuilder sb = new StringBuilder();
            for (int y = _maxY; y >= _maxY - 15; y--)
            {
                for (int x = 0; x < 7; x++)
                {
                    sb.Append(_allPoints.Contains(new Point(y, x)) ? '#' : ' ');
                }
            }
            var key = sb.ToString();
            if (_footprints.TryGetValue(key, out var old))
                _magicFootprintRange = (i - old.Item1, _maxY - old.Item2);
            else
                _footprints[key] = (i, _maxY);
        }

        private void Print(int maxLines = 50, bool clear = true)
        {
            if (clear)
                Console.Clear();
            for (int y = _maxY; y >= Math.Max(0, _maxY - maxLines); y--)
            {
                if (y >= 1000)
                    Console.Write(y);
                else if (y >= 100)
                    Console.Write(y + " ");
                else if (y >= 10)
                    Console.Write(y + " ");
                else
                    Console.Write(y + "  ");
                Console.Write("|");
                for (int x = 0; x < 7; x++)
                {
                    if (_allPoints.Contains(new Point(y, x)))
                        Console.Write('#');
                    else
                        Console.Write(' ');
                }
                Console.WriteLine("|");
            }
            if (_maxY <= maxLines)
                Console.WriteLine("    .-------.");
            else 
                Console.WriteLine("    |.......|");
        }

        private int _currentWind = 0;
        private Direction GetNextWind()
        {
            var wind = _wind[_currentWind];
            _currentWind++;
            if (_currentWind >= _wind.Count)
                _currentWind = 0;
            return wind;
        }
        private Rock GetNextForm(int i, int y)
        {
            switch (i % 5)
            {
                case 0:
                    return Rock.GetLine(y);
                case 1:
                    return Rock.GetCross(y);
                case 2:
                    return Rock.GetCorner(y);
                case 3:
                    return Rock.GetVerticalLine(y);
                case 4:
                    return Rock.GetSquare(y);
            }
            throw new NotImplementedException("Modulo magic failed me");
        }


        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
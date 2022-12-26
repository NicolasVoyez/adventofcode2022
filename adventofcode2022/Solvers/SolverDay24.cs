using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdventOfCode2022.Solvers
{
    class SolverDay24 : ISolver
    {
        private List<List<Point>> _allWindPosition = new List<List<Point>>();
        private List<Point> _InitialWinds = new List<Point>();
        private List<Direction> _InitialWindsDirection = new List<Direction>();
        private Point _startPosition = new Point(0, 1);
        private int _minX = 1;
        private int _minY = 1;
        private int _maxX;
        private int _maxY;
        private Point _endPosition;
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            _maxY = splitContent.Length - 2;
            _maxX = splitContent[1].Length - 2;
            _endPosition = new Point(_maxY +1, _maxX);
            for (int y = _minY; y <= _maxY; y++)
            {
                for (int x = _minX; x <= _maxX; x++)
                {
                    var curr = splitContent[y][x];
                    if (curr == '.' || curr == '#')
                        continue;
                    if (curr == '>')
                        _InitialWindsDirection.Add(Direction.Right);
                    else if (curr == '<')
                        _InitialWindsDirection.Add(Direction.Left);
                    else if (curr == '^')
                        _InitialWindsDirection.Add(Direction.Up);
                    else if (curr == 'v')
                        _InitialWindsDirection.Add(Direction.Down);
                    else if (curr == '#')
                        continue;
                    else
                        throw new Exception("WTF !");
                    _InitialWinds.Add(new Point(y, x));
                }
            }

            _allWindPosition.Add(_InitialWinds);
            for (int times = 1; times < _maxX * _maxY; times++)
                _allWindPosition.Add(new List<Point>());

            for (int wi = 0; wi < _InitialWinds.Count; wi++)
            {
                var myDirection = _InitialWindsDirection[wi];
                for (int times = 1; times < _allWindPosition.Count; times++)
                {
                    var newPos = _allWindPosition[times - 1][wi].ToDirection(myDirection);
                    if (newPos.X > _maxX)
                        newPos = new Point(newPos.Y, _minX);
                    if (newPos.Y > _maxY)
                        newPos = new Point(_minY, newPos.X);
                    if (newPos.X < _minX)
                        newPos = new Point(newPos.Y, _maxX);
                    if (newPos.Y < _minY)
                        newPos = new Point(_maxY, newPos.X);
                    _allWindPosition[times].Add(newPos);
                }
            }
        }

        private void Print(int round, List<Point> winds, Point position, IEnumerable<Point> visited = null)
        {
            Console.WriteLine("Round " + round);
            Console.WriteLine();
            if (visited == null)
                visited = new List<Point>();

            for (int x = 0; x <= _maxX + 1; x++)
            {
                var c = new Point(0, x);
                if (visited.Contains(c))
                    Console.BackgroundColor = ConsoleColor.DarkBlue;

                if (position.Equals(c))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("X");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (_startPosition.Equals(c))
                    Console.Write('S');
                else
                    Console.Write('#');
                Console.BackgroundColor = ConsoleColor.Black;
            }
            Console.WriteLine();

            for (int y = 1; y <= _maxY; y++)
            {
                Console.Write('#');
                for (int x = 1; x <= _maxX; x++)
                {
                    var c = new Point(y, x);
                    if (visited.Contains(c))
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                    var windsAt = winds.Where(w => w.Equals(c)).ToList();
                    if (c.Equals(position))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("X");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (windsAt.Count == 0)
                        Console.Write(" ");
                    else if (windsAt.Count > 4)
                        throw new Exception("WTF ?");
                    else if (windsAt.Count > 1)
                        Console.Write(windsAt.Count);
                    else
                    {
                        var idx = winds.IndexOf(c);
                        var dir = _InitialWindsDirection[idx];
                        if (dir == Direction.Right)
                            Console.Write('>');
                        else if (dir == Direction.Left)
                            Console.Write('<');
                        else if (dir == Direction.Up)
                            Console.Write('^');
                        else if (dir == Direction.Down)
                            Console.Write('v');
                    }
                    Console.BackgroundColor = ConsoleColor.Black;

                }
                Console.Write('#');
                Console.WriteLine();
            }
            for (int x = 0; x <= _maxX + 1; x++)
            {
                var c = new Point(_maxY + 1, x);
                if (visited.Contains(c))
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                if (_endPosition.Equals(c))
                    Console.Write('E');
                else
                    Console.Write('#');
                Console.BackgroundColor = ConsoleColor.Black;
            }
            Console.WriteLine();
        }

        public string SolveFirstProblem()
        {
            int bestRound = int.MaxValue;
            HashSet<Point> bestVisited = new HashSet<Point>();
            Queue<(Point, int, HashSet<Point>)> todo = new Queue<(Point, int, HashSet<Point>)>();
            HashSet<(Point, int)> done = new HashSet<(Point, int)>();
            todo.Enqueue((_startPosition, 0, new HashSet<Point> { _startPosition }));
            while (todo.Count != 0)
            {
                var curr = todo.Dequeue();
                if (!done.Add((curr.Item1, curr.Item2 % _allWindPosition.Count)))
                    continue;
                var currRound = curr.Item2 + 1;
                if (currRound > bestRound)
                    continue;

                //Print(curr.Item2,_allWindPosition[curr.Item2 % _allWindPosition.Count], curr.Item1);

                foreach (var pos in curr.Item1.Around(_minX, _minY, _maxX, _maxY +1))
                {
                    if (pos.Equals(_endPosition))
                    {
                        bestRound = currRound;
                        var visited = new HashSet<Point>(curr.Item3);
                        visited.Add(pos);
                        bestVisited = visited;
                        break;
                    }
                    if (pos.Y == _maxY +1 || pos.Y == 0 || pos.X == _maxX + 1 || pos.X == 0)
                        continue; // In a Wall

                    if (!_allWindPosition[currRound % _allWindPosition.Count].Contains(pos))
                    {
                        var visited = new HashSet<Point>(curr.Item3);
                        visited.Add(pos);
                        todo.Enqueue((pos, currRound, visited));
                    }
                }
                if (!_allWindPosition[currRound % _allWindPosition.Count].Contains(curr.Item1))
                    todo.Enqueue((curr.Item1, currRound, curr.Item3)); // wait in place
            }

            Print(bestRound, _allWindPosition[bestRound % _allWindPosition.Count], _endPosition, bestVisited);
            return (bestRound).ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            int bestRound = int.MaxValue;
            Queue<(Point, int, int)> todo = new Queue<(Point, int, int)>(); // position, round , Step (0=go, 1= back, 2= reGo)
            HashSet<(Point, int, int)> done = new HashSet<(Point, int, int)>();
            todo.Enqueue((_startPosition, 0, 0));
            while (todo.Count != 0)
            {
                var curr = todo.Dequeue();
                if (!done.Add((curr.Item1, curr.Item2 % _allWindPosition.Count, curr.Item3)))
                    continue;
                var currRound = curr.Item2 + 1;
                if (currRound > bestRound)
                    continue;

                foreach (var pos in curr.Item1.Around(_minX, 0, _maxX, _maxY + 1))
                {
                    int gobackAndForth = curr.Item3;
                    if (pos.Equals(_endPosition))
                    {
                        if (gobackAndForth == 2)
                        {
                            bestRound = currRound;
                            break;
                        }
                        else if (gobackAndForth == 0)
                        {
                            todo.Clear();
                            gobackAndForth = 1;
                        }
                    }
                    else if (pos.Equals(_startPosition))
                    {
                        if (gobackAndForth == 1)
                        {
                            todo.Clear();
                            gobackAndForth = 2;
                        }
                    }
                    else if (pos.Y == _maxY + 1 || pos.Y == 0 || pos.X == _maxX + 1 || pos.X == 0)
                        continue; // In a Wall

                    if (!_allWindPosition[currRound % _allWindPosition.Count].Contains(pos))
                    {
                        todo.Enqueue((pos, currRound, gobackAndForth));
                    }
                }
                if (!_allWindPosition[currRound % _allWindPosition.Count].Contains(curr.Item1))
                    todo.Enqueue((curr.Item1, currRound, curr.Item3)); // wait in place
            }

            //Print(bestRound, _allWindPosition[bestRound % _allWindPosition.Count], _endPosition);
            return (bestRound).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

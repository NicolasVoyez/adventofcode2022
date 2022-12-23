using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode2022.Solvers
{
    class SolverDay14 : ISolver
    {

        private int _MaxX= int.MinValue;
        private int _MaxY = int.MinValue;

        private Grid<char> _ex1Grid;
        private Grid<char> _ex2Grid;
        private Point _sandStart;
        private Point _sand2Start;
        public void InitInput(string content)
        {

            var splitForMaxes = content.Split(new string[] { "\r\n", " -> " }, StringSplitOptions.RemoveEmptyEntries);
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            int minX = int.MaxValue;
            foreach (var s in splitForMaxes)
            {
                var si = s.SplitAsInt(",").ToList();
                if (si[0] > maxX)
                    maxX = si[0];
                else if (si[0] < minX)
                    minX = si[0];
                if (si[1] > maxY)
                    maxY = si[1];
            }

            _MaxX = maxX - minX +1;
            _MaxY = maxY +1;
            _sandStart = new Point(0, 500 - minX);
            _sand2Start = new Point(0, 750 - minX);

            _ex1Grid = new Grid<char>(new char[_MaxY +1, _MaxX +1],_MaxY,_MaxX);
            _ex2Grid = new Grid<char>(new char[_MaxY + 3, _MaxX + 1 + 500], _MaxY +2, _MaxX + 500);

            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var currentLine in splitContent)
            {
                var s = currentLine.SplitREE(" -> ");
                for (int i = 1; i < s.Length; i++)
                {
                    var from = s[i - 1].SplitAsInt(",");
                    from[0] -= minX;
                    var to = s[i].SplitAsInt(",");
                    to[0] -= minX;
                    if (from[0] == to[0])
                        for (int y = Math.Min(from[1], to[1]); y <= Math.Max(from[1], to[1]); y++)
                        {
                            _ex1Grid.Set(y, from[0], '#');
                            _ex2Grid.Set(y, from[0] + 250, '#');
                        }
                    else if (from[1] == to[1])
                        for (int x = Math.Min(from[0], to[0]); x <= Math.Max(from[0], to[0]); x++)
                        {
                            _ex1Grid.Set(from[1], x, '#');
                            _ex2Grid.Set(from[1], x + 250, '#');
                        }
                    else
                        throw new NotImplementedException("Diagonals ? WTF");

                    for (int x = 0; x < _ex2Grid.XMax; x++)
                    {
                        _ex2Grid.Set(_ex2Grid.YMax - 1, x, '#');
                    }
                }
            }
        }

        public string SolveFirstProblem()
        {
            Point currentSand = _sandStart;
            int i = 0;
            while (TryGetNextPosition(currentSand, _ex1Grid, out Point newPosition))
            {
                i++;
                if (currentSand.Equals(newPosition))
                {
                    {
                        _ex1Grid.Set(currentSand.Y, currentSand.X, 'O');
                        currentSand = _sandStart;
                    }
                }
                else
                    currentSand = newPosition;

                //OneByOneShow(newPosition);
                i = 0;
            }
            //OneByOneShow(_sandStart);   
            return _ex1Grid.All().Count(x => x.Value == 'O').ToString();
        }

        private void OneByOneShow(Point current, Grid<char> grid, int skip= 0)
        {
            Console.Clear();
            grid.Print(x =>
            {
                if (x.X < skip || x.X > grid.XMax - skip)
                    return;
                if (x.X == current.X && x.Y == current.Y)
                    Console.Write('X');
                else if (x.Value == '#' || x.Value == 'O')
                    Console.Write(x.Value);
                else
                    Console.Write(' ');
            });
            Console.ReadKey();
        }

        private bool TryGetNextPosition(Point curr, Grid<char> grid, out Point newPosition)
        {
            
            newPosition = curr;
            if (curr.Y >= grid.YMax)
                return false;
            if (grid[curr.Y+ 1, curr.X].Value != '#' && grid[curr.Y + 1, curr.X].Value != 'O')
            {
                newPosition = new Point(curr.Y + 1, curr.X);
                return true;
            }
            if (curr.X <= 0)
                return false;
            if (grid[curr.Y +1, curr.X -1].Value != '#' && grid[curr.Y + 1, curr.X - 1].Value != 'O')
            {
                newPosition = new Point(curr.Y +1, curr.X - 1);
                return true;
            }
            if (curr.X >= grid.XMax)
                return false;
            if (grid[curr.Y + 1, curr.X + 1].Value != '#' && grid[curr.Y + 1, curr.X + 1].Value != 'O')
            {
                newPosition = new Point(curr.Y +1 , curr.X + 1);
                return true;
            }
            return true;
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            Point currentSand = _sand2Start;
            int i = 0;
            while (TryGetNextPosition(_sand2Start, _ex2Grid, out Point end) && !end.Equals(_sand2Start))
            {
                if (!TryGetNextPosition(currentSand, _ex2Grid, out Point newPosition))
                    throw new NotImplementedException("Should never happen, there is a floor");
                i++;
                if (currentSand.Equals(newPosition))
                {
                    {
                        _ex2Grid.Set(currentSand.Y, currentSand.X, 'O');
                        currentSand = _sand2Start;
                    }
                }
                else
                    currentSand = newPosition;

                /*if (i % 10 == 0)
                {
                    OneByOneShow(newPosition, _ex2Grid, 245);
                    i = 0;
                }*/
            }
            OneByOneShow(_sandStart, _ex2Grid, 165);
            return _ex2Grid.All().Count(x => x.Value == 'O').ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

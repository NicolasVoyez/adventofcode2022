using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventOfCode2022.Solvers
{
    class SolverDay22 : ISolver
    {
        private List<(Direction?, int)> _rotasMoves = new List<(Direction?, int)>();
        private Grid<char> _grid;
        private Point _start;
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var orders = splitContent.Last();

            _start = new Point(0, splitContent[0].IndexOf('.'));
            var gridMaterial = splitContent.Take(splitContent.Length - 1).Aggregate("", (x, y) => x + "\r\n" + y);
            _grid = new Grid<char>(gridMaterial, c => c, ' ');

            for (int i = 0; i < orders.Length; i += 2)
            {
                int moves = 0;
                Direction? rota = Direction.Right;
                if (i + 2 <= orders.Length && int.TryParse(orders.Substring(i, 2), out moves))
                {
                    if (i + 2 < orders.Length)
                    {
                        rota = GetDirection(orders[i + 2]);
                    }
                    else
                    {
                        rota = null;
                    }
                    i += 1;
                }
                else if (i + 1 <= orders.Length && int.TryParse(orders.Substring(i, 1), out moves))
                {
                    if (i + 1 < orders.Length)
                    {
                        rota = GetDirection(orders[i + 1]);
                    }
                    else
                    {
                        rota = null;
                    }
                }
                _rotasMoves.Add((rota, moves));
            }
        }
        private Direction GetDirection(char c)
        {
            return c switch
            {
                'R' => Direction.Right,
                'L' => Direction.Left,
                'U' => Direction.Up,
                'D' => Direction.Down,
            };
        }
        private Direction GetDirection(Direction rota, Direction oldDir)
        {
            if (rota == Direction.Right)
            {
                return oldDir switch
                {
                    Direction.Right => Direction.Down,
                    Direction.Left => Direction.Up,
                    Direction.Up => Direction.Right,
                    Direction.Down => Direction.Left,
                };
            }
            else
            {
                return oldDir switch
                {
                    Direction.Right => Direction.Up,
                    Direction.Left => Direction.Down,
                    Direction.Up => Direction.Left,
                    Direction.Down => Direction.Right,
                };
            }
        }

        public string SolveFirstProblem()
        {
            Grid<char>.Cell<char> current = _grid[_start.Y, _start.X];
            Direction currentDir = Direction.Right;
            foreach (var (rota, moves) in _rotasMoves)
            {
                for (int i = 0; i < moves; i++)
                {
                    var next = _grid.GetNext(currentDir, current, true).Value;
                    if (next.Value == '#')
                        break; // wall
                    else if (next.Value == '.')
                    {
                        current = next;
                        AddOccupy(current, currentDir);
                    }
                    else
                    {
                        next = _grid.FindInDirection(current, currentDir, c => c != ' ', null, true).Value;
                        if (next.Value == '#')
                            break; // wall
                        else if (next.Value == '.')
                        {
                            current = next;
                            AddOccupy(current, currentDir);
                        }
                    }
                }

                if (rota.HasValue)
                    currentDir = GetDirection(rota.Value, currentDir);
                //Print(current);
            }
            return GetScore(current, currentDir);

        }

        private string GetScore(Grid<char>.Cell<char> current, Direction currentDir)
        {
            int dirScore = 0;
            switch (currentDir)
            {
                case Direction.Right: dirScore = 0; break;
                case Direction.Down: dirScore = 1; break;
                case Direction.Left: dirScore = 2; break;
                case Direction.Up: dirScore = 3; break;
            }

            return (1000 * (current.Y + 1) + 4 * (current.X + 1) + dirScore).ToString();
        }

        private HashSet<Grid<char>.Cell<char>> occupiedL = new HashSet<Grid<char>.Cell<char>>();
        private HashSet<Grid<char>.Cell<char>> occupiedR = new HashSet<Grid<char>.Cell<char>>();
        private HashSet<Grid<char>.Cell<char>> occupiedU = new HashSet<Grid<char>.Cell<char>>();
        private HashSet<Grid<char>.Cell<char>> occupiedD = new HashSet<Grid<char>.Cell<char>>();
        private void Print(Grid<char>.Cell<char> current, bool wait = true)

        {
            Console.Clear();
            _grid.Print(c =>
            {
                if (c == current)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write('X');
                }
                else if (occupiedL.Contains(c))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("<");
                }
                else if (occupiedR.Contains(c))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(">");
                }
                else if (occupiedU.Contains(c))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("^");
                }
                else if (occupiedD.Contains(c))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("v");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(c.Value);
                }
            });
            Console.ForegroundColor = ConsoleColor.White;
            if (wait)
                Console.ReadKey();
        }

        private HashSet<string> verified = new HashSet<string>() {"C2", "D1","D2", "B1", "B2", "A2", "A1", "E2", "E1", "C1", "F2", "F1", "G2" };
        public string SolveSecondProblem(string firstProblemSolution)
        {
            ClearOccupied();
            Grid<char>.Cell<char> current = _grid[_start.Y, _start.X];
            Direction currentDir = Direction.Right;
            foreach (var (rota, moves) in _rotasMoves)
            {
                for (int i = 0; i < moves; i++)
                {
                    var next = _grid.GetNext(currentDir, current, false, (x, y) => new Grid<char>.Cell<char>(y, x, ' ')).Value;
                    if (next.Value == '#')
                        break; // wall
                    else if (next.Value == '.')
                    {
                        current = next;
                        AddOccupy(current, currentDir);
                    }
                    else
                    {
                        var kvp = GetCubeWrap(next);
                        next = kvp.Item1;
                        if (next.Value == '#')
                            break; // wall
                        else if (next.Value == '.')
                        {
                            string CubeWrapText = $"{kvp.Item3} FROM {current}:{currentDir} to {next}:{kvp.Item2}";
                            current = next;
                            currentDir = kvp.Item2;
                            AddOccupy(current, currentDir);

                            if (false && verified.Add(kvp.Item3))
                            {
                                Print(current, false);
                            }
                        }
                        else
                            throw new Exception("Should not happen");
                    }
                }

                if (rota.HasValue)
                    currentDir = GetDirection(rota.Value, currentDir);
            }
            Print(current);
            return GetScore(current, currentDir);
        }

        private void ClearOccupied()
        {
            occupiedL = new HashSet<Grid<char>.Cell<char>>();
            occupiedR = new HashSet<Grid<char>.Cell<char>>();
            occupiedU = new HashSet<Grid<char>.Cell<char>>();
            occupiedD = new HashSet<Grid<char>.Cell<char>>();
        }

        private void AddOccupy(Grid<char>.Cell<char> current, Direction currentDir)
        {
            occupiedD.Remove(current);
            occupiedL.Remove(current);
            occupiedU.Remove(current);
            occupiedD.Remove(current);
            switch (currentDir)
            {
                case Direction.Right:
                    occupiedR.Add(current);
                    break;
                case Direction.Left:
                    occupiedL.Add(current);
                    break;
                case Direction.Up:
                    occupiedU.Add(current);
                    break;
                case Direction.Down:
                    occupiedD.Add(current);
                    break;
            };
        }

        private (Grid<char>.Cell<char>, Direction, string) GetCubeWrap(Grid<char>.Cell<char> c)
        {
            if (c.X == -1)
            {
                if (c.Y >= 100 && c.Y < 150)
                    return (_grid[150 - c.Y, 50], Direction.Right, "C1"); // C 
                else if (c.Y >= 150)
                    return (_grid[0, c.Y - 100], Direction.Down, "D1"); // D 

            }
            if (c.X == 49)
            {
                if (c.Y < 49)
                    return (_grid[150 - c.Y, 0], Direction.Right, "C2"); // C 
                else if (c.Y < 99)
                    return (_grid[100, c.Y - 49], Direction.Down, "A1"); // A
            }
            if (c.X == 50)
            {
                return (_grid[149, c.Y - 100], Direction.Up, "B1"); // B
            }
            if (c.X == 100)
            {
                if (c.Y < 100 && c.Y >= 50)
                    return (_grid[49, c.Y + 50], Direction.Up, "G1"); // G
                if (c.Y >= 100 && c.Y < 150)
                    return (_grid[150 - c.Y, 149], Direction.Left, "F1"); // F
            }
            if (c.X == 150)
            {
                return (_grid[(150 - c.Y), 99], Direction.Left, "F2"); // F
            }
            if (c.Y == -1)
            {
                if (c.X >= 50 && c.X < 100)
                    return (_grid[c.X + 100, 0], Direction.Right, "D2"); // D
                if (c.X >= 100)
                    return (_grid[199, c.X - 100], Direction.Up, "E1"); // E
            }
            if (c.Y == 50)
            {
                return (_grid[c.X - 50, 99], Direction.Left, "G2"); // G
            }
            if (c.Y == 99)
            {
                if (c.X < 50)
                    return (_grid[c.X + 50,  50], Direction.Right, "A2"); // A
            }
            if (c.Y == 150)
            {
                return (_grid[c.X + 100, 49], Direction.Left, "B2"); // B
            }
            if (c.Y == 200)
            {
                return (_grid[0, c.X + 100], Direction.Down, "E2"); // E
            }

            throw new Exception("Buggy case");
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

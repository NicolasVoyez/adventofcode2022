using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Solvers
{
    class SolverDay23 : ISolver
    {
        class Elf
        {
            public Elf(int x, int y)
            {
                Position = new Point(y, x);
            }

            public Point Position { get; set; }

            public override bool Equals(object? obj)
            {
                if (!(obj is Elf oe))
                    return false;
                return Equals(oe);
            }
            public bool Equals(Elf other)
            {
                return Position.Equals(other.Position);
            }
            public override int GetHashCode()
            {
                return Position.GetHashCode();
            }
            public static bool operator ==(Elf e1, Elf e2)
            {
                if (ReferenceEquals(e1, null) && ReferenceEquals(e2, null))
                    return true;
                if (ReferenceEquals(e1, null) ^ ReferenceEquals(e2, null))
                    return false;
                return e1.Equals(e2);
            }
            public static bool operator !=(Elf e1, Elf e2)
            {
                return !(e1 == e2);
            }

        }

        private HashSet<Point> _elves = new HashSet<Point>();
        private List<Direction> _proposalOrder = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int y = 0; y < splitContent.Length; y++)
            {
                var line = splitContent[y];
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                        _elves.Add(new Point(y, x));
                }
            }
        }

        public string SolveFirstProblem()
        {
            for (int r = 0; r < 10; r++)
            {
               //Print("round " + r.ToString());
                Run();
            }

            //Print("ending game", false);
            var minX = _elves.Min(e => e.X);
            var minY = _elves.Min(e => e.Y);
            var maxX = _elves.Max(e => e.X);
            var maxY = _elves.Max(e => e.Y);
            return ((maxX - minX + 1) * (maxY - minY + 1) - _elves.Count).ToString();
        }

        private void Print(string round, bool clear = true)
        {
            if (clear)
                Console.Clear();
            Console.WriteLine(round + " status");
            var minX = _elves.Min(e => e.X);
            var minY = _elves.Min(e => e.Y);
            var maxX = _elves.Max(e => e.X);
            var maxY = _elves.Max(e => e.Y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Console.Write(_elves.Contains(new Point(y, x)) ? '#' : '.');
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }

        private bool Run()
        {
            bool somethingMoved = false;
            // part 1 get proposals
            Dictionary<Point, List<Point>> proposals = new Dictionary<Point, List<Point>>();
            foreach (var elf in _elves)
            {
                var ps = GetPositions(elf).Select(p => (_elves.Contains(p), p)).ToArray();
                if (ps.All(kvp => !kvp.Item1))
                    continue;
                foreach (var d in _proposalOrder)
                {
                    if (TryGetNext(ps, d, out Point next))
                    {
                        somethingMoved = true;
                        if (proposals.ContainsKey(next))
                            proposals[next].Add(elf);
                        else

                            proposals[next] = new List<Point> { elf };
                        break;
                    }
                }
            }
            // part 2 resolve doubloons
            foreach (var (proposal, elves) in proposals)
            {
                if (elves.Count != 1)
                    continue;
                _elves.Remove(elves[0]);
                _elves.Add(proposal);
            }

            // part 3 move proposal Order
            var firstP = _proposalOrder.First();
            _proposalOrder.Remove(firstP);
            _proposalOrder.Add(firstP);

            return somethingMoved;
        }

        private IEnumerable<Point> GetPositions(Point elf)
        {
            yield return new Point(elf.Y - 1, elf.X + 1);
            yield return new Point(elf.Y    , elf.X + 1);
            yield return new Point(elf.Y + 1, elf.X + 1);
            yield return new Point(elf.Y - 1, elf.X - 1);
            yield return new Point(elf.Y    , elf.X - 1);
            yield return new Point(elf.Y + 1, elf.X - 1);
            yield return new Point(elf.Y - 1, elf.X);
            yield return new Point(elf.Y + 1, elf.X);
        }
        private bool TryGetNext((bool,Point)[] pos, Direction d, out Point next)
        {
            next = default;
            switch (d)
            {
                case Direction.Right:
                    next = pos[1].Item2;
                    return !pos[0].Item1 && !pos[1].Item1 && !pos[2].Item1;
                    break;
                case Direction.Left:
                    next = pos[4].Item2;
                    return !pos[3].Item1 && !pos[4].Item1 && !pos[5].Item1;
                    break;
                case Direction.Up:
                    next = pos[6].Item2;
                    return !pos[3].Item1 && !pos[6].Item1 && !pos[0].Item1;
                    break;
                case Direction.Down:
                    next = pos[7].Item2;
                    return !pos[2].Item1 && !pos[5].Item1 && !pos[7].Item1;
                    break;
            };
            throw new Exception("EX");
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            for (int r = 11; r < int.MaxValue; r++)
            {
                //Print("round " + r.ToString());
                if (!Run())
                {
                    return r.ToString();
                }
            }

            throw new Exception("Is that even possible ?");
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

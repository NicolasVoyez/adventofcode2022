using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdventOfCode2022.Solvers
{
    class SolverDay9 : ISolver
    {
        private List<(Direction, int)> _input = new List<(Direction, int)>();
        private List<Direction> _moves = new List<Direction>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var currentLine in splitContent)
            {
                var split = currentLine.SplitREE();
                var times = int.Parse(split[1]);
                var direction = split[0][0];
                var dir = direction switch
                {
                    'R' => Direction.Right,
                    'L' => Direction.Left,
                    'U' => Direction.Up,
                    'D' => Direction.Down,
                };
                _input.Add((dir, times));
                for (int i = 0; i < times; i++)
                    _moves.Add(dir);
            }
        }

        public string SolveFirstProblem()
        {
            Point _headPosition = new Point(0, 0);
            Point _tailPosition = new Point(0, 0);
            var _visitedPositions = new HashSet<Point>();
            _visitedPositions.Add(_headPosition);
            foreach (var direction in _moves)
            {
                _headPosition = _headPosition.ToDirection(direction);
                _tailPosition = Follow(direction, _tailPosition, _headPosition);
                _visitedPositions.Add(_tailPosition);
            }
            return _visitedPositions.Count.ToString();
        }

        public Point Follow(Direction direction, Point follower, Point toFollow)
        {
            if (Math.Abs(toFollow.X - follower.X) <= 1 && Math.Abs(toFollow.Y - follower.Y) <= 1)
                return follower;

            int nextX = follower.X;
            int nextY = follower.Y;
            if (toFollow.X > follower.X)
                nextX++;
            else if (toFollow.X < follower.X)
                nextX--;
            if (toFollow.Y > follower.Y)
                nextY++;
            else if (toFollow.Y < follower.Y)
                nextY--;

            /* OLD part diagonalize
            if (toFollow.X != follower.X && toFollow.Y != follower.Y)
                switch (direction)
                {
                    case Direction.Right:
                    case Direction.Left:
                        follower = new Point(toFollow.Y > follower.Y ? (follower.Y + 1) : (follower.Y - 1), follower.X);
                        break;
                    case Direction.Up:
                    case Direction.Down:
                        follower = new Point(follower.Y, toFollow.X > follower.X ? (follower.X + 1) : (follower.X - 1));
                        break;
                }
            if (!toFollow.Equals(follower))
                return follower.ToDirection(direction);*/

            return new Point(nextY,nextX);
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            Point _headPosition = new Point(0, 0);
            Point[] others = new Point[9];
            for (int i = 0; i < 9; i++)
                others[i] = _headPosition;

            var _visitedPositions = new HashSet<Point>();
            _visitedPositions.Add(_headPosition);
            var previousDir = Direction.Up;
            foreach (var direction in _moves)
            {
                previousDir = direction;
                _headPosition = _headPosition.ToDirection(direction);
                for (int i = 0; i < 9; i++)
                {
                    var newPos = Follow(direction, others[i], i == 0 ? _headPosition : others[i-1]);
                    if (newPos.Equals(others[i]))
                        break;
                    others[i] = newPos;
                }
                _visitedPositions.Add(others[8]);
            }
            return _visitedPositions.Count.ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}

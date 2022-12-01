using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Helpers
{
    public class PositionOnlyGrid
    {
        public int YMax { get; private set; }
        public int XMax { get; private set; }

        private HashSet<(int x, int y)> _positions;


        /// <summary>
        /// Receiving  alist of positions
        /// </summary>
        /// <param name="content"></param>
        public PositionOnlyGrid(IEnumerable<(int x, int y)> positions)
        {
            YMax = positions.Max(p => p.x);
            XMax = positions.Max(p => p.y);

            _positions = new HashSet<(int x, int y)>(positions);
        }

        public void Print(Action printElement = null, Action printEmpty = null)
        {
            if (printElement == null)
                printElement = () => Console.Write("#");
            if (printEmpty == null)
                printEmpty = () => Console.Write(".");

            for (int y = 0; y < YMax; y++)
            {
                for (int x = 0; x < XMax; x++)
                {
                    if (_positions.Contains((x, y)))
                        printElement();
                    else
                        printEmpty();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public void FoldY( int y)
        {
            HashSet<(int x, int y)> newPoints = new HashSet<(int x, int y)>();
            foreach (var point in _positions)
            {
                if (point.y <= y)
                    newPoints.Add(point);
                else
                    newPoints.Add((point.x, 2* y - point.y));
            }
            _positions = newPoints;
            YMax = y;
        }
        public int Count => _positions.Count;

        public void FoldX(int x)
        {
            HashSet<(int x, int y)> newPoints = new HashSet<(int x, int y)>();
            foreach (var point in _positions)
            {
                if (point.x <= x)
                    newPoints.Add(point);
                else
                    newPoints.Add((2* x - point.x, point.y));
            }
            _positions = newPoints;
            XMax = x;
        }
    }
}

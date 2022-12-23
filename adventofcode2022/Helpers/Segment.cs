using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2022.Helpers
{
    public enum Direction
    {
        Right,
        Left,   
        Up,
        Down
    }

    public struct DoublePoint
    {
        public double Y { get; }
        public double X { get; }

        public DoublePoint(double y, double x)
        {
            Y = y;
            X = x;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DoublePoint pt))
                return false;
            return Equals(pt);
        }
        public bool Equals(DoublePoint obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return X == obj.X && Y == obj.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Segment
    {
        public int FromX { get; set; }

        public Segment(int fromX, int fromY, int toX, int toY)
        {
            FromX = fromX;
            ToX = toX;
            FromY = fromY;
            ToY = toY;
            DX = ToX - FromX;
            DY = ToY - FromY;
            IsVertical = ToX == FromX;
            IsHorizontal = ToY == FromY;
            if (!IsVertical)
            {
                A = (double)DY / DX;
                B = ToY - A * ToX; // y = ax+b b= y-ax
            }
        }

        public int ToX { get; }
        public int FromY { get; }
        public int ToY { get; }
        public int DX { get; }
        public bool IsVertical { get; }
        public bool IsHorizontal { get; }
        public int? DY { get; }
        public double A { get; }
        public double B { get; }

        public bool IsParallelTo(Segment s2)
        {
            if (s2.IsHorizontal && IsHorizontal)
                return true;
            if (s2.IsHorizontal || IsHorizontal)
                return false;
            if (s2.IsVertical && IsVertical)
                return true;
            if (s2.IsVertical || IsVertical)
                return false;
            return A == s2.A;
        }

        public IEnumerable<DoublePoint> GetIntersections(Segment s2)
        {
            if (IsParallelTo(s2))
            {
                var s2Points = new HashSet<DoublePoint>(s2.GetAllPoints());
                foreach (var point in GetAllPoints())
                {
                    if (s2Points.Contains(point))
                        yield return point;
                }
            }
            else
            {
                double iX, iY;
                if (IsVertical)
                {
                    // y = ax+b
                    iX = FromX;
                    iY = s2.A * iX + s2.B;
                }
                else if (s2.IsVertical)
                {
                    iX = s2.FromX;
                    iY = A * iX + B;
                }
                else
                {
                    // ax+b = cx + d
                    // (a-c)x = d - b
                    // x = (d-b) / (a-c)
                    // y = ax+b
                    iX = (s2.B - B) / (A - s2.A);
                    iY = A * iX + B;
                }

                if (IsInSquareWithSegmentAsDiagonal(iX, iY) && s2.IsInSquareWithSegmentAsDiagonal(iX, iY))
                {
                    yield return new DoublePoint(iY, iX);
                }
            }
        }

        private HashSet<DoublePoint> _iterated = null;
        public HashSet<DoublePoint> GetAllPoints()
        {
            if (_iterated != null)
                return _iterated;
            _iterated = new HashSet<DoublePoint>();
            if (IsVertical)
            {
                for (int y = (FromY <= ToY) ? FromY : ToY; (FromY <= ToY) ? (y <= ToY) : (y <= FromY); y++)
                {
                    _iterated.Add(new DoublePoint( y, FromX));
                }
            }
            else
            {
                for (int x = (FromX <= ToX) ? FromX : ToX; (FromX <= ToX) ? (x <= ToX) : (x <= FromX); x++)
                {
                    _iterated.Add(new DoublePoint(A * x + B, x));
                }
            }
            return _iterated;
        }

        private bool IsInSquareWithSegmentAsDiagonal(double x, double y)
        {
            
            return (x >= Math.Min(FromX, ToX)) && (x <= Math.Max(FromX, ToX)) && (y >= Math.Min(FromY, ToY)) && (y <= Math.Max(FromY, ToY));
        }

        public override string ToString()
        {
            var h = IsHorizontal ? "H" : "";
            var v = IsVertical ? "V" : "";
            return $"{FromX},{FromY} -> {ToX},{ToY} : {h}{v}";
        }
    }
}

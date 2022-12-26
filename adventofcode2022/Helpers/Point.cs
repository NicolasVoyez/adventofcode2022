namespace AdventOfCode2022.Helpers
{
    public struct Point
    {
        public int Y { get; }
        public int X { get; }

        public Point(int y, int x)
        {
            Y = y;
            X = x;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point pt))
                return false;
            return Equals(pt);
        }
        public bool Equals(Point obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return X == obj.X && Y == obj.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() ^ 27 * Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "(X:" + X + " Y:" + Y + ")";
        }

        // NOTE: D24 put up - and Down + , while it's inverted for days 9 & 17
        public Point ToDirection(Direction d, int qty = 1)
        {
            return d switch
            {
                Direction.Right => new Point(Y, X + qty),
                Direction.Left => new Point(Y, X - qty),
                Direction.Up => new Point(Y - qty, X),
                Direction.Down => new Point(Y + qty, X),
                _ => throw new ArgumentException("not a direction"),
            };
        }

        internal int ManhattanDistance(Point other)
        {
            return Math.Abs(Y - other.Y) + Math.Abs(X - other.X);
        }

        internal IEnumerable<Point> Around()
        {
            yield return new Point(Y, X + 1);
            yield return new Point(Y, X - 1);
            yield return new Point(Y + 1, X);
            yield return new Point(Y - 1, X);
        }
        internal IEnumerable<Point> Around(int minX, int minY, int maxX, int maxY)
        {
            if (X + 1 <= maxX && Y >= minY && Y <= maxY)
                yield return new Point(Y, X + 1);
            if (X - 1 >= minX && Y >= minY && Y <= maxY)
                yield return new Point(Y, X - 1);
            if (Y + 1 <= maxY && X >= minX && X <= maxX)
                yield return new Point(Y + 1, X);
            if (Y - 1 >= minY && X >= minX && X <= maxX)
                yield return new Point(Y - 1, X);
        }
    }
}

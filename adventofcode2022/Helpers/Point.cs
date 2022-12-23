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
            return "(X:" + X + " Y:" + Y+")";
        }

        public Point ToDirection(Direction d, int qty = 1)
        {
            return d switch
            {
                Direction.Right => new Point(Y, X + qty),
                Direction.Left => new Point(Y, X - qty),
                Direction.Up => new Point( Y + qty, X),
                Direction.Down => new Point( Y - qty, X),
                _ => throw new ArgumentException("not a direction"),
            };
        }

        internal int ManhattanDistance(Point other)
        {
            return Math.Abs(Y- other.Y) + Math.Abs(X- other.X);
        }
    }
}

namespace adventofcode2022.Helpers
{
    public class Point3D
    {
        public int Z { get; }
        public int Y { get; }
        public int X { get; }

        public Point3D(int x, int y, int z)
        {
            Z = z;
            Y = y;
            X = x;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point3D pt))
                return false;
            return Equals(pt);
        }
        public bool Equals(Point3D obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return X == obj.X && Y == obj.Y && Z == obj.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() ^ 27 * Y.GetHashCode() ^ 97 * Z.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "(X:" + X + " Y:" + Y + " Z:" + Z +")";
        }

        internal int ManhattanDistance(Point3D other)
        {
            return Math.Abs(Y - other.Y) + Math.Abs(X - other.X) + Math.Abs(Z - other.Z);
        }

        public IEnumerable<Point3D> Neighbors
        {
            get
            {
               yield return new Point3D(X + 1, Y, Z);
               yield return new Point3D(X - 1, Y, Z);
               yield return new Point3D(X, Y + 1, Z);
               yield return new Point3D(X, Y - 1, Z);
               yield return new Point3D(X, Y, Z + 1);
               yield return new Point3D(X, Y, Z - 1);
            }
        }
    }
}

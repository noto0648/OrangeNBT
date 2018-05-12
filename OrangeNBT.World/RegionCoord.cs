namespace OrangeNBT.World
{
    public struct RegionCoord
    {
        public static readonly RegionCoord Empty = new RegionCoord();

        private int _x;
        private int _z;

        public int X { get { return _x; } set { _x = value; } }
        public int Z { get { return _z; } set { _z = value; } }

        public RegionCoord(int x, int y)
        {
            _x = x;
            _z = y;
        }

        public override int GetHashCode()
        {
            return _x ^ _z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RegionCoord)) return false;
            RegionCoord self = (RegionCoord)obj;
            return X == self.X && Z == self.Z;
        }

        public static bool operator ==(RegionCoord left, RegionCoord right)
        {
            return left.X == right.X && left.Z == right.Z;
        }

        public static bool operator !=(RegionCoord left, RegionCoord right)
        {
            return !(left == right);
        }
    }
}

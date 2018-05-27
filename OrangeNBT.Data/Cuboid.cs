namespace OrangeNBT.Data
{
    public class Cuboid
    {
        public static readonly Cuboid Empty = new Cuboid();

        private int _x;
        private int _y;
        private int _z;
        private int _width;
        private int _height;
        private int _length;

        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Z { get { return _z; } set { _z = value; } }

        public int MinX { get { return _x; } }
        public int MinY { get { return _y; } }
        public int MinZ { get { return _z; } }

        public int MaxX { get { return _x + _width; } }
        public int MaxY { get { return _y + _height; } }
        public int MaxZ { get { return _z + _length; } }

        public int Width { get { return _width; } set { _width = value; } }
        public int Height { get { return _height; } set { _height = value; } }
        public int Length { get { return _length; } set { _length = value; } }

        public Cuboid() { }

        public Cuboid(int x, int y, int z, int width, int height, int length)
        {
            _x = x;
            _y = y;
            _z = z;
            _width = width;
            _height = height;
            _length = length;
        }

        public bool Contains(int x, int y,int z)
        {
            return _x <= x && _y <= y && _z <= z && x < _x + _width && y < _y + _height && z < _z + _length;
        }

        public bool Contains(double x, double y, double z)
        {
            return _x <= x && _y <= y && _z <= z && x < _x + _width && y < _y + _height && z < _z + _length;
        }

        public override int GetHashCode()
        {
            return _x ^ _y ^ _z ^ (_width * _height * _length);
        }

        public override bool Equals(object obj)
        {
            Cuboid c = obj as Cuboid;
            if (c == null) return false;
            return X == c.X && Y == c.Y && Z == c.Z && c.Width == Width && c.Height == Height && Length == c.Length;
        }
        
        public static bool operator ==(Cuboid a, Cuboid b)
        {
            if (a == null || b == null) return false;
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.Width == b.Width && a.Height == b.Height && a.Length == b.Length;
        }

        public static bool operator !=(Cuboid a, Cuboid b)
        {
            return !(a == b);
        }
        
    }
}

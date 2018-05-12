using OrangeNBT.NBT;

namespace OrangeNBT.Data
{
    public class Position : ITagProvider<TagList>
    {
        private double _x;
        private double _y;
        private double _z;

        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }

        public Position() { }

        public Position(TagList pos)
        {
            _x = ((TagDouble)pos[0]).Value;
            _y = ((TagDouble)pos[1]).Value;
            _z = ((TagDouble)pos[2]).Value;
        }

        public Position(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public Position(Position original)
            : this(original.X, original.Y, original.Z) { }

        public TagList BuildTag()
        {
            return new TagList("pos", TagType.Double) { new TagDouble(_x), new TagDouble(_y), new TagDouble(_z) };
        }

        public override string ToString()
        {
            return string.Format("Pos:({0},{1},{2})", _x, _y, _z);
        }

        public override int GetHashCode()
        {
            return _x.GetHashCode() * 256 + _y.GetHashCode() ^ _z.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Position pos = obj as Position;
            return (pos == null ? false : (this == pos));
        }

        public static bool operator ==(Position a, Position b)
        {
            if (a == null || b == null) return false;
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }

    }
}

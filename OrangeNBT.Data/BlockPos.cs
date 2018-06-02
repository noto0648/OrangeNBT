namespace OrangeNBT.Data
{
    public struct BlockPos
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public BlockPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override int GetHashCode()
        {
            return X ^ Y ^ Z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BlockPos))
                return false;
            BlockPos pos = (BlockPos)obj;
            return (pos.X == X && pos.Y == Y && pos.Z == Z);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + ")";
        }

        public static BlockPos operator +(BlockPos left, BlockPos right)
        {
            return new BlockPos(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static bool operator ==(BlockPos a, BlockPos b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(BlockPos a, BlockPos b)
        {
            return !(a == b);
        }
    }
}

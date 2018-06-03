using System;
using System.Collections.Generic;
using System.Text;

namespace OrangeNBT.World
{
    public struct ChunkCoord
    {
        public static readonly ChunkCoord Empty = new ChunkCoord();

        private int _x;
        private int _z;

        public int X { get { return _x; } set { _x = value; } }
        public int Z { get { return _z; } set { _z = value; } }

        public RegionCoord RegionCoord
        {
            get
            {
                return new RegionCoord(_x >> 5, _z >> 5);
            }
        }

        public ChunkCoord(int x, int z)
        {
            _x = x;
            _z = z;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", _x, _z);
        }

        public override int GetHashCode()
        {
            return _x ^ _z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChunkCoord)) return false;
            ChunkCoord self = (ChunkCoord)obj;
            return X == self.X && Z == self.Z;
        }

        public static bool operator ==(ChunkCoord left, ChunkCoord right)
        {
            return left.X == right.X && left.Z == right.Z;
        }

        public static bool operator !=(ChunkCoord left, ChunkCoord right)
        {
            return !(left == right);
        }
    }
}

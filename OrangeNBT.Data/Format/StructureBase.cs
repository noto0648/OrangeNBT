using System;
using System.Collections.Generic;
using System.Text;

namespace OrangeNBT.Data.Format
{
    public abstract class StructureBase
    {
        protected readonly int _width;
        protected readonly int _height;
        protected readonly int _length;

        public int Width => _width;
        public int Height => _height;
        public int Length => _length;

        public StructureBase(int width, int height, int length)
        {
            _width = width;
            _height = height;
            _length = length;
        }

        public StructureBase(Cuboid cuboid)
            : this(cuboid.Width, cuboid.Height, cuboid.Length)
        { }


    }
}

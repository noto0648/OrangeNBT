using System;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.World.Core;

namespace OrangeNBT.World
{
    public abstract class Dimension : IDisposable
    {
        public const int Overworld = 0;
        public const int Nether = -1;
        public const int TheEnd = 1;

        public bool IsInfinity { get { return true; } }

        public abstract IBlockAccess Blocks { get; }

        public abstract IEntityCollection Entities { get; }

        public abstract IChunkManager Chunks { get; }

        public abstract ICache Cache { get; }

        public abstract void Dispose();

        public abstract void Save();

    }
}

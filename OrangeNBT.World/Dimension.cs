using OrangeNBT.Data;
using OrangeNBT.World.Core;
using System;

namespace OrangeNBT.World
{
    public abstract class Dimension : IDisposable
    {
        public const int Overworld = 0;
        public const int Nether = -1;
        public const int TheEnd = 1;

        public bool IsInfinity { get { return true; } }

        public abstract IWorldAccess Blocks { get; }

        public abstract IEntityCollection Entities { get; }

        public abstract IChunkManager Chunks { get; }

        public abstract ICache Cache { get; }

        public abstract void Dispose();

		public abstract void Save();

    }
}

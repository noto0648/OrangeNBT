using OrangeNBT.Data;
using OrangeNBT.World.Core;
using System;
using System.IO;

namespace OrangeNBT.World.Anvil
{
    public class AnvilDimension : Dimension
    {
        private AnvilChunkManager _chunk;
        private AnvilBlockManager _blocks;
        private AnvilEntityCollection _entities;
        private AnvilCache _cache;

        public override IWorldAccess Blocks =>  _blocks;

        public override IEntityCollection Entities => _entities;

        public override IChunkManager Chunks => _chunk;

        public override ICache Cache => _cache;

        public AnvilDimension(string directory, int cacheCapacity = 256)
        {
            _cache = new AnvilCache(cacheCapacity);

            _chunk = new AnvilChunkManager(directory + Path.DirectorySeparatorChar + "region", _cache);
            _blocks = new AnvilBlockManager(_chunk);
            _entities = new AnvilEntityCollection(_chunk);
        }

		public override void Save(int version)
        {
            _chunk.Save(version);
        }

        public override void Dispose()
        {
            ((IDisposable)_chunk).Dispose();
        }
    }
}

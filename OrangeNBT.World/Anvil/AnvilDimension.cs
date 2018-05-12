using System;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.World;
using OrangeNBT.World.Core;
using OrangeNBT.NBT;
using System.IO;

namespace OrangeNBT.World.Anvil
{
    public class AnvilDimension : Dimension
    {
        private AnvilChunkManager _chunk;
        private AnvilBlockManager _blocks;
        private AnvilEntityCollection _entities;
        private AnvilCache _cache;

        public override IBlockAccess Blocks =>  _blocks;

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

        public override void Save()
        {
            _chunk.Save();
        }

        public override void Dispose()
        {
            ((IDisposable)_chunk).Dispose();
        }
    }
}

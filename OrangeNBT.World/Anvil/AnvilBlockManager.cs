using OrangeNBT.Data;
using OrangeNBT.NBT;
using OrangeNBT.World.Core;

namespace OrangeNBT.World.Anvil
{
	public class AnvilBlockManager : IBlockManager
    {
        private IChunkManager _chunkCache;

        private bool _autoLight;

        public bool AutoLight { get { return _autoLight; } set { _autoLight = value; } }

        public AnvilBlockManager(IChunkManager chunkManager)
        {
            _chunkCache = chunkManager;
        }

        public BlockSet GetBlock(int x, int y, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetBlock(x & 15, y, z & 15);
        }

        public int GetBlockLight(int x, int y, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetBlockLight(x & 15, y, z & 15);
        }

        public int GetHeight(int x, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetHeight(x & 15, z & 15);
        }

        public int GetSkyLight(int x, int y, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetSkyLight(x & 15, y, z & 15);
        }

        public int GetBiome(int x, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetBiome(x & 15, z & 15);
        }

        private void RelightCheck()
        {
            if (!_autoLight) return;
        }

        public bool SetBlockLight(int x, int y, int z, int light)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetBlockLight(x & 15, y, z & 15, light);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public bool SetBlock(int x, int y, int z, BlockSet data)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetBlock(x & 15, y, z & 15, data);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public bool SetHeight(int x, int z, int height)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetHeight(x & 15, z & 15, height);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public bool SetSkyLight(int x, int y, int z, int light)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetSkyLight(x & 15, y, z & 15, light);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public TagCompound GetTileEntity(int x, int y, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetTileEntity(x & 15, y, z & 15);
        }

        public bool SetTileEntity(int x, int y, int z, TagCompound tag)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetTileEntity(x & 15, y, z & 15, tag);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public bool SetBiome(int x, int z, int biome)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetBiome(x & 15, z & 15, biome);
            chunk.IsModified = true;
            //RelightCheck();
            return r;
        }
    }
}

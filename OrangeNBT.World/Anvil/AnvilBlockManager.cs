using System;
using System.Collections.Generic;
using System.Text;
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

        public int GetBlockData(int x, int y, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetBlockData(x % 16, y, z % 16);
        }

        public int GetBlockId(int x, int y, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetBlockId(x % 16, y, z % 16);
        }

        public int GetBlockLight(int x, int y, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetBlockLight(x % 16, y, z % 16);
        }

        public int GetHeight(int x, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetHeight(x % 16, z % 16);
        }

        public int GetSkyLight(int x, int y, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetSkyLight(x % 16, y, z % 16);
        }

        public int GetBiome(int x, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetBiome(x % 16, z % 16);
        }

        private void RelightCheck()
        {
            if (!_autoLight) return;
        }

        public bool SetBlockLight(int x, int y, int z, int light)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetBlockLight(x % 16, y, z % 16, light);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public bool SetData(int x, int y, int z, int data)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetData(x % 16, y, z % 16, data);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public bool SetHeight(int x, int z, int height)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetHeight(x % 16, z % 16, height);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public bool SetBlockId(int x, int y, int z, int id)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetBlockId(x % 16, y, z % 16, id);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public bool SetSkyLight(int x, int y, int z, int light)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetSkyLight(x % 16, y, z % 16, light);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public TagCompound GetTileEntity(int x, int y, int z)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            return chunk.GetTileEntity(x % 16, y, z % 16);
        }

        public bool SetTileEntity(int x, int y, int z, TagCompound tag)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetTileEntity(x % 16, y, z % 16, tag);
            chunk.IsModified = true;
            RelightCheck();
            return r;
        }

        public bool SetBiome(int x, int z, int biome)
        {
            IChunk chunk = _chunkCache.GetChunk(new ChunkCoord(x >> 4, z >> 4));
            bool r = chunk.SetBiome(x % 16, z % 16, biome);
            chunk.IsModified = true;
            //RelightCheck();
            return r;
        }
    }
}

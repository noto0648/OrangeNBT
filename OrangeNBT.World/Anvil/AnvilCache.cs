using OrangeNBT.NBT;
using OrangeNBT.NBT.IO;
using OrangeNBT.World.Core;
using System.Collections.Generic;

namespace OrangeNBT.World.Anvil
{
    public class AnvilCache : ICache
    {
        private Dictionary<ChunkCoord, IChunk> _chunks;
        private int _attentionLevel;
        public AnvilCache()
            : this(512) { }

        public AnvilCache(int capacity)
        {
            _chunks = new Dictionary<ChunkCoord, IChunk>(capacity);
            _attentionLevel = (int)(capacity * 0.8f);
        }

        public void Add(IChunk chunk)
        {
            if (_chunks.Count >= _attentionLevel)
            {
                SyncAndCheck();
            }

            _chunks.Add(chunk.Coord, chunk);
        }

        private void SyncAndCheck()
        {
            List<ChunkCoord> coords = new List<ChunkCoord>(_chunks.Count);
            foreach(IChunk ck in _chunks.Values)
            {
                if (!ck.IsModified)
                    coords.Add(ck.Coord);
            }
            for (int i = 0; i < coords.Count; i++)
                _chunks.Remove(coords[i]);
        }

        public void Remove(IChunk chunk)
        {
            _chunks.Remove(chunk.Coord);
        }

        public void Clear()
        {
            _chunks.Clear();
        }

        private void CacheStorage(AnvilChunk chunk)
        {
            TagCompound c = chunk.BuildTag();
            NBTFile.ToFile(string.Format("{0}-{1}-{2}.ocache", 0, chunk.Coord.X, chunk.Coord.Z), c);
        }
        
        public IChunk Fetch(ChunkCoord coord)
        {
            return _chunks[coord];
        }

        public bool Contains(ChunkCoord coord)
        {
            return _chunks.ContainsKey(coord);
        }

        public IEnumerator<IChunk> GetEnumerator()
        {
            foreach (IChunk ck in _chunks.Values)
                yield return ck;
        }
    }
}

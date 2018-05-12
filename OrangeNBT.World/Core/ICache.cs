using System.Collections.Generic;

namespace OrangeNBT.World.Core
{
    public interface ICache
    {
        void Add(IChunk chunk);
        void Remove(IChunk chunk);
        void Clear();
        IChunk Fetch(ChunkCoord coord);
        bool Contains(ChunkCoord coord);
        IEnumerator<IChunk> GetEnumerator();
    }
}

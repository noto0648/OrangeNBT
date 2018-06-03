using System.Collections.Generic;

namespace OrangeNBT.World.Core
{
    public interface IChunkManager
    {
        IChunk GetChunk(ChunkCoord coord);

        IChunk CreateChunk(ChunkCoord coord);

        IEnumerable<IChunk> ListAllChunks();

        IEnumerable<ChunkCoord> ListAllCoords();

    }
}

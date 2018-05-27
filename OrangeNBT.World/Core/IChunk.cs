using System;
using System.Collections.Generic;
using System.Text;

namespace OrangeNBT.World.Core
{
    public interface IChunk: IWorldAccess
    {
        bool IsModified { get; set; }
        ChunkCoord Coord { get; }

        void Save();
    }
}

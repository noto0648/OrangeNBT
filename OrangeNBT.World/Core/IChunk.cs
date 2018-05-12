using System;
using System.Collections.Generic;
using System.Text;

namespace OrangeNBT.World.Core
{
    public interface IChunk: IBlockAccess
    {
        bool IsModified { get; set; }
        ChunkCoord Coord { get; }

        void Save();
    }
}

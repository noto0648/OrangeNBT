using System;
using System.Collections.Generic;
using System.Text;

namespace OrangeNBT.World.Core
{
    public interface IChunk: IWorldAccess
    {
        bool IsModified { get; set; }
        ChunkCoord Coord { get; }
		int DataVersion { get; set; }

        void Save();
    }
}

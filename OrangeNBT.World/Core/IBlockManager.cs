using System;
using System.Collections.Generic;
using System.Text;

namespace OrangeNBT.World.Core
{
    public interface IBlockManager : IWorldAccess
    {
        bool AutoLight { get; set; }
    }
}

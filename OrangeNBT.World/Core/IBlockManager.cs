using System;
using System.Collections.Generic;
using System.Text;

namespace OrangeNBT.World.Core
{
    public interface IBlockManager : IBlockAccess
    {
        bool AutoLight { get; set; }
    }
}

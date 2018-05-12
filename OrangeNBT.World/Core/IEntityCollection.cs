using System;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.NBT;

namespace OrangeNBT.World.Core
{
    public interface IEntityCollection : IEnumerable<TagCompound>
    {
        void Add(TagCompound tag);

        void Add(TagCompound tag, bool safe);

        IEnumerable<TagCompound> GetWithin(Cuboid area);

        void Remove(TagCompound tag);

    }
}

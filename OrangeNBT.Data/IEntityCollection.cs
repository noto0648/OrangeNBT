using OrangeNBT.NBT;
using System.Collections.Generic;

namespace OrangeNBT.Data
{
    public interface IEntityCollection : IEnumerable<TagCompound>
    {
        void Add(TagCompound tag);

        void Add(TagCompound tag, bool safe);

        IEnumerable<TagCompound> GetWithin(Cuboid area);

        void Remove(TagCompound tag);

    }
}

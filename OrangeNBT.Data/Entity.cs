using OrangeNBT.NBT;

namespace OrangeNBT.Data
{
    public static class Entity
    {
        public static bool IsEntityTag(TagCompound compound)
        {
            return true;
        }

        
        public static void SetPosition(TagCompound compound, Position pos)
        {
            if (!compound.ContainsKey("Pos"))
            {
                compound.Add(new TagList("Pos"));
            }

            TagList list = compound["Pos"] as TagList;
            compound["Pos"] = pos.BuildTag();
        }

        
        public static Position GetPosition(TagCompound compound)
        {
            if (compound.ContainsKey("Pos"))
            {
                TagList list = compound["Pos"] as TagList;
                if (list != null && list.Count == 3)
                {
                    return new Position(list);
                }
            }
            return new Position();
        }
        
    }
}

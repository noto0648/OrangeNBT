using System;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.Data;
using OrangeNBT.Helper;
using OrangeNBT.NBT;

namespace OrangeNBT.World
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
            //TagDouble x = (TagDouble)list[0];
            //TagDouble y = (TagDouble)list[1];
            //TagDouble z = (TagDouble)list[2];
            //x.Value = pos.X;
            //y.Value = pos.Item2;
            //z.Value = pos.Item3;
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

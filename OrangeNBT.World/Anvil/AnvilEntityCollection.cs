using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.NBT;
using OrangeNBT.World.Core;
using OrangeNBT.Helper;
using OrangeNBT.Data;

namespace OrangeNBT.World.Anvil
{
    public class AnvilEntityCollection : IEntityCollection
    {
        //private List<TagCompound> _entities;
        private AnvilChunkManager _chunk;

        public AnvilEntityCollection(AnvilChunkManager chunk)
        {
            _chunk = chunk;
            //_entities = new List<TagCompound>();
        }

        public void Add(TagCompound tag)
        {
            Position pos = Entity.GetPosition(tag);
            //Entity.SetPosition(tag, new Tuple<double, double, double>(pos.Item1 % 16, pos.Item2 % 16, pos.Item3 % 16));4
            int x = (int)Math.Floor(pos.X / 16);
            int z = (int)Math.Floor(pos.Z / 16);
            AnvilChunk c = (AnvilChunk)_chunk.GetChunk(new ChunkCoord(x, z));
            c.Entities.Add(tag);

            //_entities.Add(tag);
        }

        public void Add(TagCompound tag, bool safe)
        {
            Add(tag);
        }

        public IEnumerator<TagCompound> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TagCompound> GetWithin(Cuboid area)
        {
            int fromX = area.MinX / 16;
            int fromZ = area.MinZ / 16;
            int toX = area.MaxX / 16;
            int toZ = area.MaxZ / 16;

            for (int z = fromZ; z < toZ; z++)
            {
                for (int x = fromX; x < toX; x++)
                {
                    AnvilChunk c = (AnvilChunk)_chunk.GetChunk(new ChunkCoord(x, z));
                    foreach (TagCompound e in c.Entities)
                    {
                        Position pos = Entity.GetPosition(e);
                        if (area.Contains(pos.X, pos.Y, pos.Z))
                        {
                            yield return e;
                        }
                    }
                }
            }

        }


        public void Remove(TagCompound tag)
        {
            Position pos = Entity.GetPosition(tag);
            int x = (int)Math.Floor(pos.X / 16);
            int z = (int)Math.Floor(pos.Z / 16);
            AnvilChunk c = (AnvilChunk)_chunk.GetChunk(new ChunkCoord(x, z));
            c.Entities.Remove(tag);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}

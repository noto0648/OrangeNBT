using System;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.NBT;
using OrangeNBT.Helper;
using System.Collections;

namespace OrangeNBT.Data.Format
{
    public class Schematic : StructureBase, IBlockAccess, ITagProvider<TagCompound>
    {
        private string _materials = "Alpha";
        private byte[] _blocks;
        private byte[] _metadata;
        private TagList _tileEntities;
        private IEntityCollection _entities;

        public Schematic(int width, int height, int length)
            : base(width, height, length)
        {
            _blocks = new byte[width * height * length];
            _metadata = new byte[width * height * length];
            _tileEntities = new TagList("TileEntities", TagType.Compound);
        }

        public Schematic(Cuboid cuboid)
            : this(cuboid.Width, cuboid.Height, cuboid.Length)
        { }

        public int GetBlockData(int x, int y, int z)
        {
            return _blocks[ToIndex(x, y, z)];
        }

        public int GetBlockId(int x, int y, int z)
        {
            return _metadata[ToIndex(x, y, z)];
        }

        public TagCompound GetTileEntity(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        //public bool SetBlock(int x, int y, int z, IBlock block, int metadata = 0)
        public bool SetBlock(int x, int y, int z, int block, int metadata = 0)
        {
            _blocks[ToIndex(x, y, z)] = (byte)block;
            _metadata[ToIndex(x, y, z)] = (byte)metadata;
            return true;
        }

        public bool SetTileEntity(int x, int y, int z, TagCompound tag)
        {
            throw new NotImplementedException();
        }

        protected int ToIndex(int x, int y, int z)
        {
            return x + z * _width + y * _length * _width;
        }

        public bool SetData(int x, int y, int z, int data)
        {
            _metadata[ToIndex(x, y, z)] = (byte)data;
            return true;
        }

        public bool SetBlockId(int x, int y, int z, int id)
        {
            _blocks[ToIndex(x, y, z)] = (byte)id;
            return true;
        }

        public TagCompound BuildTag()
        {
            return new TagCompound("Schematic")
            {
                new TagShort("Width", (short)_width),
                new TagShort("Height", (short)_height),
                new TagShort("Length", (short)_length),
                new TagString("Materials", _materials),
                new TagByteArray("Blocks", _blocks),
                new TagByteArray("Data", _metadata),
                _tileEntities,
                ((EntityCollection)_entities).BuildTag()
            };
        }

        public static Schematic LoadFromNBT(TagCompound root)
        {
            if ((!root.ContainsKey("Width")) || (!root.ContainsKey("Height")) || (!root.ContainsKey("Length")) || (!root.ContainsKey("Materials")) || (!root.ContainsKey("Blocks")) || (!root.ContainsKey("Data")))
            {
                throw new ArgumentException("Unknown format");
            }
            if (root.GetString("Materials") != "Alpha")
            {
                throw new ArgumentException("Unknown format");
            }
            int w = root.GetShort("Width");
            int h = root.GetShort("Height");
            int l = root.GetShort("Length");
            Schematic obj = new Schematic(w, h, l);
            byte[] blocks = ((TagByteArray)root["Blocks"]).Value;
            byte[] metadata = ((TagByteArray)root["Data"]).Value;

            for (int z = 0; z < obj.Length; z++)
            {
                for (int y = 0; y < obj.Height; y++)
                {
                    for (int x = 0; x < obj.Width; x++)
                    {
                        obj.SetBlock(x, y, z, blocks[obj.ToIndex(x, y, z)], metadata[obj.ToIndex(x, y, z)]);
                    }
                }
            }

            if (root.ContainsKey("TileEntities", TagType.List))
            {
                obj._tileEntities = ((TagList)root[("TileEntities")]);
            }
            if (root.ContainsKey("Entities", TagType.List))
            {
                obj._entities = new EntityCollection(root["Entities"] as TagList);
            }
            return obj;
        }

        class EntityCollection : IEntityCollection, ITagProvider<TagList>
        {
            private TagList _list;

            public EntityCollection(TagList tagList)
            {
                _list = tagList;
            }

            EntityCollection()
            {
                _list = new TagList("Entities");
            }

            public void Add(TagCompound tag)
            {
                Add(tag, false);
            }

            public void Add(TagCompound tag, bool safe)
            {
                if (safe && Entity.IsEntityTag(tag))
                    _list.Add(tag);
                if (!safe)
                    _list.Add(tag);
            }

            public TagList BuildTag()
            {
                return _list;
            }

            public IEnumerator<TagCompound> GetEnumerator()
            {
                foreach (TagCompound e in _list)
                    yield return e;
            }

            public IEnumerable<TagCompound> GetWithin(Cuboid area)
            {
                foreach (TagCompound e in _list)
                {
                    Position p = Entity.GetPosition(e);
                    if (area.Contains(p.X, p.Y, p.Z))
                        yield return e;
                }
            }

            public void Remove(TagCompound tag)
            {
                _list.Remove(tag);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _list.GetEnumerator();
            }
        }
    }
}

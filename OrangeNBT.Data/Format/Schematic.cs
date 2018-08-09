using OrangeNBT.Data.Anvil;
using OrangeNBT.Helper;
using OrangeNBT.NBT;
using System;
using System.Diagnostics;

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
		
        private int GetBlockId(int x, int y, int z)
        {
            return _blocks[ToIndex(x, y, z)];
        }

		private int GetBlockData(int x, int y, int z)
        {
            return _metadata[ToIndex(x, y, z)];
        }
		
		public BlockSet GetBlock(int x, int y, int z)
		{
			if(AnvilDataProvider.Instance.GetBlock(GetBlockId(x, y, z))==null)
			{
				Debug.WriteLine("Skipped:" + GetBlockId(x, y, z));
			}
			return new BlockSet(AnvilDataProvider.Instance.GetBlock(GetBlockId(x, y, z)), GetBlockData(x, y, z));
		}

		public bool SetBlock(int x, int y, int z, BlockSet block)
		{
			return SetBlock(x, y, z, block.Block.Id, block.Metadata);
		}

		public TagCompound GetTileEntity(int x, int y, int z)
		{
			foreach (TagCompound comp in _tileEntities)
			{
				if (comp.ContainsKey("x") && comp.ContainsKey("y") && comp.ContainsKey("z"))
				{
					int tx = comp.GetInt("x");
					int ty = comp.GetInt("y");
					int tz = comp.GetInt("z");
					if (tx == x && ty == y && tz == z)
					{
						return comp;
					}
				}
			}
			return null;
		}

        private bool SetBlock(int x, int y, int z, int block, int metadata = 0)
        {
            _blocks[ToIndex(x, y, z)] = (byte)block;
            _metadata[ToIndex(x, y, z)] = (byte)metadata;
            return true;
        }

		public bool SetTileEntity(int x, int y, int z, TagCompound tag)
		{
			TagCompound cp = GetTileEntity(x, y, z);
			if (cp != null && tag != null)
			{
				cp = tag;
				return true;
			}
			else if (cp != null && tag == null)
			{
				_tileEntities.Remove(cp);
				return true;
			}
			else if (cp == null && tag != null)
			{
				_tileEntities.Add(tag);
				return true;
			}
			return false;
		}

        protected int ToIndex(int x, int y, int z)
        {
            return x + z * _width + y * _length * _width;
        }

        private bool SetData(int x, int y, int z, int data)
        {
            _metadata[ToIndex(x, y, z)] = (byte)data;
            return true;
        }

		private bool SetBlockId(int x, int y, int z, int id)
        {
            _blocks[ToIndex(x, y, z)] = (byte)id;
            return true;
        }

        public TagCompound BuildTag()
        {
			TagCompound t = new TagCompound("Schematic")
            {
                new TagShort("Width", (short)_width),
                new TagShort("Height", (short)_height),
                new TagShort("Length", (short)_length),
                new TagString("Materials", _materials),
                new TagByteArray("Blocks", _blocks),
                new TagByteArray("Data", _metadata),
            };

			if (_tileEntities != null)
				t.Add(_tileEntities);
			if (_entities != null)
				t.Add(((EntityCollection)_entities).BuildTag());

			return t;
        }

        public static Schematic FromNBT(TagCompound root)
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
    }
}

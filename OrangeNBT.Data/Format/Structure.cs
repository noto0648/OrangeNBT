using OrangeNBT.Helper;
using OrangeNBT.NBT;
using System;
using System.Collections.Generic;

namespace OrangeNBT.Data.Format
{
	public class Structure : StructureBase, IBlockAccess, ITagProvider<TagCompound>
	{
		private int _dataVersion = 1;
		private string _author = "orange_nbt";

		public int DataVersion { get { return _dataVersion; } set { _dataVersion = value; } }
		public string Author { get { return _author; } set { _author = value; } }

		private BlockSet[,,] _blockList;
		private IEntityCollection _entityList;

		public Structure(int width, int height, int length)
			:base(width, height, length)
		{
			_blockList = new BlockSet[width, height, length];

			for (int x = 0; x < _width; x++)
				for (int y = 0; y < _height; y++)
					for (int z = 0; z < _length; z++)
					{
						_blockList[x, y, z] = new BlockSet("minecraft:air", 0) { X = x, Y = y, Z = z };
					}
			_entityList = new EntityCollection();
		}

		public Structure(int width, int height, int length, int version)
			: this(width, height, length)
		{
			_dataVersion = version;
		}

		private BlockSet GetAt(int x, int y, int z)
		{
			return _blockList[x, y, z];
		}

		public int GetBlockData(int x, int y, int z)
		{
			return GetAt(x, y, z).Metadata;
		}

		public int GetBlockId(int x, int y, int z)
		{
			return GetAt(x, y, z).BlockId;
		}

		public TagCompound GetTileEntity(int x, int y, int z)
		{
			return GetAt(x, y, z).NBT;
		}

		public bool SetBlockId(int x, int y, int z, int id)
		{
			GetAt(x, y, z).BlockId = id;
			return true;
		}

		public bool SetData(int x, int y, int z, int data)
		{
			GetAt(x, y, z).Metadata = data;
			return true;
		}

		public bool SetTileEntity(int x, int y, int z, TagCompound tag)
		{
			GetAt(x, y, z).NBT = tag;
			return true;
		}

		private static BlockSet GetFromCompound(TagCompound root)
		{
			Dictionary<string, string> ps = new Dictionary<string, string>();
			string name = root.GetString("Name");
			if (root.ContainsKey("Properties", TagType.Compound))
			{
				TagCompound tag = root["Properties"] as TagCompound;
				foreach (string key in tag.Keys)
				{
					ps.Add(key, tag.GetString(key));
				}
			}
			IBlock block = GameData.JavaEdition.GetBlock(name);
			int meta = block.GetMetadata(ps);
			return new BlockSet(name, meta);
		}

		public static Structure FromNBT(TagCompound root)
		{
			if (!((root.ContainsKey("version", TagType.Int) || root.ContainsKey("DataVersion", TagType.Int)) && root.ContainsKey("blocks", TagType.List) && root.ContainsKey("palette", TagType.List) && root.ContainsKey("entities", TagType.List) && root.ContainsKey("size", TagType.List)))
			{
				throw new ArgumentException("Unknown format");
			}
			TagList size = root["size"] as TagList;
			if (size == null) throw new ArgumentException("Unknown format");
			int version = root.ContainsKey("version") ? root.GetInt("version") : root.GetInt("DataVersion");
			Structure template = new Structure(((TagInt)size[0]).Value, ((TagInt)size[1]).Value, ((TagInt)size[2]).Value, version);

			TagList palette = root["palette"] as TagList;
			TagList blocks = root["blocks"] as TagList;

			for (int i = 0; i < blocks.Count; i++)
			{
				TagCompound btag = blocks[i] as TagCompound;
				if (btag == null) continue;

				int statue = btag.GetInt("state");

				if (statue >= palette.Count) continue;

				TagCompound cmd = palette[statue] as TagCompound;
				BlockSet block = GetFromCompound(cmd);

				TagList pos = btag["pos"] as TagList;
				if (pos != null && pos.Count == 3)
				{
					block.X = ((TagInt)pos[0]).Value;
					block.Y = ((TagInt)pos[1]).Value;
					block.Z = ((TagInt)pos[2]).Value;
				}

				if (btag.ContainsKey("nbt", TagType.Compound))
				{
					block.NBT = btag["nbt"] as TagCompound;
				}
				template._blockList[block.X, block.Y, block.Z] = block;
			}

			TagList entities = root["entities"] as TagList;
			foreach (TagCompound en in entities)
			{
				TagList _pos = en["pos"] as TagList;
				TagList bpos = en["blockPos"] as TagList;
				if (en.ContainsKey("nbt", TagType.Compound))
				{
					template._entityList.Add(en);
				}
			}

			return template;
		}

		public TagCompound BuildTag()
		{
			List<PaletteObj> palette = new List<PaletteObj>();

			TagList blocks = new TagList("blocks", TagType.Compound);
			for (int x = 0; x < _width; x++)
				for (int y = 0; y < _height; y++)
					for (int z = 0; z < _length; z++)
					{
						BlockSet set = GetAt(x, y, z);
						PaletteObj plt = new PaletteObj() { Name = set.BlockName, Metadata = set.Metadata };
						if (!palette.Contains(plt))
						{
							palette.Add(plt);
						}
						int state = palette.IndexOf(plt);
						TagCompound blk = new TagCompound("") { new TagInt("state", state), new TagList("pos") { new TagInt(set.X), new TagInt(set.Y), new TagInt(set.Z) } };
						if (set.NBT != null)
						{
							blk.Add("nbt", set.NBT);
						}
						blocks.Add(blk);
					}

			TagList paletteTag = new TagList("palette", TagType.Compound);
			for (int i = 0; i < palette.Count; i++)
			{
				paletteTag.Add(palette[i].BuildTag());
			}
			TagList entities = new TagList("entities", TagType.Compound);
			foreach(TagCompound e in _entityList)
			{
				entities.Add(e);
			}

			TagList size = new TagList("size") { new TagInt(_width), new TagInt(_height), new TagInt(_length) };
			TagCompound root = new TagCompound() { new TagInt("DataVersion", _dataVersion), new TagString("author", _author), paletteTag, blocks, entities, size };
			return root;
		}

		private class PaletteObj : ITagProvider<TagCompound>
		{
			public string Name { get; set; }
			public int Metadata { get; set; }

			public override int GetHashCode()
			{
				return (int)((Metadata ^ ((Name != null) ? Name.GetHashCode() : 125)));
			}

			public override bool Equals(object obj)
			{
				PaletteObj plt = obj as PaletteObj;
				return plt != null && plt.Name == Name && plt.Metadata == Metadata;
			}

			public TagCompound BuildTag()
			{
				TagCompound compound = new TagCompound() { new TagString("Name", this.Name) };
				IBlock blockInfo = GameData.JavaEdition.GetBlock(Name);
				IDictionary<string, string> properties = blockInfo.GetProperties(Metadata);
				if (properties.Count > 0)
				{
					TagCompound ps = new TagCompound("Properties");
					foreach (string key in properties.Keys)
					{
						ps.Add(key, properties[key]);
					}
					compound.Add("Properties", ps);
				}
				return compound;

			}
		}

		private class BlockSet : ITagProvider<TagCompound>
		{
			public int X { get; set; }
			public int Y { get; set; }
			public int Z { get; set; }

			public string BlockName { get; set; }
			public int Metadata { get; set; }
			public TagCompound NBT { get; set; }

			private int _blockId = -1;

			public int BlockId
			{
				get
				{
					if (_blockId == -1)
					{
						IBlock blockInfo = GameData.JavaEdition.GetBlock(BlockName);
						if (blockInfo != null)
						{
							_blockId = blockInfo.Id;
						}
					}
					return _blockId;
				}
				set
				{
					try
					{
						_blockId = value;
						IBlock blockInfo = GameData.JavaEdition.GetBlock(_blockId);

						if (blockInfo != null)
						{
							BlockName = blockInfo.Name;
						}

					}
					catch (Exception)
					{

					}
				}
			}
			public BlockSet(string id, int meta)
				: this(0, 0, 0, id, meta) { }

			public BlockSet(string id, int meta, TagCompound nbt)
				: this(0, 0, 0, id, meta, nbt) { }

			public BlockSet(int x, int y, int z, string id, int meta)
				: this(x, y, z, id, meta, null) { }

			protected BlockSet(int x, int y, int z, string id, int meta, TagCompound nbt)
			{
				this.X = x;
				this.Y = y;
				this.Z = z;
				this.BlockName = id;
				this.Metadata = meta;
				this.NBT = nbt;
			}

			public override int GetHashCode()
			{
				return BlockId.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				BlockSet bs = obj as BlockSet;
				if (bs != null)
				{
					bool flag = (this.BlockId == bs.BlockId) && (bs.Metadata == this.Metadata);
					if ((this.Y < 0) && (bs.Y < 0))
					{
						return flag;
					}
					return (bs.X == this.X) && (bs.Y == this.Y) && (bs.Z == this.Z) && (flag);
				}
				return false;
			}

			public static BlockSet FromNBT(TagCompound nbt)
			{
				string id = nbt.GetString("Block");
				int meta = 0;
				if (nbt.ContainsKey("Meta", TagType.Byte))
				{
					meta = nbt.GetByte("Meta");
				}
				else if (nbt.ContainsKey("Meta", TagType.Int))
				{
					meta = nbt.GetInt("Meta");
				}
				if (nbt.ContainsKey("TagData"))
				{
					TagCompound tagCompound = nbt["TagData"] as TagCompound;
					return new BlockSet(id, meta, tagCompound);
				}
				return new BlockSet(id, meta);
			}

			public TagCompound BuildTag()
			{
				TagCompound nbt = new TagCompound()
				{
					{ "Block", this.BlockName },
					{ "Meta", (byte)this.Metadata }
				};
				if (NBT != null)
				{
					nbt.Add("TagData", this.NBT);
				}
				return nbt;
			}
		}
	}
}

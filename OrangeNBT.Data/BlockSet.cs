using OrangeNBT.NBT;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrangeNBT.Data
{
    public class BlockSet : ICloneable
    {
		private readonly IBlock _block = new AirBlock();
		private readonly int _metadata;

		public IBlock Block => _block;
		public int Metadata => _metadata;

		public string Name
		{
			get
			{
				return _block.GetName(_metadata);
			}
		}

		public bool IsAir
		{
			get { return Block.Name == "minecraft:air"; }
		}

		public BlockSet(BlockSet blockSet)
		{
			_block = blockSet.Block;
			_metadata = blockSet.Metadata;
		}

		public BlockSet(IBlock block, int metadata)
		{
			if (block != null)
				_block = block;
			_metadata = metadata;
		}

		public BlockSet(IBlock block, TagCompound properties)
		{
			_block = block;
			Dictionary<string, string> ps = new Dictionary<string, string>();
			foreach(TagString tag in properties)
			{
				ps.Add(tag.Name, tag.Value);
			}
			_metadata = _block.GetMetadata(ps);

		}

		public BlockSet(IBlock block, IDictionary<string,string> properties)
		{
			_block = block;
			_metadata = _block.GetMetadata(properties);
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", Name, _metadata);
		}

		public TagCompound PersistentId()
		{
			return new TagCompound() { new TagString("name", Name), new TagInt("val", _metadata) };
		}

		public override bool Equals(object obj)
		{
			BlockSet target = obj as BlockSet;
			return target == null ? false : (target._block == _block && target._metadata == _metadata);
		}

		public override int GetHashCode()
		{
			return _block.GetHashCode() ^ _metadata;
		}

		public object Clone()
		{
			return new BlockSet(this);
		}

		public class AirBlock : IBlock
		{
			public int Luminance => 0;

			public int Opacity => 0;

			public bool HasTileEntity => false;

			public int Id => 0;

			public string Name => "minecraft:air";

			public TagCompound BuildTileEntity()
			{
				return null;
			}

			public int GetMetadata(IDictionary<string, string> properties)
			{
				return 0;
			}

			public string GetName(int metadata)
			{
				return "minecraft:air";
			}

			public IDictionary<string, string> GetProperties(int metadata)
			{
				return new Dictionary<string, string>();
			}
		}
	}
}

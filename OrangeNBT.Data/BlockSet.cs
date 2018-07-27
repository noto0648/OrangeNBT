using OrangeNBT.Data.Anvil.Helper;
using OrangeNBT.NBT;
using System;
using System.Collections.Generic;

namespace OrangeNBT.Data
{
	public class BlockSet : ICloneable
    {
		private readonly IBlock _block = new AirBlock();
		private readonly int _systemId;
		public IBlock Block => _block;

		[Obsolete]
		public int Metadata => _systemId;
		public int RuntimeId => _systemId;

		private IDictionary<string, string> _properties = new Dictionary<string, string>();
		public IDictionary<string, string> Properties => _properties;

		public string Name
		{
			get
			{
				return _block.GetName(_systemId);
			}
		}

		public bool IsAir
		{
			get { return Block.Name == "minecraft:air"; }
		}

		public BlockSet(BlockSet blockSet)
		{
			_block = blockSet.Block;
			_systemId = blockSet._systemId;
			_properties = CloneDictionary(blockSet.Properties);
		}

		public BlockSet(IBlock block)
		{
			if (block != null)
				_block = block;
		}

		public BlockSet(IBlock block, int metadata)
		{
			if (block != null)
				_block = block;
			_systemId = metadata;
		}

		public BlockSet(IBlock block, IDictionary<string, string> properties)
		{
			_block = block;
			_properties = properties;
			_systemId = block.GetRuntimeId(properties);
		}

		public BlockSet(IBlock block, TagCompound properties)
		{
			_block = block;
			_properties = PropertyConverter.From(properties);
			_systemId = block.GetRuntimeId(_properties);
		}

		public BlockSet(IBlock block, TagCompound properties, int systemId)
		{
			_block = block;
			_properties = PropertyConverter.From(properties);
			_systemId = systemId;
		}

		public BlockSet(IBlock block, IDictionary<string, string> properties, int systemId)
		{
			_block = block;
			_properties = properties;
			_systemId = systemId;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", Name, _systemId);
		}

		public TagCompound PersistentId()
		{
			return new TagCompound() { new TagString("name", Name), new TagInt("val", _systemId) };
		}

		public override bool Equals(object obj)
		{
			BlockSet target = obj as BlockSet;
			return target == null ? false : (target._block == _block && target._systemId == _systemId && EqualsProperties(target.Properties, Properties));
		}

		public override int GetHashCode()
		{
			return _block.GetHashCode() ^ _systemId;
		}

		public object Clone()
		{
			return new BlockSet(this);
		}

		internal static IDictionary<string, string> CloneDictionary(IDictionary<string, string> input)
		{
			Dictionary<string, string> r = new Dictionary<string, string>(input.Count);
			foreach(string key in input.Keys)
			{
				r.Add(key, input[key]);
			}
			return r;
		}

		internal static bool EqualsProperties(IDictionary<string, string> p1, IDictionary<string, string> p2)
		{
			if (p1 == null && p2 == null)
				return true;
			if (p1 == null || p2 == null)
				return false;
			if (p1.Count != p2.Count)
				return false;
			foreach (string key in p1.Keys)
			{
				if (!p2.ContainsKey(key))
					return false;
				if (p1[key] != p2[key])
					return false;
			}
			return true;
		}

		public class AirBlock : IBlock
		{
			public int Luminance => 0;

			public int Opacity => 0;

			public bool HasTileEntity => false;

			public int Id => 0;

			public string Name => "minecraft:air";

			public BlockSet DefaultBlockSet => throw new NotImplementedException();

			public TagCompound BuildTileEntity()
			{
				return null;
			}

			public string GetName(int metadata)
			{
				return "minecraft:air";
			}

			public IDictionary<string, string> GetProperties(int metadata)
			{
				return new Dictionary<string, string>();
			}

			public BlockSet GetBlock(IDictionary<string, string> properties)
			{
				return new BlockSet(this);
			}

			public int GetRuntimeId(IDictionary<string, string> properties)
			{
				return 0;
			}
		}
	}
}

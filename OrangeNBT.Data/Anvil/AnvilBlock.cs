using OrangeNBT.NBT;
using System.Collections.Generic;

namespace OrangeNBT.Data.Anvil
{
	public class AnvilBlock : IBlock
	{
		private int _luminance;
		private int _opacity = 255;
		private bool _hasTileEntity;
		private TagCompound _tileEntity;
		private readonly int _id;
		private readonly string _name;

		public int Luminance => _luminance;

		public int Opacity => _opacity;

		public bool HasTileEntity => _hasTileEntity;

		public int Id => _id;

		public string Name => GetName(0);

		public BlockSet DefaultBlockSet => new BlockSet(this, 0);

		private Metadata[] _metadatas = new Metadata[16];

		public AnvilBlock(int id, string name)
		{
			_id = id;
			_name = name;
		}

		#region Chain methods
		public AnvilBlock Lum(int luminance)
		{
			_luminance = luminance;
			return this;
		}

		public AnvilBlock Opa(int opacity)
		{
			_opacity = opacity;
			return this;
		}

		public AnvilBlock Tile(TagCompound tag)
		{
			_hasTileEntity = true;
			_tileEntity = tag;
			return this;
		}

		#endregion

		public void SetMetadataInfo(int meta, string str, IDictionary<string, string> args)
		{
			_metadatas[meta] = new Metadata() { Id = str, Data = meta, Properties = args };
		}

		public TagCompound BuildTileEntity()
		{
			if (_hasTileEntity)
				return _tileEntity;
			return null;
		}

		public string GetName(int metadata)
		{
			if (metadata < _metadatas.Length &&  _metadatas[metadata] != null)
				return string.Format("minecraft:{0}", _metadatas[metadata].Id);
			return string.Format("minecraft:{0}", _name);
		}

		public IDictionary<string, string> GetProperties(int metadata)
		{
			if (metadata < _metadatas.Length && _metadatas[metadata] != null)
				return _metadatas[metadata].Properties;
			if (_metadatas.Length > 0 && _metadatas[0] != null)
				return _metadatas[0].Properties;

			return new Dictionary<string, string>();
		}

		public BlockSet GetBlock(IDictionary<string, string> properties)
		{
			return new BlockSet(this, GetRuntimeId(properties));
		}

		public int GetRuntimeId(IDictionary<string, string> properties)
		{
			for (int i = 0; i < _metadatas.Length; i++)
			{
				if (_metadatas[i] != null)
				{
					if (BlockSet.EqualsProperties(_metadatas[i].Properties, properties))
					{
						return _metadatas[i].Data;
					}
				}
			}
			return 0;
		}


		private class Metadata
		{
			public int Data { get; set; }
			public string Id { get; set; }
			public IDictionary<string, string> Properties {get;set;}
		}
	}
}

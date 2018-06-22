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

		private Metadata[] _metadatas;

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

		public int GetMetadata(IDictionary<string, string> properties)
		{
			for(int i = 0; i < _metadatas.Length; i++)
			{
				if(_metadatas[i] != null)
				{
					if (_metadatas[i].Properties == null || _metadatas[i].Properties.Count != properties.Count)
					{
						return 0;
					}
					bool breakFlag = false;
					foreach(string key in _metadatas[i].Properties.Keys)
					{
						if (!properties.ContainsKey(key) || properties[key] != _metadatas[i].Properties[key])
						{
							breakFlag = true;
							break;
						}
					}
					if(!breakFlag)
					{
						return i;
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

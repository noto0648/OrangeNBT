using System;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.NBT;

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

		public TagCompound BuildTileEntity()
		{
			if (_hasTileEntity)
				return _tileEntity;
			return null;
		}

		public string GetName(int metadata)
		{
			return string.Format("minecraft:{0}", _name);
		}

		public IDictionary<string, string> GetParamerters(int metadata)
		{
			throw new NotImplementedException();
		}
	}
}

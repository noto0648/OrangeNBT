using OrangeNBT.Data;
using OrangeNBT.Helper;
using OrangeNBT.NBT;
using OrangeNBT.World.Core;

namespace OrangeNBT.World.Anvil
{
	public class AnvilSectionClassic : AnvilSection
    {
		private byte[] _blocks;             //lsb_array
		private NibbleArray _addBlocks;     //msb_array
		private NibbleArray _data;

		public AnvilSectionClassic(int y, bool makeSkyLight)
			:base(y, makeSkyLight)
		{
			_blocks = new byte[4096];
			_data = new NibbleArray();
		}

		public override BlockSet GetBlock(int x, int y, int z)
		{
			return new BlockSet(GameData.JavaEdition.GetBlock(GetBlockId(x, y, z)), _data[x, y, z]);
		}

		public override void Load(TagCompound c)
		{
			_blocks = c.GetByteArray("Blocks");

			if (c.ContainsKey("Add", TagType.ByteArray))
			{
				_addBlocks = (new NibbleArray(c.GetByteArray("Add"), 4));
			}
			_data = (new NibbleArray(c.GetByteArray("Data"), 4));
			base.Load(c);

		}

		private int GetBlockId(int x, int y, int z)
		{
			int block = _blocks[y << 8 | z << 4 | x] & 255;
			if (_addBlocks != null)
			{
				block |= _addBlocks[x, y, z] << 8;
			}
			return block;
		}


		public override bool SetBlock(int x, int y, int z, BlockSet block)
		{
			SetBlockId(x, y, z, block.Block.Id);
			_data[x, y, z] = block.Metadata;
			return true;
		}

		private bool SetBlockId(int x, int y, int z, int id)
		{
			int blockOrigin = _blocks[y << 8 | z << 4 | x] & 255;

			if (_addBlocks != null)
			{
				blockOrigin |= _addBlocks[x, y, z] << 8;
			}

			if (blockOrigin != 0)
			{
				_modifyCount--;
			}
			if (id != 0)
			{
				_modifyCount++;
			}
			_blocks[y << 8 | z << 4 | x] = (byte)(id & 255);

			if (id > 255)
			{
				if (_addBlocks == null)
				{
					_addBlocks = new NibbleArray(_blocks.Length);
				}

				_addBlocks[x, y, z] = (id & 3840) >> 8;
			}
			else if (_addBlocks != null)
			{
				_addBlocks[x, y, z] = 0;
			}
			return true;
		}

		public override TagCompound BuildTag()
		{
			TagCompound c = new TagCompound()
			{
				new TagByteArray("Blocks", _blocks),
				new TagByteArray("Data", _data),
				new TagByteArray("BlockLight", _blockLight),
				new TagByteArray("SkyLight", _skyLight),
				new TagByte("Y", (byte)_y)
			};
			if (_addBlocks != null)
				c.Add("Add", new TagByteArray(_addBlocks));
			return c;
		}

	}
}

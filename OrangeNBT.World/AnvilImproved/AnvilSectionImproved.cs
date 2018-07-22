using OrangeNBT.Data;
using OrangeNBT.Helper;
using OrangeNBT.NBT;
using OrangeNBT.World.Anvil;
using System;

namespace OrangeNBT.World.AnvilImproved
{
	public class AnvilSectionImproved : AnvilSection
    {
		//TODO : for memory, change the system-id list
		private BlockSet[,,] _blocks;

		public AnvilSectionImproved(int y, bool makeSkyLight)
			: base(y, makeSkyLight)
		{
			_blocks = new BlockSet[Width, Height, Length];
		}

		public override BlockSet GetBlock(int x, int y, int z)
		{
			return _blocks[x, y, z];
		}

		public override bool SetBlock(int x, int y, int z, BlockSet block)
		{
			_blocks[x, y, z] = block;
			return true;
		}

		public override void Load(TagCompound c)
		{
			TagList paletteList = (TagList)c["Palette"];
			long[] data = c.GetLongArray("BlockStates");
			BlockSet[] palette = new BlockSet[paletteList.Count];
			for(int i = 0; i < palette.Length; i++)
			{
				TagCompound r = (TagCompound)paletteList[i];
				if(r.ContainsKey("Properties", TagType.String))
					palette[i] = new BlockSet(GameData.JavaEdition.GetBlock(r.GetString("Name")), (TagCompound)r["Properties"]);
				else
					palette[i] = new BlockSet(GameData.JavaEdition.GetBlock(r.GetString("Name")), 0);
			}
			DenseArray array = new DenseArray(data, data.Length * 64 / 4096);

			for (int y = 0; y < Height; y++)
			{
				for (int z = 0; z < Length; z++)
				{
					for (int x = 0; x < Width; x++)
					{
						int blockIndex = (((y * Height) + z) * Width) + x;
						int val = array[blockIndex];
						SetBlock(x, y, z, palette[val]);
					}
				}
			}
			base.Load(c);
		}

		public override TagCompound BuildTag()
		{
			BlockPalette palette = new BlockPalette();
			int[] indexList = new int[Width * Height * Length];

			for (int y = 0; y < Height; y++)
			{
				for (int z = 0; z < Length; z++)
				{
					for (int x = 0; x < Width; x++)
					{
						int blockIndex = (((y * Height) + z) * Width) + x;
						indexList[blockIndex] = palette.GetIndex(GetBlock(x, y, z));
					}
				}
			}
			int bits = (int)Math.Max(4, Math.Ceiling(Math.Log(palette.Count, 2)));
			DenseArray array = new DenseArray(bits, indexList.Length);
			for (int i = 0; i < indexList.Length; i++)
			{
				array[i] = indexList[i];
			}

			return new TagCompound()
			{
				palette.BuildTag(),
				new TagLongArray("BlockStates", array.RawArray),
				new TagByteArray("BlockLight", _blockLight),
				new TagByteArray("SkyLight", _skyLight),
				new TagByte("Y", (byte)_y)
			};
		}
		
	}
}

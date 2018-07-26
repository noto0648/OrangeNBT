using OrangeNBT.Data;
using OrangeNBT.Helper;
using OrangeNBT.NBT;
using OrangeNBT.World.Anvil;
using System;
using System.Collections.Generic;

namespace OrangeNBT.World.AnvilImproved
{
	public class AnvilSectionImproved : AnvilSection
    {
		private int[,,] _blocks;

		public AnvilSectionImproved(int y, bool makeSkyLight)
			: base(y, makeSkyLight)
		{
			_blocks = new int[Width, Height, Length];
		}

		public override BlockSet GetBlock(int x, int y, int z)
		{
			IBlock block = GameData.JavaEdition.GetBlock(_blocks[x, y, z]);
			IDictionary<string, string> properties = block.GetProperties(_blocks[x, y, z]);
			return block.GetBlock(properties);
		}

		public override bool SetBlock(int x, int y, int z, BlockSet block)
		{
			_blocks[x, y, z] =  block.RuntimeId;
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
				IBlock block = GameData.JavaEdition.GetBlock(r.GetString("Name"));
				if (r.ContainsKey("Properties", TagType.Compound))
				{
					Dictionary<string, string> ps = new Dictionary<string, string>();
					foreach (TagString tag in (TagCompound)r["Properties"])
					{
						ps.Add(tag.Name, tag.Value);
					}
					palette[i] = block.GetBlock(ps);
				}
				else
				{
					palette[i] = new BlockSet(block, block.DefaultBlockSet.Properties, block.DefaultBlockSet.RuntimeId);
				}
			}
			DenseArray array = new DenseArray(data, data.Length * 64 / 4096);

			for (int y = 0; y < Height; y++)
			{
				for (int z = 0; z < Length; z++)
				{
					for (int x = 0; x < Width; x++)
					{
						int blockIndex = (y * Height + z) * Width + x;
						int val = array[blockIndex];
						_blocks[x, y, z] = palette[val].RuntimeId;
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
						int blockIndex = (y * Height + z) * Width + x;
						indexList[blockIndex] = palette.GetIndex(_blocks[x, y, z]);
					}
				}
			}
			int bits = (int)Math.Max(4, Math.Ceiling(Math.Log(palette.Count, 2)));
			DenseArray array = new DenseArray(bits, Width * Height * Length * bits / 64);
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

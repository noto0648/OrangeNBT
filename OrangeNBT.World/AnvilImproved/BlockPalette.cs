using OrangeNBT.Data;
using OrangeNBT.NBT;
using OrangeNBT.NBT.IO;
using System;
using System.Collections.Generic;

namespace OrangeNBT.World.AnvilImproved
{
	public class BlockPalette : ITagProvider<TagList>
    {
		private List<BlockSet> _blockSets;
		private Dictionary<int, int> _blocks = new Dictionary<int, int>();
		public int Count
		{
			get { return _blockSets.Count; }
		}

		public BlockPalette(int size)
		{
			_blockSets = new List<BlockSet>(size);
		}

		public BlockPalette()
		{
			_blockSets = new List<BlockSet>();
		}

		public int GetIndex(int runtimeId)
		{
			if(_blocks.ContainsKey(runtimeId))
			{
				return _blocks[runtimeId];
			}

			int index = _blockSets.Count;
			IBlock block = GameData.JavaEdition.GetBlock(runtimeId);
			_blockSets.Add(new BlockSet(block, block.GetProperties(runtimeId), runtimeId));
			_blocks.Add(runtimeId, index);
			return index;
		}

		public int GetIndex(BlockSet block)
		{
			if (block == null) return 0;
			int index = _blockSets.IndexOf(block);
			if (index == -1)
			{
				_blockSets.Add(block);
				return _blockSets.Count - 1;
			}
			return index;
		}

		public TagList BuildTag()
		{
			TagList palette = new TagList("Palette", TagType.Compound);
			for(int i = 0; i < Count; i++)
			{
				BlockSet block = _blockSets[i];
				TagCompound tag = new TagCompound(string.Empty)
				{
					new TagString("Name", block.Name)
				};
				IDictionary<string, string> properties = block.Properties;//block.Block.GetProperties(block.Metadata);
				if (properties != null && properties.Count > 0)
				{
					TagCompound props = new TagCompound("Properties");
					foreach(string key in properties.Keys)
					{
						props.Add(new TagString(key, properties[key]));
					}
					tag.Add(props);
				}
				palette.Add(tag);
			}
			return palette;
		}
	}
}

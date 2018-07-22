using OrangeNBT.Data;
using OrangeNBT.NBT;
using System.Collections.Generic;

namespace OrangeNBT.World.AnvilImproved
{
	public class BlockPalette : ITagProvider<TagList>
    {
		private List<BlockSet> _blockSets;

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
				IDictionary<string, string> properties = block.Block.GetProperties(block.Metadata);
				if (properties != null && properties.Count > 0)
				{
					TagList props = new TagList("Properties", TagType.String);
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

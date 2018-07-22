using OrangeNBT.NBT;

namespace OrangeNBT.World.AnvilImproved
{
	public class HeightMap : ITagProvider<TagLongArray>
    {
		private readonly string _name;

		private long[] _data;	//0~5
		//private byte[] _data;
		public HeightMap(string name, long[] longArray)
		{
			_name = name;
			_data = new long[36];
		}


		public TagLongArray BuildTag()
		{
			return new TagLongArray(_name, _data);
		}

		public static HeightMap Load(TagLongArray array)
		{
			return new HeightMap(array.Name, array.Value);
		}
	}
}

using OrangeNBT.Data;
using OrangeNBT.NBT;

namespace OrangeNBT.World.AnvilImproved
{
	public class HeightMap : ITagProvider<TagLongArray>
    {
		private const int BasicSize = 256;

		private readonly string _name;

		private DenseArray _array;

		public HeightMap(string name)
		{
			_name = name;
			_array = new DenseArray(9, 36);
		}

		public HeightMap(string name, long[] longArray)
		{
			_name = name;
			//_data = new long[36];
			_array = new DenseArray(longArray.Length * 64 / BasicSize, longArray.Length);
		}


		public TagLongArray BuildTag()
		{
			return new TagLongArray(_name, _array.RawArray);
		}

		public static HeightMap Load(TagLongArray array)
		{
			return new HeightMap(array.Name, array.Value);
		}
	}
}

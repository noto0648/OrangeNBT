using OrangeNBT.Data;
using OrangeNBT.NBT;

namespace OrangeNBT.World.AnvilImproved
{
	public class HeightMap : ITagProvider<TagLongArray>
    {
		private const int BasicSize = 256;

		private readonly string _name;

		private DenseArray _array;

		public int this[int index]
		{
			get { return _array[index]; }
			set { _array[index] = value; }
		}

		public HeightMap(string name)
		{
			_name = name;
			_array = new DenseArray(9, 256);
		}

		public HeightMap(string name, long[] longArray)
		{
			_name = name;
			_array = new DenseArray(longArray.Length * 64 / BasicSize, 256);
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

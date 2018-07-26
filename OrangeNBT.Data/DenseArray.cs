namespace OrangeNBT.Data
{
	public class DenseArray
    {
		private readonly int _bitsPerBlock;
		private long[] _data;
		private readonly uint _mask;

		public long[] RawArray => _data;

		public int this[int index]
		{
			get { return Get(index); }
			set { Set(index, value); }
		}

		public int Length
		{
			get { return _data.Length * 64 / _bitsPerBlock; }
		}

		public DenseArray(int bitsPerBlock, int size)
		{
			_bitsPerBlock = bitsPerBlock;
			_data = new long[size];
			_mask = (uint)((1 << _bitsPerBlock) - 1);
		}

		public DenseArray(long[] raw, int bitsPerBlock)
		{
			_data = raw;
			_bitsPerBlock = bitsPerBlock;
			_mask = (uint)((1 << _bitsPerBlock) - 1);
		}

		private int Get(int index)
		{
			int startLong = (index * _bitsPerBlock) / 64;
			int startOffset = (index * _bitsPerBlock) % 64;
			int endLong = ((index + 1) * _bitsPerBlock - 1) / 64;
			ulong val;
			if (startLong == endLong)
			{
				val = (((ulong)_data[startLong]) >> startOffset) & _mask;
			}
			else
			{
				int endOffset = 64 - startOffset;
				val = ((ulong)_data[startLong] >> startOffset | (ulong)_data[endLong] << endOffset) & _mask;
			}
			return (int)val;
		}

		private void Set(int index, int val)
		{
			int startLong = (index * _bitsPerBlock) / 64;
			int startOffset = (index * _bitsPerBlock) % 64;
			int endLong = ((index + 1) * _bitsPerBlock - 1) / 64;

			uint nval = (uint)val;
			nval &= _mask;

			_data[startLong] = _data[startLong] & ~(_bitsPerBlock << startOffset) | ((long)nval &_mask) << startOffset;

			if (startLong != endLong)
			{
				_data[endLong] = (nval >> (64 - startOffset));
			}
		}
	}
}

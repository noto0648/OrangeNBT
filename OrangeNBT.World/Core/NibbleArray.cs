
namespace OrangeNBT.World.Core
{
    public class NibbleArray
    {
        private byte[] _bytes;
        private int _depthBits;
        private int _length;

        public byte[] Bytes { get { return _bytes; } }

        public int Length { get { return _length; } }

        public NibbleArray()
            : this(4096, 4) { }

        public NibbleArray(int length)
            : this(length, 4) { }

        public NibbleArray(int length, int bit)
        {
            _bytes = new byte[length / 2];
            _length = length;
            _depthBits = bit;
        }

        public NibbleArray(byte[] baseArray, int bit)
        {
            _bytes = baseArray;
            _depthBits = bit;
        }

        public int this[int x, int y, int z]
        {
            get
            {
                return GetFromIndex(GetIndex(x, y, z));
            }
            set
            {
                SetImpl(GetIndex(x, y, z), value);
            }
        }

        private bool IsLowerNibble(int index)
        {
            return (index & 1) == 0;
        }

        private int GetIndex(int x, int y, int z)
        {
            return (y << (_depthBits + 4) | z << _depthBits | x);
        }

        private int GetNibbleIndex(int index)
        {
            return index >> 1;
        }

        private int GetFromIndex(int index)
        {
            int i = GetNibbleIndex(index);
            return IsLowerNibble(index) ? _bytes[i] & 15 : _bytes[i] >> 4 & 15;
        }

        private void SetImpl(int index, int value)
        {
            int i = GetNibbleIndex(index);

            if (IsLowerNibble(index))
            {
                _bytes[i] = (byte)(_bytes[i] & 240 | value & 15);
            }
            else
            {
                _bytes[i] = (byte)(_bytes[i] & 15 | (value & 15) << 4);
            }
        }

        public override int GetHashCode()
        {
            return _bytes.Length ^ _depthBits;
        }

        public static implicit operator byte[](NibbleArray array)
        {
            return array.Bytes;
        }
    }
}

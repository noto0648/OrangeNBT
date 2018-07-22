using OrangeNBT.Data;
using OrangeNBT.Helper;
using OrangeNBT.NBT;
using OrangeNBT.World.Core;
using System;

namespace OrangeNBT.World.Anvil
{
    public abstract class AnvilSection : IWorldAccess, ITagProvider<TagCompound>
    {
        public const int Width = 16;
        public const int Height = 16;
        public const int Length = 16;

		protected readonly int _y;
        public int Y { get { return _y; } }

        protected NibbleArray _blockLight;
		protected NibbleArray _skyLight;

		protected int _modifyCount;

        public bool IsEmpty { get { return _modifyCount == 0; } }

        public AnvilSection(int y, bool makeSkyLight)
        {
            _y = y;
            _blockLight = new NibbleArray();

            if (makeSkyLight)
            {
                _skyLight = new NibbleArray();
            }
        }

        public bool SetSkyLight(int x, int y, int z, int light)
        {
            _skyLight[x, y, z] = light;
            return true;
        }

        public bool SetBlockLight(int x, int y, int z, int light)
        {
            _blockLight[x, y, z] = light;
            return true;
        }

        public bool SetHeight(int x, int z, int height)
        {
            throw new NotSupportedException();
        }

        public int GetSkyLight(int x, int y, int z)
        {
            return _skyLight[x, y, z];
        }

        public int GetBlockLight(int x, int y, int z)
        {
            return _blockLight[x, y, z];
        }

        public int GetHeight(int x, int z)
        {
            throw new NotSupportedException();
        }

		public abstract BlockSet GetBlock(int x, int y, int z);

		public abstract bool SetBlock(int x, int y, int z, BlockSet block);

		public virtual void Load(TagCompound c)
		{
			_blockLight = (new NibbleArray(c.GetByteArray("BlockLight"), 4));
			_skyLight = (new NibbleArray(c.GetByteArray("SkyLight"), 4));
		}

		public abstract TagCompound BuildTag();

		public TagCompound GetTileEntity(int x, int y, int z)
        {
            throw new NotSupportedException();
        }

        public bool SetTileEntity(int x, int y, int z, TagCompound tag)
        {
            throw new NotSupportedException();
        }

        public int GetBiome(int x, int z)
        {
            throw new NotSupportedException();
        }

        public bool SetBiome(int x, int z, int biome)
        {
            throw new NotSupportedException();
        }


	}
}

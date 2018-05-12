using OrangeNBT.Helper;
using OrangeNBT.NBT;
using OrangeNBT.World.Core;
using System;

namespace OrangeNBT.World.Anvil
{
    public class AnvilSection : IBlockAccess, ITagProvider<TagCompound>
    {
        public const int Width = 16;
        public const int Height = 16;
        public const int Length = 16;

        private int _y;
        public int Y { get { return _y; } set { _y = Math.Max(0, Y); } }

        private byte[] _blocks;             //lsb_array
        private NibbleArray _addBlocks;     //msb_array
        private NibbleArray _data;
        private NibbleArray _blockLight;
        private NibbleArray _skyLight;

        private int _modifyCount;

        public bool IsEmpty { get { return _modifyCount == 0; } }

        public AnvilSection(int y, bool makeSkyLight)
        {
            _y = y;

            _blocks = new byte[4096];
            _data = new NibbleArray();
            _blockLight = new NibbleArray();

            if (makeSkyLight)
            {
                _skyLight = new NibbleArray();
            }
        }

        public int GetBlockId(int x, int y, int z)
        {
            int block = _blocks[y << 8 | z << 4 | x] & 255;
            if (_addBlocks != null)
            {
                block |= _addBlocks[x, y, z] << 8;
            }
            return block;
        }

        public int GetBlockData(int x, int y, int z)
        {
            return _data[x, y, z];
        }

        public bool SetBlockId(int x, int y, int z, int id)
        {
            int blockOrigin = _blocks[y << 8 | z << 4 | x] & 255;

            if (_addBlocks != null)
            {
                blockOrigin |= _addBlocks[x, y, z] << 8;
            }

            if (blockOrigin != 0)
            {
                _modifyCount--;
            }
            if (id != 0)
            {
                _modifyCount++;
            }
            _blocks[y << 8 | z << 4 | x] = (byte)(id & 255);

            if (id > 255)
            {
                if (_addBlocks == null)
                {
                    _addBlocks = new NibbleArray(_blocks.Length);
                }

                _addBlocks[x, y, z] = (id & 3840) >> 8;
            }
            else if (_addBlocks != null)
            {
                _addBlocks[x, y, z] = 0;
            }
            return true;
        }

        public bool SetData(int x, int y, int z, int data)
        {
            _data[x, y, z] = data;
            return true;
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

        public void Load(TagCompound c)
        {
            _blocks = c.GetByteArray("Blocks");

            if (c.ContainsKey("Add", TagType.ByteArray))
            {
                _addBlocks = (new NibbleArray(c.GetByteArray("Add"), 4));
            }
            _data = (new NibbleArray(c.GetByteArray("Data"), 4));
            _blockLight = (new NibbleArray(c.GetByteArray("BlockLight"), 4));
            _skyLight = (new NibbleArray(c.GetByteArray("SkyLight"), 4));

        }

        public TagCompound BuildTag()
        {
            TagCompound c = new TagCompound()
            {
                new TagByteArray("Blocks", _blocks),
                new TagByteArray("Data", _data),
                new TagByteArray("BlockLight", _blockLight),
                new TagByteArray("SkyLight", _skyLight),
                new TagByte("Y", (byte)_y)
            };
            if (_addBlocks != null)
                c.Add("Add", new TagByteArray(_addBlocks));
            return c;
        }

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

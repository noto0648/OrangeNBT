using System;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.NBT;

namespace OrangeNBT.Data
{
    public interface IBlockAccess
    {
        //int GetBlockData(int x, int y, int z);

        //int GetBlockId(int x, int y, int z);

        TagCompound GetTileEntity(int x, int y, int z);

		//bool SetData(int x, int y, int z, int data);

		//bool SetBlockId(int x, int y, int z, int id);

		//bool SetBlock(int x, int y, int z, IBlock block, int metadata = 0);

		BlockSet GetBlock(int x, int y, int z);

		bool SetBlock(int x, int y, int z, BlockSet block);

        bool SetTileEntity(int x, int y, int z, TagCompound tag);

    }
}

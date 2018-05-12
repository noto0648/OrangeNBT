using System;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.NBT;

namespace OrangeNBT.World.Core
{
    public interface IBlockAccess
    {
        int GetBlockData(int x, int y, int z);

        int GetBlockId(int x, int y, int z);

        int GetSkyLight(int x, int y, int z);

        int GetBlockLight(int x, int y, int z);

        int GetHeight(int x, int z);

        TagCompound GetTileEntity(int x, int y, int z);

        int GetBiome(int x, int z);

        bool SetData(int x, int y, int z, int data);

        bool SetBlockId(int x, int y, int z, int id);

        bool SetSkyLight(int x, int y, int z, int light);

        bool SetBlockLight(int x, int y, int z, int light);

        bool SetHeight(int x, int z, int height);

        bool SetTileEntity(int x, int y, int z, TagCompound tag);

        bool SetBiome(int x, int z, int biome);
    }
}

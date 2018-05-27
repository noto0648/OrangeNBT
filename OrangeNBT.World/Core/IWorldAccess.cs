using System;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.Data;
using OrangeNBT.NBT;

namespace OrangeNBT.World.Core
{
    public interface IWorldAccess : IBlockAccess
    {
        int GetSkyLight(int x, int y, int z);

        int GetBlockLight(int x, int y, int z);

        int GetHeight(int x, int z);

        int GetBiome(int x, int z);

        bool SetSkyLight(int x, int y, int z, int light);

        bool SetBlockLight(int x, int y, int z, int light);

        bool SetHeight(int x, int z, int height);

        bool SetBiome(int x, int z, int biome);
    }
}

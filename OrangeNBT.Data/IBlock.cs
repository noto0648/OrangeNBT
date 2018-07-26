using OrangeNBT.NBT;
using System.Collections.Generic;

namespace OrangeNBT.Data
{
    public interface IBlock
    {
        int Luminance { get; }

        int Opacity { get; }

        bool HasTileEntity { get; }

        int Id { get; }

		string Name { get; }

		BlockSet DefaultBlockSet { get; }

		TagCompound BuildTileEntity();

        string GetName(int metadata);

        IDictionary<string, string> GetProperties(int runtimeId);

		BlockSet GetBlock(IDictionary<string, string> properties);

		int GetRuntimeId(IDictionary<string, string> properties);
	}
}

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

		TagCompound BuildTileEntity();

        string GetName(int metadata);

        IDictionary<string, string> GetProperties(int metadata);

		int GetMetadata(IDictionary<string, string> properties);

	}
}

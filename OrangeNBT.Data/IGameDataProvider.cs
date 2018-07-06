using System;
using System.Collections.Generic;
using System.Text;

namespace OrangeNBT.Data
{
    public interface IGameDataProvider
    {
		string Version { get; }

		IBlock GetBlock(int id);

		IBlock GetBlock(string id);

		IBlock GetBlockFromRuntimeID(int id);

		int GetMetadataFromRuntimeID(int id);
	}
}

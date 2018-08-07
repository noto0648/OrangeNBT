using OrangeNBT.NBT;
using OrangeNBT.NBT.IO;
using System.Collections.Generic;

namespace OrangeNBT.Data.Anvil.Helper
{
	public static class AnvilBlockAccessHelper
    {
		public static string GetBlockName(this IBlockAccess world, int x, int y, int z)
		{
			return world.GetBlock(x, y, z).Name;
		}

		public static Dictionary<string,string> GetBlockProperties(this IBlockAccess world, int x, int y, int z)
		{
			return (Dictionary<string, string>)world.GetBlock(x, y, z).Properties;
		}

		public static bool SetBlock(this IBlockAccess world, int x, int y, int z, string blockName, params string[] args)
		{
			IBlock block = GameData.JavaEdition.GetBlock(blockName);
			if (args == null || args.Length == 0)
				return world.SetBlock(x, y, z, block.DefaultBlockSet);
			if(args.Length == 1)
			{
				TagCompound tag = NBTFile.FromJson(args[0]);
				return world.SetBlock(x, y, z, new BlockSet(block, tag));
			}
			else if((args.Length & 1) == 0)
			{
				Dictionary<string, string> ps1 = PropertyConverter.From(args);
				foreach(string key in block.DefaultBlockSet.Properties.Keys)
				{
					if (!ps1.ContainsKey(key))
						ps1.Add(key, block.DefaultBlockSet.Properties[key]);
				}
				return world.SetBlock(x, y, z, new BlockSet(block, ps1));
			}
			return world.SetBlock(x, y, z, block.DefaultBlockSet);
		}

		//public static bool SetBlock(this IBlockAccess world, int x, int y, int z, string blockName, dynamic propertis)
		//{
		//	IBlock block = GameData.JavaEdition.GetBlock(blockName);

		//	return world.SetBlock(x, y, z, new BlockSet(block, PropertyConverter.From(propertis)));
		//}
	}
}

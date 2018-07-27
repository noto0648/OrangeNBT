using OrangeNBT.NBT;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OrangeNBT.Data.Anvil.Helper
{
	public static class PropertyConverter
    {
		public static Dictionary<string, string> From(params string[] args)
		{
			Dictionary<string, string> ps = new Dictionary<string, string>();
			for (int i = 0; i < args.Length / 2; i++)
			{
				ps.Add(args[i * 2], args[i * 2 + 1]);
			}
			return ps;
		}

		public static Dictionary<string, string> From(TagCompound tag)
		{
			Dictionary<string, string> ps = new Dictionary<string, string>();
			foreach (TagString t in tag)
			{
				ps.Add(t.Name, t.Value);
			}
			return ps;
		}

		public static Dictionary<string, string> From(dynamic val)
		{
			Dictionary<string, string> ps = new Dictionary<string, string>();
			Type t = val.GetType();
			PropertyInfo[] pos = t.GetProperties();
			for(int i = 0; i < pos.Length; i++)
			{
				ps.Add(pos[i].Name, pos[i].GetValue(val).ToString());
			}
			return ps;
		}
	}
}

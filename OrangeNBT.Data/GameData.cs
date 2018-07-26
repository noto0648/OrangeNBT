using OrangeNBT.Data.AnvilImproved;
using System;

namespace OrangeNBT.Data
{
	public static class GameData
    {
		public static IGameDataProvider JavaEdition { get; set; } = AnvilImprovedDataProvider.Instance;

		private static IGameDataProvider _pocketEdition;
		public static IGameDataProvider PocketEdition
		{
			get
			{
				if (_pocketEdition != null) return _pocketEdition;
				try
				{
					Type type = Type.GetType("OrangeNBT.Data.Bedrock.BedrockDataProvider");
					_pocketEdition = (IGameDataProvider)type.GetProperty("Instance").GetValue(null);
					return _pocketEdition;
				}
				catch(Exception)
				{
					throw new NotSupportedException();
				}
			}
			set
			{
				_pocketEdition = value;
			}
		}
	}
}

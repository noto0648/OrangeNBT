using OrangeNBT.Data.Anvil;

namespace OrangeNBT.Data
{
	public static class GameData
    {
		public static IGameDataProvider Data { get; set; } = AnvilDataProvider.Instance;
	}
}

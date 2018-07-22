using System.Collections.Generic;

namespace OrangeNBT.World.Anvil
{
	public enum ChunkStatus
    {
		Empty,
		Base,
		Carved,
		LiquidCarved,
		Decorated,
		Lighted,
		MobsSpawned,
		Finalized,
		Fullchunk,
		Postprocessed
    }

	public static class ChunkStatusHelper
	{
		private static readonly Dictionary<ChunkStatus, string> StatusText = new Dictionary<ChunkStatus, string>()
		{
			{ ChunkStatus.Base, "base" },
			{ ChunkStatus.Carved, "carved" },
			{ ChunkStatus.Decorated, "decorated" },
			{ ChunkStatus.Empty, "empty" },
			{ ChunkStatus.Finalized, "finalized" },
			{ ChunkStatus.Fullchunk, "fullchunk" },
			{ ChunkStatus.Lighted, "lighted" },
			{ ChunkStatus.LiquidCarved, "liquid_carved" },
			{ ChunkStatus.MobsSpawned, "mobs_spawned" },
			{ ChunkStatus.Postprocessed, "postprocessed" }

		};

		public static string ToKeyString(this ChunkStatus status)
		{
			return StatusText[status];
		}

		public static ChunkStatus Parse(string text)
		{
			foreach (ChunkStatus key in StatusText.Keys)
			{
				if (StatusText[key] == text)
					return key;
			}
			return ChunkStatus.Empty;
		}

	}

}

using OrangeNBT.Data;
using OrangeNBT.Helper;
using OrangeNBT.NBT;
using OrangeNBT.World.Anvil;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrangeNBT.World.AnvilImproved
{
	public class AnvilChunkImproved : AnvilChunk
	{
		private const int BorderVersion = 1500;
		private static readonly string[] ListOfList = new string[] { "LiquidTicks", "Lights", "LiquidsToBeTicked", "ToBeTicked", "PostProcessing" };
		private ChunkStatus _status = ChunkStatus.Empty;

		private Dictionary<string, HeightMap> _heightMaps;
		public Dictionary<string, HeightMap> HeightMaps => _heightMaps;

		private byte[] _carvingMaskAir;
		private byte[] _carvingMaskLiquid;

		private Dictionary<string, TagList> _lists = new Dictionary<string, TagList>();
		private TagCompound _structures = new TagCompound("Structures") { new TagCompound("Starts"), new TagCompound("References") };

		public AnvilChunkImproved(AnvilChunkManager manager, ChunkCoord coord) : base(manager, coord)
		{
			_heightMaps = new Dictionary<string, HeightMap>();
		}

		protected override AnvilSection CreateNewSection(int y, bool skylight)
		{
			return new AnvilSectionImproved(y, skylight);
		}

		public new static AnvilChunk Load(AnvilChunkManager manager, TagCompound compound)
		{
			if (compound == null || !compound.ContainsKey("Level"))
				return null;
			if(compound.ContainsKey("DataVersion") && compound.GetInt("DataVersion") < BorderVersion)
			{
				return AnvilChunk.Load(manager, compound);
			}

			TagCompound level = compound["Level"] as TagCompound;
			int cx = level.GetInt("xPos");
			int cy = level.GetInt("zPos");
			AnvilChunkImproved c = new AnvilChunkImproved(manager, new ChunkCoord(cx, cy));

			c._status = ChunkStatusHelper.Parse(level.GetString("Status"));
			//c.InhabitedTime = tag.GetLong("InhabitedTime");

			TagList sections = (TagList)level["Sections"];
			c._sections = new AnvilSection[SectionsPerChunk];
			for (int i = 0; i < sections.Count; i++)
			{
				TagCompound sec = sections[i] as TagCompound;
				if (sec == null)
					continue;

				c._sections[i] = new AnvilSectionImproved(sec.GetByte("Y"), true);
				c._sections[i].Load(sec);
			}

			if (level.ContainsKey("Biomes", TagType.IntArray))
			{
				c._biomes = level.GetIntArray("Biomes");
			}

			if(level.ContainsKey("Heightmaps"))
			{
				foreach(TagLongArray t in (TagCompound)level["Heightmaps"])
				{
					c._heightMaps.Add(t.Name, new HeightMap(t.Name, t.Value));
				}
			}

			if (level.ContainsKey("CarvingMasks"))
			{
				TagCompound tag = (TagCompound)level["CarvingMasks"];
				if (tag.ContainsKey("AIR"))
					c._carvingMaskAir = tag.GetByteArray("AIR");

				if (tag.ContainsKey("LIQUID"))
					c._carvingMaskLiquid = tag.GetByteArray("LIQUID");
			}

			if(level.ContainsKey("Structures"))
			{
				c._structures = (TagCompound)level["Structures"];
			}
			for(int i = 0; i < ListOfList.Length; i++)
			{
				if (level.ContainsKey(ListOfList[i]))
				{
					c._lists.Add(ListOfList[i], (TagList)level[ListOfList[i]]);
				}
			}

			TagList entities = (TagList)level["Entities"];
			foreach (TagCompound t in entities)
			{
				c.Entities.Add(t);
			}

			TagList tiles = (TagList)level["TileEntities"];
			foreach (TagCompound t in tiles)
			{
				int x = t.GetInt("x");
				int y = t.GetInt("y");
				int z = t.GetInt("z");
				c._tileEntities.Add(new BlockPos(x, y, z), t);
			}

			return c;
		}

		public override TagCompound BuildTag(int version = 0)
		{
			if (version < BorderVersion)
			{
				return base.BuildTag(version);
			}
			TagList tagSections = GenSectionsTag();
			TagList tiles = GenTileEntitiesTag();
			TagCompound level = new TagCompound("Level")
				{
					tagSections,
					new TagInt("xPos", _coord.X),
					new TagInt("zPos", _coord.Z),
					new TagLong("LastUpdate", DateTime.Now.Ticks),
					new TagString("Status", _status.ToKeyString()),
					new TagIntArray("Biomes", _biomes),
					_entities.BuildTag(),
					tiles
				};

			TagCompound heightMaps = new TagCompound("Heightmaps");
			foreach (string key in _heightMaps.Keys)
			{
				heightMaps.Add(_heightMaps[key].BuildTag());
			}
			level.Add(heightMaps);

			if (_carvingMaskAir != null && _carvingMaskAir.Length > 0)
			{
				if (level.ContainsKey("CarvingMasks"))
					level.Add(new TagCompound("CarvingMasks"));
				((TagCompound)level["CarvingMasks"]).Add("AIR", _carvingMaskAir);
			}
			if (_carvingMaskLiquid != null && _carvingMaskLiquid.Length > 0)
			{
				if (level.ContainsKey("CarvingMasks"))
					level.Add(new TagCompound("CarvingMasks"));
				((TagCompound)level["CarvingMasks"]).Add("LIQUID", _carvingMaskLiquid);
			}

			foreach (string key in _lists.Keys)
			{
				level.Add(_lists[key]);
			}

			level.Add(_structures);

			return new TagCompound() {

				new TagInt("DataVersion", version),
				level
			};
		}

	}
}

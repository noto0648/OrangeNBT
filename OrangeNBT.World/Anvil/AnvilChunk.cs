using OrangeNBT.Data;
using OrangeNBT.Helper;
using OrangeNBT.NBT;
using OrangeNBT.World.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OrangeNBT.World.Anvil
{
    public class AnvilChunk : IChunk, ITagProvider<TagCompound>
    {
        public const int SectionsPerChunk = 16;
        public const int Width = AnvilSection.Width;
        public const int Height = SectionsPerChunk * AnvilSection.Height;
        public const int Length = AnvilSection.Length;

        protected AnvilSection[] _sections = new AnvilSection[SectionsPerChunk];
		protected readonly ChunkCoord _coord;
		protected bool _isModified;
		protected int[] _biomes = new int[Width * Length];
        private int[] _heights = new int[Width * Length];
		protected ChunkEntityCollection _entities;
		protected Dictionary<BlockPos, TagCompound> _tileEntities;
		private bool _isTerrainPopulated;
		private bool _isLightPopulated;

        public ChunkEntityCollection Entities { get { return _entities; } }
        public bool IsModified { get { return _isModified; } set { _isModified = value; } }
        public ChunkCoord Coord { get { return _coord; } }

		public virtual int DataVersion { get; set; } = 0;

		protected readonly AnvilChunkManager _manager;

        public AnvilChunk(AnvilChunkManager manager, ChunkCoord coord)
        {
            _coord = coord;
            _manager = manager;
            _entities = new ChunkEntityCollection(new TagList());
            _tileEntities = new Dictionary<BlockPos, TagCompound>();
        }

        #region Processing

        public void GenerateHeightMap()
        {
            //UnTested code
            for (int z = 0; z < Length; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = Height - 1; y >= 0; y--)
                    {
                        if (!GetBlock(x, y, z).IsAir)	
                        {
                            SetHeight(x, z, y);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region ValueAccess

        private AnvilSection GetSection(int y, bool create)
        {
            int sectionY = y / AnvilSection.Height;
            AnvilSection section = _sections[sectionY];

            if (section == null && create)
            {
                section = CreateNewSection(sectionY, true);
                _sections[sectionY] = section;
            }
            return section;
        }
		
        public bool SetBlock(int x, int y, int z, BlockSet block)
        {
            AnvilSection section = GetSection(y, true);
            int blockY = y % AnvilSection.Height;
            _isModified = true;
            return section.SetBlock(x, blockY, z, block);
        }
		
        public bool SetBlockLight(int x, int y, int z, int light)
        {
            AnvilSection section = GetSection(y, true);
            int blockY = y % AnvilSection.Height;
            _isModified = true;
            return section.SetBlockLight(x, blockY, z, light);
        }

        public bool SetSkyLight(int x, int y, int z, int light)
        {
            AnvilSection section = GetSection(y, true);
            int blockY = y % AnvilSection.Height;
            _isModified = true;
            return section.SetSkyLight(x, blockY, z, light);
        }

        public bool SetHeight(int x, int z, int height)
        {
            _heights[z * Length + x] = height;
            _isModified = true;
            return true;
        }

        public bool SetBiome(int x, int z, int biome)
        {
            _biomes[z * Length + x] = (byte)biome;
            _isModified = true;
            return true;
        }

        public bool SetTileEntity(int x, int y, int z, TagCompound tag)
        {
            BlockPos pos = new BlockPos(x, y, z);
            if (_tileEntities.ContainsKey(pos))
            {
                _tileEntities[pos] = tag;
                if (tag == null)
                {
                    _tileEntities.Remove(pos);
                }
                _isModified = true;
                return true;
            }
            _tileEntities.Add(pos, tag);
            _isModified = true;
            return true;
        }

        public BlockSet GetBlock(int x, int y, int z)
        {
            AnvilSection section = GetSection(y, true);
            int blockY = y % AnvilSection.Height;
            return section.GetBlock(x, blockY, z);
        }

        public int GetSkyLight(int x, int y, int z)
        {
            AnvilSection section = GetSection(y, true);
            int blockY = y % AnvilSection.Height;
            return section.GetSkyLight(x, blockY, z);
        }

        public int GetBlockLight(int x, int y, int z)
        {
            AnvilSection section = GetSection(y, true);
            int blockY = y % AnvilSection.Height;
            return section.GetBlockLight(x, blockY, z);
        }

        public int GetHeight(int x, int z)
        {
            return _heights[z * Length + x];
        }

        public int GetBiome(int x, int z)
        {
            return _biomes[z * Length + x];
        }

        public TagCompound GetTileEntity(int x, int y, int z)
        {
            BlockPos pos = new BlockPos(x, y, z);
            if (_tileEntities.ContainsKey(pos))
            {
                return _tileEntities[pos];
            }
            return null;
        }

        #endregion

        #region IO

		protected virtual AnvilSection CreateNewSection(int y, bool skylight)
		{
			return new AnvilSectionClassic(y, skylight);
		}

        public static AnvilChunk Load(AnvilChunkManager manager, TagCompound compound)
        {
            /**
             * 
             * Untested Code
             * 
             * 
             */
            if (compound == null || !compound.ContainsKey("Level"))
                return null;

            TagCompound level = compound["Level"] as TagCompound;
            int cx = level.GetInt("xPos");
            int cy = level.GetInt("zPos");
            AnvilChunk c = new AnvilChunk(manager, new ChunkCoord(cx, cy));

			if (compound.ContainsKey("DataVersion"))
			{
				c.DataVersion = compound.GetInt("DataVersion");
			}
			c._heights = level.GetIntArray("HeightMap");
			bool isTerrainPopulated= level.GetBool("TerrainPopulated");
			bool isLightPopulated = level.GetBool("LightPopulated");

			//if(isTerrainPopulated && isLightPopulated)
			//{
			//	c._status = ChunkStatus.Lighted;
			//}
			//else if(isTerrainPopulated)
			//{
			//	c._status = ChunkStatus.Carved;
			//}
			c._isTerrainPopulated = level.GetBool("TerrainPopulated");
			c._isLightPopulated = level.GetBool("LightPopulated");

			//c.InhabitedTime = tag.GetLong("InhabitedTime");

			TagList sections = (TagList)level["Sections"];
            c._sections = new AnvilSection[SectionsPerChunk];
            for (int i = 0; i < sections.Count; i++)
            {
                TagCompound sec = sections[i] as TagCompound;
                if (sec == null)
                    continue;

                c._sections[i] = new AnvilSectionClassic(sec.GetByte("Y"), true);
                c._sections[i].Load(sec);
            }

            if (level.ContainsKey("Biomes", TagType.ByteArray))
            {
                byte[] oldBiome = level.GetByteArray("Biomes");
				for (int i = 0; i < oldBiome.Length; i++)
					c._biomes[i] = oldBiome[i];

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

        public void Save()
        {
            if (!_isModified) return;

            _manager.SaveChunk(this);
            _isModified = false;
        }

		protected TagList GenSectionsTag()
		{
			TagList tagSections = new TagList("Sections", TagType.Compound);
			for (int i = 0; i < _sections.Length; i++)
			{
				if (_sections[i] != null)
				{
					tagSections.Add(_sections[i].BuildTag());
				}
			}
			return tagSections;
		}

		protected TagList GenTileEntitiesTag()
		{
			TagList tiles = new TagList("TileEntities", TagType.Compound);
			foreach (TagCompound c in _tileEntities.Values)
			{
				tiles.Add(c);
			}
			return tiles;
		}

		public virtual TagCompound BuildTag()
		{
			return BuildTag(0);
		}

		public virtual TagCompound BuildTag(int version)
		{
			TagList tagSections = GenSectionsTag();
			TagList tiles = GenTileEntitiesTag();

			byte[] biomeArray = new byte[Width * Length];
			for (int i = 0; i < biomeArray.Length; i++)
				biomeArray[i] = (byte)_biomes[i];

			return new TagCompound()
			{
				new TagInt("DataVersion", version),
				new TagCompound("Level")
				{
					tagSections,
					new TagInt("xPos", _coord.X),
					new TagInt("zPos", _coord.Z),
					new TagLong("LastUpdate", DateTime.Now.Ticks),
					new TagByte("V", 1),
					new TagByte("TerrainPopulated", (byte)(_isTerrainPopulated? 1 : 0)),
					new TagByte("LightPopulated", (byte)(_isLightPopulated? 1 : 0)),
					new TagIntArray("HeightMap", _heights),
					new TagByteArray("Biomes", biomeArray),
					_entities.BuildTag(),
					tiles
				}
			};
		}


        #endregion

        #region Entities

        public class ChunkEntityCollection : ITagProvider<TagList>, IEnumerable<TagCompound>
        {
            private List<TagCompound> _entities;

            internal ChunkEntityCollection(TagList tagList)
            {
                _entities = new List<TagCompound>(tagList.Count + 10);
                foreach (TagCompound c in tagList)
                {
                    _entities.Add(c);
                }
            }

            public void Add(TagCompound tag)
            {
                _entities.Add(tag);
            }

            public void Add(TagCompound tag, bool safe)
            {
                if (!safe)
                {
                    Add(tag);
                    return;
                }
                if (!Entity.IsEntityTag(tag))
                    return;
                Add(tag);
            }

            public void Remove(TagCompound tag)
            {
                _entities.Remove(tag);
            }

            public IEnumerator<TagCompound> GetEnumerator()
            {
                return _entities.GetEnumerator();
            }

            public TagList BuildTag()
            {
                TagList r = new TagList("Entities");
                foreach (TagCompound c in _entities)
                {
                    r.Add(c);
                }
                return r;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _entities.GetEnumerator();
            }
        }

        #endregion

    }
}

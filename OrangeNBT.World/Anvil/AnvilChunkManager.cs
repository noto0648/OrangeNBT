using OrangeNBT.NBT;
using OrangeNBT.NBT.IO;
using OrangeNBT.World.AnvilImproved;
using OrangeNBT.World.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace OrangeNBT.World.Anvil
{
    public class AnvilChunkManager : IChunkManager, IDisposable
    {
        private Dictionary<RegionCoord, RegionFile> _regionCache;
        private bool _canCreate = true;
        private string _regionDirectory;

        private readonly AnvilCache _cache;

        internal AnvilChunkManager(string regionDir, AnvilCache cache)
        {
            _regionDirectory = regionDir;
            _cache = cache;
            _regionCache = new Dictionary<RegionCoord, RegionFile>();
        }

        public IChunk GetChunk(ChunkCoord coord)
        {
            if(!_cache.Contains(coord))
            {
                IChunk ck = LoadChunk(coord);
                if (ck == null)
                    return null;
                _cache.Add(ck);
            }
            return _cache.Fetch(coord);
        }

        public IChunk CreateChunk(ChunkCoord coord)
        {
            if (_cache.Contains(coord))
            {
                return _cache.Fetch(coord);
            }
            AnvilChunk c = new AnvilChunk(this, coord);
            _cache.Add(c);
            FetchRegion(coord.RegionCoord, true);
            return c;
        }

        private string GetRegionFile(RegionCoord r)
        {
            return _regionDirectory + Path.DirectorySeparatorChar + string.Format("r.{0}.{1}.mca", r.X, r.Z);
        }

        private RegionFile FetchRegion(RegionCoord r, bool create = false)
        {
            if(!_regionCache.ContainsKey(r))
            {
                string filePath = GetRegionFile(r);
                if ((File.Exists(filePath) && !_canCreate) || _canCreate)
                    _regionCache.Add(r, new RegionFile(filePath, r));
            }
            return _regionCache[r];
        }

        private IEnumerable<RegionFile> ListRegions()
        {
            if (!Directory.Exists(_regionDirectory))
            {
                Directory.CreateDirectory(_regionDirectory);
            }

            string[] regions = Directory.GetFiles(_regionDirectory, "*.mca");
            string parentDirectory = _regionDirectory + Path.DirectorySeparatorChar;
            for (int i = 0; i < regions.Length; i++)
            {
                string[] codes = regions[i].Replace(parentDirectory, string.Empty).Split('.');
                if (codes.Length == 4 && codes[0] == "r")
                {
                    int cx, cz;
                    if (int.TryParse(codes[1], out cx) && int.TryParse(codes[2], out cz))
                    {
                        RegionCoord coord = new RegionCoord(cx, cz);
                        if (!_regionCache.ContainsKey(coord))
                        {
                            RegionFile rf = new RegionFile(regions[i], coord);
                            yield return rf;
                        }
                        else
                        {
                            yield return _regionCache[coord];
                        }
                    }

                }
            }
        }

        private AnvilChunk LoadChunk(ChunkCoord coord)
        {
            RegionFile f = FetchRegion(coord.RegionCoord);
            TagCompound chunkTag = null;
            using (Stream stream = f.ReadChunk(new ChunkCoord(coord.X & 31, coord.Z & 31)))
            {
                if (stream != null)
                    chunkTag = NBTFile.FromStream(stream, false);
            }
            if (chunkTag == null)
                return null;

            return AnvilChunkImproved.Load(this, chunkTag);
        }

        internal void Save(int version)
        {
            foreach(IChunk chunk in _cache)
            {
                if (!chunk.IsModified) continue;
                SaveChunk((AnvilChunk)chunk, version);
                chunk.IsModified = false;
            }
            
        }

        internal void SaveChunk(AnvilChunk chunk, int version)
        {
            RegionFile f = FetchRegion(chunk.Coord.RegionCoord);
            TagCompound chunkTag = chunk.BuildTag(version);
            using (Stream stream = f.WriteChunk(new ChunkCoord(chunk.Coord.X & 31, chunk.Coord.Z & 31)))
            {
                NBTFile.ToStream(stream, chunkTag);
            }
            
        }

		public bool Contains(ChunkCoord coord)
		{
			RegionFile rf = FetchRegion(coord.RegionCoord);
			if (rf == null) return false;
			return rf.ContainsChunk(new ChunkCoord(coord.X & 31, coord.Z & 31));
		}

        public void Dispose()
        {
            if (_regionCache == null)
                return;

            _cache.Clear();
            _regionCache.Clear();
            foreach(RegionFile r in _regionCache.Values)
            {
                r.Dispose();
            }
            _regionCache = null;
        }

        public IEnumerable<IChunk> ListAllChunks()
        {
			foreach(ChunkCoord ck in ListAllCoords())
			{
				IChunk chunk = GetChunk(ck);
				if (chunk == null)
					continue;
				yield return chunk;
			}
        }

        public IEnumerable<ChunkCoord> ListAllCoords()
        {
            foreach (RegionFile r in ListRegions())
            {
                for (int z = 0; z < 32; z++)
                {
                    for (int x = 0; x < 32; x++)
                    {
						ChunkCoord ck = new ChunkCoord(r.Coord.X * 32 + x, r.Coord.Z * 32 + z);
						if(Contains(ck))
							yield return ck;
                    }
                }
            }
        }
    }
}

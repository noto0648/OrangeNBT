using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OrangeNBT.World.Anvil
{
    public class AnvilWorldFolder : IDisposable
    {
        private string _filePath;
        private Dictionary<RegionCoord, RegionFile> _regionFiles;

        public AnvilWorldFolder(string directoryPath)
        {
            _filePath = directoryPath;
            if(!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }

            _regionFiles = new Dictionary<RegionCoord,RegionFile>();
        }

        private string GetRegionFileName(RegionCoord coord)
        {
            return _filePath + Path.DirectorySeparatorChar + string.Format("r.{0}.{1}.mca", coord.X, coord.Z);
        }

        public RegionFile GetRegionFile(RegionCoord coord)
        {
            if (_regionFiles.ContainsKey(coord)) return _regionFiles[coord];
            RegionFile region = new RegionFile(GetRegionFileName(coord), coord);
            _regionFiles.Add(coord, region);
            return region;
        }
        
        public RegionFile TryLoadRegionFile(string fileName)
        {
            string[] names = fileName.Replace(_filePath + Path.DirectorySeparatorChar, "").Split('.');
            if (names.Length == 4 && names[0] == "r")
            {
                RegionCoord rc = new RegionCoord(int.Parse(names[1]), int.Parse(names[2]));
                return new RegionFile(fileName, rc);
            }
            return null;
        }

        public IEnumerable<ChunkCoord> ListChunks()
        {
            string[] regionFiles = Directory.GetFiles(_filePath, "*.mca");
            for (int i = 0; i < regionFiles.Length; i++)
            {
                RegionFile rf = TryLoadRegionFile(regionFiles[i]);
                if (rf == null)
                    continue;
                _regionFiles.Add(rf.Coord, rf);

                for (int j = 0; j < rf.Count; j++)
                {
                    int cx = j & 0x1F;
                    int cy = j >> 5;
                    cx += rf.Coord.X << 5;
                    cy += rf.Coord.Z << 5;
                    yield return new ChunkCoord(cx, cy);
                }
            }
        }

        public bool ContainsChunk(ChunkCoord coord)
        {
            if (File.Exists(GetRegionFileName(coord.RegionCoord)))
                return false;
            RegionFile rf = GetRegionFile(coord.RegionCoord);
            return rf.ContainsChunk(coord);
        }

        public Stream ReadChunk(ChunkCoord coord)
        {
            RegionFile rf = GetRegionFile(coord.RegionCoord);
            return rf.ReadChunk(coord);
        }

        public void Dispose()
        {
            if (_regionFiles != null)
            {
                foreach (RegionFile r in _regionFiles.Values)
                {
                    //r.Dispose();
                }
                _regionFiles = null;
            }
        }
    }
}

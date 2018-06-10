using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using OrangeNBT.Helper;

namespace OrangeNBT.World
{
    public class RegionFile : IDisposable
    {
        private const int SectorBytes = 4096;
        private const int SectorInts = SectorBytes / 4;
        private const int ChunkHeaderSize = 5;

        private const int VerGZip = 1;
        private const int VerDeflate = 2;

        private string _filePath;
        private object _file;
        private FileStream _stream;
        private RegionCoord _coord;
        private List<bool> _freeSectors;
        private int _sizeDelta;

        private int[] _offsets = new int[SectorInts];
        private int[] _modTime = new int[SectorInts];

        public RegionCoord Coord
        {
            get { return _coord; }
        }

        public int Count
        {
            get { return _offsets.Length; }
        }

        public RegionFile(string path, RegionCoord coord)
        {
            _filePath = path;
            _coord = coord;
            _file = new object();
            InitializeImpl();
        }

        private void InitializeImpl()
        {
            lock(_file)
            {
                _stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                if (_stream.Length < SectorBytes)
                {
                    WriteEmpty();
                    WriteEmpty();
                    _stream.Flush();
                    _sizeDelta += SectorBytes * 2;
                }

                if ((_stream.Length & 4095L) != 0L)
                {
                    _stream.Seek(0, SeekOrigin.End);
                    for (int i = 0; i < (_stream.Length & 4095L); ++i)
                    {
                        _stream.WriteByte(0);
                    }
                    _stream.Flush();
                }

                int length = (int)_stream.Length / SectorBytes;
                _freeSectors = new List<bool>(length);
                for (int i = 0; i < length; i++)
                {
                    _freeSectors.Add(true);
                }

                _freeSectors[0] = false;
                _freeSectors[1] = false;

                _stream.Seek(0, SeekOrigin.Begin);

                for (int i = 0; i < SectorInts; i++)
                {
                    int k = _stream.ReadInt32Helper();
                    _offsets[i] = k;
                    if (k != 0 && (k >> 8) + (k & 255) <= _freeSectors.Count)
                    {
                        for (int l = 0; l < (k & 255); ++l)
                        {
                            _freeSectors[(k >> 8) + l] = false;
                        }
                    }
                }

                for (int i = 0; i < SectorInts; ++i)
                {
                    int lastModValue = _stream.ReadInt32Helper();
                    _modTime[i] = lastModValue;
                }
            }
        }

        public Stream ReadChunk(ChunkCoord coord)
        {

            lock (_file)
            {
                int offset = GetOffset(coord.X, coord.Z);

                if (offset == 0)
                {
                    return null;
                }
                int sectorNum = offset >> 8;
                int numSectors = offset & 255;

                if (sectorNum + numSectors > _freeSectors.Count)
                {
                    return null;
                }
                _stream.Seek((sectorNum * SectorBytes), SeekOrigin.Begin);
                int len = _stream.ReadInt32Helper();

                if (len > SectorBytes * numSectors || len <= 0)
                {
                    return null;
                }

                sbyte ver = _stream.ReadByteHelper().ConvertToSbyte();

                byte[] dat = new byte[len - 1];
                _stream.Read(dat, 0, dat.Length);

                if (ver == VerGZip)
                {
                    Stream ret = new GZipStream(new MemoryStream(dat), CompressionMode.Decompress);
                    return ret;
                }
                else if (ver == VerDeflate)
                {
                    Stream ret = new Ionic.Zlib.ZlibStream(new MemoryStream(dat), Ionic.Zlib.CompressionMode.Decompress);
                    return ret;
                }
            }
            return null;
        }

        protected void WriteImpl(int chunkX, int chunkZ, byte[] rawData, int len, DateTime timestamp)
        {
            lock (_file)
            {
                try
                {
                    int offset = GetOffset(chunkX, chunkZ);
                    int sectorNum = offset >> 8;
                    int sectorAllocated = offset & 255;
                    int sectorNeeded = (len + ChunkHeaderSize) / SectorBytes + 1;

                    if (sectorNeeded >= 256)
                    {
                        return;
                    }

                    if (sectorNum != 0 && sectorAllocated == sectorNeeded)
                    {
                        WriteImpl(sectorNum, rawData, len);
                    }
                    else
                    {

                        for (int i = 0; i < sectorAllocated; ++i)
                        {
                            _freeSectors[sectorNum + i] = true;
                        }

                        //int runStart = sectorFree.FindIndex(b => b == true);
                        int runStart = _freeSectors.IndexOf(true);
                        int runLength = 0;

                        if (runStart != -1)
                        {
                            for (int i = runStart; i < _freeSectors.Count; ++i)
                            {
                                if (runLength != 0)
                                {
                                    if (_freeSectors[i])
                                        runLength++;
                                    else
                                        runLength = 0;
                                }
                                else if (_freeSectors[i])
                                {
                                    runStart = i;
                                    runLength = 1;
                                }

                                if (runLength >= sectorNeeded)
                                {
                                    break;
                                }
                            }
                        }

                        if (runLength >= sectorNeeded)
                        {
                            sectorNum = runStart;
                            SetOffset(chunkX, chunkZ, (runStart << 8) | sectorNeeded);

                            for (int i = 0; i < sectorNeeded; ++i)
                            {
                                _freeSectors[sectorNum + i] = false;
                            }

                            WriteImpl(sectorNum, rawData, len);
                        }
                        else
                        {
                            _stream.Seek(0, SeekOrigin.End);
                            sectorNum = _freeSectors.Count;

                            for (int i = 0; i < sectorNeeded; ++i)
                            {
                                //_stream.Write(_emptySector, 0, _emptySector.Length);
                                WriteEmpty();
                                _freeSectors.Add(false);
                            }

                            _sizeDelta += SectorBytes * sectorNeeded;
                            WriteImpl(sectorNum, rawData, len);
                            SetOffset(chunkX, chunkZ, (sectorNum << 8) | sectorNeeded);
                        }
                    }

                    SetTimestamp(chunkX, chunkZ, timestamp);
                }
                catch (IOException)
                {
                    //Console.WriteLine(err.StackTrace);
                }
            }
        }

        private void WriteImpl(int seekLevel, byte[] data, int len)
        {
            lock (_file)
            {
                _stream.Seek((seekLevel * SectorBytes), SeekOrigin.Begin);
                _stream.WriteHelper(len + 1);
                _stream.WriteHelper((byte)VerDeflate);
                _stream.Write(data, 0, len);
            }
        }

        public Stream WriteChunk(ChunkCoord coord)
        {
            return new Ionic.Zlib.ZlibStream(new ChunkBuffer(this, coord.X, coord.Z), Ionic.Zlib.CompressionMode.Compress);
        }

        private void WriteEmpty()
        {
            byte[] bit = BitConverter.GetBytes(0);
            for (int i = 0; i < SectorInts; i++)
            {
                _stream.Write(bit, 0, 4);
            }
        }

        private int GetOffset(int x, int z)
        {
            return _offsets[x + z * 32];
        }

        private void SetOffset(int x, int z, int offset)
        {
            _offsets[x + z * 32] = offset;

            lock (_file)
            {
                _stream.Seek(((x + z * 32) * 4), SeekOrigin.Begin);
                _stream.WriteHelper(offset);
            }
        }

        public bool ContainsChunk(ChunkCoord coord)
        {
            return GetOffset(coord.X, coord.Z) != 0;
        }

        private void SetTimestamp(int x, int z)
        {
            SetTimestamp(x, z, DateTime.UtcNow);
        }

        private void SetTimestamp(int x, int z, int timestamp)
        {
            _modTime[x + z * 32] = timestamp;

            lock (_file)
            {
                _stream.Seek(4096 + ((x + z * 32) * 4), SeekOrigin.Begin);
                _stream.WriteHelper(timestamp);
            }
        }

        private void SetTimestamp(int x, int z, DateTime time)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            int timestamp = (int)((time - epoch).Ticks / (10000L * 1000L));
            SetTimestamp(x, z, timestamp);
        }

        private int GetTimestamp(int x, int z)
        {
            return _modTime[x + z * 32];
        }

        public void DeleteChunk(int x, int z)
        {
            lock (_file)
            {
                int offset = GetOffset(x, z);
                int sectorNumber = offset >> 8;
                int sectorsAllocated = offset & 0xFF;
                _stream.Seek(sectorNumber * SectorInts, SeekOrigin.Begin);
                for (int i = 0; i < sectorsAllocated; i++)
                {
                    WriteEmpty();
                }
                SetOffset(x, z, 0);
                SetTimestamp(x, z, 0);
            }

        }

        public void Dispose()
        {
            if (_stream == null)
                return;
            _stream.Dispose();
            _stream = null;
            _file = null;
        }


        class ChunkBuffer : MemoryStream, IDisposable
        {
            private int _x, _z;
            private RegionFile _region;
            private DateTime _timestamp;

            public ChunkBuffer(RegionFile r, int x, int z)
                : base(8096)
            {
                _region = r;
                _x = x;
                _z = z;
            }
            public ChunkBuffer(RegionFile r, int x, int z, DateTime timestamp)
                : this(r, x, z)
            {
                _timestamp = timestamp;
            }

            protected override void Dispose(bool disposing)
            {
                Dispose();
                base.Dispose(disposing);
            }

            public new void Dispose()
            {
                ArraySegment<byte> segment;
                if (!TryGetBuffer(out segment))
                {
                    //base.Dispose();
                    return;
                }
                _region.WriteImpl(_x, _z, segment.Array, (int)this.Length, _timestamp);
            }

        }

    }
}

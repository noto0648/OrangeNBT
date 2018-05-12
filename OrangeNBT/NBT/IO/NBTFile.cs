using System.IO;
using System.IO.Compression;

namespace OrangeNBT.NBT.IO
{
    public static class NBTFile
    {
        public static string ToJson(TagCompound tag)
        {
            return ToJson(tag, JsonOptions.Default);
        }

        public static string ToJson(TagCompound tag, JsonOptions ops)
        {
            return NBTJsonConverter.ToJson(tag, ops);
        }

        public static TagCompound FromJson(string json)
        {
            return NBTJsonConverter.FromJson(json) as TagCompound;
        }

        public static void ToStream(Stream stream, TagCompound tag)
        {
            ToStream(stream, tag, false);
        }

        public static void ToStream(Stream stream, TagCompound tag, bool compressing)
        {
            using (NBTBinaryWriter bw = new NBTBinaryWriter(compressing ? new GZipStream(stream, CompressionMode.Compress) : stream))
            {
                TagBase.WriteNamedTag(tag, bw);
                bw.Dispose();
            }
        }

        public static TagCompound FromStream(Stream stream)
        {
            bool compress = false;
            using (NBTBinaryReader br = new NBTBinaryReader(stream))
            {
                br.BaseStream.Position = 0;
                byte firstByte = br.ReadByte();
                byte secondByte = br.ReadByte();
                compress = (firstByte == 0x1F) && (secondByte == 0x8B);
                br.BaseStream.Position = 0;
                return FromStream(stream, compress);
            }
        }

        public static TagCompound FromStream(Stream stream, bool compressing)
        {
            Stream newStream = stream;
            if (compressing)
            {
                newStream = new GZipStream(stream, CompressionMode.Decompress);
            }

            TagBase baseTag;
            using (NBTBinaryReader br = new NBTBinaryReader(newStream))
            {
                baseTag = TagBase.ReadNamedTag(br);
                br.Dispose();
            }
            return baseTag as TagCompound;

        }

        public static TagCompound FromFile(string filePath)
        {
            TagCompound compound;
            using (Stream s = File.OpenRead(filePath))
            {
                compound = FromStream(s);
            }
            return compound;
        }

        public static void ToFile(string path, TagCompound tag, bool compress = true)
        {
            using (MemoryStream inFile = new MemoryStream())
            {
                using (FileStream outPut = new FileStream(path, FileMode.Create))
                {
                    ToStream(inFile, tag, compress);
                    byte[] buf = inFile.ToArray();
                    outPut.Write(buf, 0, buf.Length);
                }
            }
        }

    }
}

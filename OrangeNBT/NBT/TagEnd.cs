using OrangeNBT.Helper;
using System.IO;
using OrangeNBT.NBT.IO;

namespace OrangeNBT.NBT
{
    public class TagEnd : TagBase
    {

        public override TagType TagType
        {
            get { return TagType.End; }
        }

        public TagEnd() : base(string.Empty) { }

        internal override void WriteBinary(NBTBinaryWriter bw) { }

        internal override void ReadBinary(NBTBinaryReader br) { }

        public override TagBase Clone()
        {
            return new TagEnd();
        }

    }
}

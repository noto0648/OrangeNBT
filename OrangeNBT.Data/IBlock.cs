using OrangeNBT.NBT;
namespace OrangeNBT.Data
{
    public interface IBlock
    {
        int Luminance { get; }

        int Opacity { get; }

        bool HasTileEntity { get; }

        TagCompound BuildTileEntity();
    }
}

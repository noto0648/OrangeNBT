
namespace OrangeNBT.NBT
{
    public interface ITagProvider<T>
    {
        T BuildTag();
    }
}

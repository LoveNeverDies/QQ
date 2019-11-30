
namespace Newbe.Mahua.Plugins.Parrot.Model.Base
{
    public interface IKey<out T>
    {
        T ID { get; }
    }
}

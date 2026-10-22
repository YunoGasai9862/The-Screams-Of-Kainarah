
namespace Assets.Scripts.Broadcaster.Interface
{
    public interface IBroadcaster
    {
        void Broadcast<T>(T value);
    }
}

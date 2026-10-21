
namespace Assets.Scripts.Broadcaster.Interface
{
    public interface IBroadcaster
    {
        void Broadcast<T>(T value, int retries = 5, int delay = 3);
    }
}

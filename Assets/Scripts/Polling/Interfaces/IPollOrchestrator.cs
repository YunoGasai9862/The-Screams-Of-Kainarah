
namespace Assets.Scripts.Polling.Interfaces
{
    public interface IPollOrchestrator
    {
        void RegisterPoller(IPoller poller);

        void DecommissionPoller(IPoller poller);
    }
}

using System.Collections;

namespace Assets.Scripts.Polling.Interfaces
{
    public interface IPoller
    {
        IEnumerator Poll(int pollingIntervalInSeconds);
    }
}

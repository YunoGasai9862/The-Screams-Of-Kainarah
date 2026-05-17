
using Assets.Scripts.Polling.Configuration;
using NUnit.Framework;
using System.Collections.Generic;

namespace Assets.Scripts.Polling.Interfaces
{
    public interface IPollOrchestrator
    {
         List<IPoller> BuildPollerRegistry(PollOrchestratorConfiguration pollOrchestratorConfiguration);

         void RegisterPoller(IPoller poller);

        void DecommissionPoller(IPoller poller);
    }
}

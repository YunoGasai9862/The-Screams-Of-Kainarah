
using Assets.Scripts.Polling.Configuration;
using NUnit.Framework;
using System.Collections.Generic;

namespace Assets.Scripts.Polling.Interfaces
{
    public interface IPollOrchestrator
    {
         Dictionary<IPoller, PollOrchestratorConfiguration.Orchestrator> BuildPollerRegistry(PollOrchestratorConfiguration pollOrchestratorConfiguration);

         void RegisterPoller(IPoller poller, PollOrchestratorConfiguration.Orchestrator orchestrator);

         void DecommissionPoller(IPoller poller);
    }
}

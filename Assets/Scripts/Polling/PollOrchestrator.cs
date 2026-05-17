using Assets.Scripts.Polling.Configuration;
using Assets.Scripts.Polling.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Polling.Configuration.PollOrchestratorConfiguration;

namespace Assets.Scripts.Polling
{
    public class PollOrchestrator : MonoBehaviour, IPollOrchestrator
    {
        List<IPoller> Pollers { get; set; } = new List<IPoller>();

        [SerializeField]
        public PollOrchestratorConfiguration pollOrchestratorConfiguration;

        public void DecommissionPoller(IPoller poller)
        {
            Pollers.Remove(poller);
        }

        public void RegisterPoller(IPoller poller)
        {
          

            Pollers.Add(poller);
        }

        public List<IPoller> BuildPollerRegistry(PollOrchestratorConfiguration pollOrchestratorConfiguration)
        {
            List<IPoller> pollers = new List<IPoller>();

            foreach (Orchestrator orchestrator in pollOrchestratorConfiguration.orchestrators)
            {

               if (!(orchestrator.registryObject is IPoller))
                {
                    Debug.Log($"The registry object {orchestrator.registryObject.name} does not implement the IPoller interface and cannot be registered to the PollOrchestrator.");
                    continue;
                }

               pollers.Add(orchestrator.registryObject as IPoller);
            }

            return pollers;
        }
    }
}

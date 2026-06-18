using Assets.Scripts.Polling.Configuration;
using Assets.Scripts.Polling.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Polling
{
    public class PollOrchestrator : Scene.MonoBehaviorScene, IPollOrchestrator
    {
        Dictionary<IPoller, PollOrchestratorConfiguration.Orchestrator> Pollers { get; set; } = new Dictionary<IPoller, PollOrchestratorConfiguration.Orchestrator>();

        [SerializeField]
        public PollOrchestratorConfiguration pollOrchestratorConfiguration;

        public void DecommissionPoller(IPoller poller)
        {
            if (poller == null)
            {
                Debug.Log("Cannot register a null poller to the PollOrchestrator!! - Please send a valid IPoller");
                return;
            }

            if (!Pollers.TryGetValue(poller, out PollOrchestratorConfiguration.Orchestrator orchestrator))
            {
                Debug.Log($"The poller {poller} is not registered - Please register the poller first!");
                return;
            }

            Pollers.Remove(poller);
        }

        public void RegisterPoller(IPoller poller, PollOrchestratorConfiguration.Orchestrator orchestrator)
        {
          
            if (poller == null)
            {
                Debug.Log("Cannot register a null poller to the PollOrchestrator!! - Please send a valid IPoller");
                return;
            }

            if (Pollers.TryGetValue(poller, out PollOrchestratorConfiguration.Orchestrator existingOrchestrator))
            {
                Debug.Log($"The poller {poller} is already registered to the PollOrchestrator!! - Duplicate entry will not be entertained!");
                return;
            }

            Pollers.TryAdd(poller, pollOrchestratorConfiguration.orchestrators.FirstOrDefault(o => o.registryObject == poller));
        }

        public Dictionary<IPoller, PollOrchestratorConfiguration.Orchestrator> BuildPollerRegistry(PollOrchestratorConfiguration pollOrchestratorConfiguration)
        {
            Dictionary<IPoller, PollOrchestratorConfiguration.Orchestrator> pollers = new Dictionary<IPoller, PollOrchestratorConfiguration.Orchestrator>();

            foreach (PollOrchestratorConfiguration.Orchestrator orchestrator in pollOrchestratorConfiguration.orchestrators)
            {

               if (!(orchestrator.registryObject is IPoller))
                {
                    Debug.Log($"The registry object {orchestrator.registryObject.name} does not implement the IPoller interface and cannot be registered to the PollOrchestrator.");
                    continue;
                }

               pollers.Add(orchestrator.registryObject as IPoller, orchestrator);
            }

            return pollers;
        }

        private async void Update()
        {
            foreach (KeyValuePair<IPoller, PollOrchestratorConfiguration.Orchestrator> poller in Pollers)
            {
                StartCoroutine(poller.Key.Poll(poller.Value.pollingIntervalInSeconds));
            }

            await Task.Delay(pollOrchestratorConfiguration.pollOrchestratorIntervalInSeconds * 1000);
        }
    }
}

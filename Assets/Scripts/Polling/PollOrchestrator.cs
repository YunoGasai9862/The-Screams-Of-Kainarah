using Assets.Scripts.Polling.Configuration;
using Assets.Scripts.Polling.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Polling
{
    public class PollOrchestrator : MonoBehaviour, IPollOrchestrator
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

            if (!Pollers.Contains(poller))
            {
                Debug.Log($"The poller {poller} is not registered - Please register the poller first!");
                return;
            }

            Pollers.Remove(poller);
        }

        public void RegisterPoller(IPoller poller)
        {
          
            if (poller == null)
            {
                Debug.Log("Cannot register a null poller to the PollOrchestrator!! - Please send a valid IPoller");
                return;
            }

            if (Pollers.Contains(poller))
            {
                Debug.Log($"The poller {poller} is already registered to the PollOrchestrator!! - Duplicate entry will not be entertained!");
                return;
            }

            Pollers.Add(poller);
        }

        public List<IPoller> BuildPollerRegistry(PollOrchestratorConfiguration pollOrchestratorConfiguration)
        {
            List<IPoller> pollers = new List<IPoller>();

            foreach (PollOrchestratorConfiguration.Orchestrator orchestrator in pollOrchestratorConfiguration.orchestrators)
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

        private async void Update()
        {
            foreach (IPoller poller in Pollers)
            {
                StartCoroutine(poller.Poll());

                //await Task.Delay(poller.)
            }
        }
    }
}

using Assets.Scripts.Polling.Configuration;
using Assets.Scripts.Polling.Interfaces;
using System.Collections.Generic;
using UnityEngine;

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
    }
}

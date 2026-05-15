using System;
using UnityEngine;

namespace Assets.Scripts.Polling.Configuration
{
    [CreateAssetMenu(fileName = "PollOrchestratorConfiguration", menuName = "Poll Orchestrator Configuration")]
    [Serializable]
    public class PollOrchestratorConfiguration: ScriptableObject
    {
        [Serializable]
        public class Orchestrator
        {
            [SerializeField]
            public string registryKey;

            [SerializeField]
            public float pollingIntervalInSecondds;
        }

        public Orchestrator[] orchestrators;   
    }
}

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
            public UnityEngine.Object registryObject;

            [SerializeField]
            public float pollingIntervalInSeconds;
        }

        public Orchestrator[] orchestrators;   
    }
}

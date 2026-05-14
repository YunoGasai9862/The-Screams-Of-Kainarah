using System;
using UnityEngine;

namespace Assets.Scripts.Polling.Configuration
{
    [CreateAssetMenu(fileName = "PollOrchestratorConfiguration", menuName = "Poll Orchestrator Configuration")]
    [Serializable]
    public class PollOrchestratorConfiguration: ScriptableObject
    {
        [SerializeField]
        public string RegistryKey;

        [SerializeField]
        public float PollingIntervalInSecondds;
    }
}

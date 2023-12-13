using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckpointsScriptableObject", menuName ="Checkpoints Scriptable Object")]
public class CheckPoints : ScriptableObject {

    [Serializable]
    public class Checkpoint
    {
        public GameObject checkpoint;
        public bool shouldResetPlayerAttributes;
        public bool shouldRespawn;
        [HideInInspector]
        public GameObject respawnCheckpoint;
        public bool finishLevelCheckpoint;

    }
    public Checkpoint[] checkpoints;
}
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckpointsScriptableObject", menuName ="Checkpoints Scriptable Object")]
public class CheckPoints : ScriptableObject {

    [Serializable]
    public class Checkpoint
    {
        public GameObject checkpoint;
        public bool shouldResetPlayerAttributes;
        public Checkpoint lastCheckPoint;
    }
    public Checkpoint[] checkpoints;
}
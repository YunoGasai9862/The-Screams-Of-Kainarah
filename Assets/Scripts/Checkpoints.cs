using Annotations.Enums;
using System;
using UnityEngine;
[Asset(Asset.SCRIPTABLE_OBJECT, "CheckPoints", InstantiationOrder = 3)]
[CreateAssetMenu(fileName = "CheckpointsScriptableObject", menuName = "Checkpoints Scriptable Object")]

public class CheckPoints : ScriptableObject {

    [Serializable]
    public class Checkpoint
    {
        public GameObject checkpoint;
        public bool shouldResetPlayerAttributes;
        public bool shouldRespawn;
        public bool finishLevelCheckpoint;

    }
    public Checkpoint[] checkpoints;
}
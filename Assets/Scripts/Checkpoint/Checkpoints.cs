using Annotations.Enums;
using System;
using UnityEngine;
[Asset(Asset.SCRIPTABLE_OBJECT, "CheckPoints", InstantiationOrder = 7)]
[CreateAssetMenu(fileName = "CheckpointsScriptableObject", menuName = "Checkpoints Scriptable Object")]

public class CheckPoints : ScriptableObject {

    [Serializable]
    public class Checkpoint
    {
        public Guid guid;
        public GameObject checkpoint;
        public bool shouldResetPlayerAttributes;
        public bool shouldRespawn;
        public bool finishLevelCheckpoint;

        public Checkpoint(Guid guid, GameObject checkpoint, bool shouldResetPlayerAttributes, bool shouldRespawn, bool finishLevelCheckpoint)
        {
            this.guid = guid;
            this.checkpoint = checkpoint;
            this.shouldResetPlayerAttributes = shouldResetPlayerAttributes;
            this.shouldRespawn = shouldRespawn;
            this.finishLevelCheckpoint = finishLevelCheckpoint;
        }
    }

    public Checkpoint[] checkpoints;
}
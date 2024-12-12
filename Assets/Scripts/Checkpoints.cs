using System;
using UnityEngine;
[Asset(AssetType = Asset.PRELOADING_SCRIPTABLE_OBJECT)]
[CreateAssetMenu(fileName = "CheckpointsScriptableObject", menuName ="Checkpoints Scriptable Object")]
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
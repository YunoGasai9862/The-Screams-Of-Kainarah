
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PickableItems", menuName = "Scriptable Pickable Items")]
public class PickableItems : ScriptableObject
{

    [Serializable]
    public class PickableEntities
    {
        public string objectName;
        public GameObject prefabToInstantiate;
        public bool shouldBeDisabledAfterSomeTime;
    }

    public PickableEntities[] pickableEntities;
}

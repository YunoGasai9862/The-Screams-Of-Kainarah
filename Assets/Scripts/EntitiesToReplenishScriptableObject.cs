using System;
using UnityEngine;

public class EntitiesToReplenishScriptableObject : ScriptableObject
{
    [Serializable]
    public class EntitiesToReset
    {
        public GameObject entity;
    }
}
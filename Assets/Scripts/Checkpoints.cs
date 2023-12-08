using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CheckPoints : ScriptableObject { 
    public class Checkpoint
    {
        public GameObject checkpoint;
        public bool shouldResetValues;
    }
}
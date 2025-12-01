using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ResetConfig", menuName = "Reset Config")]
public class ResetConfig: ScriptableObject
{
    [Serializable]
    public class Reset
    {
        public string key; 
        
        public AnimatorControllerParameterType type;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ResetConfig: ScriptableObject
{
    [Serializable]
    public class Reset
    {
        [SerializeField]
        public string key;

        [SerializeField]
        public AnimatorControllerParameterType type;
    }

    [SerializeField]
    public List<Reset> resetParameters;
}
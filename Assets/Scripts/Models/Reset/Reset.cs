
using System;
using UnityEngine;

[Serializable]
public class Reset
{
    [SerializeField]
    public string m_key;

    [SerializeField]
    public Value m_val;

    [Serializable]
    public class Value
    {
        [SerializeField]
        public AnimatorControllerParameterType m_type;

        [SerializeField]
        public string m_oldValue;

        [SerializeField]
        public string m_newValue;

        public override string ToString()
        {
            return $"Old Value :{m_oldValue}, NewValue: {m_newValue}";
        }
    }
}
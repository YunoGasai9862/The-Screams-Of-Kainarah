using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ResetConfig", menuName = "Reset Config")]
public class ResetSystem : ScriptableObject
{
    [SerializeField]
    public ResetState state;

    [SerializeField]
    public List<Reset> resetParameters;

    [Serializable]
    public enum ResetState
    {
        COMPLETE_RESET,

        PARTIAL_RESET,

        REVERT
    }

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
            public Field m_oldValue;

            [SerializeField]
            public Field m_newValue;

            public override string ToString()
            {
                return $"Old Value :{m_oldValue}, NewValue: {m_newValue}";
            }
        }

        [Serializable]
        public class Field
        {
            [SerializeField]
            public Type fieldType;

            [SerializeField]
            public string value;
        }
    }

    public override string ToString()
    {
        string result = "";

        resetParameters?.ForEach(val => result += $"ResetParametersKey : {val.m_key} - ResetParametersValue: {val.m_val.ToString()}\n");

        return $"result, ResetState: {state}";
    }
}
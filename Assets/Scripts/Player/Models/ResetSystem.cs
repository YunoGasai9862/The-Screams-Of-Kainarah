using System;
using System.Collections.Generic;
using UnityEngine;

public class ResetSystem
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

    public override string ToString()
    {
        string result = "";

        resetParameters?.ForEach(val => result += $"ResetParametersKey : {val.m_key} - ResetParametersValue: {val.m_val.ToString()}\n");

        return $"result, ResetState: {state}";
    }
}
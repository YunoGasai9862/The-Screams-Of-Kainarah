using System;
using System.Collections.Generic;
using UnityEngine;

public class ResetSystem
{
    [SerializeField]
    public ResetState State { get; set; }

    [SerializeField]
    public List<Reset> ResetParameters { get; set; }

    public override string ToString()
    {
        string result = "";

        ResetParameters?.ForEach(val => result += $"ResetParametersKey : {val.m_key} - ResetParametersValue: {val.m_val.ToString()}\n");

        return $"result, ResetState: {State}";
    }
}
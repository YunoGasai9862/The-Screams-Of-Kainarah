using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResetSystem
{
    public ResetState State { get; set; }
    public List<Reset> ResetParameters { get; set; } = new List<Reset>();

    public enum ResetState
    {
        COMPLETE_RESET,
        
        PARTIAL_RESET,

        REVERT
    }

    public class Reset
    {
        public string Key { get; set; }

        public Value Val { get; set; }

        public class Value
        {
            public AnimatorControllerParameterType Type { get; set; }

            public dynamic OldValue { get; set; }

            public dynamic NewValue { get; set; }

            public override string ToString()
            {
                return $"Old Value :{OldValue}, NewValue: {OldValue}";
            }
        }
    }

    public override string ToString()
    {
        string result = "";

        ResetParameters?.ForEach(val => result += $"ResetParametersKey : {val.Key} - ResetParametersValue: {val.Val.ToString()}\n");

        return $"result, ResetState: {State}";
    }
}

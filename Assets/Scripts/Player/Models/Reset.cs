using System.Collections.Generic;
using System.Linq;

public class Reset
{
    public ResetState State { get; set; }
    public  Dictionary<string, Value> ResetParameters { get; set; } = new Dictionary<string, Value>();

    public enum ResetState
    {
        COMPLETE_RESET,
        
        PARTIAL_RESET,

        REVERT
    }

    public class Value
    {
        public dynamic OldValue { get; set; }

        public dynamic NewValue { get; set; }

        public override string ToString()
        {
            return $"Old Value :{OldValue}, NewValue: {OldValue}";
        }
    }

    public override string ToString()
    {
        return $"ResetParametersKeys: {string.Join(",", ResetParameters?.Keys.ToArray())} ResetParametersValues: {ResetParameters?.Values}, ResetState: {State}";
    }
}

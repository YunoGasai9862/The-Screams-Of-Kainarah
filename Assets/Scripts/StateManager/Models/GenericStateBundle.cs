using System;

public class GenericStateBundle<StateBundleT>  where StateBundleT : IStateBundle
{
    public StateBundleT? StateBundle { get; set; }

    public override string ToString()
    {
        return $"StateBundle: {StateBundle}";
    }
} 
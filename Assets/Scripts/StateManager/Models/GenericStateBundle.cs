using System;

public class GenericStateBundle<StateBundleT>  where StateBundleT : IStateBundle
{
    public StateBundleT StateBundle { get; set; }

    public override string ToString()
    {
        return $"StateBundle: {StateBundle}";
    }
}

public class GenericStateBundle<StateBundleT, SubType> where StateBundleT : IStateBundle
{
    public StateBundleT StateBundle { get; set; }

    public SubType Type { get; set; }

    public override string ToString()
    {
        return $"StateBundle: {StateBundle} - Type: {Type}";
    }
}
using System;

public class GenericStateBundle
{
}

public class GenericStateBundle<StateBundleT> : GenericStateBundle where StateBundleT : IStateBundle
{
    public StateBundleT StateBundle { get; set; }

    public override string ToString()
    {
        return $"StateBundle: {StateBundle}";
    }
}

public class GenericStateBundle<StateBundleT, SubType> : GenericStateBundle where StateBundleT : IStateBundle
{
    public StateBundleT StateBundle { get; set; }

    public SubType Type { get; set; }

    public override string ToString()
    {
        return $"StateBundle: {StateBundle} - Type: {Type}";
    }
}
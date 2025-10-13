using System;

public class GenericStateBundle<StateBundleT, EnimationStateConsumerZ>  where StateBundleT : IStateBundle
                                                                        where EnimationStateConsumerZ : IEnimationState
{
    public StateBundleT? StateBundle { get; set; }

    public override string ToString()
    {
        return $"StateBundle: {StateBundle}";
    }
} 
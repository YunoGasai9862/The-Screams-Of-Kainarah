using System;

public class AnimationStateBundle<StateBundle, AnimationStateConsumerType> : GenericStateBundle<StateBundle> where StateBundle : IStateBundle
{
    public AnimationStateConsumerType? AnimationStateConsumer { get; set; }

    public override string ToString()
    {
        return $"AnimationStateConsumerType: {AnimationStateConsumer}";
    }
}